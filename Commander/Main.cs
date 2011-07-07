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
        public static SaveGame SaveGame;
        public static TrialMode TrialMode;
        public static GameScene GameInProgress;
        public static Main Instance;
        public static string SelectedWorld;
        public static LevelsFactory LevelsFactory;

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static List<string> AvailableMusics = new List<string>()
        {
            "ingame1",
            "ingame2",
            "ingame3",
            "ingame4",
            "ingame5",
            "ingame6",
            "ingame7"
        };
        public static string SelectedMusic;
        private static double TimeBetweenTwoMusicChange = 0;

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
            SaveGame = new SaveGame();
            Window.AllowUserResizing = false;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
                Components.Add(new GamerServicesComponent(this));

            Instance = this;

            LevelsFactory = new LevelsFactory();
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
            Audio.Initialize(0, 0);

            Persistence.AddData(SaveGame);

            Persistence.LoadPackage("chargement");

            SelectedMusic = AvailableMusics[Random.Next(0, AvailableMusics.Count)];
            AvailableMusics.Remove(SelectedMusic);

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

            if (Initializing && Persistence.IsPackageLoaded("chargement"))
            {
                Audio.SetMaxInstancesSfx("sfxTourelleBase", 2);
                Audio.SetMaxInstancesSfx("sfxTourelleMissile", 1);
                Audio.SetMaxInstancesSfx("sfxTourelleLaserMultiple", 1);
                Audio.SetMaxInstancesSfx("sfxTourelleMissileExplosion", 2);
                Audio.SetMaxInstancesSfx("sfxTourelleLaserSimple", 3);
                Audio.SetMaxInstancesSfx("sfxTourelleSlowMotion", 2);
                Audio.SetMaxInstancesSfx("sfxCorpsCelesteTouche", 2);
                Audio.SetMaxInstancesSfx("sfxCorpsCelesteExplose", 1);
                Audio.SetMaxInstancesSfx("sfxNouvelleVague", 2);
                Audio.SetMaxInstancesSfx("sfxPowerUpResistanceTire1", 1);
                Audio.SetMaxInstancesSfx("sfxPowerUpResistanceTire2", 1);
                Audio.SetMaxInstancesSfx("sfxPowerUpResistanceTire3", 1);
                Audio.SetMaxInstancesSfx("sfxTourelleVendue", 1);
                Audio.SetMaxInstancesSfx("sfxMoney1", 1);
                Audio.SetMaxInstancesSfx("sfxMoney2", 1);
                Audio.SetMaxInstancesSfx("sfxMoney3", 1);
                Audio.SetMaxInstancesSfx("sfxLifePack", 2);
                Audio.SetMaxInstancesSfx("sfxMineGround", 1);

                Visuals.AddScene(new LoadingScene());
                        
                Initializing = false;
            }

            Inputs.Active = this.IsActive;

            Persistence.Update(gameTime);
            Visuals.Update(gameTime);
            Inputs.Update(gameTime);

            if (!Initializing && Persistence.IsPackageLoaded("principal"))
                Audio.Update(gameTime);

            if (Persistence.DataLoaded("savePlayer"))
                TrialMode.Update(gameTime);

            TimeBetweenTwoMusicChange -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        protected override void Draw(GameTime gameTime)
        {
            Visuals.Draw();
        }


        public static void ChangeMusic()
        {
            if (TimeBetweenTwoMusicChange > 0)
                return;

            Audio.StopMusic(Main.SelectedMusic, true, Preferences.TimeBetweenTwoMusics - 50);
            string ancienneMusique = Main.SelectedMusic;
            Main.SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(Main.SelectedMusic);
            Main.AvailableMusics.Add(ancienneMusique);
            Audio.PlayMusic(Main.SelectedMusic, true, 1000, true);
            TimeBetweenTwoMusicChange = Preferences.TimeBetweenTwoMusics;
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
