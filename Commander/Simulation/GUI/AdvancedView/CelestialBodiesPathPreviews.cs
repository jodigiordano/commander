namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodiesPathPreviews
    {
        public List<CelestialBody> CelestialBodies;

        private Dictionary<CelestialBody, double> Highlights;

        private List<Image> NormalLines;
        private List<Image> HighlightLines;
        private int NextNormalLine;
        private int NextHighlightLine;
        private Simulator Simulator;
        private List<KeyValuePair<Vector3, float>> PathPositions;

        private float LineSizeX;
        private const int MaxNormalLines = 200;
        private const int MaxHighlightLines = 30;


        public CelestialBodiesPathPreviews(Simulator simulator)
        {
            Simulator = simulator;

            NormalLines = new List<Image>();

            for (int i = 0; i < MaxNormalLines; i++)
                NormalLines.Add(new Image("LigneTrajet")
                {
                    Blend = BlendType.Add,
                    SizeX = 1,
                    Alpha = 50,
                    VisualPriority = VisualPriorities.Default.CelestialBodiePath
                });

            HighlightLines = new List<Image>();

            for (int i = 0; i < MaxHighlightLines; i++)
                HighlightLines.Add(new Image("LigneTrajet")
                {
                    Blend = BlendType.Add,
                    SizeX = 1f,
                    Alpha = 150,
                    VisualPriority = VisualPriorities.Default.CelestialBodiePath - 0.000001
                });

            LineSizeX = NormalLines[0].AbsoluteSize.X;

            PathPositions = new List<KeyValuePair<Vector3, float>>();

            Highlights = new Dictionary<CelestialBody, double>();
        }


        public void Initialize()
        {
            Sync();
        }


        public void Sync()
        {
            Highlights.Clear();

            foreach (var c in CelestialBodies)
                Highlights.Add(c, c.ActualRotationTime);
        }


        public bool Visible
        {
            set
            {
                if (!value)
                    return;

                foreach (var c in CelestialBodies)
                    Highlights[c] = c.ActualRotationTime;
            }
        }


        public void Draw()
        {
            NextNormalLine = 0;
            NextHighlightLine = 0;

            foreach (var c in CelestialBodies)
            {
                if (!c.ShowPath)
                    continue;

                if (c is AsteroidBelt)
                    continue;

                PathPositions.Clear();

                var maxXY = Math.Max(Math.Abs(c.Path.X), Math.Abs(c.Path.Y));

                int nbLines = (int) (maxXY / (LineSizeX / 10));

                var highLightPerc = UpdateHighlight(c);
                bool hightlightShown = false;

                for (int i = 0; i < nbLines + 1; i++)
                {
                    float perc = i * (1f / nbLines);
                    PathPositions.Add(new KeyValuePair<Vector3, float>(c.GetPositionAtPerc(perc), perc)); //todo: can be removed
                }

                for (int i = 0; i < nbLines; i++)
                {
                    var line = NormalLines[NextNormalLine++];

                    line.Position = PathPositions[i].Key;
                    line.Rotation = Core.Physics.Utilities.VectorToAngle(PathPositions[i + 1].Key - PathPositions[i].Key);

                    Simulator.Scene.Add(line);

                    if (!hightlightShown && PathPositions[i].Value >= highLightPerc)
                    {
                        var highlightLine = HighlightLines[NextHighlightLine++];

                        highlightLine.Position = line.Position;
                        highlightLine.Rotation = line.Rotation;

                        Simulator.Scene.Add(highlightLine);

                        hightlightShown = true;
                    }

                    if (NextNormalLine == MaxNormalLines)
                        return;
                }
            }
        }


        private double UpdateHighlight(CelestialBody c)
        {
            if (c.Speed == float.MaxValue)
                return 1;

            var current = (Highlights[c] + Preferences.TargetElapsedTimeMs * 5) % c.Speed;

            Highlights[c] = current;

            return current / c.Speed;
        }
    }
}
