namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;


    class TourelleSlowMotion : Tourelle
    {
        public float VitesseRotationCanon;
        private float AncienneRotation;

        public TourelleSlowMotion(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.SlowMotion;
            this.Nom = "Slow";
            this.SfxTir = "sfxTourelleSlowMotion";
            this.Couleur = new Color(255, 216, 0);

            this.VitesseRotationCanon = Main.Random.Next(-50, 50) / 1000f;
            this.AncienneRotation = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            representation.Rotation = AncienneRotation + VitesseRotationCanon;
            AncienneRotation = representation.Rotation;
        }

        public override bool miseAJour()
        {
            if (base.miseAJour())
            {
                representation.Taille = 4;
                representationBase.Taille = 4;
                representation.Origine = representation.Centre;
                return true;
            }

            return false;
        }
    }
}