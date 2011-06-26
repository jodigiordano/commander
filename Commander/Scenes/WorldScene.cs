namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Audio;
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
            base(Vector2.Zero, 1280, 720)
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
            Simulator = new Simulator(this, LevelsFactory.GetDescriptor(Descriptor.Layout))
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
                LevelDescriptor d = LevelsFactory.GetDescriptor(level);
                LevelsDescriptors.Add(d.Infos.Mission, d);
            }

            foreach (var level in Descriptor.Warps)
            {
                LevelDescriptor d = LevelsFactory.GetDescriptor(level.Key);
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

            if (!Audio.IsMusicPlaying(Main.SelectedMusic))
                Audio.ResumeMusic(Main.SelectedMusic, true, 1000);
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
                Main.ChangeMusic();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Cancel)
                DoCancelAction();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.ChangeMusic();

            if (button == GamePadConfiguration.Select)
                DoSelectAction((Player) p);
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Chargement");
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
                    Audio.PauseMusic(Main.SelectedMusic, true, 1000);
                    TransiteTo("Partie");
                    return;
                }

                if (currentGame != null)
                {
                    Audio.StopMusic(currentGame.SelectedMusic, false, 0);

                    if (!currentGame.IsFinished)
                        Main.AvailableMusics.Add(currentGame.SelectedMusic);
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
                Audio.PauseMusic(Main.SelectedMusic, true, 1000);

                return;
            }

            //else if (Main.TrialMode.Active && MondeSelectionne.LevelSelected != null && MondeSelectionne.LevelSelected.Id > 2)
            //{
            //    if (AnimationFinDemo == null)
            //    AnimationFinDemo = new AnimationCommodore(
            //        "Only the levels 1-1, 1-2 and 1-3 are available in this demo, Commander! If you want to finish the fight and save humanity, visit ephemeregames.com to buy all the levels for only 5$! By unlocking the 9 levels, you will be able to take the warp to World 2 ! Keep my website in your bookmarks if you want more infos on me, my games and my future projects.", 25000, Preferences.PrioriteGUIPanneauGeneral );
            //}



//#if XBOX
            //            else if (button == MouseConfiguration.Select &&
            //                     AnimationFinDemo != null)
            //            {
            //                try
            //                {
            //                    Guide.ShowMarketplace(Main.JoueursConnectes[0].Manette);
            //                }

//                catch (GamerPrivilegeException)
            //                {
            //                    Guide.BeginShowMessageBox
            //                    (
            //                        "Oh no!",
            //                        "You must be signed in with an Xbox Live enabled profile to buy the game. You can either:\n\n1. Go buy it directly on the marketplace (suggested).\n\n2. Restart the game and sign in with an Xbox Live profile.\n\n\nThank you for your support, commander!",
            //                        new List<string> { "Ok" },
            //                        0,
            //                        MessageBoxIcon.Warning,
            //                        null,
            //                        null);
            //            }
            //#endif
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
