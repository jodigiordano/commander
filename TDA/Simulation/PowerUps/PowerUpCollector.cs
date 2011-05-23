namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpCollector : PowerUp
    {
        public SpaceshipCollector Spaceship { get; private set; }
        private HumanBattleship HumanBattleship;


        public PowerUpCollector(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.Collector;
            BuyImage = "Collecteur";
            BuyPrice = 0;
            BuyTitle = "The collector";
            BuyDescription = "Collect minerals on the battlefield.";
            NeedInput = true;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return !Spaceship.Active; }
        }


        public override void Start()
        {
            Spaceship = new SpaceshipCollector(Simulation)
            {
                Position = HumanBattleship.Position,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0),
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }
    }
}
