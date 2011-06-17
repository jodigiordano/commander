namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class HelpScene : Scene
    {
        private Image FondEcran;
        private Image Bulle;
        private Image Lieutenant;
        private TextTypeWriter TypeWriter;
        private List<KeyValuePair<Text,Image>> TitresSlides;
        private HorizontalSlider SlidesSlider;
        private Cursor Curseur;
        private string ChoixTransition;


        public HelpScene()
            : base(Vector2.Zero, 1280, 720)
        {
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

            Curseur = new Cursor(this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

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

            SlidesSlider = new HorizontalSlider(this, new Vector3(0, -250, 0), 0, 3, 0, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);
        }


        protected override void UpdateLogic(GameTime gameTime) { }


        protected override void UpdateVisual()
        {
            Add(FondEcran);
            Add(TitresSlides[SlidesSlider.Valeur].Key);
            Add(TitresSlides[SlidesSlider.Valeur].Value);
            SlidesSlider.Draw();
            Curseur.Draw();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (!p.Master)
                return;

            if (button == MouseConfiguration.Select)
                SlidesSlider.DoClick(((Player) p).Circle);

            if (button == MouseConfiguration.Back)
                BeginTransition();
        }


        public override void DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            if (!p.Master)
                return;

            Player player = (Player) p;

            player.Move(ref delta, MouseConfiguration.Speed);
            Curseur.Position = player.Position;
        }


        public override void DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            if (!p.Master)
                return;

            Player player = (Player) p;

            if (button == GamePadConfiguration.MoveCursor)
            {
                player.Move(ref delta, GamePadConfiguration.Speed);
                Curseur.Position = player.Position;
            }
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (!p.Master)
                return;

            if (key == KeyboardConfiguration.Back || key == KeyboardConfiguration.Cancel)
                BeginTransition();

            if (key == KeyboardConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (!p.Master)
                return;

            if (button == GamePadConfiguration.Select)
                SlidesSlider.DoClick(((Player) p).Circle);

            if (button == GamePadConfiguration.Cancel)
                BeginTransition();

            if (button == GamePadConfiguration.ChangeMusic)
                Main.ChangeMusic();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (player.Master)
                TransiteTo("Chargement");
        }


        private void BeginTransition()
        {
            TransiteTo("Menu");
        }
    }
}
