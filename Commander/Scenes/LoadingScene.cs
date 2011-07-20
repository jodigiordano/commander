namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.Threading;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Commander.Simulation;
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

        private Image Logo;
        private Image Background;
        private Translator LoadingTranslation;

        private SandGlass SandGlass;
        private State SceneState;

        private Thread ThreadLoadScenes;
        private bool ScenesAreLoaded;

        private Particle Stars;
        private double StarsEmitter;
        private List<ShootingStar> ShootingStars;

        private double TimeBeforeTransition = 2000;


        private static List<string> LoadingQuotes = new List<string>()
        {
            "Building a better world. Please wait.",
            "Counting pixels by hand. Please wait.",
            "Downloading the Internet. Please wait.",
            "Waiting for christmas. Please wait.",
            "Applying a Windows patch. Please wait.",
            "Logging in the Matrix. Please wait.",
            "Organizing the resistance. Please wait.",
            "Generating the universe. Please wait.",
            "Understanding my girlfriend. Please wait.",
            "Getting rid of poverty. Please wait.",
            "Searching my soulmate. Please wait.",
            "Compiling the kernel. Please wait.",
            "Learning quantum physic. Please wait.",
            "Mastering chinese langage. Please wait.",
            "Watching a trilogy. Please wait."
        };


        public LoadingScene()
            : base(1280, 720)
        {
            Name = "Chargement";
            EnableVisuals = true;
            EnableInputs = true;
            EnableUpdate = true;

            InitLoadingTranslation();

            SandGlass = new SandGlass(this, 5000, new Vector3(0, 250, 0), 0.3f)
            {
                RemainingTime = 5000,
                Alpha = 0
            };

            Logo = new Image("Logo", new Vector3(0, -100, 0))
            {
                SizeX = 16,
                VisualPriority = 0.3f
            };

            Persistence.LoadPackage("principal");

            SceneState = State.LoadingAssets;

            Background = new Image("fondecran" + Main.Random.Next(1, 7), Vector3.Zero)
            {
                VisualPriority = 1
            };

            this.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 3000));
            this.VisualEffects.Add(Logo, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 3000));
            this.VisualEffects.Add(LoadingTranslation.PartieNonTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 3000));
            this.VisualEffects.Add(LoadingTranslation.PartieTraduite, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 3000));
            SandGlass.FadeIn(3000);

            ThreadLoadScenes = new Thread(LoadScenes);
            ScenesAreLoaded = false;

            Particles.Add(@"etoilesScintillantes");
            Particles.Add(@"etoileFilante");
            Stars = Particles.Get(@"etoilesScintillantes");
            Stars.VisualPriority = Preferences.PrioriteGUIEtoiles;
            StarsEmitter = 0;

            ShootingStars = new List<ShootingStar>();
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            StarsEmitter += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (StarsEmitter >= 300)
            {
                Vector2 v2 = Vector2.Zero;
                Stars.Trigger(ref v2);
                StarsEmitter = 0;
            }

            UpdateShootingStars();

            switch (SceneState)
            {
                case State.LoadingAssets:

                    UpdateSandGlass(gameTime);
                    LoadingTranslation.Update();

                    if (Persistence.IsPackageLoaded("principal") && LoadingTranslation.Termine)
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
                        VisualEffects.Add(LoadingTranslation.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
                        VisualEffects.Add(LoadingTranslation.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));

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
            Add(Logo);

            LoadingTranslation.Draw();
            SandGlass.Draw();
        }


        public override void OnFocus() //Not ran the first time
        {
            base.OnFocus();

            VisualEffects.Clear();
            LoadingTranslation.PartieTraduite.Alpha = 0;
            LoadingTranslation.PartieNonTraduite.Alpha = 0;
            ThreadLoadScenes = new Thread(LoadScenes);
            ScenesAreLoaded = false;
            SandGlass.FadeOut(0);
        }


        private void InitLoadingTranslation()
        {
            LoadingTranslation = new Translator
            (this, new Vector3(0, 150, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), LoadingQuotes[Main.Random.Next(0, LoadingQuotes.Count)], 3, true, 3000, 250, 0.3f);
            LoadingTranslation.Centre = true;
        }


        private void UpdateShootingStars()
        {
            if (Main.Random.Next(0, 100) == 0)
            {
                ShootingStar ss = new ShootingStar();
                ss.Scene = this;
                ss.Terrain = new Core.Physics.PhysicalRectangle(-800, -450, 1400, 900);
                ss.LoadContent();
                ss.Initialize();

                ShootingStars.Add(ss);
            }

            for (int i = ShootingStars.Count - 1; i > -1; i--)
            {
                ShootingStar ss = ShootingStars[i];

                ss.Update();

                if (!ss.Alive)
                    ShootingStars.RemoveAt(i);
            }
        }
    }
}
