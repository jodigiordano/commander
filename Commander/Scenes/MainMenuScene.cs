﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : Scene
    {
        private Main Main;
        private Text NewGame;
        private Text Quit;
        private Text ResumeGame;
        private Text Help;
        private Text Options;
        private Text Editor;
        private Text Title;
        private Text CurrentChoice;

        public string SelectedMusic;
        private double TimeBetweenTwoMusicChange;

        private AnimationTransition AnimationTransition;
        private string TransitionChoice;

        private Simulation Simulation;


        public MainMenuScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Menu";

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

            ScenarioDescriptor descripteurScenario = ScenariosFactory.getDescripteurMenu();

#if !DEBUG
            descripteurScenario.PlanetarySystem[5].CanSelect = false;
#endif

            //Simulation = new Simulation(Main, this, ScenariosFactory.getDescripteurTestsPerformance())
            Simulation = new Simulation(Main, this, descripteurScenario)
            {
                PositionCurseur = new Vector3(400, 20, 0),
                Players = Main.Players,
                DemoMode = true
            };
            Simulation.Initialize();

            AnimationTransition = new AnimationTransition(500, Preferences.PrioriteTransitionScene)
            {
                Scene = this
            };

            SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(SelectedMusic);
            TimeBetweenTwoMusicChange = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);

            CurrentChoice = null;
        }

        
        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            TransitionChoice = "chargement";
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            TimeBetweenTwoMusicChange -= gameTime.ElapsedGameTime.TotalMilliseconds;
            Simulation.Update(gameTime);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
            AnimationTransition.Start();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            AnimationTransition.Stop();

            if (Transition == TransitionType.Out)
            {
                switch (TransitionChoice)
                {
                    case "save the\nworld": Visuals.Transite("MenuToNouvellePartie"); break;
                    case "help": Visuals.Transite("MenuToAide"); break;
                    case "options": Visuals.Transite("MenuToOptions"); break;
                    case "editor": Visuals.Transite("MenuToEditeur"); break;
                    case "chargement": Visuals.Transite("MenuToChargement"); break;

                    case "quit":
                        if (Preferences.Target == Setting.Xbox360 && Main.TrialMode.Active)
                            Visuals.Transite("MenuToAcheter");
                        else
                            Main.Exit();
                        break;

                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                        {
                            Main.GameInProgress.State = GameState.Running;
                            Visuals.Transite("MenuToPartie");
                        }
                        break;
                }
            }

            Transition = TransitionType.None;
        }


        public void ChangeMusic()
        {
            if (TimeBetweenTwoMusicChange > 0)
                return;

            Audio.StopMusic(SelectedMusic, true, Preferences.TimeBetweenTwoMusics - 50);
            string ancienneMusique = SelectedMusic;
            SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(SelectedMusic);
            Main.AvailableMusics.Add(ancienneMusique);
            Audio.PlayMusic(SelectedMusic, true, 1000, true);
            TimeBetweenTwoMusicChange = Preferences.TimeBetweenTwoMusics;
        }


        //public override void Show()
        //{
        //    base.Add(Title);

        //    Simulation.Show();
        //}


        //public override void Hide()
        //{
        //    base.Remove(Title);

        //    Simulation.Hide();
        //}


        protected override void UpdateVisual()
        {
            //if (CurrentChoice != null)
            //{
            //    base.Remove(CurrentChoice);
            //    CurrentChoice = null;
            //}

            if (Simulation.SelectedCelestialBody != null)
            {
                switch (Simulation.SelectedCelestialBody.Nom)
                {
                    case "save the\nworld": Add(NewGame); CurrentChoice = NewGame; break;
                    case "quit": Add(Quit); CurrentChoice = Quit; break;
                    case "help": Add(Help); CurrentChoice = Help; break;
                    case "options": Add(Options); CurrentChoice = Options; break;
                    case "editor":

                        if (Preferences.Debug)
                        {
                            Add(Editor);
                            CurrentChoice = Editor;
                        }

                        break;

                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                        {
                            Add(ResumeGame);
                            CurrentChoice = ResumeGame;
                        }

                        break;
                }
            }

            Add(Title);
            Simulation.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Transition = TransitionType.In;

            if (!Audio.IsMusicPlaying(SelectedMusic))
                Audio.PlayMusic(SelectedMusic, true, 1000, true);
            else
                Audio.UnpauseMusic(SelectedMusic, true, 1000);

            Input.AddListener(Simulation);
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();

            if (Simulation.SelectedCelestialBody != null && Simulation.SelectedCelestialBody.Nom == "resume game" && Main.GameInProgress != null && !Main.GameInProgress.IsFinished)
                Audio.PauseMusic(SelectedMusic, true, 1000);

            Input.RemoveListener(Simulation);
        }


        #region Input Handling

        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
                beginTransition();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
                beginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.ChangeMusic)
                ChangeMusic();
        }


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            if (Simulation.SelectedCelestialBody == null)
                return;

#if !DEBUG
            if (Simulation.SelectedCelestialBody.Nom == "editor")
                return;
#endif

            if ((Simulation.SelectedCelestialBody.Nom == "resume game" &&
                Main.GameInProgress != null &&
                !Main.GameInProgress.IsFinished) ||
                
                (Simulation.SelectedCelestialBody.Nom != "resume game"))
            {
                Transition = TransitionType.Out;
                TransitionChoice = Simulation.SelectedCelestialBody.Nom;
            }
        }

        #endregion
    }
}