namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;

    
    class CelestialBodySteeringBehavior
    {
        public Vector3 BasePosition;
        public Vector3 Path;
        public double ActualRotationTime;

        private CelestialBody CelestialBody;
        private Matrix RotationMatrix;


        public CelestialBodySteeringBehavior(CelestialBody celestialBody)
        {
            CelestialBody = celestialBody;

            BasePosition = Vector3.Zero;
            Path = Vector3.Zero;
            ActualRotationTime = 0;
            PathRotation = 0;
            Speed = float.MaxValue;

            CelestialBody.position = CelestialBody.LastPosition = Path;
        }


        #region Speed

        private float speed;

        public float Speed
        {
            get { return speed; }
            set
            {
                double actualPourc = (speed == float.MaxValue) ? 0 : ActualRotationTime / speed;

                speed = value;

                ActualRotationTime = speed * actualPourc;

                Move();
            }
        }

        #endregion


        #region PathRotation

        private float pathRotation;

        public float PathRotation
        {
            get { return pathRotation; }
            set
            {
                pathRotation = value;

                Matrix.CreateRotationZ(pathRotation, out RotationMatrix);
            }
        }

        #endregion


        public void Move()
        {
            if (Speed != float.MaxValue)
                ActualRotationTime = (ActualRotationTime + Preferences.TargetElapsedTimeMs) % Speed;

            Move(Speed, ActualRotationTime, ref Path, ref BasePosition, ref RotationMatrix, ref CelestialBody.position);
        }


        public Vector3 GetPositionAtPerc(float perc)
        {
            Vector3 result = new Vector3();

            Move(1, MathHelper.Clamp(perc, 0, 1), ref Path, ref BasePosition, ref RotationMatrix, ref result);

            return result;
        }


        public void Move(float speed, double timeOffset, ref Vector3 offset, ref Vector3 result)
        {
            Move(speed, (ActualRotationTime + timeOffset) % speed, ref Path, ref offset, ref RotationMatrix, ref result);
        }


        public static void Move(double rotationTime, double actualRotationTime, ref Vector3 basePosition, ref Vector3 offset, ref Matrix rotationMatrix, ref Vector3 result)
        {
            if (rotationTime == float.MaxValue)
            {
                result.X = basePosition.X * (float) Math.Cos(0);
                result.Y = basePosition.Y * (float) Math.Sin(0);
                result.Z = 0;
            }

            else
            {
                result.X = basePosition.X * (float) Math.Cos((MathHelper.TwoPi / rotationTime) * actualRotationTime);
                result.Y = basePosition.Y * (float) Math.Sin((MathHelper.TwoPi / rotationTime) * actualRotationTime);
                result.Z = 0;
            }

            Vector3.Transform(ref result, ref rotationMatrix, out result);
            Vector3.Add(ref result, ref offset, out result);
        }
    }
}
