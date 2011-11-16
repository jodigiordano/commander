namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class WaveGenerator
    {
        public static List<string> DistancesStrings = new List<string>()
        {
            Distance.Joined.ToString("g"),
            Distance.Near.ToString("g"),
            Distance.Normal.ToString("g"),
            Distance.Far.ToString("g")
        };


        private static List<Distance> Distances = new List<Distance>()
        {
            Distance.Joined,
            Distance.Near,
            Distance.Normal,
            Distance.Far
        };


        public static WaveDescriptor Generate(int difficulty, int qty, List<EnemyType> enemies)
        {
            var result = new WaveDescriptor()
            {
                LivesLevel = difficulty,
                SpeedLevel = difficulty,
                Quantity = qty,
                Enemies = enemies
            };

            if (enemies.Count == 0)
                return result;

            switch (Main.Random.Next(0, 3))
            {
                case 0: GenerateHomogene(result); break;
                case 1: GenerateDistinctsWhichFollow(result, qty, enemies.Count); break;
                case 2: GeneratePackedHomogene(result, qty); break;
            }

            return result;
        }


        private static void GenerateDistinctsWhichFollow(WaveDescriptor description, int qty, int enemiesCount)
        {
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
            description.SwitchEvery = qty / enemiesCount;
            description.Delay = Main.Random.Next(2000, 4000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }


        private static void GenerateHomogene(WaveDescriptor description)
        {
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
            description.SwitchEvery = -1;
            description.Delay = 0;
            description.ApplyDelayEvery = -1;
        }


        private static void GeneratePackedHomogene(WaveDescriptor description, int qty)
        {
            description.Distance = Distance.Near;
            description.SwitchEvery = qty / Main.Random.Next(5, 10);
            description.Delay = Main.Random.Next(4000, 8000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }
    }
}
