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

    class ProjectileMissile : Projectile
    {
        public float ZoneImpact;
        public Enemy Cible;
        private Particle Trainee;
        private double CompteurTrainee;
        private bool Orphelin;

        public override void Initialize()
        {
            base.Initialize();

            Shape = Shape.Rectangle;

            if (RepresentationVivant == null)
            {
                RepresentationVivant = new IVisible
                (
                    EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("ProjectileMissile1"),
                    position
                );

                Rectangle = new RectanglePhysique(RepresentationVivant.Rectangle);
            }

            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;
            RepresentationVivant.Position = Position;
            RepresentationVivant.Origine = RepresentationVivant.Centre;
            RepresentationVivant.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            RepresentationVivant.VisualPriority = PrioriteAffichage + 0.001f;

            MovingEffect = null;
            ExplodingEffect = Scene.Particules.Get("projectileMissileExplosion");
            ExplodingEffect.VisualPriority = Preferences.PrioriteSimulationTourelle - 0.001f;

            LifePoints = 5;

            Trainee = Scene.Particules.Get("traineeMissile");
            Trainee.VisualPriority = this.RepresentationVivant.VisualPriority - 0.0001f;

            ConeEmitter emetteur = (ConeEmitter)Trainee.ParticleEffect[0];
            emetteur.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            CompteurTrainee = 400;
            Orphelin = false;
        }


        public override void Update()
        {
            CompteurTrainee -= 16.66;

            Rectangle.set(ref RepresentationVivant.rectangle);

            if (!Orphelin && Cible.Alive)
            {
                Vector3 nouvelleDirection = Cible.Position - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
                Orphelin = true;

            Position += Speed * Direction;

            ConeEmitter emetteur = (ConeEmitter)Trainee.ParticleEffect[0];
            emetteur.Direction = (float)Math.Atan2(Direction.Y, Direction.X) - MathHelper.Pi;

            if (CompteurTrainee > 0)
                Trainee.Trigger(ref this.position);

            base.Update();
        }


        public override void Draw()
        {
            if (Cible.Alive)
                RepresentationVivant.Rotation = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);

            base.Draw();
        }

        public override void DoDie()
        {
            ((CircleEmitter)ExplodingEffect.ParticleEffect[1]).Radius = ZoneImpact;

            base.DoDie();

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMissileExplosion");

            Scene.Particules.Return(Trainee);

            Projectile.PoolProjectilesMissile.Return(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.Return(Trainee);

            Projectile.PoolProjectilesMissile.Return(this);
        }
    }
}