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
                if (PlanetarySystem[i].Priorite == Descriptor.CelestialBodyToProtect)
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


        private string nomRepresentation(Size taille, string nomBase)
        {
            return nomBase + ((taille == Size.Small) ? 1 : (taille == Size.Normal) ? 2 : 3).ToString();
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
                       (int) descriptor.Size,
                       descriptor.Speed,
                       Simulator.Scene.Particles.Get(descriptor.ParticulesEffect),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );

                }

                // Sun
                else if (descriptor.Image == null && descriptor.ParticulesEffect != null)
                {
                    c = new CelestialBody
                    (
                       Simulator,
                       descriptor.Name,
                       descriptor.Position,
                       descriptor.Offset,
                       (int) descriptor.Size,
                       descriptor.Speed,
                       Simulator.Scene.Particles.Get(descriptor.ParticulesEffect),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );

                }

                // Gaz body
                else if (descriptor.Image != null && descriptor.ParticulesEffect != null)
                {
                    c = new CelestialBody
                    (
                        Simulator,
                        descriptor.Name,
                        descriptor.Position,
                        descriptor.Offset,
                        (int) descriptor.Size,
                        descriptor.Speed,
                        Simulator.Scene.Particles.Get(descriptor.ParticulesEffect),
                        descriptor.StartingPosition, NextCelestialBodyVisualPriority -= 0.001f,
                        descriptor.InBackground,
                       descriptor.Rotation
                    );

                    c.Representation = new Image(nomRepresentation(descriptor.Size, descriptor.Image))
                    {
                        VisualPriority = c.ParticulesRepresentation.VisualPriority + 0.001f
                    };

                    if (descriptor.InBackground)
                        c.Representation.Color.A = 60;
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
                       (int) descriptor.Size,
                       descriptor.Speed,
                       new Image(nomRepresentation(descriptor.Size, descriptor.Image)),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );
                }

                // Asteroids belt
                else
                {
                    List<Image> representations = new List<Image>();

                    for (int j = 0; j < descriptor.Images.Count; j++)
                        representations.Add(new Image(descriptor.Images[j]));

                    c = new AsteroidBelt
                    (
                        Simulator,
                        descriptor.Name,
                        descriptor.Position,
                        (int) descriptor.Size,
                        descriptor.Speed,
                        representations,
                        descriptor.StartingPosition
                    );
                }

                c.Priorite = descriptor.PathPriority;
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
                    if (PlanetarySystem[i].Priorite == descriptor.PathPriority)
                    {
                        celestialBody = PlanetarySystem[i];
                        break;
                    }

                if (descriptor.HasGravitationalTurret)
                {
                    t = Simulator.TurretsFactory.Create(TurretType.Gravitational);

                    t.CanSell = false;
                    t.CanUpdate = false;
                    t.Level = 1;
                    t.BackActiveThisTickOverride = true;
                    t.Visible = false;
                    t.CelestialBody = celestialBody;
                    t.Position = celestialBody.Position;

                    celestialBody.Turrets.Add(t);
                    Turrets.Add(t);
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
