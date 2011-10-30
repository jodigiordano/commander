namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PowerUpCollector : PowerUp
    {
        public SpaceshipCollector Collector { get; private set; }
        private PowerUpsBattleship HumanBattleship;


        public PowerUpCollector(Simulator simulator, PowerUpsBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.Collector;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "Collecteur";
            BuyPrice = 0;
            BuyTitle = "The collector (FREE!)";
            BuyDescription = "Collect minerals on the battlefield.";
            NeedInput = true;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Collector.Active; }
        }


        public override void Update()
        {
            Position = Collector.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            Collector.SteeringBehavior.NextMovement = delta;
        }


        public override void DoInputCanceled()
        {
            Collector.Active = false;
        }


        public override void Start()
        {
            Collector = new SpaceshipCollector(Simulator)
            {
                Position = HumanBattleship.Position,
                VisualPriority = VisualPriorities.Default.PlayerCursor,
                StartingObject = HumanBattleship
            };
            Collector.SteeringBehavior.ApplySafeBouncing();

            SfxIn = Collector.SfxIn;
            SfxOut = Collector.SfxOut;
        }
    }
}
