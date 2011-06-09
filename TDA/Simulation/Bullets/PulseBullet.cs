namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;


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

            MovingEffect = Scene.Particles.Get("pulseEffect");
        }


        public override void DoHit(ILivingObject by) {}
    }
}