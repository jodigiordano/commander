namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class SimPlayersController
    {
        public CelestialBody CelestialBodyToProtect;
        public List<CelestialBody> CelestialBodies;
        public CommonStash CommonStash;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public SandGlass SandGlass;
        public Vector3 InitialPlayerPosition;


        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;


        //todo
        public CelestialBody SelectedCelestialBody { get { return Player.ActualSelection.CelestialBody; } }


        private Simulator Simulation;
        private SimPlayer Player;


        public SimPlayersController(Simulator simulation)
        {
            Simulation = simulation;

            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);
            AvailablePowerUps = new Dictionary<PowerUpType, bool>(PowerUpTypeComparer.Default);
        }


        public void Initialize()
        {
            foreach (var turret in Simulation.TurretsFactory.Availables.Keys)
                AvailableTurrets.Add(turret, false);

            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Keys)
                AvailablePowerUps.Add(powerUp, false);

            Player = new SimPlayer(Simulation)
            {
                CelestialBodies = CelestialBodies,
                ActivesPowerUps = ActivesPowerUps,
                CommonStash = CommonStash,
                Position = InitialPlayerPosition,
                AvailableTurrets = AvailableTurrets,
                AvailablePowerUps = AvailablePowerUps
            };

            Player.Initialize();
            Player.Changed += new SimPlayerHandler(DoPlayerChanged);
            Player.Moved += new SimPlayerHandler(DoPlayerMoved);

            CheckAvailablePowerUps();
            CheckAvailableTurrets();

            NotifyCommonStashChanged(CommonStash);
            NotifyPlayerChanged(Player);
            NotifyPlayerMoved(Player);
        }


        #region Events

        public delegate void TurretTypeCelestialObjectVector3Handler(TurretType typeTourelle, CelestialBody corpsCeleste, Vector3 position);
        public delegate void CelestialObjectTurretHandler(CelestialBody corpsCeleste, Turret turret);

        public event TurretHandler AchatTourelleDemande;
        public event TurretHandler VenteTourelleDemande;
        public event TurretHandler MiseAJourTourelleDemande;
        public event TurretHandler TurretToPlaceSelected;
        public event TurretHandler TurretToPlaceDeselected;
        public event NoneHandler ProchaineVagueDemandee;
        public event CommonStashHandler CommonStashChanged;
        public event SimPlayerHandler PlayerSelectionChanged;
        public event SimPlayerHandler PlayerMoved;
        public event PowerUpTypeHandler ActivatePowerUpAsked;
        public event PowerUpTypeHandler DesactivatePowerUpAsked;


        private void NotifyTurretToPlaceSelected(Turret turret)
        {
            if (TurretToPlaceSelected != null)
                TurretToPlaceSelected(turret);
        }


        private void NotifyTurretToPlaceDeselected(Turret turret)
        {
            if (TurretToPlaceDeselected != null)
                TurretToPlaceDeselected(turret);
        }


        private void NotifyActivatePowerUpAsked(PowerUpType type)
        {
            if (ActivatePowerUpAsked != null)
                ActivatePowerUpAsked(type);
        }


        private void NotifyDesactivatePowerUpAsked(PowerUpType type)
        {
            if (DesactivatePowerUpAsked != null)
                DesactivatePowerUpAsked(type);
        }


        private void NotifyCommonStashChanged(CommonStash stash)
        {
            if (CommonStashChanged != null)
                CommonStashChanged(stash);
        }


        private void NotifyAchatTourelleDemande(Turret turret)
        {
            if (AchatTourelleDemande != null)
                AchatTourelleDemande(turret);
        }


        private void NotifyVenteTourelleDemande(Turret turret)
        {
            if (VenteTourelleDemande != null)
                VenteTourelleDemande(turret);
        }


        private void NotifyMiseAJourTourelleDemande(Turret turret)
        {
            if (MiseAJourTourelleDemande != null)
                MiseAJourTourelleDemande(turret);
        }


        private void NotifyProchaineVagueDemandee()
        {
            if (ProchaineVagueDemandee != null)
                ProchaineVagueDemandee();
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
        
        #endregion


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
                Player.PowerUpInUse = powerUp.Type;

            if (powerUp.Type == PowerUpType.FinalSolution)
                ((PowerUpLastSolution) powerUp).Selection = Player.ActualSelection;

            Player.UpdateSelection();
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
            {
                Player.Position = powerUp.Position;
                NotifyPlayerMoved(Player);
            }

            if (powerUp.Type == PowerUpType.FinalSolution && ((PowerUpLastSolution) powerUp).GoAhead)
                DoPowerUpUse();

            Player.PowerUpInUse = PowerUpType.None;
            CheckAvailablePowerUps();
            Player.UpdateSelection();
        }


        public void DoObjectCreated(IObjetPhysique obj)
        {
            if (obj is RailGunBullet || obj is MineBullet)
                DoPowerUpUse();
        }


        public void DoObjetDetruit(IObjetPhysique objet)
        {
            Enemy ennemi = objet as Enemy;

            if (ennemi != null)
            {
                CommonStash.Cash += ennemi.CashValue;
                CommonStash.Score += ennemi.PointsValue;
                CommonStash.TotalScore += ennemi.PointsValue;

                Player.UpdateSelection();
                NotifyCommonStashChanged(CommonStash);

                return;
            }


            Mineral mineral = objet as Mineral;

            if (mineral != null)
            {
                if (mineral.Type == MineralType.Life1)
                {
                    CommonStash.Lives += mineral.Value;
                    CelestialBodyToProtect.LifePoints += mineral.Value;
                }
                else
                {
                    CommonStash.Cash += mineral.Value;
                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }


            CelestialBody celestialBody = objet as CelestialBody;

            if (celestialBody != null)
            {
                Player.DoCelestialBodyDestroyed();
                Player.UpdateSelection();
                return;
            }
        }


        public void DoMouseMoved(Core.Input.Player p, ref Vector3 delta)
        {
            if (Simulation.DemoMode)
            {
                Player.Move(ref delta, MouseConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, MouseConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.CelestialBody.OuterTurretZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.CelestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
            NotifyPlayerChanged(Player);
        }


        public void DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, ref Vector3 delta)
        {
            if (button != GamePadConfiguration.MoveCursor)
                return;

            if (Simulation.DemoMode)
            {
                Player.Move(ref delta, GamePadConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }

            Player.Move(ref delta, GamePadConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.CelestialBody.OuterTurretZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.CelestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
        }


        public void DoMouseScrolled(Core.Input.Player p, int delta)
        {
            if (Simulation.DemoMode)
            {
                if (Player.ActualSelection.CelestialBody == null)
                    return;

                if (delta > 0)
                    Player.NextGameAction();
                else
                    Player.PreviousGameAction();

                return;
            }


            DoNextorPreviousAction(delta);
        }


        public void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (Simulation.DemoMode)
            {
                if ( Player.ActualSelection.CelestialBody == null )
                    return;

                if ( button == GamePadConfiguration.SelectionNext )
                    Player.NextGameAction();
                else if ( button == GamePadConfiguration.SelectionPrevious )
                    Player.PreviousGameAction();

                return;
            }

            if (button == GamePadConfiguration.Select)
                DoSelectAction();
            else if (button == GamePadConfiguration.Cancel)
                DoCancelAction();
            else if (button == GamePadConfiguration.SelectionNext)
                DoNextorPreviousAction(1);
            else if (button == GamePadConfiguration.SelectionPrevious)
                DoNextorPreviousAction(-1);
        }


        public void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (Simulation.DemoMode)
                return;

            if (button == MouseConfiguration.Select)
                DoSelectAction();
            else if (button == MouseConfiguration.Cancel)
                DoCancelAction();
        }


        public void DoTourelleAchetee(Turret tourelle)
        {
            CommonStash.Cash -= tourelle.BuyPrice;
            NotifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TurretType.Gravitational && !Simulation.DemoMode)
                Audio.PlaySfx(@"Partie", @"sfxTourelleGravitationnelleAchetee");
        }


        public void DoTourelleVendue(Turret tourelle)
        {
            CommonStash.Cash += tourelle.SellPrice;
            NotifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TurretType.Gravitational)
                Audio.PlaySfx(@"Partie", @"sfxTourelleGravitationnelleAchetee");
            else
                Audio.PlaySfx(@"Partie", @"sfxTourelleVendue");
        }


        public void DoTourelleMiseAJour(Turret tourelle)
        {
            CommonStash.Cash -= tourelle.BuyPrice; //parce qu'effectue une fois la tourelle mise a jour
            NotifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();
        }


        public void DoTurretReactivated(Turret turret)
        {
            Player.UpdateSelection();
        }


        public void Update(GameTime gameTime)
        {
            CheckAvailablePowerUps();
            Player.Update();

            if (Player.ActualSelection.TurretToPlace != null)
            {
                Turret turretToPlace = Player.ActualSelection.TurretToPlace;
                CelestialBody celestialBody = Player.ActualSelection.TurretToPlace.CelestialBody;
                turretToPlace.Position = Player.Position;

                if (celestialBody.OuterTurretZone.Outside(Player.Position))
                    Player.Position = celestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);

                turretToPlace.CanPlace = celestialBody.InnerTurretZone.Outside(turretToPlace.Position);
                
                if (turretToPlace.CanPlace)
                    foreach (var turret in celestialBody.Turrets)
                    {
                        turretToPlace.CanPlace = !turret.Visible ||
                            !Physics.collisionCercleCercle(turret.Circle, turretToPlace.Circle);

                        if (!turretToPlace.CanPlace)
                            break;
                    }
            }
        }


        public void Draw()
        {
            if (Player.ActualSelection.TurretToPlace != null)
                Player.ActualSelection.TurretToPlace.Draw();
        }


        private void DoPlayerChanged(SimPlayer player)
        {
            NotifyPlayerChanged(player);
        }


        private void DoPlayerMoved(SimPlayer player)
        {
            NotifyPlayerMoved(player);
        }


        private void DoNextorPreviousAction(int delta)
        {
            // turret's options
            if (Player.ActualSelection.Turret != null)
            {
                if (delta > 0)
                    Player.NextTurretOption();
                else
                    Player.PreviousTurretOption();

                return;
            }


            // shop turrets
            if (Player.ActualSelection.CelestialBody != null &&
                Player.ActualSelection.Turret == null)
            {
                if (delta > 0)
                    Player.NextShitToBuy();
                else
                    Player.PreviousShitToBuy();

                return;
            }
        }


        private void DoSelectAction()
        {
            // activate a power-up
            if (Player.ActualSelection.PowerUpToBuy != PowerUpType.None &&
                AvailablePowerUps[Player.ActualSelection.PowerUpToBuy])
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[Player.ActualSelection.PowerUpToBuy];

                NotifyActivatePowerUpAsked(Player.ActualSelection.PowerUpToBuy);

                if (p.PayOnActivation)
                {
                    CommonStash.Cash -= p.BuyPrice;
                    NotifyCommonStashChanged(CommonStash);
                }
                
                CheckAvailablePowerUps();
                Player.UpdateSelection();

                return;
            }


            // buy a turret
            if (Player.ActualSelection.TurretToBuy != TurretType.None)
            {
                Player.ActualSelection.TurretToPlace = Simulation.TurretsFactory.Create(Player.ActualSelection.TurretToBuy);
                Player.ActualSelection.TurretToPlace.CelestialBody = Player.ActualSelection.CelestialBody;
                Player.ActualSelection.TurretToPlace.Position = Player.Position;
                Player.ActualSelection.TurretToPlace.ToPlaceMode = true;
                Player.UpdateSelection();
                NotifyTurretToPlaceSelected(Player.ActualSelection.TurretToPlace);

                return;
            }

            // place a turret
            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CanPlace)
            {
                Player.ActualSelection.TurretToPlace.ToPlaceMode = false;
                NotifyAchatTourelleDemande(Player.ActualSelection.TurretToPlace);
                NotifyTurretToPlaceDeselected(Player.ActualSelection.TurretToPlace);
                Player.ActualSelection.TurretToPlace = null;
                Player.UpdateSelection();
                return;
            }


            // upgrade or sell a turret
            if (Player.ActualSelection.Turret != null && !Player.ActualSelection.Turret.Disabled)
            {
                switch (Player.ActualSelection.TurretOption)
                {
                    case TurretAction.Sell:
                        NotifyVenteTourelleDemande(Player.ActualSelection.Turret);
                        break;
                    case TurretAction.Update:
                        NotifyMiseAJourTourelleDemande(Player.ActualSelection.Turret);
                        break;
                }

                Player.UpdateSelection();

                return;
            }


            // call next wave
            if (Player.PowerUpInUse == PowerUpType.None && Physics.collisionCercleRectangle(Player.Cercle, SandGlass.Rectangle))
            {
                NotifyProchaineVagueDemandee();
                return;
            }
        }


        private void DoCancelAction()
        {
            if (Player.ActualSelection.TurretToPlace == null)
                return;

            NotifyTurretToPlaceDeselected(Player.ActualSelection.TurretToPlace);
            Player.ActualSelection.TurretToPlace = null;
            Player.UpdateSelection();
        }


        private void DoPowerUpUse()
        {
            if (Player.PowerUpInUse == PowerUpType.None)
                return;

            PowerUp p = Simulation.PowerUpsFactory.Availables[Player.PowerUpInUse];

            if (!p.PayOnUse)
                return;

            CommonStash.Cash -= p.UsePrice;
            NotifyCommonStashChanged(CommonStash);

            CheckAvailablePowerUps();
            Player.UpdateSelection();

            if (CommonStash.Cash < p.UsePrice)
                NotifyDesactivatePowerUpAsked(Player.PowerUpInUse);
        }


        private void CheckAvailablePowerUps()
        {
            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Values)
                AvailablePowerUps[powerUp.Type] =
                    powerUp.BuyPrice <= CommonStash.Cash &&
                    powerUp.UsePrice <= CommonStash.Cash &&
                    ActivesPowerUps[powerUp.Type];
        }


        private void CheckAvailableTurrets()
        {
            foreach (var turret in Simulation.TurretsFactory.Availables.Values)
                AvailableTurrets[turret.Type] = turret.BuyPrice <= CommonStash.Cash;
        }
    }
}
