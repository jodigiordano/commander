namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class WorldScene : Scene
    {
        private Simulator Simulator;
        private WorldDescriptor Descriptor;
        private Dictionary<string, string> Warps;
        private Dictionary<string, LevelDescriptor> LevelsDescriptors;
        private List<KeyAndValue<CelestialBody, Image>> LevelCompletitionStates;
        private Dictionary<int, Text> LevelsNumbers;
        private Dictionary<CelestialBody, string> WarpsCelestialBodies;
        private Dictionary<int, CelestialBody> CelestialBodies;
        private LevelStates States;


        public WorldScene(WorldDescriptor descriptor) :
            base(1280, 720)
        {
            Name = "World" + descriptor.Id;
            Descriptor = descriptor;
            Warps = new Dictionary<string, string>();
            LevelsDescriptors = new Dictionary<string, LevelDescriptor>();
            LevelCompletitionStates = new List<KeyAndValue<CelestialBody, Image>>();
            LevelsNumbers = new Dictionary<int, Text>();
            WarpsCelestialBodies = new Dictionary<CelestialBody, string>();
            CelestialBodies = new Dictionary<int, CelestialBody>();
            States = new LevelStates(this);
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

            // Level numbers
            foreach (var level in Descriptor.Levels)
            {
                var cb = CelestialBodies[level.Key];

                LevelsNumbers.Add(level.Key, new Text(LevelsDescriptors[cb.Name].Infos.Mission, "Pixelite")
                {
                    SizeX = 3,
                    VisualPriority = cb.VisualPriority + 0.00001,
                    Alpha = 150
                }.CenterIt());
            }

            States.CelestialBodies = CelestialBodies;
            States.Descriptor = Descriptor;
            States.Initialize();
        }


        public bool Unlocked
        {
            get
            {
                int save = 0;
                bool unlocked = true;

                foreach (var level in Descriptor.UnlockedCondition)
                    if (!Main.PlayerSaveGame.Progress.TryGetValue(level, out save) || save <= 0)
                    {
                        unlocked = false;
                        break;
                    }

                return unlocked;
            }
        }


        public int WorldId
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

            // Draw level numbers
            foreach (var kvp in LevelsNumbers)
            {
                var cb = CelestialBodies[kvp.Key];

                kvp.Value.Position = cb.Position - new Vector3(0, cb.Circle.Radius + 20, 0);
                kvp.Value.Alpha = (byte) (States.GetLevelNumberAlpha(kvp.Key));
                Add(kvp.Value);
            }

            States.Draw();
        }


        public override void OnFocus()
        {
            Simulator.EnableInputs = true;

            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            InitializeLevelsStates();
            Main.SelectedWorld = this;
            Main.PlayerSaveGame.CurrentWorld = Descriptor.Id;

            Simulator.SyncPlayers();

            Main.MusicController.ResumeMusic();

            Simulator.TeleportPlayers(false);
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


        //public bool PausedGameSelected(Player p)
        //{
        //    var level = GetSelectedLevel(p);

        //    return Main.GameInProgress != null && level != null && Main.GameInProgress.Simulator.LevelDescriptor.Infos.Id == level.Infos.Id;
        //}


        public void ShowWarpBlockedMessage()
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(Inputs.MasterPlayer);

            Simulator.MessagesController.ShowMessage(c, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        private void InitializeLevelsStates()
        {
            // Level Completition State
            foreach (var kvp in LevelCompletitionStates)
            {
                var descriptor = LevelsDescriptors[kvp.Key.Name];

                int value = 0;
                bool done = Main.PlayerSaveGame.Progress.TryGetValue(descriptor.Infos.Id, out value) && value > 0;

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

            States.Sync();
        }
    }
}
