namespace EphemereGames.Commander
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpTheResistance : PowerUp
    {
        public TheResistance TheResistance { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpTheResistance(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.TheResistance;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "TheResistance";
            BuyPrice = 250;
            ActiveTime = 20000;
            BuyTitle = "The resistance (" + BuyPrice + "M$)";
            BuyDescription = "Call reinforcement for " + (int) ActiveTime / 1000 + " sec. with a 3-spaceships army.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !TheResistance.Active; }
        }


        public override void Start()
        {
            TheResistance = new TheResistance(Simulation)
            {
                ActiveTime = ActiveTime,
                StartingObject = HumanBattleship
            };

            Audio.PlaySfx(@"Partie", TheResistance.SfxIn);
        }


        public override void Stop()
        {
            Audio.PlaySfx(@"Partie", TheResistance.SfxOut);
        }
    }
}