namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physics;
    using System;


    class NanobotsTurret : Turret
    {
        public NanobotsTurret( Simulator simulation )
            : base( simulation )
        {
            Type = TurretType.Nanobots;
            Name = "Nanobots";
            SfxShooting = "sfxTourelleBase";
            Color = new Color( 182, 147, 26 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 120, 2000, 1, 0, BulletType.Nanobots, "", "", 0, 0, 0),
                new TurretLevel(1, 300, 100, 120, 2000, 1, 4000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 0.2f, 30, 1f),
                new TurretLevel(2, 200, 300, 140, 1900, 1, 6000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 0.4f, 30, 1f),
                new TurretLevel(3, 300, 600, 160, 1800, 2, 10000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 0.6f, 30, 1f),
                new TurretLevel(4, 400, 1000, 180, 1700, 2, 15000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 0.8f, 30, 1f),
                new TurretLevel(5, 500, 1500, 200, 1600, 2, 20000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 1.0f, 30, 1f),
                new TurretLevel(6, 600, 2100, 220, 1500, 2, 25000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 1.2f, 30, 1f),
                new TurretLevel(7, 700, 2750, 240, 1400, 3, 30000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 1.4f, 30, 1f),
                new TurretLevel(8, 800, 3600, 280, 1300, 3, 35000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 1.6f, 30, 1f),
                new TurretLevel(9, 900, 4500, 300, 1200, 3, 40000, BulletType.Nanobots, "tourelleMortar3", "tourelleMortarBase", 1.8f, 30, 1f),
                new TurretLevel(10, 1000, 5500, 320, 1100, 3, 45000, BulletType.Nanobots, "tourelleMortar3", "tourelleMortarBase", 2.0f, 30, 1f)
            };

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
