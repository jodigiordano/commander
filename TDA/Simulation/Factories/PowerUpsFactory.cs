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
        TheResistance = 3,
        DeadlyShootingStars = 4,
        Pulse = 5,
        Miner = 6,
        Shield = 7,
        Sniper = 8,
        AutomaticCollector = 9,
        DarkSide = 10,
        RailGun = 11
    };


    public enum PowerUpCategory
    {
        Spaceship,
        Turret,
        Other
    }


    class PowerUpsFactory
    {
        public Dictionary<PowerUpType, PowerUp> Availables;
        public HumanBattleship HumanBattleship;
        private Simulation Simulation;


        public PowerUpsFactory(Simulation simulation)
        {
            Simulation = simulation;

            Availables = new Dictionary<PowerUpType, PowerUp>();
        }


        public PowerUp Create(PowerUpType type)
        {
            PowerUp t = null;

            switch (type)
            {
                case PowerUpType.Collector:             t = new PowerUpCollector(Simulation, HumanBattleship);          break;
                case PowerUpType.FinalSolution:         t = new PowerUpLastSolution(Simulation);                        break;
                case PowerUpType.Spaceship:             t = new PowerUpSpaceship(Simulation, HumanBattleship);          break;
                case PowerUpType.TheResistance:         t = new PowerUpTheResistance(Simulation, HumanBattleship);      break;
                case PowerUpType.DeadlyShootingStars:   t = new PowerUpDeadlyShootingStars(Simulation);                 break;
                case PowerUpType.RailGun:               t = new PowerUpRailGun(Simulation, HumanBattleship);            break;
                case PowerUpType.AutomaticCollector:    t = new PowerUpAutomaticCollector(Simulation, HumanBattleship); break;
                case PowerUpType.DarkSide:              t = new PowerUpDarkSide(Simulation);                            break;
                case PowerUpType.Miner:                 t = new PowerUpMiner(Simulation, HumanBattleship);              break;
                case PowerUpType.Pulse:                 t = new PowerUpPulse(Simulation);                               break;
                case PowerUpType.Shield:                t = new PowerUpShield(Simulation);                              break;
                case PowerUpType.Sniper:                t = new PowerUpSniper(Simulation, HumanBattleship);             break;
                default:                                t = new PowerUpCollector(Simulation, HumanBattleship);          break;
            }

            return t;
        }
    }
}
