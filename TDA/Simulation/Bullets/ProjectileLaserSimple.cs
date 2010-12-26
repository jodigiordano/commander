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
        private double DureeVie                                     { get; set; }
        public Ennemi Cible                                         { get; set; }
        public Tourelle TourelleEmettrice                           { get; set; }
        private ParticuleEffectWrapper RepresentationVivantAlt      { get; set; }
        private LigneVisuel RepresentationVivantAlt2                { get; set; }


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

            RepresentationDeplacement = null;
            RepresentationExplose = null;

            PointsVie = Int16.MaxValue;

            DureeVie = 800;
        }


        public override void Update(GameTime gameTime)
        {
            if (Cible.EstVivant)
            {
                Vector3 nouvelleDirection = Cible.Position - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
            {
                PointsVie = 0;
                return;
            }

            Position = TourelleEmettrice.Position;

            Ligne.Debut = this.Position;
            Ligne.Fin = Cible.Position;

            DureeVie -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (DureeVie <= 0)
                PointsVie = 0;

            RepresentationVivantAlt.Emettre(ref this.position);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
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

            base.Draw(gameTime);
        }


        public override void doTouche(IObjetVivant par)
        {

        }


        public override void doMeurt()
        {
            base.doMeurt();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.retourner(this);
        }


        public override void doMeurtSilencieusement()
        {
            base.doMeurtSilencieusement();

            Scene.Particules.retourner(RepresentationVivantAlt);

            Projectile.PoolProjectilesLaserSimple.retourner(this);
        }
    }
}