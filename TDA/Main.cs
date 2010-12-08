namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Persistance;
    using Core.Utilities;
    using Microsoft.Xna.Framework;
    using System.Reflection;
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

            Core.Persistance.Facade.Initialize(
                "Content",
                "packages.xml",
                Services,
                Threads[0],
                Threads[0],
                new StorageMessages());

            Core.Input.Facade.Initialize(new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));

            Core.Visuel.Facade.Initialize(
                Graphics,
                1280,
                720,
                Content,
                0,
                0,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" },
                Threads[0],
                Threads[1]);

            Core.Physique.Facade.Initialize();
            Core.Audio.Facade.Initialize(0, 0);

            Core.Persistance.Facade.ajouterDonnee(SaveGame);

            Core.Persistance.Facade.charger("chargement");

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

            if (Initializing && Core.Persistance.Facade.estCharge("chargement"))
            {
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleBase", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissile", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserMultiple", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleMissileExplosion", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleLaserSimple", 3);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleSlowMotion", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteTouche", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxCorpsCelesteExplose", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxNouvelleVague", 2);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire1", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire2", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxPowerUpResistanceTire3", 1);
                Core.Audio.Facade.setMaxInstancesActivesEffetSonore("sfxTourelleVendue", 1);

                Core.Visuel.Facade.mettreAJourScene("Chargement", new Chargement(this));
                Core.Visuel.Facade.mettreAJourScene("Validation", new Validation(this));
                Core.Visuel.Facade.mettreAJourScene("Menu", null);
                Core.Visuel.Facade.mettreAJourScene("Partie", null);
                Core.Visuel.Facade.mettreAJourScene("NouvellePartie", null);
                Core.Visuel.Facade.mettreAJourScene("Aide", null);
                Core.Visuel.Facade.mettreAJourScene("Options", null);
                Core.Visuel.Facade.mettreAJourScene("Editeur", null);

                Initializing = false;
            }

            Core.Persistance.Facade.Update(gameTime);
            Core.Visuel.Facade.Update(gameTime);
            Core.Input.Facade.Update(gameTime);

            if (!Initializing)
                Core.Audio.Facade.Update(gameTime);

            if (Core.Persistance.Facade.donneeEstCharge("savePlayer"))
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
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
