namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    delegate void PhysicalObjectHandler(IObjetPhysique obj);
    delegate void SimPlayerHandler(SimPlayer player);
    delegate void CommonStashHandler(CommonStash stash);
    delegate void CelestialObjectHandler(CorpsCeleste celestialObject);
    delegate void NewGameStateHandler(GameState state);

    class Simulation : InputListener
    {
        public Scene Scene;
        public Main Main;
        public DescripteurScenario DemoModeSelectedScenario;
        public DescripteurScenario DescriptionScenario;
        public bool Debug;
        public Dictionary<PlayerIndex, Player> Players;

        private ScenarioController ControleurScenario;
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
        public GameState Etat                      { get { return ControleurScenario.State; } }

        private bool modeDemo = false;
        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                SimPlayersController.ModeDemo = value;
                ControleurScenario.DemoMode = value;
            }
        }


        private bool modeEditeur = false;
        public bool ModeEditeur
        {
            get { return modeEditeur; }
            set
            {
                modeEditeur = value;

                ControleurScenario.EditorMode = value;
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
            ControleurScenario = new ScenarioController(this, new Scenario(this, DescriptionScenario));
            ControleurVaisseaux = new ControleurVaisseaux(this);
            ControleurMessages = new ControleurMessages(this);
            GUIController = new GUIController(this);


            ControleurCollisions.Projectiles = ControleurProjectiles.Projectiles;
            ControleurCollisions.Ennemis = ControleurEnnemis.Ennemis;
            SimPlayersController.CelestialBodies = ControleurScenario.CelestialBodies;
            ControleurSystemePlanetaire.CorpsCelestes = ControleurScenario.CelestialBodies;
            ControleurTourelles.ControleurSystemePlanetaire = ControleurSystemePlanetaire;
            ControleurEnnemis.VaguesInfinies = ControleurScenario.InfiniteWaves;
            ControleurEnnemis.Vagues = ControleurScenario.Waves;
            ControleurCollisions.Tourelles = ControleurTourelles.Tourelles;
            SimPlayersController.CommonStash = ControleurScenario.CommonStash;
            SimPlayersController.CelestialBodyToProtect = ControleurScenario.CelestialBodyToProtect;
            GUIController.Path = ControleurSystemePlanetaire.Chemin;
            GUIController.PathPreview = ControleurSystemePlanetaire.CheminProjection;
            ControleurEnnemis.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurTourelles.TourellesDepart = ControleurScenario.StartingTurrets;
            ControleurEnnemis.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurCollisions.CorpsCelestes = ControleurScenario.CelestialBodies;
            SimPlayersController.AvailableSpaceships = ControleurVaisseaux.OptionsDisponibles;
            ControleurCollisions.Mineraux = ControleurEnnemis.Mineraux;
            ControleurEnnemis.ValeurTotalMineraux = ControleurScenario.Scenario.ValeurMinerauxDonnes;
            ControleurEnnemis.PourcentageMinerauxDonnes = ControleurScenario.Scenario.PourcentageMinerauxDonnes;
            ControleurEnnemis.NbPackViesDonnes = ControleurScenario.Scenario.NbPackViesDonnes;
            ControleurVaisseaux.Ennemis = ControleurEnnemis.Ennemis;
            ControleurMessages.Tourelles = ControleurTourelles.Tourelles;
            //ControleurMessages.BulleGUI = PlayerController.BulleGUI;
            ControleurMessages.CorpsCelesteAProteger = ControleurScenario.CelestialBodyToProtect;
            ControleurMessages.CorpsCelestes = ControleurScenario.CelestialBodies;
            ControleurMessages.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurMessages.Sablier = GUIController.SandGlass;
            GUIController.CompositionNextWave = ControleurEnnemis.CompositionProchaineVague;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = ControleurSystemePlanetaire.CorpsCelestes;
            GUIController.Scenario = ControleurScenario.Scenario;
            GUIController.Enemies = ControleurEnnemis.Ennemis;
            //ControleurMessages.Curseur = GUIController.Cursor;
            SimPlayersController.InitialPlayerPosition = PositionCurseur;
            GUIController.InfiniteWaves = ControleurScenario.InfiniteWaves;
            GUIController.Waves = ControleurScenario.Waves;
            GUIController.DemoModeSelectedScenario = this.DemoModeSelectedScenario;


            ControleurCollisions.ObjetTouche += new ControleurCollisions.ObjetToucheHandler(ControleurEnnemis.doObjetTouche);
            SimPlayersController.AchatTourelleDemande += new SimPlayersController.TurretTypeCelestialObjectTurretSpotHandler(ControleurTourelles.doAcheterTourelle);
            ControleurEnnemis.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ControleurScenario.doWaveEnded);
            ControleurEnnemis.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            ControleurCollisions.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(ControleurTourelles.doDansZoneActivationTourelle);
            ControleurTourelles.ObjectCreated += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurSystemePlanetaire.doObjetCree);
            ControleurProjectiles.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doWaveStarted);
            SimPlayersController.VenteTourelleDemande += new SimPlayersController.CelestialObjectTurretSpotHandler(ControleurTourelles.doVendreTourelle);
            ControleurTourelles.TurretSold += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleVendue);
            ControleurTourelles.TurretBought += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleAchetee);
            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ControleurScenario.doEnemyReachedEnd);
            SimPlayersController.AchatCollecteurDemande += new CelestialObjectHandler(ControleurVaisseaux.doAcheterCollecteur);
            ControleurTourelles.TurretUpdated += new ControleurTourelles.TurretHandler(SimPlayersController.doTourelleMiseAJour);
            SimPlayersController.MiseAJourTourelleDemande += new SimPlayersController.CelestialObjectTurretSpotHandler(ControleurTourelles.doMettreAJourTourelle);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurTourelles.doObjetDetruit);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.ProchaineVagueDemandee += new NoneHandler(ControleurEnnemis.doProchaineVagueDemandee);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            SimPlayersController.AchatDoItYourselfDemande += new CelestialObjectHandler(ControleurVaisseaux.doAcheterDoItYourself);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.DestructionCorpsCelesteDemande += new CelestialObjectHandler(ControleurSystemePlanetaire.doDetruireCorpsCeleste);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            SimPlayersController.AchatTheResistanceDemande += new CelestialObjectHandler(ControleurVaisseaux.doAcheterTheResistance);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurScenario.doObjectDestroyed);

            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(this.doEnnemiAtteintFinTrajet);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(this.doCorpsCelesteDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doNextWave);
            SimPlayersController.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);
            ControleurScenario.NewGameState += new NewGameStateHandler(GUIController.doGameStateChanged);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.doPlayerSelectionChanged);
            ControleurTourelles.TurretReactivated += new ControleurTourelles.TurretHandler(SimPlayersController.doTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.doPlayerMoved);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(SimPlayersController.doObjetCree);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(GUIController.doObjectCreated);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);
            ControleurScenario.NewGameState += new NewGameStateHandler(this.doNewGameState);
            ControleurScenario.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);

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


        public void EtreNotifierNouvelEtatPartie(NewGameStateHandler handler)
        {
            ControleurScenario.NewGameState += handler;
        }


        private void doNewGameState(GameState state)
        {
            ControleurEnnemis.ObjetDetruit -= SimPlayersController.doObjetDetruit;

            int scenario = ControleurScenario.Scenario.Numero;
            int score = ControleurScenario.Scenario.CommonStash.TotalScore;

            if (!Main.SaveGame.HighScores.ContainsKey(scenario))
                Main.SaveGame.HighScores.Add(scenario, new HighScores(scenario));

            Main.SaveGame.HighScores[scenario].Add(Main.PlayersController.MasterPlayer.Name, score);

            EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
        }


        private void doEnnemiAtteintFinTrajet(Ennemi ennemi, CorpsCeleste corpsCeleste)
        {
            if (Etat == GameState.Won)
                return;

            if (!this.ModeDemo && this.Etat != GameState.Lost)
            {
                foreach (var joueur in this.Main.Players.Values)
                    EphemereGames.Core.Input.Facade.VibrateController(joueur.Index, 300, 0.5f, 0.5f);
            }
        }


        private void doCorpsCelesteDetruit(IObjetPhysique objet)
        {
            foreach (var joueur in this.Main.Players.Values)
                EphemereGames.Core.Input.Facade.VibrateController(joueur.Index, 300, 0.5f, 0.5f);
        }


        #region InputListener Membres

        bool InputListener.Active
        {
            get { return Etat == GameState.Running; }
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