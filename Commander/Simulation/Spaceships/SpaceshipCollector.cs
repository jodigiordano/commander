namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class SpaceshipCollector : SpaceshipSpaceship
    {
        public SpaceshipCollector(Simulator simulator)
            : base(simulator)
        {
            Image = new Image("Collecteur")
            {
                SizeX = 4
            };

            RotationMaximaleRad = 0.2f;
            BuyPrice = 0;
            ShootingFrequency = double.NaN;
            Active = true;
            SfxOut = "sfxPowerUpCollecteurOut";
            SfxIn = "sfxPowerUpCollecteurIn";
            Active = true;
            ShowTrail = true;
        }


        public override bool Active { get; set; }
    }
}
