namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Simulator : InputListener
    {
        public CommanderScene Scene;
        public Dictionary<CelestialBody, int> AvailableLevelsWorldMode;
        public Dictionary<CelestialBody, int> AvailableWarpsWorldMode;
        public bool DebugMode;
        public bool WorldMode;
        public bool DemoMode;
        public bool CutsceneMode;
        public bool EditorWorldMode;
        public bool CanSelectCelestialBodies;
        
        public bool EditorMode;
        internal EditorState EditorState;
        internal Level Level;
        internal PhysicalRectangle Battlefield;
        internal PhysicalRectangle OuterBattlefield;

        public PausedGameChoice PausedGameChoice;
        public EditorWorldChoice EditorWorldChoice;
        public int NewGameChoice;
        public GameState State { get { return LevelsController.State; } set { LevelsController.State = value; } }


        public PlanetarySystemController PlanetarySystemController;
        public MessagesController MessagesController;
        private LevelsController LevelsController;
        private EnemiesController EnemiesController;
        private BulletsController BulletsController;
        private CollisionsController CollisionsController;
        private SimPlayersController SimPlayersController;
        private TurretsController TurretsController;
        private SpaceshipsController SpaceshipsController;   
        private GUIController GUIController;
        private PowerUpsController PowerUpsController;
        private EditorController EditorController;
        private EditorGUIController EditorGUIController;
        private PanelsController PanelsController;
        internal CameraController CameraController;
        internal TweakingController TweakingController;
        private AudioController AudioController;
        internal TurretsFactory TurretsFactory;
        internal PowerUpsFactory PowerUpsFactory;
        internal EnemiesFactory EnemiesFactory;
        internal MineralsFactory MineralsFactory;
        internal BulletsFactory BulletsFactory;


        public Simulator(CommanderScene scene, LevelDescriptor descriptor)
        {
            Scene = scene;

            Scene.Particles.Add(
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
                    "hoverRectangle",
                    "traineeMissile",
                    "etincelleLaser",
                    "toucherTerre",
                    "anneauTerreMeurt",
                    "anneauTerreMeurt2",
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
                    "gunnerTurret",
                    "nanobots",
                    "nanobots2",
                    "railgun",
                    "railgunExplosion",
                    "pulseEffect",
                    "shieldEffect",
                    "spaceshipTrail",
                    "spaceshipTrail2",
                    "darkSideEffect",
                    "starExplosion",
                    "mothershipMissile",
                    "nextWave",
                    "mothershipAbduction",
                    "love"
                }, true);

            Scene.Particles.SetMaxInstances(@"toucherTerre", 3);

            TurretsFactory = new TurretsFactory(this);
            PowerUpsFactory = new PowerUpsFactory(this);
            EnemiesFactory = new EnemiesFactory(this);
            MineralsFactory = new MineralsFactory(this);
            BulletsFactory = new BulletsFactory(this);

            TurretsFactory.Initialize();
            PowerUpsFactory.Initialize();
            EnemiesFactory.Initialize();
            MineralsFactory.Initialize();
            BulletsFactory.Initialize();

            Level = new Level(this, descriptor);

            TweakingController = new TweakingController(this);
            CollisionsController = new CollisionsController(this);
            BulletsController = new BulletsController(this);
            EnemiesController = new EnemiesController(this);
            SimPlayersController = new SimPlayersController(this);
            TurretsController = new TurretsController(this);
            PlanetarySystemController = new PlanetarySystemController(this);
            SpaceshipsController = new SpaceshipsController(this);
            MessagesController = new MessagesController(this);
            GUIController = new GUIController(this);
            PowerUpsController = new PowerUpsController(this);
            LevelsController = new LevelsController(this);
            EditorController = new EditorController(this);
            EditorGUIController = new EditorGUIController(this);
            PanelsController = new PanelsController(this);
            AudioController = new AudioController(this);
            CameraController = new Simulation.CameraController(this);

            WorldMode = false;
            DemoMode = false;
            CutsceneMode = false;
            EditorMode = false;
            EditorWorldMode = false;
            CanSelectCelestialBodies = true;
            EditorState = EditorState.Editing;
            DebugMode = Preferences.Debug;
            PausedGameChoice = PausedGameChoice.None;
            EditorWorldChoice = EditorWorldChoice.None;
            NewGameChoice = -1;


            CollisionsController.ObjectHit += new PhysicalObjectPhysicalObjectHandler(EnemiesController.DoObjectHit);
            CollisionsController.ObjectHit += new PhysicalObjectPhysicalObjectHandler(BulletsController.DoObjectHit);
            CollisionsController.ObjectOutOfBounds += new PhysicalObjectHandler(BulletsController.DoObjectOutOfBounds);
            SimPlayersController.BuyTurretAsked += new TurretSimPlayerHandler(TurretsController.DoBuyTurret);
            EnemiesController.WaveEnded += new NoneHandler(LevelsController.DoWaveEnded);
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjectDestroyed);
            CollisionsController.InTurretRange += new TurretPhysicalObjectHandler(TurretsController.DoInRangeTurret);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(GUIController.DoWaveStarted);
            SimPlayersController.SellTurretAsked += new TurretSimPlayerHandler(TurretsController.DoSellTurret);
            TurretsController.TurretSold += new TurretSimPlayerHandler(SimPlayersController.DoTurretSold);
            TurretsController.TurretBought += new TurretSimPlayerHandler(SimPlayersController.DoTurretBought);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(PlanetarySystemController.DoEnemyReachedEnd);
            TurretsController.TurretUpgraded += new TurretSimPlayerHandler(SimPlayersController.DoTurretUpgraded);
            TurretsController.TurretUpgraded += new TurretSimPlayerHandler(GUIController.DoTurretUpgraded);
            SimPlayersController.UpgradeTurretAsked += new TurretSimPlayerHandler(TurretsController.DoUpgradeTurret);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjectDestroyed);
            SimPlayersController.NextWaveAsked += new NoneHandler(EnemiesController.DoNextWaveAsked);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(SimPlayersController.DoObjectCreated);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            SpaceshipsController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            EnemiesController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            PlanetarySystemController.ObjectHit += new PhysicalObjectHandler(LevelsController.DoObjectHit);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(LevelsController.DoObjectDestroyed);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(this.DoEnemyReachedEndOfPath);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(this.DoCelestialBodyDestroyed);
            SimPlayersController.CommonStashChanged += new CommonStashHandler(GUIController.DoCommonStashChanged);
            LevelsController.NewGameState += new NewGameStateHandler(GUIController.DoGameStateChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.DoPlayerSelectionChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(this.DoPlayerSelectionChanged);
            TurretsController.TurretReactivated += new TurretHandler(SimPlayersController.DoTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.DoPlayerMoved);
            LevelsController.NewGameState += new NewGameStateHandler(this.DoNewGameState); //must come after GUIController.DoGameStateChanged
            LevelsController.CommonStashChanged += new CommonStashHandler(GUIController.DoCommonStashChanged);
            SimPlayersController.TurretToPlaceSelected += new TurretSimPlayerHandler(GUIController.DoTurretToPlaceSelected);
            SimPlayersController.TurretToPlaceDeselected += new TurretSimPlayerHandler(GUIController.DoTurretToPlaceDeselected);
            SimPlayersController.BuyTurretAsked += new TurretSimPlayerHandler(GUIController.DoTurretBought);
            SimPlayersController.SellTurretAsked += new TurretSimPlayerHandler(GUIController.DoTurretSold);
            SimPlayersController.ActivatePowerUpAsked += new PowerUpTypeSimPlayerHandler(PowerUpsController.DoActivatePowerUpAsked);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(SpaceshipsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(SpaceshipsController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(GUIController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(GUIController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(SimPlayersController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(SimPlayersController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(CollisionsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(BulletsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(BulletsController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(PlanetarySystemController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(PlanetarySystemController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(LevelsController.DoPowerUpStarted);
            CollisionsController.TurretBoosted += new TurretTurretHandler(TurretsController.DoTurretBoosted);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(PowerUpsController.DoPlayerMoved);
            LevelsController.NewGameState += new NewGameStateHandler(PowerUpsController.DoNewGameState);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(SimPlayersController.DoObjectCreated);
            CollisionsController.BulletDeflected += new EnemyBulletHandler(BulletsController.DoBulletDeflected);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            SimPlayersController.DesactivatePowerUpAsked += new PowerUpTypeSimPlayerHandler(PowerUpsController.DoDesactivatePowerUpAsked);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(GUIController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(GUIController.DoPlayerDisconnected);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(PowerUpsController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(PowerUpsController.DoPlayerDisconnected);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(PanelsController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(PanelsController.DoPlayerDisconnected);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(SpaceshipsController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(SpaceshipsController.DoPlayerDisconnected);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(CameraController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(CameraController.DoPlayerDisconnected);
            SimPlayersController.ShowAdvancedViewAsked += new SimPlayerHandler(GUIController.DoShowAdvancedViewAsked);
            SimPlayersController.HideAdvancedViewAsked += new SimPlayerHandler(GUIController.DoHideAdvancedViewAsked);
            CollisionsController.ObjectHit += new PhysicalObjectPhysicalObjectHandler(SimPlayersController.DoObjectHit);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(EditorController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(EditorController.DoPlayerDisconnected);
            EditorController.PlayerConnected += new EditorPlayerHandler(EditorGUIController.DoPlayerConnected);
            EditorController.PlayerDisconnected += new EditorPlayerHandler(EditorGUIController.DoPlayerDisconnected);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(EditorController.DoPlayerMoved);
            EditorController.PlayerChanged += new EditorPlayerHandler(EditorGUIController.DoPlayerChanged);
            EditorController.EditorCommandExecuted += new EditorCommandHandler(PlanetarySystemController.DoEditorCommandExecuted); //must be executed before sync of gui
            EditorController.EditorCommandExecuted += new EditorCommandHandler(EditorGUIController.DoEditorCommandExecuted);
            EditorController.EditorCommandExecuted += new EditorCommandHandler(PowerUpsController.DoEditorCommandExecuted); //must be done before the Players Controller
            EditorController.EditorCommandExecuted += new EditorCommandHandler(SimPlayersController.DoEditorCommandExecuted); //must be done before the GUI
            EditorController.EditorCommandExecuted += new EditorCommandHandler(LevelsController.DoEditorCommandExecuted); // must be donne before the GUI
            EditorController.EditorCommandExecuted += new EditorCommandHandler(GUIController.DoEditorCommandExecuted);
            SimPlayersController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            LevelsController.NewGameState += new NewGameStateHandler(PanelsController.DoGameStateChanged);
            PanelsController.PanelOpened += new NoneHandler(GUIController.DoPanelOpened);
            PanelsController.PanelClosed += new NoneHandler(GUIController.DoPanelClosed);
            PanelsController.PanelOpened += new NoneHandler(AudioController.DoPanelOpened);
            PanelsController.PanelClosed += new NoneHandler(AudioController.DoPanelClosed);
            PanelsController.PanelOpened += new NoneHandler(CameraController.DoPanelOpened);
            PanelsController.PanelClosed += new NoneHandler(CameraController.DoPanelClosed);
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(GUIController.DoObjectDestroyed);
            EnemiesController.NextWaveCompositionChanged += new NextWaveHandler(GUIController.DoNextWaveCompositionChanged);
            LevelsController.NewGameState += new NewGameStateHandler(SimPlayersController.DoNewGameState);
            CollisionsController.PlayersCollided += new SimPlayerSimPlayerHandler(SimPlayersController.DoPlayersCollided);
            CollisionsController.PlayersCollided += new SimPlayerSimPlayerHandler(AudioController.DoPlayersCollided);
            CollisionsController.PlayersCollided += new SimPlayerSimPlayerHandler(GUIController.DoPlayersCollided);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(BulletsController.DoStartingPathCollision);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(GUIController.DoStartingPathCollision);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(AudioController.DoStartingPathCollision);
            CollisionsController.ShieldCollided += new CollidableBulletHandler(SpaceshipsController.DoShieldCollided);
            CollisionsController.ShieldCollided += new CollidableBulletHandler(BulletsController.DoShieldCollided);
            CollisionsController.ShieldCollided += new CollidableBulletHandler(AudioController.DoShieldCollided);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(AudioController.DoWaveStarted);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(AudioController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(AudioController.DoPowerUpStopped);
            TurretsController.TurretBought += new TurretSimPlayerHandler(AudioController.DoTurretBoughtStarted);
            TurretsController.TurretSold += new TurretSimPlayerHandler(AudioController.DoTurretSold);
            TurretsController.TurretFired += new TurretHandler(AudioController.DoTurretFired);
            PowerUpsController.PowerUpInputCanceled += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputCanceled);
            PowerUpsController.PowerUpInputPressed += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputPressed);
            PowerUpsController.PowerUpInputReleased += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputReleased);
            EnemiesController.ObjectHit += new PhysicalObjectHandler(AudioController.DoObjectHit);
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            PlanetarySystemController.ObjectHit += new PhysicalObjectHandler(AudioController.DoObjectHit);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            SimPlayersController.PlayerBounced += new SimPlayerDirectionHandler(AudioController.DoPlayerBounced);
            SimPlayersController.PlayerBounced += new SimPlayerDirectionHandler(GUIController.DoPlayerBounced);
            SimPlayersController.PlayerRotated += new SimPlayerHandler(AudioController.DoPlayerRotated);
            TurretsController.TurretWandered += new TurretHandler(AudioController.DoTurretWandered);
            TurretsController.TurretUpgraded += new TurretSimPlayerHandler(AudioController.DoTurretUpgradingStarted);
            BulletsController.BulletDeflected += new BulletHandler(AudioController.DoBulletDeflected);
            EnemiesController.WaveNearToStart += new NoneHandler(AudioController.DoWaveNearToStart);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(AudioController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(AudioController.DoPlayerDisconnected);
            EnemiesController.WaveEnded += new NoneHandler(AudioController.DoWaveEnded);
            SimPlayersController.PlayerFired += new SimPlayerHandler(AudioController.DoPlayerFired);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(AudioController.DoPlayerSelectionChanged);
            SimPlayersController.PlayerActionRefused += new SimPlayerHandler(AudioController.DoPlayerActionRefused);
            LevelsController.NewGameState += new NewGameStateHandler(AudioController.DoNewGameState);
            EnemiesController.ObjectCreated += new PhysicalObjectHandler(AudioController.DoObjectCreated);

            Main.CheatsController.CheatActivated += new StringHandler(DoCheatActivated);
            Main.Options.ShowHelpBarChanged += new BooleanHandler(DoShowHelpBarChanged);
        }


        public void Initialize()
        {
            State = GameState.Running;

            Scene.Particles.Clear();

            TweakingController.BulletsFactory = BulletsFactory;
            TweakingController.EnemiesFactory = EnemiesFactory;
            TweakingController.MineralsFactory = MineralsFactory;
            TweakingController.PowerUpsFactory = PowerUpsFactory;
            TweakingController.TurretsFactory = TurretsFactory;

            if (EditorMode && EditorState == EditorState.Editing)
                Battlefield = new PhysicalRectangle(-5000, -5000, 10000, 10000);
            else
                Battlefield = Level.Descriptor.GetBoundaries(new Vector3(6 * (int) Size.Big));

            OuterBattlefield = new PhysicalRectangle(Battlefield.X - 200, Battlefield.Y - 200, Battlefield.Width + 400, Battlefield.Height + 400);

            Level.Initialize();

            LevelsController.Level = Level;
            TurretsFactory.Availables = LevelsController.AvailableTurrets;
            PowerUpsFactory.Availables = LevelsController.AvailablePowerUps;
            PowerUpsFactory.HumanBattleship = GUIController.HumanBattleship;
            CollisionsController.Bullets = BulletsController.Bullets;
            CollisionsController.Enemies = EnemiesController.Enemies;
            CollisionsController.Turrets = TurretsController.Turrets;
            CollisionsController.CelestialBodies = LevelsController.CelestialBodies;
            CollisionsController.Minerals = EnemiesController.Minerals;
            CollisionsController.ShootingStars = PlanetarySystemController.ShootingStars;
            CollisionsController.Battlefield = OuterBattlefield;
            SimPlayersController.CelestialBodies = LevelsController.CelestialBodies;
            SimPlayersController.CommonStash = LevelsController.CommonStash;
            SimPlayersController.CelestialBodyToProtect = LevelsController.CelestialBodyToProtect;
            SimPlayersController.StartingPathMenu = GUIController.StartingPathMenu;
            SimPlayersController.ActivesPowerUps = PowerUpsController.ActivesPowerUps;
            PlanetarySystemController.CelestialBodies = LevelsController.CelestialBodies;
            TurretsController.PlanetarySystemController = PlanetarySystemController;
            TurretsController.StartingTurrets = LevelsController.StartingTurrets;
            EnemiesController.InfiniteWaves = LevelsController.InfiniteWaves;
            EnemiesController.Waves = LevelsController.Waves;
            EnemiesController.PathPreview = PlanetarySystemController.PathPreview;
            EnemiesController.Path = PlanetarySystemController.Path;
            EnemiesController.MineralsCash = LevelsController.Level.Minerals;
            EnemiesController.LifePacksGiven = LevelsController.Level.LifePacks;
            GUIController.Path = PlanetarySystemController.Path;
            GUIController.PathPreview = PlanetarySystemController.PathPreview;
            GUIController.CelestialBodies = PlanetarySystemController.CelestialBodies;
            GUIController.Level = LevelsController.Level;
            GUIController.Enemies = EnemiesController.Enemies;
            GUIController.InfiniteWaves = LevelsController.InfiniteWaves;
            GUIController.Waves = LevelsController.Waves;
            GUIController.AvailablePowerUps = SimPlayersController.AvailablePowerUps;
            GUIController.AvailableTurrets = SimPlayersController.AvailableTurrets;
            GUIController.Turrets = TurretsController.Turrets;
            SpaceshipsController.Enemies = EnemiesController.Enemies;
            SpaceshipsController.PowerUpsBattleship = GUIController.HumanBattleship;
            SpaceshipsController.Minerals = EnemiesController.Minerals;
            MessagesController.Turrets = TurretsController.Turrets;
            MessagesController.CelestialBodies = PlanetarySystemController.CelestialBodies;
            EditorController.GeneralMenu = EditorGUIController.GeneralMenu;
            EditorController.Panels = EditorGUIController.Panels;
            EditorController.EditorGUIPlayers = EditorGUIController.Players;
            EditorController.CelestialBodies = LevelsController.CelestialBodies;
            EditorGUIController.CelestialBodies = LevelsController.CelestialBodies;
            GUIController.CommonStash = LevelsController.CommonStash;
            GUIController.ActiveWaves = EnemiesController.ActiveWaves;
            SimPlayersController.ActiveWaves = EnemiesController.ActiveWaves;
            CollisionsController.Players = SimPlayersController.PlayersList;
            CollisionsController.Path = PlanetarySystemController.Path;
            GUIController.EnemiesData = EnemiesController.EnemiesData;
            AudioController.EnemiesData = EnemiesController.EnemiesData;
            AudioController.CelestialBodyToProtect = LevelsController.CelestialBodyToProtect;

            TweakingController.Initialize();
            LevelsController.Initialize();
            EnemiesController.Initialize();
            TurretsController.Initialize();
            PlanetarySystemController.Initialize();
            GUIController.Initialize();
            PowerUpsController.Initialize();
            SimPlayersController.Initialize(); // Must be done after the PowerUpsController
            CollisionsController.Initialize();
            AudioController.Initialize();
            CameraController.Initialize();

            if (EditorMode)
            {
                EditorGUIController.Initialize();
                EditorController.Initialize();
            }

            BulletsController.Initialize();
            PanelsController.Initialize();
        }


        public void CleanUp()
        {
            Main.CheatsController.CheatActivated -= DoCheatActivated;
            Main.Options.ShowHelpBarChanged -= DoShowHelpBarChanged;
            Inputs.RemoveListener(this);
        }


        public void OnFocus()
        {
            SyncPlayers();
            EnableInputs = true;
            AudioController.DoFocusGained();
        }


        public void OnFocusLost()
        {
            EnableInputs = false;
            AudioController.DoFocusLost();
        }


        public bool SpawnEnemies
        {
            set { EnemiesController.SpawnEnemies = value; }
        }



        public LevelDescriptor LevelDescriptor
        {
            get { return Level.Descriptor; }
            set { Level.Descriptor = value; }
        }


        public CelestialBody GetSelectedCelestialBody(Player p)
        {
            return SimPlayersController.GetSelectedCelestialBody(p);
        }


        public HelpBarPanel HelpBar
        {
            get { return GUIController.HelpBar; }
        }


        public void Update()
        {
            LevelsController.Update();
            PanelsController.Update(); // must be done before SimPlayersController to check if the player want to move or not next tick

            foreach (var p in Inputs.ConnectedPlayers)
            {
                var player = (Commander.Player) p;

                player.ConnectedThisTick = false;

                if (p.InputType == InputType.MouseAndKeyboard || p.InputType == InputType.KeyboardOnly)
                    MoveKeyboard(player);
            }

            if (State != GameState.Paused)
            {
                CollisionsController.Update();
                BulletsController.Update();
                PlanetarySystemController.Update(); // must be done before the TurretController because the celestial bodies must move before the turrets
                TurretsController.Update(); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
                EnemiesController.Update();
                SimPlayersController.Update();
                SpaceshipsController.Update();
                MessagesController.Update();
                GUIController.Update();
                PowerUpsController.Update();
                AudioController.Update();
            }

            else
            {
                SimPlayersController.Update();
            }


            if (EditorMode)
            {
                EditorGUIController.Update();
                EditorController.Update();
            }

            CameraController.Update();
        }


        public void Draw()
        {
            CollisionsController.Draw();
            BulletsController.Draw();
            EnemiesController.Draw();
            TurretsController.Draw();
            PlanetarySystemController.Draw();
            SpaceshipsController.Draw();
            MessagesController.Draw();
            GUIController.Draw();
            PanelsController.Draw();
            SimPlayersController.Draw();
            LevelsController.Draw();

            if (EditorMode)
            {
                EditorGUIController.Draw();
                EditorController.Draw();
            }
        }


        public void AddNewGameStateListener(NewGameStateHandler handler)
        {
            LevelsController.NewGameState += handler;
        }


        public void AddSpaceship(Spaceship spaceship)
        {
            SpaceshipsController.DoAddSpaceshipAsked(spaceship);
        }


        public void SyncPlayers()
        {
            foreach (var p in Inputs.Players)
            {
                Player player = (Player) p;

                if (p.State == PlayerState.Connected && !SimPlayersController.HasPlayer(player))
                    DoPlayerConnected(player);
                else if (p.State == PlayerState.Disconnected && SimPlayersController.HasPlayer(player))
                    DoPlayerDisconnected(player);

                // Sync representation, if needed
                if (p.State == PlayerState.Connected)
                {
                    var simPlayer = SimPlayersController.GetPlayer(p);

                    GUIController.SyncPlayer(simPlayer);
                    PanelsController.SyncPlayer(simPlayer);
                }
            }

            if (DemoMode)
            {
                GUIController.SyncNewGameMenu();
            }
        }


        public void TeleportPlayers(bool teleportOut, Vector3 position)
        {
            SimPlayersController.MovePlayers(position);

            TeleportPlayers(teleportOut);
        }


        public void TeleportPlayers(bool teleportOut)
        {
            GUIController.TeleportPlayers(teleportOut);

            if (Inputs.ConnectedPlayers.Count > 0)
                AudioController.TeleportPlayers(teleportOut);
        }


        public void ShowHelpBarMessage(Player p, HelpBarMessage message)
        {
            GUIController.ShowHelpBarMessage(p, message);
        }


        public void ShowPanel(PanelType type, bool sync)
        {
            if (sync)
                SimPlayersController.SyncPausePlayers();

            PanelsController.ShowPanel(type);
        }


        public void SyncLevel()
        {
            Level.SyncDescriptor();
        }


        internal void TriggerNewGameState(GameState state)
        {
            State = state;
            LevelsController.TriggerNewGameState(state);
        }


        private void DoPlayerSelectionChanged(SimPlayer player)
        {
            PausedGameChoice = player.ActualSelection.PausedGameChoice;
            EditorWorldChoice = player.ActualSelection.EditorWorldChoice;
            NewGameChoice = player.ActualSelection.NewGameChoice;
        }


        private void DoNewGameState(GameState state)
        {
            if (state == GameState.Won || state == GameState.Lost)
            {
                int levelId = LevelsController.Level.Id;
                int score = LevelsController.Level.CommonStash.TotalScore;

                Main.SaveGameController.UpdateProgress(Inputs.MasterPlayer.Name, state, levelId, score);
                Main.SaveGameController.SaveAll();
            }
        }


        private void DoEnemyReachedEndOfPath(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!DemoMode && State != GameState.Lost)
            {
                foreach (var player in Inputs.Players)
                {
                    //Inputs.VibrateControllerHighFrequency(player, 300, 0.5f);
                    Inputs.VibrateControllerLowFrequency(player, 300, 0.9f);
                }
            }
        }


        private void DoCelestialBodyDestroyed(ICollidable obj)
        {
            foreach (var player in Inputs.Players)
            {
                //Inputs.VibrateControllerHighFrequency(player, 300, 0.5f);
                Inputs.VibrateControllerLowFrequency(player, 300, 0.9f);
            }
        }


        private void DoCheatActivated(string name)
        {
            if (DemoMode || EditorMode || WorldMode)
                return;

            if (State != GameState.Running)
                return;

            if (name == "LevelWon")
            {
                State = GameState.Won;
                LevelsController.ComputeFinalScore();
                LevelsController.TriggerNewGameState(State);
            }
        }


        private void DoShowHelpBarChanged(bool value)
        {
            HelpBar.ActiveOptions = value;
        }


        #region InputListener Membres

        public bool EnableInputs { get; set; }


        void InputListener.DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Disconnect)
            {
                if (!player.ConnectedThisTick)
                    Inputs.DisconnectPlayer(p);

                return;
            }

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
            {
                MessagesController.DoQuoteNow();
                MessagesController.DoQuoteNow("Thank you,\n\nCommander!");
                return;
            }

            if (key == player.KeyboardConfiguration.ZoomIn)
                CameraController.ZoomIn();
            else if (key == player.KeyboardConfiguration.ZoomOut)
                CameraController.ZoomOut();

            if (DebugMode)
            {
                if (key == player.KeyboardConfiguration.Debug)
                    CollisionsController.Debug = true;

                if (key == player.KeyboardConfiguration.Tweaking)
                    TweakingController.Sync();
            }

            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (key == player.KeyboardConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer.PausePlayer);

                if (key == player.KeyboardConfiguration.Back)
                    PanelsController.CloseCurrentPanel();

                return;
            }

            if (key == player.KeyboardConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, true);

            // Fire
            if (key == player.KeyboardConfiguration.Fire)
                SimPlayersController.Fire(p);

            // Rotate
            if (simPlayer.Firing)
            {
                if (key == player.KeyboardConfiguration.RotateLeft)
                {
                    var rotation = Core.Physics.Utilities.VectorToAngle(simPlayer.LastMouseDirection);
                    rotation -= MathHelper.PiOver4 * 0.75f;
                    simPlayer.LastMouseDirection = Core.Physics.Utilities.AngleToVector(rotation);
                }
                else if (key == player.KeyboardConfiguration.RotateRight)
                {
                    var rotation = Core.Physics.Utilities.VectorToAngle(simPlayer.LastMouseDirection);
                    rotation += MathHelper.PiOver4 * 0.75f;
                    simPlayer.LastMouseDirection = Core.Physics.Utilities.AngleToVector(rotation);
                }
            }


            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (key == player.KeyboardConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (key == player.KeyboardConfiguration.Select)
                    PowerUpsController.DoInputPressed(simPlayer);
            }

            if (EditorMode)
            {
                if (key == player.KeyboardConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
                else if (key == player.KeyboardConfiguration.QuickToggle)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);
                else if (key == player.KeyboardConfiguration.SelectionNext)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);
                else if (key == player.KeyboardConfiguration.SelectionPrevious)
                    EditorController.DoNextOrPreviousAction(simPlayer, -1);
                else if (key == player.KeyboardConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (WorldMode)
            {
                if (EditorWorldMode)
                {
                    if (key == player.KeyboardConfiguration.QuickToggle)
                        SimPlayersController.DoEditorWorldChoice(player, 1);
                    else if (key == player.KeyboardConfiguration.SelectionNext)
                        SimPlayersController.DoEditorWorldChoice(player, 1);
                    else if (key == player.KeyboardConfiguration.SelectionPrevious)
                        SimPlayersController.DoEditorWorldChoice(player, -1);
                }

                else
                {
                    if (key == player.KeyboardConfiguration.QuickToggle)
                        SimPlayersController.DoPausedGameChoice(player, 1);
                    else if (key == player.KeyboardConfiguration.SelectionNext)
                        SimPlayersController.DoPausedGameChoice(player, 1);
                    else if (key == player.KeyboardConfiguration.SelectionPrevious)
                        SimPlayersController.DoPausedGameChoice(player, -1);
                }
            }

            else if (DemoMode)
            {
                if (key == player.KeyboardConfiguration.QuickToggle)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (key == player.KeyboardConfiguration.SelectionNext)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (key == player.KeyboardConfiguration.SelectionPrevious)
                    SimPlayersController.DoNewGameChoice(player, -1);
            }

            else
            {
                if (key == player.KeyboardConfiguration.Back)
                    TriggerNewGameState(GameState.Paused);

                if (EditorMode && EditorState == EditorState.Editing)
                    return;

                if (key == player.KeyboardConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);                
                else if (key == player.KeyboardConfiguration.QuickToggle)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);
                else if (key == player.KeyboardConfiguration.SelectionNext)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);
                else if (key == player.KeyboardConfiguration.SelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction(player, -1);
                else if (key == player.KeyboardConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);
            }
        }


        void InputListener.DoKeyPressed(Core.Input.Player p, Keys key)
        {
            // Start Moving
            var player = (Commander.Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) //player disconnected
                return;

            if (key == player.KeyboardConfiguration.MoveLeft)
                simPlayer.MovingLeft = true;

            if (key == player.KeyboardConfiguration.MoveRight)
                simPlayer.MovingRight = true;

            if (key == player.KeyboardConfiguration.MoveUp)
                simPlayer.MovingUp = true;

            if (key == player.KeyboardConfiguration.MoveDown)
                simPlayer.MovingDown = true;
        }


        private void MoveKeyboard(Player p)
        {
            var player = (Commander.Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) //disconnected
                return;

            Vector3 delta = Vector3.Zero;

            if (simPlayer.MovingLeft)
                delta.X -= 1;

            if (simPlayer.MovingRight)
                delta.X += 1;

            if (simPlayer.MovingUp)
                delta.Y -= 1;

            if (simPlayer.MovingDown)
                delta.Y += 1;

            if (delta != Vector3.Zero)
                delta.Normalize();

            delta *= player.MovingSpeed * 10;

            if (EditorMode)
                EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

            if (simPlayer.PowerUpInUse != PowerUpType.None)
                PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

            if (PanelsController.IsPanelVisible)
                PanelsController.DoMoveDelta(simPlayer, ref delta);
            else
                SimPlayersController.DoMoveDelta(player, ref delta);

            if (simPlayer.Firing)
            {
                if (PanelsController.IsPanelVisible)
                    PanelsController.DoDirectionDelta(simPlayer, ref simPlayer.LastMouseDirection);
                else
                    SimPlayersController.DoDirectionDelta(player, ref simPlayer.LastMouseDirection);
            }

            simPlayer.MovingLeft = simPlayer.MovingRight = simPlayer.MovingUp = simPlayer.MovingDown = false;
        }


        void InputListener.DoKeyReleased(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (DebugMode && key == player.KeyboardConfiguration.Debug)
                CollisionsController.Debug = false;

            // Stop Moving
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (key == player.KeyboardConfiguration.MoveLeft)
                simPlayer.MovingLeft = false;

            if (key == player.KeyboardConfiguration.MoveRight)
                simPlayer.MovingRight = false;

            if (key == player.KeyboardConfiguration.MoveUp)
                simPlayer.MovingUp = false;

            if (key == player.KeyboardConfiguration.MoveDown)
                simPlayer.MovingDown = false;

            if (!DemoMode && key == player.KeyboardConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, false);

            if (simPlayer.Firing && key == player.KeyboardConfiguration.Fire)
                SimPlayersController.StopFire(p);
        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == player.MouseConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer.PausePlayer);

                return;
            }

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == player.MouseConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == player.MouseConfiguration.Select)
                    PowerUpsController.DoInputPressed(simPlayer);
            }

            if (EditorMode)
            {
                if (button == player.MouseConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (!DemoMode)
            {
                if (button == player.MouseConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);

                else if (button == player.MouseConfiguration.AlternateSelect)
                    SimPlayersController.DoAlternateAction(player);

                else if (button == player.MouseConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);
            }
        }


        void InputListener.DoMouseButtonPressed(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            if (PanelsController.IsPanelVisible)
                return;

            if (button == player.MouseConfiguration.Fire)
                SimPlayersController.Fire(player);
        }


        void InputListener.DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
            {
                if (button == player.MouseConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (simPlayer.Firing)
                SimPlayersController.StopFire(p);

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None && button == player.MouseConfiguration.Select)
                PowerUpsController.DoInputReleased(simPlayer);
        }


        void InputListener.DoMouseScrolled(Core.Input.Player p, int delta)
        {
            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
                EditorController.DoNextOrPreviousAction(simPlayer, delta);

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (WorldMode)
            {
                if (EditorWorldMode)
                    SimPlayersController.DoEditorWorldChoice(player, delta);
                else
                    SimPlayersController.DoPausedGameChoice(player, delta);
            }
            else if (DemoMode)
                SimPlayersController.DoNewGameChoice(player, delta);
            else
                SimPlayersController.DoNextOrPreviousAction(player, delta);
        }


        void InputListener.DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            simPlayer.LastMouseDirection = delta;

            if (PanelsController.IsPanelVisible)
                PanelsController.DoDirectionDelta(simPlayer, ref delta);
            else
                SimPlayersController.DoDirectionDelta(player, ref delta);
        }


        void InputListener.DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (button == player.GamepadConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }

            if (button == player.GamepadConfiguration.ZoomIn)
                CameraController.ZoomIn();
            else if (button == player.GamepadConfiguration.ZoomOut)
                CameraController.ZoomOut();

            if (DebugMode)
            {
                if (button == player.GamepadConfiguration.Debug)
                    CollisionsController.Debug = true;

                if (button == player.GamepadConfiguration.Tweaking)
                {
                    TweakingController.Sync();
                    Main.LevelsFactory.Initialize();
                }
            }

            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == player.GamepadConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer.PausePlayer);

                else if (button == player.GamepadConfiguration.Back)
                    PanelsController.CloseCurrentPanel();

                return;
            }

            if (EditorMode)
            {
                if (button == player.GamepadConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);

                else if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);

                else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                    EditorController.DoNextOrPreviousAction(simPlayer, -1);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == player.GamepadConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == player.GamepadConfiguration.Select || button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                    PowerUpsController.DoInputPressed(simPlayer);
            }


            if (WorldMode)
            {
                if (EditorWorldMode)
                {
                    if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                        SimPlayersController.DoEditorWorldChoice(player, 1);
                    else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                        SimPlayersController.DoEditorWorldChoice(player, -1);
                }

                else
                {
                    if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                        SimPlayersController.DoPausedGameChoice(player, 1);
                    else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                        SimPlayersController.DoPausedGameChoice(player, -1);
                }
            }

            else if (DemoMode)
            {
                if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                    SimPlayersController.DoNewGameChoice(player, -1);
            }

            else
            {
                if (button == player.GamepadConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);

                else if (button == player.GamepadConfiguration.AlternateSelect)
                    SimPlayersController.DoAlternateAction(player);

                else if (button == player.GamepadConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);

                else if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);

                else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction(player, -1);

                else if (button == player.GamepadConfiguration.Back)
                {
                    SimPlayersController.SyncPausePlayers();
                    TriggerNewGameState(GameState.Paused);
                }

                else if (button == player.GamepadConfiguration.AdvancedView)
                    SimPlayersController.DoAdvancedViewAction(player, true);
            }
        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (DebugMode && button == player.GamepadConfiguration.Debug)
                CollisionsController.Debug = false;

            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;


            if (EditorMode)
            {
                if (button == player.GamepadConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (button == player.GamepadConfiguration.AdvancedView && !DemoMode)
                SimPlayersController.DoAdvancedViewAction(player, false);

            if (simPlayer.PowerUpInUse != PowerUpType.None &&
               (button == player.GamepadConfiguration.Select || button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext))
                PowerUpsController.DoInputReleased(simPlayer);
        }


        void InputListener.DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            var player = (Commander.Player) p;

            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (button == player.GamepadConfiguration.MoveCursor)
            {
                delta *= player.MovingSpeed * 10;

                if (EditorMode)
                    EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

                if (simPlayer.PowerUpInUse != PowerUpType.None)
                    PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

                if (PanelsController.IsPanelVisible)
                    PanelsController.DoMoveDelta(simPlayer, ref delta);
                else
                    SimPlayersController.DoMoveDelta(player, ref delta);
            }

            else if (button == player.GamepadConfiguration.DirectionCursor)
            {
                if (PanelsController.IsPanelVisible)
                    PanelsController.DoDirectionDelta(simPlayer, ref delta);
                else
                    SimPlayersController.DoDirectionDelta(player, ref delta);

                if (delta == Vector3.Zero)
                    SimPlayersController.StopFire(p);
                else
                    SimPlayersController.Fire((Player) p);
            }
        }


        void InputListener.PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
            {
                MessagesController.DoQuoteNow();
                MessagesController.DoQuoteNow("Thank you,\n\nCommander!");
            }
        }


        void InputListener.PlayerMouseConnectionRequested(Core.Input.Player p, MouseButton button)
        {

        }


        void InputListener.PlayerGamePadConnectionRequested(Core.Input.Player p, Buttons button)
        {

        }


        public void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Commander.Player) p;
            player.ConnectedThisTick = true;

            SimPlayersController.AddPlayer(player);
        }


        public void DoPlayerDisconnected(Core.Input.Player p)
        {
            var player = (Commander.Player) p;

            SimPlayersController.RemovePlayer(player);
        }

        #endregion
    }
}