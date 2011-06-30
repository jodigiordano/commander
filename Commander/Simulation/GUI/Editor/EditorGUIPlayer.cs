namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIPlayer
    {
        public EditorGeneralMenu GeneralMenu;
        public EditorGeneralMenuChoice SelectedGeneralMenu;
        public Dictionary<EditorGeneralMenuChoice, ContextualMenu> GeneralMenuChoices;
        public int GeneralMenuSubMenuIndex;
        public EditorCelestialBodyMenu CelestialBodyMenu;

        private Simulator Simulator;


        public EditorGUIPlayer(Simulator simulator, Color color)
        {
            Simulator = simulator;

            CelestialBodyMenu = new EditorCelestialBodyMenu(Simulator, color);
        }


        public void Update()
        {
            CelestialBodyMenu.Update();
        }


        public ContextualMenu OpenedMenu
        {
            get
            {
                if (CelestialBodyMenu.Visible)
                    return CelestialBodyMenu.Menu;

                return null;
            }
        }


        public void Draw()
        {
            if (Simulator.EditorState == EditorState.Playtest)
                return;

            CelestialBodyMenu.Draw();
        }
    }
}
