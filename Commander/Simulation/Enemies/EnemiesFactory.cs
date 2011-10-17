namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class EnemiesFactory
    {
        public List<Enemy> All;
        private Dictionary<EnemyType, Pool<Enemy>> EnemiesPools;
        private Simulator Simulator;


        public static readonly Dictionary<EnemyType, string> ImagesEnemies = new Dictionary<EnemyType, string>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, "Asteroid" },
            { EnemyType.Comet, "Comet" },
            { EnemyType.Plutoid, "Plutoid" },
            { EnemyType.Centaur, "Centaur" },
            { EnemyType.Trojan, "Trojan" },
            { EnemyType.Meteoroid, "Meteoroid" },
            { EnemyType.Damacloid, "Damacloid" },
            { EnemyType.Swarm, "Swarm" },
            { EnemyType.Vulcanoid, "Vulcanoid" },
            { EnemyType.Alien1, "Alien1" },
            { EnemyType.Alien2, "Alien2" },
            { EnemyType.Alien3, "Alien3" },
            { EnemyType.Alien4, "Alien4" }
        };


        public static readonly Dictionary<string, EnemyType> EnemiesImages = new Dictionary<string, EnemyType>()
        {
            { "Asteroid", EnemyType.Asteroid },
            { "Comet", EnemyType.Comet },
            { "Plutoid", EnemyType.Plutoid },
            { "Centaur", EnemyType.Centaur },
            { "Trojan", EnemyType.Trojan },
            { "Meteoroid", EnemyType.Meteoroid },
            { "Damacloid", EnemyType.Damacloid },
            { "Swarm", EnemyType.Swarm },
            { "Vulcanoid", EnemyType.Vulcanoid },
            { "Alien1", EnemyType.Alien1 },
            { "Alien2", EnemyType.Alien2 },
            { "Alien3", EnemyType.Alien3 },
            { "Alien4", EnemyType.Alien4 }
        };


        private static readonly Dictionary<EnemyType, Color> ColorsEnemies = new Dictionary<EnemyType, Color>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, new Color(255, 178, 12) },
            { EnemyType.Comet, new Color(255, 142, 161) },
            { EnemyType.Plutoid, new Color(7, 143, 255) },
            { EnemyType.Centaur, new Color(92, 198, 11) },
            { EnemyType.Trojan, new Color(255, 66, 217) },
            { EnemyType.Meteoroid, new Color(239, 0, 0) },
            { EnemyType.Damacloid, new Color(255, 0, 170) },
            { EnemyType.Swarm, new Color(255, 0, 170) },
            { EnemyType.Vulcanoid, new Color(0, 76, 255) },
            { EnemyType.Alien1, Colors.Default.AlienBright },
            { EnemyType.Alien2, Colors.Default.AlienBright },
            { EnemyType.Alien3, Colors.Default.AlienBright },
            { EnemyType.Alien4, Colors.Default.AlienBright }
        };


        private static readonly Dictionary<EnemyType, string> SfxHit = new Dictionary<EnemyType, string>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, "AsteroidHit" },
            { EnemyType.Comet, "CometHit" },
            { EnemyType.Plutoid, "PlutoidHit" },
            { EnemyType.Centaur, "CentaurHit" },
            { EnemyType.Trojan, "TrojanHit" },
            { EnemyType.Meteoroid, "MeteoroidHit" },
            { EnemyType.Damacloid, "DamacloidHit" },
            { EnemyType.Swarm, "SwarmHit" },
            { EnemyType.Vulcanoid, "VulcanoidHit" },
            { EnemyType.Alien1, "AsteroidHit" },
            { EnemyType.Alien2, "AsteroidHit" },
            { EnemyType.Alien3, "AsteroidHit" },
            { EnemyType.Alien4, "AsteroidHit" }
        };


        private static readonly Dictionary<EnemyType, string> SfxDie = new Dictionary<EnemyType, string>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, "AsteroidDestroyed" },
            { EnemyType.Comet, "CometDestroyed" },
            { EnemyType.Plutoid, "PlutoidDestroyed" },
            { EnemyType.Centaur, "CentaurDestroyed" },
            { EnemyType.Trojan, "TrojanDestroyed" },
            { EnemyType.Meteoroid, "MeteoroidDestroyed" },
            { EnemyType.Damacloid, "DamacloidDestroyed" },
            { EnemyType.Swarm, "SwarmDestroyed" },
            { EnemyType.Vulcanoid, "VulcanoidDestroyed" },
            { EnemyType.Alien1, "AsteroidDestroyed" },
            { EnemyType.Alien2, "AsteroidDestroyed" },
            { EnemyType.Alien3, "AsteroidDestroyed" },
            { EnemyType.Alien4, "AsteroidDestroyed" }
        };


        private static readonly Dictionary<EnemyType, string> SfxCreated = new Dictionary<EnemyType, string>(EnemyTypeComparer.Default)
        {
            { EnemyType.Asteroid, "AsteroidOut" },
            { EnemyType.Comet, "CometOut" },
            { EnemyType.Plutoid, "PlutoidOut" },
            { EnemyType.Centaur, "CentaurOut" },
            { EnemyType.Trojan, "TrojanOut" },
            { EnemyType.Meteoroid, "MeteoroidOut" },
            { EnemyType.Damacloid, "DamacloidOut" },
            { EnemyType.Swarm, "SwarmOut" },
            { EnemyType.Vulcanoid, "VulcanoidOut" },
            { EnemyType.Alien1, "AsteroidOut" },
            { EnemyType.Alien2, "AsteroidOut" },
            { EnemyType.Alien3, "AsteroidOut" },
            { EnemyType.Alien4, "AsteroidOut" }
        };


        public EnemiesFactory(Simulator simulator)
        {
            Simulator = simulator;

            EnemiesPools = new Dictionary<EnemyType, Pool<Enemy>>(EnemyTypeComparer.Default);
            All = new List<Enemy>();
        }


        public void Initialize()
        {
            EnemiesPools.Clear();
            EnemiesPools.Add(EnemyType.Asteroid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Centaur, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Comet, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Meteoroid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Plutoid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Trojan, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Damacloid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Vulcanoid, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Swarm, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Alien1, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Alien2, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Alien3, new Pool<Enemy>());
            EnemiesPools.Add(EnemyType.Alien4, new Pool<Enemy>());

            All.Clear();
            All.Add(Get(EnemyType.Asteroid, 1, 1, 1));
            All.Add(Get(EnemyType.Centaur, 1, 1, 1));
            All.Add(Get(EnemyType.Comet, 1, 1, 1));
            All.Add(Get(EnemyType.Meteoroid, 1, 1, 1));
            All.Add(Get(EnemyType.Plutoid, 1, 1, 1));
            All.Add(Get(EnemyType.Trojan, 1, 1, 1));
            All.Add(Get(EnemyType.Damacloid, 1, 1, 1));
            All.Add(Get(EnemyType.Vulcanoid, 1, 1, 1));
            All.Add(Get(EnemyType.Swarm, 1, 1, 1));
            All.Add(Get(EnemyType.Alien1, 1, 1, 1));
            All.Add(Get(EnemyType.Alien2, 1, 1, 1));
            All.Add(Get(EnemyType.Alien3, 1, 1, 1));
            All.Add(Get(EnemyType.Alien4, 1, 1, 1));
        }


        public Enemy Get(EnemyType type, int speedLevel, int livesLevel, int value)
        {
            Enemy e = EnemiesPools[type].Get();

            e.Name = GetTexture(type);
            e.Type = type;
            e.Simulator = Simulator;
            e.Speed = GetSpeed(type, speedLevel);
            e.LifePoints = e.StartingLifePoints = GetLives(type, livesLevel); ;
            e.CashValue = value;
            e.PointsValue = livesLevel;
            e.Color = ColorsEnemies[type];
            e.Level = (speedLevel + livesLevel) / 2;
            e.FadeInTime = (type == EnemyType.Swarm) ? 250 : 1000;
            e.ImpulseSpeed = (type == EnemyType.Swarm) ? 1f : 0;
            e.ImpulseTime = (type == EnemyType.Swarm) ? 250 : 0;
            e.FollowPath = type == EnemyType.Alien1 || type == EnemyType.Alien2 || type == EnemyType.Alien3 || type == EnemyType.Alien4;

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


        public float GetLives(EnemyType type, int livesLevel)
        {
            float lives = 0;

            switch (type)
            {
                // Asteroids
                case EnemyType.Asteroid:   lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Plutoid:    lives = 10 + 15 * (livesLevel - 1); break;
                case EnemyType.Centaur:    lives = 20 + 20 * (livesLevel - 1); break;
                case EnemyType.Comet:      lives = 5 + 2 * (livesLevel - 1); break;
                case EnemyType.Meteoroid:  lives = 1 + 5 * (livesLevel - 1); break;
                case EnemyType.Trojan:     lives = 15 + 10 * (livesLevel - 1); break;
                case EnemyType.Damacloid:  lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Vulcanoid:  lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Swarm:      lives = 1 + 1 * (livesLevel - 1); break;

                // Aliens
                case EnemyType.Alien1: lives = 5 + 10 * (livesLevel - 1); break;
                case EnemyType.Alien2: lives = 10 + 15 * (livesLevel - 1); break;
                case EnemyType.Alien3: lives = 20 + 20 * (livesLevel - 1); break;
                case EnemyType.Alien4: lives = 5 + 2 * (livesLevel - 1); break;
            }

            return lives;
        }


        public float GetSpeed(EnemyType type, int speedLevel)
        {
            float speed = 0;

            switch (type)
            {
                // Asteroids
                case EnemyType.Asteroid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Centaur: speed = 0.8f + 0 * (speedLevel - 1); break;
                case EnemyType.Comet: speed = 4f + 0 * (speedLevel - 1); break;
                case EnemyType.Meteoroid: speed = 1.5f + 0 * (speedLevel - 1); break;
                case EnemyType.Plutoid: speed = 2 + 0 * (speedLevel - 1); break;
                case EnemyType.Trojan: speed = 2.5f + 0 * (speedLevel - 1); break;
                case EnemyType.Damacloid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Vulcanoid: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Swarm: speed = 1 + 0 * (speedLevel - 1); break;

                // Aliens
                case EnemyType.Alien1: speed = 1 + 0 * (speedLevel - 1); break;
                case EnemyType.Alien2: speed = 0.8f + 0 * (speedLevel - 1); break;
                case EnemyType.Alien3: speed = 4f + 0 * (speedLevel - 1); break;
                case EnemyType.Alien4: speed = 1.5f + 0 * (speedLevel - 1); break;
            }

            return speed;
        }


        public int GetSize(EnemyType type)
        {
            int size = 0;

            switch (type)
            {
                // Asteroids
                case EnemyType.Asteroid: size = 35; break;
                case EnemyType.Centaur: size = 30; break;
                case EnemyType.Comet: size = 10; break;
                case EnemyType.Meteoroid: size = 35; break;
                case EnemyType.Plutoid: size = 35; break;
                case EnemyType.Trojan: size = 15; break;
                case EnemyType.Damacloid: size = 24; break;
                case EnemyType.Vulcanoid: size = 20; break;
                case EnemyType.Swarm: size = 10; break;

                // Aliens
                case EnemyType.Alien1: size = 35; break;
                case EnemyType.Alien2: size = 30; break;
                case EnemyType.Alien3: size = 10; break;
                case EnemyType.Alien4: size = 35; break;
            }

            return size;
        }


        public double GetVisualPriority(EnemyType type, float pourcPath)
        {
            double vp = pourcPath > 0.8 ? VisualPriorities.Default.EnemyNearCelestialBodyToProtect : VisualPriorities.Default.Enemy;
            
            vp -= pourcPath / 100;
                
            switch (type)
            {
                // Asteroids
                case EnemyType.Asteroid: vp -= 0.00001; break;
                case EnemyType.Centaur: vp -= 0.00002; break;
                case EnemyType.Comet: vp -= 0.00003; break;
                case EnemyType.Meteoroid: vp -= 0.00004; break;
                case EnemyType.Plutoid: vp -= 0.00005; break;
                case EnemyType.Trojan: vp -= 0.00006; break;
                case EnemyType.Damacloid: vp -= 0.00007; break;
                case EnemyType.Vulcanoid: vp -= 0.00008; break;
                case EnemyType.Swarm: vp -= 0.00009; break;

                // Aliens
                case EnemyType.Alien1: vp -= 0.00010; break;
                case EnemyType.Alien2: vp -= 0.00011; break;
                case EnemyType.Alien3: vp -= 0.00012; break;
                case EnemyType.Alien4: vp -= 0.00013; break;
            }

            return vp;
        }


        public string GetSfxHit(EnemyType type)
        {
            return SfxHit[type];
        }


        public string GetSfxDie(EnemyType type)
        {
            return SfxDie[type];
        }


        public string GetSfxCreated(EnemyType type)
        {
            return SfxCreated[type];
        }


        public static string GetTexture(EnemyType type)
        {
            return type.ToString("g");
        }


        public static List<EnemyType> ToEnemyTypeList(List<string> enemiesNames)
        {
            var result = new List<EnemyType>();

            foreach (var e in enemiesNames)
                result.Add(EnemiesImages[e]);

            return result;
        }
    }
}
