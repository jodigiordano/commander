namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;


    class PulseBullet : Bullet
    {
        public PulseBullet()
            : base()
        {
            Rectangle = new RectanglePhysique();
        }


        public override void Initialize()
        {
            base.Initialize();
         
            Rectangle.set((int) Position.X - 10, (int) Position.Y - 10, 20, 20);

            MovingEffect = Scene.Particles.Get(@"pulseEffect");
            MovingEffect.VisualPriority = VisualPriority;
        }


        public override void DoHit(ILivingObject by) {}
    }
}