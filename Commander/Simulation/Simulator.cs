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
        public Dictionary<string, LevelDescriptor> AvailableLevelsDemoMode;
        public bool DebugMode;
        public bool WorldMode;
        public bool DemoMode;
        public bool CutsceneMode;
        public bool CanSelectCelestialBodies;
        
        public bool EditorMode;
        internal EditorState EditorState;
        internal Level Level;

        public bool HelpMode { get { return LevelsController.Help.Active; } }
        public PausedGameChoice PausedGameChoice;
        public NewGameChoice NewGameChoice;
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
                    @"projectileMissileDeplacement",
                    @"projectileBaseExplosion",
                    @"etoile",
                    @"etoileBleue",
                    @"planeteGazeuse",
                    @"etoilesScintillantes",
                    @"projectileMissileExplosion",
                    @"projectileLaserSimple",
                    @"projectileLaserMultiple",
                    @"selectionCorpsCeleste",
                    @"traineeMissile",
                    @"etincelleLaser",
                    @"toucherTerre",
                    @"anneauTerreMeurt",
                    @"anneauTerreMeurt2",
                    @"bouleTerreMeurt",
                    @"missileAlien",
                    @"implosionAlien",
                    @"explosionEnnemi",
                    @"mineral1",
                    @"mineral2",
                    @"mineral3",
                    @"mineralPointsVie",
                    @"mineralPris",
                    @"etincelleMissile",
                    @"etincelleLaserSimple",
                    @"etincelleSlowMotion",
                    @"etincelleSlowMotionTouche",
                    @"etoileFilante",
                    @"trouRose",
                    @"boosterTurret",
                    @"gunnerTurret",
                    @"nanobots",
                    @"nanobots2",
                    @"railgun",
                    @"railgunExplosion",
                    @"pulseEffect",
                    @"shieldEffect",
                    @"spaceshipTrail",
                    @"spaceshipTrail2",
                    @"darkSideEffect",
                    @"starExplosion",
                    @"mothershipMissile",
                    @"nextWave",
                    @"mothershipAbduction"
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

            WorldMode = false;
            DemoMode = false;
            CutsceneMode = false;
            EditorMode = false;
            CanSelectCelestialBodies = true;
            EditorState = EditorState.Editing;
            DebugMode = Preferences.Debug;
            PausedGameChoice = PausedGameChoice.None;
            NewGameChoice = NewGameChoice.None;


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
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
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
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(GUIController.DoObjectDestroyed);
            EnemiesController.NextWaveCompositionChanged += new NextWaveHandler(GUIController.DoNextWaveCompositionChanged);
            LevelsController.NewGameState += new NewGameStateHandler(SimPlayersController.DoNewGameState);
            CollisionsController.PlayersCollided += new SimPlayerSimPlayerHandler(SimPlayersController.DoPlayersCollided);
            CollisionsController.PlayersCollided += new SimPlayerSimPlayerHandler(AudioController.DoPlayersCollided);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(BulletsController.DoStartingPathCollision);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(GUIController.DoStartingPathCollision);
            CollisionsController.StartingPathCollision += new BulletCelestialBodyHandler(AudioController.DoStartingPathCollision);
            CollisionsController.ShieldCollided += new CollidableBulletHandler(SpaceshipsController.DoShieldCollided);
            CollisionsController.ShieldCollided += new CollidableBulletHandler(BulletsController.DoShieldCollided);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(AudioController.DoWaveStarted);
            PowerUpsController.PowerUpStarted += new PowerUpSimPlayerHandler(AudioController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpSimPlayerHandler(AudioController.DoPowerUpStopped);
            TurretsController.TurretBought += new TurretSimPlayerHandler(AudioController.DoTurretBought);
            TurretsController.TurretSold += new TurretSimPlayerHandler(AudioController.DoTurretSold);
            TurretsController.TurretFired += new TurretHandler(AudioController.DoTurretFired);
            TurretsController.TurretReactivated += new TurretHandler(AudioController.DoTurretReactivated);
            PowerUpsController.PowerUpInputCanceled += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputCanceled);
            PowerUpsController.PowerUpInputPressed += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputPressed);
            PowerUpsController.PowerUpInputReleased += new PowerUpSimPlayerHandler(AudioController.DoPowerUpInputReleased);
            EnemiesController.ObjectHit += new PhysicalObjectHandler(AudioController.DoObjectHit);
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            PlanetarySystemController.ObjectHit += new PhysicalObjectHandler(AudioController.DoObjectHit);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(AudioController.DoObjectDestroyed);
            SimPlayersController.PlayerBounced += new SimPlayerHandler(AudioController.DoPlayerBounced);
            TurretsController.TurretWandered += new TurretHandler(AudioController.DoTurretWandered);
            TurretsController.TurretUpgraded += new TurretSimPlayerHandler(AudioController.DoTurretUpgraded);
            BulletsController.BulletDeflected += new BulletHandler(AudioController.DoBulletDeflected);
            EnemiesController.WaveNearToStart += new NoneHandler(AudioController.DoWaveNearToStart);

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
            GUIController.AvailableLevelsDemoMode = AvailableLevelsDemoMode;
            GUIController.AvailablePowerUps = SimPlayersController.AvailablePowerUps;
            GUIController.AvailableTurrets = SimPlayersController.AvailableTurrets;
            GUIController.Turrets = TurretsController.Turrets;
            SpaceshipsController.Enemies = EnemiesController.Enemies;
            SpaceshipsController.PowerUpsBattleship = GUIController.HumanBattleship;
            SpaceshipsController.Minerals = EnemiesController.Minerals;
            MessagesController.Turrets = TurretsController.Turrets;
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
            PanelsController.Update();

            foreach (var p in Inputs.ConnectedPlayers)
                if (p.InputType == InputType.Mouse)
                    MoveKeyboard((Player) p);

            if (LevelsController.Help.Active)
                return;

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
        }


        public void ShowHelpBarMessage(HelpBarMessage message, InputType type)
        {
            GUIController.ShowHelpBarMessage(message, type);
        }


        public void ShowPanel(PanelType type, bool sync)
        {
            if (sync)
                SimPlayersController.SyncPausePlayers();

            PanelsController.ShowPanel(type);
        }


        internal void SyncLevel()
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
            if (key == KeyboardConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }

            if (DebugMode)
            {
                if (key == KeyboardConfiguration.Debug)
                    CollisionsController.Debug = true;

                if (key == KeyboardConfiguration.Tweaking)
                    TweakingController.Sync();
            }

            if (LevelsController.Help.Active)
            {
                if (key == KeyboardConfiguration.Back)
                    LevelsController.Help.Skip();
                if (key == KeyboardConfiguration.Next)
                    LevelsController.Help.NextDirective();
                if (key == KeyboardConfiguration.Previous)
                    LevelsController.Help.PreviousDirective();

                return;
            }

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (key == KeyboardConfiguration.Back)
                {
                    if (State == GameState.Paused)
                    {
                        SimPlayersController.SyncPausePlayers();
                        TriggerNewGameState(GameState.Running);
                    }
                }

                return;
            }

            if (key == KeyboardConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, true);


            if (EditorMode)
            {
                if (key == KeyboardConfiguration.QuickToggle)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);
                else if (key == KeyboardConfiguration.SelectionNext)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);
                else if (key == KeyboardConfiguration.SelectionPrevious)
                    EditorController.DoNextOrPreviousAction(simPlayer, -1);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (WorldMode)
            {
                if (key == KeyboardConfiguration.QuickToggle)
                    SimPlayersController.DoPausedGameChoice(player, 1);
                else if (key == KeyboardConfiguration.SelectionNext)
                    SimPlayersController.DoPausedGameChoice(player, 1);
                else if (key == KeyboardConfiguration.SelectionPrevious)
                    SimPlayersController.DoPausedGameChoice(player, -1);
            }

            else if (DemoMode)
            {
                if (key == KeyboardConfiguration.QuickToggle)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (key == KeyboardConfiguration.SelectionNext)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (key == KeyboardConfiguration.SelectionPrevious)
                    SimPlayersController.DoNewGameChoice(player, -1);
            }

            else
            {
                if (key == KeyboardConfiguration.QuickToggle)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);
                else if (key == KeyboardConfiguration.Back)
                    TriggerNewGameState(GameState.Paused);
                else if (key == KeyboardConfiguration.SelectionNext)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);
                else if (key == KeyboardConfiguration.SelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction(player, -1);
            }
        }


        void InputListener.DoKeyPressed(Core.Input.Player p, Keys key)
        {
            // Start Moving
            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) //player disconnected
                return;

            if (key == KeyboardConfiguration.MoveLeft)
                simPlayer.MovingLeft = true;

            if (key == KeyboardConfiguration.MoveRight)
                simPlayer.MovingRight = true;

            if (key == KeyboardConfiguration.MoveUp)
                simPlayer.MovingUp = true;

            if (key == KeyboardConfiguration.MoveDown)
                simPlayer.MovingDown = true;
        }


        private void MoveKeyboard(Player player)
        {
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

            delta *= GamePadConfiguration.Speed;

            if (EditorMode)
                EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

            if (simPlayer.PowerUpInUse != PowerUpType.None)
                PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

            if (PanelsController.IsPanelVisible)
                PanelsController.DoMoveDelta(simPlayer.PausePlayer, ref delta);
            else
                SimPlayersController.DoMoveDelta(player, ref delta);

            if (simPlayer.Firing)
            {
                if (PanelsController.IsPanelVisible)
                    PanelsController.DoDirectionDelta(simPlayer.PausePlayer, ref simPlayer.LastMouseDirection);
                else
                    SimPlayersController.DoDirectionDelta(player, ref simPlayer.LastMouseDirection);
            }

            simPlayer.MovingLeft = simPlayer.MovingRight = simPlayer.MovingUp = simPlayer.MovingDown = false;
        }


        void InputListener.DoKeyReleased(Core.Input.Player p, Keys key)
        {
            if (DebugMode && key == KeyboardConfiguration.Debug)
                CollisionsController.Debug = false;

            if (LevelsController.Help.Active)
                return;

            // Stop Moving
            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (key == KeyboardConfiguration.MoveLeft)
                simPlayer.MovingLeft = false;

            if (key == KeyboardConfiguration.MoveRight)
                simPlayer.MovingRight = false;

            if (key == KeyboardConfiguration.MoveUp)
                simPlayer.MovingUp = false;

            if (key == KeyboardConfiguration.MoveDown)
                simPlayer.MovingDown = false;

            if (!DemoMode && key == KeyboardConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, false);
        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (LevelsController.Help.Active)
            {
                if (button == MouseConfiguration.Next)
                    LevelsController.Help.NextDirective();
                else if (button == MouseConfiguration.Previous)
                    LevelsController.Help.PreviousDirective();

                return;
            }

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == MouseConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer.PausePlayer);

                return;
            }

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == MouseConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == MouseConfiguration.Select)
                    PowerUpsController.DoInputPressed(simPlayer);
            }

            if (EditorMode)
            {
                if (button == MouseConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (!DemoMode)
            {
                if (button == MouseConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);

                if (button == MouseConfiguration.AlternateSelect)
                    SimPlayersController.DoAlternateAction(player);

                if (button == MouseConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);
            }
        }


        void InputListener.DoMouseButtonPressed(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Fire)
                SimPlayersController.Fire((Player) p);
        }


        void InputListener.DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            if (LevelsController.Help.Active)
                return;

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
            {
                if (button == MouseConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (simPlayer.Firing)
                simPlayer.Firing = false;

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None && button == MouseConfiguration.Select)
                PowerUpsController.DoInputReleased(simPlayer);
        }


        void InputListener.DoMouseScrolled(Core.Input.Player p, int delta)
        {
            if (LevelsController.Help.Active)
                return;

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
                EditorController.DoNextOrPreviousAction(simPlayer, delta);

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (WorldMode)
                SimPlayersController.DoPausedGameChoice(player, delta);
            else if (DemoMode)
                SimPlayersController.DoNewGameChoice(player, delta);
            else
                SimPlayersController.DoNextOrPreviousAction(player, delta);
        }


        void InputListener.DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            if (LevelsController.Help.Active)
                return;

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            simPlayer.LastMouseDirection = delta;

            if (PanelsController.IsPanelVisible)
                PanelsController.DoDirectionDelta(simPlayer.PausePlayer, ref delta);
            else
                SimPlayersController.DoDirectionDelta(player, ref delta);
        }


        void InputListener.DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }

            if (DebugMode)
            {
                if (button == GamePadConfiguration.Debug)
                    CollisionsController.Debug = true;

                if (button == GamePadConfiguration.Tweaking)
                {
                    TweakingController.Sync();
                    Main.LevelsFactory.Initialize();
                }
            }


            if (LevelsController.Help.Active)
            {
                if (button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    LevelsController.Help.NextDirective();
                else if (button == GamePadConfiguration.SelectionPrevious || button == GamePadConfiguration.AlternateSelectionPrevious)
                    LevelsController.Help.PreviousDirective();
                else if (button == GamePadConfiguration.Cancel)
                    LevelsController.Help.Skip();

                return;
            }

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == GamePadConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer.PausePlayer);

                else if (button == GamePadConfiguration.Back)
                {
                    if (State == GameState.Paused)
                        TriggerNewGameState(GameState.Running);
                }

                return;
            }

            if (EditorMode)
            {
                if (button == GamePadConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);

                else if (button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);

                else if (button == GamePadConfiguration.SelectionPrevious || button == GamePadConfiguration.AlternateSelectionPrevious)
                    EditorController.DoNextOrPreviousAction(simPlayer, -1);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == GamePadConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    PowerUpsController.DoInputPressed(simPlayer);
            }


            if (WorldMode)
            {
                if (button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    SimPlayersController.DoPausedGameChoice(player, 1);
                else if (button == GamePadConfiguration.SelectionPrevious || button == GamePadConfiguration.AlternateSelectionPrevious)
                    SimPlayersController.DoPausedGameChoice(player, -1);
            }

            else if (DemoMode)
            {
                if (button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (button == GamePadConfiguration.SelectionPrevious || button == GamePadConfiguration.AlternateSelectionPrevious)
                    SimPlayersController.DoNewGameChoice(player, -1);
            }

            else
            {
                if (button == GamePadConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);

                else if (button == GamePadConfiguration.AlternateSelect)
                    SimPlayersController.DoAlternateAction(player);

                else if (button == GamePadConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);

                else if (button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);

                else if (button == GamePadConfiguration.SelectionPrevious || button == GamePadConfiguration.AlternateSelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction(player, -1);

                else if (button == GamePadConfiguration.Back)
                {
                    SimPlayersController.SyncPausePlayers();
                    TriggerNewGameState(GameState.Paused);
                }

                else if (button == GamePadConfiguration.AdvancedView)
                    SimPlayersController.DoAdvancedViewAction(player, true);
            }
        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            if (DebugMode && button == GamePadConfiguration.Debug)
                CollisionsController.Debug = false;

            if (LevelsController.Help.Active)
                return;

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;


            if (EditorMode)
            {
                if (button == GamePadConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (button == GamePadConfiguration.AdvancedView && !DemoMode)
                SimPlayersController.DoAdvancedViewAction(player, false);

            if (simPlayer.PowerUpInUse != PowerUpType.None &&
               (button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext || button == GamePadConfiguration.AlternateSelectionNext))
                PowerUpsController.DoInputReleased(simPlayer);
        }


        void InputListener.DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            if (LevelsController.Help.Active)
                return;

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (button == GamePadConfiguration.MoveCursor)
            {
                delta *= GamePadConfiguration.Speed;

                if (EditorMode)
                    EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

                if (simPlayer.PowerUpInUse != PowerUpType.None)
                    PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

                if (PanelsController.IsPanelVisible)
                    PanelsController.DoMoveDelta(simPlayer.PausePlayer, ref delta);
                else
                    SimPlayersController.DoMoveDelta(player, ref delta);
            }

            else if (button == GamePadConfiguration.DirectionCursor)
            {
                if (PanelsController.IsPanelVisible)
                    PanelsController.DoDirectionDelta(simPlayer.PausePlayer, ref delta);
                else
                    SimPlayersController.DoDirectionDelta(player, ref delta);

                SimPlayersController.Fire((Player) p);
            }
        }


        void InputListener.PlayerKeyboardConnectionRequested(Core.Input.Player player, Keys key)
        {

        }


        void InputListener.PlayerMouseConnectionRequested(Core.Input.Player player, MouseButton button)
        {

        }


        void InputListener.PlayerGamePadConnectionRequested(Core.Input.Player player, Buttons button)
        {

        }


        public void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Commander.Player) p;

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