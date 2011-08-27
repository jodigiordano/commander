namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class GunnerTurret : Turret
    {
        public Bullet ActiveBullet;
        private IDestroyable EnemyAttacked;


        public GunnerTurret( Simulator simulator )
            : base( simulator )
        {
            Type = TurretType.Gunner;
            Name = @"Gunner";
            Description = @"Shoot a lot of bullets at in insane rate";
            SfxShooting = @"sfxTourelleBase";
            Color = new Color( 57, 216, 17 );

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
