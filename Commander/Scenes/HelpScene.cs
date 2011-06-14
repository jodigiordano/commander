namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class HelpScene : Scene
    {
        private Main Main;
        private Image FondEcran;
        private Image Bulle;
        private Image Lieutenant;
        private TextTypeWriter TypeWriter;
        private List<KeyValuePair<Text,Image>> TitresSlides;
        private HorizontalSlider SlidesSlider;
        private Cursor Curseur;

        private AnimationTransition AnimationTransition;
        private string ChoixTransition;
        private double TempsEntreDeuxChangementMusique;

        public HelpScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Aide";

            Lieutenant = new Image("lieutenant", new Vector3(120, -420, 0));
            Lieutenant.SizeX = 8;
            Lieutenant.Origin = new Vector2(0, Lieutenant.AbsoluteSize.Y);
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Bulle = new Image("bulleRenversee", new Vector3(80, -150, 0));
            Bulle.SizeX = 8;
            Bulle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;
            Bulle.Origin = Vector2.Zero;

            FondEcran = new Image("fondecran7", Vector3.Zero);
            FondEcran.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                "",
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
            TypeWriter.Text.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            TitresSlides = new List<KeyValuePair<Text, Image>>();

            Text titre;
            Image slide;

            titre = new Text("Help:Controls", "Pixelite", Color.White, new Vector3(0, -300, 0));
            titre.SizeX = 4;
            titre.Origin = titre.Center;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new Image(Preferences.Target == Setting.Xbox360 ? "HelpControls" : "HelpControlsWin", new Vector3(0, 50, 0));
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<Text,Image>(titre, slide));

            titre = new Text("Help:Battlefield", "Pixelite", Color.White, new Vector3(0, -300, 0));
            titre.SizeX = 4;
            titre.Origin = titre.Center;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new Image("HelpBattlefield", new Vector3(0, 50, 0));
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<Text, Image>(titre, slide));

            titre = new Text("Help:Turrets/Power-ups", "Pixelite", Color.White, new Vector3(0, -300, 0));
            titre.SizeX = 4;
            titre.Origin = titre.Center;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new Image(Preferences.Target == Setting.Xbox360 ? "HelpMercenaries" : "HelpMercenariesWin", new Vector3(0, 50, 0));
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<Text, Image>(titre, slide));

            titre = new Text("Help:Other", "Pixelite", Color.White, new Vector3(0, -300, 0));
            titre.SizeX = 4;
            titre.Origin = titre.Center;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new Image(Preferences.Target == Setting.Xbox360 ? "HelpTheResistance" : "HelpTheResistanceWin", new Vector3(0, 50, 0));
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<Text, Image>(titre, slide));

            SlidesSlider = new HorizontalSlider(this, Curseur, new Vector3(0, -250, 0), 0, 3, 0, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            AnimationTransition = new AnimationTransition(500, Preferences.PrioriteTransitionScene)
            {
                Scene = this
            };

            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }

        private void doJoueurPrincipalDeconnecte()
        {
            ChoixTransition = "chargement";
            Transition = TransitionType.Out;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (AnimationTransition.Finished(gameTime))
            {
                if (Transition == TransitionType.Out)
                    switch (ChoixTransition)
                    {
                        case "menu": EphemereGames.Core.Visual.Visuals.Transite("AideToMenu"); break;
                        case "chargement": EphemereGames.Core.Visual.Visuals.Transite("AideToChargement"); break;
                    }

                Transition = TransitionType.None;
            }
        }


        protected override void UpdateVisual()
        {
            Add(FondEcran);
            Add(TitresSlides[SlidesSlider.Valeur].Key);
            Add(TitresSlides[SlidesSlider.Valeur].Value);
            SlidesSlider.Draw();
            Curseur.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Transition = TransitionType.In;
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
                SlidesSlider.doClick();

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
                SlidesSlider.doClick();

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
