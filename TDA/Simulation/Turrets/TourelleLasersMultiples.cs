namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class TourelleLasersMultiples : Tourelle
    {
        public TourelleLasersMultiples(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.LaserMultiple;
            this.Nom = "Multi-laser";
            this.SfxTir = "sfxTourelleLaserMultiple";
            this.Couleur = new Color(255, 96, 28);
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

                representation.VisualPriority = representationBase.VisualPriority - 0.00001f;
            }
        }
    }
}
