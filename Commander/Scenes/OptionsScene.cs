namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework.Input;

    class OptionsScene : Scene
    {
        private Main Main;

        private Text TitreMusique;
        private HorizontalSlider Musique;
        private Text TitreEffetsSonores;
        private HorizontalSlider EffetsSonores;
        private Cursor Curseur;

        private Image Titre;
        private Image FondEcran;
        private Image Bulle;
        private Image Lieutenant;

        private static string Credits = "Hello, my name is Jodi Giordano. I glued all this stuff together. Special thanks to my supporting friends and my family, the LATECE laboratory crew, UQAM, Mercury Project and SFXR. If you want more info about me and my games, please visit ephemeregames.com... Now get back to work, commander!";
        private TextTypeWriter TypeWriter;

        private AnimationTransition AnimationTransition;
        private string ChoixTransition;
        private double TempsEntreDeuxChangementMusique;


        public OptionsScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Options";

            Titre = new Image("options", new Vector3(-550, -150, 0));
            Titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            Titre.Origin = Vector2.Zero;

            TitreMusique = new Text
                    (
                        "Music",
                        "Pixelite",
                        Color.White,
                        new Vector3(-420, 130, 0)
                    );
            TitreMusique.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreMusique.SizeX = 2f;

            TitreEffetsSonores = new Text
                    (
                        "Sound Effects",
                        "Pixelite",
                        Color.White,
                        new Vector3(-420, 200, 0)
                    );
            TitreEffetsSonores.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;
            TitreEffetsSonores.SizeX = 2f;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            Musique = new HorizontalSlider(this, Curseur, new Vector3(-120, 140, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);
            EffetsSonores = new HorizontalSlider(this, Curseur, new Vector3(-120, 210, 0), 0, 10, 5, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            Lieutenant = new Image("lieutenant", new Vector3(-75, -220, 0));
            Lieutenant.SizeX = 8;
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            Bulle = new Image("bulleRenversee", new Vector3(80, -150, 0));
            Bulle.SizeX = 8;
            Bulle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;
            Bulle.Origin = Vector2.Zero;

            FondEcran = new Image("fondecran12", Vector3.Zero);
            FondEcran.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Credits,
                Color.Black,
                new Vector3(170, -130, 0),
                "Pixelite",
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
            TypeWriter.Text.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            AnimationTransition = new AnimationTransition(500, Preferences.PrioriteTransitionScene)
            {
                Scene = this
            };

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

            EphemereGames.Core.Audio.Audio.VolumeMusique = Musique.Valeur / 10f;
            EphemereGames.Core.Audio.Audio.VolumeEffetsSonores = EffetsSonores.Valeur / 10f;

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            TypeWriter.Update(gameTime);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
            AnimationTransition.Start();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            AnimationTransition.Stop();

            if (Transition == TransitionType.Out)
                switch (ChoixTransition)
                {
                    case "menu": EphemereGames.Core.Visual.Visuals.Transite("OptionsToMenu"); break;
                    case "chargement": EphemereGames.Core.Visual.Visuals.Transite("OptionsToChargement"); break;
                }

            Transition = TransitionType.None;
        }


        protected override void UpdateVisual()
        {
            Add(TitreMusique);
            Add(TitreEffetsSonores);
            Add(Titre);
            Add(Lieutenant);
            Add(Bulle);
            Add(FondEcran);
            Add(TypeWriter.Text);

            if (Transition != TransitionType.None)
                AnimationTransition.Draw();

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
                EphemereGames.Core.Persistence.Persistence.SaveData("savePlayer");
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

            MainMenuScene menu = (MainMenuScene)EphemereGames.Core.Visual.Visuals.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }
    }
}
