namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SimPlayersController
    {
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public List<Wave> ActiveWaves;

        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;

        public event TurretSimPlayerHandler BuyTurretAsked;
        public event TurretSimPlayerHandler SellTurretAsked;
        public event TurretSimPlayerHandler UpgradeTurretAsked;
        public event TurretSimPlayerHandler TurretToPlaceSelected;
        public event TurretSimPlayerHandler TurretToPlaceDeselected;
        public event NoneHandler NextWaveAsked;
        public event CommonStashHandler CommonStashChanged;
        public event SimPlayerHandler PlayerSelectionChanged;
        public event SimPlayerHandler PlayerMoved;
        public event SimPlayerHandler PlayerFired;
        public event SimPlayerHandler PlayerActionRefused;
        public event PowerUpTypeSimPlayerHandler ActivatePowerUpAsked;
        public event PowerUpTypeSimPlayerHandler DesactivatePowerUpAsked;
        public event SimPlayerHandler PlayerConnected;
        public event SimPlayerHandler PlayerDisconnected;
        public event SimPlayerHandler ShowAdvancedViewAsked;
        public event SimPlayerHandler HideAdvancedViewAsked;
        public event PhysicalObjectHandler ObjectCreated;
        public event SimPlayerDirectionHandler PlayerBounced;
        public event SimPlayerHandler PlayerRotated;

        private Simulator Simulator;

        private SimPlayer PlayerInAdvancedView;

        private CelestialBody CelestialBodyToProtect { get { return Simulator.Data.Level.CelestialBodyToProtect; } }
        private List<CelestialBody> CelestialBodies { get { return Simulator.Data.Level.PlanetarySystem; } }
        private CommonStash CommonStash { get { return Simulator.Data.Level.CommonStash; } }


        public SimPlayersController(Simulator simulator)
        {
            Simulator = simulator;

            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);
            AvailablePowerUps = new Dictionary<PowerUpType, bool>(PowerUpTypeComparer.Default);
        }


        public void Initialize()
        {
            InitializePowerUpsAndTurrets();

            CheckAvailablePowerUps();
            CheckAvailableTurrets();

            PlayerInAdvancedView = null;

            NotifyCommonStashChanged(CommonStash);
        }


        public void AddPlayer(Commander.Player player)
        {
            var simPlayer = new SimPlayer(Simulator, player)
            {
                Position = CommonStash.StartingPosition + player.SpawningPosition,
                Direction = new Vector3(0, -1, 0),
                AvailableTurrets = AvailableTurrets,
                AvailablePowerUps = AvailablePowerUps,
                Color = player.Color,
                ImageName = player.ImageName,
                BulletAttackPoints = (float) Simulator.Data.Level.BulletDamage,
                BouncedHandler = DoPlayerBounced,
                RotatedHandler = DoPlayerRotated
            };

            simPlayer.Initialize();

            Simulator.Data.Players.Add(player, simPlayer);

            NotifyPlayerConnected(simPlayer);
        }


        public void RemovePlayer(Commander.Player player)
        {
            var simPlayer = Simulator.Data.Players[player];

            DoAdvancedViewAction(player, false);
            DoCancelAction(player);

            simPlayer.DoDisconnect();

            Simulator.Data.Players.Remove(player);

            NotifyPlayerDisconnected(simPlayer);
        }


        public void MovePlayers(Vector3 basePosition)
        {
            foreach (var p in Simulator.Data.Players)
                p.Value.Position = basePosition + ((Commander.Player) p.Key).SpawningPosition;
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer p)
        {
            if (powerUp.Type == PowerUpType.FinalSolution)
                ((PowerUpLastSolution) powerUp).Selection = p.ActualSelection;
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer p)
        {
            if (powerUp.NeedInput)
            {
                p.Position = powerUp.Position;
                NotifyPlayerMoved(p);
            }

            if (powerUp.Type == PowerUpType.FinalSolution && ((PowerUpLastSolution) powerUp).GoAhead)
                DoPowerUpUse(p);
        }


        public void DoObjectCreated(ICollidable obj)
        {
            var rgb = obj as RailGunBullet;

            if (rgb != null && rgb.OwnerPlayer != null)
            {
                DoPowerUpUse(rgb.OwnerPlayer);
                return;
            }
        }


        public void DoObjectHit(ICollidable obj, ICollidable by)
        {
            Mineral mineral = obj as Mineral;

            if (mineral != null)
            {
                if (mineral.Type == MineralType.Life1)
                {
                    CommonStash.Lives += mineral.Value;
                    CelestialBodyToProtect.LifePoints += mineral.Value;
                }
                else if (!Simulator.EditorMode || Simulator.EditorPlaytestingMode)
                {
                    CommonStash.Cash += mineral.Value;
                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }
        }


        public void DoObjectDestroyed(ICollidable obj)
        {
            Enemy enemy = obj as Enemy;

            if (enemy != null)
            {
                if (Simulator.State == GameState.Running && (!Simulator.EditorMode || Simulator.EditorPlaytestingMode))
                    CommonStash.Cash += enemy.CashValue;

                if (Simulator.State == GameState.Running)
                {
                    CommonStash.Score += enemy.PointsValue;
                    CommonStash.TotalScore += enemy.PointsValue;

                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }


            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                foreach (var player in Simulator.Data.Players.Values)
                    player.DoCelestialBodyDestroyed();

                return;
            }
        }


        public void DoMoveDelta(Commander.Player p, ref Vector3 delta)
        {
            SimPlayer player = Simulator.Data.Players[p];

            player.Move(ref delta, ((Commander.Player) p).MovingSpeed);

            if (Simulator.DemoMode)
                return;

            if (player.ActualSelection.TurretToPlace != null &&
                player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.Outside(player.Position))
                player.Position = player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.NearestPointToCircumference(player.Position);
        }


        public void DoDirectionDelta(Commander.Player p, ref Vector3 delta)
        {
            SimPlayer player = Simulator.Data.Players[p];

            player.Rotate(ref delta, ((Commander.Player) p).RotatingSpeed);
        }


        public void DoToggleChoice(Commander.Player p, int delta)
        {
            var player = Simulator.Data.Players[p];

            if (delta > 0)
                player.NextContextualMenuSelection();
            else
                player.PreviousContextualMenuSelection();
        }


        public void DoNewGameState(GameState state)
        {
            if (state == GameState.Won || state == GameState.Lost)
                foreach (var p in Simulator.Data.Players)
                {
                    DoCancelAction(p.Key);
                    p.Value.ActualSelection.Initialize();
                }
        }


        public void DoTurretBought(Turret turret, SimPlayer player)
        {
            CommonStash.Cash -= turret.BuyPrice;
            NotifyCommonStashChanged(CommonStash);
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            CommonStash.Cash += turret.SellPrice;
            NotifyCommonStashChanged(CommonStash);
        }


        public void DoTurretUpgraded(Turret turret, SimPlayer player)
        {
            CommonStash.Cash -= turret.BuyPrice; //parce qu'effectue une fois la tourelle mise a jour
            NotifyCommonStashChanged(CommonStash);
        }


        public void DoTurretReactivated(Turret turret)
        {

        }


        public void Update()
        {
            if (Simulator.State == GameState.Running)
            {
                CheckAvailablePowerUps();
                CheckAvailableTurrets();
            }


            foreach (var player in Simulator.Data.Players.Values)
            {
                player.Update();

                if (player.Firing)
                {
                    var bullets = player.SpaceshipMove.Fire();

                    if (bullets.Count != 0)
                        NotifyPlayerFired(player);

                    foreach (var b in bullets)
                        NotifyObjectCreated(b);

                    if (player.InnerPlayer.InputType == InputType.Gamepad)
                        StopFire(player.InnerPlayer);
                }

                NotifyPlayerChanged(player);
                NotifyPlayerMoved(player);

                player.ActualSelection.Update();
            }
        }


        public void Draw()
        {
            foreach (var player in Simulator.Data.Players.Values)
                if (player.ActualSelection.TurretToPlace != null)
                    player.ActualSelection.TurretToPlace.Draw();
        }


        public void Fire(Commander.Player p)
        {
            var player = Simulator.Data.Players[p];

            if (Simulator.DemoMode)
                Simulator.Data.Players[p].Firing = true;

            else if (player.ActualSelection.TurretToPlace == null)
                Simulator.Data.Players[p].Firing = true;
        }


        public void StopFire(Commander.Player p)
        {
            Simulator.Data.Players[p].Firing = false;
        }


        public void DoSelectAction(Commander.Player pl)
        {
            var player = Simulator.Data.Players[pl];

            // activate a power-up
            if (player.ActualSelection.PowerUpToBuy != PowerUpType.None)
            {
                if (!AvailablePowerUps[player.ActualSelection.PowerUpToBuy])
                {
                    NotifyPlayerActionRefused(player);
                }

                else
                {
                    PowerUp p = Simulator.Data.Level.AvailablePowerUps[player.ActualSelection.PowerUpToBuy];

                    NotifyActivatePowerUpAsked(player.ActualSelection.PowerUpToBuy, player);

                    if (p.PayOnActivation)
                    {
                        CommonStash.Cash -= p.BuyPrice;
                        NotifyCommonStashChanged(CommonStash);
                    }
                }

                return;
            }


            // buy a turret
            if (player.ActualSelection.TurretToBuy != TurretType.None)
            {
                if (!AvailableTurrets[player.ActualSelection.TurretToBuy])
                {
                    NotifyPlayerActionRefused(player);
                }

                else
                {
                    player.ActualSelection.TurretToPlace = Simulator.TurretsFactory.Create(player.ActualSelection.TurretToBuy);
                    player.ActualSelection.TurretToPlace.CelestialBody = player.ActualSelection.CelestialBody;
                    player.ActualSelection.TurretToPlace.Position = player.Position;
                    player.ActualSelection.TurretToPlace.ToPlaceMode = true;

                    NotifyTurretToPlaceSelected(player.ActualSelection.TurretToPlace, player);
                }

                return;
            }

            // place a turret
            if (player.ActualSelection.TurretToPlace != null)
            {
                if (!player.ActualSelection.TurretToPlace.CanPlace)
                {
                    NotifyPlayerActionRefused(player);
                }

                else
                {
                    player.ActualSelection.TurretToPlace.SetCanPlaceColor();
                    player.ActualSelection.TurretToPlace.ToPlaceMode = false;
                    NotifyBuyTurretAsked(player.ActualSelection.TurretToPlace, player);
                    NotifyTurretToPlaceDeselected(player.ActualSelection.TurretToPlace, player);
                    player.ActualSelection.Turret = player.ActualSelection.TurretToPlace;
                    player.ActualSelection.TurretToPlace = null;
                }

                return;
            }


            // upgrade a turret
            if (player.ActualSelection.Turret != null && !player.ActualSelection.Turret.Disabled)
            {
                switch (player.ActualSelection.TurretChoice)
                {
                    case TurretChoice.Sell:
                        NotifySellTurretAsked(player.ActualSelection.Turret, player);
                        break;
                    case TurretChoice.Update:
                        if (!player.AvailableTurretOptions[TurretChoice.Update])
                        {
                            NotifyPlayerActionRefused(player);
                        }

                        else
                        {
                            NotifyUpgradeTurretAsked(player.ActualSelection.Turret, player);
                        }
                        break;
                }

                return;
            }


            // call next wave //todo
            //if (player.PowerUpInUse == PowerUpType.None && StartingPathMenu.CheckedIn == player && ActiveWaves.Count < 3)
            //{
            //    NotifyNextWaveAsked();
            //    return;
            //}
        }


        public void DoAlternateAction(Commander.Player p)
        {

        }


        public void DoCancelAction(Commander.Player p)
        {
            var player = Simulator.Data.Players[p];

            if (player.ActualSelection.TurretToPlace == null)
                return;

            NotifyTurretToPlaceDeselected(player.ActualSelection.TurretToPlace, player);
            player.ActualSelection.TurretToPlace = null;
        }


        public void DoAdvancedViewAction(Commander.Player p, bool pressed)
        {
            var player = Simulator.Data.Players[p];

            if (PlayerInAdvancedView == null && pressed)
            {
                PlayerInAdvancedView = player;
                NotifyShowAdvancedViewAsked(player);
                return;
            }

            if (PlayerInAdvancedView == player && !pressed)
            {
                PlayerInAdvancedView = null;
                NotifyHideAdvancedViewAsked(player);
                return;
            }
        }


        public void DoEditorCommandExecuted(EditorCommand command)
        {
            if (command.Name == "AddOrRemovePowerUp")
            {
                InitializePowerUpsAndTurrets();
                CheckAvailablePowerUps();

                //foreach (var pl in Simulator.Data.Players.Values)
                //    pl.Initialize(); todo

                return;
            }
        }


        public void DoPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            p1.SpaceshipMove.SteeringBehavior.Bouncing -= p1.SpaceshipMove.SteeringBehavior.Acceleration * p1.SpaceshipMove.SteeringBehavior.Speed;
            p2.SpaceshipMove.SteeringBehavior.Bouncing += p1.SpaceshipMove.SteeringBehavior.Acceleration * p1.SpaceshipMove.SteeringBehavior.Speed;

            p1.SpaceshipMove.SteeringBehavior.Bouncing += p2.SpaceshipMove.SteeringBehavior.Acceleration * p1.SpaceshipMove.SteeringBehavior.Speed;
            p2.SpaceshipMove.SteeringBehavior.Bouncing -= p2.SpaceshipMove.SteeringBehavior.Acceleration * p1.SpaceshipMove.SteeringBehavior.Speed;

            p1.SpaceshipMove.SteeringBehavior.Acceleration = Vector3.Zero;
            p2.SpaceshipMove.SteeringBehavior.Acceleration = Vector3.Zero;

            Inputs.VibrateControllerLowFrequency(p1.InnerPlayer, 120, 0.8f);
            Inputs.VibrateControllerLowFrequency(p2.InnerPlayer, 120, 0.8f);
        }


        public void DoPlayerBounced(SimPlayer player, Direction d)
        {
            Core.Input.Inputs.VibrateControllerLowFrequency(player.InnerPlayer, 150, 0.7f);

            NotifyPlayerBounced(player, d);
        }


        public void DoPlayerRotated(SimPlayer player)
        {
            NotifyPlayerRotated(player);
        }


        private void InitializePowerUpsAndTurrets()
        {
            AvailablePowerUps.Clear();
            AvailableTurrets.Clear();

            foreach (var turret in Simulator.Data.Level.AvailableTurrets.Keys)
                AvailableTurrets.Add(turret, false);

            foreach (var powerUp in Simulator.Data.Level.AvailablePowerUps.Keys)
                AvailablePowerUps.Add(powerUp, false);
        }


        private void DoPowerUpUse(SimPlayer player)
        {
            if (player.PowerUpInUse == PowerUpType.None)
                return;

            PowerUp p = Simulator.Data.Level.AvailablePowerUps[player.PowerUpInUse];

            if (!p.PayOnUse)
                return;

            CommonStash.Cash -= p.UsePrice;
            NotifyCommonStashChanged(CommonStash);

            if (CommonStash.Cash < p.UsePrice)
                NotifyDesactivatePowerUpAsked(player.PowerUpInUse, player);
        }


        private void CheckAvailablePowerUps()
        {
            foreach (var powerUp in Simulator.Data.Level.AvailablePowerUps.Values)
                AvailablePowerUps[powerUp.Type] =
                    powerUp.BuyPrice <= CommonStash.Cash &&
                    powerUp.UsePrice <= CommonStash.Cash &&
                    ActivesPowerUps[powerUp.Type];
        }


        private void CheckAvailableTurrets()
        {
            foreach (var turret in Simulator.Data.Level.AvailableTurrets.Values)
                AvailableTurrets[turret.Type] = turret.BuyPrice <= CommonStash.Cash;
        }


        private void NotifyTurretToPlaceSelected(Turret turret, SimPlayer player)
        {
            if (TurretToPlaceSelected != null)
                TurretToPlaceSelected(turret, player);
        }


        private void NotifyTurretToPlaceDeselected(Turret turret, SimPlayer player)
        {
            if (TurretToPlaceDeselected != null)
                TurretToPlaceDeselected(turret, player);
        }


        private void NotifyActivatePowerUpAsked(PowerUpType type, SimPlayer player)
        {
            if (ActivatePowerUpAsked != null)
                ActivatePowerUpAsked(type, player);
        }


        private void NotifyDesactivatePowerUpAsked(PowerUpType type, SimPlayer player)
        {
            if (DesactivatePowerUpAsked != null)
                DesactivatePowerUpAsked(type, player);
        }


        private void NotifyCommonStashChanged(CommonStash stash)
        {
            if (CommonStashChanged != null)
                CommonStashChanged(stash);
        }


        private void NotifyBuyTurretAsked(Turret turret, SimPlayer player)
        {
            if (BuyTurretAsked != null)
                BuyTurretAsked(turret, player);
        }


        private void NotifySellTurretAsked(Turret turret, SimPlayer player)
        {
            if (SellTurretAsked != null)
                SellTurretAsked(turret, player);
        }


        private void NotifyUpgradeTurretAsked(Turret turret, SimPlayer player)
        {
            if (UpgradeTurretAsked != null)
                UpgradeTurretAsked(turret, player);
        }


        private void NotifyNextWaveAsked()
        {
            if (NextWaveAsked != null)
                NextWaveAsked();
        }


        private void NotifyPlayerChanged(SimPlayer player)
        {
            if (PlayerSelectionChanged != null)
                PlayerSelectionChanged(player);
        }


        private void NotifyPlayerMoved(SimPlayer player)
        {
            if (PlayerMoved != null)
                PlayerMoved(player);
        }


        private void NotifyPlayerConnected(SimPlayer player)
        {
            if (PlayerConnected != null)
                PlayerConnected(player);
        }


        private void NotifyPlayerDisconnected(SimPlayer player)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(player);
        }


        private void NotifyShowAdvancedViewAsked(SimPlayer player)
        {
            if (ShowAdvancedViewAsked != null)
                ShowAdvancedViewAsked(player);
        }


        private void NotifyHideAdvancedViewAsked(SimPlayer player)
        {
            if (HideAdvancedViewAsked != null)
                HideAdvancedViewAsked(player);
        }


        private void NotifyObjectCreated(ICollidable objet)
        {
            if (ObjectCreated != null)
                ObjectCreated(objet);
        }


        private void NotifyPlayerBounced(SimPlayer player, Direction d)
        {
            if (PlayerBounced != null)
                PlayerBounced(player, d);
        }


        private void NotifyPlayerRotated(SimPlayer player)
        {
            if (PlayerRotated != null)
                PlayerRotated(player);
        }


        private void NotifyPlayerFired(SimPlayer player)
        {
            if (PlayerFired != null)
                PlayerFired(player);
        }


        private void NotifyPlayerActionRefused(SimPlayer player)
        {
            if (PlayerActionRefused != null)
                PlayerActionRefused(player);
        }
    }
}
