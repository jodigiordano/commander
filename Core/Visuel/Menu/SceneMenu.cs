//=====================================================================
//
// Une scène avec des trucs pour les menus
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
        // Action à effectuer lorsque la flèche vers le haut est pesée
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
        // Action à effectuer lorsque la flèche vers le bas est pesée
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