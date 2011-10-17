namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class WavesPanel : SlideshowPanel
    {
        private Simulator Simulator;


        public WavesPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
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

            for (int i = 0; i < Simulator.LevelDescriptor.Waves.Count; i++)
            {
                var widget = (WaveSubPanel) GetWidgetByName("wave" + i);
                widget.Sync(Simulator.LevelDescriptor.Waves[i]);
            }

            Alpha = 0;
        }


        public void SyncEnemiesCurrentWave(List<EnemyType> enemies)
        {
            ((WaveSubPanel) GetWidgetByName("wave" + Slider.Value)).Enemies = enemies;
        }
    }
}
