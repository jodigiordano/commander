namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class LiteSimulator
    {
        private class LiteCelestialBody
        {
            public Vector3 Position;
            public Vector3 Offset;
            public float Rotation;
            public Size Size;
            public double RotationTime;

            private Vector3 StartingPosition;
            public double StartingTime;
            public Circle Circle;
            public double ActualRotationTime;
            private Matrix RotationMatrix;


            public LiteCelestialBody(Vector3 position, Vector3 offset, float rotation, int startingPosition, Size size, double speed)
            {
                Position = position;
                Offset = offset;
                Rotation = MathHelper.ToRadians(rotation);
                Size = size;
                RotationTime = speed;

                StartingPosition = Position;
                StartingTime = RotationTime * (startingPosition / 100.0f);
                Circle = new Circle(StartingPosition, (int)Size);
                ActualRotationTime = RotationTime * (startingPosition / 100.0f);
                Matrix.CreateRotationZ(Rotation, out RotationMatrix);
            }


            public void Move()
            {
                if (RotationTime == 0)
                    return;

                Position.X = StartingPosition.X * (float)Math.Cos((MathHelper.TwoPi / RotationTime) * ActualRotationTime);
                Position.Y = StartingPosition.Y * (float)Math.Sin((MathHelper.TwoPi / RotationTime) * ActualRotationTime);

                Vector3.Transform(ref Position, ref RotationMatrix, out Position);
                Vector3.Add(ref Position, ref Offset, out Position);

                Circle.Position = Position;
            }


            public static LiteCelestialBody ToLiteCelestialBody(CelestialBodyDescriptor descriptor)
            {
                return new LiteCelestialBody
                (
                    descriptor.Position,
                    descriptor.Offset,
                    descriptor.Rotation,
                    descriptor.StartingPosition,
                    descriptor.Size,
                    descriptor.Speed
                );
            }


            public static LiteCelestialBody ToLiteCelestialBody(CelestialBody celestialBody)
            {
                return ToLiteCelestialBody(celestialBody.GenerateDescriptor());
            }
        }


        public bool InBorders(CelestialBodyDescriptor descriptor, PhysicalRectangle limits)
        {
            LiteCelestialBody corpsCeleste = new LiteCelestialBody
            (
                descriptor.Position,
                descriptor.Offset,
                descriptor.Rotation,
                descriptor.StartingPosition,
                descriptor.Size,
                descriptor.Speed
            );

            // au temps 0
            corpsCeleste.ActualRotationTime = 0;
            corpsCeleste.Move();
            if (!limits.Includes(corpsCeleste.Position + new Vector3((int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(-(int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, (int)descriptor.Size, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, -(int)descriptor.Size, 0)))
                return false;

            // au temps 1/4
            corpsCeleste.ActualRotationTime = corpsCeleste.RotationTime / 4;
            corpsCeleste.Move();
            if (!limits.Includes(corpsCeleste.Position + new Vector3((int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(-(int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, (int)descriptor.Size, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, -(int)descriptor.Size, 0)))
                return false;

            // au temps 1/2
            corpsCeleste.ActualRotationTime = corpsCeleste.RotationTime / 2;
            corpsCeleste.Move();
            if (!limits.Includes(corpsCeleste.Position + new Vector3((int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(-(int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, (int)descriptor.Size, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, -(int)descriptor.Size, 0)))
                return false;

            // au temps 3/4
            corpsCeleste.ActualRotationTime = corpsCeleste.RotationTime * (3.0/4.0);
            corpsCeleste.Move();
            if (!limits.Includes(corpsCeleste.Position + new Vector3((int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(-(int)descriptor.Size, 0, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, (int)descriptor.Size, 0)) ||
                !limits.Includes(corpsCeleste.Position + new Vector3(0, -(int)descriptor.Size, 0)))
                return false;

            return true;
        }


        public bool CollidesWithOthers(CelestialBodyDescriptor celestialBody, List<CelestialBody> others)
        {
            LiteCelestialBody celestialBodyLite = LiteCelestialBody.ToLiteCelestialBody(celestialBody);

            List<LiteCelestialBody> othersLites = new List<LiteCelestialBody>();

            foreach (var c in others)
                othersLites.Add(LiteCelestialBody.ToLiteCelestialBody(c));

            for (int i = 0; i < othersLites.Count; i++)
            {
                LiteCelestialBody other = othersLites[i];

                double rotationTime = Math.Max(celestialBodyLite.RotationTime, other.RotationTime);

                for (double x = 0; x <= rotationTime; x += 250)
                {
                    celestialBodyLite.ActualRotationTime = (celestialBodyLite.StartingTime + x) % celestialBodyLite.RotationTime;
                    celestialBodyLite.Move();

                    other.ActualRotationTime = (other.StartingTime + x) % other.RotationTime;
                    other.Move();

                    if (celestialBodyLite.Circle.Intersects(other.Circle))
                        return true;
                }
            }

            return false;
        }
    }
}
