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
    using ProjectMercury.Emitters;


    class GunnerBullet : Projectile
    {
        public Ennemi Target;
        public Turret Turret;
        private ParticuleEffectWrapper ExplodingEffectAlt;
        private ParticuleEffectWrapper TurretEffect;
        private Matrix CanonRotation;


        public override void Initialize()
        {
            base.Initialize();

            Vitesse = 0;
            RepresentationVivant = null;

            Position = Target.Position;

            Rectangle = new RectanglePhysique((int) Position.X - 10, (int) Position.Y - 10, 20, 20);
            CanonRotation = new Matrix();

            ExplodingEffectAlt = Scene.Particules.recuperer("projectileBaseExplosion");
            ExplodingEffectAlt.VisualPriority = Target.RepresentationVivant.VisualPriority - 0.0001f;

            TurretEffect = Scene.Particules.recuperer("gunnerTurret");
            TurretEffect.ParticleEffect[0].ReleaseColour = Turret.Color.ToVector3();
            TurretEffect.VisualPriority = Turret.VisualPriority + 0.0001f;

            MovingEffect = null;
            ExplodingEffect = null;

            LifePoints = Int16.MaxValue;
        }


        public override void Update()
        {
            if (!Target.Alive)
            {
                LifePoints = 0;
                return;
            }

            int radius = (int) Target.Cercle.Radius;

            Position = Target.Position + new Vector3(
                Main.Random.Next(radius, radius),
                Main.Random.Next(radius, radius),
                0);

            base.Update();
        }


        public override void Draw()
        {
            // emits explosion at target's position
            ExplodingEffectAlt.Emettre(ref this.position);

            // emits canon explosion
            Vector2 canonEdge = new Vector2(0, -Turret.CanonImage.TextureSize.Y * Turret.CanonImage.Size.Y);
            Matrix.CreateRotationZ(Turret.CanonImage.Rotation, out CanonRotation);
            Vector2.Transform(ref canonEdge, ref CanonRotation, out canonEdge);

            canonEdge.X += Turret.CanonImage.Position.X;
            canonEdge.Y += Turret.CanonImage.Position.Y;

            // emits canon explosion - explosion direction
            Vector2 targetPosition = new Vector2(Target.Position.X, Target.Position.Y);
            Vector2.Subtract(ref targetPosition, ref canonEdge, out targetPosition);
            ((ConeEmitter) TurretEffect.ParticleEffect[0]).Direction = (float) Math.Atan2(targetPosition.Y, targetPosition.X);

            //TurretEffect.Emettre(ref canonEdge);
            Vector2 turretPosition = new Vector2(Turret.Position.X, Turret.Position.Y);
            TurretEffect.Emettre(ref turretPosition);

            base.Draw();
        }


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particules.retourner(ExplodingEffectAlt);

            Projectile.PoolGunnerBullets.retourner(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.retourner(ExplodingEffectAlt);

            Projectile.PoolGunnerBullets.retourner(this);
        }
    }
}