namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodiesPathPreviews
    {
        public List<CelestialBody> CelestialBodies;

        private List<Image> Lines;
        private int NextVisualLine;
        private Simulator Simulator;
        private List<Vector3> PathPositions;

        private float LineSizeX;
        private const int MaxLines = 200;


        public CelestialBodiesPathPreviews(Simulator simulator)
        {
            Simulator = simulator;

            Lines = new List<Image>();

            for (int i = 0; i < MaxLines; i++)
                Lines.Add(new Image("LigneTrajet")
                {
                    Blend = BlendType.Add,
                    SizeX = 1,
                    Alpha = 50,
                    VisualPriority = VisualPriorities.Default.CelestialBodiePath
                });

            LineSizeX = Lines[0].AbsoluteSize.X;

            PathPositions = new List<Vector3>();
        }


        public void Draw()
        {
            NextVisualLine = 0;

            foreach (var c in CelestialBodies)
            {
                if (!c.ShowPath)
                    continue;

                if (c is AsteroidBelt)
                    continue;

                PathPositions.Clear();

                var maxXY = Math.Max(Math.Abs(c.Path.X), Math.Abs(c.Path.Y));

                int nbLines = (int) (maxXY / (LineSizeX / 10));


                for (int i = 0; i < nbLines + 1; i++)
                    PathPositions.Add(c.GetPositionAtPerc(i * (1f / nbLines)));

                for (int i = 0; i < nbLines; i++)
                {
                    var line = Lines[NextVisualLine++];

                    line.Position = PathPositions[i];
                    line.Rotation = Core.Physics.Utilities.VectorToAngle(PathPositions[i + 1] - PathPositions[i]);

                    Simulator.Scene.Add(line);

                    if (NextVisualLine == MaxLines)
                        return;
                }
            }
        }
    }
}
