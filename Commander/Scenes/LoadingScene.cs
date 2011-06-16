namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.Threading;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class LoadingScene : Scene
    {
        private enum State
        {
            Finished,
            LoadingAssets,
            ConnectPlayer,
            LoadSaveGame,
            LoadScenes
        }

        public Main Main;

        private Image Logo;
        private Image Background;
        private Translator LoadingTranslation;
        private Translator PressStart;

        private SandGlass SandGlass;
        private State SceneState;

        private PlayerIndex ConnectingPlayer = PlayerIndex.One;
        private bool WaitingForPlayerToConnect = true;
        private Thread ThreadLoadScenes;
        private bool ScenesAreLoaded;

        private Particle Stars;
        private double StarsEmitter;
        private List<ShootingStar> ShootingStars;


        private static List<string> LoadingQuotes = new List<string>()
        {
            "Building a better world. Please wait.",
            "Counting pixels by hand. Please wait.",
            "Downloading the Internet. Please wait.",
            "Waiting for christmas. Please wait.",
            "Looking for Ben Laden. Please wait.",
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


        public LoadingScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Name = "Chargement";
            EnableVisuals = true;
            EnableInputs = true;
            EnableUpdate = true;

            InitLoadingTranslation();
            InitPressStart();

            SandGlass = new SandGlass(this, 5000, new Vector3(0, 250, 0), 0.3f)
            {
                RemainingTime = 5000
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
            if (StarsEmitter >= 100)
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
                        SceneState = State.ConnectPlayer;
                        VisualEffects.Add(LoadingTranslation.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
                        VisualEffects.Add(LoadingTranslation.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
                        SandGlass.FadeOut(1000);
                        VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
                        VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));

                    }
                    break;


                case State.ConnectPlayer:
                    PressStart.Update();

                    WaitingForPlayerToConnect = !(WaitingForPlayerToConnect && Main.PlayersController.IsConnected(ConnectingPlayer));

                    if (!WaitingForPlayerToConnect)
                    {
                        if ( !Persistence.DataLoaded( "savePlayer" ) )
                        {
                            Persistence.LoadData( "savePlayer" );
                            Persistence.LoadData( "generateurData" );
                        }

                        SceneState = State.LoadSaveGame;
                    }
                    break;


                case State.LoadSaveGame:
                    if (Persistence.DataLoaded("savePlayer"))
                    {
                        ThreadLoadScenes.Start();

                        VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
                        VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));

                        SandGlass.FadeIn(1500);

                        SceneState = State.LoadScenes;
                    }
                    break;


                case State.LoadScenes:
                    UpdateSandGlass(gameTime);
                    LoadingTranslation.Update();

                    if (ScenesAreLoaded)
                    {
                        foreach (var kvp in ScenesLoaded)
                            Visuals.UpdateScene(kvp.Key, kvp.Value);

                        SceneState = State.Finished;
                        Visuals.Transite("ChargementToMenu");
                    }

                    break;
            }
        }


        Dictionary<string, Scene> ScenesLoaded = new Dictionary<string,Scene>();
        private void LoadScenes()
        {
            ScenesLoaded.Add("Menu", new MainMenuScene(Main));
            ScenesLoaded.Add("NouvellePartie", new WorldsScene(Main));
            ScenesLoaded.Add("Aide", new HelpScene(Main));
            ScenesLoaded.Add("Options", new OptionsScene(Main));
            ScenesLoaded.Add("Editeur", new EditorScene(Main));
            ScenesLoaded.Add("Acheter", new BuyScene());

            ScenesAreLoaded = true;
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
            PressStart.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Main.PlayersController.Initialize();
            VisualEffects.Clear();
            ConnectingPlayer = PlayerIndex.One;
            WaitingForPlayerToConnect = true;
            InitPressStart();
            LoadingTranslation.PartieTraduite.Color.A = 0;
            LoadingTranslation.PartieNonTraduite.Color.A = 0;
            SceneState = State.ConnectPlayer;
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            doConnectPlayer(inputIndex);
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            doConnectPlayer(inputIndex);
        }


        private void doConnectPlayer(PlayerIndex inputIndex)
        {
            if (SceneState != State.ConnectPlayer)
                return;

            ConnectingPlayer = inputIndex;
            Main.PlayersController.Connect(inputIndex);
            WaitingForPlayerToConnect = true;
        }


        private void InitLoadingTranslation()
        {
            LoadingTranslation = new Translator
            (this, new Vector3(0, 150, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), LoadingQuotes[Main.Random.Next(0, LoadingQuotes.Count)], 3, true, 3000, 250, 0.3f);
            LoadingTranslation.Centre = true;
        }


        private void InitPressStart()
        {
            PressStart = new Translator
            (this, new Vector3(0, 150, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), (Preferences.Target == Setting.Xbox360) ? "Press a button, Commander" : "Click a button, Commander", 3, true, 3000, 250, 0.3f);
            PressStart.Centre = true;
        }


        private void UpdateShootingStars()
        {
            if (Main.Random.Next(0, 100) == 0)
            {
                ShootingStar ss = new ShootingStar();
                ss.Scene = this;
                ss.Terrain = new Core.Physics.RectanglePhysique(-800, -450, 1400, 900);
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
