namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Persistance;
    using Core.Physique;

    class ProjectileBase : Projectile
    {
        public override void Initialize()
        {
            base.Initialize();

            Vitesse = 10;
            Forme = Forme.Rectangle;

            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible
                (
                    Core.Persistance.Facade.recuperer<Texture2D>("ProjectileBase"),
                    Position
                );

                Rectangle = new RectanglePhysique(RepresentationVivant.Rectangle);
            }

            RepresentationVivant.Position = Position;
            RepresentationVivant.Origine = RepresentationVivant.Centre;
            RepresentationVivant.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);
            RepresentationDeplacement = Scene.Particules.recuperer("projectileMissileDeplacement");
            RepresentationExplose = Scene.Particules.recuperer("projectileBaseExplosion");
            RepresentationVivant.PrioriteAffichage = PrioriteAffichage + 0.001f;
            RepresentationDeplacement.PrioriteAffichage = PrioriteAffichage + 0.001f;
            RepresentationExplose.PrioriteAffichage = 0.35f;
            Rectangle.X = (int)Position.X;
            Rectangle.Y = (int)Position.Y;
            PointsVie = 5;
        }

        public override void doMeurt()
        {
            base.doMeurt();

            Projectile.PoolProjectilesBase.retourner(this);
        }

        public override void doMeurtSilencieusement()
        {
            base.doMeurtSilencieusement();

            Projectile.PoolProjectilesBase.retourner(this);
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle.set(ref RepresentationVivant.rectangle);
            Position += Vitesse * Direction;

            base.Update(gameTime);
        }
    }
}