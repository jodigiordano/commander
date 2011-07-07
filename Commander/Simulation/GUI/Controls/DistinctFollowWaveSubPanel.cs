namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class DistinctFollowWaveSubPanel : VerticalPanel
    {
        private ChoicesHorizontalSlider Distances;
        private NumericHorizontalSlider DelayWidget;


        public DistinctFollowWaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            ShowFrame = false;

            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 0);
            DelayWidget = new NumericHorizontalSlider("Delay", 2000, 10000, 2000, 100, 100);

            AddWidget("Distances", Distances);
            AddWidget("Delay", DelayWidget);
        }


        public Distance Distance
        {
            get { return (Distance) Enum.Parse(typeof(Distance), Distances.Value); }
            set { Distances.Value = value.ToString("g"); }
        }


        public int Delay
        {
            get { return DelayWidget.Value; }
            set { DelayWidget.Value = value; }
        }
    }
}
