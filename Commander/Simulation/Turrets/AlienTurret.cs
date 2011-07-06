namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class AlienTurret : GravitationalTurret
    {
        public AlienTurret(Simulator simulator)
            : base(simulator)
        {
            Name = "Alien";
            Type = TurretType.Alien;
            AntennaRotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            AntennaRotationBase = 0;
            Color = new Color(202, 196, 255);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            Upgrade();
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                AntennaRotationBase = (AntennaRotationSpeed > 0) ? -0.02f : 0.02f;
                CanonImage.Color.A = 60;
                BaseImage.Color.A = 60;
                return true;
            }

            return false;
        }
    }
}