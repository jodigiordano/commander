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
        {
            ZoneImpactDestruction = new Cercle(new Vector3(), 300);
            PointsAttaque = 50000;
        }


        public PowerUpType Type
        {
            get { return PowerUpType.FinalSolution; }
        }


        public String BuyImage
        {
            get { return "Destruction"; }
        }


        public int BuyPrice
        {
            get { return 50; }
        }
    }
}
