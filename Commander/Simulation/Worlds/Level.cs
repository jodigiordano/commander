namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Level
    {
        // descriptor
        public LevelDescriptor Descriptor;

        // infos
        public Image Background;

        // objective
        public CelestialBody CelestialBodyToProtect;
        
        // other
        public List<CelestialBody> PlanetarySystem;
        public List<Turret> Turrets;
        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;
        public CommonStash CommonStash;
        public Dictionary<TurretType, Turret> AvailableTurrets;
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps;
        public string HelpText;
        
        // minerals
        public int Minerals;
        public int LifePacks;

        // spaceship
        public double BulletDamage;

        private Simulator Simulator;
        private double NextCelestialBodyVisualPriority = VisualPriorities.Default.CelestialBody;


        public Level(Simulator simulator, LevelDescriptor descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;
        }


        public void Initialize()
        {
            Background = new Image(Descriptor.Infos.Background, Vector3.Zero);
            Background.VisualPriority = Preferences.PrioriteFondEcran;

            InitializePlanetarySystem();
            InitializeTurrets();
            InitializeWaves();
            InitializeSpaceship();
            InitializeAvailableTurrets();
            InitializeAvailablePowerUps();

            for (int i = 0; i < PlanetarySystem.Count; i++)
                if (PlanetarySystem[i].PathPriority == Descriptor.Objective.CelestialBodyToProtect)
                {
                    CelestialBodyToProtect = PlanetarySystem[i];
                    break;
                }

            if (CelestialBodyToProtect != null)
                CelestialBodyToProtect.LifePoints = Descriptor.Player.Lives;

            CommonStash = new CommonStash()
            {
                Lives = Descriptor.Player.Lives,
                Cash = Descriptor.Player.Money,
                StartingPosition = Descriptor.Player.StartingPosition
            };

            Minerals = Descriptor.Minerals.Cash;
            LifePacks = Descriptor.Minerals.LifePacks;

            HelpText = Descriptor.HelpText;
        }


        public void SyncDescriptor()
        {
            // Sync planetary system
            Descriptor.PlanetarySystem = Simulator.PlanetarySystemController.GenerateDescriptor();

            // Sync turrets
            Descriptor.AvailableTurrets.Clear();

            foreach (var type in AvailableTurrets.Keys)
                Descriptor.AvailableTurrets.Add(type);

            // Sync power-ups
            Descriptor.AvailablePowerUps.Clear();

            foreach (var type in AvailablePowerUps.Keys)
                Descriptor.AvailablePowerUps.Add(type);

            // Sync info
            Descriptor.Infos.Background = Background.TextureName;

            // Sync player
            Descriptor.Player.Lives = CommonStash.Lives;
            Descriptor.Player.Money = CommonStash.Cash;
            Descriptor.Minerals.Cash = Minerals;
            Descriptor.Minerals.LifePacks = LifePacks;
            Descriptor.Player.StartingPosition = CommonStash.StartingPosition;

            // Sync planet toLocal protect
            if (CelestialBodyToProtect != null)
                Descriptor.Objective.CelestialBodyToProtect = CelestialBodyToProtect.PathPriority;

            // Sync Waves
            Descriptor.Waves.Clear();

            foreach (var w in Waves)
                Descriptor.Waves.Add(w.Descriptor);

            // Sync Asteroid Belt
            var asteroidBelt = LevelDescriptor.GetAsteroidBelt(Descriptor.PlanetarySystem);

            Bag<EnemyType> enemies = new Bag<EnemyType>();

            foreach (var w in Descriptor.Waves)
                foreach (var e in w.Enemies)
                    enemies.Add(e);

            if (asteroidBelt == null)
            {
                Descriptor.AddAsteroidBelt();
                asteroidBelt = LevelDescriptor.GetAsteroidBelt(Descriptor.PlanetarySystem);
            }

            asteroidBelt.Images = new List<string>();

            foreach (var e in enemies)
                asteroidBelt.Images.Add(e.ToString("g"));

            // Bullet damage
            Descriptor.Player.BulletDamage = BulletDamage;
        }


        public int GetStarsCount(int score)
        {
            return Descriptor.GetStarsCount(score);
        }


        public int GetTotalCash(int cash)
        {
            return Descriptor.GetTotalCash(cash);
        }


        public int GetTotalLives(int lives)
        {
            return Descriptor.GetTotalLives(lives);
        }


        public int GetTotalTime(double time)
        {
            return Descriptor.GetTotalTime(time);
        }


        public int GetTotalScore(double time, int cash, int lives)
        {
            return Descriptor.GetFinalScore(lives, cash, (int) time);
        }


        public int GetPotentialScore()
        {
            return Descriptor.GetPotentialScore();
        }


        private void InitializePlanetarySystem()
        {
            PlanetarySystem = new List<CelestialBody>();

            foreach (var descriptor in Descriptor.PlanetarySystem)
            {
                var c = descriptor.GenerateSimulatorObject(NextCelestialBodyVisualPriority -= 0.001);

                if (Simulator.EditorEditingMode)
                    c.AliveOverride = true;

                c.Simulator = Simulator;
                c.Descriptor = descriptor;

                if (c is AsteroidBelt)
                {
                    var ab = (AsteroidBelt) c;
                    var boundaries = Simulator.Data.Battlefield.Inner;

                    ab.SteeringBehavior.BasePosition = new Vector3(boundaries.Center.X, boundaries.Center.Y, 0);
                    ab.SteeringBehavior.Path = new Vector3(boundaries.Width / 2, boundaries.Height / 2, 0);
                    ab.SteeringBehavior.Speed = EditorLevelGenerator.PossibleRotationTimes[Main.Random.Next(2, 7)];
                }

                c.Initialize();

                PlanetarySystem.Add(c);
            }
        }


        private void InitializeTurrets()
        {
            Turrets = new List<Turret>();

            foreach (var celestialBody in PlanetarySystem)
            {
                if (!(celestialBody.Descriptor is CustomizableBodyDescriptor))
                    continue;

                if (((CustomizableBodyDescriptor) celestialBody.Descriptor).HasGravitationalTurret)
                {
                    celestialBody.TurretsController.AddToStartingPath(false);

                    Turrets.Add(celestialBody.TurretsController.StartingPathTurret);
                }

                if (!(celestialBody.Descriptor is PlanetCBDescriptor))
                    continue;

                foreach (var turretDesc in ((PlanetCBDescriptor) celestialBody.Descriptor).StartingTurrets)
                {
                    var t = Simulator.TurretsFactory.Create(turretDesc.Type);

                    t.CanSell = turretDesc.CanSell;
                    t.CanUpdate = turretDesc.CanUpgrade;
                    t.Level = turretDesc.Level;
                    t.BackActiveThisTickOverride = true;
                    t.Visible = turretDesc.Visible;
                    t.CelestialBody = celestialBody;
                    t.RelativePosition = Vector3.Multiply(turretDesc.Position, 8);
                    t.Position = celestialBody.Position;
                    t.CanSelect = turretDesc.CanSelect;

                    celestialBody.TurretsController.Turrets.Add(t);
                    Turrets.Add(t);
                }
            }
        }


        private void InitializeWaves()
        {
            Waves = new LinkedList<Wave>();

            if (Descriptor.InfiniteWaves != null)
            {
                InfiniteWaves = new InfiniteWave(Simulator, Descriptor.InfiniteWaves);
                Waves.AddLast(InfiniteWaves.GetNextWave());
            }

            else
                for (int i = 0; i < Descriptor.Waves.Count; i++)
                    Waves.AddLast(new Wave(Simulator, Descriptor.Waves[i]));
        }


        private void InitializeSpaceship()
        {
            // Bullet damage
            if (Descriptor.Player.BulletDamage >= 0)
            {
                BulletDamage = Descriptor.Player.BulletDamage;
                return;
            }

            double averageLife = 0;

            if (InfiniteWaves != null)
            {
                BulletDamage = InfiniteWaves.GetAverageLife();
            }

            else
            {
                foreach (var w in Descriptor.Waves)
                    averageLife += w.GetAverageLife(Simulator);

                BulletDamage = Descriptor.Waves.Count == 0 ? averageLife : averageLife / Descriptor.Waves.Count;
            }

            BulletDamage /= 15;
        }


        private void InitializeAvailableTurrets()
        {
            AvailableTurrets = new Dictionary<TurretType, Turret>(TurretTypeComparer.Default);

            foreach (var type in Descriptor.AvailableTurrets)
                AvailableTurrets.Add(type, Simulator.TurretsFactory.Create(type));
        }


        private void InitializeAvailablePowerUps()
        {
            AvailablePowerUps = new Dictionary<PowerUpType, PowerUp>(PowerUpTypeComparer.Default);

            foreach (var type in Descriptor.AvailablePowerUps)
                AvailablePowerUps.Add(type, Simulator.PowerUpsFactory.Create(type));
        }
    }
}
