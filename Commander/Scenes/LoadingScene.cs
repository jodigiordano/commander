namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.Threading;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LoadingScene : Scene
    {
        private enum State
        {
            TransiteToMenu,
            Finished,
            LoadingAssets,
            LoadSharedSaveGame,
            LoadScenes
        }

        private Logo Logo;
        private Image Background;
        private Translator LoadingTranslation;

        private SandGlass SandGlass;
        private State SceneState;

        private Thread ThreadLoadScenes;
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
            : base(1280, 720)
        {
            Name = "Chargement";
            EnableVisuals = true;
            EnableInputs = true;
            EnableUpdate = true;

            SandGlass = new SandGlass(this, 5000, new Vector3(0, 250, 0), 0.3f)
            {
                Color = Colors.Default.NeutralDark,
                RemainingTime = 5000,
                Alpha = 0
            };


            Logo = new Logo(this, Vector3.Zero, 0.3);

            Persistence.LoadPackage("principal");

            SceneState = State.LoadingAssets;

            Background = new Image("PixelBlanc", Vector3.Zero)
            {
                VisualPriority = 1,
                Size = new Vector2(1280, 720),
                Color = Color.White
            };

            InitLoadingTranslation();

            VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 500));
            VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
            VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeOutTo0(255, 2500, 500));
            VisualEffects.Add(LoadingTranslation.ToTranslate, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeTranslation, 1000));
            VisualEffects.Add(LoadingTranslation.Translated, Core.Visual.VisualEffects.FadeInFrom0(255, TimeBeforeTranslation, 1000));
            SandGlass.FadeIn(TimeBeforeTranslation, 1000);

            ThreadLoadScenes = new Thread(LoadScenes);
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
                        LoadingTranslation.Update();
                    }

                    if (Persistence.IsPackageLoaded("principal") && LoadingTranslation.Finished)
                    {
                        SceneState = State.LoadSharedSaveGame;

                        Persistence.LoadData("SharedSaveGame");
                    }
                    break;

                case State.LoadSharedSaveGame:

                    if (Main.SharedSaveGame.IsLoaded)
                    {
                        SceneState = State.LoadScenes;

                        ThreadLoadScenes.Start();
                    }

                    break;

                case State.LoadScenes:
                    UpdateSandGlass(gameTime);
                    LoadingTranslation.Update();

                    if (ScenesAreLoaded)
                    {
                        VisualEffects.Add(LoadingTranslation.Translated, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
                        VisualEffects.Add(LoadingTranslation.ToTranslate, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));

                        SandGlass.FadeOut(1000);

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
            ScenesLoaded.Add(new HelpScene());
            ScenesLoaded.Add(new OptionsScene());
            ScenesLoaded.Add(new EditorScene());
            ScenesLoaded.Add(new BuyScene());

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

            Logo.Draw();
            LoadingTranslation.Draw();
            SandGlass.Draw();
        }


        public override void OnFocus() //Not ran the first time
        {
            base.OnFocus();

            VisualEffects.Clear();
            LoadingTranslation.Translated.Alpha = 0;
            LoadingTranslation.ToTranslate.Alpha = 0;
            ThreadLoadScenes = new Thread(LoadScenes);
            ScenesAreLoaded = false;
            SandGlass.FadeOut(0);
            TimeBeforeTransition = 2000;
            TimeBeforeTranslation = 3500;
        }


        private void InitLoadingTranslation()
        {
            LoadingTranslation = new Translator(
                this, new Vector3(0, 150, 0),
                "Alien", Colors.Default.AlienBright,
                "Pixelite", Colors.Default.NeutralDark,
                LoadingQuotes[Main.Random.Next(0, LoadingQuotes.Count)], 3, true, 3000, 250, 0.3f, false)
                {
                    CenterText = true
                };
        }
    }
}
