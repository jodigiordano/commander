namespace EphemereGames.Commander.Simulation
{
    abstract class SpaceshipBehavior
    {
        public abstract bool Active { get; }

        protected Spaceship Spaceship;


        public SpaceshipBehavior(Spaceship spaceship)
        {
            Spaceship = spaceship;
        }


        public abstract void Update();
    }
}
