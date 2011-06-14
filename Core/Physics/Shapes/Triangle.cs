//=============================================================================
//
// Défini un triangle qui peut subir des transformation
//
//=============================================================================

namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;

    public class Triangle
    {

        //=============================================================================
        // Attributs
        //=============================================================================

        public Vector2 Sommet1 { get; set; }        // Sommet 1 du triangle
        public Vector2 Sommet2 { get; set; }        // Sommet 2 du triangle
        public Vector2 Sommet3 { get; set; }        // Sommet 3 du triangle

        private Vector2 origine;                    // Origine du triangle
        private float rotation;                     // Rotation du triangle
        private Matrix matriceTransformation;       // Matrice de transformation


        //=============================================================================
        // Getters / Setters
        //=============================================================================

        public Vector2 Position
        {
            get { return this.Sommet1; }
            set
            {
                this.matriceTransformation = Matrix.CreateTranslation(new Vector3(value - this.Position, 0.0f));

                this.Sommet1 = Vector2.Transform(this.Sommet1, this.matriceTransformation);
                this.Sommet2 = Vector2.Transform(this.Sommet2, this.matriceTransformation);
                this.Sommet3 = Vector2.Transform(this.Sommet3, this.matriceTransformation);
            }
        }


        public float Rotation
        {
            get { return this.rotation; }

            set
            {
                this.rotation = value;

                this.matriceTransformation =
                    Matrix.CreateTranslation(new Vector3(-this.Position, 0.0f)) *
                    Matrix.CreateRotationZ(this.rotation) *
                    Matrix.CreateTranslation(new Vector3(this.Position, 0.0f));

                this.Sommet1 = Vector2.Transform(this.Sommet1, this.matriceTransformation);
                this.Sommet2 = Vector2.Transform(this.Sommet2, this.matriceTransformation);
                this.Sommet3 = Vector2.Transform(this.Sommet3, this.matriceTransformation);
            }
        }


        //=============================================================================
        // Constructeur
        //=============================================================================

        public Triangle(Vector2 sommet1, Vector2 sommet2, Vector2 sommet3)
        {
            this.Sommet1 = sommet1;
            this.Sommet2 = sommet2;
            this.Sommet3 = sommet3;

            this.Position = sommet1;
            this.rotation = 0.0f;
        }
    }
}
