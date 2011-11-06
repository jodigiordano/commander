namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Path : IVisual
    {
        private const int MaxVisibleLines = 400;
        private const int MaxCurvePoints = 400;
        private const int CelestialBodyDistance = 70;

        public double Length;
        public bool Active;
        public bool TakeIntoAccountFakeGravTurret;
        public bool TakeIntoAccountFakeGravTurretLv2;

        private Path3D InnerPath;
        private List<double> Times;
        private List<Vector3> Points;
        private OrderedBag<CelestialBody> CelestialBodiesPath;
        private int DistanceTwoPoints = 45;
        private Image[] Lines;
        private Simulator Simulator;
        private int PointsCount;
        private double LengthBefore;
        private ColorInterpolator ColorInterpolator;


        public Path(Simulator simulator, ColorInterpolator color, byte alpha, BlendType blend)
        {
            Simulator = simulator;
            ColorInterpolator = color;
            
            InnerPath = new Path3D();
            Times = new List<double>(MaxCurvePoints);
            Points = new List<Vector3>(MaxCurvePoints);

            for(int i = 0; i < MaxCurvePoints; i++)
            {
                Times.Add(0);
                Points.Add(Vector3.Zero);
            }

            PointsCount = 0;
            Length = 0;
            LengthBefore = 0;

            Lines = new Image[MaxVisibleLines];

            for (int i = 0; i < MaxVisibleLines; i++)
            {
                Lines[i] = new Image("LigneTrajet", Vector3.Zero)
                {
                    Blend = blend,
                    SizeX = 1.5f,
                    Alpha = alpha
                };
            }

            Active = true;
            TakeIntoAccountFakeGravTurret = false;
            TakeIntoAccountFakeGravTurretLv2 = false;
        }


        public void Initialize()
        {
            CelestialBodiesPath = new OrderedBag<CelestialBody>();

            foreach (var c in Simulator.Data.Level.PlanetarySystem)
                if (c.HasGravitationalTurret || (TakeIntoAccountFakeGravTurret && c.FakeHasGravitationalTurret))
                    AddCelestialBody(c, false);

            Active = true;

            Update();
        }


        public double LengthDelta
        {
            get
            {
                return Length - LengthBefore;
            }
        }


        public byte Alpha
        {
            get
            {
                return Lines[0].Color.A;
            }

            set
            {
                for (int i = 0; i < MaxVisibleLines; i++) //todo: only actually visible
                    Lines[i].Alpha = value;
            }
        }


        public CelestialBody FirstCelestialBody
        {
            get
            {
                return CelestialBodiesPath.Count == 0 ? null : CelestialBodiesPath.GetFirst();
            }
        }


        public CelestialBody LastCelestialBody
        {
            get
            {
                return CelestialBodiesPath.Count == 0 ? null : CelestialBodiesPath.GetLast();
            }
        }


        public bool ContainsCelestialBody(CelestialBody celestialBody)
        {
            return CelestialBodiesPath.Contains(celestialBody);
        }


        public void RemoveCelestialBody(CelestialBody celestialBody)
        {
            if (FirstCelestialBody != null)
            {
                FirstCelestialBody.FirstOnPath = false;
                LastCelestialBody.LastOnPath = false;
            }

            CelestialBodiesPath.Remove(celestialBody);

            if (FirstCelestialBody != null)
            {
                FirstCelestialBody.FirstOnPath = true;
                LastCelestialBody.LastOnPath = true;
            }
        }


        public void AddCelestialBody(CelestialBody celestialBody, bool beforeLast)
        {
            if (ContainsCelestialBody(celestialBody))
                return;

            if (celestialBody.PathPriority == int.MinValue)
                return;

            if (!celestialBody.HasGravitationalTurret && (TakeIntoAccountFakeGravTurret && !celestialBody.FakeHasGravitationalTurret))
                return;

            if (LastCelestialBody != null)
            {
                FirstCelestialBody.FirstOnPath = false;
                LastCelestialBody.LastOnPath = false;
                LastCelestialBody.StayOnPathUponDeath = false;

                if (beforeLast)
                {
                    var last = LastCelestialBody;

                    celestialBody.PathPriority = last.PathPriority;
                    CelestialBodiesPath.Remove(last);
                    last.PathPriority++;
                    CelestialBodiesPath.Add(last);
                }

                CelestialBodiesPath.Add(celestialBody);

                FirstCelestialBody.FirstOnPath = true;
                LastCelestialBody.LastOnPath = true;
                LastCelestialBody.StayOnPathUponDeath = true;
            }

            else
            {
                CelestialBodiesPath.Add(celestialBody);
            }
        }


        public void Position(double offset, ref Vector3 position)
        {
            InnerPath.GetPosition(offset, ref position);
        }


        public float GetPercentage(double offset)
        {
            return InnerPath.GetPercentage(offset);
        }


        public float GetRotation(double offset)
        {
            return InnerPath.GetRotation(offset);
        }


        public void Direction(double time, out Vector3 result)
        {
            InnerPath.GetDirection(time, out result);
        }


        public void Update()
        {
            if (!Active)
                return;

            RecalculatePath();
        }


        public void Draw()
        {
            if (!Active)
                return;

            int linesCount = Math.Min((int) (Length / DistanceTwoPoints) + 1, MaxVisibleLines);
            
            for (int j = 0; j < linesCount; j++)
            {
                var line = Lines[j];

                InnerPath.GetPosition(j * DistanceTwoPoints, ref line.position);

                line.Rotation = InnerPath.GetRotation(j * DistanceTwoPoints);
                ColorInterpolator.GetPerc((j + 1f) / linesCount, ref line.color);
                line.VisualPriority = VisualPriorities.Default.Path - InnerPath.GetPercentage(j * DistanceTwoPoints) / 100;
                Simulator.Scene.Add(line);
            }
        }


        public void Fade(int start, int end, EffectsController<IVisual> ec, Core.IntegerHandler callback)
        {
            var effect = VisualEffects.Fade(start, end, 0, 500);

            for (int i = 0; i < MaxVisibleLines; i++)
                ec.Add(Lines[i], effect, callback);
        }


        private void RecalculatePath()
        {
            UpdateInnerPoints();

            LengthBefore = Length;
            Length = 0;
            Times[0] = Length;

            for (int i = 1; i < PointsCount; i++)
            {
                Length += Vector3.Subtract(Points[i], Points[i - 1]).Length();
                Times[i] = Length;
            }

            InnerPath.Initialize(Points, Times, PointsCount);
        }


        private Vector3 _v1;
        private Matrix _matrix1;
        private void UpdateInnerPoints()
        {
            PointsCount = 0;

            for (int i = 0; i < CelestialBodiesPath.Count; i++)
            {
                var celestialBody = CelestialBodiesPath[i];

                // Straight line
                if (i == 0 || i == CelestialBodiesPath.Count - 1 || celestialBody.StraightLine)
                {
                    Points[PointsCount++] = CelestialBodiesPath[i].Position;
                    continue;
                }

                // Line between the previous and next CB
                Line.NearestPoint
                    (
                        ref CelestialBodiesPath[i - 1].position,
                        ref CelestialBodiesPath[i + 1].position,
                        ref celestialBody.position,
                        ref _v1
                    );

                // Perpendicular vector from line to CB
                Vector3.Subtract(ref celestialBody.position, ref _v1, out _v1);

                // Radius of CB. Depends on the distance of the CB from the line.
                // As the CB approach the line, the radius diminish
                float radius = MathHelper.Min(_v1.Length(), celestialBody.OuterTurretZone.Radius + CelestialBodyDistance);

                // Entry point
                var entryVec = celestialBody.position - CelestialBodiesPath[i - 1].position;
                var entryAngle = Core.Physics.Utilities.VectorToAngle(ref entryVec) + MathHelper.PiOver2;
                entryVec = celestialBody.position + Core.Physics.Utilities.AngleToVector(entryAngle) * radius;

                // Exit point
                var exitVec = celestialBody.position - CelestialBodiesPath[i + 1].position;
                var exitAngle = Core.Physics.Utilities.VectorToAngle(ref exitVec)- MathHelper.PiOver2;
                exitVec = celestialBody.position + Core.Physics.Utilities.AngleToVector(exitAngle) * radius;

                Points[PointsCount++] = entryVec;

                if (entryAngle < 0)
                    entryAngle += MathHelper.TwoPi;

                if (exitAngle < 0)
                    exitAngle += MathHelper.TwoPi;

                float angle = (entryAngle < exitAngle) ? entryAngle + (MathHelper.TwoPi - exitAngle) : entryAngle - exitAngle;
                float step = angle / 4;

                for (int y = 1; y < 4; y++)
                {
                    float next = entryAngle - y * step;

                    if (next < 0)
                        next += MathHelper.TwoPi;

                    Points[PointsCount++] = celestialBody.position +
                    Core.Physics.Utilities.AngleToVector(next) * radius;
                }

                Points[PointsCount++] = exitVec;

                if ((TakeIntoAccountFakeGravTurretLv2 && celestialBody.FakeHasGravitationalTurretLv2) || celestialBody.HasLevel2GravitationalTurret)
                {
                    step = MathHelper.TwoPi / 8;

                    for (int y = 1; y < 8; y++)
                    {
                        float next = exitAngle - y * step;

                        if (next < 0)
                            next += MathHelper.TwoPi;

                        Points[PointsCount++] = celestialBody.position +
                        Core.Physics.Utilities.AngleToVector(next) * radius;
                    }

                    Points[PointsCount++] = exitVec;
                }
            }
        }


        Rectangle IVisual.VisiblePart
        {
            set { throw new NotImplementedException(); }
        }


        Vector2 IVisual.Origin
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        Vector2 IVisual.Size
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        Color IVisual.Color
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
