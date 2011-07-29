namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class OptionsPanel : VerticalPanel
    {
        private NumericHorizontalSlider Music;
        private NumericHorizontalSlider Sfx;


        public OptionsPanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Options");

            Music = new NumericHorizontalSlider("Music", 0, 10, 5, 1, 100);
            Sfx = new NumericHorizontalSlider("Sfx  ", 0, 10, 5, 1, 100);

            AddWidget("Music", Music);
            AddWidget("Sfx", Sfx);

            CloseButtonHandler = DoClose;

            Alpha = 0;
        }


        public void Initialize()
        {
            Music.Value = Main.SharedSaveGame.VolumeMusic;
            Sfx.Value = Main.SharedSaveGame.VolumeSfx;
        }


        protected override bool Click(Circle circle)
        {
            bool click = base.Click(circle);

            if (click)
            {
                Main.SharedSaveGame.VolumeMusic = Music.Value;
                Main.SharedSaveGame.VolumeSfx = Sfx.Value;
                Main.SharedSaveGame.ApplyChanges();
            }

            return click;
        }


        private void DoClose(PanelWidget widget)
        {
            Main.SharedSaveGame.Save();
            Fade(Alpha, 0, 500);
        }
    }
}
