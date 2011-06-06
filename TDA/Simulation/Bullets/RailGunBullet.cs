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


    class RailGunBullet : Projectile
    {
        public float ZoneImpact;


        public override void Initialize()
        {
            base.Initialize();

            Shape = Shape.Rectangle;

            if (Rectangle == null)
                Rectangle = new RectanglePhysique((int) Position.X - 10, (int) Position.Y - 10, 20, 20);

            MovingEffect = Scene.Particules.Get("railgun");
            ExplodingEffect = Scene.Particules.Get("railgunExplosion");
            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;
            Rectangle.X = (int)Position.X;
            Rectangle.Y = (int)Position.Y;
            LifePoints = 5;
        }

        public override void DoDie()
        {
            base.DoDie();

            Projectile.PoolRailGunBullet.Return(this);
        }

        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Projectile.PoolRailGunBullet.Return(this);
        }

        public override void Update()
        {
            Position += Speed * Direction;
            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;

            base.Update();
        }
    }
}