namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    [XmlRoot(ElementName = "Level")]
    public class LevelDescriptor
    {
        public InfosDescriptor Infos;
        public ObjectiveDescriptor Objective;

        [XmlArrayItem("CelestialBody")]
        public List<CelestialBodyDescriptor> PlanetarySystem;

        public InfiniteWavesDescriptor InfiniteWaves;

        [XmlArrayItem("Wave")]
        public List<WaveDescriptor> Waves;

        public PlayerDescriptor Player;
        public List<TurretType> AvailableTurrets;
        public List<PowerUpType> AvailablePowerUps;

        public MineralsDescriptor Minerals;
        public string HelpText;

        private XmlSerializer Serializer;


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

            HelpText = "";

            Serializer = new XmlSerializer(this.GetType());
        }


        public void AddPlanet(Size size, Vector3 position, string name, string image, int speed, int pathPriority)
        {
            var d = new PlanetCBDescriptor()
            {
                Name = name,
                Invincible = true,
                Position = position,
                PathPriority = pathPriority,
                Size = size,
                Speed = speed,
                Image = image
            };

            PlanetarySystem.Add(d);
        }


        public void AddPinkHole(Vector3 position, string name, int speed, int priority)
        {
            PlanetarySystem.Add(CreatePinkHole(position, name, speed, priority));
        }


        public PinkHoleCBDescriptor CreatePinkHole(Vector3 position, string name, int speed, int priority)
        {
            return new PinkHoleCBDescriptor()
            {
                Name = name,
                Invincible = true,
                Position = position,
                PathPriority = priority,
                Size = Size.Small,
                Speed = speed
            };
        }


        public void AddAsteroidBelt()
        {
            var c = new AsteroidBeltCBDescriptor()
            {
                Name = "Asteroid belt",
                Images = new List<string>() { "Asteroid" }
            };

            PlanetarySystem.Add(c);
        }


        public double GetParTime()
        {
            if (InfiniteWaves != null)
                return 0;

            double parTime = 0;

            foreach (var wave in Waves)
                parTime += wave.StartingTime;

            return parTime;
        }


        public int GetPotentialScore()
        {
            int maxCash = 0;

            foreach (var wave in Waves)
                maxCash += wave.Quantity * wave.CashValue;

            int maxLives = Player.Lives + Minerals.LifePacks;

            return (int) ((GetTotalLives(maxLives) + GetTotalCash(maxCash) + GetTotalTime(GetParTime() * 0.75)) * 0.60);
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


        public static AsteroidBeltCBDescriptor GetAsteroidBelt(List<CelestialBodyDescriptor> celestialBodies)
        {
            foreach (var c in celestialBodies)
                if (c is AsteroidBeltCBDescriptor)
                    return (AsteroidBeltCBDescriptor) c;

            return null;
        }


        public PhysicalRectangle GetBoundaries(Vector3 padding)
        {
            PhysicalRectangle boundaries = new PhysicalRectangle();

            boundaries.X = (int) -Preferences.BattlefieldBoundaries.X / 2;
            boundaries.Y = (int) -Preferences.BattlefieldBoundaries.Y / 2;
            boundaries.Width = (int) Preferences.BattlefieldBoundaries.X;
            boundaries.Height = (int) Preferences.BattlefieldBoundaries.Y;

            foreach (var cb in PlanetarySystem)
            {
                // don't include asteroid belt
                if (!(cb is CustomizableBodyDescriptor))
                    continue;

                var b = ((CustomizableBodyDescriptor) cb).GetBoundaries();

                if (b.Left < boundaries.Left)
                    boundaries.X = b.Left;

                if (b.Right > boundaries.Right)
                    boundaries.Width += b.Right - boundaries.Right;

                if (b.Top < boundaries.Top)
                    boundaries.Y = b.Top;

                if (b.Bottom > boundaries.Bottom)
                    boundaries.Height += b.Bottom - boundaries.Bottom;
            }

            boundaries.X -= (int) (padding.X / 2);
            boundaries.Y -= (int) (padding.Y / 2);
            boundaries.Width += (int) padding.X;
            boundaries.Height += (int) padding.Y;

            return boundaries;
        }


        public string ToXML()
        {
            using (StringWriter writer = new StringWriter())
            {
                Serializer.Serialize(writer, this);

                return writer.ToString();
            }
        }
    }


    [XmlRoot(ElementName = "Infos")]
    public class InfosDescriptor
    {
        public int Id;
        public string Background;


        public InfosDescriptor()
        {
            Id = -1;
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
}
