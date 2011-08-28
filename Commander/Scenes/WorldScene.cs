namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class WorldScene : CommanderScene
    {
        public LevelStates LevelStates;
        public Simulator Simulator;
        public bool NeedReinit;
        public bool CanGoBackToMainMenu;

        private WorldDescriptor Descriptor;
        private Dictionary<string, string> Warps;
        private Dictionary<string, LevelDescriptor> LevelsDescriptors;
        private List<KeyAndValue<CelestialBody, Image>> LevelCompletitionStates;
        private Dictionary<CelestialBody, string> WarpsCelestialBodies;
        private Dictionary<int, CelestialBody> CelestialBodies;


        public WorldScene(WorldDescriptor descriptor) :
            base("World" + descriptor.Id)
        {
            Descriptor = descriptor;
            Warps = new Dictionary<string, string>();
            LevelsDescriptors = new Dictionary<string, LevelDescriptor>();
            LevelCompletitionStates = new List<KeyAndValue<CelestialBody, Image>>();
            WarpsCelestialBodies = new Dictionary<CelestialBody, string>();
            CelestialBodies = new Dictionary<int, CelestialBody>();
            LevelStates = new LevelStates(this);
            NeedReinit = false;
            CanGoBackToMainMenu = true;
        }


        public void Initialize()
        {
            // Initialize the simulator
            Simulator = new Simulator(this, Main.LevelsFactory.GetLevelDescriptor(Descriptor.Layout))
            {
                DemoMode = true,
                WorldMode = true,
                AvailableLevelsDemoMode = LevelsDescriptors
            };

            Simulator.Initialize();
            Inputs.AddListener(Simulator);
            Simulator.EnableInputs = false;
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            // Initialize the descriptions of each level (name, difficulty, highscore, etc.)
            foreach (var level in Descriptor.Levels)
            {
                LevelDescriptor d = Main.LevelsFactory.GetLevelDescriptor(level.Key);
                LevelsDescriptors.Add(d.Infos.Mission, d);
            }

            foreach (var level in Descriptor.Warps)
            {
                LevelDescriptor d = Main.LevelsFactory.GetLevelDescriptor(level.Key);
                LevelsDescriptors.Add(d.Infos.Mission, d);
                Warps.Add(d.Infos.Mission, level.Value);
            }

            // Keep track of celestial bodies and pink holes
            InitializeCelestialBodies();

            LevelStates.CelestialBodies = CelestialBodies;
            LevelStates.Descriptor = Descriptor;
            LevelStates.LevelsDescriptors = LevelsDescriptors;
            LevelStates.Initialize();
        }


        public bool CanSelectCelestialBodies
        {
            set { Simulator.CanSelectCelestialBodies = value; }
        }


        public bool Unlocked
        {
            get
            {
                bool unlocked = true;

                foreach (var level in Descriptor.UnlockedCondition)
                    if (!Main.SaveGameController.IsLevelUnlocked(level))
                    {
                        unlocked = false;
                        break;
                    }

                return unlocked;
            }
        }


        public int Id
        {
            get { return Descriptor.Id; }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();

            LevelStates.Draw();
        }


        public override void OnFocus()
        {
            if (NeedReinit)
            {
                Simulator.Initialize();
                InitializeCelestialBodies();

                NeedReinit = false;
            }

            Simulator.EnableInputs = true;

            //

            InitializeLevelsStates();
            Main.SelectedWorld = this;
            Main.SaveGameController.PlayerSaveGame.CurrentWorld = Descriptor.Id;

            Simulator.SyncPlayers();

            Main.MusicController.ResumeMusic();

            if (Main.GameInProgress != null)
            {
                var cb = CelestialBodies[Main.GameInProgress.Level.Infos.Id];
                Simulator.TeleportPlayers(false, cb.Position + new Vector3(0, cb.Circle.Radius + 30, 0));
            }
            else
                Simulator.TeleportPlayers(false);

            //if (LastLevelTerminated)
            if (Main.GameInProgress != null)
                Add(Main.LevelsFactory.GetEndOfWorldAnimation(this));
        }


        public override void OnFocusLost()
        {
            Simulator.EnableInputs = false;

            Simulator.TeleportPlayers(true);
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
                DoSelectAction((Player) p);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.Back)
                DoBackAction();

            if (key == KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Back)
                DoBackAction();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);

            if (button == GamePadConfiguration.Select)
                DoSelectAction((Player) p);
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player player, Keys key)
        {
            if (player.State == PlayerState.Disconnected)
                player.Connect();
        }


        public override void PlayerMouseConnectionRequested(Core.Input.Player player, MouseButton button)
        {
            if (player.State == PlayerState.Disconnected)
                player.Connect();
        }


        public override void PlayerGamePadConnectionRequested(Core.Input.Player player, Buttons button)
        {
            if (player.State == PlayerState.Disconnected)
                player.Connect();
        }


        public override void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Player) p;

            player.ChooseAssets();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        private void DoBackAction()
        {
            if (!CanGoBackToMainMenu)
                return;

            TransiteTo("Menu");
        }


        private void DoSelectAction(Player p)
        {
            // Select a warp
            if (WorldSelected != null)
            {
                if (WorldSelected.Unlocked)
                    TransiteTo(WorldSelected.Name + "Annunciation");
                else
                    ShowWarpBlockedMessage();

                return;
            }

            // Select a level
            var level = GetSelectedLevel(p);

            if (level != null)
            {
                GameScene currentGame = Main.GameInProgress;

                // Resume Game
                if (Main.GamePausedToWorld &&
                    currentGame.Simulator.LevelDescriptor.Infos.Id == level.Infos.Id &&
                    Simulator.PausedGameChoice == PausedGameChoice.Resume)
                {
                    currentGame.Simulator.State = GameState.Running;
                    Main.MusicController.PauseMusic();
                    TransiteTo(currentGame.Name);
                    return;
                }

                // Start a new game
                if (currentGame != null)
                    currentGame.MusicController.StopMusic(true);

                currentGame = new GameScene("Game1", level);
                Main.GameInProgress = currentGame;
                currentGame.Simulator.AddNewGameStateListener(DoNewGameState);

                if (Visuals.GetScene(currentGame.Name) == null)
                    Visuals.AddScene(currentGame);
                else
                    Visuals.UpdateScene(currentGame.Name, currentGame);

                TransiteTo(currentGame.Name);
                Main.MusicController.PauseMusic();

                return;
            }
        }


        public void DoNewGameState(GameState gameState)
        {
            InitializeLevelsStates();
        }


        private LevelDescriptor GetSelectedLevel(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return c != null ? LevelsDescriptors[c.Name] : null;
        }


        public WorldScene WorldSelected
        {
            get
            {
                CelestialBody c = Simulator.GetSelectedCelestialBody(Inputs.MasterPlayer);

                return (c != null && c is PinkHole) ? (WorldScene) Visuals.GetScene(Warps[c.Name]) : null;
            }
        }


        public void ShowWarpBlockedMessage()
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(Inputs.MasterPlayer);

            Simulator.MessagesController.ShowMessage(c, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        private bool LastLevelTerminated
        {
            get
            {
                return Main.GameInProgress != null && Main.GameInProgress.State == GameState.Won && Main.GameInProgress.Level.Infos.Id == Descriptor.LastLevelId;
            }
        }


        private void InitializeLevelsStates()
        {
            // Level Completition State
            foreach (var kvp in LevelCompletitionStates)
            {
                var descriptor = LevelsDescriptors[kvp.Key.Name];

                bool done = Main.SaveGameController.IsLevelUnlocked(descriptor.Infos.Id);

                kvp.Value = new Image((done) ? "LevelDone" : "LevelNotDone");

                kvp.Value.VisualPriority = kvp.Key.Image.VisualPriority - 0.0001f;
                kvp.Value.SizeX = (kvp.Key.Circle.Radius < (int) Size.Normal) ? 0.5f : 0.80f;
            }

            // Warps
            foreach (var warp in WarpsCelestialBodies)
            {
                var pinkHole = (PinkHole) warp.Key;
                var unlocked = ((WorldScene) Visuals.GetScene(warp.Value)).Unlocked;

                pinkHole.BlendType = unlocked ? BlendType.Add : BlendType.Substract;
                pinkHole.Color = unlocked ? new Color(255, 0, 255) : new Color(0, 0, 0);
            }

            LevelStates.Sync();
        }


        private void InitializeCelestialBodies()
        {
            LevelCompletitionStates.Clear();
            CelestialBodies.Clear();
            WarpsCelestialBodies.Clear();

            foreach (var celestialBody in Simulator.PlanetarySystemController.CelestialBodies)
            {
                if (LevelsDescriptors.ContainsKey(celestialBody.Name) && !(celestialBody is PinkHole))
                {
                    LevelCompletitionStates.Add(new KeyAndValue<CelestialBody, Image>(celestialBody, null));

                    CelestialBodies.Add(LevelsDescriptors[celestialBody.Name].Infos.Id, celestialBody);
                }

                if (celestialBody is PinkHole)
                    WarpsCelestialBodies.Add(celestialBody, celestialBody.Name);
            }
        }
    }
}
