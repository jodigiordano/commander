//=====================================================================
//
// Barre de sensibilité (sous forme d'une barre de défilement)
// Différences :
//      - incrémenter alors qu'on est à la dernière valeur permet de revenir à la première valeur (et vice-versa)
//      - affichage spécial suivant les valeurs
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class BarreSensibilite : BarreDefilement
    {
        private String Texte { get; set; }

        public BarreSensibilite(int valeur, int min, int max, Vector3 position, SpriteFont police, Color couleur, String texte, Scene scene)
            : base(valeur, min, max, position, police, couleur, scene)
        {
            Texte = texte;
            ValeurVisible = new IVisible("<  " + valeur.ToString() + texte + "  >", police, couleur, position, scene);
        }


        public override int incrementer()
        {
            if (Valeur + 1 > Max)
            {
                Valeur = Min;
                return Valeur;
            }
            else
                return base.incrementer();
        }


        public override int decrementer()
        {
            if (Valeur - 1 < Min)
            {
                Valeur = Max;
                return Valeur;
            }
            else
                return base.decrementer();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            ValeurVisible.Texte = "<  " + Valeur.ToString() + "  >";
            ValeurVisible.Draw(spriteBatch);
        }
    }
}
