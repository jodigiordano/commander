namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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
        private AnimationTransition AnimationTransition;

        private SandGlass SandGlass;
        private State SceneState;

        private PlayerIndex ConnectingPlayer = PlayerIndex.One;
        private bool WaitingForPlayerToConnect = true;

        private Thread ThreadLoadScenes;
        private bool ThreadLoadScenesFinished;


        private static List<String> LoadingQuotes = new List<string>()
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

            Nom = "Chargement";
            Active = true;

            InitLoadingTransation();
            InitPressStart();

            SandGlass = new SandGlass(Main, this, 5000, new Vector3(0, 250, 0), 0.3f)
            {
                RemainingTime = 5000
            };

            Logo = new Image("Logo", new Vector3(0, -100, 0))
            {
                SizeX = 16,
                VisualPriority = 0.3f
            };

            EphemereGames.Core.Persistance.Facade.LoadPackage("principal");

            SceneState = State.LoadingAssets;

            Background = new Image("SplashScreenBg", Vector3.Zero)
            {
                VisualPriority = 1
            };

            ThreadLoadScenes = new Thread(doChargerScenes)
            {
                IsBackground = true
            };
            ThreadLoadScenesFinished = false;

            AnimationTransition = new AnimationTransition(500, Preferences.PrioriteTransitionScene)
            {
                Scene = this
            };

            Show();
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

            if (Transition == TransitionType.In)
                SceneState = State.ConnectPlayer;
            else
                EphemereGames.Core.Visuel.Facade.Transite("ChargementToMenu");

            Transition = TransitionType.None;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            switch (SceneState)
            {
                case State.LoadingAssets:

                    if (!SandGlass.IsFlipping)
                        SandGlass.RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (SandGlass.RemainingTime <= 0)
                    {
                        SandGlass.RemainingTime = 5000;
                        SandGlass.Flip();
                    }

                    SandGlass.Update();
                    LoadingTranslation.Update();

                    if (EphemereGames.Core.Persistance.Facade.PackageLoaded("principal") && LoadingTranslation.Termine)
                    {
                        SceneState = State.ConnectPlayer;
                        Effects.Add(LoadingTranslation.PartieTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        Effects.Add(LoadingTranslation.PartieNonTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        SandGlass.FadeOut(1000);
                        Effects.Add(PressStart.PartieTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));
                        Effects.Add(PressStart.PartieNonTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));

                    }
                    break;


                case State.ConnectPlayer:
                    PressStart.Update();

                    WaitingForPlayerToConnect = !(WaitingForPlayerToConnect && Main.PlayersController.IsConnected(ConnectingPlayer));

                    if (!WaitingForPlayerToConnect)
                    {
                        if ( !EphemereGames.Core.Persistance.Facade.DataLoaded( "savePlayer" ) )
                        {
                            EphemereGames.Core.Persistance.Facade.LoadData( "savePlayer" );
                            EphemereGames.Core.Persistance.Facade.LoadData( "generateurData" );
                        }

                        SceneState = State.LoadSaveGame;
                    }
                    break;


                case State.LoadSaveGame:
                    if (EphemereGames.Core.Persistance.Facade.DataLoaded("savePlayer"))
                    {
                        if (!ThreadLoadScenesFinished)
                            ThreadLoadScenes.Start();

                        Effects.Add(PressStart.PartieTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        Effects.Add(PressStart.PartieNonTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));

                        InitLoadingTransation();
                        LoadingTranslation.Show();

                        Effects.Add(LoadingTranslation.PartieTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));
                        Effects.Add(LoadingTranslation.PartieNonTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));

                        SandGlass.FadeIn(1500);

                        SceneState = State.LoadScenes;
                    }
                    break;


                case State.LoadScenes:
                    if (!SandGlass.IsFlipping)
                        SandGlass.RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (SandGlass.RemainingTime <= 0)
                    {
                        SandGlass.RemainingTime = 5000;
                        SandGlass.Flip();
                    }

                    LoadingTranslation.Update();
                    SandGlass.Update();

                    if (ThreadLoadScenesFinished)
                    {
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Menu", SceneMenu);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("NouvellePartie", SceneNouvellePartie);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Aide", SceneAide);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Options", SceneOptions);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Editeur", SceneEditeur);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Acheter", SceneAcheter);

                        SceneState = State.Finished;
                        Transition = TransitionType.Out;
                    }
                    break;
            }
        }


        private Scene SceneMenu, SceneNouvellePartie, SceneAide, SceneOptions, SceneEditeur, SceneAcheter;
        public void doChargerScenes()
        {
            SceneMenu = new Menu(Main);
            SceneNouvellePartie = new NouvellePartie(Main);
            SceneAide = new Aide(Main);
            SceneOptions = new Options(Main);
            SceneEditeur = new Editeur(Main);
            SceneAcheter = new Acheter(Main);

            ThreadLoadScenesFinished = true;
        }


        public override void Show()
        {
            LoadingTranslation.Show();
            SandGlass.Show();
            PressStart.Show();

            base.Add(Background);
            base.Add(Logo);
        }


        public override void Hide()
        {
            LoadingTranslation.Hide();
            SandGlass.Hide();
            PressStart.Hide();

            base.Remove(Background);
            base.Remove(Logo);
        }


        protected override void UpdateVisual()
        {
            if (Transition != TransitionType.None)
                AnimationTransition.Draw();

            LoadingTranslation.Draw();
            SandGlass.Draw();
            PressStart.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Main.PlayersController.Initialize();
            Effects.Stop();
            Effects.Clear();
            ConnectingPlayer = PlayerIndex.One;
            WaitingForPlayerToConnect = true;
            InitPressStart();
            LoadingTranslation.PartieTraduite.Couleur.A = 0;
            LoadingTranslation.PartieNonTraduite.Couleur.A = 0;

            Transition = TransitionType.In;
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


        private void InitLoadingTransation()
        {
            if (LoadingTranslation != null)
                LoadingTranslation.Hide();

            LoadingTranslation = new Translator
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 255),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 255),
                LoadingQuotes[Main.Random.Next(0, LoadingQuotes.Count)],
                3,
                true,
                3000,
                250,
                0.3f
            );
            LoadingTranslation.Centre = true;
        }


        private void InitPressStart()
        {
            if (PressStart != null)
                PressStart.Hide();

            PressStart = new Translator
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 0),
                (Preferences.Target == Setting.Xbox360) ? "Press a button, Commander" : "Click a button, Commander",
                3,
                true,
                3000,
                250,
                0.3f
            );
            PressStart.Centre = true;
        }
    }
}
