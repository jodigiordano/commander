namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;


    public class Path
    {
        private Microsoft.Xna.Framework.Curve Curve;
        public double Length;


        public Path() : this(new List<float>(), new List<float>()) { }


        public Path(List<float> positions, List<float> times)
        {
            Curve = new Microsoft.Xna.Framework.Curve()
            {
                PreLoop = Microsoft.Xna.Framework.CurveLoopType.Constant,
                PostLoop = Microsoft.Xna.Framework.CurveLoopType.Constant
            };

            Length = 0;

            Initialize(positions, times);
        }


        public void Initialize(List<float> positions, List<float> times)
        {
            Initialize(positions, times, (int) Math.Min(positions.Count, times.Count));
        }


        public void Initialize(List<float> positions, List<float> times, int qty)
        {
            Curve.Keys.Clear();

            for (int j = 0; j < qty; j++)
                Curve.Keys.Add(new Microsoft.Xna.Framework.CurveKey((float) times[j], positions[j]));

            Curve.ComputeTangents(Microsoft.Xna.Framework.CurveTangent.Smooth);

            Length = (qty > 0) ? times[qty - 1] : 0;
        }


        public float GetStartingPosition()
        {
            return Curve.Evaluate(0.0f);
        }

        public float GetEndingPosition()
        {
            return Curve.Evaluate(Curve.Keys[Curve.Keys.Count - 1].Value);
        }


        public float GetPosition(float time)
        {
            return Curve.Evaluate(time);
        }


        public float GetRelativePosition(float time)
        {
            return Curve.Evaluate(time) - Curve.Evaluate(0.0f);
        }


        public double GetDirection(float time)
        {
            float positionBefore = GetPosition(time);
            float positionAfter = GetPosition(time + 1);

            return positionAfter - positionBefore;
        }


        public static Path CreateCurve(CurveType type, float time)
        {
            Path path = new Path();

            switch (type)
            {
                case CurveType.InversedLinear:
                    path.Initialize(new List<float>() { 1, 0 }, new List<float> { 0, time });
                    break;

                case CurveType.Linear:
                    path.Initialize(new List<float>() { 0, 1 }, new List<float> { 0, time });
                    break;

                case CurveType.Exponential:
                    path.Initialize(new List<float>() { 0, 0.1f, 1 }, new List<float> { 0, time / 2, time });
                    break;

                case CurveType.Log:
                    path.Initialize(new List<float>() { 0, 0.8f, 1 }, new List<float> { 0, time / 2, time });
                    break;
            }

            return path;
        }
    }
}
