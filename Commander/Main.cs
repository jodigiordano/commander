namespace EphemereGames.Commander
{
    using System;
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
        public static string SelectedWorld;
        public static LevelsFactory LevelsFactory;
        public static MusicController MusicController;

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static Random Random = new Random();


        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;

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
        }


        protected override void Initialize()
        {
            base.Initialize();

            Persistence.Initialize("Content", "packages.xml", Services);
            Visuals.Initialize(Graphics, 1280, 720, Content);

            Visuals.TransitionAnimation = new AnimationTransition(500, Preferences.PrioriteTransitionScene);

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
                System.Diagnostics.Process.Start("http://commander.ephemeregames.com");
#endif

            base.OnExiting(sender, args);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Initializing && Persistence.IsPackageLoaded(@"loading"))
            {
                MusicController.setActiveBank(@"Story1");
                MusicController.InitializeSfxPriorities();

                Visuals.AddScene(new LoadingScene());
                        
                Initializing = false;
            }

            Inputs.Active = this.IsActive;

            Persistence.Update(gameTime);
            Visuals.Update(gameTime);
            Inputs.Update(gameTime);

            if (!Initializing && Persistence.IsPackageLoaded("principal"))
            {
                Audio.Update(gameTime);
                MusicController.Update();
            }

            if (Persistence.IsDataLoaded("Save"))
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
            Core.Utilities.ErrorHandling.Run<Main>(1280, 720, Preferences.FullScreen, GetRunningVersion());
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
