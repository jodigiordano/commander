namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    delegate void NoneHandler();


    class Main : Game
    {
        public ManagedThread[] Threads;
        public SaveGame SaveGame;
        public GenerateurData GeneratorData;
        public PlayersController PlayersController;
        public TrialMode TrialMode;
        public Partie GameInProgress;

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;

        private static int nextHash = 0;
        public static int NextHashCode { get { return nextHash++; } }


        public static List<String> AvailableMusics = new List<string>()
        {
            "ingame1", "ingame2", "ingame3", "ingame4", "ingame5", "ingame6", "ingame7", "ingame8"
        };

        public static Random Random = new Random();


        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;

            TrialMode = new TrialMode(this);
            Graphics.IsFullScreen = Preferences.FullScreen;
            Content.RootDirectory = "Content";
            SaveGame = new SaveGame();
            GeneratorData = new GenerateurData();
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

            Threads = new ManagedThread[3];
            Threads[0] = new ManagedThread(3, 1000);
            Threads[1] = new ManagedThread(4, 1000);
            Threads[2] = new ManagedThread(5, 1000);

            Core.Persistance.Facade.Initialize(
                "Content",
                "packages.xml",
                Services,
                Threads[0],
                Threads[0]);

            Core.Visuel.Facade.Initialize(
                Graphics,
                1280,
                720,
                Content,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" },
                Threads[1],
                Threads[2]);

            Core.Input.Facade.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            Core.Physique.Facade.Initialize();
            Core.Audio.Facade.Initialize(0, 0);

            Core.Persistance.Facade.AddData( SaveGame );
            Core.Persistance.Facade.AddData( GeneratorData );

            Core.Persistance.Facade.LoadPackage("chargement");

            PlayersController.Initialize();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            for (int i = 0; i < Threads.Length; i++)
                Threads[i].KillImmediately();

#if WINDOWS
            if (Preferences.Target == Setting.WindowsDemo)
                System.Diagnostics.Process.Start("http://commander.ephemeregames.com");
#endif

            base.OnExiting(sender, args);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Initializing && Core.Persistance.Facade.PackageLoaded("chargement"))
            {
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleBase", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissile", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserMultiple", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissileExplosion", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserSimple", 3);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleSlowMotion", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteTouche", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteExplose", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxNouvelleVague", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire1", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire2", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire3", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleVendue", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxMoney1", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxMoney2", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxMoney3", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxLifePack", 2);

                Core.Visuel.Facade.UpdateScene("Chargement", new LoadingScene(this));
                Core.Visuel.Facade.UpdateScene("Menu", null);
                Core.Visuel.Facade.UpdateScene("Partie", null);
                Core.Visuel.Facade.UpdateScene("NouvellePartie", null);
                Core.Visuel.Facade.UpdateScene("Aide", null);
                Core.Visuel.Facade.UpdateScene("Options", null);
                Core.Visuel.Facade.UpdateScene("Editeur", null);
                        
                Initializing = false;
            }

            Core.Input.Facade.Active = this.IsActive;

            Core.Persistance.Facade.Update(gameTime);
            Core.Visuel.Facade.Update(gameTime);
            Core.Input.Facade.Update(gameTime);

            if (!Initializing)
                Core.Audio.Facade.Update(gameTime);

            if (Core.Persistance.Facade.DataLoaded("savePlayer"))
                TrialMode.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Core.Visuel.Facade.Draw();
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
