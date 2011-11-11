namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class WavesPanel : SlideshowPanel
    {
        private Simulator Simulator;


        public WavesPanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(1200, 600), VisualPriorities.Default.EditorPanel, Color.White)
        {
            Simulator = simulator;

            SetTitle("Waves");
            Slider.SpaceForLabel = 200;
            Slider.SetLabel("Wave #");
        }


        public override void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            for (int i = 0; i < 20; i++)
                AddWidget("wave" + i, new WaveSubPanel(Simulator, new Vector2(Dimension.X, Dimension.Y), VisualPriority + 0.00001, Color.White, i));

            for (int i = 0; i < Simulator.Data.Level.Descriptor.Waves.Count; i++)
            {
                var widget = (WaveSubPanel) GetWidgetByName("wave" + i);
                widget.Sync(Simulator.Data.Level.Descriptor.Waves[i]);
            }

            Alpha = 0;
        }


        public void SyncEnemiesCurrentWave(List<EnemyType> enemies)
        {
            ((WaveSubPanel) GetWidgetByName("wave" + Slider.Value)).Enemies = enemies;
        }
    }
}
