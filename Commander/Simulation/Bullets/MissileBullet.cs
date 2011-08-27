namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class MissileBullet : Bullet
    {
        public IDestroyable Target;
        public Image Image1;
        public Image Image2;
        public bool Wander;

        private Particle TrailEffect;
        private double TrailEffectCounter;


        public MissileBullet()
            : base()
        {
            Shape = Shape.Rectangle;
            Rectangle = new PhysicalRectangle();
            Explosive = true;
            SfxExplosion = @"sfxTourelleMissileExplosion";
        }


        public override void LoadAssets()
        {
            Image1 = new Image("ProjectileMissile1", Position);
            Image2 = new Image("ProjectileMissile2", Position)
            {
                SizeX = 2
            };

            Image = Image1;

            Vector2 imageSize = Image.AbsoluteSize;

            Rectangle.Width = (int) (imageSize.X);
            Rectangle.Height = (int) (imageSize.Y);

            base.LoadAssets();
        }


        public bool Big
        {
            set
            {
                Image = value ? Image2 : Image1;

                Vector2 imageSize = Image.AbsoluteSize;

                Rectangle.Width = (int) (imageSize.X);
                Rectangle.Height = (int) (imageSize.Y);
            }
        }


        public override void Initialize()
        {
            base.Initialize();

            Rectangle.X = (int) Position.X - Rectangle.Width;
            Rectangle.Y = (int) Position.Y - Rectangle.Height;

            Image.Position = Position;
            Image.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            Image.VisualPriority = VisualPriority + 0.001f;

            ExplodingEffect = Scene.Particles.Get(@"projectileMissileExplosion");
            ExplodingEffect.VisualPriority = VisualPriorities.Default.DefaultBullet;

            TrailEffect = Scene.Particles.Get(@"traineeMissile");
            TrailEffect.VisualPriority = Image.VisualPriority - 0.0001f;

            ConeEmitter emitter = (ConeEmitter)TrailEffect.ParticleEffect[0];
            emitter.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            LifePoints = 5;
            TrailEffectCounter = 400;
            Wander = false;
        }


        public override void Update()
        {
            TrailEffectCounter -= Preferences.TargetElapsedTimeMs;

            if (!Wander && Target.Alive)
            {
                Vector3 newDirection = Target.Position - this.Position;
                newDirection.Normalize();

                this.Direction = newDirection;
            }

            else
                Wander = true;

            Position += Speed * Direction;
            Rotation = (float) Math.Atan2(Direction.Y, Direction.X);

            ConeEmitter emitter = (ConeEmitter)TrailEffect.ParticleEffect[0];
            emitter.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            base.Update();
        }


        public override void Draw()
        {
            if (Target.Alive)
                Image.Rotation = MathHelper.Pi + Rotation;

            if (TrailEffectCounter > 0)
                TrailEffect.Trigger(ref this.position);

            base.Draw();
        }

        public override void DoDie()
        {
            ((CircleEmitter) ExplodingEffect.ParticleEffect[1]).Radius = ExplosionRange;

            Scene.Particles.Return(TrailEffect);

            base.DoDie();
        }


        public override void DoDieSilent()
        {
            Scene.Particles.Return(TrailEffect);

            base.DoDieSilent();
        }
    }
}