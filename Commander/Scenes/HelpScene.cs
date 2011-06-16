namespace EphemereGames.Commander
{
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

        private string ChoixTransition;
        private double TempsEntreDeuxChangementMusique;

        public HelpScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Name = "Aide";

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

            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(DoPlayerDisconnected);
        }

        private void DoPlayerDisconnected()
        {
            Visuals.Transite("AideToChargement");
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        protected override void UpdateVisual()
        {
            Add(FondEcran);
            Add(TitresSlides[SlidesSlider.Valeur].Key);
            Add(TitresSlides[SlidesSlider.Valeur].Value);
            SlidesSlider.Draw();
            Curseur.Draw();
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
                SlidesSlider.doClick();

            if (button == p.MouseConfiguration.Back)
                BeginTransition();
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
                BeginTransition();

            if (key == p.KeyboardConfiguration.ChangeMusic)
                BeginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
                SlidesSlider.doClick();

            if (button == p.GamePadConfiguration.Cancel)
                BeginTransition();

            if (button == p.GamePadConfiguration.ChangeMusic)
                BeginChangeMusic();
        }


        private void BeginTransition()
        {
            Visuals.Transite("AideToMenu");
        }


        private void BeginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            MainMenuScene menu = (MainMenuScene)Visuals.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }
    }
}
