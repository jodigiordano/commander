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
        public Scene Scene;
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
        public PhysicalRectangle Terrain;
        public PhysicalRectangle InnerTerrain;
        public GameState State { get { return LevelsController.State; } set { LevelsController.State = value; } }
        public CelestialBody CelestialBodyPausedGame;


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
        internal TweakingController TweakingController;
        internal TurretsFactory TurretsFactory;
        internal PowerUpsFactory PowerUpsFactory;
        internal EnemiesFactory EnemiesFactory;
        internal MineralsFactory MineralsFactory;
        internal BulletsFactory BulletsFactory;


        public Simulator(Scene scene, LevelDescriptor descriptor)
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
                    @"mothershipMissile"
                }, false);

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

            Terrain = new PhysicalRectangle(-840, -560, 1680, 1120);
            InnerTerrain = new PhysicalRectangle(-640, -360, 1280, 720);

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
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjetDestroyed);
            CollisionsController.InTurretRange += new TurretPhysicalObjectHandler(TurretsController.DoInRangeTurret);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(GUIController.DoWaveStarted);
            SimPlayersController.SellTurretAsked += new TurretSimPlayerHandler(TurretsController.DoSellTurret);
            TurretsController.TurretSold += new TurretSimPlayerHandler(SimPlayersController.DoTurretSold);
            TurretsController.TurretBought += new TurretSimPlayerHandler(SimPlayersController.DoTurretBought);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(LevelsController.DoEnemyReachedEnd);
            TurretsController.TurretUpdated += new TurretSimPlayerHandler(SimPlayersController.DoTurretUpdated);
            SimPlayersController.UpgradeTurretAsked += new TurretSimPlayerHandler(TurretsController.DoUpgradeTurret);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjetDestroyed);
            SimPlayersController.NextWaveAsked += new NoneHandler(EnemiesController.DoNextWaveAsked);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(SimPlayersController.DoObjectCreated);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            EnemiesController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(LevelsController.DoObjectDestroyed);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(this.DoEnemyReachedEndOfPath);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(this.DoCelestialBodyDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(GUIController.DoNextWave);
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
            SimPlayersController.ShowAdvancedViewAsked += new SimPlayerHandler(GUIController.DoShowAdvancedViewAsked);
            SimPlayersController.HideAdvancedViewAsked += new SimPlayerHandler(GUIController.DoHideAdvancedViewAsked);
            SimPlayersController.ShowNextWaveAsked += new SimPlayerHandler(GUIController.DoShowNextWaveAsked);
            SimPlayersController.HideNextWaveAsked += new SimPlayerHandler(GUIController.DoHideNextWaveAsked);
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
        }


        public void Initialize()
        {
            State = GameState.Running;

            Level.Initialize();

            LevelsController.Level = Level;
            TurretsFactory.Availables = LevelsController.AvailableTurrets;
            PowerUpsFactory.Availables = LevelsController.AvailablePowerUps;
            PowerUpsFactory.HumanBattleship = GUIController.HumanBattleship;
            TweakingController.BulletsFactory = BulletsFactory;
            TweakingController.EnemiesFactory = EnemiesFactory;
            TweakingController.MineralsFactory = MineralsFactory;
            TweakingController.PowerUpsFactory = PowerUpsFactory;
            TweakingController.TurretsFactory = TurretsFactory;
            CollisionsController.Bullets = BulletsController.Bullets;
            CollisionsController.Enemies = EnemiesController.Enemies;
            CollisionsController.Turrets = TurretsController.Turrets;
            CollisionsController.CelestialBodies = LevelsController.CelestialBodies;
            CollisionsController.Minerals = EnemiesController.Minerals;
            CollisionsController.ShootingStars = PlanetarySystemController.ShootingStars;
            SimPlayersController.CelestialBodies = LevelsController.CelestialBodies;
            SimPlayersController.CommonStash = LevelsController.CommonStash;
            SimPlayersController.CelestialBodyToProtect = LevelsController.CelestialBodyToProtect;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            SimPlayersController.ActivesPowerUps = PowerUpsController.ActivesPowerUps;
            PlanetarySystemController.CelestialBodies = LevelsController.CelestialBodies;
            TurretsController.PlanetarySystemController = PlanetarySystemController;
            TurretsController.StartingTurrets = LevelsController.StartingTurrets;
            EnemiesController.InfiniteWaves = LevelsController.InfiniteWaves;
            EnemiesController.Waves = LevelsController.Waves;
            EnemiesController.PathPreview = PlanetarySystemController.PathPreview;
            EnemiesController.Path = PlanetarySystemController.Path;
            EnemiesController.MineralsCash = LevelsController.Level.Cash;
            EnemiesController.LifePacksGiven = LevelsController.Level.LifePacks;
            GUIController.Path = PlanetarySystemController.Path;
            GUIController.PathPreview = PlanetarySystemController.PathPreview;
            GUIController.CompositionNextWave = EnemiesController.NextWaveData;
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
            SpaceshipsController.HumanBattleship = GUIController.HumanBattleship;
            SpaceshipsController.Minerals = EnemiesController.Minerals;
            MessagesController.Turrets = TurretsController.Turrets;
            EditorController.GeneralMenu = EditorGUIController.GeneralMenu;
            EditorController.Panels = EditorGUIController.Panels;
            EditorController.EditorGUIPlayers = EditorGUIController.Players;
            EditorController.CelestialBodies = LevelsController.CelestialBodies;
            EditorGUIController.CelestialBodies = LevelsController.CelestialBodies;

            TweakingController.Initialize();
            LevelsController.Initialize();
            EnemiesController.Initialize();
            TurretsController.Initialize();
            PlanetarySystemController.Initialize();
            GUIController.Initialize();
            PowerUpsController.Initialize();
            SimPlayersController.Initialize(); // Must be done after the PowerUpsController
            CollisionsController.Initialize();
            EditorGUIController.Initialize();
            EditorController.Initialize();
            BulletsController.Initialize();
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
            }

            if (EditorMode)
            {
                EditorGUIController.Update();
                EditorController.Update();

                if (State == GameState.Paused)
                {
                    SimPlayersController.Update();
                    GUIController.Update();
                }
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


        public void SyncPlayers()
        {
            foreach (var p in Inputs.Players)
            {
                Player player = (Player) p;

                if (p.State == PlayerState.Connected && !SimPlayersController.HasPlayer(player))
                    DoPlayerConnected(player);
                else if (p.State == PlayerState.Disconnected && SimPlayersController.HasPlayer(player))
                    DoPlayerDisconnected(player);
            }

            if (DemoMode)
            {
                GUIController.SyncNewGameMenu();
            }
        }


        public void ShowHelpBarMessage(HelpBarMessage message)
        {
            GUIController.ShowHelpBarMessage(message);
        }


        internal void SyncLevel()
        {
            Level.SyncDescriptor();
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

                if (!Main.SharedSaveGame.HighScores.ContainsKey(levelId))
                    Main.SharedSaveGame.HighScores.Add(levelId, new HighScores(levelId));

                Main.SharedSaveGame.HighScores[levelId].Add(Inputs.MasterPlayer.Name, score);

                Main.SharedSaveGame.Save();
                Main.PlayerSaveGame.Save();
            }
        }


        private void DoEnemyReachedEndOfPath(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!this.DemoMode && this.State != GameState.Lost)
            {
                foreach (var player in Inputs.Players)
                    Inputs.VibrateController(player, 300, 0.5f, 0.5f);
            }
        }


        private void DoCelestialBodyDestroyed(IObjetPhysique obj)
        {
            foreach (var player in Inputs.Players)
                Inputs.VibrateController(player, 300, 0.5f, 0.5f);
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
                if (key == KeyboardConfiguration.Cancel)
                    LevelsController.Help.Skip();
                if (key == KeyboardConfiguration.Next)
                    LevelsController.Help.NextDirective();
                if (key == KeyboardConfiguration.Back)
                    LevelsController.Help.PreviousDirective();

                return;
            }

            if (!DemoMode && (key == KeyboardConfiguration.Back || key == KeyboardConfiguration.Cancel))
            {
                State = GameState.Paused;
                LevelsController.TriggerNewGameState(State);
                return;
            }
        }


        void InputListener.DoKeyPressed(Core.Input.Player p, Keys key)
        {
            // Start Moving
            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(player);

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

            if (simPlayer == null)
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

            EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

            if (!EditorMode && State != GameState.Running)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None)
                PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

            SimPlayersController.DoMoveDelta(player, ref delta);

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
        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (LevelsController.Help.Active)
            {
                if (button == MouseConfiguration.Select)
                    LevelsController.Help.NextDirective();
                else if (button == MouseConfiguration.Back)
                    LevelsController.Help.PreviousDirective();

                return;
            }

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

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

                if (button == MouseConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);

                if (button == MouseConfiguration.AdvancedView)
                    SimPlayersController.DoAdvancedViewAction(player, true);
            }
        }


        void InputListener.DoMouseButtonPressed(Core.Input.Player p, MouseButton button)
        {
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

            if (!DemoMode && button == MouseConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, false);

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

            if (simPlayer.Firing)
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
                if (button == GamePadConfiguration.SelectionNext)
                    LevelsController.Help.NextDirective();
                else if (button == GamePadConfiguration.SelectionPrevious)
                    LevelsController.Help.PreviousDirective();
                else if (button == GamePadConfiguration.Cancel)
                    LevelsController.Help.Skip();

                return;
            }

            var player = (Player) p;
            var simPlayer = SimPlayersController.GetPlayer(p);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
            {
                if (button == GamePadConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
                else if (button == GamePadConfiguration.SelectionNext)
                    EditorController.DoNextOrPreviousAction(simPlayer, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    EditorController.DoNextOrPreviousAction(simPlayer, -1);
            }

            if (EditorMode && EditorState == EditorState.Editing)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == GamePadConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext)
                    PowerUpsController.DoInputPressed(simPlayer);
            }


            if (WorldMode)
            {
                if (button == GamePadConfiguration.SelectionNext)
                    SimPlayersController.DoPausedGameChoice(player, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    SimPlayersController.DoPausedGameChoice(player, -1);
            }

            else if (DemoMode)
            {
                if (button == GamePadConfiguration.SelectionNext)
                    SimPlayersController.DoNewGameChoice(player, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    SimPlayersController.DoNewGameChoice(player, -1);
            }

            else
            {
                if (button == GamePadConfiguration.Select)
                    SimPlayersController.DoSelectAction(player);
                else if (button == GamePadConfiguration.Cancel)
                    SimPlayersController.DoCancelAction(player);
                else if (button == GamePadConfiguration.SelectionNext)
                    SimPlayersController.DoNextOrPreviousAction(player, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction(player, -1);

                if (button == GamePadConfiguration.AdvancedView)
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
               (button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext))
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

                EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

                if (!EditorMode && State != GameState.Running)
                    return;

                if (simPlayer.PowerUpInUse != PowerUpType.None)
                    PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

                SimPlayersController.DoMoveDelta(player, ref delta);
            }

            else if (button == GamePadConfiguration.DirectionCursor)
            {
                simPlayer.Firing = true;
                SimPlayersController.DoDirectionDelta(player, ref delta);
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