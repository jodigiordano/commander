namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physics;
    using System;


    class GunnerTurret : Turret
    {
        public Bullet ActiveBullet;
        private Enemy EnemyAttacked;


        public GunnerTurret( Simulation simulation )
            : base( simulation )
        {
            Type = TurretType.Gunner;
            Name = "Gunner";
            SfxShooting = "sfxTourelleBase";
            Color = new Color( 57, 216, 17 );

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 150, 1000, 1, 0, BulletType.Gunner, "", "", 0, 0, 0),
                new TurretLevel(1, 150, 75, 150, 300, 1, 3000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 5, 0, 0),
                new TurretLevel(2, 100, 125, 200, 275, 1, 5000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 7, 0, 0),
                new TurretLevel(3, 150, 200, 250, 250, 1, 7000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 10, 0, 0),
                new TurretLevel(4, 200, 300, 275, 225, 1, 9000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 7, 0, 0),
                new TurretLevel(5, 250, 425, 300, 200, 1, 11000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 10, 0, 0),
                new TurretLevel(6, 300, 575, 325, 175, 1, 13000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 12, 0, 0),
                new TurretLevel(7, 350, 750, 350, 150, 1, 15000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 14, 0, 0),
                new TurretLevel(8, 400, 950, 400, 125, 1, 17000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase", 9, 0, 0),
                new TurretLevel(9, 450, 1175, 425, 100, 1, 19000, BulletType.Gunner, "tourelleGunner3", "tourelleGunnerBase", 11, 0, 0),
                new TurretLevel(10, 500, 1425, 450, 75, 1, 21000, BulletType.Gunner, "tourelleGunner3", "tourelleGunnerBase", 12, 0, 0)
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
