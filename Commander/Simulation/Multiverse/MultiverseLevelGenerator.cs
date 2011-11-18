namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class MultiverseLevelGenerator
    {
        public static List<Size> PossibleSizes = new List<Size>()
        {
            Size.Small,
            Size.Normal,
            Size.Big
        };


        public static List<float> PossibleRotationTimes = new List<float>()
        {
            float.MaxValue,
            1200000,
            900000,
            600000,
            480000,
            360000,
            240000,
            120000,
            60000,
            30000,
            10000,
        };


        public static List<string> PossibleCelestialBodiesAssets = new List<string>()
        {
            "planete1",
            "planete2",
            "planete3",
            "planete4",
            "planete5",
            "planete6",
            "planete7",
            "planete8",
            "planete9",
            "planete10",
            "planete11",
            "planete12",
            "battleship1",
            "battleship2",
            "battleship3",
            "battleship4",
            "battleship5",
            "stationSpatiale1",
            "stationSpatiale2",
            "vaisseauAlien1"
        };


        private static Dictionary<Size, PhysicalRectangle> Limits = new Dictionary<Size, PhysicalRectangle>()
        {
            { Size.Small,  new PhysicalRectangle(-640 + 123, -370 + 112, 1098, 560) },
            { Size.Normal, new PhysicalRectangle(-640 + 147, -370 + 136, 1050, 512) },
            { Size.Big,  new PhysicalRectangle(-640 + 179, -370 + 168,  986, 448) }
        };


        private static Dictionary<Size, Vector3> MinimalDistances = new Dictionary<Size, Vector3>()
        {
            { Size.Small,  new Vector3( 64,  92, 120) },
            { Size.Normal, new Vector3( 92, 120, 144) },
            { Size.Big,  new Vector3(120, 144, 176) }
        };


        private static Dictionary<EnemyType, string> EnemiesImages = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Asteroid, "Asteroid" },
            { EnemyType.Centaur, "Centaur" },
            { EnemyType.Plutoid, "Plutoid" },
            { EnemyType.Comet, "Comet" },
            { EnemyType.Trojan, "Trojan" },
            { EnemyType.Meteoroid, "Meteoroid" }
        };


        public static CelestialBody GeneratePlanetCB(Simulator simulator, double visualPriority)
        {
            var cb = GeneratePlanetCBDescriptor().GenerateSimulatorObject(visualPriority);
            cb.Simulator = simulator;

            return cb;
        }


        public static CelestialBody GeneratePinkHoleCB(Simulator simulator, double visualPriority)
        {
            var cb = GeneratePinkHoleCBDescriptor().GenerateSimulatorObject(visualPriority);

            cb.Simulator = simulator;

            return cb;
        }


        public static InfiniteWave GenerateInfiniteWave(Simulator simulator, List<EnemyType> enemies)
        {
            return new InfiniteWave(simulator, new InfiniteWavesDescriptor()
            {
                StartingDifficulty = 10,
                DifficultyIncrement = 0,
                MineralsPerWave = 0,
                MinMaxEnemiesPerWave = new Vector2(10, 30),
                Enemies = new List<EnemyType>(enemies),
                FirstOneStartNow = true,
                Upfront = true,
                NbWaves = 10
            });
        }


        private static CelestialBodyDescriptor GeneratePlanetCBDescriptor()
        {
            var d = new PlanetCBDescriptor()
            {
                Name = "CB" + Main.Random.Next(0, int.MaxValue),
                Invincible = false,
                CanSelect = true,
                PathPriority = -1,
                Image = "planete" + Main.Random.Next(1, 13).ToString()
            };

            var rp = Limits[d.Size];

            int size = Main.Random.Next(0, 3);
            d.Size = (size == 0) ? Size.Small : (size == 1) ? Size.Normal : Size.Big;
            d.Speed = float.MaxValue;
            d.Path = Vector3.Zero;
            d.Position = Vector3.Zero;
            d.Rotation = 0;

            return d;
        }


        private static CelestialBodyDescriptor GeneratePinkHoleCBDescriptor()
        {
            return new PinkHoleCBDescriptor()
            {
                Name = "Planete" + Main.Random.Next(0, int.MaxValue),
                Invincible = false,
                CanSelect = true,
                PathPriority = -1,
                Speed = float.MaxValue,
                Path = Vector3.Zero,
                Position = Vector3.Zero,
                Rotation = 0,
                Size = Size.Normal
            };
        }
    }
}
