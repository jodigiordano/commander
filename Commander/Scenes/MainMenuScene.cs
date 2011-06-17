namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : Scene
    {
        private Text NewGame;
        private Text Quit;
        private Text ResumeGame;
        private Text Help;
        private Text Options;
        private Text Editor;
        private Text Title;

        private Simulator Simulator;


        public MainMenuScene()
            : base(Vector2.Zero, 1280, 720)
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

            //Simulation = new Simulation(Main, this, ScenariosFactory.getDescripteurTestsPerformance())
            Simulator = new Simulator(this, levelDescriptor)
            {
                PositionCurseur = new Vector3(400, 20, 0),
                DemoMode = true
            };
            Simulator.Initialize();
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            if (Simulator.SelectedCelestialBody != null)
            {
                switch (Simulator.SelectedCelestialBody.Name)
                {
                    case "save the\nworld": Add(NewGame); break;
                    case "quit": Add(Quit); break;
                    case "help": Add(Help); break;
                    case "options": Add(Options); break;
                    case "editor":

                        if (Preferences.Debug)
                        {
                            Add(Editor);
                        }

                        break;

                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                        {
                            Add(ResumeGame);
                        }

                        break;
                }
            }

            Add(Title);
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            if (!Audio.IsMusicPlaying(Main.SelectedMusic))
                Audio.PlayMusic(Main.SelectedMusic, true, 1000, true);
            else
                Audio.ResumeMusic(Main.SelectedMusic, true, 1000);

            Inputs.AddListener(Simulator);
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();

            if (Simulator.SelectedCelestialBody != null && Simulator.SelectedCelestialBody.Name == "resume game" && Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                Audio.PauseMusic(Main.SelectedMusic, true, 1000);

            Inputs.RemoveListener(Simulator);
        }


        #region Input Handling

        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (!p.Master)
                return;

            if (button == MouseConfiguration.Select)
                beginTransition();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (!p.Master)
                return;

            if (button == GamePadConfiguration.Select)
                beginTransition();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (!p.Master)
                return;

            if (key == KeyboardConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (player.Master)
                TransiteTo("Chargement");
        }


        private void beginTransition()
        {
            if (Simulator.SelectedCelestialBody == null)
                return;

#if !DEBUG
            if (Simulator.SelectedCelestialBody.Name == "editor")
                return;
#endif

            if ((Simulator.SelectedCelestialBody.Name == "resume game" &&
                Main.GameInProgress != null &&
                !Main.GameInProgress.IsFinished) ||
                
                (Simulator.SelectedCelestialBody.Name != "resume game"))
            {
                switch (Simulator.SelectedCelestialBody.Name)
                {
                    case "save the\nworld": Visuals.Transite("Menu", "Intro"); break;
                    case "help": Visuals.Transite("Menu", "Aide"); break;
                    case "options": Visuals.Transite("Menu", "Options"); break;
                    case "editor": Visuals.Transite("Menu", "Editeur"); break;

                    case "quit":
                        if (Preferences.Target == Setting.Xbox360 && Main.TrialMode.Active)
                            Visuals.Transite("Menu", "Acheter");
                        else
                            Main.Instance.Exit();
                        break;

                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                        {
                            Main.GameInProgress.State = GameState.Running;
                            Visuals.Transite("Menu", "Partie");
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
