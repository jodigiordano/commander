namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    [XmlRoot(ElementName = "World")]
    public class WorldDescriptor
    {
        public int Id;
        public string Name;
        public List<KeyValuePair<int, List<int>>> Levels;
        public List<KeyValuePair<int, string>> Warps;
        public int Layout;
        public List<int> UnlockedCondition;
        public string WarpBlockedMessage;
        public int LastLevelId;
        public string Music;


        public WorldDescriptor()
        {
            Id = -1;
            Name = "";
            Levels = new List<KeyValuePair<int, List<int>>>();
            Warps = new List<KeyValuePair<int, string>>();
            Layout = -1;
            UnlockedCondition = new List<int>();
            WarpBlockedMessage = "";
            LastLevelId = -1;
            Music = "";
        }


        public bool ContainsLevel(int id)
        {
            foreach (var l in Levels)
                if (l.Key == id)
                    return true;

            return false;
        }
    }


    [XmlRoot(ElementName = "Level")]
    public class LevelDescriptor
    {
        public InfosDescriptor Infos;
        public ObjectiveDescriptor Objective;

        [XmlArrayItem("CelestialBody")]
        public List<CelestialBodyDescriptor> PlanetarySystem;

        [ContentSerializer(Optional = true)]
        public DescriptorInfiniteWaves InfiniteWaves;

        [XmlArrayItem("Wave")]
        public List<WaveDescriptor> Waves;

        public PlayerDescriptor Player;
        public List<TurretType> AvailableTurrets;
        public List<PowerUpType> AvailablePowerUps;

        [ContentSerializer(Optional = true)]
        public MineralsDescriptor Minerals;

        [ContentSerializer(Optional = true)]
        public List<string> HelpTexts;


        public LevelDescriptor()
        {
            Infos = new InfosDescriptor();
            Objective = new ObjectiveDescriptor();

            PlanetarySystem = new List<CelestialBodyDescriptor>();
            Waves = new List<WaveDescriptor>();
            InfiniteWaves = null;
            Player = new PlayerDescriptor();

            AvailableTurrets = new List<TurretType>();
            AvailablePowerUps = new List<PowerUpType>();

            Minerals = new MineralsDescriptor();

            HelpTexts = new List<string>();
        }


        public void AddCelestialBody(Size size, Vector3 position, string name, string image, int speed, int pathPriority)
        {
            CelestialBodyDescriptor d = new CelestialBodyDescriptor();
            d.Name = name;
            d.Invincible = true;
            d.Position = position;
            d.StartingPosition = 0;
            d.PathPriority = pathPriority;
            d.Size = size;
            d.Speed = speed;
            d.Image = image;

            PlanetarySystem.Add(d);
        }


        public void AddPinkHole(Vector3 position, string name, int speed, int priority)
        {
            PlanetarySystem.Add(CreatePinkHole(position, name, speed, priority));
        }


        public CelestialBodyDescriptor CreatePinkHole(Vector3 position, string name, int speed, int priority)
        {
            return new CelestialBodyDescriptor()
            {
                Name = name,
                Invincible = true,
                Position = position,
                StartingPosition = 0,
                PathPriority = priority,
                Size = Size.Small,
                Speed = speed,
                ParticulesEffect = "trouRose"
            };
        }


        public void AddAsteroidBelt()
        {
            var c = new CelestialBodyDescriptor()
            {
                Name = "Asteroid belt",
                Path = new Vector3(700, -400, 0),
                Speed = 2560000,
                StartingPosition = 40,
                Size = Size.Small,
                Images = new List<string>() { "Asteroid" },
                PathPriority = int.MinValue + 1
            };
            
            PlanetarySystem.Add(c);
        }


        public double ParTime
        {
            get
            {
                if (InfiniteWaves != null)
                    return 0;

                double parTime = 0;

                foreach (var wave in Waves)
                    parTime += wave.StartingTime;

                return parTime;
            }
        }


        public int GetPotentialScore()
        {
            int maxCash = 0;

            foreach (var wave in Waves)
                maxCash += wave.Quantity * wave.CashValue;

            int maxLives = Player.Lives + Minerals.LifePacks;

            return (int) ((GetTotalLives(maxLives) + GetTotalCash(maxCash) + GetTotalTime(ParTime * 0.75)) * 0.60);
        }


        public int GetStarsCount(int score)
        {
            if (score == 0)
                return 0;

            int best = GetPotentialScore();

            return (score >= best) ? 3 :
                   (score >= best * 0.75) ? 2 : 1;
        }


        public int GetFinalScore(int lives, int cash, int time)
        {
            return GetTotalLives(lives) + GetTotalCash(cash) + GetTotalTime(time);
        }


        public int GetTotalCash(int cash)
        {
            return cash;
        }


        public int GetTotalLives(int lives)
        {
            return lives * 50;
        }


        public int GetTotalTime(double time)
        {
            return (int) (time / 100);
        }


        public static CelestialBodyDescriptor GetAsteroidBelt(List<CelestialBodyDescriptor> celestialBodies)
        {
            foreach (var c in celestialBodies)
                if (c.Name == "Asteroid belt" || c.Images.Count != 0 || c.PathPriority == int.MinValue + 1)
                    return c;

            return null;
        }
    }


    [XmlRoot(ElementName = "Infos")]
    public class InfosDescriptor
    {
        public int Id;
        public string Mission;
        public string Difficulty;
        public string Background;


        public InfosDescriptor()
        {
            Id = -1;
            Mission = "1-1";
            Difficulty = "Easy";
            Background = "background4";
        }
    }


    [XmlRoot(ElementName = "Objective")]
    public class ObjectiveDescriptor
    {
        public int CelestialBodyToProtect;


        public ObjectiveDescriptor()
        {
            CelestialBodyToProtect = -1;
        }
    }


    [XmlRoot(ElementName = "InfiniteWaves")]
    public class DescriptorInfiniteWaves
    {
        public List<EnemyType> Enemies;
        public int StartingDifficulty;
        public int DifficultyIncrement;
        public Vector2 MinMaxEnemiesPerWave;
        public int MineralsPerWave;
        public bool FirstOneStartNow;

        [ContentSerializer(Optional = true)]
        public bool Upfront;

        [ContentSerializer(Optional = true)]
        public int NbWaves;

        public DescriptorInfiniteWaves()
        {
            Enemies = new List<EnemyType>();
            StartingDifficulty = 1;
            DifficultyIncrement = 0;
            MinMaxEnemiesPerWave = new Vector2(1, 1);
            FirstOneStartNow = false;
            Upfront = false;
            NbWaves = 1;
        }
    }


    [XmlRoot(ElementName = "CelestialBody")]
    public class CelestialBodyDescriptor
    {
        [ContentSerializer(Optional = true)]
        public string Name;

        [ContentSerializer(Optional = true)]
        public List<string> Images;

        [ContentSerializer(Optional = true)]
        public string Image;

        public bool HasGravitationalTurret;

        [ContentSerializer(Optional = true)]
        public List<TurretDescriptor> StartingTurrets;

        public Size Size;

        [ContentSerializer(Optional = true)]
        public string ParticulesEffect;

        public float Speed;
        public int PathPriority;
        public Vector3 Path;
        public Vector3 Position;

        [ContentSerializer(Optional = true)]
        public float Rotation;

        [ContentSerializer(Optional = true)]
        public bool InBackground;

        public bool CanSelect;
        public bool Invincible;
        public int StartingPosition;

        [ContentSerializer(Optional = true)]
        public bool FollowPath;

        [ContentSerializer(Optional = true)]
        public bool HasMoons;

        [ContentSerializer(Optional = true)]
        public bool StraightLine;

        

        public CelestialBodyDescriptor()
        {
            Name = "CorpsCeleste";
            Image = null;
            ParticulesEffect = null;
            HasGravitationalTurret = false;
            StartingTurrets = new List<TurretDescriptor>();
            Speed = 0;
            PathPriority = -1;
            Path = Vector3.Zero;
            Position = Vector3.Zero;
            CanSelect = true;
            Invincible = false;
            Size = Size.Normal;
            StartingPosition = 0;
            Images = new List<string>();
            InBackground = false;
            FollowPath = false;
            HasMoons = true;
            StraightLine = false;
            Rotation = 0;
        }


        public void AddTurret(TurretType type, int level, Vector3 position, bool visible, bool canSell, bool canUpgrade, bool canSelect)
        {
            StartingTurrets.Add(CreateTurret(type, level, position, visible, canSell, canUpgrade, canSelect));
        }


        public void AddTurret(TurretType type, int level, Vector3 position)
        {
            AddTurret(type, level, position, true, true, true, true);
        }


        public void AddTurret(TurretType type, int level, Vector3 position, bool visible, bool canSelect)
        {
            AddTurret(type, level, position, visible, true, true, canSelect);
        }


        public TurretDescriptor CreateTurret(TurretType type, int level, Vector3 position, bool visible, bool canSell, bool canUpgrade, bool canSelect)
        {
            return new TurretDescriptor()
            {
                Type = type,
                Level = level,
                Position = position,
                Visible = visible,
                CanSell = canSell,
                CanUpgrade = canUpgrade,
                CanSelect = canSelect
            };
        }
    }


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


    [XmlRoot(ElementName = "Player")]
    public class PlayerDescriptor
    {
        public int Money;
        public int Lives;
        public Vector3 StartingPosition;
        public double BulletDamage;


        public PlayerDescriptor()
        {
            Money = 0;
            Lives = 0;
            StartingPosition = Vector3.Zero;
            BulletDamage = -1;
        }
    }


    [XmlRoot(ElementName = "Enemy")]
    public class EnemyDescriptor
    {
        [XmlIgnore]
        public static Pool<EnemyDescriptor> Pool = new Pool<EnemyDescriptor>();

        public EnemyType Type;
        public int SpeedLevel;
        public int LivesLevel;
        public int CashValue;
        public double StartingTime;

        [XmlIgnore]
        public double StartingPosition;
    }


    [XmlRoot(ElementName = "Turret")]
    public class TurretDescriptor
    {
        public TurretType Type;
        public int Level;
        public Vector3 Position;
        public bool Visible;
        public bool CanSell;
        public bool CanUpgrade;
        public bool CanSelect;


        public TurretDescriptor()
        {
            Type = TurretType.Basic;
            Level = 1;
            Position = Vector3.Zero;
            Visible = true;
            CanSell = true;
            CanUpgrade = true;
            CanSelect = true;
        }
    }


    [XmlRoot(ElementName = "Minerals")]
    public class MineralsDescriptor
    {
        public int Cash;
        public int LifePacks;


        public MineralsDescriptor()
        {
            Cash = 0;
            LifePacks = 0;
        }
    }
}
