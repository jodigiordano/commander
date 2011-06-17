namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;


    class ShieldBullet : Bullet
    {
        public ShieldBullet()
            : base()
        {
            Shape = Shape.Rectangle;
            Rectangle = new RectanglePhysique();
        }


        public override void Initialize()
        {
            base.Initialize();

            Rectangle.set((int) Position.X - 10, (int) Position.Y - 10, 40, 40);

            MovingEffect = Scene.Particles.Get(@"shieldEffect");
        }


        public override void DoHit(ILivingObject by) {}
    }
}