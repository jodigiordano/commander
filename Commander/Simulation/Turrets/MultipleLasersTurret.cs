namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;

    class MultipleLasersTurret : Turret
    {
        public MultipleLasersTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.MultipleLasers;
            Name = @"Multi-laser";
            Description = @"Shoot a laser that hit every enemy on it's path";
            Color = new Color(255, 96, 28);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override double VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.00001;
            }
        }
    }
}
