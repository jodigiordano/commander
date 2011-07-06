namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodiesPathPreviews
    {
        public List<CelestialBody> CelestialBodies;

        private List<VisualLine> Lines;
        private int NextVisualLine;
        private Simulator Simulator;
        private List<Vector3> PathPositions;


        public CelestialBodiesPathPreviews(Simulator simulator)
        {
            Simulator = simulator;

            Lines = new List<VisualLine>();

            for (int i = 0; i < 200; i++)
                Lines.Add(new VisualLine(Vector3.Zero, Vector3.Zero, Color.White));

            PathPositions = new List<Vector3>();
        }


        public void Draw()
        {
            NextVisualLine = 0;

            foreach (var c in CelestialBodies)
            {
                if (!c.ShowPath)
                    continue;

                PathPositions.Clear();

                for (int i = 0; i < 21; i++)
                    PathPositions.Add(c.GetPositionAtPerc(i * 0.05f));

                for (int i = 0; i < 20; i++)
                {
                    var line = Lines[NextVisualLine++];

                    line.Start = PathPositions[i];
                    line.End = PathPositions[i+1];
                    line.VisualPriority = Preferences.PrioriteFondEcran - 0.00001;

                    Simulator.Scene.Add(line);
                }
            }
        }
    }
}
