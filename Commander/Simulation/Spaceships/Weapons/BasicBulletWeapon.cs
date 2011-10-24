namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class BasicBulletWeapon : SpaceshipWeapon
    {
        private Matrix RotatioMatrix;


        public BasicBulletWeapon(Simulator simulator, Spaceship spaceship, double frequency, float attackPoints)
            : base(simulator, spaceship, frequency, attackPoints)
        {
            
        }


        protected override void DoFire()
        {
            Matrix.CreateRotationZ(MathHelper.PiOver2, out RotatioMatrix);
            Vector3 ParallelDirection = Vector3.Transform(Spaceship.Direction, RotatioMatrix);
            ParallelDirection.Normalize();

            Vector3 translation = ParallelDirection * Main.Random.Next(-12, 12);

            var p = (BasicBullet) Simulator.BulletsFactory.Get(BulletType.Base);

            p.Position = Spaceship.Position + translation;
            p.Direction = Spaceship.Direction;
            p.AttackPoints = AttackPoints;
            p.VisualPriority = VisualPriorities.Default.DefaultBullet;
            p.Speed = 10;
            p.Image.SizeX = 0.75f;
            p.ShowMovingEffect = false;
            p.Color = Spaceship.ShieldColor;

            Bullets.Add(p);
        }
    }
}