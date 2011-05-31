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


        public PowerUpTheResistance(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.TheResistance;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "TheResistance";
            BuyPrice = 250;
            BuyTitle = "The resistance";
            BuyDescription = "Call reinforcement with a 3-spaceships army.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return !Spaceship.Active; }
        }


        public override void Start()
        {
            Spaceship = new TheResistance(Simulation)
            {
                TempsActif = 20000,
                StartingObject = HumanBattleship
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }
    }
}
