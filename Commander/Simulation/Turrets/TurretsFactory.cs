namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class TurretsFactory
    {
        public Dictionary<TurretType, Turret> Availables;
        public Dictionary<int, TurretBoostLevel> BoostLevels;
        private Simulator Simulator;


        public TurretsFactory(Simulator simulator)
        {
            Simulator = simulator;

            Availables = new Dictionary<TurretType, Turret>(TurretTypeComparer.Default);

            BoostLevels = new Dictionary<int, TurretBoostLevel>();
            BoostLevels.Add(0, new TurretBoostLevel()
            {
                FireRateMultiplier = 1,
                RangeMultiplier = 1,
                Level = 0,
                BulletSpeedMultiplier = 1,
                BulletHitPointsMultiplier = 1,
                BulletExplosionRangeMultiplier = 1
            });
            BoostLevels.Add(1, new TurretBoostLevel()
            {
                FireRateMultiplier = 1.1f,
                RangeMultiplier = 1.3f,
                Level = 1,
                BulletSpeedMultiplier = 1.1f,
                BulletHitPointsMultiplier = 1.1f,
                BulletExplosionRangeMultiplier = 1.1f
            });
            BoostLevels.Add(2, new TurretBoostLevel()
            {
                FireRateMultiplier = 1.2f,
                RangeMultiplier = 1.6f,
                Level = 2,
                BulletSpeedMultiplier = 1.2f,
                BulletHitPointsMultiplier = 1.2f,
                BulletExplosionRangeMultiplier = 1.2f
            });
            BoostLevels.Add(3, new TurretBoostLevel()
            {
                FireRateMultiplier = 1.4f,
                RangeMultiplier = 1.8f,
                Level = 3,
                BulletSpeedMultiplier = 1.3f,
                BulletHitPointsMultiplier = 1.3f,
                BulletExplosionRangeMultiplier = 1.3f
            });
        }


        public Turret Create(TurretType type)
        {
            Turret t = null;

            switch (type)
            {
                case TurretType.Basic:              t = new BasicTurret(Simulator);            break;
                case TurretType.Gravitational:      t = new GravitationalTurret(Simulator);    break;
                case TurretType.MultipleLasers:     t = new MultipleLasersTurret(Simulator);   break;
                case TurretType.Laser:              t = new LaserTurret(Simulator);            break;
                case TurretType.Missile:            t = new MissileTurret(Simulator);          break;
                case TurretType.Alien:              t = new AlienTurret(Simulator);            break;
                case TurretType.SlowMotion:         t = new SlowMotionTurret(Simulator);       break;
                case TurretType.Booster:            t = new BoosterTurret(Simulator);          break;
                case TurretType.Gunner:             t = new GunnerTurret(Simulator);           break;
                case TurretType.Nanobots:           t = new NanobotsTurret(Simulator);         break;
                case TurretType.RailGun:            t = new RailGunTurret(Simulator);          break;
                default:                            t = new BasicTurret(Simulator);            break;
            }

            return t;
        }
    }
}
