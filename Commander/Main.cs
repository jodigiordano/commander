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
        public static Main Instance;
        public static GameScene CurrentGame;
        public static WorldScene CurrentWorld;
        public static WorldAnnunciationScene CurrentWorldAnnounciation;
        public static LevelsFactory LevelsFactory;
        public static XACTMusicController MusicController;
        public static NewsController NewsController;
        public static SaveGameController SaveGameController;
        public static CheatsController CheatsController;
        public static InputsFactory InputsFactory;

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
                PreferredBackBufferHeight = (int) Preferences.BackBuffer.Y
            };

            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int) Preferences.TargetElapsedTimeMs);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = false;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
                Components.Add(new GamerServicesComponent(this));

            Instance = this;

            LevelsFactory = new LevelsFactory();
            InputsFactory = new InputsFactory();
            MusicController = new XACTMusicController();
            NewsController = new NewsController();
            SaveGameController = new SaveGameController();
            CheatsController = new CheatsController();

            Boot = BootSequence.Initial;

            Activated += new EventHandler<EventArgs>(DoWindowsFocusGained);
            Deactivated += new EventHandler<EventArgs>(DoWindowsFocusLost);
        }


        public static void SetCurrentWorld(int id)
        {
            SetCurrentWorld(LevelsFactory.Worlds[id]);
        }


        public static void SetCurrentWorld(World world)
        {
            if (CurrentWorld == null)
                CurrentWorld = (WorldScene) Visuals.GetScene("World");

            if (CurrentWorldAnnounciation == null)
                CurrentWorldAnnounciation = (WorldAnnunciationScene) Visuals.GetScene("WorldAnnunciation");

            CurrentWorld.World = world;
            CurrentWorld.EditorMode = world.EditorMode;
            CurrentWorld.Initialize();

            CurrentWorldAnnounciation.World = world;
            CurrentWorldAnnounciation.Initialize();
        }


        protected override void Initialize()
        {
            base.Initialize();

            Persistence.Initialize("Content", "packages.xml", Services);
            Physics.Initialize();

            SaveGameController.Initialize();
            MusicController.Initialize();
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

                        Persistence.LoadPackage(@"loading");
                        Boot = BootSequence.LoadingMinimalPackage;
                    }
                    break;

                case BootSequence.LoadingMinimalPackage:
                    if (Persistence.IsPackageLoaded(@"loading"))
                    {
                        InputsFactory.Initialize();
                        Inputs.Initialize(new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2));

                        //todo: pass the Player Class to Inputs so it can spawn players on the fly
                        //for mouse: a fifth player is needed
                        if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                        {
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                        }

                        else
                        {
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                            Inputs.AddPlayer(new Player());
                        }
                        Inputs.Ready();

                        CheatsController.Initialize();

                        Visuals.TransitionAnimations = new List<ITransitionAnimation>()
                        {
                            new AnimationTransitionAsteroids(500, VisualPriorities.Foreground.Transition),
                            new AnimationTransitionAlienBattleship(500, VisualPriorities.Foreground.Transition),
                            new AnimationTransitionAlienMothership(750, VisualPriorities.Foreground.Transition)
                        };

                        LevelsFactory.Initialize();

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

                    MusicController.Update();
                    XACTAudio.Update();

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
            base.OnExiting(sender, args);
        }


        private void DoWindowsFocusGained(object sender, EventArgs args)
        {
            if (Boot != BootSequence.Running)
                return;

            if (!Persistence.IsPackageLoaded(@"principal"))
                return;

            MusicController.ResumeCurrentMusic();
        }


        private void DoWindowsFocusLost(object sender, EventArgs args)
        {
            if (Boot != BootSequence.Running)
                return;

            if (!Persistence.IsPackageLoaded(@"principal"))
                return;

            MusicController.PauseCurrentMusicNow();
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
            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                Core.Utilities.SilentErrorHandling.Run<Main>();
            else
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
