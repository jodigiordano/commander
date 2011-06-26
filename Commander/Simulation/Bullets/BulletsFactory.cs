namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;


    class BulletsFactory
    {
        private Dictionary<BulletType, object> BulletsPools;
        private Simulator Simulator;


        public BulletsFactory(Simulator simulator)
        {
            Simulator = simulator;

            BulletsPools = new Dictionary<BulletType, object>(BulletTypeComparer.Default)
            {
                { BulletType.Base, new Pool<BasicBullet>() },
                { BulletType.Gunner, new Pool<GunnerBullet>() },
                { BulletType.LaserMultiple, new Pool<MultipleLasersBullet>() },
                { BulletType.LaserSimple, new Pool<LaserBullet>() },
                { BulletType.Missile, new Pool<MissileBullet>() },
                { BulletType.Nanobots, new Pool<NanobotsBullet>() },
                { BulletType.RailGun, new Pool<RailGunBullet>() },
                { BulletType.SlowMotion, new Pool<SlowMotionBullet>() },
                { BulletType.Shield, new Pool<ShieldBullet>() },
                { BulletType.Pulse, new Pool<PulseBullet>() },
                { BulletType.Mine, new Pool<MineBullet>() }
            };
        }


        public Bullet Get(BulletType type)
        {
            Bullet b = null;

            switch (type)
            {
                case BulletType.Base: b = ((Pool<BasicBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Gunner: b = ((Pool<GunnerBullet>) BulletsPools[type]).Get(); break;
                case BulletType.LaserMultiple: b = ((Pool<MultipleLasersBullet>) BulletsPools[type]).Get(); break;
                case BulletType.LaserSimple: b = ((Pool<LaserBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Missile: b = ((Pool<MissileBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Nanobots: b = ((Pool<NanobotsBullet>) BulletsPools[type]).Get(); break;
                case BulletType.RailGun: b = ((Pool<RailGunBullet>) BulletsPools[type]).Get(); break;
                case BulletType.SlowMotion: b = ((Pool<SlowMotionBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Shield: b = ((Pool<ShieldBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Pulse: b = ((Pool<PulseBullet>) BulletsPools[type]).Get(); break;
                case BulletType.Mine: b = ((Pool<MineBullet>) BulletsPools[type]).Get(); break;
            }

            b.Scene = Simulator.Scene;
            b.Type = type;

            if (!b.AssetsLoaded)
                b.LoadAssets();

            return b;
        }


        public void Return(Bullet bullet)
        {
            switch (bullet.Type)
            {
                case BulletType.Base: ((Pool<BasicBullet>) BulletsPools[bullet.Type]).Return((BasicBullet) bullet); break;
                case BulletType.Gunner: ((Pool<GunnerBullet>) BulletsPools[bullet.Type]).Return((GunnerBullet) bullet); break;
                case BulletType.LaserMultiple: ((Pool<MultipleLasersBullet>) BulletsPools[bullet.Type]).Return((MultipleLasersBullet) bullet); break;
                case BulletType.LaserSimple: ((Pool<LaserBullet>) BulletsPools[bullet.Type]).Return((LaserBullet) bullet); break;
                case BulletType.Missile: ((Pool<MissileBullet>) BulletsPools[bullet.Type]).Return((MissileBullet) bullet); break;
                case BulletType.Nanobots: ((Pool<NanobotsBullet>) BulletsPools[bullet.Type]).Return((NanobotsBullet) bullet); break;
                case BulletType.RailGun: ((Pool<RailGunBullet>) BulletsPools[bullet.Type]).Return((RailGunBullet) bullet); break;
                case BulletType.SlowMotion: ((Pool<SlowMotionBullet>) BulletsPools[bullet.Type]).Return((SlowMotionBullet) bullet); break;
                case BulletType.Shield: ((Pool<ShieldBullet>) BulletsPools[bullet.Type]).Return((ShieldBullet) bullet); break;
                case BulletType.Pulse: ((Pool<PulseBullet>) BulletsPools[bullet.Type]).Return((PulseBullet) bullet); break;
                case BulletType.Mine: ((Pool<MineBullet>) BulletsPools[bullet.Type]).Return((MineBullet) bullet); break;
            }
        }
    }
}
