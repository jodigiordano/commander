namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class TourelleLasersSimples : Tourelle
    {
        public Projectile ProjectileEnCours;

        private Ennemi ennemiAttaque;
        public override Ennemi EnnemiAttaque
        {
            get
            {
                return ennemiAttaque;
            }
            set
            {
                if (ProjectileEnCours != null && ProjectileEnCours.EstVivant)
                    return;

                if (ProjectileEnCours != null && !ProjectileEnCours.EstVivant)
                    ProjectileEnCours = null;

                ennemiAttaque = value;
            }
        }


        public TourelleLasersSimples(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.LaserSimple;
            this.Nom = "Laser";
            this.SfxTir = "sfxTourelleLaserSimple";
            this.Couleur = new Color(255, 71, 187);
        }

        public override bool miseAJour()
        {
            if (base.miseAJour())
            {
                representation.Origine = new Vector2(6, 8);
                representation.Taille = 4;
                representationBase.Taille = 4;

                return true;
            }

            return false;
        }
    }
}
