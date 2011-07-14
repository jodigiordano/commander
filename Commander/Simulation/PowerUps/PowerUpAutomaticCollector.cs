namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpAutomaticCollector : PowerUp
    {
        public SpaceshipAutomaticCollector AutomaticCollector { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpAutomaticCollector(Simulator simulator, HumanBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.AutomaticCollector;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "automaticCollector";
            BuyPrice = 250;
            ActiveTime = 20000;
            BuyTitle = "The automatic collector (" + BuyPrice + "M$)";
            BuyDescription = "Call an automatic collector for " + (int) ActiveTime / 1000 + " sec.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !AutomaticCollector.Active; }
        }


        public override void Start()
        {
            AutomaticCollector = new SpaceshipAutomaticCollector(Simulation)
            {
                ActiveTime = ActiveTime,
                Speed = 8,
                Bouncing = new Vector3(Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], 0),
                StartingObject = HumanBattleship,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
            };

            Audio.PlaySfx(AutomaticCollector.SfxIn);
        }


        public override void Stop()
        {
            Audio.PlaySfx(AutomaticCollector.SfxOut);
        }
    }
}
