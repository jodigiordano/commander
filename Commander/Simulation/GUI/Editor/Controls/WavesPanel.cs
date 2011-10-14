namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class WavesPanel : SlideshowPanel
    {
        private Simulator Simulator;


        public WavesPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Waves");
            Slider.SetLabel("Wave");
        }


        public void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            for (int i = 0; i < 20; i++)
                AddWidget("wave" + i, new WaveSubPanel(Simulator, new Vector2(Dimension.X, Dimension.Y), Preferences.PrioriteGUIPanneauGeneral, Color.White));

            for (int i = 0; i < Simulator.LevelDescriptor.Waves.Count; i++)
            {
                var widget = (WaveSubPanel) GetWidgetByName("wave" + i);
                widget.Sync(Simulator.LevelDescriptor.Waves[i]);
            }
        }
    }
}
