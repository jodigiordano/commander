//=====================================================================
//
// Barre de défilement
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class BarreDefilement : IScenable
    {
        //=====================================================================
        // Attributs
        //=====================================================================

        public IVisible ValeurVisible       { get; set; }
        public int Min                      { get; private set; }
        public int Max                      { get; private set; }
        public Vector3 Position             { get; set; }
        public TypeMelange Melange          { get; set; }
        public List<IScenable> Composants   { get; set; }
        public Scene Scene                  { get; set; }
        public float PrioriteAffichage      { get; set; }

        protected SpriteFont police;
        protected Color couleur;
        private int valeur;


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        public int Valeur
        {
            get { return valeur; }
            set { valeur = (int)MathHelper.Clamp(value, Min, Max); }
        }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public BarreDefilement(int valeur, int min, int max, Vector3 position, SpriteFont police, Color couleur, Scene scene)
        {
            this.valeur = valeur;
            this.Min = min;
            this.Max = max;
            this.Position = position;
            this.police = police;
            this.couleur.R = couleur.R;
            this.couleur.G = couleur.G;
            this.couleur.B = couleur.B;
            this.couleur.A = couleur.A;
            this.Melange = TypeMelange.Alpha;
            this.Scene = scene;
            this.PrioriteAffichage = 0;

            ValeurVisible = new IVisible("<  " + valeur.ToString() + "  >", police, couleur, position, scene);
        }


        //=====================================================================
        // Logique
        //=====================================================================

        public virtual int incrementer()
        {
            return (valeur = Math.Min(valeur + 1, Max));
        }

        public virtual int decrementer()
        {
            return (valeur = Math.Max(valeur - 1, Min));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            ValeurVisible.Texte = "<  " + valeur.ToString() + "  >";
            ValeurVisible.Draw(spriteBatch);
        }
    }
}
