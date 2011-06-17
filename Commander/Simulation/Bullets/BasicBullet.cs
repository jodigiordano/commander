namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class BasicBullet : Bullet
    {
        public BasicBullet()
            : base()
        {
            Shape = Shape.Rectangle;
            Rectangle = new RectanglePhysique();
        }


        public override void LoadAssets()
        {
            Image = new Image("ProjectileBase", Position);

            Vector2 imageSize = Image.AbsoluteSize;

            Rectangle.Width = (int) (imageSize.X);
            Rectangle.Height = (int) (imageSize.Y);

            base.LoadAssets();
        }


        public override void Initialize()
        {
            base.Initialize();

            Rectangle.X = (int) (Position.X - Rectangle.Width);
            Rectangle.Y = (int) (Position.Y - Rectangle.Height);

            Image.Position = Position;
            Image.Rotation = (MathHelper.PiOver2) + (float) Math.Atan2(Direction.Y, Direction.X);

            MovingEffect = Scene.Particles.Get(@"projectileMissileDeplacement");
            ExplodingEffect = Scene.Particles.Get(@"projectileBaseExplosion");

            Image.VisualPriority = VisualPriority + 0.001f;
            MovingEffect.VisualPriority = VisualPriority + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;

            LifePoints = 5;
        }


        public override void Update()
        {
            Position += Speed * Direction;

            base.Update();
        }
    }
}