namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class PowerUpsFactory
    {
        public Dictionary<PowerUpType, PowerUp> Availables;
        public Dictionary<PowerUpType, PowerUp> All;
        public HumanBattleship HumanBattleship;
        private Simulator Simulator;


        public PowerUpsFactory(Simulator simulator)
        {
            Simulator = simulator;

            Availables = new Dictionary<PowerUpType, PowerUp>(PowerUpTypeComparer.Default);
            All = new Dictionary<PowerUpType, PowerUp>(PowerUpTypeComparer.Default);

            All.Add(PowerUpType.Collector, Create(PowerUpType.Collector));
            All.Add(PowerUpType.FinalSolution, Create(PowerUpType.FinalSolution));
            All.Add(PowerUpType.Spaceship, Create(PowerUpType.Spaceship));
            All.Add(PowerUpType.TheResistance, Create(PowerUpType.TheResistance));
            All.Add(PowerUpType.DeadlyShootingStars, Create(PowerUpType.DeadlyShootingStars));
            All.Add(PowerUpType.RailGun, Create(PowerUpType.RailGun));
            All.Add(PowerUpType.AutomaticCollector, Create(PowerUpType.AutomaticCollector));
            All.Add(PowerUpType.DarkSide, Create(PowerUpType.DarkSide));
            All.Add(PowerUpType.Miner, Create(PowerUpType.Miner));
            All.Add(PowerUpType.Pulse, Create(PowerUpType.Pulse));
            All.Add(PowerUpType.Shield, Create(PowerUpType.Shield));
            All.Add(PowerUpType.Sniper, Create(PowerUpType.Sniper));
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
