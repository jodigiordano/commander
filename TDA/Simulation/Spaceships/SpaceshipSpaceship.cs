namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    class SpaceshipSpaceship : Spaceship
    {
        private Vector3 Acceleration;
        
        public Vector3 Bouncing;
        public double ActiveTime;
        public int BuyPrice;


        public SpaceshipSpaceship(Simulation simulation)
            : base(simulation)
        {
            RotationMaximaleRad = 0.2f;
            Acceleration = Vector3.Zero;
            BuyPrice = 50;
            SfxOut = "sfxPowerUpDoItYourselfOut";
            SfxIn = "sfxPowerUpDoItYourselfIn";
        }


        public override bool Active
        {
            get { return ActiveTime > 0; }
        }


        public float VisualPriority
        {
            set { this.Image.VisualPriority = value; }
        }


        public override void Update()
        {
            base.Update();

            ActiveTime -= 16.66f;

            if (AutomaticMode)
                DoAutomaticMode();
            else
                DoManualMode(NextInput);

            DoBouncing();
        }


        private void DoManualMode(Vector3 donneesThumbstick)
        {
            donneesThumbstick /= 10;

            donneesThumbstick.X = MathHelper.Clamp(donneesThumbstick.X, -1, 1);
            donneesThumbstick.Y = MathHelper.Clamp(donneesThumbstick.Y, -1, 1);

            if (donneesThumbstick.X != 0 || donneesThumbstick.Y != 0)
            {
                // Trouver la direction visée
                Vector3 directionVisee = donneesThumbstick;
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



            DoAcceleration(ref donneesThumbstick);

            Position += Speed * Acceleration;
        }


        private void DoBouncing()
        {
            if (Position.X > 640 - Preferences.DeadZoneXbox.X - Circle.Radius)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Acceleration.X) * Speed * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.X < -640 + Preferences.DeadZoneXbox.X + Circle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Acceleration.X) * Speed * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y > 370 - Preferences.DeadZoneXbox.Y - Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 1.5f;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Acceleration.Y) * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y < -370 + Preferences.DeadZoneXbox.Y + Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 1.5f;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Acceleration.Y) * Speed * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            Position += Bouncing;

            if (Bouncing.X > 0)
                Bouncing.X = Math.Max(0, Bouncing.X - 0.5f);
            else if (Bouncing.X < 0)
                Bouncing.X = Math.Min(0, Bouncing.X + 0.5f);

            if (Bouncing.Y > 0)
                Bouncing.Y = Math.Max(0, Bouncing.Y - 0.5f);
            else if (Bouncing.Y < 0)
                Bouncing.Y = Math.Min(0, Bouncing.Y + 0.5f);
        }


        private void DoAcceleration(ref Vector3 donneesThumbstick)
        {
            Acceleration += donneesThumbstick / 5f;

            Acceleration.X = MathHelper.Clamp(Acceleration.X, -2, 2);
            Acceleration.Y = MathHelper.Clamp(Acceleration.Y, -2, 2);

            if (Acceleration.X > 0)
                Acceleration.X = Math.Max(0, Acceleration.X - 0.03f);
            else if (Acceleration.X < 0)
                Acceleration.X = Math.Min(0, Acceleration.X + 0.03f);

            if (Acceleration.Y > 0)
                Acceleration.Y = Math.Max(0, Acceleration.Y - 0.03f);
            else if (Acceleration.Y < 0)
                Acceleration.Y = Math.Min(0, Acceleration.Y + 0.03f);
        }
    }
}
