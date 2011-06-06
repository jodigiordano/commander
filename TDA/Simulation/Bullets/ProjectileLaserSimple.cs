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

    class ProjectileLaserSimple : Projectile
    {
        private double DureeVie;
        public Enemy Cible;
        public Turret TourelleEmettrice;
        private Particle RepresentationVivantAlt;
        private LigneVisuel RepresentationVivantAlt2;


        public override void Initialize()
        {
            base.Initialize();

            Position = TourelleEmettrice.Position;
            Speed = 0;
            Shape = Shape.Line;
            RepresentationVivant = null;

            Line = new Ligne(this.Position, Cible.Position);

            RepresentationVivantAlt = Scene.Particules.Get("projectileLaserSimple");
            RepresentationVivantAlt.VisualPriority = PrioriteAffichage + 0.001f;
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Length = Line.Longueur;
            emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[1];
            emetteur.Length = Line.Longueur;

            RepresentationVivantAlt2 = new LigneVisuel(Line.Debut, Line.Fin, new Color(255, 0, 110), 4);
            RepresentationVivantAlt2.VisualPriority = PrioriteAffichage + 0.002f;

            MovingEffect = null;
            ExplodingEffect = null;

            LifePoints = Int16.MaxValue;

            DureeVie = 800;
        }


        public override void Update()
        {
            if (Cible.Alive)
            {
                Vector3 nouvelleDirection = Cible.Position - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
            {
                LifePoints = 0;
                return;
            }

            Position = TourelleEmettrice.Position;

            Line.Debut = this.Position;
            Line.Fin = Cible.Position;

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void Draw()
        {
            if (Alive)
                RepresentationVivantAlt.Trigger(ref this.position);

            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            LineEmitter emetteur2 = (LineEmitter)RepresentationVivantAlt.ParticleEffect[1];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.Length = (Cible.Position - TourelleEmettrice.Position).Length();
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (emetteur.Length / 2), this.Direction.Y * (emetteur.Length / 2));
            emetteur2.Angle = emetteur.Angle;
            emetteur2.Length = emetteur.Length;
            emetteur2.TriggerOffset = emetteur.TriggerOffset;

            RepresentationVivantAlt2.Debut = Line.Debut;
            RepresentationVivantAlt2.Fin = Line.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par)
        {

        }


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particules.Return(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.Return(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.Return(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.Return(this);
        }
    }
}