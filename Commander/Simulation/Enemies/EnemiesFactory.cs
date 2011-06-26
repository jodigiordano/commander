namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class EnemiesFactory
    {
        public List<Enemy> AvailableEnemies { get; private set; }
        private Dictionary<EnemyType, Pool<Enemy>> EnemiesPools;
        private Simulator Simulator;


        public Dictionary<EnemyType, string> ImagesEnemies = new Dictionary<EnemyType, string>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, @"Asteroid" },
            { EnemyType.Comet, @"Comet" },
            { EnemyType.Plutoid, @"Plutoid" },
            { EnemyType.Centaur, @"Centaur" },
            { EnemyType.Trojan, @"Trojan" },
            { EnemyType.Meteoroid, @"Meteoroid" },
            { EnemyType.Apohele, @"Apohele" },
            { EnemyType.Damacloid, @"Damacloid" },
            { EnemyType.Swarm, @"Swarm" },
            { EnemyType.Vulcanoid, @"Vulcanoid" }
        };


        private Dictionary<EnemyType, Color> ColorsEnemies = new Dictionary<EnemyType, Color>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, new Color(255, 178, 12) },
            { EnemyType.Comet, new Color(255, 142, 161) },
            { EnemyType.Plutoid, new Color(7, 143, 255) },
            { EnemyType.Centaur, new Color(92, 198, 11) },
            { EnemyType.Trojan, new Color(255, 66, 217) },
            { EnemyType.Meteoroid, new Color(239, 0, 0) },
            { EnemyType.Apohele, new Color(255, 255, 255) },
            { EnemyType.Damacloid, new Color(255, 0, 170) },
            { EnemyType.Swarm, new Color(255, 0, 170) },
            { EnemyType.Vulcanoid, new Color(0, 76, 255) }
        };


        public EnemiesFactory(Simulator simulator)
        {
            Simulator = simulator;

            EnemiesPools = new Dictionary<EnemyType, Pool<Enemy>>(EnemyTypeComparer.Default);
            EnemiesPools.Add(EnemyType.Asteroid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Centaur, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Comet, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Meteoroid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Plutoid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Trojan, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Apohele, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Damacloid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Vulcanoid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Swarm, new Pool<Enemy>());

            AvailableEnemies = new List<Enemy>();
            AvailableEnemies.Add(Get(EnemyType.Asteroid, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Centaur, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Comet, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Meteoroid, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Plutoid, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Trojan, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Apohele, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Damacloid, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Vulcanoid, 1, 1, 1));
            AvailableEnemies.Add(Get(EnemyType.Swarm, 1, 1, 1));
        }


        public Enemy Get(EnemyType type, int speedLevel, int livesLevel, int value)
        {
            Enemy e = EnemiesPools[type].Get();

            e.Name = GetTexture(type);
            e.Type = type;
            e.Simulation = Simulator;
            e.Speed = GetSpeed(type, speedLevel);
            e.LifePoints = e.StartingLifePoints = GetLives(type, livesLevel); ;
            e.CashValue = value;
            e.PointsValue = livesLevel;
            //e.Id = Enemy.NextID;
            e.Color = ColorsEnemies[type];
            e.Level = (speedLevel + livesLevel) / 2;
            e.FadeInTime = (type == EnemyType.Swarm) ? 250 : 1000;
            e.ImpulseSpeed = (type == EnemyType.Swarm) ? 1f : 0;
            e.ImpulseTime = (type == EnemyType.Swarm) ? 250 : 0;

            if (type == EnemyType.Swarm)
            {
                Vector3 direction = new Vector3(
                    Main.Random.Next(-100, 100),
                    Main.Random.Next(-100, 100),
                    0
                );

                direction.Normalize();

                e.ImpulseDirection = direction;
            }

            return e;
        }


        public void Return(Enemy enemy)
        {
            EnemiesPools[enemy.Type].Return(enemy);
        }


        public static float GetLives(EnemyType type, int livesLevel)
        {
            float lives = 0;

            switch (type)
            {
                case EnemyType.Asteroid:   lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Centaur:    lives = 50 + 25 * (livesLevel - 1); break;
                case EnemyType.Comet:      lives = 5 + 2 * (livesLevel - 1); break;
                case EnemyType.Meteoroid:  lives = 1 + 5 * (livesLevel - 1); break;
                case EnemyType.Plutoid:    lives = 25 + 15 * (livesLevel - 1); break;
                case EnemyType.Trojan:     lives = 25 + 10 * (livesLevel - 1); break;
                case EnemyType.Apohele:    lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Damacloid:  lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Vulcanoid:  lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Swarm:      lives = 1 + 1 * (livesLevel - 1); break;
            }

            return lives;
        }


        public static float GetSpeed(EnemyType type, int speedLevel)
        {
            float speed = 0;

            switch (type)
            {
                case EnemyType.Asteroid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Centaur: speed = 0.8f + 0 * (speedLevel - 1); break;
                case EnemyType.Comet: speed = 4 + 0 * (speedLevel - 1); break;
                case EnemyType.Meteoroid: speed = 1.5f + 0 * (speedLevel - 1); break;
                case EnemyType.Plutoid: speed = 2 + 0 * (speedLevel - 1); break;
                case EnemyType.Trojan: speed = 2.5f + 0 * (speedLevel - 1); break;
                case EnemyType.Apohele: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Damacloid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Vulcanoid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Swarm: speed = 1 + 0 * (speedLevel - 1); break;

            }

            return speed;
        }


        public static int GetSize(EnemyType type)
        {
            int size = 0;

            switch (type)
            {
                case EnemyType.Asteroid: size = 35; break;
                case EnemyType.Centaur: size = 30; break;
                case EnemyType.Comet: size = 10; break;
                case EnemyType.Meteoroid: size = 35; break;
                case EnemyType.Plutoid: size = 35; break;
                case EnemyType.Trojan: size = 15; break;
                case EnemyType.Apohele: size = 20; break;
                case EnemyType.Damacloid: size = 20; break;
                case EnemyType.Vulcanoid: size = 20; break;
                case EnemyType.Swarm: size = 10; break;
            }

            return size;
        }


        public static float GetVisualPriority(EnemyType type, float pourcPath)
        {
            float vp = (pourcPath >= 0.95f) ? Preferences.PrioriteSimulationCorpsCeleste - 0.1f : Preferences.PrioriteSimulationEnnemi;

            switch (type)
            {
                case EnemyType.Asteroid: vp -= 0.00001f; break;
                case EnemyType.Centaur: vp -= 0.00002f; break;
                case EnemyType.Comet: vp -= 0.00003f; break;
                case EnemyType.Meteoroid: vp -= 0.00004f; break;
                case EnemyType.Plutoid: vp -= 0.00005f; break;
                case EnemyType.Trojan: vp -= 0.00006f; break;
                case EnemyType.Apohele: vp -= 0.00007f; break;
                case EnemyType.Damacloid: vp -= 0.00008f; break;
                case EnemyType.Vulcanoid: vp -= 0.00009f; break;
                case EnemyType.Swarm: vp -= 0.00010f; break;
            }

            return vp;
        }


        public static string GetTexture(EnemyType type)
        {
            return type.ToString("g");
        }
    }
}
