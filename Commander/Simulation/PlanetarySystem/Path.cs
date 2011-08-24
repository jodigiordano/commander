namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Path
    {
        private const int MaxVisibleLines = 300;
        private const int MaxCurvePoints = 300;
        private const int CelestialBodyDistance = 70;

        public List<CelestialBody> CelestialBodies;
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
        private Scene Scene;
        private int NbPoints;
        private double LengthBefore;


        public Path(Simulator simulator, Color color, BlendType blend)
        {
            Scene = simulator.Scene;
            
            InnerPath = new Path3D();
            Times = new List<double>(MaxCurvePoints);
            Points = new List<Vector3>(MaxCurvePoints);

            for(int i = 0; i < MaxCurvePoints; i++)
            {
                Times.Add(0);
                Points.Add(Vector3.Zero);
            }

            NbPoints = 0;
            Length = 0;
            LengthBefore = 0;

            Lines = new Image[MaxVisibleLines];

            for (int i = 0; i < MaxVisibleLines; i++)
            {
                Lines[i] = new Image("LigneTrajet", Vector3.Zero)
                {
                    Color = color,
                    Blend = blend,
                    SizeX = 1.5f
                };
            }

            Active = true;
            TakeIntoAccountFakeGravTurret = false;
            TakeIntoAccountFakeGravTurretLv2 = false;
        }


        public void Initialize()
        {
            CelestialBodiesPath = new OrderedBag<CelestialBody>();

            foreach (var c in CelestialBodies)
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
                line.color.G = line.color.B = (byte) (255 * (1 - (((float) j + 1) / linesCount)));
                line.VisualPriority = VisualPriorities.Default.Path - InnerPath.GetPercentage(j * DistanceTwoPoints) / 100;
                Scene.Add(line);
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

            for (int i = 1; i < NbPoints; i++)
            {
                _vecteur1 = Vector3.Subtract(Points[i], Points[i - 1]);
                Length += _vecteur1.Length();
                Times[i] = Length;
            }

            InnerPath.Initialize(Points, Times, NbPoints);
        }


        private Vector3 _vecteur1, _vecteur2;
        private Matrix _matrice1;
        private void UpdateInnerPoints()
        {
            NbPoints = 0;

            for (int i = 0; i < CelestialBodiesPath.Count; i++)
            {
                var celestialBody = CelestialBodiesPath[i];

                if (i == 0 || i == CelestialBodiesPath.Count - 1 || celestialBody.StraightLine)
                {
                    Points[NbPoints++] = CelestialBodiesPath[i].Position;
                    continue;
                }

                // Ligne
                Line.PointPlusProche
                    (
                        ref CelestialBodiesPath[i - 1].position,
                        ref CelestialBodiesPath[i + 1].position,
                        ref celestialBody.position,
                        ref _vecteur1
                    );

                // Vecteur perpendiculaire
                Vector3.Subtract(ref celestialBody.position, ref _vecteur1, out _vecteur1);

                //Radius
                float radius = MathHelper.Min(_vecteur1.Length(), celestialBody.OuterTurretZone.Radius + CelestialBodyDistance);

                //Entry point
                Vector3 vecPO = celestialBody.position - CelestialBodiesPath[i - 1].position;
                float POangle = Core.Physics.Utilities.VectorToAngle(ref vecPO);
                POangle += MathHelper.PiOver2;
                Vector3 entryVec = celestialBody.position +
                    Core.Physics.Utilities.AngleToVector(POangle) * radius;

                //Exit point
                Vector3 vecPD = celestialBody.position - CelestialBodiesPath[i + 1].position;
                float PDangle = Core.Physics.Utilities.VectorToAngle(ref vecPD);
                PDangle -= MathHelper.PiOver2;
                Vector3 exitVec = celestialBody.position +
                    Core.Physics.Utilities.AngleToVector(PDangle) * radius;

                Points[NbPoints++] = entryVec;

                if (POangle < 0)
                    POangle += MathHelper.TwoPi;

                if (PDangle < 0)
                    PDangle += MathHelper.TwoPi;

                float angle = (POangle < PDangle) ? POangle + (MathHelper.TwoPi - PDangle) : POangle - PDangle;
                float step = angle / 4;

                for (int y = 1; y < 4; y++)
                {
                    float next = POangle - y * step;

                    if (next < 0)
                        next += MathHelper.TwoPi;

                    Points[NbPoints++] = celestialBody.position +
                    Core.Physics.Utilities.AngleToVector(next) * radius;
                }

                Points[NbPoints++] = exitVec;

                if ((TakeIntoAccountFakeGravTurretLv2 && celestialBody.FakeHasGravitationalTurretLv2) || celestialBody.HasLevel2GravitationalTurret)
                {
                    step = MathHelper.TwoPi / 8;

                    for (int y = 1; y < 8; y++)
                    {
                        float next = PDangle - y * step;

                        if (next < 0)
                            next += MathHelper.TwoPi;

                        Points[NbPoints++] = celestialBody.position +
                        Core.Physics.Utilities.AngleToVector(next) * radius;
                    }

                    Points[NbPoints++] = exitVec;
                }
            }
        }
    }
}
