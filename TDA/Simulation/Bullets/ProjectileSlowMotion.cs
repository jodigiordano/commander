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

    class ProjectileSlowMotion : Projectile
    {
        private double DureeVie     { get; set; }
        public float Rayon          { get; set; }


        public override void Initialize()
        {
            base.Initialize();

            Vitesse = 0;
            Forme = Forme.Cercle;
            Cercle = new Cercle(Position, Rayon);
            Rectangle = new RectanglePhysique(Cercle.Rectangle);

            ExplodingEffect = Scene.Particules.recuperer("etincelleSlowMotion");
            ExplodingEffect.VisualPriority = Preferences.PrioriteSimulationTourelle - 0.001f;

            LifePoints = Int16.MaxValue;

            DureeVie = 1;
        }


        public override void Update()
        {
            Cercle.Position = this.Position;
            
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
            ((CircleEmitter)ExplodingEffect.ParticleEffect[0]).Radius = Cercle.Radius;
            ((RadialGravityModifier)ExplodingEffect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.Position.X, this.Position.Y);

            ExplodingEffect.Emettre(ref this.position);
            Scene.Particules.retourner(ExplodingEffect);

            Projectile.PoolProjectilesSlowMotion.retourner(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.retourner(ExplodingEffect);

            Projectile.PoolProjectilesSlowMotion.retourner(this);
        }
    }
}