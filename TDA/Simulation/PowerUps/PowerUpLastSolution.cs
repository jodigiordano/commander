namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpLastSolution : PowerUp
    {
        public Cercle ZoneImpactDestruction { get; private set; }
        public int PointsAttaque { get; private set; }
        public Vector3 BuyPosition { get; set; }


        public PowerUpLastSolution(Simulation simulation)
            : base(simulation)
        {
            ZoneImpactDestruction = new Cercle(new Vector3(), 300);
            PointsAttaque = 50000;
            Type = PowerUpType.FinalSolution;
            BuyImage = "Destruction";
            BuyPrice = 500;
            BuyTitle = "The final solution";
            BuyDescription = "Create a massive explosion by destroying a planet.";
            NeedInput = true;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return false; }
        }


        public override void Start()
        {

        }
    }
}
