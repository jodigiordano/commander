namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class PowerUpMiner : PowerUp
    {
        public SpaceshipMiner Miner { get; private set; }
        private HumanBattleship HumanBattleship;
        private bool Firing;


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
            Miner.NextInput = delta;
        }


        public override void DoInputPressed()
        {
            Firing = true;
        }


        public override void DoInputReleased()
        {
            if (Firing)
            {
                Miner.Fire();
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
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f,
                Bouncing = new Vector3(Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], 0),
                StartingObject = HumanBattleship,
                AutomaticMode = false
            };

            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", Miner.SfxIn);

            Firing = false;
        }


        public override void Stop()
        {
            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", Miner.SfxOut);

            Firing = false;
        }
    }
}
