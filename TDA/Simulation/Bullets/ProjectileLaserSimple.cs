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
        public Ennemi Cible;
        public Turret TourelleEmettrice;
        private ParticuleEffectWrapper RepresentationVivantAlt;
        private LigneVisuel RepresentationVivantAlt2;


        public override void Initialize()
        {
            base.Initialize();

            Position = TourelleEmettrice.Position;
            Vitesse = 0;
            Forme = Forme.Ligne;
            RepresentationVivant = null;

            Ligne = new Ligne(this.Position, Cible.Position);

            RepresentationVivantAlt = Scene.Particules.recuperer("projectileLaserSimple");
            RepresentationVivantAlt.VisualPriority = PrioriteAffichage + 0.001f;
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Length = Ligne.Longueur;
            emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[1];
            emetteur.Length = Ligne.Longueur;

            RepresentationVivantAlt2 = new LigneVisuel(Ligne.Debut, Ligne.Fin, new Color(255, 0, 110), 4);
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

            Ligne.Debut = this.Position;
            Ligne.Fin = Cible.Position;

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void Draw()
        {
            if (Alive)
                RepresentationVivantAlt.Emettre(ref this.position);

            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            LineEmitter emetteur2 = (LineEmitter)RepresentationVivantAlt.ParticleEffect[1];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.Length = (Cible.Position - TourelleEmettrice.Position).Length();
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (emetteur.Length / 2), this.Direction.Y * (emetteur.Length / 2));
            emetteur2.Angle = emetteur.Angle;
            emetteur2.Length = emetteur.Length;
            emetteur2.TriggerOffset = emetteur.TriggerOffset;

            RepresentationVivantAlt2.Debut = Ligne.Debut;
            RepresentationVivantAlt2.Fin = Ligne.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par)
        {

        }


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.retourner(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.retourner(this);
        }
    }
}