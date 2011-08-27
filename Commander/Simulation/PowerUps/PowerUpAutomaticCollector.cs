namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpAutomaticCollector : PowerUp
    {
        public SpaceshipAutomaticCollector AutomaticCollector { get; private set; }

        private PowerUpsBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpAutomaticCollector(Simulator simulator, PowerUpsBattleship humanBattleship)
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
                Speed = 8,
                StartingObject = HumanBattleship,
                VisualPriority = VisualPriorities.Default.PlayerCursor
            };
            AutomaticCollector.SteeringBehavior.ApplySafeBouncing();

            Audio.PlaySfx(AutomaticCollector.SfxIn);
        }


        public override void Stop()
        {
            Audio.PlaySfx(AutomaticCollector.SfxOut);
        }
    }
}
