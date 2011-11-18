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

            Alpha = 0;
        }


        public override void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            for (int i = 0; i < 50; i++)
                AddWidget("wave" + i, new WaveSubPanel(Simulator, new Vector2(Dimension.X, Dimension.Y), VisualPriority + 0.00001, Color.White, i));
        }


        public override void Open()
        {
            for (int i = 0; i < Simulator.Data.Level.Descriptor.Waves.Count; i++)
            {
                var widget = (WaveSubPanel) GetWidgetByName("wave" + i);
                widget.Sync(Simulator.Data.Level.Descriptor.Waves[i]);
            }

            base.Open();
        }


        public override void Close()
        {
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            foreach (var w in Widgets)
            {
                var subPanel = (WaveSubPanel) w.Value;

                if (subPanel.EnemiesCount != 0 && subPanel.Quantity != 0)
                    descriptors.Add(subPanel.GenerateDescriptor());
            }

            Simulator.Data.Level.Waves.Clear();

            foreach (var wd in descriptors)
                Simulator.Data.Level.Waves.AddLast(new Wave(Simulator, wd));

            base.Close();
        }
    }
}
