namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpMiner : PowerUp
    {
        public SpaceshipMiner Spaceship { get; private set; }
        private HumanBattleship HumanBattleship;


        public PowerUpMiner(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
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
            Spaceship = new SpaceshipMiner(Simulation)
            {
                Position = HumanBattleship.Position,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0),
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }


        public override void Stop()
        {
            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxOut);
        }
    }
}
