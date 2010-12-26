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
        public PlayersController PlayersController;
        public TrialMode TrialMode;

        private GraphicsDeviceManager Graphics;
        private bool Initializing = true;


        public static List<String> MusiquesDisponibles = new List<string>()
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
            Threads[0] = new ManagedThread(3);
            Threads[1] = new ManagedThread(4);
            Threads[2] = new ManagedThread(5);

            EphemereGames.Core.Persistance.Facade.Initialize(
                "Content",
                "packages.xml",
                Services,
                Threads[0],
                Threads[0]);

            EphemereGames.Core.Visuel.Facade.Initialize(
                Graphics,
                1280,
                720,
                Content,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" },
                Threads[0],
                Threads[1]);

            EphemereGames.Core.Input.Facade.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            EphemereGames.Core.Physique.Facade.Initialize();
            EphemereGames.Core.Audio.Facade.Initialize(0, 0);

            EphemereGames.Core.Persistance.Facade.AddData(SaveGame);

            EphemereGames.Core.Persistance.Facade.LoadPackage("chargement");

            PlayersController.Initialize();
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            for (int i = 0; i < Threads.Length; i++)
                Threads[i].KillImmediately();

            if (Preferences.Target == Setting.WindowsDemo)
                System.Diagnostics.Process.Start("http://ephemeregames.com/?page_id=35");

            base.OnExiting(sender, args);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Initializing && EphemereGames.Core.Persistance.Facade.PackageLoaded("chargement"))
            {
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleBase", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissile", 1);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserMultiple", 1);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissileExplosion", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserSimple", 3);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleSlowMotion", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteTouche", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteExplose", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxNouvelleVague", 2);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire1", 1);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire2", 1);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire3", 1);
                EphemereGames.Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleVendue", 1);

                EphemereGames.Core.Visuel.Facade.UpdateScene("Chargement", new Chargement(this));
                EphemereGames.Core.Visuel.Facade.UpdateScene("Validation", new Validation(this));
                EphemereGames.Core.Visuel.Facade.UpdateScene("Menu", null);
                EphemereGames.Core.Visuel.Facade.UpdateScene("Partie", null);
                EphemereGames.Core.Visuel.Facade.UpdateScene("NouvellePartie", null);
                EphemereGames.Core.Visuel.Facade.UpdateScene("Aide", null);
                EphemereGames.Core.Visuel.Facade.UpdateScene("Options", null);
                EphemereGames.Core.Visuel.Facade.UpdateScene("Editeur", null);

                Initializing = false;
            }

            EphemereGames.Core.Input.Facade.Active = this.IsActive;

            EphemereGames.Core.Persistance.Facade.Update(gameTime);
            EphemereGames.Core.Visuel.Facade.Update(gameTime);
            EphemereGames.Core.Input.Facade.Update(gameTime);

            if (!Initializing)
                EphemereGames.Core.Audio.Facade.Update(gameTime);

            if (EphemereGames.Core.Persistance.Facade.DataLoaded("savePlayer"))
                TrialMode.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            EphemereGames.Core.Visuel.Facade.Draw();
        }
    }


    static class Program
    {
        static void Main(string[] args)
        {
            EphemereGames.Core.Utilities.ErrorHandling.Run<Main>(1280, 720, Preferences.FullScreen, GetRunningVersion());
        }


        private static Version GetRunningVersion()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
