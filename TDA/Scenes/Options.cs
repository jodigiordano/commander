namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
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

        private static String Credits = "Hello, my name is Jodi Giordano. I glued all this stuff together. Special thanks to my supporting friends and my family, the LATECE laboratory crew, UQAM, Mercury Project and SFXR. If you want more info about me and my games, please visit ephemeregames.com... Now get back to work, commander!";
        private TextTypeWriter TypeWriter;

        private AnimationTransition AnimationTransition;
        private String ChoixTransition;
        private double TempsEntreDeuxChangementMusique;


        public Options(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Options";

            Titre = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("options"), new Vector3(-550, -150, 0));
            Titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitreMusique = new IVisible
                    (
                        "Music",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-420, 130, 0)
                    );
            TitreMusique.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreMusique.Taille = 2f;

            TitreEffetsSonores = new IVisible
                    (
                        "Sound Effects",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-420, 200, 0)
                    );
            TitreEffetsSonores.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreEffetsSonores.Taille = 2f;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            Musique = new HorizontalSlider(this, Curseur, new Vector3(-120, 140, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);
            EffetsSonores = new HorizontalSlider(this, Curseur, new Vector3(-120, 210, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            Lieutenant = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("lieutenant"), new Vector3(120, -420, 0));
            Lieutenant.Taille = 8;
            Lieutenant.Origine = new Vector2(0, Lieutenant.Texture.Height);
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            
            Bulle = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleRenversee"), new Vector3(80, -150, 0));
            Bulle.Taille = 8;
            Bulle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;

            FondEcran = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("fondecran12"), Vector3.Zero);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Main,
                Credits,
                Color.Black,
                new Vector3(170, -130, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
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
            TypeWriter.Texte.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            ChoixTransition = "chargement";
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            Main.SaveGame.VolumeMusic = Musique.Valeur;
            Main.SaveGame.VolumeSfx = EffetsSonores.Valeur;

            EphemereGames.Core.Audio.Facade.VolumeMusique = Musique.Valeur / 10f;
            EphemereGames.Core.Audio.Facade.VolumeEffetsSonores = EffetsSonores.Valeur / 10f;

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            TypeWriter.Update(gameTime);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            if (Transition == TransitionType.Out)
                switch (ChoixTransition)
                {
                    case "menu": EphemereGames.Core.Visuel.Facade.Transite("OptionsToMenu"); break;
                    case "chargement": EphemereGames.Core.Visuel.Facade.Transite("OptionsToChargement"); break;
                }

            Transition = TransitionType.None;
        }


        protected override void UpdateVisual()
        {
            ajouterScenable(TitreMusique);
            ajouterScenable(TitreEffetsSonores);
            ajouterScenable(Titre);
            ajouterScenable(Lieutenant);
            ajouterScenable(Bulle);
            ajouterScenable(FondEcran);
            ajouterScenable(TypeWriter.Texte);

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);

            Curseur.Draw();
            Musique.Draw();
            EffetsSonores.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Musique.Valeur = Main.SaveGame.VolumeMusic;
            EffetsSonores.Valeur = Main.SaveGame.VolumeSfx;

            Transition = TransitionType.In;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            if (Main.PlayersController.MasterPlayer.Connected)
                EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
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
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = "menu";
        }


        private void beginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Menu menu = (Menu)EphemereGames.Core.Visuel.Facade.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }
    }
}
