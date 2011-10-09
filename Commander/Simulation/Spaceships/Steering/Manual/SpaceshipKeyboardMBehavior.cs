namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class SpaceshipKeyboardMBehavior : SpaceshipSteeringBehavior
    {
        public SpaceshipKeyboardMBehavior(Spaceship spaceship)
            : base(spaceship)
        {

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
                ApplyRotation(ref rotation);
            }
            else
            {
                ApplyRotation(ref movement);
            }

            ApplyAcceleration(ref movement);

            Spaceship.Position += Speed * Acceleration;
        }
    }
}
