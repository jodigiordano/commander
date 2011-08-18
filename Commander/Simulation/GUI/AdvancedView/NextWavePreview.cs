namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NextWavePreview
    {
        public CelestialBody CelestialBody;
        public double TimeNextWave;
        public int RemainingWaves;

        private Text Progress;
        private Simulator Simulator;



        public NextWavePreview(Simulator simulator, double visualPriority)
        {
            Simulator = simulator;

            Progress = new Text("Pixelite")
            {
                SizeX = 2,
                Color = Colors.Default.AlienBright,
                VisualPriority = visualPriority
            };
        }


        public void Draw()
        {
            if (CelestialBody == null || RemainingWaves == 0)
                return;

            Progress.Data = RemainingWaves + " | " + String.Format("{0:0.0}", TimeNextWave / 1000);
            Progress.CenterIt();

            Progress.Position = CelestialBody.Position + new Vector3(0, CelestialBody.Circle.Radius + 15, 0);

            Simulator.Scene.Add(Progress);
        }
    }
}
