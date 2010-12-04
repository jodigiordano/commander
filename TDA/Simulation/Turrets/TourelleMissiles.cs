namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    class TourelleMissiles : Tourelle
    {
        public TourelleMissiles(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.Missile;
            this.Nom = "Missile";
            this.SfxTir = "sfxTourelleMissile";
            this.Couleur = new Color(25, 121, 255);
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


        public override float PrioriteAffichage
        {
            set
            {
                base.PrioriteAffichage = value;

                representation.PrioriteAffichage = representationBase.PrioriteAffichage - 0.001f;
            }
        }
    }
}