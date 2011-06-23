namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class EditorPlayer
    {
        public Circle Circle;
        public Color Color;
        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorGeneralMenuAction, TextMenu> GeneralMenuSubMenus;

        public EditorPlayerSelection ActualSelection;

        private Simulator Simulator;


        public EditorPlayer(Simulator simulator)
        {
            Simulator = simulator;

            ActualSelection = new EditorPlayerSelection();
        }


        public void Initialize()
        {

        }


        public void Update()
        {
            CheckGeneralMenu();

            if (ActualSelection.GeneralMenu == EditorGeneralMenuAction.None)
            {
                ActualSelection.GeneralMenuSubMenuIndex = -1;
            }
        }


        public void NextGeneralMenuOption()
        {
            if (ActualSelection.GeneralMenu == EditorGeneralMenuAction.None)
                return;

            int nbChoices = GeneralMenuSubMenus[ActualSelection.GeneralMenu].ChoicesCount;

            ActualSelection.GeneralMenuSubMenuIndex++;

            if (ActualSelection.GeneralMenuSubMenuIndex >= GeneralMenu.Menus.Count)
                ActualSelection.GeneralMenuSubMenuIndex = 0;
        }


        public void PreviousGeneralMenuOption()
        {
            if (ActualSelection.GeneralMenu == EditorGeneralMenuAction.None)
                return;

            int nbChoices = GeneralMenuSubMenus[ActualSelection.GeneralMenu].ChoicesCount;

            ActualSelection.GeneralMenuSubMenuIndex--;

            if (ActualSelection.GeneralMenuSubMenuIndex < 0)
                ActualSelection.GeneralMenuSubMenuIndex = nbChoices - 1;
        }


        private void CheckGeneralMenu()
        {
            ActualSelection.GeneralMenu = GeneralMenu.GetSelection(Circle);
        }
    }
}