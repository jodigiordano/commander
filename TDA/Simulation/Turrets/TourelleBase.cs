namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;


    class TourelleBase : Tourelle
    {
        public TourelleBase(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.Base;
            this.Nom = "Basic";
            this.SfxTir = "sfxTourelleBase";
            this.Couleur = new Color(57, 216, 17);
        }

        public override bool miseAJour()
        {
            if (base.miseAJour())
            {
                representation.Origine = new Vector2(24, 36);
                return true;
            }

            return false;
        }
    }
}