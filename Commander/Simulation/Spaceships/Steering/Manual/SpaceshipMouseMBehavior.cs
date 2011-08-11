﻿namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class SpaceshipMouseMBehavior : SpaceshipSteeringBehavior
    {
        private float MaxRotation;


        public SpaceshipMouseMBehavior(Spaceship spaceship)
            : base(spaceship)
        {
            MaxRotation = MaxRotationPerUpdateRad;
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
                ApplyRotation(ref rotation);
            else
                ApplyRotation(ref movement);

            ApplyAcceleration(ref movement);

            Spaceship.Position += Speed * Acceleration;
        }
    }
}
