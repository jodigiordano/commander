//=====================================================================
//
// Définition d'une animation de base
// Une animation est une (ou plusieurs) textures sur lesquels on
// affiche effectue des transformations dans le temps
//
// important: lorsqu'on redéfini suivant(), ne pas oublier de
// faire un appel à base.suivant(gameTime) pour que l'animation soit
// initialisée
//
//=====================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Core.Visuel
{
    [Serializable]
    public class Animation : IScenable
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        [ContentSerializerIgnore]
        public bool EnPause                 { get; set; }

        [ContentSerializerIgnore]
        public IVisible Objet               { get; set; }

        [ContentSerializer(Optional = true)]
        public float PrioriteAffichage      { get; set; }

        [ContentSerializerIgnore]
        public Scene Scene                  { protected get; set; }


        private double duree = 0;                               // Durée de vie de l'animation
        private double tempsRestant;                            // Temps où l'animation s'arrête


        //=====================================================================
        // Getter / Setter
        //=====================================================================

        protected Vector3 position;

        [ContentSerializer(Optional = true)]
        public Vector3 Position
        {
            get { return (Objet != null) ? Objet.position : position; }
            set
            {
                if (Objet != null)
                    Objet.Position = value;
                else
                    this.position = value;
            }
        }

        protected TypeMelange melange;

        [ContentSerializer(Optional = true)]
        public TypeMelange Melange
        {
            get { return (Objet != null) ? Objet.Melange : melange; }
            set
            {
                if (Objet != null)
                    Objet.Melange = value;
                else
                    this.melange = value;
            }
        }

        protected List<IScenable> composants;

        [ContentSerializer(Optional = true)]
        public List<IScenable> Composants
        {
            get { return (Objet != null) ? Objet.Composants : composants; }
            set
            {
                if (Objet != null)
                    Objet.Composants = value;
                else
                    this.composants = value;
            }
        }
		
					
        public double Duree
        {
            get { return duree; }
            set
            {
                tempsRestant += value - duree;
                duree = value;
            }
        }

        public double TempsRestant
        {
            get { return tempsRestant; }
        }

        protected double TempsRelatif
        {
            get { return this.Duree - this.TempsRestant; }
        }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Animation()
        {
            Duree = 0;
            this.EnPause = false;
            this.Objet = null;
            this.Position = Vector3.Zero;
            this.Melange = TypeMelange.Alpha;
            this.Composants = null;
            this.PrioriteAffichage = 0;
        }


        public Animation(double duree)
        {
            Duree = duree;
            this.EnPause = false;
            this.Objet = null;
            this.Position = Vector3.Zero;
            this.Melange = TypeMelange.Alpha;
            this.Composants = null;
            this.PrioriteAffichage = 0;
        }

        public virtual void Initialize()
        {
            tempsRestant = Duree;
        }


        //=====================================================================
        // Logique
        //=====================================================================

        /// <summary>
        /// Prochaine "frame" de l'animation.
        /// Important: lorsqu'on redéfini suivant(), ne pas oublier de
        /// faire un appel à base.suivant(gameTime) (idéalement à la fin du suivant())
        /// pour que le temps restant soit updaté
        /// </summary>
        public virtual void suivant(GameTime gameTime)
        {
            tempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        //
        // Détermine si l'animation est terminée
        //

        public virtual bool estTerminee(GameTime gameTime)
        {
            return tempsRestant <= 0;
        }


        //
        // Met fin à l'animation
        //

        public virtual void stop()
        {
            tempsRestant = 0;
        }


        //
        // Afficher l'animation
        //

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            this.Objet.Draw(spriteBatch);
        }
    }
}
