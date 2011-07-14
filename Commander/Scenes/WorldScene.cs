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
        private List<KeyAndValue<CelestialBody, Image>> LevelsStates;
        private Dictionary<CelestialBody, string> WarpsCelestialBodies;


        public WorldScene(WorldDescriptor descriptor) :
            base(1280, 720)
        {
            Name = descriptor.Name;
            Descriptor = descriptor;
            Warps = new Dictionary<string, string>();
            LevelsDescriptors = new Dictionary<string, LevelDescriptor>();
            LevelsStates = new List<KeyAndValue<CelestialBody, Image>>();
            WarpsCelestialBodies = new Dictionary<CelestialBody, string>();
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
                LevelDescriptor d = Main.LevelsFactory.GetLevelDescriptor(level);
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
                    LevelsStates.Add(new KeyAndValue<CelestialBody, Image>(celestialBody, null));

                if (celestialBody is PinkHole)
                    WarpsCelestialBodies.Add(celestialBody, celestialBody.Name);
            }
        }


        public bool Unlocked
        {
            get
            {
                int save = 0;
                bool unlocked = true;

                foreach (var level in Descriptor.UnlockedCondition)
                    if (!Main.SaveGame.Progress.TryGetValue(level, out save) || save <= 0)
                    {
                        unlocked = false;
                        break;
                    }

                return unlocked;
            }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();

            foreach (var kvp in LevelsStates)
                Add(kvp.Value);
        }


        public override void OnFocus()
        {
            Simulator.EnableInputs = true;

            InitializeLevelsStates();
            Main.SelectedWorld = Name;

            Simulator.SyncPlayers();

            Main.MusicController.ResumeMusic();
        }


        public override void OnFocusLost()
        {
            Simulator.EnableInputs = false;
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Cancel)
                DoCancelAction();

            if (button == MouseConfiguration.Select)
                DoSelectAction((Player) p);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.Cancel)
                DoCancelAction();

            if (key == KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Cancel)
                DoCancelAction();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);

            if (button == GamePadConfiguration.Select)
                DoSelectAction((Player) p);
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        private void DoCancelAction()
        {
            TransiteTo("Menu");
        }


        private void DoSelectAction(Player p)
        {
            // Select a warp
            if (WorldSelected != null)
            {
                if (WorldSelected.Unlocked)
                    TransiteTo(WorldSelected.Name);
                else
                    ShowWarpBlockedMessage();

                return;
            }

            // Select a level
            var level = GetSelectedLevel(p);

            if (level != null)
            {
                GameScene currentGame = Main.GameInProgress;

                if (currentGame != null && 
                    !currentGame.IsFinished &&
                    currentGame.Simulator.LevelDescriptor.Infos.Id == level.Infos.Id &&
                    Simulator.GameAction == PausedGameChoice.Resume)
                {
                    currentGame.Simulator.State = GameState.Running;
                    Main.MusicController.PauseMusic();
                    TransiteTo("Partie");
                    return;
                }

                if (currentGame != null)
                {
                    currentGame.MusicController.StopMusic(true);
                }

                currentGame = new GameScene(level);
                Main.GameInProgress = currentGame;
                currentGame.Simulator.AddNewGameStateListener(DoNewGameState);
                Simulator.MessagesController.StopPausedMessage();

                if (Visuals.GetScene("Partie") == null)
                    Visuals.AddScene(currentGame);
                else
                    Visuals.UpdateScene("Partie", currentGame);

                TransiteTo("Partie");
                Main.MusicController.PauseMusic();

                return;
            }
        }


        private void DoNewGameState(GameState gameState)
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


        public bool PausedGameSelected(Player p)
        {
            var level = GetSelectedLevel(p);

            return Main.GameInProgress != null && level != null && Main.GameInProgress.Simulator.LevelDescriptor.Infos.Id == level.Infos.Id;
        }


        public void ShowWarpBlockedMessage()
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(Inputs.MasterPlayer);

            Simulator.MessagesController.ShowMessage(c, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        private void InitializeLevelsStates()
        {
            foreach (var kvp in LevelsStates)
            {
                var descriptor = LevelsDescriptors[kvp.Key.Name];

                int value = 0;
                bool done = Main.SaveGame.Progress.TryGetValue(descriptor.Infos.Id, out value) && value > 0;

                kvp.Value = new Image((done) ? "LevelDone" : "LevelNotDone",
                    kvp.Key.Position + new Vector3(kvp.Key.Circle.Radius, kvp.Key.Circle.Radius, 0) +
                    ((kvp.Key.Circle.Radius < (int) Size.Normal) ? new Vector3(-10, -10, 0) : new Vector3(-30, -30, 0)));

                kvp.Value.VisualPriority = kvp.Key.Image.VisualPriority - 0.0001f;
                kvp.Value.SizeX = (kvp.Key.Circle.Radius < (int) Size.Normal) ? 0.5f : 0.80f;
            }


            foreach (var warp in WarpsCelestialBodies)
                ((PinkHole) warp.Key).Couleur = (((WorldScene) Visuals.GetScene(warp.Value)).Unlocked ? new Color(255, 0, 255) : new Color(255, 0, 0));
        }
    }
}
