namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class TurretsFactory
    {
        public Dictionary<TurretType, Turret> Availables;
        public Dictionary<TurretType, Turret> All;
        public Dictionary<int, TurretBoostLevel> BoostLevels;
        public Dictionary<TurretType, LinkedListWithInit<TurretLevel>> TurretsLevels;

        private Simulator Simulator;


        public TurretsFactory(Simulator simulator)
        {
            Simulator = simulator;

            Availables = new Dictionary<TurretType, Turret>(TurretTypeComparer.Default);
            All = new Dictionary<TurretType, Turret>(TurretTypeComparer.Default);

            BoostLevels = new Dictionary<int, TurretBoostLevel>();

            TurretsLevels = new Dictionary<TurretType, LinkedListWithInit<TurretLevel>>(TurretTypeComparer.Default);
        }


        public void Initialize()
        {
            BoostLevels.Clear();
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

            TurretsLevels.Clear();
            TurretsLevels.Add(TurretType.Basic, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.Base, "", "", 0, 0, "", ""),
                new TurretLevel(1, 30, 15, 75, 650, 1, 1000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 5, 7, "BasicTurretMoving", "BasicTurretFiring1"),
                new TurretLevel(2, 80, 45, 100, 600, 1, 2000, BulletType.Base, "tourelleBase1", "tourelleBaseBase2", 7, 7, "BasicTurretMoving", "BasicTurretFiring2"),
                new TurretLevel(3, 150, 150, 150, 500, 2, 4000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 7, 8, "BasicTurretMoving", "BasicTurretFiring3"),
                new TurretLevel(4, 300, 225, 175, 450, 2, 5000, BulletType.Base, "tourelleBase2", "tourelleBaseBase2", 10, 8, "BasicTurretMoving", "BasicTurretFiring4"),
                new TurretLevel(5, 500, 540, 250, 300, 3, 8000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 9, 9, "BasicTurretMoving", "BasicTurretFiring5"),
                new TurretLevel(6, 750, 675, 275, 250, 3, 9000, BulletType.Base, "tourelleBase3", "tourelleBaseBase2", 11, 9, "BasicTurretMoving", "BasicTurretFiring6"),
                new TurretLevel(7, 1000, 825, 300, 200, 3, 10000, BulletType.Base, "tourelleBase3", "tourelleBaseBase3", 12, 10, "BasicTurretMoving", "BasicTurretFiring7")
            });
            TurretsLevels.Add(TurretType.Booster, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 50, Int16.MaxValue, 1, 0, BulletType.None, "", "", 0, 0, "", ""),
                new TurretLevel(1, 400, 200, 75, Int16.MaxValue, 1, 5000, BulletType.None, "PixelBlanc", "tourelleBooster", 0, 0, "BoosterTurretMoving", "BoosterTurretFiring1"),
                new TurretLevel(2, 600, 500, 125, Int16.MaxValue, 1, 10000, BulletType.None, "PixelBlanc", "tourelleBooster2", 0, 0, "BoosterTurretMoving", "BoosterTurretFiring2"),
                new TurretLevel(3, 800, 1000, 175, Int16.MaxValue, 1, 20000, BulletType.None, "PixelBlanc", "tourelleBooster3", 0, 0, "BoosterTurretMoving", "BoosterTurretFiring3")
            });
            TurretsLevels.Add(TurretType.Gravitational, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 1, Int16.MaxValue, 1, 0, BulletType.None, "", "", 0, 0, "", ""),
                new TurretLevel(1, 1000, 500, 1, Int16.MaxValue, 1, 500, BulletType.None, "tourelleGravitationnelleAntenne", "tourelleGravitationnelleBase", 0, 0, "GravitationalTurretMoving", "GravitationalTurretFiring1"),
                new TurretLevel(2, 500, 750, 1, Int16.MaxValue, 1, 500, BulletType.None, "tourelleGravitationnelleAntenne2", "tourelleGravitationnelleBase", 0, 0, "GravitationalTurretMoving", "GravitationalTurretFiring2")
            });
            TurretsLevels.Add(TurretType.Gunner, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 150, 1000, 1, 0, BulletType.Gunner, "", "", 0, 0, "", ""),
                new TurretLevel(1, 150, 75, 150, 300, 1, 3000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 5, 0, "GunnerTurretMoving", "GunnerTurretFiring1"),
                new TurretLevel(2, 100, 125, 200, 275, 1, 5000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase2", 7, 0, "GunnerTurretMoving", "GunnerTurretFiring2"),
                new TurretLevel(3, 150, 200, 250, 250, 1, 7000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 10, 0, "GunnerTurretMoving", "GunnerTurretFiring3"),
                new TurretLevel(4, 200, 300, 275, 225, 1, 9000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase2", 7, 0, "GunnerTurretMoving", "GunnerTurretFiring4"),
                new TurretLevel(5, 250, 425, 300, 200, 1, 11000, BulletType.Gunner, "tourelleGunner1", "tourelleGunnerBase", 10, 0, "GunnerTurretMoving", "GunnerTurretFiring5"),
                new TurretLevel(6, 300, 575, 325, 175, 1, 13000, BulletType.Gunner, "tourelleGunner2", "tourelleGunnerBase2", 12, 0, "GunnerTurretMoving", "GunnerTurretFiring6"),
                new TurretLevel(7, 350, 750, 350, 150, 1, 15000, BulletType.Gunner, "tourelleGunner3", "tourelleGunnerBase", 14, 0, "GunnerTurretMoving", "GunnerTurretFiring7")
            });
            TurretsLevels.Add(TurretType.Laser, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 75, 1300, 1, 0, BulletType.LaserSimple, "", "", 0.15f, 0, "", ""),
                new TurretLevel(1, 30, 15, 75, 1300, 1, 500, BulletType.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase", 0.15f, 0, "LaserTurretMoving", "LaserTurretFiring1"),
                new TurretLevel(2, 60, 45, 80, 1250, 1, 1000, BulletType.LaserSimple, "tourelleLaserCanon1", "tourelleLaserBase2", 0.25f, 0, "LaserTurretMoving", "LaserTurretFiring2"),
                new TurretLevel(3, 120, 150, 90, 1200, 1, 2000, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase3", 0.35f, 0, "LaserTurretMoving", "LaserTurretFiring3"),
                new TurretLevel(4, 150, 225, 100, 1150, 1, 3500, BulletType.LaserSimple, "tourelleLaserCanon2", "tourelleLaserBase4", 0.50f, 0, "LaserTurretMoving", "LaserTurretFiring4"),
                new TurretLevel(5, 240, 540, 110, 1100, 1, 4000, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase5", 0.75f, 0, "LaserTurretMoving", "LaserTurretFiring5"),
                new TurretLevel(6, 270, 675, 125, 1050, 1, 4500, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase6", 1.00f, 0, "LaserTurretMoving", "LaserTurretFiring6"),
                new TurretLevel(7, 300, 825, 150, 1000, 1, 5000, BulletType.LaserSimple, "tourelleLaserCanon3", "tourelleLaserBase7", 1.50f, 0, "LaserTurretMoving", "LaserTurretFiring7")
            });
            TurretsLevels.Add(TurretType.Missile, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 150, 2600, 1, 0, BulletType.Missile, "", "", 30, 1.8f, "", "") { BulletExplosionRange = 50 },
                new TurretLevel(1, 100, 125, 100, 2400, 1, 5000, BulletType.Missile, "tourelleMissileCanon1", "tourelleMissileBase", 3, 2.0f, "MissileTurretMoving", "MissileTurretLaunching1") { BulletExplosionRange = 60, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(2, 150, 200, 125, 2200, 1, 7000, BulletType.Missile, "tourelleMissileCanon1", "tourelleMissileBase2", 6, 2.2f, "MissileTurretMoving", "MissileTurretLaunching2") { BulletExplosionRange = 70, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(3, 200, 300, 150, 2000, 1, 9000, BulletType.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 9, 2.4f, "MissileTurretMoving", "MissileTurretLaunching3") { BulletExplosionRange = 80, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(4, 250, 425, 175, 1800, 1, 11000, BulletType.Missile, "tourelleMissileCanon2", "tourelleMissileBase2", 12, 2.6f, "MissileTurretMoving", "MissileTurretLaunching4") { BulletExplosionRange = 90, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(5, 300, 575, 200, 1600, 1, 13000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 15, 2.8f, "MissileTurretMoving", "MissileTurretLaunching5") { BulletExplosionRange = 100, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(6, 350, 750, 225, 1400, 1, 15000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase2", 18, 3.0f, "MissileTurretMoving", "MissileTurretLaunching6") { BulletExplosionRange = 110, BulletExplosionSfx = "MissileExplosion" },
                new TurretLevel(7, 450, 1175, 250, 1000, 1, 19000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase3", 21, 3.4f, "MissileTurretMoving", "MissileTurretLaunching7") { BulletExplosionRange = 130, BulletExplosionSfx = "MissileExplosion" }
            });
            TurretsLevels.Add(TurretType.MultipleLasers, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 120, 2000, 1, 0, BulletType.LaserMultiple, "", "", 0.8f, 0, "", ""),
                new TurretLevel(1, 300, 100, 120, 2000, 1, 4000, BulletType.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase", 0.8f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring1"),
                new TurretLevel(2, 200, 300, 140, 1900, 1, 6000, BulletType.LaserMultiple, "tourelleLaserMultiple1", "tourelleLaserMultipleBase2", 1.0f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring2"),
                new TurretLevel(3, 300, 600, 160, 1800, 2, 10000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase", 0.6f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring3"),
                new TurretLevel(4, 400, 1000, 180, 1700, 2, 15000, BulletType.LaserMultiple, "tourelleLaserMultiple2", "tourelleLaserMultipleBase2", 0.8f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring4"),
                new TurretLevel(5, 700, 2750, 240, 1400, 3, 30000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase", 0.7f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring5"),
                new TurretLevel(6, 800, 3600, 280, 1300, 3, 35000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase2", 0.8f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring6"),
                new TurretLevel(7, 900, 4500, 300, 1200, 3, 40000, BulletType.LaserMultiple, "tourelleLaserMultiple3", "tourelleLaserMultipleBase3", 0.9f, 0, "MultipleLasersTurretMoving", "MultipleLasersTurretFiring7")
            });
            TurretsLevels.Add(TurretType.Nanobots, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 120, 2000, 1, 0, BulletType.Nanobots, "", "", 0, 0, "", ""),
                new TurretLevel(1, 300, 100, 120, 2000, 1, 4000, BulletType.Nanobots, "tourelleNanobots1", "tourelleNanobotsBase", 0.2f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring1") { BulletExplosionRange = 30 },
                new TurretLevel(2, 200, 300, 140, 1900, 1, 6000, BulletType.Nanobots, "tourelleNanobots2", "tourelleNanobotsBase", 0.4f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring2") { BulletExplosionRange = 30 },
                new TurretLevel(3, 300, 600, 160, 1800, 2, 10000, BulletType.Nanobots, "tourelleNanobots1", "tourelleNanobotsBase2", 0.6f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring3") { BulletExplosionRange = 30 },
                new TurretLevel(4, 400, 1000, 180, 1700, 2, 15000, BulletType.Nanobots, "tourelleNanobots2", "tourelleNanobotsBase2", 0.8f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring4") { BulletExplosionRange = 30 },
                new TurretLevel(5, 500, 1500, 200, 1600, 2, 20000, BulletType.Nanobots, "tourelleNanobots1", "tourelleNanobotsBase3", 1.0f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring5") { BulletExplosionRange = 30 },
                new TurretLevel(6, 600, 2100, 220, 1500, 2, 25000, BulletType.Nanobots, "tourelleNanobots2", "tourelleNanobotsBase3", 1.2f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring6") { BulletExplosionRange = 30 },
                new TurretLevel(7, 700, 2750, 240, 1400, 3, 30000, BulletType.Nanobots, "tourelleNanobots3", "tourelleNanobotsBase3", 1.4f, 1f, "NanobotsTurretMoving", "NanobotsTurretFiring7") { BulletExplosionRange = 30 }
            });
            TurretsLevels.Add(TurretType.RailGun, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.RailGun, "", "", 0, 0, "", ""),
                new TurretLevel(1, 30, 15, 75, 4000, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring1") { BulletExplosionRange = 100 },
                new TurretLevel(2, 30, 15, 75, 3500, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring2") { BulletExplosionRange = 100 },
                new TurretLevel(3, 30, 15, 75, 3000, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring3") { BulletExplosionRange = 100 },
                new TurretLevel(4, 30, 15, 75, 2500, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring4") { BulletExplosionRange = 100 },
                new TurretLevel(5, 30, 15, 75, 2250, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring5") { BulletExplosionRange = 100 },
                new TurretLevel(6, 30, 15, 75, 2000, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring6") { BulletExplosionRange = 100 },
                new TurretLevel(7, 30, 15, 75, 1750, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 15, "RailgunTurretMoving", "RailgunTurretFiring7") { BulletExplosionRange = 100 }
            });
            TurretsLevels.Add(TurretType.SlowMotion, new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 100, 2000, 1, 0, BulletType.SlowMotion, "", "", 0.2f, 0, "", ""),
                new TurretLevel(1, 120, 110, 120, 1400, 1, 3500, BulletType.SlowMotion, "tourelleSlowMotionCanon1", "tourelleSlowMotionBase", 0.6f, 0, "SlowMotionTurretMoving", "SlowMotionTurret"),
                new TurretLevel(2, 140, 180, 140, 1300, 1, 5000, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase", 0.8f, 0, "SlowMotionTurretMoving", "SlowMotionTurret"),
                new TurretLevel(3, 160, 260, 160, 1200, 1, 6500, BulletType.SlowMotion, "tourelleSlowMotionCanon2", "tourelleSlowMotionBase2", 1.0f, 0, "SlowMotionTurretMoving", "SlowMotionTurret"),
                new TurretLevel(4, 180, 350, 180, 1100, 1, 8000, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase", 1.2f, 0, "SlowMotionTurretMoving", "SlowMotionTurret"),
                new TurretLevel(5, 240, 680, 240, 1000, 1, 12500, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase2", 1.4f, 0, "SlowMotionTurretMoving", "SlowMotionTurret"),
                new TurretLevel(6, 280, 820, 260, 900, 1, 14000, BulletType.SlowMotion, "tourelleSlowMotionCanon3", "tourelleSlowMotionBase3", 1.6f, 0, "SlowMotionTurretMoving", "SlowMotionTurret")
            });

            All.Clear();
            All.Add(TurretType.Basic, Create(TurretType.Basic));
            All.Add(TurretType.Gravitational, Create(TurretType.Gravitational));
            All.Add(TurretType.MultipleLasers, Create(TurretType.MultipleLasers));
            All.Add(TurretType.Laser, Create(TurretType.Laser));
            All.Add(TurretType.Missile, Create(TurretType.Missile));
            All.Add(TurretType.SlowMotion, Create(TurretType.SlowMotion));
            All.Add(TurretType.Booster, Create(TurretType.Booster));
            All.Add(TurretType.Gunner, Create(TurretType.Gunner));
            All.Add(TurretType.Nanobots, Create(TurretType.Nanobots));
            All.Add(TurretType.RailGun, Create(TurretType.RailGun));
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
                case TurretType.SlowMotion:         t = new SlowMotionTurret(Simulator);       break;
                case TurretType.Booster:            t = new BoosterTurret(Simulator);          break;
                case TurretType.Gunner:             t = new GunnerTurret(Simulator);           break;
                case TurretType.Nanobots:           t = new NanobotsTurret(Simulator);         break;
                case TurretType.RailGun:            t = new RailGunTurret(Simulator);          break;
                default:                            t = new BasicTurret(Simulator);            break;
            }

            return t;
        }


        public Turret Create(TurretType type, int level, Vector3 relativePosition, bool visible, bool canSell, bool canUpgrade, bool canSelect)
        {
            var t = Create(type);

            t.Level = level;
            t.RelativePosition = relativePosition;
            t.Visible = visible;
            t.CanSell = canSell;
            t.CanUpdate = canUpgrade;
            t.CanSelect = canSelect;

            return t;
        }
    }
}
