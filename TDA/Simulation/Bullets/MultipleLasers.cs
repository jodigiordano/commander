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

    class MultipleLasersBullet : Bullet
    {
        private const int LONGUEUR_LIGNE = 1500;

        private double DureeVie;
        public Enemy Cible;
        public Vector3 CibleOffset;
        public Turret TourelleEmettrice;
        private Particle RepresentationVivantAlt;
        private LigneVisuel RepresentationVivantAlt2;
        private bool Orphelin;

        public override void Initialize()
        {
            base.Initialize();

            Position = TourelleEmettrice.Position + CibleOffset;
            Speed = 0;
            Shape = Shape.Line;
            Image = null;

            Line = new Ligne(this.Position, this.Position + this.Direction * LONGUEUR_LIGNE);

            RepresentationVivantAlt = Scene.Particles.Get("projectileLaserMultiple");
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Length = LONGUEUR_LIGNE;
            RepresentationVivantAlt.VisualPriority = PrioriteAffichage + 0.001f;

            RepresentationVivantAlt2 = new LigneVisuel(this.Position, this.Position + this.Direction * LONGUEUR_LIGNE, new Color(255, 216, 0, 100), 4);
            RepresentationVivantAlt2.VisualPriority = PrioriteAffichage + 0.002f;

            MovingEffect = null;
            ExplodingEffect = null;

            LifePoints = Int16.MaxValue;

            DureeVie = 800;

            Orphelin = false;
        }


        public override void Update()
        {
            Position = TourelleEmettrice.Position + CibleOffset;

            if (!Orphelin && Cible.Alive)
            {
                Vector3 nouvelleDirection = (Cible.Position + CibleOffset) - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
            {
                Orphelin = true;
            }

            Line.Debut = this.Position;
            Line.Fin = this.Position + this.Direction * LONGUEUR_LIGNE;

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            RepresentationVivantAlt.Trigger(ref this.position);

            base.Update();
        }


        public override void Draw()
        {
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (LONGUEUR_LIGNE/2), this.Direction.Y * (LONGUEUR_LIGNE/2));

            RepresentationVivantAlt2.Debut = Line.Debut;
            RepresentationVivantAlt2.Fin = Line.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par) {}


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particles.Return(RepresentationVivantAlt);

            Bullet.PoolMultipleLasersBullets.Return(this);
        }

        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particles.Return(RepresentationVivantAlt);

            Bullet.PoolMultipleLasersBullets.Return(this);
        }
    }
}