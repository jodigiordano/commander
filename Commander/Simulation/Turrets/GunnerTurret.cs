namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class GunnerTurret : Turret
    {
        public Bullet ActiveBullet;
        private Enemy EnemyAttacked;


        public GunnerTurret( Simulator simulator )
            : base( simulator )
        {
            Type = TurretType.Gunner;
            Name = "Gunner";
            SfxShooting = "sfxTourelleBase";
            Color = new Color( 57, 216, 17 );

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
