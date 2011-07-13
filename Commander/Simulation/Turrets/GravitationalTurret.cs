namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class GravitationalTurret : Turret
    {
        public float AntennaRotationSpeed;
        public float AntennaRotationBase;


        public GravitationalTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Gravitational;
            Name = @"Gravitational";
            Description = @"Modify the path taken by enemies";

            AntennaRotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            AntennaRotationBase = 0;
            Color = new Color(202, 196, 255);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Draw()
        {
            CanonImage.Rotation += AntennaRotationSpeed;
            BaseImage.Rotation += AntennaRotationBase;

            base.Draw();
        }


        public override double VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.00001f;
            }
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                CanonImage.Origin = CanonImage.Center;
                return true;
            }

            return false;
        }
    }
}