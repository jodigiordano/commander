namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Scenario
    {
        public int Id;
        public string Mission;
        public string Difficulty;
        public double ParTime;
        public Image Background;

        public List<CorpsCeleste> PlanetarySystem;
        public List<Turret> Turrets;

        public VaguesInfinies InfiniteWaves;
        public LinkedList<Wave> Waves;

        public CommonStash CommonStash;

        public Dictionary<TurretType, Turret> AvailableTurrets;
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps;
        public CorpsCeleste CelestialBodyToProtect;

        public int MineralsValue;
        public Vector3 MineralsPercentages;
        public int LifePacks;

        public List<string> HelpTexts;

        private Simulation Simulation;
        private ScenarioDescriptor Descriptor;

        private float NextCelestialBodyVisualPriority = Preferences.PrioriteSimulationCorpsCeleste;


        public Scenario(Simulation simulation, ScenarioDescriptor descriptor)
        {
            Simulation = simulation;
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

            CelestialBodyToProtect = PlanetarySystem.Find(c => c.Priorite == Descriptor.CelestialBodyToProtect);
            CelestialBodyToProtect.LifePoints = descriptor.Player.Lives;

            CommonStash = new CommonStash();
            CommonStash.Lives = descriptor.Player.Lives;
            CommonStash.Cash = descriptor.Player.Money;

            MineralsValue = descriptor.MineralsValue;
            MineralsPercentages = descriptor.MineralsPercentages;
            LifePacks = descriptor.LifePacks;

            HelpTexts = descriptor.HelpTexts;
        }


        private String nomRepresentation(Size taille, String nomBase)
        {
            return nomBase + ((taille == Size.Small) ? 1 : (taille == Size.Normal) ? 2 : 3).ToString();
        }


        public int NbStars(int score)
        {
            return Descriptor.NbStars(score);
        }


        private void InitializePlanetarySystem()
        {
            PlanetarySystem = new List<CorpsCeleste>();

            foreach (var descriptor in Descriptor.PlanetarySystem)
            {
                CorpsCeleste c;

                // Pink Hole
                if (descriptor.Image == null &&
                    descriptor.ParticulesEffect != null &&
                    descriptor.ParticulesEffect == "trouRose")
                {
                    c = new TrouRose
                    (
                       Simulation,
                       descriptor.Name,
                       descriptor.Position,
                       descriptor.Offset,
                       (int) descriptor.Size,
                       descriptor.Speed,
                       Simulation.Scene.Particules.recuperer(descriptor.ParticulesEffect),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );

                }

                // Sun
                else if (descriptor.Image == null && descriptor.ParticulesEffect != null)
                {
                    c = new CorpsCeleste
                    (
                       Simulation,
                       descriptor.Name,
                       descriptor.Position,
                       descriptor.Offset,
                       (int) descriptor.Size,
                       descriptor.Speed,
                       Simulation.Scene.Particules.recuperer(descriptor.ParticulesEffect),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );

                }

                // Gaz body
                else if (descriptor.Image != null && descriptor.ParticulesEffect != null)
                {
                    c = new CorpsCeleste
                    (
                        Simulation,
                        descriptor.Name,
                        descriptor.Position,
                        descriptor.Offset,
                        (int) descriptor.Size,
                        descriptor.Speed,
                        Simulation.Scene.Particules.recuperer(descriptor.ParticulesEffect),
                        descriptor.StartingPosition, NextCelestialBodyVisualPriority -= 0.001f,
                        descriptor.InBackground,
                       descriptor.Rotation
                    );
                    c.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(nomRepresentation(descriptor.Size, descriptor.Image)), Vector3.Zero);
                    c.Representation.Origine = c.Representation.Centre;
                    c.Representation.VisualPriority = c.ParticulesRepresentation.VisualPriority + 0.001f;

                    if (descriptor.InBackground)
                        c.Representation.Couleur.A = 60;
                }

                // Normal
                else if (descriptor.Image != null)
                {
                    c = new CorpsCeleste
                    (
                       Simulation,
                       descriptor.Name,
                       descriptor.Position,
                       descriptor.Offset,
                       (int) descriptor.Size,
                       descriptor.Speed,
                       new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(nomRepresentation(descriptor.Size, descriptor.Image)), Vector3.Zero),
                       descriptor.StartingPosition,
                       NextCelestialBodyVisualPriority -= 0.001f,
                       descriptor.InBackground,
                       descriptor.Rotation
                    );
                }

                // Asteroids belt
                else
                {
                    List<IVisible> representations = new List<IVisible>();

                    for (int j = 0; j < descriptor.Images.Count; j++)
                    {
                        IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(descriptor.Images[j]), Vector3.Zero);
                        iv.Origine = iv.Centre;
                        representations.Add(iv);
                    }

                    c = new AsteroidBelt
                    (
                        Simulation,
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
                CorpsCeleste celestialBody = PlanetarySystem.Find(c => c.Priorite == descriptor.PathPriority);
                Turret t;

                if (descriptor.HasGravitationalTurret)
                {
                    t = Simulation.TurretsFactory.Create(TurretType.Gravitational);

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
                    t = Simulation.TurretsFactory.Create(turretDesc.Type);

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
                InfiniteWaves = new VaguesInfinies(Simulation, Descriptor.InfiniteWaves);
                Waves.AddLast(InfiniteWaves.getProchaineVague());
            }

            else
                for (int i = 0; i < Descriptor.Waves.Count; i++)
                    Waves.AddLast(new Wave(Simulation, Descriptor.Waves[i]));
        }


        private void InitializeAvailableTurrets()
        {
            AvailableTurrets = new Dictionary<TurretType, Turret>();

            foreach (var type in Descriptor.AvailableTurrets)
                AvailableTurrets.Add(type, Simulation.TurretsFactory.Create(type));
        }


        private void InitializeAvailablePowerUps()
        {
            AvailablePowerUps = new Dictionary<PowerUpType, PowerUp>();

            foreach (var type in Descriptor.AvailablePowerUps)
                AvailablePowerUps.Add(type, Simulation.PowerUpsFactory.Create(type));
        }
    }
}
