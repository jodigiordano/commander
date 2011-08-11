namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpMiner : PowerUp
    {
        public SpaceshipMiner Miner { get; private set; }
        private HumanBattleship HumanBattleship;
        private bool Firing;


        public PowerUpMiner(Simulator simulator, HumanBattleship humanBattleship)
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
            Miner.NextMovement = delta;
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

                Audio.PlaySfx(@"sfxMineGround");

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
                ApplyAutomaticBehavior = false,
                Owner = Owner
            };
            Miner.ApplySafeBouncing();

            Audio.PlaySfx(Miner.SfxIn);

            Firing = false;
        }


        public override void Stop()
        {
            Audio.PlaySfx(Miner.SfxOut);

            Firing = false;

            Miner.Active = false;
        }
    }
}
