namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Core.Physique;
    using Core.Input;

    class PlayersController
    {
        public CorpsCeleste CelestialBodyToProtect;
        public List<CorpsCeleste> CelestialBodies;
        public Cursor Cursor;
        public Player Player;
        public Dictionary<PowerUp, bool> AvailableSpaceships;
        public Sablier SandGlass;
        public bool ModeDemo;
        public Vector3 InitialPlayerPosition;

        //todo
        public CorpsCeleste CelestialBodySelected { get { return Player.ActualSelection.CelestialBody; } }


        private Simulation Simulation;


        public PlayersController(Simulation simulation)
        {
            this.Simulation = simulation;
        }


        public void Initialize()
        {
            Player.Cursor = Cursor;
            Player.CelestialBodies = CelestialBodies;
            Player.AvailableSpaceships = AvailableSpaceships;
            Player.Initialize();
            Player.Position = InitialPlayerPosition;
            Player.SelectionChanged += new Player.PlayerHandler(doPlayerSelectionChanged);

            notifyCashChanged(Player.Cash);
            notifyScoreChanged(Player.Score);
            notifyPlayerSelectionChanged(Player.ActualSelection);
        }


        #region Events

        public delegate void CelestialObjectHandler(CorpsCeleste celestialObject);
        public event CelestialObjectHandler AchatDoItYourselfDemande;
        public event CelestialObjectHandler DestructionCorpsCelesteDemande;
        public event CelestialObjectHandler AchatCollecteurDemande;
        public event CelestialObjectHandler AchatTheResistanceDemande;

        public delegate void TurretTypeCelestialObjectTurretSpotHandler(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event TurretTypeCelestialObjectTurretSpotHandler AchatTourelleDemande;

        public delegate void CelestialObjectTurretSpotHandler(CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event CelestialObjectTurretSpotHandler VenteTourelleDemande;
        public event CelestialObjectTurretSpotHandler MiseAJourTourelleDemande;

        public delegate void NoneHandler();
        public event NoneHandler ProchaineVagueDemandee;

        public delegate void IntegerHandler(int score);
        public event IntegerHandler ScoreChanged;
        public event IntegerHandler CashChanged;

        public delegate void PlayerSelectionHandler(PlayerSelection selection);
        public event PlayerSelectionHandler PlayerSelectionChanged;

        private void notifyDestructionCorpsCelesteDemande(CorpsCeleste corpsCeleste)
        {
            if (DestructionCorpsCelesteDemande != null)
                DestructionCorpsCelesteDemande(corpsCeleste);
        }


        private void notifyCashChanged(int cash)
        {
            if (CashChanged != null)
                CashChanged(cash);
        }


        private void notifyScoreChanged(int score)
        {
            if (ScoreChanged != null)
                ScoreChanged(score);
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


        private void notifyPlayerSelectionChanged(PlayerSelection playerSelection)
        {
            if (PlayerSelectionChanged != null)
                PlayerSelectionChanged(playerSelection);
        }
        
        #endregion


        public void doObjetDetruit(IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi != null)
            {
                Player.Cash += ennemi.ValeurUnites;
                Player.Score += ennemi.ValeurPoints;

                notifyCashChanged(Player.Cash);
                notifyScoreChanged(Player.Score);

                return;
            }


            //CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            //if (corpsCeleste != null && Player.Se.CelestialBody == corpsCeleste)
            //{
            //    SelectedCelestialBodyController.Update();
            //    SelectedTurretToBuyController.Update();

            //    notifyPlayerSelectionChanged(PlayerSelection);

            //    return;
            //}


            Mineral mineral = objet as Mineral;

            if (mineral != null)
            {
                if (mineral.Type == 3)
                {
                    Player.Lives += mineral.Valeur;
                    CelestialBodyToProtect.PointsVie += mineral.Valeur;
                }
                else
                {
                    Player.Cash += mineral.Valeur;
                    notifyCashChanged(Player.Cash);
                }

                return;
            }
        }


        public void doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player.Position += delta;
            Player.UpdateSelection();
        }


        public void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player.Position += delta * Cursor.Vitesse;
            Player.UpdateSelection();
        }


        public void doMouseScrolled(PlayerIndex inputIndex, int delta)
        {
            // turret's options
            if (Player.ActualSelection.TurretSpot != null && Player.ActualSelection.TurretSpot.EstOccupe)
            {
                if (delta > 0)
                    Player.NextTurretOption();
                else
                    Player.PreviousTurretOption();

                return;
            }


            // Celestial object' options
            if (Player.ActualSelection.CelestialBody != null && Player.ActualSelection.TurretSpot == null)
            {
                if (delta > 0)
                    Player.NextPowerUpToBuy();
                else
                    Player.PreviousPowerUpToBuy();

                return;
            }


            // shop turrets
            if (delta > 0)
                Player.NextTurretToBuy();
            else
                Player.PreviousTurretToBuy();
        }


        public void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            // buy a powerup
            if (!ModeDemo && Player.ActualSelection.CelestialBody != null && Player.ActualSelection.TurretSpot == null && button == MouseButton.Left)
            {
                switch (Player.ActualSelection.CelestialBodyOption)
                {
                    case PowerUp.DoItYourself:
                        notifyDoItYourselfDemande(Player.ActualSelection.CelestialBody);
                        Player.Cash -= Player.ActualSelection.CelestialBody.PrixDoItYourself;
                        notifyCashChanged(Player.Cash);
                        break;
                    case PowerUp.CollectTheRent:
                        notifyAchatCollecteurDemande(Player.ActualSelection.CelestialBody);
                        Player.Cash -= Player.ActualSelection.CelestialBody.PrixCollecteur;
                        notifyCashChanged(Player.Cash);
                        break;
                    case PowerUp.FinalSolution:
                        Player.Cash -= Player.ActualSelection.CelestialBody.PrixDestruction;
                        notifyCashChanged(Player.Cash);
                        Player.ActualSelection.CelestialBodyOption = PowerUp.Aucune;
                        notifyDestructionCorpsCelesteDemande(Player.ActualSelection.CelestialBody);
                        break;
                    case PowerUp.TheResistance:
                        notifyAchatTheResistanceDemande(Player.ActualSelection.CelestialBody);
                        Player.Cash -= Player.ActualSelection.CelestialBody.PrixTheResistance;
                        notifyCashChanged(Player.Cash);
                        break;
                }

                return;
            }


            // buy a turret
            if (Player.ActualSelection.TurretToBuy != null && button == MouseButton.Left)
            {
                notifyAchatTourelleDemande(Player.ActualSelection.TurretToBuy.Type, Player.ActualSelection.CelestialBody, Player.ActualSelection.TurretSpot);

                return;
            }


            // upgrade or sell a turret
            if (Player.ActualSelection.TurretSpot != null && Player.ActualSelection.TurretSpot.EstOccupe && button == MouseButton.Left)
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


            // todo: key
            // call next wave
            if (!ModeDemo && button == MouseButton.Left && Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, SandGlass.Rectangle))
            {
                notifyProchaineVagueDemandee();
                return;
            }
        }


        public void doTourelleAchetee(Tourelle tourelle)
        {
            Player.Cash -= tourelle.PrixAchat;
            notifyCashChanged(Player.Cash);

            Player.UpdateSelection();

            if (tourelle.Type == TypeTourelle.Gravitationnelle && !Simulation.ModeDemo)
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
        }


        public void doTourelleVendue(Tourelle tourelle)
        {
            Player.Cash += tourelle.PrixVente;
            notifyCashChanged(Player.Cash);

            if (tourelle.Type == TypeTourelle.Gravitationnelle)
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
        }


        public void doTourelleMiseAJour(Tourelle tourelle)
        {
            Player.Cash -= tourelle.PrixAchat; //parce qu'effectue une fois la tourelle mise a jour
            notifyCashChanged(Player.Cash);
        }


        public void doTurretReactivated(Tourelle turret)
        {
            Player.doTurretReactivated(turret);
        }


        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
        }


        private void doPlayerSelectionChanged(Player player)
        {
            notifyPlayerSelectionChanged(player.ActualSelection);
        }
    }
}
