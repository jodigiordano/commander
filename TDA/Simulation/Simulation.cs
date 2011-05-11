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
    delegate void TurretHandler(Turret turret);


    class Simulation : InputListener
    {
        public Scene Scene;
        public Main Main;
        public ScenarioDescriptor DemoModeSelectedScenario;
        public ScenarioDescriptor DescriptionScenario;
        public bool Debug;
        public Dictionary<PlayerIndex, Player> Players;
        public GameAction GameAction;
        public TurretsFactory TurretsFactory;
        public PowerUpsFactory PowerUpsFactory;
        public EnemiesFactory EnemiesFactory;
        public MineralsFactory MineralsFactory;
        public RectanglePhysique Terrain;
        public GameState Etat { get { return ScenarioController.State; } set { ScenarioController.State = value; } }
        public bool HelpMode { get { return ScenarioController.Help.Active; } }
        public CorpsCeleste CelestialBodyPausedGame;
        public bool WorldMode;
        public Vector3 PositionCurseur; 

        public PlanetarySystemController ControleurSystemePlanetaire;
        public ControleurMessages ControleurMessages;
        private ScenarioController ScenarioController;
        private ControleurEnnemis ControleurEnnemis;
        private ControleurProjectiles ControleurProjectiles;
        private ControleurCollisions ControleurCollisions;
        private SimPlayersController SimPlayersController;
        private TurretsController TurretsController;
        private ControleurVaisseaux ControleurVaisseaux;   
        private GUIController GUIController;
        private bool modeDemo = false;
        private bool modeEditeur = false;


        public Simulation(Main main, Scene scene, ScenarioDescriptor scenario)
        {
            Scene = scene;
            Main = main;
            DescriptionScenario = scenario;
            TurretsFactory = new TurretsFactory(this);
            PowerUpsFactory = new PowerUpsFactory(this);
            EnemiesFactory = new EnemiesFactory(this);
            MineralsFactory = new MineralsFactory(this);

#if DEBUG
            this.Debug = true;
#else
            this.Debug = false;
#endif
        }


        public void Initialize()
        {
            Scene.Particules.Add(
                new List<string>() {
                    "projectileMissileDeplacement",
                    "projectileBaseExplosion",
                    "etoile",
                    "etoileBleue",
                    "planeteGazeuse",
                    "etoilesScintillantes",
                    "projectileMissileExplosion",
                    "projectileLaserSimple",
                    "projectileLaserMultiple",
                    "selectionCorpsCeleste",
                    "traineeMissile",
                    "etincelleLaser",
                    "toucherTerre",
                    "anneauTerreMeurt",
                    "bouleTerreMeurt",
                    "missileAlien",
                    "implosionAlien",
                    "explosionEnnemi",
                    "mineral1",
                    "mineral2",
                    "mineral3",
                    "mineralPointsVie",
                    "mineralPris",
                    "etincelleMissile",
                    "etincelleLaserSimple",
                    "etincelleSlowMotion",
                    "etincelleSlowMotionTouche",
                    "etoileFilante",
                    "trouRose",
                    "boosterTurret"
                }, false);

            DemoModeSelectedScenario = new ScenarioDescriptor();
            WorldMode = false;
            GameAction = GameAction.None;
            Terrain = new RectanglePhysique(-840, -560, 1680, 1120);

            ControleurCollisions = new ControleurCollisions(this);
            ControleurProjectiles = new ControleurProjectiles(this);
            ControleurEnnemis = new ControleurEnnemis(this);
            SimPlayersController = new SimPlayersController(this);
            TurretsController = new TurretsController(this);
            ControleurSystemePlanetaire = new PlanetarySystemController(this);
            ScenarioController = new ScenarioController(this, new Scenario(this, DescriptionScenario));
            ControleurVaisseaux = new ControleurVaisseaux(this);
            ControleurMessages = new ControleurMessages(this);
            GUIController = new GUIController(this);

            TurretsFactory.Availables = ScenarioController.AvailableTurrets;
            PowerUpsFactory.Availables = ScenarioController.AvailablePowerUps;

            ControleurCollisions.Projectiles = ControleurProjectiles.Projectiles;
            ControleurCollisions.Ennemis = ControleurEnnemis.Ennemis;
            SimPlayersController.CelestialBodies = ScenarioController.CelestialBodies;
            ControleurSystemePlanetaire.CelestialBodies = ScenarioController.CelestialBodies;
            TurretsController.PlanetarySystemController = ControleurSystemePlanetaire;
            ControleurEnnemis.VaguesInfinies = ScenarioController.InfiniteWaves;
            ControleurEnnemis.Vagues = ScenarioController.Waves;
            ControleurCollisions.Tourelles = TurretsController.Turrets;
            SimPlayersController.CommonStash = ScenarioController.CommonStash;
            SimPlayersController.CelestialBodyToProtect = ScenarioController.CelestialBodyToProtect;
            GUIController.Path = ControleurSystemePlanetaire.Path;
            GUIController.PathPreview = ControleurSystemePlanetaire.PathPreview;
            ControleurEnnemis.CheminProjection = ControleurSystemePlanetaire.PathPreview;
            TurretsController.StartingTurrets = ScenarioController.StartingTurrets;
            ControleurEnnemis.Chemin = ControleurSystemePlanetaire.Path;
            ControleurCollisions.CorpsCelestes = ScenarioController.CelestialBodies;
            SimPlayersController.AvailableSpaceships = ControleurVaisseaux.AvailableSpaceships;
            ControleurCollisions.Mineraux = ControleurEnnemis.Mineraux;
            ControleurEnnemis.ValeurTotalMineraux = ScenarioController.Scenario.MineralsValue;
            ControleurEnnemis.PourcentageMinerauxDonnes = ScenarioController.Scenario.MineralsPercentages;
            ControleurEnnemis.NbPackViesDonnes = ScenarioController.Scenario.LifePacks;
            ControleurVaisseaux.Ennemis = ControleurEnnemis.Ennemis;
            ControleurMessages.Tourelles = TurretsController.Turrets;
            ControleurMessages.CorpsCelesteAProteger = ScenarioController.CelestialBodyToProtect;
            ControleurMessages.CorpsCelestes = ScenarioController.CelestialBodies;
            ControleurMessages.Chemin = ControleurSystemePlanetaire.Path;
            ControleurMessages.Sablier = GUIController.SandGlass;
            GUIController.CompositionNextWave = ControleurEnnemis.CompositionProchaineVague;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = ControleurSystemePlanetaire.CelestialBodies;
            GUIController.Scenario = ScenarioController.Scenario;
            GUIController.Enemies = ControleurEnnemis.Ennemis;
            SimPlayersController.InitialPlayerPosition = PositionCurseur;
            GUIController.InfiniteWaves = ScenarioController.InfiniteWaves;
            GUIController.Waves = ScenarioController.Waves;
            GUIController.DemoModeSelectedScenario = this.DemoModeSelectedScenario;
            ControleurVaisseaux.HumanBattleship = GUIController.HumanBattleship;


            ControleurCollisions.ObjetTouche += new ControleurCollisions.ObjetToucheHandler(ControleurEnnemis.doObjetTouche);
            SimPlayersController.AchatTourelleDemande += new TurretHandler(TurretsController.DoBuyTurret);
            ControleurEnnemis.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ScenarioController.doWaveEnded);
            ControleurEnnemis.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            ControleurCollisions.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(TurretsController.DoInRangeTurret);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            ControleurProjectiles.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doWaveStarted);
            SimPlayersController.VenteTourelleDemande += new TurretHandler(TurretsController.DoSellTurret);
            TurretsController.TurretSold += new TurretHandler(SimPlayersController.doTourelleVendue);
            TurretsController.TurretBought += new TurretHandler(SimPlayersController.doTourelleAchetee);
            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ScenarioController.doEnemyReachedEnd);
            SimPlayersController.AchatCollecteurDemande += new NoneHandler(ControleurVaisseaux.doAcheterCollecteur);
            TurretsController.TurretUpdated += new TurretHandler(SimPlayersController.doTourelleMiseAJour);
            SimPlayersController.MiseAJourTourelleDemande += new TurretHandler(TurretsController.DoUpgradeTurret);
            ControleurSystemePlanetaire.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            ControleurSystemePlanetaire.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.ProchaineVagueDemandee += new NoneHandler(ControleurEnnemis.doProchaineVagueDemandee);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            SimPlayersController.AchatDoItYourselfDemande += new NoneHandler(ControleurVaisseaux.doAcheterDoItYourself);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.doObjetDetruit);
            SimPlayersController.DestructionCorpsCelesteDemande += new PhysicalObjectHandler(ControleurSystemePlanetaire.DoDestroyCelestialBody);
            ControleurSystemePlanetaire.ObjectDestroyed += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            SimPlayersController.AchatTheResistanceDemande += new NoneHandler(ControleurVaisseaux.doAcheterTheResistance);
            ControleurSystemePlanetaire.ObjectDestroyed += new PhysicalObjectHandler(ScenarioController.doObjectDestroyed);

            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(this.doEnnemiAtteintFinTrajet);
            ControleurSystemePlanetaire.ObjectDestroyed += new PhysicalObjectHandler(this.doCorpsCelesteDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doNextWave);
            SimPlayersController.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);
            ScenarioController.NewGameState += new NewGameStateHandler(GUIController.doGameStateChanged);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.doPlayerSelectionChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(this.doPlayerSelectionChanged);
            TurretsController.TurretReactivated += new TurretHandler(SimPlayersController.doTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.doPlayerMoved);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(SimPlayersController.doObjetCree);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(GUIController.doObjectCreated);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);
            ScenarioController.NewGameState += new NewGameStateHandler(this.doNewGameState); //must come after GUIController.doGameStateChanged
            ScenarioController.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);

            SimPlayersController.TurretToPlaceSelected += new TurretHandler(GUIController.doTurretToPlaceSelected);
            SimPlayersController.TurretToPlaceDeselected += new TurretHandler(GUIController.doTurretToPlaceDeselected);
            SimPlayersController.AchatTourelleDemande += new TurretHandler(GUIController.doTurretBought);
            SimPlayersController.VenteTourelleDemande += new TurretHandler(GUIController.doTurretSold);

            SimPlayersController.Initialize();
            ControleurEnnemis.Initialize();
            ControleurProjectiles.Initialize();
            TurretsController.Initialize();
            ControleurSystemePlanetaire.Initialize();
            ControleurCollisions.Initialize();
            ControleurVaisseaux.Initialize();
            ControleurMessages.Initialize();
            GUIController.Initialize();

            ModeDemo = modeDemo;
            ModeEditeur = modeEditeur;
        }


        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                SimPlayersController.ModeDemo = value;
                ScenarioController.DemoMode = value;
                TurretsController.DemoMode = value;
                ControleurSystemePlanetaire.DemoMode = value;
            }
        }


        public bool ModeEditeur
        {
            get { return modeEditeur; }
            set
            {
                modeEditeur = value;

                ScenarioController.EditorMode = value;
            }
        }


        public CorpsCeleste CorpsCelesteSelectionne
        {
            get
            {
                return SimPlayersController.CelestialBodySelected;
            }
        }


        public void Update(GameTime gameTime)
        {
            ScenarioController.Update(gameTime);

            if (ScenarioController.Help.Active)
                return;

            if (Etat == GameState.Paused)
                return;

            ControleurCollisions.Update(gameTime);
            ControleurProjectiles.Update(gameTime);
            ControleurSystemePlanetaire.Update(gameTime); // must be done before the TurretController because the celestial bodies must move before the turrets
            TurretsController.Update(gameTime); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
            ControleurEnnemis.Update(gameTime);
            SimPlayersController.Update(gameTime);
            ControleurVaisseaux.Update(gameTime);
            ControleurMessages.Update(gameTime);
            GUIController.Update(gameTime);
        }


        public void Draw()
        {
            ControleurCollisions.Draw(null);
            ControleurProjectiles.Draw(null);
            ControleurEnnemis.Draw(null);
            TurretsController.Draw();
            ControleurSystemePlanetaire.Draw();
            ScenarioController.Draw(null);
            ControleurVaisseaux.Draw(null);
            ControleurMessages.Draw(null);
            GUIController.Draw();
            SimPlayersController.Draw();
        }


        public void EtreNotifierNouvelEtatPartie(NewGameStateHandler handler)
        {
            ScenarioController.NewGameState += handler;
        }


        private void doPlayerSelectionChanged(SimPlayer player)
        {
            GameAction = player.ActualSelection.GameAction;
        }


        private void doNewGameState(GameState state)
        {
            if (state == GameState.Won || state == GameState.Lost)
            {
                ControleurEnnemis.ObjetDetruit -= SimPlayersController.doObjetDetruit;

                int scenario = ScenarioController.Scenario.Id;
                int score = ScenarioController.Scenario.CommonStash.TotalScore;

                if (!Main.SaveGame.HighScores.ContainsKey(scenario))
                    Main.SaveGame.HighScores.Add(scenario, new HighScores(scenario));

                Main.SaveGame.HighScores[scenario].Add(Main.PlayersController.MasterPlayer.Name, score);

                EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
            }
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

            if (Debug && key == p.KeyboardConfiguration.Debug)
                ControleurCollisions.Debug = true;

            if (ScenarioController.Help.Active)
            {
                if (key == p.KeyboardConfiguration.Cancel)
                    ScenarioController.Help.Skip();
                if (key == p.KeyboardConfiguration.Next)
                    ScenarioController.Help.NextDirective();
                if (key == p.KeyboardConfiguration.Back)
                    ScenarioController.Help.PreviousDirective();

                return;
            }

            if (!ModeDemo && (key == p.KeyboardConfiguration.Back || key == p.KeyboardConfiguration.Cancel))
            {
                Etat = GameState.Paused;
                ScenarioController.TriggerNewGameState(Etat);
            }
        }


        void InputListener.doKeyReleased(PlayerIndex inputIndex, Keys key)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (this.Debug && key == p.KeyboardConfiguration.Debug)
                ControleurCollisions.Debug = false;

            if (ScenarioController.Help.Active)
                return;
        }


        void InputListener.doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
            {
                if (button == p.MouseConfiguration.Select)
                    ScenarioController.Help.NextDirective();
                else if (button == p.MouseConfiguration.Back)
                    ScenarioController.Help.PreviousDirective();

                return;
            }

            if (ControleurVaisseaux.VaisseauCollecteurActif && button == p.MouseConfiguration.Cancel)
                ControleurVaisseaux.VaisseauCollecteurActif = false;

            SimPlayersController.doMouseButtonPressedOnce(p, button);

            if (!ModeDemo && button == p.MouseConfiguration.AdvancedView)
                GUIController.doShowAdvancedView();
        }


        void InputListener.doMouseButtonReleased(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
                return;

            if (!ModeDemo && button == p.MouseConfiguration.AdvancedView)
                GUIController.doHideAdvancedView();
        }


        void InputListener.doMouseScrolled(PlayerIndex inputIndex, int delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
                return;

            SimPlayersController.doMouseScrolled(p, delta);
        }


        void InputListener.doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
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

            if (Debug && button == p.GamePadConfiguration.Debug)
                ControleurCollisions.Debug = true;

            if (ScenarioController.Help.Active)
            {
                if (button == p.GamePadConfiguration.SelectionNext)
                    ScenarioController.Help.NextDirective();
                else if (button == p.GamePadConfiguration.SelectionPrevious)
                    ScenarioController.Help.PreviousDirective();
                else if (button == p.GamePadConfiguration.Cancel)
                    ScenarioController.Help.Skip();

                return;
            }

            if (ControleurVaisseaux.VaisseauCollecteurActif && button == p.GamePadConfiguration.Cancel)
                ControleurVaisseaux.VaisseauCollecteurActif = false;

            SimPlayersController.doGamePadButtonPressedOnce(p, button);

            if (button == p.GamePadConfiguration.AdvancedView)
                GUIController.doShowAdvancedView();
        }


        void InputListener.doGamePadButtonReleased(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (Debug && button == p.GamePadConfiguration.Debug)
                ControleurCollisions.Debug = false;

            if (ScenarioController.Help.Active)
                return;

            if (button == p.GamePadConfiguration.AdvancedView)
                GUIController.doHideAdvancedView();
        }


        void InputListener.doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
                return;

            if (ControleurVaisseaux.VaisseauControllablePresent && button == p.GamePadConfiguration.PilotSpaceShip)
                ControleurVaisseaux.NextInputVaisseau = delta * p.GamePadConfiguration.Speed;

            SimPlayersController.doGamePadJoystickMoved(p, button, ref delta);
        }

        #endregion
    }
}