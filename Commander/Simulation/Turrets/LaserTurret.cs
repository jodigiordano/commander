namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class LaserTurret : Turret
    {
        public Bullet ActiveBullet;
        private Enemy EnemyAttacked;


        public LaserTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Laser;
            Name = @"Laser";
            Description = @"Shoot a laser that always reach it's target";
            SfxShooting = @"sfxTourelleLaserSimple";
            Color = new Color(255, 71, 187);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

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
