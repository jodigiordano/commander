namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


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
        private Simulator Simulator;


        public PowerUpsFactory(Simulator simulator)
        {
            Simulator = simulator;

            Availables = new Dictionary<PowerUpType, PowerUp>(PowerUpTypeComparer.Default);
        }


        public PowerUp Create(PowerUpType type)
        {
            PowerUp t = null;

            switch (type)
            {
                case PowerUpType.Collector:             t = new PowerUpCollector(Simulator, HumanBattleship);          break;
                case PowerUpType.FinalSolution:         t = new PowerUpLastSolution(Simulator);                        break;
                case PowerUpType.Spaceship:             t = new PowerUpSpaceship(Simulator, HumanBattleship);          break;
                case PowerUpType.TheResistance:         t = new PowerUpTheResistance(Simulator, HumanBattleship);      break;
                case PowerUpType.DeadlyShootingStars:   t = new PowerUpDeadlyShootingStars(Simulator);                 break;
                case PowerUpType.RailGun:               t = new PowerUpRailGun(Simulator, HumanBattleship);            break;
                case PowerUpType.AutomaticCollector:    t = new PowerUpAutomaticCollector(Simulator, HumanBattleship); break;
                case PowerUpType.DarkSide:              t = new PowerUpDarkSide(Simulator);                            break;
                case PowerUpType.Miner:                 t = new PowerUpMiner(Simulator, HumanBattleship);              break;
                case PowerUpType.Pulse:                 t = new PowerUpPulse(Simulator);                               break;
                case PowerUpType.Shield:                t = new PowerUpShield(Simulator);                              break;
                case PowerUpType.Sniper:                t = new PowerUpSniper(Simulator, HumanBattleship);             break;
                default:                                t = new PowerUpCollector(Simulator, HumanBattleship);          break;
            }

            return t;
        }
    }
}
