namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpAutomaticCollector : PowerUp
    {
        public SpaceshipAutomaticCollector Spaceship { get; private set; }

        private HumanBattleship HumanBattleship;
        private float ActiveTime;


        public PowerUpAutomaticCollector(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.AutomaticCollector;
            Category = PowerUpCategory.Spaceship;
            BuyImage = "automaticCollector";
            BuyPrice = 250;
            ActiveTime = 20000;
            BuyTitle = "The automatic collector (" + BuyPrice + "M$)";
            BuyDescription = "Call an automatic collector for " + (int) ActiveTime / 1000 + " sec.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || !Spaceship.Active; }
        }


        public override void Start()
        {
            Spaceship = new SpaceshipAutomaticCollector(Simulation)
            {
                ActiveTime = ActiveTime,
                Speed = 8,
                StartingObject = HumanBattleship,
                VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
            };

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", Spaceship.SfxIn);
        }
    }
}
