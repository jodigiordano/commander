//=====================================================================
//
// Une sc�ne avec des trucs pour les menus
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class SceneMenu : Scene
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        protected LinkedList<MenuItem> ListeChoix = new LinkedList<MenuItem>();
        protected LinkedList<MenuItem>.Enumerator Iter;
        protected LinkedListNode<MenuItem> FocusChoix = null;

        protected Color CouleurSelectionne;
        protected Color CouleurNonSelectionne;
        //protected int ProchaineAction;
        //protected int FocusChoix;


        //=====================================================================
        // Constructeur
        //=====================================================================

        public SceneMenu(Vector2 position, string nomSceneParent)
            : base(position, nomSceneParent)
        {

        }

        public SceneMenu(Vector2 position, int hauteur, int largeur)
            : base(position, hauteur, largeur)
        {

        }


        //=====================================================================
        // Services
        //=====================================================================

        protected string ProchainAction
        {
            get { return FocusChoix.Value.Action; }
        }

        //
        // Action � effectuer lorsque la fl�che vers le haut est pes�e
        //

        protected void onUp()
        {
            if (FocusChoix.Previous != null)
            {
                FocusChoix.Value.Titre.Couleur = CouleurNonSelectionne;
                FocusChoix = FocusChoix.Previous;
                FocusChoix.Value.Titre.Couleur = CouleurSelectionne;
            }
        }


        //
        // Action � effectuer lorsque la fl�che vers le bas est pes�e
        //

        protected void onDown()
        {
            if (FocusChoix.Next != null)
            {
                FocusChoix.Value.Titre.Couleur = CouleurNonSelectionne;
                FocusChoix = FocusChoix.Next;
                FocusChoix.Value.Titre.Couleur = CouleurSelectionne;
            }
        }
    }
}