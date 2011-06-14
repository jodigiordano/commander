namespace EphemereGames.Commander
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpSpaceship : PowerUp
    {
        public Spaceship SpaceshipSpaceship { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpSpaceship(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
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
            SpaceshipSpaceship.NextInput = delta;
        }


        public override void Start()
        {
            SpaceshipSpaceship = new SpaceshipSpaceship(Simulation)
            {
                ShootingFrequency = 100,
                BulletHitPoints = 50,
                Position = Position,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], 0),
                ActiveTime = ActiveTime,
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            Audio.PlaySfx(@"Partie", SpaceshipSpaceship.SfxIn);
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !SpaceshipSpaceship.Active; }
        }


        public override void Stop()
        {
            Audio.PlaySfx(@"Partie", SpaceshipSpaceship.SfxOut);
        }
    }
}
