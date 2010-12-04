namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;


    class TourelleGravitationnelle : Tourelle
    {
        public float VitesseRotationAntenne;
        public float VitesseRotationBase;

        public TourelleGravitationnelle(Simulation simulation)
            : base(simulation)
        {
            this.Type = TypeTourelle.Gravitationnelle;
            this.Nom = "Gravitational";

            this.VitesseRotationAntenne = Main.Random.Next(-50, 50) / 1000f;
            this.VitesseRotationBase = 0;
            this.Couleur = new Color(202, 196, 255);
        }

        public override void Draw(GameTime gameTime)
        {
            representation.Rotation += VitesseRotationAntenne;
            representationBase.Rotation += VitesseRotationBase;

            base.Draw(gameTime);
        }

        public override float PrioriteAffichage
        {
            set
            {
                base.PrioriteAffichage = value;

                representation.PrioriteAffichage = representationBase.PrioriteAffichage - 0.001f;
            }
        }

        public override bool miseAJour()
        {
            if (base.miseAJour())
            {
                representation.Taille = 4;
                representationBase.Taille = 4;

                return true;
            }

            return false;
        }
    }
}