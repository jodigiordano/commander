namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;
    using Core.Visuel;
    using Core.Input;
    using Microsoft.Xna.Framework.Input;

    delegate void PhysicalObjectHandler(IObjetPhysique obj);
    delegate void SimPlayerHandler(SimPlayer player);

    class Simulation : InputListener
    {
        public Scene Scene;
        public Main Main;
        public DescripteurScenario DescriptionScenario;
        public bool Debug;
        public Dictionary<PlayerIndex, Player> Players;

        private ControleurScenario ControleurScenario;
        private ControleurEnnemis ControleurEnnemis;
        private ControleurProjectiles ControleurProjectiles;
        private ControleurCollisions ControleurCollisions;
        private SimPlayersController SimPlayersController;
        private ControleurTourelles ControleurTourelles;
        public ControleurSystemePlanetaire ControleurSystemePlanetaire;
        private ControleurVaisseaux ControleurVaisseaux;
        public ControleurMessages ControleurMessages;
        private GUIController GUIController;
        
        public RectanglePhysique Terrain = new RectanglePhysique(-840, -560, 1680, 1120);

        public bool EnPause;
        public EtatPartie Etat                      { get { return ControleurScenario.Etat; } }

        private bool modeDemo = false;
        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                SimPlayersController.ModeDemo = value;
                ControleurScenario.ModeDemo = value;
            }
        }


        private bool modeEditeur = false;
        public bool ModeEditeur
        {
            get { return modeEditeur; }
            set
            {
                modeEditeur = value;

                ControleurScenario.ModeEditeur = value;
            }
        }

        public bool InitParticules = true;


        public CorpsCeleste CorpsCelesteSelectionne
        {
            get
            {
                return SimPlayersController.CelestialBodySelected;
            }
        }

        public Vector3 PositionCurseur; 


        public Simulation(Main main, Scene scene, DescripteurScenario scenario)
        {
            Scene = scene;
            Main = main;
            DescriptionScenario = scenario;

#if DEBUG
            this.Debug = true;
#else
            this.Debug = false;
#endif
        }


        public void Initialize()
        {
            if (InitParticules)
            {
                Scene.Particules.vider();
                Scene.Particules.ajouter("projectileMissileDeplacement");
                Scene.Particules.ajouter("projectileBaseExplosion");
                Scene.Particules.ajouter("etoile");
                Scene.Particules.ajouter("etoileBleue");
                Scene.Particules.ajouter("planeteGazeuse");
                Scene.Particules.ajouter("etoilesScintillantes");
                Scene.Particules.ajouter("projectileMissileExplosion");
                Scene.Particules.ajouter("projectileLaserSimple");
                Scene.Particules.ajouter("projectileLaserMultiple");
                Scene.Particules.ajouter("selectionCorpsCeleste");
                Scene.Particules.ajouter("traineeMissile");
                Scene.Particules.ajouter("etincelleLaser");
                Scene.Particules.ajouter("toucherTerre");
                Scene.Particules.ajouter("anneauTerreMeurt");
                Scene.Particules.ajouter("bouleTerreMeurt");
                Scene.Particules.ajouter("missileAlien");
                Scene.Particules.ajouter("implosionAlien");
                Scene.Particules.ajouter("explosionEnnemi");
                Scene.Particules.ajouter("mineral1");
                Scene.Particules.ajouter("mineral2");
                Scene.Particules.ajouter("mineral3");
                Scene.Particules.ajouter("mineralPointsVie");
                Scene.Particules.ajouter("mineralPris");
                Scene.Particules.ajouter("etincelleMissile");
                Scene.Particules.ajouter("etincelleLaserSimple");
                Scene.Particules.ajouter("etincelleSlowMotion");
                Scene.Particules.ajouter("etincelleSlowMotionTouche");
                Scene.Particules.ajouter("etoileFilante");
                Scene.Particules.ajouter("trouRose");
            }

            ControleurCollisions = new ControleurCollisions(this);
            ControleurProjectiles = new ControleurProjectiles(this);
            ControleurEnnemis = new ControleurEnnemis(this);
            SimPlayersController = new SimPlayersController(this);
            ControleurTourelles = new ControleurTourelles(this);
            ControleurSystemePlanetaire = new ControleurSystemePlanetaire(this);
            ControleurScenario = new ControleurScenario(this, new Scenario(this, DescriptionScenario));
            ControleurVaisseaux = new ControleurVaisseaux(this);
            ControleurMessages = new ControleurMessages(this);
            GUIController = new GUIController(this);

            SimPlayersController.Players = this.Players;
            ControleurCollisions.Projectiles = ControleurProjectiles.Projectiles;
            ControleurCollisions.Ennemis = ControleurEnnemis.Ennemis;
            SimPlayersController.CelestialBodies = ControleurScenario.CorpsCelestes;
            ControleurSystemePlanetaire.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurTourelles.ControleurSystemePlanetaire = ControleurSystemePlanetaire;
            ControleurEnnemis.VaguesInfinies = ControleurScenario.VaguesInfinies;
            ControleurEnnemis.Vagues = ControleurScenario.Vagues;
            ControleurCollisions.Tourelles = ControleurTourelles.Tourelles;
            SimPlayersController.CommonStash = ControleurScenario.CommonStash;
            SimPlayersController.CelestialBodyToProtect = ControleurScenario.CorpsCelesteAProteger;
            GUIController.Path = ControleurSystemePlanetaire.Chemin;
            GUIController.PathPreview = ControleurSystemePlanetaire.CheminProjection;
            ControleurEnnemis.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurTourelles.TourellesDepart = ControleurScenario.TourellesDepart;
            ControleurEnnemis.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurCollisions.CorpsCelestes = ControleurScenario.CorpsCelestes;
            SimPlayersController.AvailableSpaceships = ControleurVaisseaux.OptionsDisponibles;
            ControleurCollisions.Mineraux = ControleurEnnemis.Mineraux;
            ControleurEnnemis.ValeurTotalMineraux = ControleurScenario.Scenario.ValeurMinerauxDonnes;
            ControleurEnnemis.PourcentageMinerauxDonnes = ControleurScenario.Scenario.PourcentageMinerauxDonnes;
            ControleurEnnemis.NbPackViesDonnes = ControleurScenario.Scenario.NbPackViesDonnes;
            ControleurVaisseaux.Ennemis = ControleurEnnemis.Ennemis;
            ControleurMessages.Tourelles = ControleurTourelles.Tourelles;
            //ControleurMessages.BulleGUI = PlayerController.BulleGUI;
            ControleurMessages.CorpsCelesteAProteger = ControleurScenario.CorpsCelesteAProteger;
            ControleurMessages.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurMessages.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurMessages.Sablier = GUIController.SandGlass;
            GUIController.CompositionNextWave = ControleurEnnemis.CompositionProchaineVague;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = ControleurSystemePlanetaire.CorpsCelestes;
            GUIController.Scenario = ControleurScenario.Scenario;
            GUIController.Enemies = ControleurEnnemis.Ennemis;
            //ControleurMessages.Curseur = GUIController.Cursor;
            SimPlayersController.InitialPlayerPosition = PositionCurseur;
            GUIController.InfiniteWaves = ControleurScenario.VaguesInfinies;
            GUIController.Waves = ControleurScenario.Vagues;


            ControleurCollisions.ObjetTouche += new ControleurCollisions.ObjetToucheHandler(ControleurEnnemis.doObjetTouche);
            SimPlayersController.AchatTourelleDemande += new SimPlayersController.TurretTypeCelestialObjectTurretSpotHandler(ControleurTourelles.doAcheterTourelle);
            ControleurEnnemis.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ControleurScenario.doVagueTerminee);
            ControleurEnnemis.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            ControleurCollisions.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(ControleurTourelles.doDansZoneActivationTourelle);
            ControleurTourelles.ObjectCreated += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurSystemePlanetaire.doObjetCree);
            ControleurProjectiles.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doWaveStarted);
            SimPlayersController.VenteTourelleDemande += new SimPlayersController.CelestialObjectTurretSpotHandler(ControleurTourelles.doVendreTourelle);
            ControleurTourelles.TurretSold += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleVendue);
            ControleurTourelles.TurretBought += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleAchetee);
            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ControleurScenario.doEnnemiAtteintFinTrajet);
            SimPlayersController.AchatCollecteurDemande += new SimPlayersController.CelestialObjectHandler(ControleurVaisseaux.doAcheterCollecteur);
            ControleurTourelles.TurretUpdated += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleMiseAJour);
            SimPlayersController.MiseAJourTourelleDemande += new SimPlayersController.CelestialObjectTurretSpotHandler(ControleurTourelles.doMettreAJourTourelle);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurTourelles.doObjetDetruit);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.ProchaineVagueDemandee += new SimPlayersController.NoneHandler(ControleurEnnemis.doProchaineVagueDemandee);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            SimPlayersController.AchatDoItYourselfDemande += new SimPlayersController.CelestialObjectHandler(ControleurVaisseaux.doAcheterDoItYourself);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.DestructionCorpsCelesteDemande += new SimPlayersController.CelestialObjectHandler(ControleurSystemePlanetaire.doDetruireCorpsCeleste);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            SimPlayersController.AchatTheResistanceDemande += new SimPlayersController.CelestialObjectHandler(ControleurVaisseaux.doAcheterTheResistance);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurScenario.doObjetDetruit);

            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(this.doEnnemiAtteintFinTrajet);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(this.doCorpsCelesteDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doNextWave);
            SimPlayersController.CashChanged += new SimPlayersController.IntegerHandler(GUIController.doCashChanged);
            SimPlayersController.ScoreChanged += new SimPlayersController.IntegerHandler(GUIController.doScoreChanged);
            ControleurScenario.NouvelEtatPartie += new ControleurScenario.NouvelEtatPartieHandler(GUIController.doGameStateChanged);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.doPlayerSelectionChanged);
            ControleurTourelles.TurretReactivated += new ControleurTourelles.TurretHandler(SimPlayersController.doTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.doPlayerMoved);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(SimPlayersController.doObjetCree);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(GUIController.doObjectCreated);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);


            ControleurScenario.Initialize();
            SimPlayersController.Initialize();
            ControleurEnnemis.Initialize();
            ControleurProjectiles.Initialize();
            ControleurTourelles.Initialize();
            ControleurSystemePlanetaire.Initialize();
            ControleurCollisions.Initialize();
            ControleurVaisseaux.Initialize();
            ControleurMessages.Initialize();
            GUIController.Initialize();

            ModeDemo = modeDemo;
            ModeEditeur = modeEditeur;
        }


        public void Update(GameTime gameTime)
        {
            if (EnPause)
                return;

            ControleurCollisions.Update(gameTime);
            ControleurProjectiles.Update(gameTime);
            ControleurTourelles.Update(gameTime); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
            ControleurEnnemis.Update(gameTime);
            SimPlayersController.Update(gameTime);
            ControleurSystemePlanetaire.Update(gameTime);
            ControleurScenario.Update(gameTime);
            ControleurVaisseaux.Update(gameTime);
            ControleurMessages.Update(gameTime);
            GUIController.Update(gameTime);
        }


        public void Draw(GameTime gameTime)
        {
            ControleurCollisions.Draw(null);
            ControleurProjectiles.Draw(null);
            ControleurEnnemis.Draw(null);
            ControleurTourelles.Draw(null);
            ControleurSystemePlanetaire.Draw(null);
            ControleurScenario.Draw(null);
            ControleurVaisseaux.Draw(null);
            ControleurMessages.Draw(null);
            GUIController.Draw();
        }


        public void EtreNotifierNouvelEtatPartie(ControleurScenario.NouvelEtatPartieHandler handler)
        {
            ControleurScenario.NouvelEtatPartie += handler;
        }


        private void doEnnemiAtteintFinTrajet(Ennemi ennemi, CorpsCeleste corpsCeleste)
        {
            if (Etat == EtatPartie.Gagnee)
                return;

            if (!this.ModeDemo && this.Etat != EtatPartie.Perdue)
            {
                foreach (var joueur in this.Main.Players.Values)
                    Core.Input.Facade.VibrateController(joueur.Index, 300, 0.5f, 0.5f);
            }
        }


        private void doCorpsCelesteDetruit(IObjetPhysique objet)
        {
            foreach (var joueur in this.Main.Players.Values)
                Core.Input.Facade.VibrateController(joueur.Index, 300, 0.5f, 0.5f);
        }


        #region InputListener Membres

        bool InputListener.Active
        {
            get { return Etat == EtatPartie.EnCours; }
        }


        void InputListener.doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (this.Debug && key == p.KeyboardConfiguration.Debug)
                ControleurCollisions.Debug = true;
        }


        void InputListener.doKeyReleased(PlayerIndex inputIndex, Keys key)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (this.Debug && key == p.KeyboardConfiguration.Debug)
                ControleurCollisions.Debug = false;
        }


        void InputListener.doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ControleurVaisseaux.VaisseauCollecteurActif && button == p.MouseConfiguration.Cancel)
                ControleurVaisseaux.VaisseauCollecteurActif = false;

            SimPlayersController.doMouseButtonPressedOnce(p, button);

            if (button == p.MouseConfiguration.AdvancedView)
                GUIController.doShowAdvancedView();
        }


        void InputListener.doMouseButtonReleased(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.AdvancedView)
                GUIController.doHideAdvancedView();
        }


        void InputListener.doMouseScrolled(PlayerIndex inputIndex, int delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            SimPlayersController.doMouseScrolled(p, delta);
        }


        void InputListener.doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ControleurVaisseaux.VaisseauControllablePresent)
                ControleurVaisseaux.NextInputVaisseau = delta;

            SimPlayersController.doMouseMoved(p, ref delta);
        }


        void InputListener.doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ControleurVaisseaux.VaisseauCollecteurActif && button == p.GamePadConfiguration.Cancel)
                ControleurVaisseaux.VaisseauCollecteurActif = false;

            SimPlayersController.doGamePadButtonPressedOnce(p, button);

            if (this.Debug && button == p.GamePadConfiguration.Debug)
                ControleurCollisions.Debug = true;

            if (button == p.GamePadConfiguration.AdvancedView)
                GUIController.doShowAdvancedView();
        }


        void InputListener.doGamePadButtonReleased(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.AdvancedView)
                GUIController.doHideAdvancedView();

            if (this.Debug && button == p.GamePadConfiguration.Debug)
                ControleurCollisions.Debug = false;
        }


        void InputListener.doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ControleurVaisseaux.VaisseauControllablePresent && button == p.GamePadConfiguration.PilotSpaceShip)
                ControleurVaisseaux.NextInputVaisseau = delta * p.GamePadConfiguration.Speed;

            SimPlayersController.doGamePadJoystickMoved(p, button, ref delta);
        }

        #endregion
    }
}