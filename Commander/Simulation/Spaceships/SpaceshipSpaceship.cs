namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SpaceshipSpaceship : Spaceship
    {
        private Vector3 Acceleration;
        
        public Vector3 Bouncing;
        public float Friction;
        public double ActiveTime;
        public int BuyPrice;


        public SpaceshipSpaceship(Simulator simulator)
            : base(simulator)
        {
            RotationMaximaleRad = 0.2f;
            Acceleration = Vector3.Zero;
            BuyPrice = 50;
            SfxOut = "sfxPowerUpDoItYourselfOut";
            SfxIn = "sfxPowerUpDoItYourselfIn";
            ShowTrail = true;

            Friction = 0;
        }


        public override bool Active
        {
            get { return ActiveTime > 0; }
        }


        public double VisualPriority
        {
            set { this.Image.VisualPriority = value; }
        }


        public override void Update()
        {
            base.Update();

            ActiveTime -= Preferences.TargetElapsedTimeMs;

            if (AutomaticMode)
                DoAutomaticMode();
            else
                DoManualMode(NextInput);

            DoBouncing();
            DoFriction();
        }


        public void Stop()
        {
            Bouncing = Vector3.Zero;
            Acceleration = Vector3.Zero;
            NextInput = Vector3.Zero;
        }


        private void DoManualMode(Vector3 input)
        {
            input /= 5;

            input.X = MathHelper.Clamp(input.X, -1, 1);
            input.Y = MathHelper.Clamp(input.Y, -1, 1);

            if (input.X != 0 || input.Y != 0)
            {
                // Trouver la direction visée
                Vector3 directionVisee = input;
                Vector3 direction = Direction;

                // Trouver l'angle d'alignement
                float angle = SignedAngle(ref directionVisee, ref direction);

                // Trouver la rotation nécessaire pour s'enligner
                float rotation = MathHelper.Clamp(RotationMaximaleRad, 0, Math.Abs(angle));

                if (angle > 0)
                    rotation = -rotation;

                // Appliquer la rotation
                Matrix.CreateRotationZ(rotation, out RotationMatrix);
                Vector3.Transform(ref direction, ref RotationMatrix, out direction);

                if (direction != Vector3.Zero)
                    direction.Normalize();

                Direction = direction;
            }



            DoAcceleration(ref input);

            Position += Speed * Acceleration;
        }


        private void DoFriction()
        {
            if (Friction > 0)
                Friction = Math.Max(0, Friction - 0.01f);
        }


        private void DoBouncing()
        {
            if (Position.X > 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Acceleration.X) * Speed * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.X < -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Acceleration.X) * Speed * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y > 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 1.5f;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Acceleration.Y) * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y < -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 1.5f;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Acceleration.Y) * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            Position += Bouncing;

            if (Bouncing.X > 0)
                Bouncing.X = Math.Max(0, Bouncing.X - 0.8f);
            else if (Bouncing.X < 0)
                Bouncing.X = Math.Min(0, Bouncing.X + 0.8f);

            if (Bouncing.Y > 0)
                Bouncing.Y = Math.Max(0, Bouncing.Y - 0.8f);
            else if (Bouncing.Y < 0)
                Bouncing.Y = Math.Min(0, Bouncing.Y + 0.8f);
        }


        private void DoAcceleration(ref Vector3 input)
        {
            Acceleration += input / 10f;

            Acceleration.X = MathHelper.Clamp(Acceleration.X, -2, 2);
            Acceleration.Y = MathHelper.Clamp(Acceleration.Y, -2, 2);

            if (Acceleration.X > 0)
                Acceleration.X = Math.Max(0, Acceleration.X - (0.03f + Friction));
            else if (Acceleration.X < 0)
                Acceleration.X = Math.Min(0, Acceleration.X + (0.03f + Friction));

            if (Acceleration.Y > 0)
                Acceleration.Y = Math.Max(0, Acceleration.Y - (0.03f + Friction));
            else if (Acceleration.Y < 0)
                Acceleration.Y = Math.Min(0, Acceleration.Y + (0.03f + Friction));
        }
    }
}
