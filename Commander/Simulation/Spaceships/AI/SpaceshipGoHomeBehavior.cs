namespace EphemereGames.Commander.Simulation
{
    class SpaceshipGoHomeBehavior : SpaceshipChasingBehavior
    {
        public SpaceshipGoHomeBehavior(Spaceship spaceship)
            : base(spaceship, spaceship.StartingObject.Position)
        {
            Spaceship.Position = Spaceship.StartingObject.Position;
            Targeting = true;
        }


        public override void Update()
        {
            if (Targeting)
                TargetPosition = Spaceship.StartingObject.Position;

            base.Update();
        }
    }
}
