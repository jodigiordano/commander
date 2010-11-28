namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.GamerServices;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Persistance;
    using Core.Physique;
    

    class Main : Microsoft.Xna.Framework.Game
    {
        //=====================================================================
        // Attributs
        //=====================================================================

        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        Texture2D Calque360;
        bool initialiser = true;

        public ManagedThread[] Threads;
        public Sauvegarde Sauvegarde;
        public ControleurJoueursConnectes ControleurJoueursConnectes;
        public List<Joueur> JoueursConnectes { get { return ControleurJoueursConnectes.JoueursConnectes; } }
        public ModeTrial ModeTrial;

        private String MusiqueActuelle;

        private Vector2[] Tampons = new Vector2[]
        {
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720),
            new Vector2(1280, 720)
        };


        //=====================================================================
        // Attributs statiques
        //=====================================================================

        public static List<String> MusiquesDisponibles = new List<string>()
        {
            "ingame1", "ingame2", "ingame3", "ingame4", "ingame5", "ingame6", "ingame7", "ingame8"
        };

        public static Random Random = new Random();


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;

            ModeTrial = new ModeTrial(this);

#if DEBUG
            Graphics.IsFullScreen = false;
#else
            Graphics.IsFullScreen = true;
#endif

#if XBOX
            Components.Add(new GamerServicesComponent(this));

#endif

            Content.RootDirectory = "Content";

            Sauvegarde = new Sauvegarde();

            Window.AllowUserResizing = false;

            ControleurJoueursConnectes = new ControleurJoueursConnectes(this);
        }


        //=====================================================================
        // Services
        //=====================================================================

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

            Core.Visuel.Facade.Initialize(
                Graphics,
                1280,
                720,
                Content,
                0,
                0,
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" },
                Tampons,
                Threads[0],
                Threads[1]);

            Core.Input.Facade.Initialize(
                new string[] { "Menu", "Partie", "Chargement", "NouvellePartie", "Aide", "Options", "Editeur", "Acheter", "Validation" },
                new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y));
            Core.Physique.Facade.Initialize();
            Core.Audio.Facade.Initialize(0, 0);

            Core.Persistance.Facade.ajouterDonnee(Sauvegarde);

            Core.Persistance.Facade.charger("chargement");
        }


        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            Calque360 = Content.Load<Texture2D>("debug/Calque360");
        }


        protected override void OnExiting(object sender, EventArgs args)
        {
            for (int i = 0; i < Threads.Length; i++)
                Threads[i].KillImmediately();

#if WINDOWS && TRIAL
            System.Diagnostics.Process.Start("http://ephemeregames.com/?page_id=35");
#endif

            base.OnExiting(sender, args);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (initialiser && Core.Persistance.Facade.estCharge("chargement"))
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

#if WINDOWS
                Core.Visuel.Facade.mettreAJourScene("Validation", new Validation(this));
#endif

                Core.Visuel.Facade.mettreAJourScene("Menu", null);
                Core.Visuel.Facade.mettreAJourScene("Partie", null);
                Core.Visuel.Facade.mettreAJourScene("NouvellePartie", null);
                Core.Visuel.Facade.mettreAJourScene("Aide", null);
                Core.Visuel.Facade.mettreAJourScene("Options", null);
                Core.Visuel.Facade.mettreAJourScene("Editeur", null);

                initialiser = false;
            }

            Core.Persistance.Facade.Update(gameTime);
            Core.Visuel.Facade.Update(gameTime);
            Core.Input.Facade.Update(gameTime);

            if (!initialiser)
                Core.Audio.Facade.Update(gameTime);

            if (Core.Persistance.Facade.donneeEstCharge("savePlayer"))
                ModeTrial.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Core.Visuel.Facade.Draw();

#if DEBUG

            if (Core.Input.Facade.estPesee(Preferences.toucheDebug, (JoueursConnectes.Count == 0) ? PlayerIndex.One : JoueursConnectes[0].Manette, ""))
            {
#if XBOX
                float PositionY = 100;
#else
                float PositionY = 0;
#endif

                SpriteBatch.Begin();
                SpriteBatch.Draw(Calque360, Vector2.Zero, Color.White);
                SpriteBatch.DrawString(Content.Load<SpriteFont>("debug/Debug"), "temps jeu ecoule: " + gameTime.ElapsedGameTime.TotalMilliseconds.ToString("F2"), new Vector2(50, PositionY), Color.White);
                SpriteBatch.DrawString(Content.Load<SpriteFont>("debug/Debug"), "temps reel ecoule: " + gameTime.ElapsedRealTime.TotalMilliseconds.ToString("F2"), new Vector2(250, PositionY), Color.White);
                SpriteBatch.DrawString(Content.Load<SpriteFont>("debug/Debug"), "roule lentement?: " + gameTime.IsRunningSlowly, new Vector2(450, PositionY), Color.White);
                SpriteBatch.End();
            }

            //SpriteBatch.Begin();
            //SpriteBatch.DrawString(Content.Load<SpriteFont>("debug/Debug"), "FPS: " + (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString("F2"), new Vector2(650, 0), Color.White);
            //SpriteBatch.End();
#endif

            base.Draw(gameTime);
        }
    }


    //=====================================================================
    // Point d'entree
    //=====================================================================

    static class Program
    {
        static void Main(string[] args)
        {
            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }
}
