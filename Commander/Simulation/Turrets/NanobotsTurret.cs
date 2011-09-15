namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class NanobotsTurret : Turret
    {
        public NanobotsTurret( Simulator simulator )
            : base( simulator )
        {
            Type = TurretType.Nanobots;
            Name = @"Nanobots";
            Description = @"Shoot a gaz that does damage over time";
            Color = new Color( 182, 147, 26 );

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
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
