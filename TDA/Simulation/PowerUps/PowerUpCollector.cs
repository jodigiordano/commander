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
            get { return TerminatedOverride || !Spaceship.Active; }
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


        public override void DoInputCanceled()
        {
            Spaceship.Active = false;
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
