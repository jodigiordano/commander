namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;

    
    class SpaceshipChasingABehavior : SpaceshipSteeringBehavior
    {
        public Vector3 TargetPosition   { get { return targetPosition; } set { targetPosition = value; Targeting = true; TargetReached = false; } }
        public bool TargetReached       { get; protected set; }
        public bool Targeting           { get; protected set; }

        private Vector3 targetPosition;
        private Matrix RotationMatrix;


        public SpaceshipChasingABehavior(Spaceship spaceship, Vector3 targetPosition)
            : base(spaceship)
        {
            TargetPosition = targetPosition;
            TargetReached = false;
            Targeting = true;
        }


        public override bool Active
        {
            get { return Targeting && !TargetReached; }
        }


        protected override void DoUpdate()
        {
            if (TargetReached || !Targeting)
                return;

            // Find the targeted direction
            Vector3 targetedDirection = TargetPosition - Spaceship.Position;
            Vector3 direction = Spaceship.Direction;

            // Find angle toLocal align toLocal
            float angle = Core.Physics.Utilities.SignedAngle(ref targetedDirection, ref direction);

            // Find rotation needed toLocal get aligned
            float rotation = MathHelper.Clamp(MaxRotationPerUpdateRad, 0, Math.Abs(angle));

            if (angle > 0)
                rotation = -rotation;

            // Apply rotation
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref direction, ref RotationMatrix, out direction);

            if (direction != Vector3.Zero)
                direction.Normalize();

            Spaceship.Direction = direction;

            // Apply movement
            Spaceship.Position += Spaceship.Direction * Spaceship.Speed;

            if ((TargetPosition - Spaceship.Position).LengthSquared() <= 600)
            {
                Targeting = false;
                TargetReached = true;
            }
        }
    }
}
