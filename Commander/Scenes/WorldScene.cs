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
        public GameScene GameInProgress;
        public LevelStates LevelStates;
        public Simulator Simulator;
        public bool NeedReinit;
        public bool CanGoBackToMainMenu;
        public WorldDescriptor Descriptor;

        private enum State
        {
            Transition,
            ConnectPlayer,
            ConnectingPlayer,
            LoadSaveGame,
            PlayerConnected
        }

        private State SceneState;
        private CommanderTitle Title;
        private Dictionary<CelestialBody, int> CBtoWarp;
        private Dictionary<int, CelestialBody> LeveltoCB;
        private Dictionary<CelestialBody, int> CBtoLevel;

        public bool EditorMode;


        public WorldScene(WorldDescriptor descriptor) :
            base(Main.LevelsFactory.GetWorldStringId(descriptor.Id))
        {
            Descriptor = descriptor;
            CBtoWarp = new Dictionary<CelestialBody, int>();
            LeveltoCB = new Dictionary<int, CelestialBody>();
            CBtoLevel = new Dictionary<CelestialBody, int>();
            LevelStates = new LevelStates(this);
            NeedReinit = false;
            CanGoBackToMainMenu = Preferences.Target != Core.Utilities.Setting.ArcadeRoyale;
            EditorMode = false;

            Title = new CommanderTitle(this, new Vector3(0, -10, 0), VisualPriorities.Default.Title);
            Title.Initialize();

            SceneState = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? State.Transition : State.PlayerConnected;
        }


        public override void Initialize()
        {
            // Initialize the simulator
            Simulator = new Simulator(this, Main.LevelsFactory.GetLevelDescriptor(Descriptor.Layout))
            {
                DemoMode = true,
                WorldMode = true,
                EditorWorldMode = EditorMode,
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
            LevelStates.Descriptor = Descriptor;
            LevelStates.Initialize();

            Main.CheatsController.CheatActivated += new StringHandler(DoCheatActivated);
            Main.MusicController.AddMusic(Descriptor.Music);
            Main.MusicController.AddMusic(Descriptor.MusicEnd);

            GameInProgress = null;
        }


        public bool GamePausedToWorld
        {
            get
            {
                return !EditorMode && GameInProgress != null && GameInProgress.State == GameState.PausedToWorld;
            }
        }


        public bool GetGamePausedSelected(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return
                c != null &&
                !CBtoWarp.ContainsKey(c) &&
                GamePausedToWorld &&
                GameInProgress.Simulator.Level.Id == CBtoLevel[c];
        }


        public bool CanSelectCelestialBodies
        {
            set { Simulator.CanSelectCelestialBodies = value; }
        }


        public bool Unlocked
        {
            get
            {
                if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                    return false;

                return Main.LevelsFactory.IsWorldUnlocked(Descriptor.UnlockedCondition);
            }
        }


        public int Id
        {
            get { return Descriptor.Id; }
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


                case State.LoadSaveGame:
                    if (Main.SaveGameController.IsPlayerSaveGameLoaded)
                    {
                        Title.Hide();
                        LevelStates.Show();

                        if (!Simulator.EnableInputs)
                            Simulator.SyncPlayers();

                        Simulator.EnableInputs = true;
                        SceneState = State.PlayerConnected;

                        Simulator.ShowHelpBarMessage((Commander.Player) Inputs.MasterPlayer, HelpBarMessage.MoveYourSpaceship);
                        Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 1000);

                        Main.SaveGameController.PlayerSaveGame.CurrentWorld = Descriptor.Id;
                    }
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
            if (NeedReinit)
            {
                Simulator.Initialize();
                InitializeCelestialBodies();
                LevelStates.Alpha = 255;

                NeedReinit = false;
            }

            InitializeLevelsStates();

            Main.SelectedWorld = this;

            if (Simulator.EditorWorldMode)
            {
                // sync the level descriptor so it can be saved.
                if (GameInProgress != null && GameInProgress.Simulator.EditorState != EditorState.Playtest)
                {
                    GameInProgress.Simulator.SyncLevel();
                    Main.LevelsFactory.SetLevelDescriptor(GameInProgress.Simulator.LevelDescriptor.Infos.Id, GameInProgress.Simulator.LevelDescriptor);
                }
            }

            else if (Preferences.Target != Core.Utilities.Setting.ArcadeRoyale)
            {
                Main.SaveGameController.PlayerSaveGame.CurrentWorld = Descriptor.Id;
            }

            Simulator.OnFocus();

            if (GameInProgress != null && Descriptor.ContainsLevel(GameInProgress.Level.Infos.Id))
            {
                var cb = LeveltoCB[GameInProgress.Level.Infos.Id];
                Simulator.TeleportPlayers(false, cb.Position + new Vector3(0, cb.Circle.Radius + 30, 0));
            }
            else
                Simulator.TeleportPlayers(false);

            if (!Simulator.EditorWorldMode && Preferences.Target != Core.Utilities.Setting.ArcadeRoyale && LastLevelWon)
                Add(Main.LevelsFactory.GetEndOfWorldAnimation(this));
            else
            {
                Main.MusicController.PlayOrResume(Descriptor.Music);
                XACTAudio.ChangeCategoryVolume("Turrets", 0);
            }

            if (Inputs.ConnectedPlayers.Count == 0) //must be done after Simulator.OnFocus() to set back no input
                InitConnectFirstPlayer();
        }


        public override void OnFocusLost()
        {
            Simulator.OnFocusLost();
            Simulator.TeleportPlayers(true);
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
                ReloadPlayerData(player);

            player.ChooseAssets();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
            {
                if (Inputs.ConnectedPlayers.Count == 0)
                    InitConnectFirstPlayer();
                else if (Main.SaveGameController.CurrentPlayer == player)
                {
                    ReloadPlayerData((Player) Inputs.MasterPlayer);
                    SceneState = State.LoadSaveGame;
                }
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
            if (EditorMode)
            {
                DoSelectActionEditor(p);
                return;
            }

            // Select a warp
            var world = GetWorldSelected(p);

            if (world != null)
            {
                if (world.Unlocked)
                    TransiteTo(world.Name + "Annunciation");
                else
                    ShowWarpBlockedMessage(p);

                return;
            }

            // Select a level
            var level = GetSelectedLevel(p);

            if (level != null)
            {
                
                GameScene currentGame = GameInProgress;

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
                GameInProgress = currentGame;
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
                    Main.LevelsFactory.SetLevelDescriptor(level.Infos.Id, Main.LevelsFactory.GetEmptyDescriptor(level.Infos.Id));
                    Main.LevelsFactory.SaveDescriptorOnDisk(level.Infos.Id);
                }

                else if (Simulator.EditorWorldChoice == EditorWorldChoice.Save)
                {
                    Main.LevelsFactory.SaveDescriptorOnDisk(level.Infos.Id);
                }

                else
                {
                    GameScene currentGame = GameInProgress;

                    // Start a new game
                    if (currentGame != null)
                        currentGame.StopMusic();

                    currentGame = new GameScene("Game1", level)
                    {
                        EditorMode = EditorMode,
                        EditorState = Simulator.EditorWorldChoice == EditorWorldChoice.Edit ? EditorState.Editing : EditorState.Playtest
                    };
                    currentGame.Initialize();
                    GameInProgress = currentGame;
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
                Main.LevelsFactory.GetLevelDescriptor(CBtoLevel[c]) : null;
        }


        public WorldScene GetWorldSelected(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            return (c != null && c is PinkHole) ?
                (WorldScene) Visuals.GetScene(Main.LevelsFactory.GetWorldStringId(CBtoWarp[c])) : null;
        }


        public void ShowWarpBlockedMessage(Player p)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(p);

            Simulator.MessagesController.ShowMessage(c, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        private bool LastLevelWon
        {
            get
            {
                return
                    GameInProgress != null &&
                    GameInProgress.State == GameState.Won &&
                    GameInProgress.Level.Infos.Id == Descriptor.Levels[Descriptor.Levels.Count - 1];
            }
        }


        private void InitializeLevelsStates()
        {
            // Warps
            foreach (var w in CBtoWarp)
            {
                var pinkHole = (PinkHole) w.Key;
                var unlocked = ((WorldScene) Visuals.GetScene(Main.LevelsFactory.GetWorldStringId(w.Value))).Unlocked;

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
                    CBtoWarp.Add(c, Descriptor.Warps[warpIndex++]);

                else
                {
                    LeveltoCB.Add(Descriptor.Levels[levelIndex], c);
                    CBtoLevel.Add(c, Descriptor.Levels[levelIndex]);

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
                foreach (var l in Descriptor.Levels)
                    Main.SaveGameController.UpdateProgress(Inputs.MasterPlayer.Name, GameState.Won, l, 0);

                LevelStates.AllLevelsUnlockedOverride = true;
                InitializeLevelsStates();

                Main.SaveGameController.SaveAll();
            }
        }


        private void ReloadPlayerData(Player p)
        {
            Main.SaveGameController.ReloadPlayerData(p);
            SceneState = State.LoadSaveGame;
        }


        private void InitConnectFirstPlayer()
        {
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 0, 1000);
            Title.Initialize();
            SceneState = State.ConnectPlayer;
            Simulator.EnableInputs = false;
            Simulator.SyncPlayers();
            Camera.Position = Vector3.Zero;

            if (Main.SelectedWorld != null)
                Main.SelectedWorld.GameInProgress = null;
        }
    }
}
