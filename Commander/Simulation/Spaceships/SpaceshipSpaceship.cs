namespace EphemereGames.Commander.Simulation
{
    class SpaceshipSpaceship : Spaceship
    {
        public SpaceshipSpaceship(Simulator simulator)
            : base(simulator)
        {
            RotationMaximaleRad = 0.2f;
            BuyPrice = 50;
            SfxOut = "sfxPowerUpDoItYourselfOut";
            SfxIn = "sfxPowerUpDoItYourselfIn";
            ShowTrail = true;

            Friction = 0;
        }
    }
}
