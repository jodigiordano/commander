namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class SlowMotionBullet : Bullet
    {
        public float Radius;
        private double LifeSpan;


        public SlowMotionBullet()
            : base()
        {
            Speed = 0;
            Shape = Shape.Circle;
            Circle = new Circle(Vector3.Zero, 0);
            Rectangle = new RectanglePhysique();
        }


        public override void Initialize()
        {
            base.Initialize();

            Circle.Position = Position;
            Circle.Radius = Radius;

            Rectangle r = Circle.Rectangle;

            Rectangle.set(ref r);

            ExplodingEffect = Scene.Particles.Get(@"etincelleSlowMotion");
            ExplodingEffect.VisualPriority = Preferences.PrioriteSimulationTourelle - 0.001f;

            LifePoints = Int16.MaxValue;
            LifeSpan = 1;
        }


        public override void Update()
        {
            Circle.Position = this.Position;
            
            Rectangle.X = (int)(this.Position.X - Rectangle.Width / 2);
            Rectangle.Y = (int)(this.Position.Y - Rectangle.Height / 2);

            LifeSpan -= Preferences.TargetElapsedTimeMs;

            if (LifeSpan <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void DoHit(ILivingObject par) {}


        public override void DoDie()
        {
            ((CircleEmitter)ExplodingEffect.ParticleEffect[0]).Radius = Circle.Radius;
            ((RadialGravityModifier)ExplodingEffect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.Position.X, this.Position.Y);

            base.DoDie();
        }
    }
}