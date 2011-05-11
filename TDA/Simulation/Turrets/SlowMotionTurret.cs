namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class SlowMotionTurret : Turret
    {
        public float RotationSpeed;
        private float LastRotation;


        public SlowMotionTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.SlowMotion;
            Name = "Slow";
            SfxShooting = "sfxTourelleSlowMotion";
            Color = new Color(255, 216, 0);

            RotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            LastRotation = 0;

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, new Cercle(Vector3.Zero, 100), 2000, 1, 0, BulletType.SlowMotion, "", "", 0.2f, null, 0),
                new TurretLevel(1, 100, 50, new Cercle(Vector3.Zero, 100), 2000, 1, 2000, BulletType.SlowMotion, "tourelleSlowMotionCanon1", "tourelleSlowMotionBase", 0.2f, null, 0),
                new TurretLevel(2, 120, 110, new Cercle(Vector3.Zero, 120), 1800, 1, 3500, BulletType.SlowMotion, "tourelleSlowMotionCanon1", "tourelleSlowMotionBase", 0.4f, null, 0),
                new TurretLevel(3, 140, 180, new Cercle(Vector3.Zero, 140), 1600, 1, 5000, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.6f, null, 0),
                new TurretLevel(4, 160, 260, new Cercle(Vector3.Zero, 160), 1400, 1, 6500, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.8f, null, 0),
                new TurretLevel(5, 180, 350, new Cercle(Vector3.Zero, 180), 1300, 1, 8000, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.9f, null, 0),
                new TurretLevel(6, 200, 450, new Cercle(Vector3.Zero, 200), 1200, 1, 9500, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 1.0f, null, 0),
                new TurretLevel(7, 220, 560, new Cercle(Vector3.Zero, 220), 1100, 1, 11000, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 1.1f, null, 0),
                new TurretLevel(8, 240, 680, new Cercle(Vector3.Zero, 240), 1000, 1, 12500, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.2f, null, 0),
                new TurretLevel(9, 280, 820, new Cercle(Vector3.Zero, 260), 900, 1, 14000, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.3f, null, 0),
                new TurretLevel(10, 300, 970, new Cercle(Vector3.Zero, 280), 800, 1, 15500, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.4f, null, 0)
            };

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