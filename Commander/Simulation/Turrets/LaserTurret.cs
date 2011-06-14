namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physics;


    class LaserTurret : Turret
    {
        public Bullet ActiveBullet;
        private Enemy EnemyAttacked;


        public LaserTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.Laser;
            Name = "Laser";
            SfxShooting = "sfxTourelleLaserSimple";
            Color = new Color(255, 71, 187);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 50, 1700, 1, 0, BulletType.LaserSimple, "", "", 0.15f, 0, 0),
                new TurretLevel(1, 10, 5, 50, 1700, 1, 500, BulletType.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.15f, 0, 0),
                new TurretLevel(2, 20, 15, 65, 1650, 1, 1000, BulletType.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.25f, 0, 0),
                new TurretLevel(3, 40, 35, 80, 1600, 1, 1500, BulletType.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.4f, 0, 0),
                new TurretLevel(4, 70, 70, 95, 1550, 1, 2000, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 0.65f, 0, 0),
                new TurretLevel(5, 100, 120, 110, 1500, 1, 2500, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 0.8f, 0, 0),
                new TurretLevel(6, 130, 185, 125, 1450, 1, 3000, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 1f, 0, 0),
                new TurretLevel(7, 160, 265, 140, 1400, 1, 3500, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase", 1.4f, 0, 0),
                new TurretLevel(8, 190, 360, 155, 1350, 1, 4000, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 1.8f, 0, 0),
                new TurretLevel(9, 210, 465, 170, 1300, 1, 4500, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 2.5f, 0, 0),
                new TurretLevel(10, 240, 585, 185, 1250, 1, 5000, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase", 4f, 0, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override Enemy EnemyWatched
        {
            get
            {
                return EnemyAttacked;
            }
            set
            {
                if (ActiveBullet != null && ActiveBullet.Alive)
                    return;

                if (ActiveBullet != null && !ActiveBullet.Alive)
                    ActiveBullet = null;

                EnemyAttacked = value;
            }
        }
    }
}
