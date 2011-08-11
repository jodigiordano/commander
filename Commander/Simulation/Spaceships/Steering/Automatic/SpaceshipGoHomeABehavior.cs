namespace EphemereGames.Commander.Simulation
{
    class SpaceshipGoHomeABehavior : SpaceshipChasingABehavior
    {
        public SpaceshipGoHomeABehavior(Spaceship spaceship)
            : base(spaceship, spaceship.StartingObject.Position)
        {
            Spaceship.Position = Spaceship.StartingObject.Position;
            Targeting = true;
        }


        protected override void DoUpdate()
        {
            if (Targeting)
                TargetPosition = Spaceship.StartingObject.Position;

            base.Update();
        }
    }
}
