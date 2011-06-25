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
        private EditorGeneralMenuChoice FadingOutMenuChoice;


        public EditorGUIPlayer(Simulator simulator, Color color)
        {
            Simulator = simulator;

            GeneralMenuChoices = new Dictionary<EditorGeneralMenuChoice, ContextualMenu>(EditorGeneralMenuChoiceComparer.Default);

            //=================================================================

            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Load", Simulator.EditorCommands["ShowLoadPanel"]),
                new EditorTextContextualMenuChoice("Save", Simulator.EditorCommands["SaveLevel"]),
                new EditorTextContextualMenuChoice("Save as...", Simulator.EditorCommands["ShowSavePanel"]),
                new EditorToggleContextualMenuChoice(new List<string>() { "Playtest", "Edit" }, new List<EditorCommand>() { Simulator.EditorCommands["PlaytestState"], Simulator.EditorCommands["EditState"] }),
                new EditorTextContextualMenuChoice("Restart", Simulator.EditorCommands["RestartSimulation"]),
                new EditorTextContextualMenuChoice("Pause", Simulator.EditorCommands["PauseSimulation"]),
                new EditorTextContextualMenuChoice("Delete", Simulator.EditorCommands["ShowDeletePanel"])
            };

            var menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, color, choices, 5);
            menu.SetTitle("File");
            GeneralMenuChoices.Add(EditorGeneralMenuChoice.File, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Background", Simulator.EditorCommands["ShowBackgroundPanel"]),
                new EditorTextContextualMenuChoice("General", Simulator.EditorCommands["ShowGeneralPanel"]),
                new EditorTextContextualMenuChoice("Turrets", Simulator.EditorCommands["ShowTurretsPanel"]),
                new EditorTextContextualMenuChoice("Power-ups", Simulator.EditorCommands["ShowPowerUpsPanel"]),
                new EditorTextContextualMenuChoice("Player", Simulator.EditorCommands["ShowPlayerPanel"])
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, color, choices, 5);
            menu.SetTitle("Gameplay");
            GeneralMenuChoices.Add(EditorGeneralMenuChoice.Gameplay, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Quick generate", Simulator.EditorCommands["QuickGeneratePlanetarySystem"]),
                new EditorTextContextualMenuChoice("Generate", Simulator.EditorCommands["ShowGeneratePlanetarySystemPanel"]),
                new EditorTextContextualMenuChoice("Add a planet", Simulator.EditorCommands["AddPlanet"]),
                new EditorTextContextualMenuChoice("Validate", Simulator.EditorCommands["ValidatePlanetarySystem"]),
                new EditorTextContextualMenuChoice("Clear", new EditorCelestialBodyCommand("Clear")),
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, color, choices, 5);
            menu.SetTitle("Battlefield");
            GeneralMenuChoices.Add(EditorGeneralMenuChoice.Battlefield, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Edit...", Simulator.EditorCommands["ShowWavesPanel"]),
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, color, choices, 5);
            menu.SetTitle("Waves");
            GeneralMenuChoices.Add(EditorGeneralMenuChoice.Waves, menu);

            //=================================================================

            CelestialBodyMenu = new EditorCelestialBodyMenu(Simulator, color);

            //=================================================================

            FadingOutMenuChoice = EditorGeneralMenuChoice.None;
        }


        public void Draw()
        {
            if (SelectedGeneralMenu != EditorGeneralMenuChoice.None)
            {
                var menu = GeneralMenuChoices[SelectedGeneralMenu];

                menu.SelectedIndex = GeneralMenuSubMenuIndex;

                menu.Position = GeneralMenu.Menus[SelectedGeneralMenu].Position;

                menu.Draw();
            }

            if (FadingOutMenuChoice != EditorGeneralMenuChoice.None)
            {
                var menu = GeneralMenuChoices[FadingOutMenuChoice];
                menu.Position = GeneralMenu.Menus[FadingOutMenuChoice].Position;
                menu.Draw();
            }

            if (Simulator.EditorState == EditorState.Playtest)
                return;

            CelestialBodyMenu.Draw();
        }


        public void DoGeneralMenuChanged(EditorGeneralMenuChoice before, EditorGeneralMenuChoice now)
        {
            if (before != EditorGeneralMenuChoice.None)
            {
                GeneralMenuChoices[before].Fade(255, 0, 250, FadeCompleted);
                FadingOutMenuChoice = before;
            }


            if (now != EditorGeneralMenuChoice.None)
                GeneralMenuChoices[now].Fade(0, 255, 250, null);
        }


        private void FadeCompleted()
        {
            FadingOutMenuChoice = EditorGeneralMenuChoice.None;
        }
    }
}
