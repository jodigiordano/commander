namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class GunnerBullet : Bullet
    {
        public Enemy Target;
        public Turret Turret;
        private Particle TurretEffect;
        private Matrix CanonRotation;


        public GunnerBullet()
            : base()
        {
            Speed = 0;
            Rectangle = new RectanglePhysique();
            CanonRotation = new Matrix();
        }


        public override void Initialize()
        {
            base.Initialize();

            Position = Target.Position;

            Rectangle.set((int) Position.X - 10, (int) Position.Y - 10, 20, 20);
            
            ExplodingEffect = Scene.Particles.Get("projectileBaseExplosion");
            ExplodingEffect.VisualPriority = Target.Image.VisualPriority - 0.0001f;

            TurretEffect = Scene.Particles.Get("gunnerTurret");
            TurretEffect.ParticleEffect[0].ReleaseColour = Turret.Color.ToVector3();
            TurretEffect.VisualPriority = Turret.VisualPriority + 0.0001f;

            LifePoints = Int16.MaxValue;
        }


        public override void Update()
        {
            if (!Target.Alive)
            {
                LifePoints = 0;
                return;
            }

            int radius = (int) Target.Circle.Radius;

            Position = Target.Position + new Vector3(
                Main.Random.Next(radius, radius),
                Main.Random.Next(radius, radius),
                0);

            base.Update();
        }


        public override void Draw()
        {
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
            TurretEffect.Trigger(ref turretPosition);

            base.Draw();
        }


        public override void DoDie()
        {
            Scene.Particles.Return(TurretEffect);

            base.DoDie();
        }


        public override void DoDieSilent()
        {
            Scene.Particles.Return(TurretEffect);

            base.DoDieSilent();
        }
    }
}