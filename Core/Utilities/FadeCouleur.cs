namespace Core.Utilities
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class FadeCouleur : Fade
    {

        private Color couleur;

        public FadeCouleur(Color couleur, Type type, double tempsFadeIn, double tempsFadeOut) : base(couleur.A, type, tempsFadeIn, tempsFadeOut) {
            this.couleur = couleur;
        }

        new public Color suivant(GameTime gameTime) {
            couleur.A = (byte) base.suivant(gameTime);

            return couleur;
        }

        new public Color init()
        {
            return couleur;
        }

        new public Color Final
        {
            get
            {
                couleur.A = (byte)base.Final;

                return couleur;
            }
        }
    }
}
