namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LoadingScene : CommanderScene
    {
        private enum State
        {
            TransiteToMenu,
            Finished,
            LoadingAssets,
            LoadScenes
        }

        private EphemereGamesLogo Logo;
        private Image Background;
        private Text LoadingQuote;

        private SandGlass SandGlass;
        private State SceneState;

        private bool ScenesAreLoaded;

        private double TimeBeforeTransition = 2000;
        private double TimeBeforeTranslation = 3500;


        private static List<string> LoadingQuotes = new List<string>()
        {
            "Building a better world. Please wait.",
            "Counting pixels by hand. Please wait.",
            "Downloading the Internet. Please wait.",
            "Applying a Windows patch. Please wait.",
            "Logging in the Matrix. Please wait.",
            "Organizing the resistance. Please wait.",
            "Generating the universe. Please wait.",
            "Compiling the kernel. Please wait."
        };


        public LoadingScene()
            : base("Chargement")
        {
            EnableVisuals = true;
            EnableInputs = true;
            EnableUpdate = true;

            SandGlass = new SandGlass(this, 5000, new Vector3(0, 250, 0), 0.3f)
            {
                Color = Colors.Default.NeutralDark,
                RemainingTime = 5000,
                Alpha = 0
            };


            Logo = new EphemereGamesLogo(this, Vector3.Zero, 0.3)
            {
                Alpha = 0
            };

            Persistence.LoadPackage("principal");

            SceneState = State.LoadingAssets;

            Background = new Image("PixelBlanc", Vector3.Zero)
            {
                VisualPriority = 1,
                Size = Preferences.BackBuffer,
                Color = Color.White,
                Alpha = 0
            };

            LoadingQuote = new Text(LoadingQuotes[Main.Random.Next(0, LoadingQuotes.Count)], @"Pixelite", new Vector3(0, 150, 0))
            {
                Color = Color.Transparent,
                SizeX = 3
            }.CenterIt();

            VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 500));
            VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
            VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeTranslation - 500, 500));
            VisualEffects.Add(LoadingQuote, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeTranslation, 1000));
            SandGlass.FadeIn(TimeBeforeTranslation, 1000);
            
            ScenesAreLoaded = false;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            TimeBeforeTranslation -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Logo.Update();

            switch (SceneState)
            {
                case State.LoadingAssets:

                    if (TimeBeforeTranslation < 0)
                    {
                        UpdateSandGlass(gameTime);
                    }

                    if (Persistence.IsPackageLoaded("principal"))
                    {
                        SceneState = State.LoadScenes;

                        MusicController.SetActiveBank(@"Story1");
                        MusicController.InitializeSfxPriorities();

                        ParallelTasks.Parallel.Start(new Action(LoadScenes));
                    }
                    break;

                case State.LoadScenes:
                    UpdateSandGlass(gameTime);

                    if (ScenesAreLoaded)
                    {
                        foreach (var scene in ScenesLoaded)
                            Visuals.AddScene(scene);

                        SceneState = State.TransiteToMenu;
                    }
                    break;

                case State.TransiteToMenu:
                    TimeBeforeTransition -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (TimeBeforeTransition <= 0)
                    {
                        Visuals.Transite("Chargement", "Menu");
                        SceneState = State.Finished;
                    }

                    break;
            }
        }


        List<Scene> ScenesLoaded = new List<Scene>();
        private void LoadScenes()
        {
            ScenesLoaded.Add(new MainMenuScene());
            ScenesLoaded.Add(new EditorScene());

            LoadWorld(1);
            LoadWorld(2);
            LoadWorld(3);

            ScenesAreLoaded = true;
        }


        private void LoadWorld(int id)
        {
            WorldDescriptor wd;
            WorldScene ws;
            WorldAnnunciationScene was;

            wd = Main.LevelsFactory.WorldsDescriptors[id];
            ws = new WorldScene(wd);
            was = new WorldAnnunciationScene(wd);

            ScenesLoaded.Add(new StoryScene("Cutscene" + wd.Id, was.Name, new IntroCutscene()));
            ScenesLoaded.Add(was);
            ScenesLoaded.Add(ws);

            ws.Initialize();
            was.Initialize();
        }


        private void UpdateSandGlass(GameTime gameTime)
        {
            if (!SandGlass.IsFlipping)
                SandGlass.RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (SandGlass.RemainingTime <= 0)
            {
                SandGlass.RemainingTime = 5000;
                SandGlass.Flip();
            }

            SandGlass.Update();
        }


        protected override void UpdateVisual()
        {
            Add(Background);

            Add(LoadingQuote);

            Logo.Draw();
            SandGlass.Draw();
        }


        public override void OnFocus() //Not ran the first time. In fact should never came back here.
        {
            base.OnFocus();

            VisualEffects.Clear();
            LoadingQuote.Alpha = 0;
            ScenesAreLoaded = false;
            SandGlass.FadeOut(0);
            TimeBeforeTransition = 2000;
            TimeBeforeTranslation = 3500;
        }
    }
}
