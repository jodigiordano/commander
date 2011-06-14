﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Audio;


    class SimPlayersController
    {
        public CelestialBody CelestialBodyToProtect;
        public List<CelestialBody> CelestialBodies;
        public CommonStash CommonStash;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public SandGlass SandGlass;
        public Vector3 InitialPlayerPosition;


        //todo
        public CelestialBody SelectedCelestialBody { get { return Player.ActualSelection.CelestialBody; } }


        private Simulation Simulation;
        private SimPlayer Player;


        public SimPlayersController(Simulation simulation)
        {
            Simulation = simulation;
        }


        public void Initialize()
        {
            Player = new SimPlayer(Simulation);
            Player.CelestialBodies = CelestialBodies;
            Player.ActivesPowerUps = ActivesPowerUps;
            Player.CommonStash = CommonStash;
            Player.Initialize();
            Player.Position = InitialPlayerPosition;
            Player.Changed += new SimPlayerHandler(DoPlayerChanged);
            Player.Moved += new SimPlayerHandler(DoPlayerMoved);

            Player.CheckAvailablePowerUps();

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
            Player.CheckAvailablePowerUps();
            Player.UpdateSelection();
        }


        public void DoObjectCreated(IObjetPhysique obj)
        {
            if (obj is RailGunBullet)
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


        public void DoMouseMoved(Player player, ref Vector3 delta)
        {
            if (Simulation.DemoMode)
            {
                Player.Move(ref delta, player.MouseConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, player.MouseConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
            NotifyPlayerChanged(Player);
        }


        public void DoGamePadJoystickMoved(Player player, Buttons button, ref Vector3 delta)
        {
            if (button != player.GamePadConfiguration.MoveCursor)
                return;

            if (Simulation.DemoMode)
            {
                Player.Move(ref delta, player.GamePadConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }

            Player.Move(ref delta, player.GamePadConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
        }


        public void DoMouseScrolled(Player player, int delta)
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


        public void DoGamePadButtonPressedOnce(Player player, Buttons button)
        {
            if (Simulation.DemoMode)
            {
                if ( Player.ActualSelection.CelestialBody == null )
                    return;

                if ( button == player.GamePadConfiguration.SelectionNext )
                    Player.NextGameAction();
                else if ( button == player.GamePadConfiguration.SelectionPrevious )
                    Player.PreviousGameAction();

                return;
            }

            if (button == player.GamePadConfiguration.Select)
                DoSelectAction();
            else if (button == player.GamePadConfiguration.Cancel)
                DoCancelAction();
            else if (button == player.GamePadConfiguration.SelectionNext)
                DoNextorPreviousAction(1);
            else if (button == player.GamePadConfiguration.SelectionPrevious)
                DoNextorPreviousAction(-1);
        }


        public void DoMouseButtonPressedOnce(Player player, MouseButton button)
        {
            if (Simulation.DemoMode)
                return;

            if (button == player.MouseConfiguration.Select)
                DoSelectAction();
            else if (button == player.MouseConfiguration.Cancel)
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
            Player.Update(gameTime);

            Turret turretToPlace = Player.ActualSelection.TurretToPlace;

            if (turretToPlace != null)
            {
                CelestialBody celestialBody = turretToPlace.CelestialBody;
                turretToPlace.Position = Player.Position;

                if (celestialBody.OuterTurretZone.Outside(Player.Position))
                    Player.Position = celestialBody.OuterTurretZone.NearestPointToCircumference(Player.Position);

                //if (!celestialBody.InnerTurretZone.Outside(Player.Position))
                //    Player.Position = celestialBody.InnerTurretZone.NearestPointToCircumference(Player.Position);

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
                Player.ActualSelection.AvailablePowerUpsToBuy[Player.ActualSelection.PowerUpToBuy])
            {
                PowerUp p = Simulation.PowerUpsFactory.Availables[Player.ActualSelection.PowerUpToBuy];

                NotifyActivatePowerUpAsked(Player.ActualSelection.PowerUpToBuy);

                if (p.PayOnActivation)
                {
                    CommonStash.Cash -= p.BuyPrice;
                    NotifyCommonStashChanged(CommonStash);
                }
                
                Player.CheckAvailablePowerUps();
                Player.UpdateSelection();

                return;
            }


            // buy a turret
            if (Player.ActualSelection.TurretToBuy != null)
            {
                Player.ActualSelection.TurretToPlace = Simulation.TurretsFactory.Create(Player.ActualSelection.TurretToBuy.Type);
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

            Player.CheckAvailablePowerUps();
            Player.UpdateSelection();
        }
    }
}