namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;


    class Main : Game
    {
        private enum BootSequence
        {
            Initial,
            LoadingSharedSaveGame,
            LoadingMinimalPackage,
            Running
        }


        public static Options Options;
        public static GameScene GameInProgress;
        public static Main Instance;
        public static WorldScene SelectedWorld;
        public static LevelsFactory LevelsFactory;
        public static XACTMusicController MusicController;
        public static NewsController NewsController;
        public static SaveGameController SaveGameController;
        public static CheatsController CheatsController;

        public static bool GamePausedToWorld { get { return GameInProgress != null && GameInProgress.State == GameState.PausedToWorld; } }

        private GraphicsDeviceManager Graphics;
        private BootSequence Boot;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static Random Random = new Random();


        public Main()
        {
            Options = new Options();

            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int) Preferences.BackBuffer.X,
                PreferredBackBufferHeight = (int) Preferences.BackBuffer.Y,
            };

            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int) Preferences.TargetElapsedTimeMs);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = false;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
                Components.Add(new GamerServicesComponent(this));

            Instance = this;

            LevelsFactory = new LevelsFactory();
            MusicController = new XACTMusicController();
            NewsController = new NewsController();
            SaveGameController = new SaveGameController();
            CheatsController = new CheatsController();

            Boot = BootSequence.Initial;

            Activated += new EventHandler<EventArgs>(DoWindowsFocusGained);
            Deactivated += new EventHandler<EventArgs>(DoWindowsFocusLost);
        }


        protected override void Initialize()
        {
            base.Initialize();

            Persistence.Initialize("Content", "packages.xml", Services);
            Physics.Initialize();

            SaveGameController.Initialize();
            MusicController.Initialize();
            LevelsFactory.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (Boot)
            {
                case BootSequence.Initial:
                    SaveGameController.LoadSharedSave();
                    Boot = BootSequence.LoadingSharedSaveGame;
                    break;

                case BootSequence.LoadingSharedSaveGame:
                    if (SaveGameController.IsSharedSaveGameLoaded)
                    {
                        Options.ShowHelpBar = SaveGameController.SharedSaveGame.ShowHelpBar;
                        Options.FullScreen = SaveGameController.SharedSaveGame.FullScreen;
                        Options.MusicVolume = SaveGameController.SharedSaveGame.MusicVolume;
                        Options.SfxVolume = SaveGameController.SharedSaveGame.SfxVolume;
                        Options.FullScreenChanged += new BooleanHandler(DoFullScreenChanged);
                        Options.ShowHelpBarChanged += new BooleanHandler(DoShowHelpBarChanged);
                        Options.VolumeMusicChanged += new Integer2Handler(DoVolumeMusicChanged);
                        Options.VolumeSfxChanged += new Integer2Handler(DoVolumeSfxChanged);
                        Options.FullScreenChanged += new BooleanHandler(SaveGameController.DoFullScreenChanged);
                        Options.ShowHelpBarChanged += new BooleanHandler(SaveGameController.DoShowHelpBarChanged);
                        Options.VolumeMusicChanged += new Integer2Handler(SaveGameController.DoVolumeMusicChanged);
                        Options.VolumeSfxChanged += new Integer2Handler(SaveGameController.DoVolumeSfxChanged);

                        XACTAudio.Initialize(Content.RootDirectory + @"\audio\Audio.xgs");
                        DoVolumeMusicChanged(Options.MusicVolume);
                        DoVolumeSfxChanged(Options.SfxVolume);

                        DoFullScreenChanged(Options.FullScreen);

                        Visuals.Initialize(Graphics, (int) Preferences.BackBuffer.X, (int) Preferences.BackBuffer.Y, Content);

                        Inputs.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

                        //todo: pass the Player Class to Inputs so it can spawn players on the fly
                        Inputs.AddPlayer(new Player());
                        Inputs.AddPlayer(new Player());
                        Inputs.AddPlayer(new Player());
                        Inputs.AddPlayer(new Player());
                        Inputs.AddPlayer(new Player());

                        Inputs.Ready();

                        CheatsController.Initialize();

                        Persistence.LoadPackage(@"loading");
                        Boot = BootSequence.LoadingMinimalPackage;
                    }
                    break;

                case BootSequence.LoadingMinimalPackage:
                    if (Persistence.IsPackageLoaded(@"loading"))
                    {
                        Visuals.TransitionAnimations = new List<ITransitionAnimation>()
                        {
                            new AnimationTransitionAsteroids(500, VisualPriorities.Foreground.Transition),
                            new AnimationTransitionAlienBattleship(500, VisualPriorities.Foreground.Transition),
                            new AnimationTransitionAlienMothership(750, VisualPriorities.Foreground.Transition)
                        };

                        Visuals.AddScene(new LoadingScene());

                        Boot = BootSequence.Running;
                    }
                    break;

                case BootSequence.Running:
                    if (!IsActive)
                        return;
                    
                    Visuals.Update(gameTime);
                    Inputs.Update(gameTime);
                    CheatsController.Update();

                    if (Persistence.IsPackageLoaded(@"principal"))
                    {
                        XACTAudio.Update();
                    }

                    break;
            }

            Persistence.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            if (Boot != BootSequence.Running)
            {
                Graphics.GraphicsDevice.Clear(Color.Black);
                return;
            }

            Visuals.Draw();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
#if WINDOWS
            if (Preferences.Target == Core.Utilities.Setting.WindowsDemo)
                System.Diagnostics.Process.Start(Preferences.WebsiteURL);
#endif

            base.OnExiting(sender, args);
        }


        private void DoWindowsFocusGained(object sender, EventArgs args)
        {
            if (Boot != BootSequence.Running)
                return;

            if (!Persistence.IsPackageLoaded(@"principal"))
                return;

            MusicController.ResumeCurrentMusic();
            //XACTAudio.Resume();
        }


        private void DoWindowsFocusLost(object sender, EventArgs args)
        {
            if (Boot != BootSequence.Running)
                return;

            if (!Persistence.IsPackageLoaded(@"principal"))
                return;

            MusicController.PauseCurrentMusic();
            //XACTAudio.Pause();
        }


        private void DoFullScreenChanged(bool fullscreen)
        {
            if (fullscreen != Graphics.IsFullScreen)
                Graphics.ToggleFullScreen();
        }


        private void DoShowHelpBarChanged(bool value)
        {
            
        }


        private void DoVolumeMusicChanged(int value)
        {
            XACTAudio.ChangeCategoryVolume("Music", value);
        }


        private void DoVolumeSfxChanged(int value)
        {
            XACTAudio.ChangeCategoryVolume("Default", value);
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
