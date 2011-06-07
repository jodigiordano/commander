namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    public class IVisible : IScenable, IPhysicalObject
    {
        protected enum TypeInterne
        {
            Texture,
            Texte,
            Autre
        }

        [ContentSerializerIgnore]
        public Vector3 position;

        [ContentSerializerIgnore]
        public Vector2 origine;

        [ContentSerializerIgnore]
        public Vector2 tailleVecteur;

        [ContentSerializerIgnore]
        public Rectangle rectangle;

        [ContentSerializer(Optional = true)]
        public float Speed                        { get; set; }

        [ContentSerializer(Optional = true)]
        public TypeBlend Blend                  { get; set; }

        [ContentSerializer(Optional = true)]
        public float VisualPriority              { get; set; }

        [ContentSerializer(Optional = true)]
        public virtual List<IScenable> Components   { get; set; }

        [ContentSerializer(Optional = true)]
        public Rectangle partieVisible;

        [ContentSerializer(Optional = true)]
        public Vector3 direction;

        [ContentSerializer(Optional = true)]
        public String Nom;

        [ContentSerializer(Optional = true)]
        public bool DessinerPartie;

        [ContentSerializer(Optional = true)]
        public String TextureNom;

        [ContentSerializer(Optional = true)]
        public String PoliceNom;

        [ContentSerializer(Optional = true)]
        public SpriteEffects Effets;

        [ContentSerializer(Optional = true)]
        public Color Couleur;

        private TypeInterne typeInterne;
        private Texture2D texture;
        private Color[] textureData;
        private String texte;
        private SpriteFont police;
        private Vector2 centre;
        private Matrix transformee;
        private Matrix _matrice1;
        private Matrix _matrice2;
        private Matrix _matrice3;
        private Matrix _matrice4;
        private float rotation;

        protected bool transformeeDeprecated = true;
        protected bool centreDeprecated = true;
        protected bool rectangleDeprecated = true;


        //=============================================================================
        // Constructeurs
        //=============================================================================

        public IVisible(Texture2D texture, Vector3 position)
            : this()
        {
            this.typeInterne = TypeInterne.Texture;
            this.texture = texture;

            this.position.X = position.X;
            this.position.Y = position.Y;
            this.position.Z = position.Z;
        }


        public IVisible(String texte, SpriteFont police, Color couleur, Vector3 position)
            : this()
        {
            this.typeInterne = TypeInterne.Texte;
            this.texte = texte;
            this.police = police;

            this.position.X = position.X;
            this.position.Y = position.Y;
            this.position.Z = position.Z;

            this.Couleur = couleur;
        }


        public IVisible()
        {
            this.Nom = "";
            this.typeInterne = TypeInterne.Autre;
            this.texture = null;
            this.textureData = null;
            this.TextureNom = "";
            this.texte = null;
            this.police = null;
            this.PoliceNom = "";

            this.rotation = 0.0f;
            tailleVecteur.X = tailleVecteur.Y = 1;

            this.Blend = TypeBlend.Alpha;
            this.Couleur = Color.White;
            this.Components = null;
            this.Effets = SpriteEffects.None;
            this.DessinerPartie = false;
            this.VisualPriority = 0;
        }


        /// <summary>
        /// Retourne la taille de la texture ou du texte
        /// </summary>
        public Vector2 tailleTexture()
        {
            return (typeInterne == TypeInterne.Texture) ? new Vector2(Texture.Width, Texture.Height) :
                   (typeInterne == TypeInterne.Texte)   ? Police.MeasureString(Texte) : Vector2.Zero;
        }


        //=============================================================================
        // Services
        //=============================================================================

        public virtual Vector2 Centre
        {
            get
            {
                if (centreDeprecated)
                {
                    if (typeInterne == TypeInterne.Texte)
                    {
                        centre = Police.MeasureString(Texte);
                        centre.X = centre.X / 2.0f;
                        centre.Y = centre.Y / 2.0f;
                    }
                    else
                    {
                        centre.X = Texture.Width / 2.0f;
                        centre.Y = Texture.Height / 2.0f;
                    }

                    centreDeprecated = false;
                }

                return centre;
            }
        }


        public virtual Rectangle Rectangle
        {
            get
            {
                if (!rectangleDeprecated)
                    return rectangle;

                if (rotation == 0 && typeInterne != TypeInterne.Texte)
                {
                    rectangle.X = (int)(position.X - origine.X * tailleVecteur.X);
                    rectangle.Y = (int)(position.Y - origine.Y * tailleVecteur.Y);
                    rectangle.Width = (int)(texture.Width * tailleVecteur.X);
                    rectangle.Height = (int)(texture.Height * tailleVecteur.Y);

                    rectangleDeprecated = false;

                    return rectangle;
                }

                if (typeInterne == TypeInterne.Texte)
                {
                    Vector2 grandeur = Police.MeasureString(Texte);

                    rectangle.X = (int)(position.X - origine.X * tailleVecteur.X);
                    rectangle.Y = (int)(position.Y - origine.Y * tailleVecteur.Y);
                    rectangle.Width = (int)(grandeur.X * tailleVecteur.X);
                    rectangle.Height = (int)(grandeur.Y * tailleVecteur.Y);
                }

                else
                {
                    rectangle.X = (int)(position.X - origine.X * tailleVecteur.X);
                    rectangle.Y = (int)(position.Y - origine.Y * tailleVecteur.Y);
                    rectangle.Width = (int)(texture.Width * tailleVecteur.X);
                    rectangle.Height = (int)(texture.Height * tailleVecteur.Y);
                }


                if (rotation != 0)
                    rectangleTransforme(TransformeeVisuel, ref rectangle);


                rectangleDeprecated = false;

                return rectangle;
            }
        }


        [ContentSerializer(Optional = true)]
        public String Texte
        {
            get { return texte; }
            set
            {
                texte = value;
                typeInterne = TypeInterne.Texte;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
                centreDeprecated = true;
            }
        }


        [ContentSerializerIgnore]
        public SpriteFont Police
        {
            get { return police; }
            set
            {
                police = value;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
                centreDeprecated = true;
            }
        }


        [ContentSerializer(Optional = true)]
        public virtual float Rotation {
            get { return rotation; }
            set
            {
                rotation = value;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
            }
        }


        [ContentSerializerIgnore]
        public virtual Texture2D Texture {
            get { return texture; }
            set
            {
                if (texture != null && (texture.Height != value.Height || texture.Width != value.Width))
                {
                    transformeeDeprecated = true;
                    rectangleDeprecated = true;
                    centreDeprecated = true;
                }

                texture = value;
                typeInterne = TypeInterne.Texture;
            }
        }


        [ContentSerializer(Optional = true)]
        public virtual Vector3 Position {
            get { return position; }
            set
            {
                position = value;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
            }
        }


        [ContentSerializer(Optional = true)]
        public float Taille
        {
            get { return tailleVecteur.X; }
            set { TailleVecteur = new Vector2(value); }
        }

        [ContentSerializer(Optional = true)]
        public virtual Vector2 TailleVecteur
        {
            get { return tailleVecteur; }
            set
            {
                tailleVecteur = value;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
            }
        }


        [ContentSerializer(Optional = true)]
        public virtual Vector2 Origine {
            get { return origine; }
            set
            {
                origine = value;
                transformeeDeprecated = true;
                rectangleDeprecated = true;
            }
        }

        public Matrix Transformee
        {
            get
            {
                Matrix.CreateTranslation(-origine.X, -origine.Y, 0, out _matrice1);
                Matrix.CreateScale(tailleVecteur.X, tailleVecteur.Y, 1, out _matrice2);
                Matrix.CreateRotationZ(rotation, out _matrice3);
                Matrix.CreateTranslation(ref position, out _matrice4);

                Matrix resultat;
                Matrix.Multiply(ref _matrice1, ref _matrice2, out resultat);
                Matrix.Multiply(ref resultat, ref _matrice3, out resultat);
                Matrix.Multiply(ref resultat, ref _matrice4, out resultat);

                return resultat;
            }
        }



        protected Matrix TransformeeVisuel
        {
            get
            {
                if (transformeeDeprecated)
                {
                    Matrix.CreateTranslation(-position.X, -position.Y, -position.Z, out _matrice1);
                    Matrix.CreateRotationZ(rotation, out _matrice2);
                    Matrix.Multiply(ref _matrice1, ref _matrice2, out transformee);
                    Matrix.CreateTranslation(ref position, out _matrice1);
                    Matrix.Multiply(ref transformee, ref _matrice1, out transformee);

                    transformeeDeprecated = false;
                }

                return transformee;
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (DessinerPartie)
            {
                switch (typeInterne)
                {
                    case TypeInterne.Texture: spriteBatch.Draw(Texture, new Vector2(position.X, position.Y), partieVisible, Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                    case TypeInterne.Texte: spriteBatch.DrawString(Police, Texte, new Vector2(position.X, position.Y), Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                    case TypeInterne.Autre: spriteBatch.Draw(Texture, new Vector2(position.X, position.Y), partieVisible, Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                }
            }

            else
            {
                switch (typeInterne)
                {
                    case TypeInterne.Texture: spriteBatch.Draw(Texture, new Vector2(position.X, position.Y), null, Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                    case TypeInterne.Texte: spriteBatch.DrawString(Police, Texte, new Vector2(position.X, position.Y), Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                    case TypeInterne.Autre: spriteBatch.Draw(Texture, new Vector2(position.X, position.Y), null, Couleur, Rotation, origine, TailleVecteur, Effets, 0); break;
                }
            }
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }


        //=============================================================================
        // Helpers
        //=============================================================================

        /// <summary>
        /// Rectangle qui subit une transformation
        /// </summary>
        /// <param name="transform">Matrice de transformation</param>
        /// <param name="rectangle">Rectangle à transformer</param>
        protected static void rectangleTransforme(Matrix transform, ref Rectangle rectangle)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            rectangle.X = (int)min.X;
            rectangle.Y = (int)min.Y;
            rectangle.Width = (int)(max.X - min.X);
            rectangle.Height = (int)(max.Y - min.Y);
        }
    }
}
