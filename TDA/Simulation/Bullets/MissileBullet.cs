namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class MissileBullet : Bullet
    {
        public float ZoneImpact;
        public Enemy Target;
        public Image Image1;
        public Image Image2;
        public bool Wander;

        private Particle TrailEffect;
        private double TrailEffectCounter;


        public override void Initialize()
        {
            base.Initialize();

            Shape = Shape.Rectangle;

            if (Image == null)
            {
                Image1 = new Image("ProjectileMissile1", Position);
                Image2 = new Image("ProjectileMissile2", Position)
                {
                    SizeX = 2
                };
                Image = Image1;

                Rectangle = new RectanglePhysique(
                    (int) (Image.Position.X - Image.AbsoluteSize.X),
                    (int) (Image.Position.Y - Image.AbsoluteSize.Y),
                    (int) (Image.AbsoluteSize.X * 2),
                    (int) (Image.AbsoluteSize.X * 2));
            }

            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;
            Image.Position = Position;
            Image.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            Image.VisualPriority = PrioriteAffichage + 0.001f;

            MovingEffect = null;
            ExplodingEffect = Scene.Particles.Get("projectileMissileExplosion");
            ExplodingEffect.VisualPriority = Preferences.PrioriteSimulationTourelle - 0.001f;

            LifePoints = 5;

            TrailEffect = Scene.Particles.Get("traineeMissile");
            TrailEffect.VisualPriority = this.Image.VisualPriority - 0.0001f;

            ConeEmitter emetteur = (ConeEmitter)TrailEffect.ParticleEffect[0];
            emetteur.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            TrailEffectCounter = 400;
            Wander = false;
        }


        public override void Update()
        {
            TrailEffectCounter -= 16.66;

            if (!Wander && Target.Alive)
            {
                Vector3 newDirection = Target.Position - this.Position;
                newDirection.Normalize();

                this.Direction = newDirection;
            }

            else
                Wander = true;

            Position += Speed * Direction;

            ConeEmitter emitter = (ConeEmitter)TrailEffect.ParticleEffect[0];
            emitter.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            if (TrailEffectCounter > 0)
                TrailEffect.Trigger(ref this.position);

            base.Update();
        }


        public override void Draw()
        {
            if (Target.Alive)
                Image.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);

            base.Draw();
        }

        public override void DoDie()
        {
            ((CircleEmitter)ExplodingEffect.ParticleEffect[1]).Radius = ZoneImpact;

            base.DoDie();

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMissileExplosion");

            Scene.Particles.Return(TrailEffect);

            Bullet.PoolMissileBullets.Return(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particles.Return(TrailEffect);

            Bullet.PoolMissileBullets.Return(this);
        }
    }
}