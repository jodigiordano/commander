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

            RepresentationExplose = Scene.Particules.recuperer("etincelleSlowMotion");
            RepresentationExplose.PrioriteAffichage = Preferences.PrioriteSimulationTourelle - 0.001f;

            PointsVie = Int16.MaxValue;

            DureeVie = 1;
        }


        public override void Update(GameTime gameTime)
        {
            Cercle.Position = this.Position;
            
            Rectangle.X = (int)(this.Position.X - Rectangle.Width / 2);
            Rectangle.Y = (int)(this.Position.Y - Rectangle.Height / 2);

            DureeVie -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (DureeVie <= 0)
                PointsVie = 0;

            base.Update(gameTime);
        }


        public override void doTouche(IObjetVivant par) {}

        public override void doMeurt()
        {
            ((CircleEmitter)RepresentationExplose.ParticleEffect[0]).Radius = Cercle.Rayon;
            ((RadialGravityModifier)RepresentationExplose.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.Position.X, this.Position.Y);

            RepresentationExplose.Emettre(ref this.position);
            Scene.Particules.retourner(RepresentationExplose);

            Projectile.PoolProjectilesSlowMotion.retourner(this);
        }


        public override void doMeurtSilencieusement()
        {
            base.doMeurtSilencieusement();

            Scene.Particules.retourner(RepresentationExplose);

            Projectile.PoolProjectilesSlowMotion.retourner(this);
        }
    }
}