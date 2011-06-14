namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class PowerUpCollector : PowerUp
    {
        public SpaceshipCollector Collector { get; private set; }
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
            get { return TerminatedOverride || !Collector.Active; }
        }


        public override void Update()
        {
            Position = Collector.Position;
        }


        public override void DoInputMoved(Vector3 position) { }


        public override void DoInputMovedDelta(Vector3 delta)
        {
            Collector.NextInput = delta;
        }


        public override void DoInputCanceled()
        {
            Collector.Active = false;
        }


        public override void Start()
        {
            Collector = new SpaceshipCollector(Simulation)
            {
                Position = HumanBattleship.Position,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], 0),
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", Collector.SfxIn);
        }


        public override void Stop()
        {
            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", Collector.SfxOut);
        }
    }
}
