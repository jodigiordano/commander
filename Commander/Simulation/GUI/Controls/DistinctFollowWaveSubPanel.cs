namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class DistinctFollowWaveSubPanel : VerticalPanel
    {
        private ChoicesHorizontalSlider Distances;
        private NumericHorizontalSlider Delay;


        public DistinctFollowWaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            ShowFrame = false;

            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 0);
            Delay = new NumericHorizontalSlider("Delay", 2000, 10000, 2000, 100, 100);

            AddWidget("Distances", Distances);
            AddWidget("Delay", Delay);
        }


        public Distance GetDistance()
        {
            return (Distance) Enum.Parse(typeof(Distance), Distances.Value);
        }


        public int GetDelay()
        {
            return Delay.Value;
        }
    }
}
