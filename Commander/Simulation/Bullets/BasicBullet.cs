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
            Rectangle = new PhysicalRectangle();
            Deflectable = true;
        }


        public override void LoadAssets()
        {
            Image = new Image("ProjectileBase", Position);

            base.LoadAssets();
        }


        public override void Initialize()
        {
            base.Initialize();

            Vector2 imageSize = Image.AbsoluteSize;

            Rectangle.Width = (int) (imageSize.Y / 2) + 4; //dimension == rectangle
            Rectangle.Height = (int) (imageSize.Y / 2) + 4;

            Rectangle.X = (int) (Position.X - Rectangle.Width);
            Rectangle.Y = (int) (Position.Y - Rectangle.Height);

            Image.Position = Position;
            Image.Rotation = (MathHelper.PiOver2) + (float) Math.Atan2(Direction.Y, Direction.X);

            MovingEffect = Scene.Particles.Get(@"projectileMissileDeplacement");
            ExplodingEffect = Scene.Particles.Get(@"projectileBaseExplosion");

            Image.VisualPriority = VisualPriority + 0.001f;
            MovingEffect.VisualPriority = VisualPriority + 0.001f;
            ExplodingEffect.VisualPriority = VisualPriorities.Default.DefaultBullet;

            LifePoints = 5;
            Deflected = false;
        }


        public override void Update()
        {
            Position += Speed * Direction;
            Rotation = (float) Math.Atan2(Direction.Y, Direction.X);

            base.Update();
        }
    }
}