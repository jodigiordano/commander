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
        SlowMotion
    };


    class TurretsFactory
    {
        public List<Turret> AvailableTurrets;
        private Simulation Simulation;

        public TurretsFactory(Simulation simulation)
        {
            Simulation = simulation;
            AvailableTurrets = new List<Turret>();

            AvailableTurrets = new List<Turret>();
            AvailableTurrets.Add(CreateTurret(TurretType.Basic));
            AvailableTurrets.Add(CreateTurret(TurretType.Gravitational));
            AvailableTurrets.Add(CreateTurret(TurretType.MultipleLasers));
            AvailableTurrets.Add(CreateTurret(TurretType.Laser));
            AvailableTurrets.Add(CreateTurret(TurretType.Missile));
            AvailableTurrets.Add(CreateTurret(TurretType.SlowMotion));
        }


        public Turret CreateTurret(TurretType type)
        {
            Turret t = null;

            switch (type)
            {
                case TurretType.Basic:              t = new BasicTurret(Simulation);            break;
                case TurretType.Gravitational:      t = new GravitationalTurret(Simulation);    break;
                case TurretType.MultipleLasers:     t = new MultipleLasersTurret(Simulation);   break;
                case TurretType.Laser:              t = new LaserTurret(Simulation);            break;
                case TurretType.Missile:            t = new MissileTurret(Simulation);         break;
                case TurretType.Alien:              t = new AlienTurret(Simulation);            break;
                case TurretType.SlowMotion:         t = new SlowMotionTurret(Simulation);       break;
                default:                            t = new BasicTurret(Simulation);            break;
            }

            return t;
        }
    }
}
