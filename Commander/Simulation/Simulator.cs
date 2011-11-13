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
        public bool MultiverseMode;
        public bool EditorMode;
        public bool EditMode;
        public bool EditorEditingMode       { get { return EditorMode && EditMode; } }
        public bool EditorPlaytestingMode   { get { return EditorMode && !EditMode; } }
        public bool GameMode                { get { return !DemoMode; } }
        public bool MainMenuMode            { get { return DemoMode && !WorldMode && !CutsceneMode && !MultiverseMode && !EditorMode; } }
        public bool EditorWorldMode         { get { return WorldMode && EditorMode; } }


        private bool canSelectCelestialBodies;
        public bool CanSelectCelestialBodies { get { return canSelectCelestialBodies && !PanelsController.IsPanelVisible; } set { canSelectCelestialBodies = value; } }
        public bool AsteroidBeltOverride;


        public GameState State { get { return LevelsController.State; } set { LevelsController.State = value; } }

        internal Data Data;

        public PlanetarySystemController PlanetarySystemController;
        public MessagesController MessagesController;
        private ObjectivesController LevelsController;
        private EnemiesController EnemiesController;
        private BulletsController BulletsController;
        private CollisionsController CollisionsController;
        private SimPlayersController SimPlayersController;
        private TurretsController TurretsController;
        private SpaceshipsController SpaceshipsController;   
        private GUIController GUIController;
        private PowerUpsController PowerUpsController;
        internal EditorController EditorController;
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
                    "love",
                    "teleport"
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

            Data = new Data(this);
            Data.Level = new Level(this, descriptor);

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
            LevelsController = new ObjectivesController(this);
            EditorController = new EditorController(this);
            PanelsController = new PanelsController(this);
            AudioController = new AudioController(this);
            CameraController = new Simulation.CameraController(this);

            DebugMode = Preferences.Debug;
            WorldMode = false;
            DemoMode = false;
            CutsceneMode = false;
            EditorMode = false;
            EditMode = false;
            MultiverseMode = false;

            AsteroidBeltOverride = false;
            CanSelectCelestialBodies = true;


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
            TurretsController.TurretReactivated += new TurretHandler(SimPlayersController.DoTurretReactivated);
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
            SimPlayersController.PlayerMoved += new SimPlayerHandler(EditorController.DoPlayerMoved);
            EditorController.EditorCommandExecuted += new EditorCommandHandler(PlanetarySystemController.DoEditorCommandExecuted); //must be executed before sync of gui
            EditorController.EditorCommandExecuted += new EditorCommandHandler(PowerUpsController.DoEditorCommandExecuted); //must be done before the Players Controller
            EditorController.EditorCommandExecuted += new EditorCommandHandler(SimPlayersController.DoEditorCommandExecuted); //must be done before the GUI
            EditorController.EditorCommandExecuted += new EditorCommandHandler(LevelsController.DoEditorCommandExecuted); // must be donne before the GUI
            EditorController.EditorCommandExecuted += new EditorCommandHandler(GUIController.DoEditorCommandExecuted);
            EditorController.EditorCommandExecuted += new EditorCommandHandler(PanelsController.DoEditorCommandExecuted);
            SimPlayersController.ObjectCreated += new PhysicalObjectHandler(BulletsController.DoObjectCreated);
            PanelsController.PanelOpened += new StringHandler(AudioController.DoPanelOpened);
            PanelsController.PanelClosed += new StringHandler(AudioController.DoPanelClosed);
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

            Data.Initialize();

            PowerUpsFactory.HumanBattleship = GUIController.HumanBattleship;
            CollisionsController.Turrets = TurretsController.Turrets;
            CollisionsController.Minerals = EnemiesController.Minerals;

            SimPlayersController.ActivesPowerUps = PowerUpsController.ActivesPowerUps;
            TurretsController.PlanetarySystemController = PlanetarySystemController;
            GUIController.AvailablePowerUps = SimPlayersController.AvailablePowerUps;
            GUIController.AvailableTurrets = SimPlayersController.AvailableTurrets;
            GUIController.Turrets = TurretsController.Turrets;
            SpaceshipsController.PowerUpsBattleship = GUIController.HumanBattleship;
            SpaceshipsController.Minerals = EnemiesController.Minerals;
            MessagesController.Turrets = TurretsController.Turrets;
            GUIController.EnemiesData = EnemiesController.EnemiesData;
            AudioController.EnemiesData = EnemiesController.EnemiesData;
            AudioController.CameraData = CameraController.CameraData;

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
            PanelsController.Initialize();

            if (EditorMode)
                EditorController.Initialize(); // Must be done after PanelsController
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
            CameraController.DoFocusGained();

            GUIController.TeleportPlayers(false);

            if (Inputs.ConnectedPlayers.Count > 0)
                AudioController.TeleportPlayers(false);
        }


        public void OnFocusLost()
        {
            EnableInputs = false;
            AudioController.DoFocusLost();

            GUIController.TeleportPlayers(true);

            if (Inputs.ConnectedPlayers.Count > 0)
                AudioController.TeleportPlayers(true);
        }


        public bool SpawnEnemies
        {
            set { EnemiesController.SpawnEnemies = value; }
        }


        public CelestialBody GetSelectedCelestialBody(Commander.Player p)
        {
            return Data.Players.ContainsKey(p) ? Data.Players[p].ActualSelection.CelestialBody : null;
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
                var player = (Commander.Player) p;

                if (p.State == PlayerState.Connected && !Data.HasPlayer(player))
                    DoPlayerConnected(player);
                else if (p.State == PlayerState.Disconnected && Data.HasPlayer(player))
                    DoPlayerDisconnected(player);
            }

            if (DemoMode)
            {
                GUIController.SyncNewGameMenu();
            }
        }


        public void MovePlayers(Vector3 position)
        {
            SimPlayersController.MovePlayers(position);
        }


        public void ShowHelpBarMessage(Commander.Player p, HelpBarMessage message)
        {
            GUIController.ShowHelpBarMessage(p, message);
        }


        public void ShowPanel(string type, Vector3 position)
        {
            PanelsController.ShowPanel(type, position);
        }


        internal void TriggerNewGameState(GameState state)
        {
            State = state;
            LevelsController.TriggerNewGameState(state);
        }


        private void DoNewGameState(GameState state)
        {
            if (state == GameState.Won)
            {
                Main.PlayersController.UpdateProgress(
                    Main.CurrentWorld.World.Id,
                    Data.Level.Descriptor.Infos.Id,
                    Inputs.MasterPlayer.Name, Data.Level.CommonStash.TotalScore);
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

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (key == player.KeyboardConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer);

                if (key == player.KeyboardConfiguration.Back)
                    PanelsController.CloseCurrentPanel();

                return;
            }

            if (key == player.KeyboardConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, true);

            // Fire
            if (key == player.KeyboardConfiguration.Fire)
                SimPlayersController.Fire(player);

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

            if (key == player.KeyboardConfiguration.QuickToggle)
                SimPlayersController.DoToggleChoice(player, 1);
            else if (key == player.KeyboardConfiguration.SelectionNext)
                SimPlayersController.DoToggleChoice(player, 1);
            else if (key == player.KeyboardConfiguration.SelectionPrevious)
                SimPlayersController.DoToggleChoice(player, -1);

            if (EditorMode)
            {
                if (key == player.KeyboardConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
                else if (key == player.KeyboardConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (key == player.KeyboardConfiguration.Back)
            {
                TriggerNewGameState(GameState.Paused);
                PanelsController.ShowPanel("Pause", player.Position);
            }

            if (EditorMode && DemoMode)
                return;

            if (key == player.KeyboardConfiguration.Select)
                SimPlayersController.DoSelectAction(player);                
            else if (key == player.KeyboardConfiguration.Cancel)
                SimPlayersController.DoCancelAction(player);
        }


        void InputListener.DoKeyPressed(Core.Input.Player p, Keys key)
        {
            // Start Moving
            var player = (Commander.Player) p;
            var simPlayer = Data.GetPlayer(player);

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


        private void MoveKeyboard(Commander.Player p)
        {
            var player = (Commander.Player) p;
            var simPlayer = Data.GetPlayer(player);

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

            if (EditorMode && delta != Vector3.Zero)
                EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

            if (simPlayer.PowerUpInUse != PowerUpType.None)
                PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

            SimPlayersController.DoMoveDelta(player, ref delta);

            if (simPlayer.Firing)
                SimPlayersController.DoDirectionDelta(player, ref simPlayer.LastMouseDirection);

            simPlayer.MovingLeft = simPlayer.MovingRight = simPlayer.MovingUp = simPlayer.MovingDown = false;
        }


        void InputListener.DoKeyReleased(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (DebugMode && key == player.KeyboardConfiguration.Debug)
                CollisionsController.Debug = false;

            // Stop Moving
            var simPlayer = Data.GetPlayer(player);

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
                SimPlayersController.StopFire(player);
        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == player.MouseConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer);

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

            if (EditorEditingMode)
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

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (EditorMode)
            {
                if (button == player.MouseConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (simPlayer.Firing)
                SimPlayersController.StopFire(player);

            if (EditorEditingMode)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None && button == player.MouseConfiguration.Select)
                PowerUpsController.DoInputReleased(simPlayer);
        }


        void InputListener.DoMouseScrolled(Core.Input.Player p, int delta)
        {
            var player = (Commander.Player) p;
            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            SimPlayersController.DoToggleChoice(player, delta);
        }


        void InputListener.DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            var player = (Commander.Player) p;
            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            simPlayer.LastMouseDirection = delta;

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
                    Main.WorldsFactory.Initialize();
                }
            }

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (PanelsController.IsPanelVisible)
            {
                if (button == player.GamepadConfiguration.Select)
                    PanelsController.DoPanelAction(simPlayer);

                else if (button == player.GamepadConfiguration.Back)
                    PanelsController.CloseCurrentPanel();

                return;
            }

            if (button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                SimPlayersController.DoToggleChoice(player, 1);
            else if (button == player.GamepadConfiguration.SelectionPrevious || button == player.GamepadConfiguration.AlternateSelectionPrevious)
                SimPlayersController.DoToggleChoice(player, -1);

            if (EditorMode)
            {
                if (button == player.GamepadConfiguration.Select)
                    EditorController.DoSelectAction(simPlayer);
            }

            if (EditorEditingMode)
                return;

            if (simPlayer.PowerUpInUse != PowerUpType.None)
            {
                if (button == player.GamepadConfiguration.Cancel)
                    PowerUpsController.DoInputCanceled(simPlayer);
                else if (button == player.GamepadConfiguration.Select || button == player.GamepadConfiguration.SelectionNext || button == player.GamepadConfiguration.AlternateSelectionNext)
                    PowerUpsController.DoInputPressed(simPlayer);
            }

            if (button == player.GamepadConfiguration.Select)
                SimPlayersController.DoSelectAction(player);

            else if (button == player.GamepadConfiguration.AlternateSelect)
                SimPlayersController.DoAlternateAction(player);

            else if (button == player.GamepadConfiguration.Cancel)
                SimPlayersController.DoCancelAction(player);

            else if (button == player.GamepadConfiguration.Back)
            {
                TriggerNewGameState(GameState.Paused);
                PanelsController.ShowPanel("Pause", player.Position);
            }

            else if (button == player.GamepadConfiguration.AdvancedView)
                SimPlayersController.DoAdvancedViewAction(player, true);
        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (DebugMode && button == player.GamepadConfiguration.Debug)
                CollisionsController.Debug = false;

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;


            if (EditorMode)
            {
                if (button == player.GamepadConfiguration.Cancel)
                    EditorController.DoCancelAction(simPlayer);
            }

            if (EditorEditingMode)
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

            var simPlayer = Data.GetPlayer(player);

            if (simPlayer == null) // disconnected
                return;

            if (button == player.GamepadConfiguration.MoveCursor)
            {
                delta *= player.MovingSpeed * 10;

                if (EditorMode)
                    EditorController.DoPlayerMovedDelta(simPlayer, ref delta);

                if (simPlayer.PowerUpInUse != PowerUpType.None)
                    PowerUpsController.DoPlayerMovedDelta(simPlayer, delta);

                SimPlayersController.DoMoveDelta(player, ref delta);
            }

            else if (button == player.GamepadConfiguration.DirectionCursor)
            {
                SimPlayersController.DoDirectionDelta(player, ref delta);

                if (delta == Vector3.Zero)
                    SimPlayersController.StopFire(player);
                else
                    SimPlayersController.Fire(player);
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