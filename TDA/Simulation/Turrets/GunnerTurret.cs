namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;
    using System;


    class GunnerTurret : Turret
    {
        public Projectile ActiveBullet;
        private Ennemi EnemyAttacked;


        public GunnerTurret( Simulation simulation )
            : base( simulation )
        {
            Type = TurretType.Gunner;
            Name = "Gunner";
            SfxShooting = "sfxTourelleBase";
            Color = new Color( 57, 216, 17 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 300, 1, 0, BulletType.Gunner, "", "", 0, 0, 0),
                new TurretLevel(1, 30, 15, 75, 275, 1, 1000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 5, 0, 0),
                new TurretLevel(2, 60, 45, 100, 250, 1, 1000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 7, 0, 0),
                new TurretLevel(3, 90, 90, 125, 225, 1, 1000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 10, 0, 0),
                new TurretLevel(4, 120, 150, 150, 200, 2, 1000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 7, 0, 0),
                new TurretLevel(5, 150, 225, 175, 175, 2, 1000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 10, 0, 0),
                new TurretLevel(6, 180, 315, 200, 150, 2, 1000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 12, 0, 0),
                new TurretLevel(7, 210, 420, 225, 125, 2, 1000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 14, 0, 0),
                new TurretLevel(8, 240, 540, 250, 100, 3, 1000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 9, 0, 0),
                new TurretLevel(9, 270, 675, 275, 75, 3, 1000, BulletType.Gunner, "tourelleGunner3", "tourelleGunnerBase", 11, 0, 0),
                new TurretLevel(10, 300, 825, 300, 50, 3, 1000, BulletType.Gunner, "tourelleGunner3", "tourelleGunnerBase", 12, 0, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override Ennemi EnemyWatched
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
