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

            RepresentationDeplacement = null;
            RepresentationExplose = null;

            PointsVie = Int16.MaxValue;

            DureeVie = 800;

            Orphelin = false;
        }


        public override void Update(GameTime gameTime)
        {
            Position = TourelleEmettrice.Position + CibleOffset;

            if (!Orphelin && Cible.EstVivant)
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

            DureeVie -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (DureeVie <= 0)
                PointsVie = 0;

            RepresentationVivantAlt.Emettre(ref this.position);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (LONGUEUR_LIGNE/2), this.Direction.Y * (LONGUEUR_LIGNE/2));

            RepresentationVivantAlt2.Debut = Ligne.Debut;
            RepresentationVivantAlt2.Fin = Ligne.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw(gameTime);
        }


        public override void doTouche(IObjetVivant par) {}


        public override void doMeurt()
        {
            base.doMeurt();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserMultiple.retourner(this);
        }

        public override void doMeurtSilencieusement()
        {
            base.doMeurtSilencieusement();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserMultiple.retourner(this);
        }
    }
}