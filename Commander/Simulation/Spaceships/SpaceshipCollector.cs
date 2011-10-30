namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class SpaceshipCollector : Spaceship
    {
        public SpaceshipCollector(Simulator simulator)
            : base(simulator)
        {
            Image = new Image("Collecteur")
            {
                SizeX = 4
            };

            BuyPrice = 0;
            Active = true;
            SfxOut = "sfxPowerUpCollecteurOut";
            SfxIn = "sfxPowerUpCollecteurIn";
            Active = true;
            ShowTrail = true;
            Weapon = new NoWeapon(simulator, this);
        }


        public override bool Active { get; set; }
    }
}
