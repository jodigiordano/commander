namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class OptionsPanel : VerticalPanel
    {
        private NumericHorizontalSlider Music;
        private NumericHorizontalSlider Sfx;
        private CheckBox Fullscreen;
        private CheckBox ShowHelpBar;


        public OptionsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Options");

            Music = new NumericHorizontalSlider("Music", 0, 10, 5, 1, 125, 100);
            Sfx = new NumericHorizontalSlider("Sfx", 0, 10, 5, 1, 125, 100);
            Fullscreen = new CheckBox("Fullscreen") { SpaceForLabel = 292 };
            ShowHelpBar = new CheckBox("Show help bar") { SpaceForLabel = 292 };

            AddWidget("Music", Music);
            AddWidget("Sfx", Sfx);
            AddWidget("Fullscreen", Fullscreen);
            AddWidget("ShowHelpBar", ShowHelpBar);

            Alpha = 0;
        }


        public override void Initialize()
        {
            Music.Value = Main.Options.MusicVolume;
            Sfx.Value = Main.Options.SfxVolume;
            Fullscreen.Value = Main.Options.FullScreen;
            ShowHelpBar.Value = Main.Options.ShowHelpBar;

            base.Initialize();
        }


        protected override bool Click(Circle circle)
        {
            bool click = base.Click(circle);

            if (click)
            {
                Main.Options.MusicVolume = Music.Value;
                Main.Options.SfxVolume = Sfx.Value;

                Main.Options.FullScreen = Fullscreen.Value;
                Main.Options.ShowHelpBar = ShowHelpBar.Value;
            }

            return click;
        }


        public void SaveOnDisk()
        {
            Main.PlayersController.SaveOptions();
        }
    }
}
