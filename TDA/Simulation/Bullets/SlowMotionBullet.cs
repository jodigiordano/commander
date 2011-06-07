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
    using ProjectMercury.Modifiers;

    class SlowMotionBullet : Bullet
    {
        private double DureeVie     { get; set; }
        public float Rayon          { get; set; }


        public override void Initialize()
        {
            base.Initialize();

            Speed = 0;
            Shape = Shape.Circle;
            Circle = new Cercle(Position, Rayon);
            Rectangle = new RectanglePhysique(Circle.Rectangle);

            ExplodingEffect = Scene.Particles.Get("etincelleSlowMotion");
            ExplodingEffect.VisualPriority = Preferences.PrioriteSimulationTourelle - 0.001f;

            LifePoints = Int16.MaxValue;

            DureeVie = 1;
        }


        public override void Update()
        {
            Circle.Position = this.Position;
            
            Rectangle.X = (int)(this.Position.X - Rectangle.Width / 2);
            Rectangle.Y = (int)(this.Position.Y - Rectangle.Height / 2);

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void DoHit(ILivingObject par) {}

        public override void DoDie()
        {
            ((CircleEmitter)ExplodingEffect.ParticleEffect[0]).Radius = Circle.Radius;
            ((RadialGravityModifier)ExplodingEffect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.Position.X, this.Position.Y);

            ExplodingEffect.Trigger(ref this.position);
            Scene.Particles.Return(ExplodingEffect);

            Bullet.PoolSlowMotionBullets.Return(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particles.Return(ExplodingEffect);

            Bullet.PoolSlowMotionBullets.Return(this);
        }
    }
}