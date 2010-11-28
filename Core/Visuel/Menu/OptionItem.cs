//=====================================================================
//
// Un item spécial contenu dans le menu (associé à une valeur)
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class OptionItem : MenuItem
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public IVisible Valeur { get; set; }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public OptionItem(IVisible titre, string action, /*Vector2 position, int positionMenu,*/ IVisible valeur)
            : base(titre, action /*, position, positionMenu*/)
        {
            this.Valeur = valeur;
        }
    }
}