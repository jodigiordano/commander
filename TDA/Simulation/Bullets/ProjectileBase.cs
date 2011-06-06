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

    class ProjectileBase : Projectile
    {
        public override void Initialize()
        {
            base.Initialize();

            Shape = Shape.Rectangle;

            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible
                (
                    EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("ProjectileBase"),
                    Position
                );

                Rectangle = new RectanglePhysique(RepresentationVivant.Rectangle);
            }

            RepresentationVivant.Position = Position;
            RepresentationVivant.Origine = RepresentationVivant.Centre;
            RepresentationVivant.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);
            MovingEffect = Scene.Particules.Get("projectileMissileDeplacement");
            ExplodingEffect = Scene.Particules.Get("projectileBaseExplosion");
            RepresentationVivant.VisualPriority = PrioriteAffichage + 0.001f;
            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;
            Rectangle.X = (int)Position.X;
            Rectangle.Y = (int)Position.Y;
            LifePoints = 5;
        }

        public override void DoDie()
        {
            base.DoDie();

            Projectile.PoolProjectilesBase.Return(this);
        }

        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Projectile.PoolProjectilesBase.Return(this);
        }

        public override void Update()
        {
            Rectangle.set(ref RepresentationVivant.rectangle);
            Position += Speed * Direction;

            base.Update();
        }
    }
}