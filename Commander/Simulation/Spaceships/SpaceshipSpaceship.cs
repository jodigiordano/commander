namespace EphemereGames.Commander.Simulation
{
    class SpaceshipSpaceship : Spaceship
    {
        public SpaceshipSpaceship(Simulator simulator)
            : base(simulator)
        {
            BuyPrice = 50;
            SfxOut = "sfxPowerUpDoItYourselfOut";
            SfxIn = "sfxPowerUpDoItYourselfIn";
            ShowTrail = true;

            Weapon = new BasicBulletWeapon(Simulator, this, 100, 1);
        }


        public double ShootingFrequency
        {
            set { Weapon.ShootingFrequency = value; }
        }


        public float BulletAttackPoints
        {
            set { Weapon.AttackPoints = value; }
        }
    }
}
