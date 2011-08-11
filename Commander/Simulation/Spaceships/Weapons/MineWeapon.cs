namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class MineWeapon : SpaceshipWeapon
    {
        private Matrix RotatioMatrix;


        public MineWeapon(Simulator simulator, Spaceship spaceship)
            : base(simulator, spaceship, double.MaxValue, 70)
        {
            
        }


        protected override void DoFire()
        {
            MineBullet mb = (MineBullet) Simulator.BulletsFactory.Get(BulletType.Mine);

            mb.Position = Spaceship.Position;
            mb.AttackPoints = AttackPoints;
            mb.VisualPriority = VisualPriorities.Default.MineBullet;
            mb.Speed = 0;
            mb.ExplosionRange = AttackPoints;

            Bullets.Add(mb);
        }
    }
}
