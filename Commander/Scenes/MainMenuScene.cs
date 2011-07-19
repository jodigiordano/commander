namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : Scene
    {
        private Simulator Simulator;

        private Text NewGame;
        private Text Quit;
        private Text ResumeGame;
        private Text Help;
        private Text Options;
        private Text Editor;
        private Text Title;

        private enum State
        {
            ConnectPlayer,
            ConnectingPlayer,
            LoadSaveGame,
            PlayerConnected
        }

        private State SceneState;
        private Translator PressStart;


        public MainMenuScene()
            : base(1280, 720)
        {
            Name = "Menu";

            NewGame = new Text("save the\nworld", "Pixelite", Color.White, new Vector3(-220, -170, 0))
            {
                SizeX = 4f,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            ResumeGame = new Text("resume game", "Pixelite", Color.White, new Vector3(-460, 75, 0))
            {
                SizeX = 4,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            Options = new Text("options\nn'stuff", "Pixelite", Color.White, new Vector3(-50, -200, 0))
            {
                SizeX = 4f,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            Help = new Text("help", "Pixelite", Color.White, new Vector3(-350, 120, 0))
            {
                SizeX = 4,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            Quit = new Text("quit", "Pixelite", Color.White, new Vector3(140, 160, 0))
            {
                SizeX = 4,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            Editor = new Text("editor", "Pixelite", Color.White, new Vector3(-600, 300, 0))
            {
                SizeX = 4,
                VisualPriority = Preferences.PrioriteFondEcran - 0.01f,
                Origin = Vector2.Zero
            };

            Title = new Text("Commander", "PixelBig", Color.White, Vector3.Zero)
            {
                SizeX = 4,
                VisualPriority = Preferences.PrioriteGUIMenuPrincipal,
            };
            Title.Origin = Title.Center;

            LevelDescriptor levelDescriptor = LevelsFactory.GetMenuDescriptor();

#if !DEBUG
            levelDescriptor.PlanetarySystem[5].CanSelect = false;
#endif

            Simulator = new Simulator(this, levelDescriptor)
            {
                DemoMode = true,
                EnableInputs = false
            };
            Simulator.Initialize();
            Inputs.AddListener(Simulator);

            SceneState = State.ConnectPlayer;
            InitPressStart();
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();

            switch (SceneState)
            {
                case State.ConnectPlayer:                    
                    VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
                    VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
                    VisualEffects.Add(Title, Core.Visual.VisualEffects.Fade(Title.Alpha, 255, 0, 1000)); 
                    SceneState = State.ConnectingPlayer;
                    break;

                case State.ConnectingPlayer:
                    PressStart.Update();
                    break;


                case State.LoadSaveGame:
                    if (Persistence.DataLoaded("savePlayer"))
                    {
                        VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(PressStart.PartieTraduite.Alpha, 0, 1000));
                        VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(PressStart.PartieTraduite.Alpha, 0, 1000));
                        VisualEffects.Add(Title, Core.Visual.VisualEffects.FadeOutTo0(Title.Alpha, 0, 1000)); 

                        if (!Simulator.EnableInputs)
                            Simulator.SyncPlayers();

                        Simulator.EnableInputs = true;
                        SceneState = State.PlayerConnected;

                        Simulator.ShowHelpBarMessage(HelpBarMessage.MoveYourSpaceship);
                    }
                    break;
            }
        }


        protected override void UpdateVisual()
        {
            foreach (var player in Inputs.ConnectedPlayers)
            {
                CelestialBody c = Simulator.GetSelectedCelestialBody(player);

                if (c != null)
                {
                    switch (c.Name)
                    {
                        case "save the\nworld": Add(NewGame); break;
                        case "quit": Add(Quit); break;
                        case "help": Add(Help); break;
                        case "options": Add(Options); break;
                        case "editor":

                            if (Preferences.Debug)
                                Add(Editor);

                            break;

                        case "resume game":
                            if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                                Add(ResumeGame);

                            break;
                    }
                }
            }

            Add(Title);
            Simulator.Draw();
            PressStart.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            Main.MusicController.PlayMusic(false);

            if (Inputs.ConnectedPlayers.Count == 0)
            {
                InitPressStart();
                SceneState = State.ConnectPlayer;
                Simulator.EnableInputs = false;
            }

            else
            {
                Simulator.EnableInputs = true;
            }
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Main.MusicController.PauseMusic();

            Simulator.EnableInputs = false;
        }


        private void InitPressStart()
        {
            PressStart = new Translator
            (this, new Vector3(0, 50, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), (Preferences.Target == Core.Utilities.Setting.Xbox360) ? "Press a button to start your engine" : "Click a button to start your engine", 3, true, 3000, 250, 0.3f);
            PressStart.Centre = true;
        }


        #region Input Handling

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
            if (!Persistence.DataLoaded("savePlayer"))
                Persistence.LoadData("savePlayer");

            SceneState = State.LoadSaveGame;
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
                BeginTransition((Player) p);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Select)
                BeginTransition((Player) p);

            if (button == GamePadConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
            {
                InitPressStart();
                SceneState = State.ConnectPlayer;
                Simulator.EnableInputs = false;
                Simulator.SyncPlayers();
            }
        }


        private void BeginTransition(Player player)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(player);

            if (c == null)
                return;

#if !DEBUG
            if (c.Name == "editor")
                return;
#endif

            switch (c.Name)
            {
                case "save the\nworld": TransiteTo("Intro"); break;
                case "help": TransiteTo("Aide"); break;
                case "options": TransiteTo("Options"); break;
                case "editor": TransiteTo("Editeur"); break;

                case "quit":
                    if (Preferences.Target == Core.Utilities.Setting.Xbox360 && Main.TrialMode.Active)
                        TransiteTo("Acheter");
                    else
                        Main.Instance.Exit();
                    break;

                case "resume game":
                    if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                    {
                        Main.GameInProgress.State = GameState.Running;
                        Main.MusicController.PauseMusic();
                        TransiteTo("Partie");
                    }
                    break;
            }
        }

        #endregion
    }
}
