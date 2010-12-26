namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LigneVisuel : IScenable
    {
        public Vector3 Position             { get; set; }
        public float VisualPriority      { get; set; }
        public Scene Scene                  { get; set; }
        public TypeBlend Blend          { get; set; }
        public List<IScenable> Components   { get; set; }

        public Color Couleur                { get; set; }
        public Vector3 Debut                { get; set; }
        public Vector3 Fin                  { get; set; }

        public int Tickness                 { get; set; }

        private static Matrix MatriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);


        public LigneVisuel(Vector2 debut, Vector2 fin, Color couleur)
        {
            VisualPriority = 0;
            Blend = TypeBlend.Default;
            Couleur = couleur;

            Debut = new Vector3(debut, 0);
            Fin = new Vector3(fin, 0);

            Tickness = 1;
        }


        public LigneVisuel(Vector3 debut, Vector3 fin, Color couleur)
        {
            VisualPriority = 0;
            Blend = TypeBlend.Default;
            Couleur = couleur;

            Debut = debut;
            Fin = fin;

            Tickness = 1;
        }

        public LigneVisuel(Vector3 debut, Vector3 fin, Color couleur, int tickness)
        {
            VisualPriority = 0;
            Blend = TypeBlend.Default;
            Couleur = couleur;

            Debut = debut;
            Fin = fin;

            Tickness = tickness;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            int offsetDebut = -Tickness / 2;

            Vector3 direction = Fin - Debut;
            Vector3 directionUnitairePerpendiculaire = Vector3.Transform(direction, MatriceRotation);
            directionUnitairePerpendiculaire.Normalize();

            Vector2 directionUnitairePerpendiculaire2 = new Vector2(directionUnitairePerpendiculaire.X, directionUnitairePerpendiculaire.Y);

            for (int i = 0; i < Tickness; i++)
            {
                Vector2 translation = directionUnitairePerpendiculaire2 * (offsetDebut + i);

                Primitives.DrawLine(spriteBatch, new Vector2(Debut.X, Debut.Y) + translation, new Vector2(Fin.X, Fin.Y) + translation, Couleur);

            }
        }
    }
}