namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    delegate void NoneHandler();


    class Main : Game
    {
        public static SaveGame SaveGame;
        public static GeneratorData GeneratorData;
        public static PlayersController PlayersController;
        public static TrialMode TrialMode;
        public static GameScene GameInProgress;
        public static Main Instance;

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static List<string> AvailableMusics = new List<string>()
        {
            "ingame1", "ingame2", "ingame3", "ingame4", "ingame5", "ingame6", "ingame7"
        };

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
            GeneratorData = new GeneratorData();
            Window.AllowUserResizing = false;
            PlayersController = new PlayersController();

            if (Preferences.Target == Setting.Xbox360)
                Components.Add(new GamerServicesComponent(this));

            Instance = this;
        }


        public Dictionary<PlayerIndex, Player> Players
        {
            get { return PlayersController.Players; }
        }


        protected override void Initialize()
        {
            base.Initialize();

            Persistence.Initialize(
                "Content",
                "packages.xml",
                Services);

            Visuals.Initialize(
                Graphics,
                1280,
                720,
                Content,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" });

            Visuals.TransitionAnimation = new AnimationTransition(500, Preferences.PrioriteTransitionScene);

            Inputs.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            Physics.Initialize();
            Audio.Initialize(0, 0);

            Persistence.AddData( SaveGame );
            Persistence.AddData( GeneratorData );

            Persistence.LoadPackage("chargement");

            PlayersController.Initialize();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
#if WINDOWS
            if (Preferences.Target == Setting.WindowsDemo)
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

                Visuals.UpdateScene("Chargement", new LoadingScene(this));
                Visuals.UpdateScene("Menu", null);
                Visuals.UpdateScene("Partie", null);
                Visuals.UpdateScene("NouvellePartie", null);
                Visuals.UpdateScene("Aide", null);
                Visuals.UpdateScene("Options", null);
                Visuals.UpdateScene("Editeur", null);
                        
                Initializing = false;
            }

            Inputs.Active = this.IsActive;

            Persistence.Update(gameTime);
            Visuals.Update(gameTime);
            Inputs.Update(gameTime);

            if (!Initializing)
                Audio.Update(gameTime);

            if (Persistence.DataLoaded("savePlayer"))
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
