namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    public enum PowerUpType
    {
        None = -1,
        Collector = 0,
        FinalSolution = 1,
        Spaceship = 2,
        TheResistance = 3
    };


    class PowerUpsFactory
    {
        public Dictionary<PowerUpType, PowerUp> Availables;
        private Simulation Simulation;

        public PowerUpsFactory( Simulation simulation )
        {
            Simulation = simulation;

            Availables = new Dictionary<PowerUpType, PowerUp>();
        }


        public PowerUp Create(PowerUpType type)
        {
            PowerUp t = null;

            switch (type)
            {
                case PowerUpType.Collector:         t = new VaisseauCollecteur(Simulation);     break;
                case PowerUpType.FinalSolution:     t = new PowerUpLastSolution(Simulation);    break;
                case PowerUpType.Spaceship:         t = new VaisseauDoItYourself(Simulation);   break;
                case PowerUpType.TheResistance:     t = new TheResistance(Simulation);          break;
                default:                            t = new VaisseauCollecteur(Simulation);     break;
            }

            return t;
        }
    }
}
