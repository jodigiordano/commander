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

    class ProjectileMissile : Projectile
    {
        public Cercle ZoneImpact;
        public Ennemi Cible;
        private ParticuleEffectWrapper Trainee;
        private double CompteurTrainee;
        private bool Orphelin;

        public override void Initialize()
        {
            base.Initialize();

            Forme = Forme.Rectangle;

            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible
                (
                    Core.Persistance.Facade.recuperer<Texture2D>("ProjectileMissile1"),
                    position,
                    Scene
                );

                Rectangle = new RectanglePhysique(RepresentationVivant.Rectangle);
            }

            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;
            RepresentationVivant.Position = Position;
            RepresentationVivant.Scene = Scene;
            RepresentationVivant.Origine = RepresentationVivant.Centre;
            RepresentationVivant.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            RepresentationVivant.PrioriteAffichage = PrioriteAffichage + 0.001f;

            RepresentationDeplacement = null;
            RepresentationExplose = Scene.Particules.recuperer("projectileMissileExplosion");
            RepresentationExplose.PrioriteAffichage = Preferences.PrioriteSimulationTourelle - 0.001f;

            PointsVie = 5;

            Trainee = Scene.Particules.recuperer("traineeMissile");
            Trainee.PrioriteAffichage = this.RepresentationVivant.PrioriteAffichage - 0.0001f;

            ConeEmitter emetteur = (ConeEmitter)Trainee.ParticleEffect[0];
            emetteur.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            CompteurTrainee = 400;
            Orphelin = false;
        }

        //public ProjectileMissile(Simulation simulation, Vector3 position, Ennemi cible, float pointsAttaque, float prioriteAffichage)
        //    : base(simulation, position, cible.Position - position)
        //{

        //}


        public override void Update(GameTime gameTime)
        {
            CompteurTrainee -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Rectangle.set(ref RepresentationVivant.rectangle);

            if (!Orphelin && Cible.EstVivant)
            {
                Vector3 nouvelleDirection = Cible.Position - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
                Orphelin = true;

            Position += Vitesse * Direction;

            ZoneImpact.Position = this.Position;

            ConeEmitter emetteur = (ConeEmitter)Trainee.ParticleEffect[0];
            emetteur.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            if (CompteurTrainee > 0)
                Trainee.Emettre(ref this.position);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (Cible.EstVivant)
                RepresentationVivant.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);

            base.Draw(gameTime);
        }

        public override void doMeurt()
        {
            ((CircleEmitter)RepresentationExplose.ParticleEffect[1]).Radius = ZoneImpact.Rayon;

            base.doMeurt();

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMissileExplosion");

            Scene.Particules.retourner(Trainee);

            Projectile.PoolProjectilesMissile.retourner(this);
        }


        public override void doMeurtSilencieusement()
        {
            base.doMeurtSilencieusement();

            Scene.Particules.retourner(Trainee);

            Projectile.PoolProjectilesMissile.retourner(this);
        }
    }
}