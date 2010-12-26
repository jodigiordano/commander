//=====================================================================
//
// Un item contenu dans le menu
//
//=====================================================================

namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class MenuItem
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public IVisible Titre { get; private set; }
        public string Action { get; private set; }
        //public Vector2 Position { get; private set; }
        //public int PositionMenu { get; private set; }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public MenuItem(IVisible titre, string action /*, Vector2 position, int positionMenu*/)
        {
            this.Titre = titre;
            this.Action = action;
            //this.Position = position;
            //this.PositionMenu = positionMenu;
        }
    }
}