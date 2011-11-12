namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.Xml.Serialization;


    [XmlRoot(ElementName = "Wave")]
    public class WaveDescriptor
    {
        public double StartingTime;
        public List<EnemyType> Enemies;
        public int SpeedLevel;
        public int LivesLevel;
        public int CashValue;
        public int Quantity;

        public Distance Distance;
        public double Delay;
        public int ApplyDelayEvery;
        public int SwitchEvery;


        public WaveDescriptor()
        {
            StartingTime = 0;
            Enemies = new List<EnemyType>();
            SpeedLevel = 1;
            LivesLevel = 1;
            CashValue = 1;
            Quantity = 1;
            Distance = Distance.Joined;
            Delay = 0;
            ApplyDelayEvery = -1;
            SwitchEvery = -1;
        }


        internal List<EnemyDescriptor> GetEnemiesToCreate(Simulator simulator)
        {
            var results = new List<EnemyDescriptor>();
            int typeIndex = 0;
            double lastTimeCreated = 0;

            if (Enemies.Count == 0)
                return results;

            for (int i = 0; i < Quantity; i++)
            {
                // switch enemy (with SwitchEvery)
                if (SwitchEvery != 0 && (i + 1) % SwitchEvery == 0)
                    typeIndex = (typeIndex + 1) % Enemies.Count;

                var type = Enemies[typeIndex];

                // compute delay (with Delay and ApplyDelayEvery)
                double delay = (ApplyDelayEvery != 0 && (i + 1) % ApplyDelayEvery == 0) ? Delay : 0;

                // compute frequency (with Distance)
                double frequency =
                    simulator.TweakingController.EnemiesFactory.GetSize(type) + (int) Distance /
                    simulator.TweakingController.EnemiesFactory.GetSpeed(type, SpeedLevel) * (1000f / 60f);

                // create enemy
                var e = EnemyDescriptor.Pool.Get();
                e.Type = type;
                e.CashValue = this.CashValue;
                e.LivesLevel = this.LivesLevel;
                e.SpeedLevel = this.SpeedLevel;
                e.StartingTime = lastTimeCreated + frequency + delay;
                e.StartingPosition = 0;

                results.Add(e);

                lastTimeCreated = e.StartingTime;
            }


            return results;
        }


        internal double GetAverageLife(Simulator simulator)
        {
            double average = 0;

            foreach (var e in Enemies)
                average += simulator.TweakingController.EnemiesFactory.GetLives(e, LivesLevel);

            return Enemies.Count == 0 ? average : average / Enemies.Count;
        }
    }
}
