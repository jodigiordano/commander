namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    delegate void NoneHandler();


    class Main : Game
    {
        public SaveGame SaveGame;
        public GeneratorData GeneratorData;
        public PlayersController PlayersController;
        public TrialMode TrialMode;
        public GameScene GameInProgress;

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
        }


        public Dictionary<PlayerIndex, Player> Players
        {
            get { return PlayersController.Players; }
        }


        protected override void Initialize()
        {
            base.Initialize();

            Core.Persistence.Persistence.Initialize(
                "Content",
                "packages.xml",
                Services);

            Core.Visual.Visuals.Initialize(
                Graphics,
                1280,
                720,
                Content,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" });

            Core.Input.Input.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            Core.Physics.Physics.Initialize();
            Core.Audio.Audio.Initialize(0, 0);

            Core.Persistence.Persistence.AddData( SaveGame );
            Core.Persistence.Persistence.AddData( GeneratorData );

            Core.Persistence.Persistence.LoadPackage("chargement");

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

            if (Initializing && Core.Persistence.Persistence.IsPackageLoaded("chargement"))
            {
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleBase", 2);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleMissile", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleLaserMultiple", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleMissileExplosion", 2);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleLaserSimple", 3);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleSlowMotion", 2);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteTouche", 2);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteExplose", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxNouvelleVague", 2);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire1", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire2", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire3", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxTourelleVendue", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxMoney1", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxMoney2", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxMoney3", 1);
                Core.Audio.Audio.setMaxInstancesActivesEffetSonore("sfxLifePack", 2);

                Core.Visual.Visuals.UpdateScene("Chargement", new LoadingScene(this));
                Core.Visual.Visuals.UpdateScene("Menu", null);
                Core.Visual.Visuals.UpdateScene("Partie", null);
                Core.Visual.Visuals.UpdateScene("NouvellePartie", null);
                Core.Visual.Visuals.UpdateScene("Aide", null);
                Core.Visual.Visuals.UpdateScene("Options", null);
                Core.Visual.Visuals.UpdateScene("Editeur", null);
                        
                Initializing = false;
            }

            Core.Input.Input.Active = this.IsActive;

            Core.Persistence.Persistence.Update(gameTime);
            Core.Visual.Visuals.Update(gameTime);
            Core.Input.Input.Update(gameTime);

            if (!Initializing)
                Core.Audio.Audio.Update(gameTime);

            if (Core.Persistence.Persistence.DataLoaded("savePlayer"))
                TrialMode.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Scene s = Core.Visual.Visuals.GetScene("NouvellePartie");

            if (s != null)
            {
                s.Add(new Text("framerate: " + 1000 / gameTime.ElapsedGameTime.TotalMilliseconds, "Pixelite", Color.White, new Vector3(-550, -250, 0)) { SizeX = 2 });
                s.Add(new Text("slow?: " + gameTime.IsRunningSlowly, "Pixelite", Color.White, new Vector3(-550, -225, 0)) { SizeX = 2 });
            }

            Core.Visual.Visuals.Draw();
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
