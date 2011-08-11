namespace EphemereGames.Commander.Simulation
{
    class SpaceshipNoneBehavior : SpaceshipSteeringBehavior
    {
        public SpaceshipNoneBehavior(Spaceship spaceship)
            : base(spaceship)
        {
        }


        public override bool Active
        {
            get { return false; }
        }


        protected override void DoUpdate()
        {

        }
    }
}
