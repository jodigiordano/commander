namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class SpaceshipMiner : Spaceship
    {
        public SimPlayer Owner;


        public SpaceshipMiner(Simulator simulator)
            : base(simulator)
        {
            Image = new Image("Resistance3")
            {
                SizeX = 4
            };

            BuyPrice = 0;
            Active = true;
            SfxOut = "sfxMinerOut";
            SfxIn = "sfxMinerIn";
            Active = true;
            ShowTrail = true;
            Weapon = new MineWeapon(Simulator, this);
        }


        public override bool Active { get; set; }
    }
}
