namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Persistance;
    using EphemereGames.Core.Physique;


    class BasicBullet : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            Shape = Shape.Rectangle;

            if (Image == null)
            {
                Image = new Image("ProjectileBase", Position);

                Rectangle = new RectanglePhysique(
                    (int) (Image.Position.X - Image.AbsoluteSize.X),
                    (int) (Image.Position.Y - Image.AbsoluteSize.Y),
                    (int) (Image.AbsoluteSize.X * 2),
                    (int) (Image.AbsoluteSize.X * 2));
            }

            Image.Position = Position;
            Image.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);
            MovingEffect = Scene.Particles.Get("projectileMissileDeplacement");
            ExplodingEffect = Scene.Particles.Get("projectileBaseExplosion");
            Image.VisualPriority = PrioriteAffichage + 0.001f;
            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;
            Rectangle.X = (int)Position.X;
            Rectangle.Y = (int)Position.Y;
            LifePoints = 5;
        }

        public override void DoDie()
        {
            base.DoDie();

            Bullet.PoolBasicBullets.Return(this);
        }

        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Bullet.PoolBasicBullets.Return(this);
        }

        public override void Update()
        {
            Position += Speed * Direction;

            base.Update();
        }
    }
}