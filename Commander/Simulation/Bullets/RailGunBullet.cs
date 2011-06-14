namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class RailGunBullet : Bullet
    {
        public RailGunBullet()
            : base()
        {
            Shape = Shape.Rectangle;
            Rectangle = new RectanglePhysique();
            Explosive = true;
        }


        public override void Initialize()
        {
            base.Initialize();

            Rectangle.set((int) Position.X - 10, (int) Position.Y - 10, 20, 20);

            MovingEffect = Scene.Particles.Get(@"railgun");
            ExplodingEffect = Scene.Particles.Get(@"railgunExplosion");

            MovingEffect.VisualPriority = VisualPriority + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;
            
            LifePoints = 5;
        }


        public override void Update()
        {
            Position += Speed * Direction;
            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;

            base.Update();
        }


        public override void DoDie()
        {
            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", "sfxRailGunExplosion2");
            
            base.DoDie();
        }
    }
}