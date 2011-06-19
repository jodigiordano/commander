namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    delegate void PhysicalObjectHandler(IObjetPhysique o);
    delegate void PhysicalObjectPhysicalObjectHandler(IObjetPhysique o1, IObjetPhysique o2);
    delegate void SimPlayerHandler(SimPlayer p);
    delegate void CommonStashHandler(CommonStash s);
    delegate void CelestialObjectHandler(CelestialBody c);
    delegate void NewGameStateHandler(GameState s);
    delegate void TurretHandler(Turret t);
    delegate void TurretSimPlayerHandler(Turret t, SimPlayer p);
    delegate void PowerUpTypeSimPlayerHandler(PowerUpType p, SimPlayer sp);
    delegate void PowerUpTypeHandler(PowerUpType p);
    delegate void PowerUpHandler(PowerUp p);
    delegate void TurretTurretHandler(Turret t1, Turret t2);
    delegate void EnemyCelestialBodyHandler(Enemy e, CelestialBody c);
    delegate void TurretPhysicalObjectHandler(Turret t, IObjetPhysique o);
    delegate void EnemyBulletHandler(Enemy e, Bullet b);


    class Simulator : InputListener
    {
        public Scene Scene;
        public Dictionary<string, LevelDescriptor> AvailableLevelsDemoMode;
        public LevelDescriptor LevelDescriptor;
        public bool DebugMode;
        public bool WorldMode;
        public bool DemoMode;
        public bool EditorMode;
        public bool HelpMode { get { return LevelsController.Help.Active; } }
        public GameAction GameAction;
        public TurretsFactory TurretsFactory;
        public PowerUpsFactory PowerUpsFactory;
        public EnemiesFactory EnemiesFactory;
        public MineralsFactory MineralsFactory;
        public BulletsFactory BulletsFactory;
        public RectanglePhysique Terrain;
        public RectanglePhysique InnerTerrain;
        public GameState State { get { return LevelsController.State; } set { LevelsController.State = value; } }
        public CelestialBody CelestialBodyPausedGame;
        
        public Vector3 PositionCurseur; 

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


        public Simulator(Scene scene, LevelDescriptor descriptor)
        {
            Scene = scene;
            LevelDescriptor = descriptor;
            TurretsFactory = new TurretsFactory(this);
            PowerUpsFactory = new PowerUpsFactory(this);
            EnemiesFactory = new EnemiesFactory(this);
            MineralsFactory = new MineralsFactory(this);
            BulletsFactory = new BulletsFactory(this);

            Terrain = new RectanglePhysique(-840, -560, 1680, 1120);
            InnerTerrain = new RectanglePhysique(-640, -320, 1280, 720);

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

            WorldMode = false;
            DemoMode = false;
            EditorMode = false;
            DebugMode = false;
            GameAction = GameAction.None;
        }


        public void Initialize()
        {
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
                    @"darkSideEffect",
                    @"starExplosion"
                }, false);

            Scene.Particles.SetMaxInstances(@"toucherTerre", 3);


            LevelsController.Level = new Level(this, LevelDescriptor);
            TurretsFactory.Availables = LevelsController.AvailableTurrets;
            PowerUpsFactory.Availables = LevelsController.AvailablePowerUps;
            CollisionsController.Bullets = BulletsController.Bullets;
            CollisionsController.Enemies = EnemiesController.Enemies;
            SimPlayersController.CelestialBodies = LevelsController.CelestialBodies;
            PlanetarySystemController.CelestialBodies = LevelsController.CelestialBodies;
            TurretsController.PlanetarySystemController = PlanetarySystemController;
            EnemiesController.InfiniteWaves = LevelsController.InfiniteWaves;
            EnemiesController.Waves = LevelsController.Waves;
            CollisionsController.Turrets = TurretsController.Turrets;
            SimPlayersController.CommonStash = LevelsController.CommonStash;
            SimPlayersController.CelestialBodyToProtect = LevelsController.CelestialBodyToProtect;
            GUIController.Path = PlanetarySystemController.Path;
            GUIController.PathPreview = PlanetarySystemController.PathPreview;
            EnemiesController.PathPreview = PlanetarySystemController.PathPreview;
            TurretsController.StartingTurrets = LevelsController.StartingTurrets;
            EnemiesController.Path = PlanetarySystemController.Path;
            CollisionsController.CelestialBodies = LevelsController.CelestialBodies;
            CollisionsController.Minerals = EnemiesController.Minerals;
            EnemiesController.MineralsValue = LevelsController.Level.MineralsValue;
            EnemiesController.MineralsPercentages = LevelsController.Level.MineralsPercentages;
            EnemiesController.LifePacksGiven = LevelsController.Level.LifePacks;
            SpaceshipsController.Enemies = EnemiesController.Enemies;
            MessagesController.Turrets = TurretsController.Turrets;
            GUIController.CompositionNextWave = EnemiesController.NextWaveData;
            SimPlayersController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = PlanetarySystemController.CelestialBodies;
            GUIController.Level = LevelsController.Level;
            GUIController.Enemies = EnemiesController.Enemies;
            SimPlayersController.InitialPlayerPosition = PositionCurseur;
            GUIController.InfiniteWaves = LevelsController.InfiniteWaves;
            GUIController.Waves = LevelsController.Waves;
            GUIController.AvailableLevelsDemoMode = AvailableLevelsDemoMode;
            SpaceshipsController.HumanBattleship = GUIController.HumanBattleship;
            PowerUpsFactory.HumanBattleship = GUIController.HumanBattleship;
            SimPlayersController.ActivesPowerUps = PowerUpsController.ActivesPowerUps;
            GUIController.Turrets = TurretsController.Turrets;
            SpaceshipsController.Minerals = EnemiesController.Minerals;
            CollisionsController.ShootingStars = PlanetarySystemController.ShootingStars;
            GUIController.AvailablePowerUps = SimPlayersController.AvailablePowerUps;
            GUIController.AvailableTurrets = SimPlayersController.AvailableTurrets;


            CollisionsController.ObjectHit += new PhysicalObjectPhysicalObjectHandler(EnemiesController.DoObjectHit);
            CollisionsController.ObjectHit += new PhysicalObjectPhysicalObjectHandler(BulletsController.DoObjectHit);
            CollisionsController.ObjectOutOfBounds += new PhysicalObjectHandler(BulletsController.DoObjectOutOfBounds);
            SimPlayersController.BuyTurretAsked += new TurretHandler(TurretsController.DoBuyTurret);
            EnemiesController.WaveEnded += new NoneHandler(LevelsController.doWaveEnded);
            EnemiesController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjetDestroyed);
            CollisionsController.InTurretRange += new TurretPhysicalObjectHandler(TurretsController.DoInRangeTurret);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(GUIController.DoWaveStarted);
            SimPlayersController.SellTurretAsked += new TurretHandler(TurretsController.DoSellTurret);
            TurretsController.TurretSold += new TurretHandler(SimPlayersController.DoTurretSold);
            TurretsController.TurretBought += new TurretHandler(SimPlayersController.DoTurretBought);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(LevelsController.doEnemyReachedEnd);
            TurretsController.TurretUpdated += new TurretHandler(SimPlayersController.DoTurretUpdated);
            SimPlayersController.UpgradeTurretAsked += new TurretHandler(TurretsController.DoUpgradeTurret);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(SimPlayersController.DoObjetDestroyed);
            SimPlayersController.NextWaveAsked += new NoneHandler(EnemiesController.DoNextWaveAsked);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(SimPlayersController.DoObjectCreated);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(CollisionsController.DoObjectDestroyed);
            SpaceshipsController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            EnemiesController.ObjectCreated += new PhysicalObjectHandler(CollisionsController.DoObjectCreated);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(LevelsController.doObjectDestroyed);
            EnemiesController.EnemyReachedEndOfPath += new EnemyCelestialBodyHandler(this.DoEnemyReachedEndOfPath);
            PlanetarySystemController.ObjectDestroyed += new PhysicalObjectHandler(this.DoCelestialBodyDestroyed);
            EnemiesController.WaveStarted += new NoneHandler(GUIController.DoNextWave);
            SimPlayersController.CommonStashChanged += new CommonStashHandler(GUIController.DoCommonStashChanged);
            LevelsController.NewGameState += new NewGameStateHandler(GUIController.DoGameStateChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(GUIController.DoPlayerSelectionChanged);
            SimPlayersController.PlayerSelectionChanged += new SimPlayerHandler(this.DoPlayerSelectionChanged);
            TurretsController.TurretReactivated += new TurretHandler(SimPlayersController.DoTurretReactivated);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(GUIController.DoPlayerMoved);
            LevelsController.NewGameState += new NewGameStateHandler(this.DoNewGameState); //must come after GUIController.doGameStateChanged
            LevelsController.CommonStashChanged += new CommonStashHandler(GUIController.DoCommonStashChanged);
            SimPlayersController.TurretToPlaceSelected += new TurretSimPlayerHandler(GUIController.DoTurretToPlaceSelected);
            SimPlayersController.TurretToPlaceDeselected += new TurretSimPlayerHandler(GUIController.DoTurretToPlaceDeselected);
            SimPlayersController.BuyTurretAsked += new TurretHandler(GUIController.DoTurretBought);
            SimPlayersController.SellTurretAsked += new TurretHandler(GUIController.DoTurretSold);
            SimPlayersController.ActivatePowerUpAsked += new PowerUpTypeSimPlayerHandler(PowerUpsController.DoActivatePowerUpAsked);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(SpaceshipsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(SpaceshipsController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(GUIController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(GUIController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(SimPlayersController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(SimPlayersController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(CollisionsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(BulletsController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(BulletsController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(PlanetarySystemController.DoPowerUpStarted);
            PowerUpsController.PowerUpStopped += new PowerUpHandler(PlanetarySystemController.DoPowerUpStopped);
            PowerUpsController.PowerUpStarted += new PowerUpHandler(LevelsController.DoPowerUpStarted);
            CollisionsController.TurretBoosted += new TurretTurretHandler(TurretsController.DoTurretBoosted);
            SimPlayersController.PlayerMoved += new SimPlayerHandler(PowerUpsController.DoPlayerMoved);
            LevelsController.NewGameState += new NewGameStateHandler(PowerUpsController.DoNewGameState);
            TurretsController.ObjectCreated += new PhysicalObjectHandler(SimPlayersController.DoObjectCreated);
            CollisionsController.BulletDeflected += new EnemyBulletHandler(BulletsController.DoBulletDeflected);
            BulletsController.ObjectDestroyed += new PhysicalObjectHandler(TurretsController.DoObjectDestroyed);
            SimPlayersController.DesactivatePowerUpAsked += new PowerUpTypeHandler(PowerUpsController.DoDesactivatePowerUpAsked);
            SimPlayersController.PlayerConnected += new SimPlayerHandler(GUIController.DoPlayerConnected);
            SimPlayersController.PlayerDisconnected += new SimPlayerHandler(GUIController.DoPlayerDisconnected);
            SimPlayersController.ShowAdvancedViewAsked += new NoneHandler(GUIController.DoShowAdvancedViewAsked);
            SimPlayersController.HideAdvancedViewAsked += new NoneHandler(GUIController.DoHideAdvancedViewAsked);
            SimPlayersController.ShowNextWaveAsked += new NoneHandler(GUIController.DoShowNextWaveAsked);
            SimPlayersController.HideNextWaveAsked += new NoneHandler(GUIController.DoHideNextWaveAsked);

            LevelsController.Initialize();
            EnemiesController.Initialize();
            TurretsController.Initialize();
            PlanetarySystemController.Initialize();
            GUIController.Initialize();
            PowerUpsController.Initialize();
            SimPlayersController.Initialize(); // Must be done after the PowerUpsController
            CollisionsController.Initialize();
        }


        public CelestialBody GetSelectedCelestialBody(Player p)
        {
            return SimPlayersController.GetSelectedCelestialBody(p);
        }


        public void Update(GameTime gameTime)
        {
            LevelsController.Update(gameTime);

            if (LevelsController.Help.Active)
                return;

            if (State == GameState.Paused)
                return;

            CollisionsController.Update();
            BulletsController.Update();
            PlanetarySystemController.Update(gameTime); // must be done before the TurretController because the celestial bodies must move before the turrets
            TurretsController.Update(gameTime); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
            EnemiesController.Update();
            SimPlayersController.Update(gameTime);
            SpaceshipsController.Update();
            MessagesController.Update(gameTime);
            GUIController.Update(gameTime);
            PowerUpsController.Update();
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

                if (p.Connected && !SimPlayersController.HasPlayer(player))
                    DoPlayerConnected(player);
                else if (!p.Connected && SimPlayersController.HasPlayer(player))
                    DoPlayerDisconnected(player);
            }
        }


        private void DoPlayerSelectionChanged(SimPlayer player)
        {
            GameAction = player.ActualSelection.GameAction;
        }


        private void DoNewGameState(GameState state)
        {
            if (state == GameState.Won || state == GameState.Lost)
            {
                EnemiesController.ObjectDestroyed -= SimPlayersController.DoObjetDestroyed;

                int levelId = LevelsController.Level.Id;
                int score = LevelsController.Level.CommonStash.TotalScore;

                if (!Main.SaveGame.HighScores.ContainsKey(levelId))
                    Main.SaveGame.HighScores.Add(levelId, new HighScores(levelId));

                Main.SaveGame.HighScores[levelId].Add(Inputs.MasterPlayer.Name, score);

                Persistence.SaveData("savePlayer");
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

            if (DebugMode && key == KeyboardConfiguration.Debug)
                CollisionsController.Debug = true;

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
            }
        }


        void InputListener.DoKeyReleased(Core.Input.Player p, Keys key)
        {
            if (this.DebugMode && key == KeyboardConfiguration.Debug)
                CollisionsController.Debug = false;

            if (LevelsController.Help.Active)
                return;
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

            if (PowerUpsController.InPowerUp)
            {
                if (button == MouseConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled();
                else if (button == MouseConfiguration.Select)
                    PowerUpsController.DoInputPressed();
            }

            if (!DemoMode)
            {
                if (button == MouseConfiguration.Select)
                    SimPlayersController.DoSelectAction((Player) p);
                
                if (button == MouseConfiguration.Cancel)
                    SimPlayersController.DoCancelAction((Player) p);

                if (button == MouseConfiguration.AdvancedView)
                    SimPlayersController.DoAdvancedViewAction((Player) p, true);
            }
        }


        void InputListener.DoMouseButtonReleased(Core.Input.Player p, MouseButton button)
        {
            if (LevelsController.Help.Active)
                return;

            if (!DemoMode && button == MouseConfiguration.AdvancedView && !DemoMode)
                SimPlayersController.DoAdvancedViewAction((Player) p, false);

            if (PowerUpsController.InPowerUp && button == MouseConfiguration.Select)
                PowerUpsController.DoInputReleased();
        }


        void InputListener.DoMouseScrolled(Core.Input.Player p, int delta)
        {
            if (LevelsController.Help.Active)
                return;

            if (DemoMode)
                SimPlayersController.DoGameAction((Player) p, delta);
            else
                SimPlayersController.DoNextOrPreviousAction((Player) p, delta);
        }


        void InputListener.DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            if (LevelsController.Help.Active)
                return;

            if (PowerUpsController.InPowerUp)
                PowerUpsController.DoInputMovedDelta(delta);

            SimPlayersController.DoMove((Player) p, ref delta);
        }


        void InputListener.DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Disconnect)
            {
                Inputs.DisconnectPlayer(p);
                return;
            }

            if (DebugMode && button == GamePadConfiguration.Debug)
                CollisionsController.Debug = true;

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

            if (PowerUpsController.InPowerUp)
            {
                if (button == GamePadConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled();
                else if (button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext)
                    PowerUpsController.DoInputPressed();
            }

            if (DemoMode)
            {
                if (button == GamePadConfiguration.SelectionNext)
                    SimPlayersController.DoGameAction((Player) p, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    SimPlayersController.DoGameAction((Player) p, -1);
            }

            else
            {
                if (button == GamePadConfiguration.Select)
                    SimPlayersController.DoSelectAction((Player) p);
                else if (button == GamePadConfiguration.Cancel)
                    SimPlayersController.DoCancelAction((Player) p);
                else if (button == GamePadConfiguration.SelectionNext)
                    SimPlayersController.DoNextOrPreviousAction((Player) p, 1);
                else if (button == GamePadConfiguration.SelectionPrevious)
                    SimPlayersController.DoNextOrPreviousAction((Player) p , - 1);

                if (button == GamePadConfiguration.AdvancedView)
                    SimPlayersController.DoAdvancedViewAction((Player) p, true);
            }
        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            if (DebugMode && button == GamePadConfiguration.Debug)
                CollisionsController.Debug = false;

            if (LevelsController.Help.Active)
                return;

            if (button == GamePadConfiguration.AdvancedView && !DemoMode)
                SimPlayersController.DoAdvancedViewAction((Player) p, false);

            if (PowerUpsController.InPowerUp && button == GamePadConfiguration.Select || button == GamePadConfiguration.SelectionNext)
                PowerUpsController.DoInputReleased();
        }


        void InputListener.DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            if (LevelsController.Help.Active)
                return;

            if (button != GamePadConfiguration.MoveCursor)
                return;

            delta *= GamePadConfiguration.Speed;

            if (PowerUpsController.InPowerUp)
                PowerUpsController.DoInputMovedDelta(delta);

            SimPlayersController.DoMove((Player) p, ref delta);
        }


        public void PlayerConnectionRequested(Core.Input.Player player)
        {
            player.Connect();
        }


        public void DoPlayerConnected(Core.Input.Player player)
        {
            SimPlayersController.AddPlayer((Player) player);
        }


        public void DoPlayerDisconnected(Core.Input.Player player)
        {
            SimPlayersController.RemovePlayer((Player) player);
        }

        #endregion
    }
}