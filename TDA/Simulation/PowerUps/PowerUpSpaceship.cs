namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpSpaceship : PowerUp
    {
        public Spaceship Spaceship { get; private set; }

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
            Position = Spaceship.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            Spaceship.NextInput = delta;
        }


        public override void Start()
        {
            Spaceship = new SpaceshipSpaceship(Simulation)
            {
                ShootingFrequency = 100,
                BulletHitPoints = 50,
                Position = Position,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0),
                ActiveTime = ActiveTime,
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Spaceship.Active; }
        }
    }
}
