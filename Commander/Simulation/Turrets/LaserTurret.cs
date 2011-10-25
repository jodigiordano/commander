namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class LaserTurret : Turret
    {
        public Bullet ActiveBullet;
        private IDestroyable EnemyAttacked;


        public LaserTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Laser;
            Name = @"Laser";
            Description = @"Shoot a laser";
            Color = new Color(255, 71, 187);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override IDestroyable EnemyWatched
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
