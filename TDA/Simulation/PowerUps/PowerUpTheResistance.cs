namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpTheResistance : PowerUp
    {
        public TheResistance Spaceship { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpTheResistance(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.TheResistance;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "TheResistance";
            BuyPrice = 250;
            ActiveTime = 20000;
            BuyTitle = "The resistance (" + BuyPrice + "M$)";
            BuyDescription = "Call reinforcement for " + (int) ActiveTime / 1000 + " sec. with a 3-spaceships army.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Spaceship.Active; }
        }


        public override void Start()
        {
            Spaceship = new TheResistance(Simulation)
            {
                ActiveTime = ActiveTime,
                StartingObject = HumanBattleship
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }


        public override void Stop()
        {
            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxOut);
        }
    }
}
