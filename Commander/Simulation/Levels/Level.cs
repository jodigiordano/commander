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
        public int Id;
        public string Mission;
        public string Difficulty;
        public double ParTime;
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
        public List<string> HelpTexts;
        
        // minerals
        public int Cash;
        public int LifePacks;

        private Simulator Simulator;
        private float NextCelestialBodyVisualPriority = Preferences.PrioriteSimulationCorpsCeleste;


        public Level(Simulator simulator, LevelDescriptor descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;
        }


        public void Initialize()
        {
            Id = Descriptor.Infos.Id;
            Mission = Descriptor.Infos.Mission;
            Difficulty = Descriptor.Infos.Difficulty;
            ParTime = Descriptor.ParTime;

            Background = new Image(Descriptor.Infos.Background, Vector3.Zero);
            Background.VisualPriority = Preferences.PrioriteFondEcran;

            InitializePlanetarySystem();
            InitializeTurrets();
            InitializeWaves();
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

            CommonStash = new CommonStash();
            CommonStash.Lives = Descriptor.Player.Lives;
            CommonStash.Cash = Descriptor.Player.Money;

            Cash = Descriptor.Minerals.Cash;
            LifePacks = Descriptor.Minerals.LifePacks;

            HelpTexts = Descriptor.HelpTexts;
        }


        public void SyncDescriptor()
        {
            // Sync planetary system
            Descriptor.PlanetarySystem = Simulator.PlanetarySystemController.GenerateDescriptor();

            // Sync turrets
            Descriptor.AvailableTurrets.Clear();

            foreach (var type in Simulator.TurretsFactory.Availables.Keys)
                Descriptor.AvailableTurrets.Add(type);

            // Sync power-ups
            Descriptor.AvailablePowerUps.Clear();

            foreach (var type in Simulator.PowerUpsFactory.Availables.Keys)
                Descriptor.AvailablePowerUps.Add(type);

            // Sync info
            Descriptor.Infos.Background = Background.TextureName;
            Descriptor.Infos.Difficulty = Difficulty;
            Descriptor.Infos.Mission = Mission;

            // Sync player
            Descriptor.Player.Lives = CommonStash.Lives;
            Descriptor.Player.Money = CommonStash.Cash;

            // Sync planet to protect
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
        }


        public int NbStars(int score)
        {
            return Descriptor.NbStars(score);
        }


        private void InitializePlanetarySystem()
        {
            PlanetarySystem = new List<CelestialBody>();

            foreach (var descriptor in Descriptor.PlanetarySystem)
            {
                CelestialBody c;

                // Pink Hole
                if (descriptor.Image == null &&
                    descriptor.ParticulesEffect != null &&
                    descriptor.ParticulesEffect == "trouRose")
                {
                    c = new PinkHole
                    (
                       Simulator,
                       descriptor.Name,
                       descriptor.Path,
                       descriptor.Position,
                       descriptor.Size,
                       descriptor.Speed == 0 ? float.MaxValue : descriptor.Speed,
                       Simulator.Scene.Particles.Get(descriptor.ParticulesEffect),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f
                    );

                }

                // Normal
                else if (descriptor.Image != null)
                {
                    c = new CelestialBody
                    (
                       Simulator,
                       descriptor.Name,
                       descriptor.Path,
                       descriptor.Position,
                       descriptor.Rotation,
                       descriptor.Size,
                       descriptor.Speed == 0 ? float.MaxValue : descriptor.Speed,
                       descriptor.Image,
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f
                    );
                }

                // Asteroids belt
                else
                {
                    c = new AsteroidBelt
                    (
                        Simulator,
                        descriptor.Name,
                        descriptor.Path,
                        descriptor.Size,
                        descriptor.Speed == 0 ? float.MaxValue : descriptor.Speed,
                        descriptor.Images,
                        descriptor.StartingPosition
                    );
                }

                c.PathPriority = descriptor.PathPriority;
                c.Selectionnable = descriptor.CanSelect;
                c.Invincible = descriptor.Invincible;

                if (Simulator.EditorMode && Simulator.EditorState == EditorState.Editing)
                    c.AliveOverride = true;

                PlanetarySystem.Add(c);
            }
        }


        private void InitializeTurrets()
        {
            Turrets = new List<Turret>();

            foreach (var descriptor in Descriptor.PlanetarySystem)
            {
                CelestialBody celestialBody = null;
                Turret t = null;

                for (int i = 0; i < PlanetarySystem.Count; i++)
                    if (PlanetarySystem[i].PathPriority == descriptor.PathPriority)
                    {
                        celestialBody = PlanetarySystem[i];
                        break;
                    }

                if (descriptor.HasGravitationalTurret)
                {
                    celestialBody.AddToStartingPath();

                    Turrets.Add(celestialBody.StartingPathTurret);
                }

                foreach (var turretDesc in descriptor.StartingTurrets)
                {
                    t = Simulator.TurretsFactory.Create(turretDesc.Type);

                    t.CanSell = turretDesc.CanSell;
                    t.CanUpdate = turretDesc.CanUpgrade;
                    t.Level = turretDesc.Level;
                    t.BackActiveThisTickOverride = true;
                    t.Visible = turretDesc.Visible;
                    t.CelestialBody = celestialBody;
                    t.RelativePosition = Vector3.Multiply(turretDesc.Position, 8);
                    t.Position = celestialBody.Position;

                    celestialBody.Turrets.Add(t);
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


        private CelestialBodyDescriptor GetCelestialBodyWithLowestPathPriority(List<CelestialBodyDescriptor> celestialBodies)
        {
            if (celestialBodies.Count == 0)
                return null;

            CelestialBodyDescriptor lowest = celestialBodies[0];

            foreach (var c in celestialBodies)
                if (lowest.PathPriority == int.MinValue || c.PathPriority < lowest.PathPriority)
                    lowest = c;

            return lowest;
        }
    }
}
