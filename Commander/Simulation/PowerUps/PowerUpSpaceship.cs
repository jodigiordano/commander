namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PowerUpSpaceship : PowerUp
    {
        public Spaceship Spaceship { get; private set; }

        private PowerUpsBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpSpaceship(Simulator simulator, PowerUpsBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.Spaceship;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "Vaisseau";
            BuyPrice = 50;
            ActiveTime = 10000;
            BuyTitle = "The spaceship (" + BuyPrice + "M$)";
            BuyDescription = "Control a crazy spaceship for " + (int) ActiveTime / 1000 + " sec.";
            NeedInput = true;
            Position = Vector3.Zero;
        }


        public override void Update()
        {
            Position = Spaceship.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            Spaceship.SteeringBehavior.NextMovement = delta;
        }


        public override void Start()
        {
            Spaceship = new Spaceship(Simulator)
            {
                Position = Position,
                VisualPriority = VisualPriorities.Default.PlayerCursor,
                StartingObject = HumanBattleship
            };
            Spaceship.SteeringBehavior.ApplySafeBouncing();
            Spaceship.Weapon = new BasicBulletWeapon(Simulator, Spaceship, 100, 50);

            SfxIn = Spaceship.SfxIn;
            SfxOut = Spaceship.SfxOut;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Spaceship.Active; }
        }
    }
}
