namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SimPlayersController
    {
        public CelestialBody CelestialBodyToProtect;
        public List<CelestialBody> CelestialBodies;
        public CommonStash CommonStash;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public StartingPathMenu StartingPathMenu;
        public OptionsPanel OptionsPanel;
        public PausePanel PausePanel;

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
        public event PausePlayerHandler PausePlayerMoved;
        public event PowerUpTypeSimPlayerHandler ActivatePowerUpAsked;
        public event PowerUpTypeSimPlayerHandler DesactivatePowerUpAsked;
        public event SimPlayerHandler PlayerConnected;
        public event SimPlayerHandler PlayerDisconnected;
        public event SimPlayerHandler ShowAdvancedViewAsked;
        public event SimPlayerHandler HideAdvancedViewAsked;
        public event PhysicalObjectHandler ObjectCreated;

        private Simulator Simulator;
        private Dictionary<Player, SimPlayer> Players;

        private SimPlayer PlayerInAdvancedView;
        private SimPlayer PlayerInNextWave;

        private bool UpdateSelection;


        public SimPlayersController(Simulator simulator)
        {
            Simulator = simulator;

            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);
            AvailablePowerUps = new Dictionary<PowerUpType, bool>(PowerUpTypeComparer.Default);

            Players = new Dictionary<Player, SimPlayer>();
        }


        public void Initialize()
        {
            InitializePowerUpsAndTurrets();

            CheckAvailablePowerUps();
            CheckAvailableTurrets();
            
            Players.Clear();

            PlayerInAdvancedView = null;
            PlayerInNextWave = null;

            UpdateSelection = true;

            PausePanel.CloseButtonHandler = DoPausePanelClosed;
            OptionsPanel.CloseButtonHandler = DoOptionsPanelClosed;
            PausePanel.SetClickHandler(DoPausePanelClicked);

            NotifyCommonStashChanged(CommonStash);
        }


        public void AddPlayer(Commander.Player player)
        {
            var simPlayer = new SimPlayer(Simulator, player)
            {
                CelestialBodies = CelestialBodies,
                CommonStash = CommonStash,
                Position = player.Position,
                Direction = new Vector3(0, -1, 0),
                AvailableTurrets = AvailableTurrets,
                AvailablePowerUps = AvailablePowerUps,
                Color = player.Color,
                ImageName = player.ImageName,
                UpdateSelectionz = UpdateSelection,
                BulletDamage = Simulator.Level.BulletHitPoints,
                PausePlayer = new PausePlayer(Simulator)
                {
                    OptionsPanel = OptionsPanel,
                    PausePanel = PausePanel
                }
            };

            simPlayer.Initialize();

            Players.Add(player, simPlayer);

            NotifyPlayerConnected(simPlayer);
        }


        public void RemovePlayer(Player player)
        {
            var simPlayer = Players[player];

            DoAdvancedViewAction(player, false);

            Players.Remove(player);

            NotifyPlayerDisconnected(simPlayer);
        }


        public bool HasPlayer(Player player)
        {
            return Players.ContainsKey(player);
        }


        public SimPlayer GetPlayer(Player player)
        {
            SimPlayer simPlayer = null;

            return Players.TryGetValue(player, out simPlayer) ? simPlayer : null;
        }


        public void SyncPausePlayers()
        {
            foreach (var p in Players.Values)
            {
                p.PausePlayer.NinjaPosition = p.Position;
                p.PausePlayer.Direction = p.Direction;
            }
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer p)
        {
            if (powerUp.Type == PowerUpType.FinalSolution)
                ((PowerUpLastSolution) powerUp).Selection = p.ActualSelection;

            p.UpdateSelection();
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

            p.UpdateSelection();
        }


        public void DoObjectCreated(IObjetPhysique obj)
        {
            var rgb = obj as RailGunBullet;

            if (rgb != null && rgb.Owner != null)
            {
                DoPowerUpUse(rgb.Owner);
                return;
            }
        }


        public void DoObjectHit(IObjetPhysique obj, IObjetPhysique by)
        {
            Mineral mineral = obj as Mineral;

            if (mineral != null)
            {
                if (mineral.Type == MineralType.Life1)
                {
                    CommonStash.Lives += mineral.Value;
                    CelestialBodyToProtect.LifePoints += mineral.Value;
                }
                else if (!Simulator.EditorMode || Simulator.EditorState == EditorState.Playtest)
                {
                    CommonStash.Cash += mineral.Value;
                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }
        }


        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            Enemy ennemi = obj as Enemy;

            if (ennemi != null)
            {
                if ( Simulator.State == GameState.Running && (!Simulator.EditorMode || Simulator.EditorState == EditorState.Playtest))
                    CommonStash.Cash += ennemi.CashValue;

                if (Simulator.State == GameState.Running)
                {
                    CommonStash.Score += ennemi.PointsValue;
                    CommonStash.TotalScore += ennemi.PointsValue;

                    foreach (var player in Players.Values)
                        player.UpdateSelection();

                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }


            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                foreach (var player in Players.Values)
                {
                    player.DoCelestialBodyDestroyed();
                    player.UpdateSelection();
                }
                return;
            }
        }


        public void DoMoveDelta(Player p, ref Vector3 delta)
        {
            SimPlayer player = Players[p];

            if (Simulator.State == GameState.Paused || OptionsPanel.Visible)
            {
                player.PausePlayer.Move(ref delta, MouseConfiguration.MovingSpeed);

                return;
            }


            if (Simulator.DemoMode)
            {
                player.Move(ref delta, MouseConfiguration.MovingSpeed);
                return;
            }


            player.Move(ref delta, MouseConfiguration.MovingSpeed);

            if (player.ActualSelection.TurretToPlace != null &&
                player.ActualSelection.CelestialBody.OuterTurretZone.Outside(player.Position))
                player.Position = player.ActualSelection.CelestialBody.OuterTurretZone.NearestPointToCircumference(player.Position);
        }


        public void DoDirectionDelta(Player p, ref Vector3 delta)
        {
            SimPlayer player = Players[p];

            if (Simulator.State == GameState.Paused || OptionsPanel.Visible)
            {
                player.PausePlayer.Rotate(ref delta, MouseConfiguration.RotatingSpeed);

                return;
            }

            player.Rotate(ref delta, MouseConfiguration.RotatingSpeed);
        }


        public void DoPausedGameChoice(Player p, int delta)
        {
            var player = Players[p];

            if (player.ActualSelection.CelestialBody == null)
                return;

            if (delta > 0)
                player.NextPausedGameChoice();
            else
                player.PreviousPausedGameChoice();

            return;
        }


        public void DoNewGameChoice(Player p, int delta)
        {
            var player = Players[p];

            if (player.ActualSelection.CelestialBody == null)
                return;

            if (delta > 0)
                player.NextNewGameChoice();
            else
                player.PreviousNewGameChoice();

            return;
        }


        public void DoNewGameState(GameState state)
        {
            if (state == GameState.Won || state == GameState.Lost)
            {
                foreach (var p in Players.Values)
                {
                    p.GameOver = true;
                    p.UpdateSelection();

                    NotifyPlayerChanged(p);
                }
            }
        }


        public void DoTurretBought(Turret turret, SimPlayer player)
        {
            CommonStash.Cash -= turret.BuyPrice;
            NotifyCommonStashChanged(CommonStash);

            foreach (var p in Players.Values)
                p.UpdateSelection();

            if (turret.Type == TurretType.Gravitational && !Simulator.DemoMode)
                Audio.PlaySfx(@"sfxTourelleGravitationnelleAchetee");
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            CommonStash.Cash += turret.SellPrice;
            NotifyCommonStashChanged(CommonStash);

            foreach (var p in Players.Values)
                p.UpdateSelection();

            if (turret.Type == TurretType.Gravitational)
                Audio.PlaySfx(@"sfxTourelleGravitationnelleAchetee");
            else
                Audio.PlaySfx(@"sfxTourelleVendue");
        }


        public void DoTurretUpdated(Turret turret, SimPlayer player)
        {
            CommonStash.Cash -= turret.BuyPrice; //parce qu'effectue une fois la tourelle mise a jour
            NotifyCommonStashChanged(CommonStash);

            foreach (var p in Players.Values)
                p.UpdateSelection();
        }


        public void DoTurretReactivated(Turret turret)
        {
            foreach (var player in Players.Values)
                player.UpdateSelection();
        }


        public CelestialBody GetSelectedCelestialBody(Player p)
        {
            return Players.ContainsKey(p) ? Players[p].ActualSelection.CelestialBody : null;
        }


        public void Update()
        {
            if (Simulator.State == GameState.Paused || OptionsPanel.Visible)
            {
                foreach (var player in Players.Values)
                {
                    player.PausePlayer.Update();

                    NotifyPausePlayerMoved(player.PausePlayer);
                }

                return;
            }


            if (Simulator.State == GameState.Running)
            {
                CheckAvailablePowerUps();
                CheckAvailableTurrets();

                foreach (var player in Players.Values)
                {
                    player.Update();

                    if (player.BouncingThisTick)
                    {
                        Inputs.VibrateController(player.BasePlayer, 150f, 0.5f, 0.5f);
                    }

                    if (player.Firing)
                    {
                        //Inputs.VibrateController(player.Player, Preferences.TargetElapsedTimeMs * 2, 0.2f, 0f); TODO: vibrate with little pauses between

                        foreach (var b in player.SpaceshipMove.BulletsThisTick())
                            NotifyObjectCreated(b);

                        player.Firing = false;
                    }

                    NotifyPlayerChanged(player);
                    NotifyPlayerMoved(player);
                }
            }
        }


        public void Draw()
        {
            foreach (var player in Players.Values)
                if (player.ActualSelection.TurretToPlace != null)
                    player.ActualSelection.TurretToPlace.Draw();
        }


        public void DoNextOrPreviousAction(Player p, int delta)
        {
            var player = Players[p];

            // turret's options
            if (player.ActualSelection.Turret != null)
            {
                if (delta > 0)
                    player.NextTurretOption();
                else
                    player.PreviousTurretOption();

                return;
            }


            // shop turrets
            else if (player.ActualSelection.CelestialBody != null &&
                player.ActualSelection.Turret == null)
            {
                if (delta > 0)
                    player.NextTurretToBuy();
                else
                    player.PreviousTurretToBuy();

                return;
            }
        }


        public void Fire(Player p)
        {
            var player = Players[p];

            if (Simulator.DemoMode)
                Players[p].Firing = true;

            else if (player.ActualSelection.PowerUpToBuy == PowerUpType.None &&
                    player.ActualSelection.TurretToBuy == TurretType.None &&
                    player.ActualSelection.TurretToPlace == null)
                Players[p].Firing = true;
        }


        public void StopFire(Player p)
        {
            Players[p].Firing = false;
        }


        public void DoPanelAction(Player pl)
        {
            var player = Players[pl].PausePlayer;

            if (OptionsPanel.Visible)
                OptionsPanel.DoClick(player.Circle);
            else if (PausePanel.Visible)
                PausePanel.DoClick(player.Circle);
        }


        public void DoSelectAction(Player pl)
        {
            var player = Players[pl];

            // activate a power-up
            if (player.ActualSelection.PowerUpToBuy != PowerUpType.None &&
                AvailablePowerUps[player.ActualSelection.PowerUpToBuy])
            {
                PowerUp p = Simulator.PowerUpsFactory.Availables[player.ActualSelection.PowerUpToBuy];

                NotifyActivatePowerUpAsked(player.ActualSelection.PowerUpToBuy, player);

                if (p.PayOnActivation)
                {
                    CommonStash.Cash -= p.BuyPrice;
                    NotifyCommonStashChanged(CommonStash);
                }

                player.UpdateSelection();

                return;
            }


            // buy a turret
            if (player.ActualSelection.TurretToBuy != TurretType.None)
            {
                player.ActualSelection.TurretToPlace = Simulator.TurretsFactory.Create(player.ActualSelection.TurretToBuy);
                player.ActualSelection.TurretToPlace.CelestialBody = player.ActualSelection.CelestialBody;
                player.ActualSelection.TurretToPlace.Position = player.Position;
                player.ActualSelection.TurretToPlace.ToPlaceMode = true;
                player.TurretToPlaceChanged = true;
                player.UpdateSelection();
                NotifyTurretToPlaceSelected(player.ActualSelection.TurretToPlace, player);

                return;
            }

            // place a turret
            if (player.ActualSelection.TurretToPlace != null &&
                player.ActualSelection.TurretToPlace.CanPlace)
            {
                player.ActualSelection.TurretToPlace.SetCanPlaceColor();
                player.ActualSelection.TurretToPlace.ToPlaceMode = false;
                NotifyBuyTurretAsked(player.ActualSelection.TurretToPlace, player);
                NotifyTurretToPlaceDeselected(player.ActualSelection.TurretToPlace, player);
                player.ActualSelection.TurretToPlace = null;
                player.TurretToPlaceChanged = true;
                player.UpdateSelection();
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
                        if (!player.ActualSelection.AvailableTurretOptions[TurretChoice.Update])
                        {
                            // cannot upgrade here (sound, shake that menu, etc.)
                        }

                        else
                        {
                            NotifyUpgradeTurretAsked(player.ActualSelection.Turret, player);
                        }
                        break;
                }

                player.UpdateSelection();

                return;
            }


            // call next wave
            if (player.PowerUpInUse == PowerUpType.None && StartingPathMenu.CheckedIn == player)
            {
                NotifyNextWaveAsked();
                return;
            }
        }


        public void DoAlternateAction(Player p)
        {

        }


        public void DoCancelAction(Player p)
        {
            var player = Players[p];

            if (player.ActualSelection.TurretToPlace == null)
                return;

            NotifyTurretToPlaceDeselected(player.ActualSelection.TurretToPlace, player);
            player.ActualSelection.TurretToPlace = null;
            player.UpdateSelection();
        }


        public void DoAdvancedViewAction(Player p, bool pressed)
        {
            var player = Players[p];

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

                foreach (var pl in Players.Values)
                    pl.Initialize();

                return;
            }


            if (command.Type == EditorCommandType.Panel)
            {
                if (((EditorPanelCommand) command).Show)
                {
                    foreach (var player in Players.Values)
                    {
                        player.ActualSelection.CelestialBody = null;
                        player.ActualSelection.Turret = null;
                        player.ActualSelection.TurretToPlace = null;

                        NotifyPlayerChanged(player);
                    }

                    UpdateSelection = false;

                    foreach (var player in Players.Values)
                        player.UpdateSelectionz = false;
                }

                else
                {
                    UpdateSelection = true;

                    foreach (var player in Players.Values)
                        player.UpdateSelectionz = true;
                }

                return;
            }
        }


        private void InitializePowerUpsAndTurrets()
        {
            AvailablePowerUps.Clear();
            AvailableTurrets.Clear();

            foreach (var turret in Simulator.TurretsFactory.Availables.Keys)
                AvailableTurrets.Add(turret, false);

            foreach (var powerUp in Simulator.PowerUpsFactory.Availables.Keys)
                AvailablePowerUps.Add(powerUp, false);
        }


        private void DoPowerUpUse(SimPlayer player)
        {
            if (player.PowerUpInUse == PowerUpType.None)
                return;

            PowerUp p = Simulator.PowerUpsFactory.Availables[player.PowerUpInUse];

            if (!p.PayOnUse)
                return;

            CommonStash.Cash -= p.UsePrice;
            NotifyCommonStashChanged(CommonStash);

            player.UpdateSelection();

            if (CommonStash.Cash < p.UsePrice)
                NotifyDesactivatePowerUpAsked(player.PowerUpInUse, player);
        }


        private void DoPausePanelClosed(PanelWidget widget)
        {
            Simulator.TriggerNewGameState(GameState.Running);
        }
        

        private void DoOptionsPanelClosed(PanelWidget widget)
        {
            if (!Simulator.DemoMode)
                Simulator.ShowPausedGamePanel();
            else
                Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoPausePanelClicked(PanelWidget widget)
        {
            if (widget.Name == "Help")
            {

            }

            else if (widget.Name == "Controls")
            {

            }

            else if (widget.Name == "Restart")
            {
                Simulator.TriggerNewGameState(GameState.Restart);
            }

            else if (widget.Name == "Options")
            {
                Simulator.ShowOptionsPanel(false);
            }

            else if (widget.Name == "GoBackToWorld")
            {
                Simulator.TriggerNewGameState(GameState.PausedToWorld);
            }

            else if (widget.Name == "Resume")
            {
                Simulator.TriggerNewGameState(GameState.Running);
            }
        }


        private void CheckAvailablePowerUps()
        {
            foreach (var powerUp in Simulator.PowerUpsFactory.Availables.Values)
                AvailablePowerUps[powerUp.Type] =
                    powerUp.BuyPrice <= CommonStash.Cash &&
                    powerUp.UsePrice <= CommonStash.Cash &&
                    ActivesPowerUps[powerUp.Type];
        }


        private void CheckAvailableTurrets()
        {
            foreach (var turret in Simulator.TurretsFactory.Availables.Values)
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


        private void NotifyPausePlayerMoved(PausePlayer player)
        {
            if (PausePlayerMoved != null)
                PausePlayerMoved(player);
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


        private void NotifyObjectCreated(IObjetPhysique objet)
        {
            if (ObjectCreated != null)
                ObjectCreated(objet);
        }
    }
}
