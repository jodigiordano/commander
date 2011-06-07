namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;
    using System;


    class NanobotsTurret : Turret
    {
        public NanobotsTurret( Simulation simulation )
            : base( simulation )
        {
            Type = TurretType.Nanobots;
            Name = "Nanobots";
            SfxShooting = "sfxTourelleBase";
            Color = new Color( 57, 216, 17 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.Nanobots, "", "", 0, 0, 0),
                new TurretLevel(1, 30, 15, 75, 4650, 1, 1000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 1f, 30, 1f),
                new TurretLevel(2, 60, 45, 100, 4600, 1, 1000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 1.5f, 30, 1f),
                new TurretLevel(3, 90, 90, 125, 4550, 1, 1000, BulletType.Nanobots, "tourelleMortar1", "tourelleMortarBase", 2.0f, 30, 1f),
                new TurretLevel(4, 120, 150, 150, 4500, 1, 1000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 2.5f, 30, 1f),
                new TurretLevel(5, 150, 225, 175, 4450, 1, 1000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 3.0f, 30, 1f),
                new TurretLevel(6, 180, 315, 200, 4400, 1, 1000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 3.5f, 30, 1f),
                new TurretLevel(7, 210, 420, 225, 4350, 1, 1000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 4.0f, 30, 1f),
                new TurretLevel(8, 240, 540, 250, 4300, 1, 1000, BulletType.Nanobots, "tourelleMortar2", "tourelleMortarBase", 4.5f, 30, 1f),
                new TurretLevel(9, 270, 675, 275, 4250, 1, 1000, BulletType.Nanobots, "tourelleMortar3", "tourelleMortarBase", 5.0f, 30, 1f),
                new TurretLevel(10, 300, 825, 300, 4200, 1, 1000, BulletType.Nanobots, "tourelleMortar3", "tourelleMortarBase", 5.5f, 30, 1f)
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
