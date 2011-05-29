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

    class ProjectileLaserMultiple : Projectile
    {
        private const int LONGUEUR_LIGNE = 1500;

        private double DureeVie;
        public Ennemi Cible;
        public Vector3 CibleOffset;
        public Turret TourelleEmettrice;
        private ParticuleEffectWrapper RepresentationVivantAlt;
        private LigneVisuel RepresentationVivantAlt2;
        private bool Orphelin;

        public override void Initialize()
        {
            base.Initialize();

            Position = TourelleEmettrice.Position + CibleOffset;
            Vitesse = 0;
            Forme = Forme.Ligne;
            RepresentationVivant = null;

            Ligne = new Ligne(this.Position, this.Position + this.Direction * LONGUEUR_LIGNE);

            RepresentationVivantAlt = Scene.Particules.recuperer("projectileLaserMultiple");
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

            Ligne.Debut = this.Position;
            Ligne.Fin = this.Position + this.Direction * LONGUEUR_LIGNE;

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            RepresentationVivantAlt.Emettre(ref this.position);

            base.Update();
        }


        public override void Draw()
        {
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (LONGUEUR_LIGNE/2), this.Direction.Y * (LONGUEUR_LIGNE/2));

            RepresentationVivantAlt2.Debut = Ligne.Debut;
            RepresentationVivantAlt2.Fin = Ligne.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par) {}


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserMultiple.retourner(this);
        }

        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserMultiple.retourner(this);
        }
    }
}