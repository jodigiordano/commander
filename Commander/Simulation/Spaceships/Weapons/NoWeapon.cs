namespace EphemereGames.Commander.Simulation
{
    class NoWeapon : SpaceshipWeapon
    {
        public NoWeapon(Simulator simulator, Spaceship spaceship)
            : base(simulator, spaceship, double.MaxValue, float.MaxValue)
        {
            
        }


        protected override void DoFire()
        {
            
        }
    }
}
