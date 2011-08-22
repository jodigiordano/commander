namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;


    class Main : Game
    {
        public static SharedSaveGame SharedSaveGame;
        public static SaveGame PlayerSaveGame;
        public static TrialMode TrialMode;
        public static GameScene GameInProgress;
        public static Main Instance;
        public static WorldScene SelectedWorld;
        public static LevelsFactory LevelsFactory;
        public static MusicController MusicController;
        public static NewsController NewsController;

        public static bool GamePausedToWorld { get { return GameInProgress != null && GameInProgress.State == GameState.PausedToWorld; } }

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static Random Random = new Random();


        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = (int) Preferences.BackBuffer.X;
            Graphics.PreferredBackBufferHeight = (int) Preferences.BackBuffer.Y;

            TrialMode = new TrialMode(this);
            Graphics.IsFullScreen = Preferences.FullScreen;
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int) Preferences.TargetElapsedTimeMs);
            Content.RootDirectory = "Content";
            SharedSaveGame = new SharedSaveGame();
            Window.AllowUserResizing = false;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
                Components.Add(new GamerServicesComponent(this));

            Instance = this;

            LevelsFactory = new LevelsFactory();

            MusicController = new MusicController();
            NewsController = new NewsController();
        }


        protected override void Initialize()
        {
            base.Initialize();

            Persistence.Initialize("Content", "packages.xml", Services);
            Visuals.Initialize(Graphics, (int) Preferences.BackBuffer.X, (int) Preferences.BackBuffer.Y, Content);

            Inputs.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            //todo: pass the Player Class to Inputs so it can spawn players on the fly
            Inputs.AddPlayer(new Player());
            Inputs.AddPlayer(new Player());
            Inputs.AddPlayer(new Player());
            Inputs.AddPlayer(new Player());

            Physics.Initialize();
            Audio.Initialize(0.5f, 0.5f);

            Persistence.AddSharedData(SharedSaveGame);

            Persistence.LoadPackage(@"loading");

            LevelsFactory.Initialize();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
#if WINDOWS
            if (Preferences.Target == Core.Utilities.Setting.WindowsDemo)
                System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif

            base.OnExiting(sender, args);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Initializing && Persistence.IsPackageLoaded(@"loading"))
            {
                Visuals.TransitionAnimations = new List<ITransitionAnimation>()
                {
                    new AnimationTransitionAsteroids(500, VisualPriorities.Foreground.Transition),
                    new AnimationTransitionAlienBattleship(500, VisualPriorities.Foreground.Transition),
                    new AnimationTransitionAlienMothership(750, VisualPriorities.Foreground.Transition)
                };
                
                MusicController.setActiveBank(@"Story1");
                MusicController.InitializeSfxPriorities();

                Visuals.AddScene(new LoadingScene());
                        
                Initializing = false;
            }

            if (!IsActive)
                return;

            Persistence.Update(gameTime);
            Visuals.Update(gameTime);
            Inputs.Update(gameTime);

            if (!Initializing && Persistence.IsPackageLoaded("principal"))
            {
                Audio.Update(gameTime);
                MusicController.Update();
            }

            if (PlayerSaveGame != null && PlayerSaveGame.IsLoaded)
                TrialMode.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Visuals.Draw();
        }
    }


    static class Program
    {
        static void Main(string[] args)
        {
            Core.Utilities.ErrorHandling.Run<Main>((int) Preferences.BackBuffer.X, (int) Preferences.BackBuffer.Y, Preferences.FullScreen, GetRunningVersion());
        }


        private static Version GetRunningVersion()
        {
            try
            {
#if WINDOWS
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
#else
                return Assembly.GetExecutingAssembly().GetName().Version;
#endif
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
