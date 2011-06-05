namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    public enum TurretType
    {
        Basic,
        Missile,
        Gravitational,
        MultipleLasers,
        Laser,
        Unknown,
        Alien,
        SlowMotion,
        Booster,
        Gunner,
        Nanobots,
        RailGun,
        Sniper
    };


    class TurretsFactory
    {
        public Dictionary<TurretType, Turret> Availables;
        public Dictionary<int, TurretBoostLevel> BoostLevels;
        private Simulation Simulation;


        public TurretsFactory(Simulation simulation)
        {
            Simulation = simulation;

            Availables = new Dictionary<TurretType, Turret>();

            BoostLevels = new Dictionary<int, TurretBoostLevel>();
            BoostLevels.Add(0, new TurretBoostLevel()
            {
                FireRateMultiplier = 1,
                RangeMultiplier = 1,
                Level = 1,
                BulletSpeedMultiplier = 1,
                BulletHitPointsMultiplier = 1,
                BulletExplosionRangeMultiplier = 1
            });
            BoostLevels.Add(1, new TurretBoostLevel()
            {
                FireRateMultiplier = 1.1f,
                RangeMultiplier = 1.5f,
                Level = 1,
                BulletSpeedMultiplier = 1.1f,
                BulletHitPointsMultiplier = 1.1f,
                BulletExplosionRangeMultiplier = 1.1f
            });
            BoostLevels.Add(2, new TurretBoostLevel()
            {
                FireRateMultiplier = 1.2f,
                RangeMultiplier = 2.0f,
                Level = 2,
                BulletSpeedMultiplier = 1.2f,
                BulletHitPointsMultiplier = 1.2f,
                BulletExplosionRangeMultiplier = 1.2f
            });
        }


        public Turret Create(TurretType type)
        {
            Turret t = null;

            switch (type)
            {
                case TurretType.Basic:              t = new BasicTurret(Simulation);            break;
                case TurretType.Gravitational:      t = new GravitationalTurret(Simulation);    break;
                case TurretType.MultipleLasers:     t = new MultipleLasersTurret(Simulation);   break;
                case TurretType.Laser:              t = new LaserTurret(Simulation);            break;
                case TurretType.Missile:            t = new MissileTurret(Simulation);          break;
                case TurretType.Alien:              t = new AlienTurret(Simulation);            break;
                case TurretType.SlowMotion:         t = new SlowMotionTurret(Simulation);       break;
                case TurretType.Booster:            t = new BoosterTurret(Simulation);          break;
                case TurretType.Gunner:             t = new GunnerTurret(Simulation);           break;
                case TurretType.Nanobots:           t = new NanobotsTurret(Simulation);         break;
                case TurretType.RailGun:            t = new RailGunTurret(Simulation);          break;
                default:                            t = new BasicTurret(Simulation);            break;
            }

            return t;
        }
    }
}
