namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Microsoft.Xna.Framework.Input;

    class Options : Scene
    {
        private Main Main;

        private IVisible TitreMusique;
        private HorizontalSlider Musique;
        private IVisible TitreEffetsSonores;
        private HorizontalSlider EffetsSonores;
        private Cursor Curseur;

        private IVisible Titre;
        private IVisible FondEcran;
        private IVisible Bulle;
        private IVisible Lieutenant;

        private static String Credits = "Hello, my name is Jodi Giordano. I glued all this stuff together. Special thanks to my supporting friends, la familia, the LATECE laboratory crew, UQAM, Mercury Project, SFXR and of course, my muse. Yeah, you over there. Stay tuned for updates by visiting ephemeregames.com... Now get back to work, commander!";
        private TextTypeWriter TypeWriter;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private String ChoixTransition;
        private double TempsEntreDeuxChangementMusique;
        private bool joueurPrincipalDeconnecte;

        public Options(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Options";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            Titre = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("options"), new Vector3(-550, -150, 0), this);
            Titre.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitreMusique = new IVisible
                    (
                        "Music",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-420, 130, 0),
                        this
                    );
            TitreMusique.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreMusique.Taille = 2f;

            TitreEffetsSonores = new IVisible
                    (
                        "Sound Effects",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-420, 200, 0),
                        this
                    );
            TitreEffetsSonores.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreEffetsSonores.Taille = 2f;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            Musique = new HorizontalSlider(Main, this, Curseur, new Vector3(-120, 140, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);
            EffetsSonores = new HorizontalSlider(Main, this, Curseur, new Vector3(-120, 210, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            Lieutenant = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("lieutenant"), new Vector3(120, -420, 0), this);
            Lieutenant.Taille = 8;
            Lieutenant.Origine = new Vector2(0, Lieutenant.Texture.Height);
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            
            Bulle = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("bulleRenversee"), new Vector3(80, -150, 0), this);
            Bulle.Taille = 8;
            Bulle.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.02f;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("fondecran12"), Vector3.Zero, this);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Main,
                Credits,
                Color.Black,
                new Vector3(170, -130, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                2.0f,
                new Vector2(370, 330),
                50,
                true,
                1000,
                true,
                new List<string>()
                {
                    "sfxLieutenantParle1",
                    "sfxLieutenantParle2",
                    "sfxLieutenantParle3",
                    "sfxLieutenantParle4"
                },
                this
            );
            TypeWriter.Texte.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;

            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);

            joueurPrincipalDeconnecte = false;
        }

        private void doJoueurPrincipalDeconnecte()
        {
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            ChoixTransition = "chargement";
            effectuerTransition = true;
            joueurPrincipalDeconnecte = true;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                {
                    switch (ChoixTransition)
                    {
                        case "menu": Core.Visuel.Facade.effectuerTransition("OptionsVersMenu"); break;
                        case "chargement": Core.Visuel.Facade.effectuerTransition("OptionsVersChargement"); break;
                    }
                }
            }

            else
            {
                Main.SaveGame.VolumeMusique = Musique.Valeur;
                Main.SaveGame.VolumeEffetsSonores = EffetsSonores.Valeur;

                Core.Audio.Facade.VolumeMusique = Musique.Valeur / 10f;
                Core.Audio.Facade.VolumeEffetsSonores = EffetsSonores.Valeur / 10f;

                TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
                TypeWriter.Update(gameTime);
                //Curseur.Update(gameTime); //todo
                Musique.Update(gameTime);
                EffetsSonores.Update(gameTime);
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable(TitreMusique);
            ajouterScenable(TitreEffetsSonores);
            ajouterScenable(Titre);
            ajouterScenable(Lieutenant);
            ajouterScenable(Bulle);
            ajouterScenable(FondEcran);
            ajouterScenable(TypeWriter.Texte);

            if (effectuerTransition)
                AnimationTransition.Draw(null);

            Curseur.Draw();
            Musique.Draw(null);
            EffetsSonores.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Musique.Valeur = Main.SaveGame.VolumeMusique;
            EffetsSonores.Valeur = Main.SaveGame.VolumeEffetsSonores;

            effectuerTransition = true;
            AnimationTransition.In = true;
            AnimationTransition.Initialize();
        }


        public override void onFocusLost()
        {
            base.onFocusLost();

            if (!joueurPrincipalDeconnecte)
                Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
            {
                Musique.doClick();
                EffetsSonores.doClick();
            }

            if (button == p.MouseConfiguration.Back)
                beginTransition();
        }


        public override void doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            p.Move(ref delta, p.MouseConfiguration.Speed);
            Curseur.Position = p.Position;
        }


        public override void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.MoveCursor)
            {
                p.Move(ref delta, p.GamePadConfiguration.Speed);
                Curseur.Position = p.Position;
            }
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.Back || key == p.KeyboardConfiguration.Cancel)
                beginTransition();

            if (key == p.KeyboardConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
            {
                Musique.doClick();
                EffetsSonores.doClick();
            }

            if (button == p.GamePadConfiguration.Cancel)
                beginTransition();

            if (button == p.GamePadConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        private void beginTransition()
        {
            if (effectuerTransition)
                return;

            effectuerTransition = true;
            ChoixTransition = "menu";
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
        }


        private void beginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Menu menu = (Menu)Core.Visuel.Facade.recupererScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }
    }
}
