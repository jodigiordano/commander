namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class SlowMotionTurret : Turret
    {
        public float RotationSpeed;
        private float LastRotation;


        public SlowMotionTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.SlowMotion;
            Name = @"Slow";
            Description = @"Slow down enemies";
            Color = new Color(255, 216, 0);

            RotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            LastRotation = 0;

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Draw()
        {
            base.Draw();

            CanonImage.Rotation = LastRotation + RotationSpeed;
            LastRotation = CanonImage.Rotation;
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