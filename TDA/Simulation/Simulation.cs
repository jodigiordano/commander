namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    delegate void PhysicalObjectHandler(IObjetPhysique obj);
    delegate void PhysicalObjectPhysicalObjectHandler(IObjetPhysique obj1, IObjetPhysique obj2);
    delegate void SimPlayerHandler(SimPlayer player);
    delegate void CommonStashHandler(CommonStash stash);
    delegate void CelestialObjectHandler(CorpsCeleste celestialObject);
    delegate void NewGameStateHandler(GameState state);
    delegate void TurretHandler(Turret turret);
    delegate void PowerUpTypeHandler(PowerUpType powerUp);
    delegate void PowerUpHandler(PowerUp powerUp);
    delegate void TurretTurretHandler(Turret turret1, Turret turret2);


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

        public PlanetarySystemController PlanetarySystemController;
        public ControleurMessages MessagesController;
        private ScenarioController ScenarioController;
        private ControleurEnnemis EnemiesController;
        private ControleurProjectiles BulletsController;
        private ControleurCollisions CollisionsController;
        private SimPlayersController SimPlayersController;
        private TurretsController TurretsController;
        private SpaceshipsController SpaceshipsController;   
        private GUIController GUIController;
        private PowerUpsController PowerUpsController;
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
                    "boosterTurret",
                    "gunnerTurret"
                }, false);

            DemoModeSelectedScenario = new ScenarioDescriptor();
            WorldMode = false;
            GameAction = GameAction.None;
            Terrain = new RectanglePhysique(-840, -560, 1680, 1120);

            CollisionsController = new ControleurCollisions(this);
            BulletsController = new ControleurProjectiles(this);
            EnemiesController = new ControleurEnnemis(this);
            SimPlayersController = new SimPlayersController(this);
            TurretsController = new TurretsController(this);
            PlanetarySystemController = new PlanetarySystemController(this);
            ScenarioController = new ScenarioController(this, new Scenario(this, DescriptionScenario));
            SpaceshipsController = new SpaceshipsController(this);
            MessagesController = new ControleurMessages(this);
            GUIController = new GUIController(this);
            PowerUpsController = new PowerUpsController(this);

            TurretsFactory.Availables = ScenarioController.AvailableTurrets;
            PowerUpsFactory.Availables = ScenarioController.AvailablePowerUps;

            CollisionsController.Projectiles = BulletsController.Projectiles;
            CollisionsController.Ennemis = EnemiesController.Ennemis;
            SimPlayersController.CelestialBodies = ScenarioController.CelestialBodies;
            PlanetarySystemController.CelestialBodies = ScenarioController.CelestialBodies;
            TurretsController.PlanetarySystemController = PlanetarySystemController;
            EnemiesController.VaguesInfinies = ScenarioController.InfiniteWaves;
            EnemiesController.Vagues = ScenarioController.Waves;
            CollisionsController.Tourelles = TurretsController.Turrets;
            SimPlayersController.CommonStash = ScenarioController.CommonStash;
            SimPlayersController.CelestialBodyToProtect = ScenarioController.CelestialBodyToProtect;
            GUIController.Path = PlanetarySystemController.Path;
            GUIController.PathPreview = PlanetarySystemController.PathPreview;
            EnemiesController.CheminProjection = PlanetarySystemController.PathPreview;
            TurretsController.StartingTurrets = ScenarioController.StartingTurrets;
            EnemiesController.Chemin = PlanetarySystemController.Path;
            CollisionsController.CorpsCelestes = ScenarioController.CelestialBodies;
            CollisionsController.Mineraux = EnemiesController.Mineraux;
            EnemiesController.ValeurTotalMineraux = ScenarioController.Scenario.MineralsValue;
            EnemiesController.PourcentageMinerauxDonnes = ScenarioController.Scenario.MineralsPercentages;
            EnemiesController.NbPackViesDonnes = ScenarioController.Scenario.LifePacks;
            SpaceshipsController.Ennemis = EnemiesController.Ennemis;
            MessagesController.Tourelles = TurretsController.Turrets;
            MessagesController.CorpsCelesteAProteger = ScenarioController.CelestialBodyToProtect;
            MessagesController.CorpsCelestes = ScenarioController.CelestialBodies;
            MessagesController.Chemin = PlanetarySystemController.Path;
            MessagesController.Sablier = GUIController.SandGlass;
            GUIController.CompositionNextWave = EnemiesController.CompositionProchaineVague;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = PlanetarySystemController.CelestialBodies;
            GUIController.Scenario = ScenarioController.Scenario;
            GUIController.Enemies = EnemiesController.Ennemis;
            SimPlayersController.InitialPlayerPosition = PositionCurseur;
            GUIController.InfiniteWaves = ScenarioController.InfiniteWaves;
            GUIController.Waves = ScenarioController.Waves;
            GUIController.DemoModeSelectedScenario = this.DemoModeSelectedScenario;
            SpaceshipsController.HumanBattleship = GUIController.HumanBattleship;
            PowerUpsFactory.HumanBattleship = GUIController.HumanBattleship;
            SimPlayersController.ActivesPowerUps = PowerUpsController.ActivesPowerUps;


            CollisionsController.ObjetTouche += new PhysicalObjectPhysicalObjectHandler(EnemiesController.doObjetTouche);
            SimPlayersController.AchatTourelleDemande += new TurretHandler(TurretsController.DoBuyTurret);
            EnemiesController.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ScenarioController.doWaveEnded);
            EnemiesController.ObjetDetruit += new PhysicalObjectHandler(SimPlayersController.DoObjetDetruit);
            CollisionsController.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(TurretsController.DoInRangeTurret);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.doObjetCree);
            BulletsController.ObjetDetruit += new PhysicalObjectHandler(CollisionsController.doObjetDetruit);
            EnemiesController.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doWaveStarted);
            SimPlayersController.VenteTourelleDemande += new TurretHandler(TurretsController.DoSellTurret);
            TurretsController.TurretSold += new TurretHandler(SimPlayersController.DoTourelleVendue);
            TurretsController.TurretBought += new TurretHandler(SimPlayersController.DoTourelleAchetee);
            EnemiesController.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ScenarioController.doEnemyReachedEnd);
            TurretsController.TurretUpdated += new TurretHandler(SimPlayersController.DoTourelleMiseAJour);
            SimPlayersController.MiseAJourTourelleDemande += new TurretHandler(TurretsController.DoUpgradeTurret);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjetDetruit);
            SimPlayersController.ProchaineVagueDemandee += new NoneHandler(EnemiesController.doProchaineVagueDemandee);
            SpaceshipsController.ObjetCree += new PhysicalObjectHandler(BulletsController.doObjetCree);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.doObjetDetruit);
            SpaceshipsController.ObjetCree += new PhysicalObjectHandler(CollisionsController.doObjetCree);
            EnemiesController.ObjetCree += new PhysicalObjectHandler(CollisionsController.doObjetCree);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(ScenarioController.doObjectDestroyed);
            EnemiesController.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(this.doEnnemiAtteintFinTrajet);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(this.doCorpsCelesteDetruit);
            EnemiesController.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doNextWave);
            SimPlayersController.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);
            ScenarioController.NewGameState += new NewGameStateHandler(GUIController.doGameStateChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.doPlayerSelectionChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(this.doPlayerSelectionChanged);
            TurretsController.TurretReactivated += new TurretHandler(SimPlayersController.DoTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.doPlayerMoved);
            ScenarioController.NewGameState += new NewGameStateHandler(this.doNewGameState); //must come after GUIController.doGameStateChanged
            ScenarioController.CommonStashChanged += new CommonStashHandler(GUIController.doCommonStashChanged);
            SimPlayersController.TurretToPlaceSelected += new TurretHandler(GUIController.doTurretToPlaceSelected);
            SimPlayersController.TurretToPlaceDeselected += new TurretHandler(GUIController.doTurretToPlaceDeselected);
            SimPlayersController.AchatTourelleDemande += new TurretHandler(GUIController.doTurretBought);
            SimPlayersController.VenteTourelleDemande += new TurretHandler(GUIController.doTurretSold);
            SimPlayersController.BuyAPowerUpAsked += new PowerUpTypeHandler(PowerUpsController.DoBuyAPowerUpAsked);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(SpaceshipsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(SpaceshipsController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(GUIController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(GUIController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(SimPlayersController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(SimPlayersController.DoPowerUpStopped);
            CollisionsController.TurretBoosted += new TurretTurretHandler(TurretsController.DoTurretBoosted);

            SimPlayersController.Initialize();
            EnemiesController.Initialize();
            BulletsController.Initialize();
            TurretsController.Initialize();
            PlanetarySystemController.Initialize();
            CollisionsController.Initialize();
            MessagesController.Initialize();
            GUIController.Initialize();
            PowerUpsController.Initialize();

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
                PlanetarySystemController.DemoMode = value;
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

            CollisionsController.Update(gameTime);
            BulletsController.Update(gameTime);
            PlanetarySystemController.Update(gameTime); // must be done before the TurretController because the celestial bodies must move before the turrets
            TurretsController.Update(gameTime); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
            EnemiesController.Update(gameTime);
            SimPlayersController.Update(gameTime);
            SpaceshipsController.Update();
            MessagesController.Update(gameTime);
            GUIController.Update(gameTime);
            PowerUpsController.Update();
        }


        public void Draw()
        {
            CollisionsController.Draw(null);
            BulletsController.Draw(null);
            EnemiesController.Draw(null);
            TurretsController.Draw();
            PlanetarySystemController.Draw();
            ScenarioController.Draw(null);
            SpaceshipsController.Draw();
            MessagesController.Draw(null);
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
                EnemiesController.ObjetDetruit -= SimPlayersController.DoObjetDetruit;

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
                CollisionsController.Debug = true;

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
                CollisionsController.Debug = false;

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

            if (SpaceshipsController.InCollector && button == p.MouseConfiguration.Cancel)
                SpaceshipsController.InCollector = false;

            SimPlayersController.DoMouseButtonPressedOnce(p, button);

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

            SimPlayersController.DoMouseScrolled(p, delta);
        }


        void InputListener.doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (ScenarioController.Help.Active)
                return;

            if (SpaceshipsController.InControllableSpaceship)
                SpaceshipsController.NextInput = delta;

            SimPlayersController.DoMouseMoved(p, ref delta);
        }


        void InputListener.doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (Debug && button == p.GamePadConfiguration.Debug)
                CollisionsController.Debug = true;

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

            if (SpaceshipsController.InCollector && button == p.GamePadConfiguration.Cancel)
                SpaceshipsController.InCollector = false;

            SimPlayersController.DoGamePadButtonPressedOnce(p, button);

            if (button == p.GamePadConfiguration.AdvancedView)
                GUIController.doShowAdvancedView();
        }


        void InputListener.doGamePadButtonReleased(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Players[inputIndex];

            if (!p.Master)
                return;

            if (Debug && button == p.GamePadConfiguration.Debug)
                CollisionsController.Debug = false;

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

            if (SpaceshipsController.InControllableSpaceship && button == p.GamePadConfiguration.PilotSpaceShip)
                SpaceshipsController.NextInput = delta * p.GamePadConfiguration.Speed;

            SimPlayersController.DoGamePadJoystickMoved(p, button, ref delta);
        }

        #endregion
    }
}