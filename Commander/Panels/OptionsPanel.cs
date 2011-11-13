namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using Microsoft.Xna.Framework;


    class OptionsPanel : VerticalPanel
    {
        private NumericHorizontalSlider Music;
        private NumericHorizontalSlider Sfx;
        private CheckBox Fullscreen;
        private CheckBox ShowHelpBar;

        private Simulator Simulator;


        public OptionsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            SetTitle("Options");

            Simulator = simulator;

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


        public override void Close()
        {
            base.Close();

            SaveOnDisk();
        }


        protected override bool Click(Commander.Player player)
        {
            bool click = base.Click(player);

            if (click)
            {
                Main.Options.MusicVolume = Music.Value;
                Main.Options.SfxVolume = Sfx.Value;

                Main.Options.FullScreen = Fullscreen.Value;
                Main.Options.ShowHelpBar = ShowHelpBar.Value;
            }

            return click;
        }


        private void SaveOnDisk()
        {
            Main.PlayersController.SaveOptions();
        }
    }
}
