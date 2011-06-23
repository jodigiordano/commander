namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIPlayer
    {
        public EditorGeneralMenu GeneralMenu;
        public EditorGeneralMenuAction SelectedGeneralMenu;

        private Dictionary<EditorGeneralMenuAction, TextMenu> GeneralMenuChoices;


        public EditorGUIPlayer(Simulator simulator, Color color)
        {
            GeneralMenuChoices = new Dictionary<EditorGeneralMenuAction, TextMenu>(EditorGeneralMenuActionComparer.Default);

            GeneralMenuChoices.Add(
                EditorGeneralMenuAction.File,
                new TextMenu(
                    simulator,
                    new List<string>() { "Load", "Save", "Save as...", "Delete" },
                    Preferences.PrioriteSimulationCorpsCeleste - 0.001,
                    color));

            GeneralMenuChoices.Add(EditorGeneralMenuAction.Battlefield, new TextMenu(simulator, new List<string>(), Preferences.PrioriteSimulationCorpsCeleste - 0.001, color));
            GeneralMenuChoices.Add(EditorGeneralMenuAction.Gameplay, new TextMenu(simulator, new List<string>(), Preferences.PrioriteSimulationCorpsCeleste - 0.001, color));
            GeneralMenuChoices.Add(EditorGeneralMenuAction.Waves, new TextMenu(simulator, new List<string>(), Preferences.PrioriteSimulationCorpsCeleste - 0.001, color));
        }


        public void Draw()
        {
            if (SelectedGeneralMenu != EditorGeneralMenuAction.None)
            {
                var menu = GeneralMenuChoices[SelectedGeneralMenu];

                menu.Position = GeneralMenu.Menus[SelectedGeneralMenu].Position;

                menu.Draw();
            }
        }
    }
}
