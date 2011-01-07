namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;

    class MultipleLasersTurret : Turret
    {
        public MultipleLasersTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.MultipleLasers;
            Name = "Multi-laser";
            SfxShooting = "sfxTourelleLaserMultiple";
            Color = new Color(255, 96, 28);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, new Cercle(Vector3.Zero, 120), 2000, 1, 0, BulletType.LaserMultiple, "", "", 0.8f, null, 0),
                new TurretLevel(1, 300, 100, new Cercle(Vector3.Zero, 120), 2000, 1, 4000, BulletType.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase", 0.8f, null, 0),
                new TurretLevel(2, 200, 300, new Cercle(Vector3.Zero, 140), 1900, 1, 6000, BulletType.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase", 1.0f, null, 0),
                new TurretLevel(3, 300, 600, new Cercle(Vector3.Zero, 160), 1800, 2, 10000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.6f, null, 0),
                new TurretLevel(4, 400, 1000, new Cercle(Vector3.Zero, 180), 1700, 2, 15000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.8f, null, 0),
                new TurretLevel(5, 500, 1500, new Cercle(Vector3.Zero, 200), 1600, 2, 20000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.9f, null, 0),
                new TurretLevel(6, 600, 2100, new Cercle(Vector3.Zero, 220), 1500, 2, 25000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 1.0f, null, 0),
                new TurretLevel(7, 700, 2750, new Cercle(Vector3.Zero, 240), 1400, 3, 30000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.7f, null, 0),
                new TurretLevel(8, 800, 3600, new Cercle(Vector3.Zero, 280), 1300, 3, 35000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.8f, null, 0),
                new TurretLevel(9, 900, 4500, new Cercle(Vector3.Zero, 300), 1200, 3, 40000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.9f, null, 0),
                new TurretLevel(10, 1000, 5500, new Cercle(Vector3.Zero, 320), 1100, 3, 45000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 1.0f, null, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override float VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.00001f;
            }
        }
    }
}
