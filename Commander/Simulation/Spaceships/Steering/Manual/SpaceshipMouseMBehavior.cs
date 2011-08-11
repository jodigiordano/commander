namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class SpaceshipMouseMBehavior : SpaceshipSteeringBehavior
    {
        private float MaxRotation;
        private float MaxMouseLengthPxSquared;


        public SpaceshipMouseMBehavior(Spaceship spaceship)
            : base(spaceship)
        {
            MaxRotation = MaxRotationPerUpdateRad;
            MaxMouseLengthPxSquared = (float) Math.Pow(12, 2);
        }


        public override bool Active
        {
            get { return true; }
        }


        protected override void DoUpdate()
        {
            var movement = NextMovement / 5;
            var rotation = NextRotation;

            movement.X = MathHelper.Clamp(movement.X, -1, 1);
            movement.Y = MathHelper.Clamp(movement.Y, -1, 1);


            if (rotation != Vector3.Zero)
            {
                MaxRotationPerUpdateRad = (Math.Min(rotation.LengthSquared(), MaxMouseLengthPxSquared) / MaxMouseLengthPxSquared) * MaxRotation;
                ApplyRotation(ref rotation);
            }
            else
            {
                MaxRotationPerUpdateRad = MaxRotation;
                ApplyRotation(ref movement);
            }

            ApplyAcceleration(ref movement);

            Spaceship.Position += Speed * Acceleration;
        }
    }
}
