namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Input;


    class SimPlayersController
    {
        public CorpsCeleste CelestialBodyToProtect;
        public List<CorpsCeleste> CelestialBodies;
        public CommonStash CommonStash;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public Sablier SandGlass;
        public bool ModeDemo;
        public Vector3 InitialPlayerPosition;


        //todo
        public CorpsCeleste CelestialBodySelected { get { return Player.ActualSelection.CelestialBody; } }


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

            NotifyCommonStashChanged(CommonStash);
            NotifyPlayerChanged(Player);
            NotifyPlayerMoved(Player);
        }


        #region Events

        public delegate void TurretTypeCelestialObjectVector3Handler(TurretType typeTourelle, CorpsCeleste corpsCeleste, Vector3 position);
        public delegate void CelestialObjectTurretHandler(CorpsCeleste corpsCeleste, Turret turret);

        public event TurretHandler AchatTourelleDemande;
        public event TurretHandler VenteTourelleDemande;
        public event TurretHandler MiseAJourTourelleDemande;
        public event TurretHandler TurretToPlaceSelected;
        public event TurretHandler TurretToPlaceDeselected;
        public event NoneHandler ProchaineVagueDemandee;
        public event CommonStashHandler CommonStashChanged;
        public event SimPlayerHandler PlayerSelectionChanged;
        public event SimPlayerHandler PlayerMoved;
        public event PowerUpTypeHandler BuyAPowerUpAsked;


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


        private void NotifyBuyAPowerUpAsked(PowerUpType type)
        {
            if (BuyAPowerUpAsked != null)
                BuyAPowerUpAsked(type);
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
                Player.InSpacehip = true;

            Player.UpdateSelection();
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
            {
                Player.InSpacehip = false;
                Player.Position = powerUp.Position;
                NotifyPlayerMoved(Player);
            }

            Player.UpdateSelection();
        }


        public void DoObjetDetruit(IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi != null)
            {
                CommonStash.Cash += ennemi.ValeurUnites;
                CommonStash.Score += ennemi.ValeurPoints;
                CommonStash.TotalScore += ennemi.ValeurPoints;

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
                    CelestialBodyToProtect.PointsVie += mineral.Value;
                }
                else
                {
                    CommonStash.Cash += mineral.Value;
                    NotifyCommonStashChanged(CommonStash);
                }

                return;
            }


            CorpsCeleste celestialBody = objet as CorpsCeleste;

            if (celestialBody != null)
            {
                Player.DoCelestialBodyDestroyed();
                Player.UpdateSelection();
                return;
            }
        }


        public void DoMouseMoved(Player player, ref Vector3 delta)
        {
            if (ModeDemo)
            {
                Player.Move(ref delta, player.MouseConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, player.MouseConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.pointPlusProcheCirconference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
            NotifyPlayerChanged(Player);
        }


        public void DoGamePadJoystickMoved(Player player, Buttons button, ref Vector3 delta)
        {
            if (button != player.GamePadConfiguration.MoveCursor)
                return;


            if (ModeDemo)
            {
                Player.Move(ref delta, player.GamePadConfiguration.Speed);
                Player.UpdateDemoSelection();
                NotifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, player.GamePadConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.pointPlusProcheCirconference(Player.Position);


            Player.UpdateSelection();
            NotifyPlayerMoved(Player);
        }


        public void DoMouseScrolled(Player player, int delta)
        {
            if (ModeDemo)
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
            if ( ModeDemo )
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
            if (ModeDemo)
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

            if (tourelle.Type == TurretType.Gravitational && !Simulation.ModeDemo)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
        }


        public void DoTourelleVendue(Turret tourelle)
        {
            CommonStash.Cash += tourelle.SellPrice;
            NotifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TurretType.Gravitational)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
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
                CorpsCeleste celestialBody = turretToPlace.CelestialBody;
                turretToPlace.Position = Player.Position;

                if (celestialBody.TurretsZone.Outside(Player.Position))
                    Player.Position = celestialBody.TurretsZone.pointPlusProcheCirconference(Player.Position);

                turretToPlace.CanPlace = celestialBody.Cercle.Outside(turretToPlace.Position);
                
                if (turretToPlace.CanPlace)
                    foreach (var turret in celestialBody.Turrets)
                    {
                        turretToPlace.CanPlace = !turret.Visible ||
                            !Core.Physique.Facade.collisionCercleCercle(turret.Cercle, turretToPlace.Cercle);

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
            // buy a powerup
            if (Player.ActualSelection.PowerUpToBuy != PowerUpType.None &&
                Player.ActualSelection.AvailablePowerUpsToBuy[Player.ActualSelection.PowerUpToBuy])
            {
                CommonStash.Cash -= Simulation.PowerUpsFactory.Availables[Player.ActualSelection.PowerUpToBuy].BuyPrice;
                NotifyBuyAPowerUpAsked(Player.ActualSelection.PowerUpToBuy);
                NotifyCommonStashChanged(CommonStash);

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
            if (Player.ActualSelection.Turret != null)
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
            if (EphemereGames.Core.Physique.Facade.collisionCercleRectangle(Player.Cercle, SandGlass.Rectangle))
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
    }
}
