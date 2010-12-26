namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public abstract class SceneMenu : Scene
    {
        protected LinkedList<MenuItem> ListeChoix = new LinkedList<MenuItem>();
        protected LinkedList<MenuItem>.Enumerator Iter;
        protected LinkedListNode<MenuItem> FocusChoix = null;

        protected Color CouleurSelectionne;
        protected Color CouleurNonSelectionne;


        public SceneMenu(Vector2 position, int height, int width) :
            base(position, height, width) { }


        protected string ProchainAction
        {
            get { return FocusChoix.Value.Action; }
        }


        protected void onUp()
        {
            if (FocusChoix.Previous != null)
            {
                FocusChoix.Value.Titre.Couleur = CouleurNonSelectionne;
                FocusChoix = FocusChoix.Previous;
                FocusChoix.Value.Titre.Couleur = CouleurSelectionne;
            }
        }


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
