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
        public Dictionary<PowerUp, bool> AvailableSpaceships;
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
            Player = new SimPlayer();
            Player.CelestialBodies = CelestialBodies;
            Player.AvailableSpaceships = AvailableSpaceships;
            Player.CommonStash = CommonStash;
            Player.Initialize();
            Player.Position = InitialPlayerPosition;
            Player.Changed += new SimPlayerHandler(doPlayerChanged);
            Player.Moved += new SimPlayerHandler(doPlayerMoved);

            notifyCommonStashChanged(CommonStash);
            notifyPlayerChanged(Player);
            notifyPlayerMoved(Player);
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
        public event CelestialObjectHandler AchatDoItYourselfDemande;
        public event CelestialObjectHandler DestructionCorpsCelesteDemande;
        public event CelestialObjectHandler AchatCollecteurDemande;
        public event CelestialObjectHandler AchatTheResistanceDemande;


        private void notifyTurretToPlaceSelected(Turret turret)
        {
            if (TurretToPlaceSelected != null)
                TurretToPlaceSelected(turret);
        }


        private void notifyTurretToPlaceDeselected(Turret turret)
        {
            if (TurretToPlaceDeselected != null)
                TurretToPlaceDeselected(turret);
        }

        
        private void notifyDestructionCorpsCelesteDemande(CorpsCeleste corpsCeleste)
        {
            if (DestructionCorpsCelesteDemande != null)
                DestructionCorpsCelesteDemande(corpsCeleste);
        }


        private void notifyCommonStashChanged(CommonStash stash)
        {
            if (CommonStashChanged != null)
                CommonStashChanged(stash);
        }


        private void notifyDoItYourselfDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatDoItYourselfDemande != null)
                AchatDoItYourselfDemande(corpsCeleste);
        }


        private void notifyAchatTourelleDemande(Turret turret)
        {
            if (AchatTourelleDemande != null)
                AchatTourelleDemande(turret);
        }


        private void notifyVenteTourelleDemande(Turret turret)
        {
            if (VenteTourelleDemande != null)
                VenteTourelleDemande(turret);
        }


        private void notifyAchatCollecteurDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatCollecteurDemande != null)
                AchatCollecteurDemande(corpsCeleste);
        }


        private void notifyAchatTheResistanceDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatTheResistanceDemande != null)
                AchatTheResistanceDemande(corpsCeleste);
        }


        private void notifyMiseAJourTourelleDemande(Turret turret)
        {
            if (MiseAJourTourelleDemande != null)
                MiseAJourTourelleDemande(turret);
        }


        private void notifyProchaineVagueDemandee()
        {
            if (ProchaineVagueDemandee != null)
                ProchaineVagueDemandee();
        }


        private void notifyPlayerChanged(SimPlayer player)
        {
            if (PlayerSelectionChanged != null)
                PlayerSelectionChanged(player);
        }


        private void notifyPlayerMoved(SimPlayer player)
        {
            if (PlayerMoved != null)
                PlayerMoved(player);
        }
        
        #endregion


        public void doObjetCree(IObjetPhysique objet)
        {
            Vaisseau spaceship = objet as Vaisseau;

            if (spaceship != null)
            {
                Player.InSpacehip = true;
                Player.UpdateSelection();
                return;
            }


            TheResistance resistance = objet as TheResistance;

            if (resistance != null)
            {
                Player.UpdateSelection();
                return;
            }
        }

        public void doObjetDetruit(IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi != null)
            {
                CommonStash.Cash += ennemi.ValeurUnites;
                CommonStash.Score += ennemi.ValeurPoints;
                CommonStash.TotalScore += ennemi.ValeurPoints;

                Player.UpdateSelection();
                notifyCommonStashChanged(CommonStash);

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
                    notifyCommonStashChanged(CommonStash);
                }

                return;
            }


            Vaisseau spaceship = objet as Vaisseau;

            if (spaceship != null)
            {
                Player.InSpacehip = false;
                Player.Position = spaceship.Position;
                notifyPlayerMoved(Player);
                Player.UpdateSelection();
                return;
            }


            TheResistance resistance = objet as TheResistance;

            if (resistance != null)
            {
                Player.UpdateSelection();
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


        public void doMouseMoved(Player player, ref Vector3 delta)
        {
            if (ModeDemo)
            {
                Player.Move(ref delta, player.MouseConfiguration.Speed);
                Player.UpdateDemoSelection();
                notifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, player.MouseConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.pointPlusProcheCirconference(Player.Position);


            Player.UpdateSelection();
            notifyPlayerMoved(Player);
            notifyPlayerChanged(Player);
        }


        public void doGamePadJoystickMoved(Player player, Buttons button, ref Vector3 delta)
        {
            if (button != player.GamePadConfiguration.MoveCursor)
                return;


            if (ModeDemo)
            {
                Player.Move(ref delta, player.GamePadConfiguration.Speed);
                Player.UpdateDemoSelection();
                notifyPlayerMoved(Player);
                return;
            }


            Player.Move(ref delta, player.GamePadConfiguration.Speed);

            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.Outside(Player.Position))
                Player.Position = Player.ActualSelection.TurretToPlace.CelestialBody.TurretsZone.pointPlusProcheCirconference(Player.Position);


            Player.UpdateSelection();
            notifyPlayerMoved(Player);
        }


        public void doMouseScrolled(Player player, int delta)
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


            doNextorPreviousAction(delta);
        }


        public void doGamePadButtonPressedOnce(Player player, Buttons button)
        {
            if (button == player.GamePadConfiguration.Select)
                doSelectAction();
            else if (button == player.GamePadConfiguration.Cancel)
                doCancelAction();
            else if (button == player.GamePadConfiguration.SelectionNext)
                doNextorPreviousAction(1);
            else if (button == player.GamePadConfiguration.SelectionPrevious)
                doNextorPreviousAction(-1);
        }


        public void doMouseButtonPressedOnce(Player player, MouseButton button)
        {
            if (ModeDemo)
                return;

            if (button == player.MouseConfiguration.Select)
                doSelectAction();
            else if (button == player.MouseConfiguration.Cancel)
                doCancelAction();
        }


        public void doTourelleAchetee(Turret tourelle)
        {
            CommonStash.Cash -= tourelle.BuyPrice;
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TurretType.Gravitational && !Simulation.ModeDemo)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
        }


        public void doTourelleVendue(Turret tourelle)
        {
            CommonStash.Cash += tourelle.SellPrice;
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TurretType.Gravitational)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
        }


        public void doTourelleMiseAJour(Turret tourelle)
        {
            CommonStash.Cash -= tourelle.BuyPrice; //parce qu'effectue une fois la tourelle mise a jour
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();
        }


        public void doTurretReactivated(Turret turret)
        {
            //Player.doTurretReactivated(turret);
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


        private void doPlayerChanged(SimPlayer player)
        {
            notifyPlayerChanged(player);
        }


        private void doPlayerMoved(SimPlayer player)
        {
            notifyPlayerMoved(player);
        }


        private void doNextorPreviousAction(int delta)
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


            // shop power-ups or turrets
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


        private void doSelectAction()
        {
            // buy a powerup
            if (Player.ActualSelection.PowerUpToBuy != PowerUp.None)
            {
                switch (Player.ActualSelection.PowerUpToBuy)
                {
                    case PowerUp.DoItYourself:
                        notifyDoItYourselfDemande(Player.ActualSelection.CelestialBody);
                        CommonStash.Cash -= Player.ActualSelection.CelestialBody.PrixDoItYourself;
                        notifyCommonStashChanged(CommonStash);
                        break;
                    case PowerUp.CollectTheRent:
                        notifyAchatCollecteurDemande(Player.ActualSelection.CelestialBody);
                        CommonStash.Cash -= Player.ActualSelection.CelestialBody.PrixCollecteur;
                        notifyCommonStashChanged(CommonStash);
                        break;
                    case PowerUp.FinalSolution:
                        CommonStash.Cash -= Player.ActualSelection.CelestialBody.PrixDestruction;
                        notifyCommonStashChanged(CommonStash);
                        Player.ActualSelection.PowerUpToBuy = PowerUp.None;
                        notifyDestructionCorpsCelesteDemande(Player.ActualSelection.CelestialBody);
                        break;
                    case PowerUp.TheResistance:
                        notifyAchatTheResistanceDemande(Player.ActualSelection.CelestialBody);
                        CommonStash.Cash -= Player.ActualSelection.CelestialBody.PrixTheResistance;
                        notifyCommonStashChanged(CommonStash);
                        break;
                }

                Player.UpdateSelection();

                return;
            }


            // buy a turret
            if (Player.ActualSelection.TurretToBuy != null)
            {
                Player.ActualSelection.TurretToPlace = Simulation.TurretFactory.CreateTurret(Player.ActualSelection.TurretToBuy.Type);
                Player.ActualSelection.TurretToPlace.CelestialBody = Player.ActualSelection.CelestialBody;
                Player.ActualSelection.TurretToPlace.Position = Player.Position;
                Player.ActualSelection.TurretToPlace.ToPlaceMode = true;
                Player.UpdateSelection();
                notifyTurretToPlaceSelected(Player.ActualSelection.TurretToPlace);

                return;
            }

            // place a turret
            if (Player.ActualSelection.TurretToPlace != null &&
                Player.ActualSelection.TurretToPlace.CanPlace)
            {
                Player.ActualSelection.TurretToPlace.ToPlaceMode = false;
                notifyAchatTourelleDemande(Player.ActualSelection.TurretToPlace);
                notifyTurretToPlaceDeselected(Player.ActualSelection.TurretToPlace);
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
                        notifyVenteTourelleDemande(Player.ActualSelection.Turret);
                        break;
                    case TurretAction.Update:
                        notifyMiseAJourTourelleDemande(Player.ActualSelection.Turret);
                        break;
                }

                Player.UpdateSelection();

                return;
            }


            // call next wave
            if (EphemereGames.Core.Physique.Facade.collisionCercleRectangle(Player.Cercle, SandGlass.Rectangle))
            {
                notifyProchaineVagueDemandee();
                return;
            }
        }


        private void doCancelAction()
        {
            if (Player.ActualSelection.TurretToPlace == null)
                return;

            notifyTurretToPlaceDeselected(Player.ActualSelection.TurretToPlace);
            Player.ActualSelection.TurretToPlace = null;
            Player.UpdateSelection();
        }
    }
}
