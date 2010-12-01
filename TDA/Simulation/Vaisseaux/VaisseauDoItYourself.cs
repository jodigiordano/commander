namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    class VaisseauDoItYourself : Vaisseau
    {
        private Vector3 Acceleration;
        
        public Vector3 Bouncing;
        public double TempsActif;
        public bool ModeAutomatique;
        public int PrixAchat;
        public Vector2 NextInput;


        public virtual bool Actif
        {
            get { return TempsActif > 0; }
        }

        public float PrioriteAffichage
        {
            set { this.Representation.PrioriteAffichage = value; }
        }


        public VaisseauDoItYourself(Simulation simulation)
            : base(simulation)
        {
            RotationMaximaleRad = 0.2f;
            Acceleration = Vector3.Zero;
            PrixAchat = 50;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TempsActif -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (ModeAutomatique)
                doModeAutomatique();
            else
                doModeManuel(NextInput);

            doBouncing();
        }


        private void doModeManuel(Vector2 donneesThumbstick)
        {
#if XBOX || MANETTE_WINDOWS
            donneesThumbstick.Y = -donneesThumbstick.Y;

            if (donneesThumbstick.X != 0 || donneesThumbstick.Y != 0)
            {
                Vector3 direction = (Position + new Vector3(donneesThumbstick, 0)) - Position;
                direction.Normalize();

                Direction = direction;
            }
#else
            donneesThumbstick /= 10;

            donneesThumbstick.X = MathHelper.Clamp(donneesThumbstick.X, -1, 1);
            donneesThumbstick.Y = MathHelper.Clamp(donneesThumbstick.Y, -1, 1);

            if (donneesThumbstick.X != 0 || donneesThumbstick.Y != 0)
            {
                // Trouver la direction visée
                Vector3 directionVisee = new Vector3(donneesThumbstick, 0);
                Vector3 direction = Direction;

                // Trouver l'angle d'alignement
                float angle = signedAngle(ref directionVisee, ref direction);

                // Trouver la rotation nécessaire pour s'enligner
                float rotation = MathHelper.Clamp(RotationMaximaleRad, 0, Math.Abs(angle));

                if (angle > 0)
                    rotation = -rotation;

                // Appliquer la rotation
                Matrix.CreateRotationZ(rotation, out MatriceRotation);
                Vector3.Transform(ref direction, ref MatriceRotation, out direction);

                if (direction != Vector3.Zero)
                    direction.Normalize();

                Direction = direction;
            }
#endif



            doAcceleration(ref donneesThumbstick);

            Position += Vitesse * Acceleration;
        }


        public override void doModeAutomatique()
        {
            PositionVisee = CorpsCelesteDepart.Position;

            base.doModeAutomatique();
        }

        private void doBouncing()
        {
            if (Position.X > 640 - Preferences.DeadZoneXbox.X - Representation.Rectangle.Width / 2)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Acceleration.X) * Vitesse * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Vitesse * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.X < -640 + Preferences.DeadZoneXbox.X + Representation.Rectangle.Width / 2)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Acceleration.X) * Vitesse * 1.5f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Vitesse * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y > 370 - Preferences.DeadZoneXbox.Y - Representation.Rectangle.Height / 2)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Vitesse * 1.5f;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Acceleration.Y) * Vitesse * 1.5f;

                Acceleration.X = 0;
                Acceleration.Y = 0;
            }

            if (Position.Y < -370 + Preferences.DeadZoneXbox.Y + Representation.Rectangle.Height / 2)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Vitesse * 1.5f;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Acceleration.Y) * Vitesse * 1.5f;

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

        private void doAcceleration(ref Vector2 donneesThumbstick)
        {
            Acceleration += new Vector3(donneesThumbstick, 0) / 5f;

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


        private float signedAngle(ref Vector3 vecteur1, ref Vector3 vecteur2)
        {
            float perpDot = vecteur1.X * vecteur2.Y - vecteur1.Y * vecteur2.X;

            return (float)Math.Atan2(perpDot, Vector3.Dot(vecteur1, vecteur2));
        }
    }
}
