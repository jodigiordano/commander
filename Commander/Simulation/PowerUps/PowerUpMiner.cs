namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PowerUpMiner : PowerUp
    {
        public SpaceshipMiner Miner { get; private set; }
        private PowerUpsBattleship HumanBattleship;
        private bool Firing;


        public PowerUpMiner(Simulator simulator, PowerUpsBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.Miner;
            Category = PowerUpCategory.Spaceship;
            PayOnActivation = false;
            PayOnUse = true;
            BuyImage = "Resistance3";
            UsePrice = 50;
            BuyTitle = "The miner (" + UsePrice + "M$ per mine)";
            BuyDescription = "Controls a spaceship that drops mines on the battlefield.";
            NeedInput = true;
            Position = Vector3.Zero;
            Firing = false;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Miner.Active; }
        }


        public override void Update()
        {
            Position = Miner.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            Miner.SteeringBehavior.NextMovement = delta;
        }


        public override void DoInputPressed()
        {
            Firing = true;
        }


        public override void DoInputReleased()
        {
            if (Firing)
            {
                Miner.Weapon.FireOnceNow();

                Firing = false;
            }
        }


        public override void DoInputCanceled()
        {
            Miner.Active = false;
        }


        public override void Start()
        {
            Miner = new SpaceshipMiner(Simulation)
            {
                Position = HumanBattleship.Position,
                VisualPriority = VisualPriorities.Default.PlayerCursor,
                StartingObject = HumanBattleship,
                Owner = Owner
            };
            Miner.SteeringBehavior.ApplySafeBouncing();

            SfxIn = Miner.SfxIn;
            SfxOut = Miner.SfxOut;

            Firing = false;
        }


        public override void Stop()
        {
            Firing = false;

            Miner.Active = false;
        }
    }
}
