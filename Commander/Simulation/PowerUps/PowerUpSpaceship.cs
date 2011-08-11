namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpSpaceship : PowerUp
    {
        public Spaceship SpaceshipSpaceship { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpSpaceship(Simulator simulator, HumanBattleship humanBattleship)
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
            Position = SpaceshipSpaceship.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            SpaceshipSpaceship.NextMovement = delta;
        }


        public override void Start()
        {
            SpaceshipSpaceship = new SpaceshipSpaceship(Simulation)
            {
                ShootingFrequency = 100,
                BulletAttackPoints = 50,
                Position = Position,
                VisualPriority = VisualPriorities.Default.PlayerCursor,
                StartingObject = HumanBattleship,
                ApplyAutomaticBehavior = false
            };
            SpaceshipSpaceship.ApplySafeBouncing();

            Audio.PlaySfx(SpaceshipSpaceship.SfxIn);
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !SpaceshipSpaceship.Active; }
        }


        public override void Stop()
        {
            Audio.PlaySfx(SpaceshipSpaceship.SfxOut);
        }
    }
}
