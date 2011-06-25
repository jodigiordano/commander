namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Level
    {
        public int Id;
        public string Mission;
        public string Difficulty;
        public double ParTime;
        public Image Background;

        public List<CelestialBody> PlanetarySystem;
        public List<Turret> Turrets;

        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;

        public CommonStash CommonStash;

        public Dictionary<TurretType, Turret> AvailableTurrets;
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps;
        public CelestialBody CelestialBodyToProtect;

        public int MineralsValue;
        public Vector3 MineralsPercentages;
        public int LifePacks;

        public List<string> HelpTexts;

        private Simulator Simulator;
        private LevelDescriptor Descriptor;

        private float NextCelestialBodyVisualPriority = Preferences.PrioriteSimulationCorpsCeleste;


        public Level(Simulator simulator, LevelDescriptor descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;

            Id = descriptor.Id;
            Mission = descriptor.Mission;
            Difficulty = descriptor.Difficulty;
            ParTime = descriptor.ParTime;

            Background = new Image(descriptor.Background, Vector3.Zero);
            Background.VisualPriority = Preferences.PrioriteFondEcran;

            InitializePlanetarySystem();
            InitializeTurrets();
            InitializeWaves();
            InitializeAvailableTurrets();
            InitializeAvailablePowerUps();

            for (int i = 0; i < PlanetarySystem.Count; i++)
                if (PlanetarySystem[i].PathPriority == Descriptor.CelestialBodyToProtect)
                {
                    CelestialBodyToProtect = PlanetarySystem[i];
                    break;
                }

            if (CelestialBodyToProtect != null)
                CelestialBodyToProtect.LifePoints = descriptor.Player.Lives;

            CommonStash = new CommonStash();
            CommonStash.Lives = descriptor.Player.Lives;
            CommonStash.Cash = descriptor.Player.Money;

            MineralsValue = descriptor.MineralsValue;
            MineralsPercentages = descriptor.MineralsPercentages;
            LifePacks = descriptor.LifePacks;

            HelpTexts = descriptor.HelpTexts;
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
                       descriptor.Position,
                       descriptor.Offset,
                       descriptor.Size,
                       descriptor.Speed,
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
                       descriptor.Position,
                       descriptor.Offset,
                       descriptor.Size,
                       descriptor.Speed,
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
                        descriptor.Position,
                        descriptor.Size,
                        descriptor.Speed,
                        descriptor.Images,
                        descriptor.StartingPosition
                    );
                }

                c.PathPriority = descriptor.PathPriority;
                c.Selectionnable = descriptor.CanSelect;
                c.Invincible = descriptor.Invincible;

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
                //InfiniteWaves = Descriptor.InfiniteWaves.Upfront ?
                //    new UpfrontInfiniteWave(Simulation, Descriptor.InfiniteWaves, Descriptor.InfiniteWaves.NbWaves) :
                //    new InfiniteWave(Simulation, Descriptor.InfiniteWaves);
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
    }
}
