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

        public delegate void TurretTypeCelestialObjectTurretSpotHandler(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement);
        public delegate void CelestialObjectTurretSpotHandler(CorpsCeleste corpsCeleste, Emplacement emplacement);

        public event TurretTypeCelestialObjectTurretSpotHandler AchatTourelleDemande;        
        public event CelestialObjectTurretSpotHandler VenteTourelleDemande;
        public event CelestialObjectTurretSpotHandler MiseAJourTourelleDemande;
        public event NoneHandler ProchaineVagueDemandee;
        public event CommonStashHandler CommonStashChanged;
        public event SimPlayerHandler PlayerSelectionChanged;
        public event SimPlayerHandler PlayerMoved;
        public event CelestialObjectHandler AchatDoItYourselfDemande;
        public event CelestialObjectHandler DestructionCorpsCelesteDemande;
        public event CelestialObjectHandler AchatCollecteurDemande;
        public event CelestialObjectHandler AchatTheResistanceDemande;


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


        private void notifyAchatTourelleDemande(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (AchatTourelleDemande != null)
                AchatTourelleDemande(typeTourelle, corpsCeleste, emplacement);
        }


        private void notifyVenteTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (VenteTourelleDemande != null)
                VenteTourelleDemande(corpsCeleste, emplacement);
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


        private void notifyMiseAJourTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (MiseAJourTourelleDemande != null)
                MiseAJourTourelleDemande(corpsCeleste, emplacement);
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
                if (mineral.Type == 3)
                {
                    CommonStash.Lives += mineral.Valeur;
                    CelestialBodyToProtect.PointsVie += mineral.Valeur;
                }
                else
                {
                    CommonStash.Cash += mineral.Valeur;
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
            Player.Move(ref delta, player.MouseConfiguration.Speed);
            Player.UpdateSelection();
            notifyPlayerMoved(Player);
        }


        public void doGamePadJoystickMoved(Player player, Buttons button, ref Vector3 delta)
        {
            if (button != player.GamePadConfiguration.MoveCursor)
                return;

            Player.Move(ref delta, player.GamePadConfiguration.Speed);
            Player.UpdateSelection();
            notifyPlayerMoved(Player);
        }


        public void doMouseScrolled(Player player, int delta)
        {
            doNextorPreviousAction(delta);
        }


        public void doGamePadButtonPressedOnce(Player player, Buttons button)
        {
            if (button == player.GamePadConfiguration.Select)
            {
                doSelectAction();
                return;
            }

            if (button == player.GamePadConfiguration.SelectionNext)
                doNextorPreviousAction(1);
            else if (button == player.GamePadConfiguration.SelectionPrevious)
                doNextorPreviousAction(-1);
        }


        public void doMouseButtonPressedOnce(Player player, MouseButton button)
        {
            if (button != player.MouseConfiguration.Select)
                return;

            doSelectAction();
        }


        public void doTourelleAchetee(Tourelle tourelle)
        {
            CommonStash.Cash -= tourelle.PrixAchat;
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TypeTourelle.Gravitationnelle && !Simulation.ModeDemo)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
        }


        public void doTourelleVendue(Tourelle tourelle)
        {
            CommonStash.Cash += tourelle.PrixVente;
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();

            if (tourelle.Type == TypeTourelle.Gravitationnelle)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
        }


        public void doTourelleMiseAJour(Tourelle tourelle)
        {
            CommonStash.Cash -= tourelle.PrixAchat; //parce qu'effectue une fois la tourelle mise a jour
            notifyCommonStashChanged(CommonStash);

            Player.UpdateSelection();
        }


        public void doTurretReactivated(Tourelle turret)
        {
            //Player.doTurretReactivated(turret);
            Player.UpdateSelection();
        }


        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
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
            if (Player.ActualSelection.TurretSpot != null &&
                Player.ActualSelection.TurretSpot.EstOccupe)
            {
                if (delta > 0)
                    Player.NextTurretOption();
                else
                    Player.PreviousTurretOption();

                return;
            }


            // Celestial object' options
            if (Player.ActualSelection.CelestialBody != null &&
                Player.ActualSelection.TurretSpot == null)
            {
                if (delta > 0)
                    Player.NextPowerUpToBuy();
                else
                    Player.PreviousPowerUpToBuy();

                return;
            }


            // shop turrets
            if (Player.ActualSelection.TurretSpot != null &&
                !Player.ActualSelection.TurretSpot.EstOccupe)
            {
                if (delta > 0)
                    Player.NextTurretToBuy();
                else
                    Player.PreviousTurretToBuy();
            }
        }


        private void doSelectAction()
        {
            // buy a powerup
            if (!ModeDemo &&
                Player.ActualSelection.CelestialBody != null &&
                Player.ActualSelection.TurretSpot == null)
            {
                switch (Player.ActualSelection.CelestialBodyOption)
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
                        Player.ActualSelection.CelestialBodyOption = PowerUp.Aucune;
                        notifyDestructionCorpsCelesteDemande(Player.ActualSelection.CelestialBody);
                        break;
                    case PowerUp.TheResistance:
                        notifyAchatTheResistanceDemande(Player.ActualSelection.CelestialBody);
                        CommonStash.Cash -= Player.ActualSelection.CelestialBody.PrixTheResistance;
                        notifyCommonStashChanged(CommonStash);
                        break;
                }

                return;
            }


            // buy a turret
            if (Player.ActualSelection.TurretToBuy != null)
            {
                notifyAchatTourelleDemande(Player.ActualSelection.TurretToBuy.Type, Player.ActualSelection.CelestialBody, Player.ActualSelection.TurretSpot);

                return;
            }


            // upgrade or sell a turret
            if (Player.ActualSelection.TurretSpot != null &&
                Player.ActualSelection.TurretSpot.EstOccupe)
            {
                switch (Player.ActualSelection.TurretOption)
                {
                    case TurretAction.Sell:
                        notifyVenteTourelleDemande(Player.ActualSelection.CelestialBody, Player.ActualSelection.TurretSpot);
                        break;
                    case TurretAction.Update:
                        notifyMiseAJourTourelleDemande(Player.ActualSelection.CelestialBody, Player.ActualSelection.TurretSpot);
                        break;
                }

                return;
            }


            // call next wave
            if (!ModeDemo &&
                EphemereGames.Core.Physique.Facade.collisionCercleRectangle(Player.Cercle, SandGlass.Rectangle))
            {
                notifyProchaineVagueDemandee();
                return;
            }
        }
    }
}
