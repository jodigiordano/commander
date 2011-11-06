namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    abstract class SpaceshipSteeringBehavior
    {
        // for manual steering
        public bool ManualMovementInputThisTick;
        public Vector3 NextMovement;
        public Vector3 NextRotation;
        public Vector3 LastNextMovement;
        public Vector3 LastNextRotation;
        public float Speed;
        public float Friction;

        public abstract bool Active { get; }

        protected Spaceship Spaceship;
        protected float MaxRotationPerUpdateRad;

        public Vector3 Acceleration;
        public Vector3 Bouncing;

        private Matrix RotationMatrix;

        private static List<int> SafeBouncing = new List<int>() { -20, -18, -16, -14, -10, 10, 14, 16, 18, 20 };

        private float MaxAcceleration;


        public SpaceshipSteeringBehavior(Spaceship spaceship)
        {
            Spaceship = spaceship;
            MaxRotationPerUpdateRad = 0.2f;
            Acceleration = Vector3.Zero;
            MaxAcceleration = 2f;
            ManualMovementInputThisTick = false;
            NextMovement = Vector3.Zero;
            NextRotation = Vector3.Zero;
            LastNextMovement = Vector3.Zero;
            LastNextRotation = Vector3.Zero;
            Speed = 4;
        }


        public void Update()
        {
            ManualMovementInputThisTick = NextMovement != Vector3.Zero;

            DoUpdate();
            ApplyBouncing();
            ApplyFriction();

            LastNextMovement = NextMovement;
            LastNextRotation = NextRotation;

            NextMovement = Vector3.Zero;
            NextRotation = Vector3.Zero;
        }


        protected abstract void DoUpdate();


        public Vector3 Momentum
        {
            get { return Speed * Acceleration; }
        }


        public float MaximumMomentum
        {
            get { return Speed * MaxAcceleration; }
        }


        public void Stop()
        {
            Bouncing = Vector3.Zero;
            Acceleration = Vector3.Zero;
            NextMovement = Vector3.Zero;
            NextRotation = Vector3.Zero;
        }


        protected virtual void ApplyRotation(ref Vector3 movement)
        {
            // Movement direction
            Vector3 currentDirection = movement;

            // Current direction
            Vector3 direction = Spaceship.Direction;

            // Delta, Converted to angle
            float angle = Core.Physics.Utilities.SignedAngle(ref currentDirection, ref direction);

            // Clamp to maximum allowed by the ship
            float rotation = MathHelper.Clamp(MaxRotationPerUpdateRad, 0, Math.Abs(angle));

            if (angle > 0)
                rotation = -rotation;

            // Apply
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref direction, ref RotationMatrix, out direction);

            if (direction != Vector3.Zero)
                direction.Normalize();

            bool rotated = Spaceship.Direction != direction;

            Spaceship.Direction = direction;

            if (rotated)
                Spaceship.NotifyRotated();
        }


        protected virtual void ApplyFriction()
        {
            if (Friction > 0)
                Friction = Math.Max(0, Friction - 0.01f); //lower == less on planets
        }


        protected virtual void ApplyBouncing()
        {
            Direction d = Direction.None;

            if (Spaceship.Position.X > Spaceship.Simulator.Data.Battlefield.Right - Spaceship.Circle.Radius)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Acceleration.X) * Spaceship.Speed * 2f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Spaceship.Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                d = Direction.Right;
            }

            if (Spaceship.Position.X < Spaceship.Simulator.Data.Battlefield.Left + Spaceship.Circle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Acceleration.X) * Spaceship.Speed * 2f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Spaceship.Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                d = Direction.Left;
            }

            if (Spaceship.Position.Y > Spaceship.Simulator.Data.Battlefield.Bottom - Spaceship.Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Spaceship.Speed * 2f;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Acceleration.Y) * Spaceship.Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                d = Direction.Down;
            }

            if (Spaceship.Position.Y < Spaceship.Simulator.Data.Battlefield.Top + Spaceship.Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Spaceship.Speed * 2f;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Acceleration.Y) * Spaceship.Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                d = Direction.Up;
            }

            Spaceship.Position += Bouncing;

            if (Bouncing.X > 0)
                Bouncing.X = Math.Max(0, Bouncing.X - 0.6f);
            else if (Bouncing.X < 0)
                Bouncing.X = Math.Min(0, Bouncing.X + 0.6f);

            if (Bouncing.Y > 0)
                Bouncing.Y = Math.Max(0, Bouncing.Y - 0.6f);
            else if (Bouncing.Y < 0)
                Bouncing.Y = Math.Min(0, Bouncing.Y + 0.6f);

            if (d != Direction.None)
                Spaceship.NotifyBounced(d);
        }


        protected virtual void ApplyAcceleration(ref Vector3 input)
        {
            Acceleration += input / 10f;

            Acceleration.X = MathHelper.Clamp(Acceleration.X, -MaxAcceleration, MaxAcceleration);
            Acceleration.Y = MathHelper.Clamp(Acceleration.Y, -MaxAcceleration, MaxAcceleration);

            if (Acceleration.X > 0)
                Acceleration.X = Math.Max(0, Acceleration.X - (0.03f + Friction)); //lower literal == less in general
            else if (Acceleration.X < 0)
                Acceleration.X = Math.Min(0, Acceleration.X + (0.03f + Friction));

            if (Acceleration.Y > 0)
                Acceleration.Y = Math.Max(0, Acceleration.Y - (0.03f + Friction));
            else if (Acceleration.Y < 0)
                Acceleration.Y = Math.Min(0, Acceleration.Y + (0.03f + Friction));
        }


        public void ApplySafeBouncing()
        {
            Bouncing = new Vector3(SafeBouncing[Main.Random.Next(0, SafeBouncing.Count)], SafeBouncing[Main.Random.Next(0, SafeBouncing.Count)], 0);
        }
    }
}
