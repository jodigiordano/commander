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


        public PowerUpSpaceship(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;
            
            Type = PowerUpType.Spaceship;
            BuyImage = "Vaisseau";
            BuyPrice = 50;
            BuyTitle = "The spaceship";
            BuyDescription = "Control a crazy spaceship for 10 seconds.";
            NeedInput = true;
            Position = Vector3.Zero;
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
                ActiveTime = 5000,
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }


        public override bool Terminated
        {
            get { return !Spaceship.Active; }
        }
    }
}
