namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class WorldScene : CommanderScene
    {
        public World World;
        public LevelStates LevelStates;
        public Simulator Simulator;
        public bool CanGoBackToMainMenu;

        private enum State
        {
            Transition,
            ConnectPlayer,
            ConnectingPlayer,
            FadeOutTitleScreen,
            PlayerConnected
        }

        private State SceneState;
        private CommanderTitle Title;
        private Dictionary<CelestialBody, int> CBtoWarp;
        private Dictionary<int, CelestialBody> LeveltoCB;
        private Dictionary<CelestialBody, int> CBtoLevel;


        public WorldScene() :
            base("World")
        {
            CBtoWarp = new Dictionary<CelestialBody, int>();
            LeveltoCB = new Dictionary<int, CelestialBody>();
            CBtoLevel = new Dictionary<CelestialBody, int>();
            LevelStates = new LevelStates(this);
            CanGoBackToMainMenu = Preferences.Target != Core.Utilities.Setting.ArcadeRoyale;

            Title = new CommanderTitle(this, new Vector3(0, -10, 0), VisualPriorities.Default.Title);
            Title.Initialize();

            SceneState = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? State.Transition : State.PlayerConnected;
        }


        public override void Initialize()
        {
            if (Simulator != null)
                Simulator.CleanUp();

            // Initialize the simulator
            Simulator = new Simulator(this, World.Layout)
            {
                DemoMode = true,
                WorldMode = true,
                EditorWorldMode = World.EditorMode,
                //EditorMode = EditorMode,
                //EditorState = EditorState.Editing,
                AvailableLevelsWorldMode = CBtoLevel,
                AvailableWarpsWorldMode = CBtoWarp,
                EnableInputs = Preferences.Target != Core.Utilities.Setting.ArcadeRoyale
            };

            Simulator.Initialize();
            Inputs.AddListener(Simulator);
            Simulator.EnableInputs = false;
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            // Keep track of celestial bodies and pink holes
            InitializeCelestialBodies();

            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                LevelStates.AllLevelsUnlockedOverride = true;

            LevelStates.CelestialBodies = LeveltoCB;
            LevelStates.Descriptor = World.Descriptor;
            LevelStates.Initialize();
            LevelStates.Show();

            Main.CheatsController.CheatActivated += new StringHandler(DoCheatActivated);

            Main.MusicController.AddMusic(World.Descriptor.Music);
            Main.MusicController.AddMusic(World.Descriptor.MusicEnd);

            Main.CurrentGame = null;
        }


        public bool GamePausedToWorld
        {
            get
            {
                return !World.EditorMode && Main.CurrentGame != null && Main.CurrentGame.State == GameState.PausedToWorld;
            }
        }


        public bool GetGamePausedSelected(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return
                c != null &&
                !CBtoWarp.ContainsKey(c) &&
                GamePausedToWorld &&
                Main.CurrentGame.Simulator.Level.Id == CBtoLevel[c];
        }


        public bool CanSelectCelestialBodies
        {
            set { Simulator.CanSelectCelestialBodies = value; }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();

            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                Title.Update();

            switch (SceneState)
            {
                case State.ConnectPlayer:
                    Title.Show();
                    LevelStates.Hide();

                    SceneState = State.ConnectingPlayer;
                    break;

                case State.ConnectingPlayer:
                    break;


                case State.FadeOutTitleScreen:
                    Title.Hide();
                    LevelStates.Show();

                    if (!Simulator.EnableInputs)
                        Simulator.SyncPlayers();

                    Simulator.EnableInputs = true;
                    SceneState = State.PlayerConnected;

                    Simulator.ShowHelpBarMessage((Commander.Player) Inputs.MasterPlayer, HelpBarMessage.MoveYourSpaceship);
                    Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 1000);

                    Main.PlayersController.SetCampaignWorld(World.Id);
                    break;
            }
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();

            LevelStates.Draw();

            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                Title.Draw();
        }


        public override void OnFocus()
        {
            InitializeLevelsStates();

            Main.CurrentWorld = this;

            if (Simulator.EditorWorldMode)
            {
                // sync the level descriptor so it can be saved.
                if (Main.CurrentGame != null && Main.CurrentGame.Simulator.EditorState != EditorState.Playtest)
                {
                    Main.CurrentGame.Simulator.SyncLevel();
                    World.SetLevelDescriptor(Main.CurrentGame.Simulator.LevelDescriptor.Infos.Id, Main.CurrentGame.Simulator.LevelDescriptor);
                }
            }

            else if (Preferences.Target != Core.Utilities.Setting.ArcadeRoyale)
            {
                Main.PlayersController.SetCampaignWorld(World.Id);
            }

            Simulator.OnFocus();

            if (Main.CurrentGame != null && World.Descriptor.ContainsLevel(Main.CurrentGame.Level.Infos.Id))
            {
                var cb = LeveltoCB[Main.CurrentGame.Level.Infos.Id];
                Simulator.MovePlayers(cb.Position + new Vector3(0, cb.Circle.Radius + 30, 0));
            }

            if (!Simulator.EditorWorldMode && Preferences.Target != Core.Utilities.Setting.ArcadeRoyale && LastLevelWon)
                Add(Main.LevelsFactory.GetEndOfWorldAnimation(World.Id, this));
            else if (!Simulator.EditorWorldMode)
            {
                Main.MusicController.PlayOrResume(World.Descriptor.Music);
            }

            if (Inputs.ConnectedPlayers.Count == 0) //must be done after Simulator.OnFocus() to set back no input
                InitConnectFirstPlayer();
        }


        public override void OnFocusLost()
        {
            Simulator.OnFocusLost();
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            if (button == player.MouseConfiguration.Select)
                DoSelectAction(player);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Select)
                DoSelectAction(player);

            else if (key == player.KeyboardConfiguration.Back)
                DoBackAction();

            else if (key == player.KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();

            else if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (button == player.GamepadConfiguration.Back)
                DoBackAction();

            else if (button == player.GamepadConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();

            else if (button == player.GamepadConfiguration.Select)
                DoSelectAction(player);
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
                return;

            if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();

            if (player.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void PlayerMouseConnectionRequested(Core.Input.Player p, MouseButton button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void PlayerGamePadConnectionRequested(Core.Input.Player p, Buttons button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Player) p;

            if (Inputs.ConnectedPlayers.Count == 1)
                SceneState = State.FadeOutTitleScreen;

            player.ChooseAssets();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
            {
                if (Inputs.ConnectedPlayers.Count == 0)
                    InitConnectFirstPlayer();
            }

            else
            {
                if (Inputs.ConnectedPlayers.Count == 0)
                    TransiteTo("Menu");
            }
        }


        private void DoBackAction()
        {
            if (!CanGoBackToMainMenu)
                return;

            TransiteTo("Menu");
        }


        private void DoSelectAction(Player p)
        {
            if (World.EditorMode)
            {
                DoSelectActionEditor(p);
                return;
            }

            // Select a warp
            var world = GetWorldSelected(p);

            if (world != null)
            {
                if (world.Unlocked)
                {
                    Main.SetCurrentWorld(world, false);
                    TransiteTo("WorldAnnunciation");
                }
                else
                {
                    ShowWarpBlockedMessage(p);
                }

                return;
            }

            // Select a level
            var level = GetSelectedLevel(p);

            if (level != null)
            {

                GameScene currentGame = Main.CurrentGame;

                // Resume Game
                if (GamePausedToWorld &&
                    currentGame.Simulator.LevelDescriptor.Infos.Id == level.Infos.Id &&
                    Simulator.PausedGameChoice == PausedGameChoice.Resume)
                {
                    currentGame.Simulator.TriggerNewGameState(GameState.Running);
                    TransiteTo(currentGame.Name);
                    return;
                }

                // Start a new game
                if (currentGame != null)
                    currentGame.StopMusic();

                currentGame = new GameScene("Game1", level);
                currentGame.Initialize();
                Main.CurrentGame = currentGame;
                currentGame.Simulator.AddNewGameStateListener(DoNewGameState);

                if (Visuals.GetScene(currentGame.Name) == null)
                    Visuals.AddScene(currentGame);
                else
                    Visuals.UpdateScene(currentGame.Name, currentGame);

                TransiteTo(currentGame.Name);

                return;
            }
        }


        public void DoSelectActionEditor(Player p)
        {
            // Select a warp
            var world = GetWorldSelected(p);

            if (world != null)
            {
                return; //todo: go to another world?
            }

            // Select a level
            var level = GetSelectedLevel(p);

            if (level != null)
            {
                if (Simulator.EditorWorldChoice == EditorWorldChoice.Reset)
                {
                    World.SetLevelDescriptor(level.Infos.Id, Main.LevelsFactory.GetEmptyLevelDescriptor(level.Infos.Id));
                    Main.LevelsFactory.SaveWorldOnDisk(World.Id);
                }

                else if (Simulator.EditorWorldChoice == EditorWorldChoice.Save)
                {
                    Main.LevelsFactory.SaveWorldOnDisk(World.Id);
                }

                else
                {
                    GameScene currentGame = Main.CurrentGame;

                    // Start a new game
                    if (currentGame != null)
                        currentGame.StopMusic();

                    currentGame = new GameScene("Game1", level)
                    {
                        EditorMode = World.EditorMode,
                        EditorState = Simulator.EditorWorldChoice == EditorWorldChoice.Edit ? EditorState.Editing : EditorState.Playtest
                    };
                    currentGame.Initialize();
                    Main.CurrentGame = currentGame;
                    currentGame.Simulator.AddNewGameStateListener(DoNewGameState);

                    if (Visuals.GetScene(currentGame.Name) == null)
                        Visuals.AddScene(currentGame);
                    else
                        Visuals.UpdateScene(currentGame.Name, currentGame);

                    TransiteTo(currentGame.Name);
                }
            }
        }


        public void DoNewGameState(GameState gameState)
        {
            InitializeLevelsStates();
        }


        private LevelDescriptor GetSelectedLevel(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return c != null ?
                World.GetLevelDescriptor(CBtoLevel[c]) : null;
        }


        private World GetWorldSelected(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return (c != null && c is PinkHole) ?
                Main.LevelsFactory.Worlds[CBtoWarp[c]] : null;
        }


        private void ShowWarpBlockedMessage(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            Simulator.MessagesController.ShowMessage(c, "Finish all the levels\n\nto access this world!", 5000, -1);
        }


        private bool LastLevelWon
        {
            get
            {
                return
                    Main.CurrentGame != null &&
                    Main.CurrentGame.State == GameState.Won &&
                    Main.CurrentGame.Level.Infos.Id == World.LastLevel;
            }
        }


        private void InitializeLevelsStates()
        {
            // Warps
            foreach (var w in CBtoWarp)
            {
                var pinkHole = (PinkHole) w.Key;
                var unlocked = Main.LevelsFactory.Worlds[w.Value].Unlocked;

                pinkHole.BlendType = unlocked ? BlendType.Add : BlendType.Substract;
                pinkHole.Color = unlocked ? new Color(255, 0, 255) : new Color(0, 0, 0);
            }

            LevelStates.Sync();
        }


        private void InitializeCelestialBodies()
        {
            LeveltoCB.Clear();
            CBtoWarp.Clear();

            int levelIndex = 0;
            int warpIndex = 0;

            Simulator.PlanetarySystemController.CelestialBodies.Sort(delegate(CelestialBody cb1, CelestialBody cb2)
            {
                return cb1.PathPriority > cb2.PathPriority ? 1 : cb1.PathPriority < cb2.PathPriority ? -1 : 0;
            });

            foreach (var c in Simulator.PlanetarySystemController.CelestialBodies)
            {
                if (c is AsteroidBelt || c.FirstOnPath)
                    continue;

                if (c is PinkHole)
                    CBtoWarp.Add(c, World.Descriptor.Warps[warpIndex++]);

                else if (levelIndex < World.Descriptor.Levels.Count)
                {
                    LeveltoCB.Add(World.Descriptor.Levels[levelIndex], c);
                    CBtoLevel.Add(c, World.Descriptor.Levels[levelIndex]);

                    levelIndex++;
                }
            }
        }


        private void DoCheatActivated(string name)
        {
            if (!EnableInputs || LevelStates.AllLevelsUnlockedOverride)
                return;

            if (name == "AllLevelsUnlocked")
            {
                World.UnlockAllLevels();

                LevelStates.AllLevelsUnlockedOverride = true;
                InitializeLevelsStates();

                Main.PlayersController.SaveAll();
            }
        }


        private void InitConnectFirstPlayer()
        {
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 0, 1000);
            Title.Initialize();
            SceneState = State.ConnectPlayer;
            Simulator.EnableInputs = false;
            Simulator.SyncPlayers();
            Camera.Position = Vector3.Zero;

            Main.CurrentGame = null;
        }
    }
}
