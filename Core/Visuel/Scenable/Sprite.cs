//=============================================================================
//
// Défini un objet visible ayant plusieurs images
//
//=============================================================================

namespace EphemereGames.Core.Visuel
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Persistance;
    
    public class Sprite : IVisible, IContenu
    {

        //===========================================================================
        // Attributs
        //===========================================================================

        private Rectangle[] rectangles;
        private int nbSprites;                              // Nombre de sprites utilisés sur le spriteSheet
        private Rectangle[] rectanglesJeu;                  // Rectangles des sprites dans le jeu
        private Vector2[] centres;                          // Centres d'un sprite

        [ContentSerializer(Optional = false)]
        public String NomSpriteSheet {get; set; }

        [ContentSerializer(Optional = false)]
        public bool Cyclique { get; set; }                  // Est-ce que le sprite doit cycler

        [ContentSerializer(Optional = false)]
        public double VitesseDefilement { get; set; }       // Vitesse de défilement du sprite en ms.

        [ContentSerializerIgnore]
        public int SpriteActuel { get; set; }               // Sprite qui a le focus

        [ContentSerializerIgnore]
        public Texture2D SpriteSheet { get; set; }          // Image qui contient l'ensemble des sprites

        [ContentSerializerIgnore]
        public bool OrigineManuelle { get; set; }           // Contrôle manuel ou non de l'origine

        [ContentSerializer(Optional = true)]
        public int NbLignes { get; set; }                   // Nombre de lignes du Sprite Sheet "normal"

        [ContentSerializer(Optional = true)]
        public int NbColonnes { get; set; }                 // Nombre de colonnes du Sprite Sheet "normal"

        private double DernierSpriteCycle = 0;


        //===========================================================================
        // Getters / Setters
        //===========================================================================

        /// <summary>
        /// Temps (restant) pour un cycle dans tous les parties du sprite
        /// </summary>
        public double TempsDefilement
        {
            get
            {
                if (Cyclique)
                    return VitesseDefilement * nbSprites;

                return VitesseDefilement * (nbSprites - SpriteActuel);
            }
        }


        public override Vector2 Centre
        {
            get {return centres[SpriteActuel]; }
        }


        public override Rectangle Rectangle
        {
            get
            {
                rectanglesJeu[SpriteActuel].X = (int)(position.X - origine.X);
                rectanglesJeu[SpriteActuel].Y = (int)(position.Y - origine.Y);
                rectanglesJeu[SpriteActuel].Width = rectangles[SpriteActuel].Width;
                rectanglesJeu[SpriteActuel].Height = rectangles[SpriteActuel].Height;

                if (Rotation != 0)
                    rectangleTransforme(TransformeeVisuel, ref rectanglesJeu[SpriteActuel]);

                return rectanglesJeu[SpriteActuel];
            }

        }


        public bool FinCycle
        {
            get { return (SpriteActuel == nbSprites - 1); }
        }


        [ContentSerializerIgnore]
        public override Texture2D Texture
        {
            get { throw new Exception("Ne recupere pas la texture d'un sprite!"); }
            set { throw new Exception("Inutile de setter la texture d'un sprite!"); }
        }


        [ContentSerializerIgnore]
        public new Vector2 Origine
        {
            get { return (OrigineManuelle) ? base.Origine : centres[SpriteActuel]; }
            set { base.Origine = value; }
        }


        [ContentSerializer(Optional = true)]
        public Rectangle[] Rectangles
        {
            get { return rectangles; }
            set
            {
                if (value == null)
                    return;

                this.rectangles = value;
                this.nbSprites = rectangles.Length;

                //Initialise le centre des sprites et les rectangles
                centres = new Vector2[nbSprites];
                rectanglesJeu = new Rectangle[nbSprites];

                for (int i = 0; i < nbSprites; i++)
                {
                    rectanglesJeu[i] = new Rectangle(0, 0, rectangles[i].Width, rectangles[i].Height);
                    centres[i] = new Vector2(rectangles[i].Width / 2, rectangles[i].Height / 2);
                }
            }
        }


        //===========================================================================
        // Constructeurs
        //===========================================================================

        public Sprite()
        {
            rectanglesJeu = null;
            centres = null;
            nbSprites = 0;
            rectangles = null;

            NomSpriteSheet = "inconnu";
            Cyclique = true;
            VitesseDefilement = 100;
            SpriteActuel = 0;
            OrigineManuelle = false;
        }


        public Sprite(Texture2D spriteSheet, Rectangle[] rectanglesSpriteSheet, bool cyclique, double vitesseDefilement)
            : base()
        {
            this.SpriteSheet = spriteSheet;
            this.SpriteActuel = 0;            
            this.Cyclique = cyclique;
            this.VitesseDefilement = vitesseDefilement;
            this.OrigineManuelle = false;

            initRectangles(rectanglesSpriteSheet);
        }

        /// <summary>
        /// Crée un Sprite à partir d'un Sprite Sheet "normal", càd un gif converti en png.
        /// ATTENTION : Le nombre d'images dans le Sprite Sheet doit absolument être égal à nbLignes * nbColonnes !
        /// </summary>
        /// <param name="nbLignes">Nombre de lignes que contient le Sprite Sheet.</param>
        /// <param name="nbColonnes">Nombre de colonnes que contient le Sprite Sheet.</param>
        /// <param name="vitesseDefilement">Vitesse de défilement en ms.</param>
        public Sprite(Texture2D spriteSheet, int nbLignes, int nbColonnes, bool cyclique, double vitesseDefilement)
            : this(spriteSheet, new Rectangle[0], cyclique, vitesseDefilement)
        {
            NbLignes = nbLignes;
            NbColonnes = nbColonnes;

            Rectangle[] tab = new Rectangle[nbLignes * nbColonnes];

            int largeur = spriteSheet.Width / nbColonnes;
            int hauteur = spriteSheet.Height / nbLignes;

            for (int i = 0; i < nbLignes; i++)
            {
                for (int j = 0; j < nbColonnes; j++)
                    tab[i * nbColonnes + j] = new Rectangle(j * largeur, i * hauteur, largeur, hauteur);
            }

            initRectangles(tab);
        }


        //===========================================================================
        // Logique
        //===========================================================================

        public void Update(GameTime gameTime)
        {
            DernierSpriteCycle += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (DernierSpriteCycle >= VitesseDefilement)
            {
                suivant();
                DernierSpriteCycle = 0;
            }
        }

        public void setPositionDebut() { SpriteActuel = 0; }
        public void setPositionFin() { SpriteActuel = nbSprites - 1; }

        public void suivant()
        {
            SpriteActuel++;

            if (SpriteActuel == nbSprites && Cyclique)
                SpriteActuel = 0;

            else if (SpriteActuel == nbSprites)
                SpriteActuel--;
        }


        //===========================================================================
        // Dessiner le sprite qui a le focus
        //===========================================================================

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteSheet, new Vector2(position.X, position.Y), Rectangles[SpriteActuel], Couleur, Rotation, Origine, TailleVecteur, Effets, 0);
        }

        private void initRectangles(Rectangle[] rectanglesSpriteSheet)
        {
            this.nbSprites = rectanglesSpriteSheet.Length;
            this.Rectangles = rectanglesSpriteSheet;

            //Initialise le centre des sprites et les rectangles
            centres = new Vector2[nbSprites];
            rectanglesJeu = new Rectangle[nbSprites];

            for (int i = 0; i < nbSprites; i++)
            {
                rectanglesJeu[i] = new Rectangle(0, 0, rectanglesSpriteSheet[i].Width, rectanglesSpriteSheet[i].Height);
                centres[i] = new Vector2(rectanglesSpriteSheet[i].Width / 2, rectanglesSpriteSheet[i].Height / 2);
            }
        }

        public string TypeAsset
        {
            get { return "Sprite"; }
        }

        public object charger(string nom, string chemin, Dictionary<string, string> parametres, ContentManager contenu)
        {
            Sprite sprite = contenu.Load<Sprite>(chemin);

            sprite.SpriteSheet = contenu.Load<Texture2D>(sprite.NomSpriteSheet);

            return sprite;
        }

        public object Clone()
        {
            if (this.Rectangles != null)
                return new Sprite(this.SpriteSheet, this.Rectangles, this.Cyclique, this.VitesseDefilement);
            else
                return new Sprite(this.SpriteSheet, this.NbLignes, this.NbColonnes, this.Cyclique, this.VitesseDefilement);
        }
    }
}
