namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public class Path3D
    {
        private List<Curve> Curves;
        private double Length;

        private static Pool<CurveKey> CurveKeysPool;


        public Path3D() : this(new List<Vector3>(), new List<double>()) { }


        public Path3D(List<Vector3> positions, List<double> gameTimes)
        {
            Curves = new List<Curve>(3);

            for (int i = 0; i < 3; i++)
                Curves.Add(new Curve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant });

            Length = 0;

            CurveKeysPool = new Pool<CurveKey>();

            Initialize(positions, gameTimes);
        }


        public void Initialize(List<Vector3> positions, List<double> gameTimes)
        {
            Initialize(positions, gameTimes, (int) Math.Min(positions.Count, gameTimes.Count));
        }


        public void Initialize(List<Vector3> positions, List<double> gameTimes, int qty)
        {
            for (int i = 0; i < Curves.Count; i++)
            {
                Curve c = Curves[i];
                int keysAvailables = c.Keys.Count;
                int toAdd = Math.Max(qty - keysAvailables, 0);
                int toDelete = Math.Max(keysAvailables - qty, 0);

                for (int j = 0; j < toAdd; j++)
                    c.Keys.Add(CurveKeysPool.Get());

                for (int j = 0; j < toDelete; j++)
                {
                    CurveKeysPool.Return((CurveKey) c.Keys[c.Keys.Count - 1]);
                    c.Keys.RemoveAt(c.Keys.Count - 1);
                }
                
                for (int j = 0; j < qty; j++)
                {
                    var k = c.Keys[j];
                    k.Position = (float) gameTimes[j];
                    k.Value = (i == 0) ? positions[j].X : (i == 1) ? positions[j].Y : positions[j].Z;
                }
            }

            for (int j = 0; j < Curves.Count; j++)
                Curves[j].ComputeTangents(CurveTangent.Smooth);

            Length = (qty > 0) ? gameTimes[qty - 1] : 0;
        }


        public void GetStartingPosition(ref Vector3 result)
        {
            result.X = Curves[0].Evaluate(0.0f);
            result.Y = Curves[1].Evaluate(0.0f);
            result.Z = Curves[2].Evaluate(0.0f);
        }


        public void GetPosition(double time, ref Vector3 result)
        {
            result.X = Curves[0].Evaluate((float)time);
            result.Y = Curves[1].Evaluate((float)time);
            result.Z = Curves[2].Evaluate((float)time);
        }


        public Vector3 GetPosition(double time)
        {
            return new Vector3
            (
                Curves[0].Evaluate((float) time),
                Curves[1].Evaluate((float) time),
                Curves[2].Evaluate((float) time)
            );
        }


        public float GetPercentage(double time)
        {
            return (Length != 0) ? (float) (time / Length) : 0;
        }


        public void GetRelativePosition(double time, ref Vector3 result)
        {
            result.X = Curves[0].Evaluate((float)time) - Curves[0].Evaluate(0f);
            result.Y = Curves[1].Evaluate((float)time) - Curves[1].Evaluate(0f);
            result.Z = Curves[2].Evaluate((float)time) - Curves[2].Evaluate(0f);
        }


        public float GetRotation(double time)
        {
            Vector3 v;

            GetDirection(time, out v);

            if (v == Vector3.Zero)
                return MathHelper.PiOver2;
            else
                return MathHelper.PiOver2 + (float) Math.Atan2(v.Y, v.X);
        }


        public void GetDirection(double time, out Vector3 result)
        {
            Vector3 v1 = new Vector3();
            Vector3 v2 = new Vector3();

            GetPosition(time, ref v1);
            GetPosition(time + 1, ref v2);

            Vector3.Subtract(ref v2, ref v1, out result);

            result.Normalize();
        }


        public static Path3D CreerVitesse(CurveType type, double time)
        {
            Path3D path = new Path3D();

            switch (type)
            {
                case CurveType.Linear:
                    path.Initialize(new List<Vector3>() { Vector3.Zero, Vector3.One }, new List<double> { 0 , time });
                    break;

                case CurveType.Exponential:
                    path.Initialize(new List<Vector3>() { Vector3.Zero, new Vector3(0.8f, 0.1f, 0.1f), Vector3.One }, new List<double> { 0, time / 2, time });
                    break;

                case CurveType.Log:
                    path.Initialize(new List<Vector3>() { Vector3.Zero, new Vector3(0.1f, 0.8f, 0.8f), Vector3.One }, new List<double> { 0, time / 2, time });
                    break;
            }

            return path;
        }
    }
}
