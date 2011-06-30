namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EditorGeneralMenu
    {
        public Dictionary<EditorGeneralMenuChoice, Image> Menus;
        public Dictionary<EditorGeneralMenuChoice, ContextualMenu> SubMenus;

        private Simulator Simulator;
        private Vector3 Position;
        private double VisualPriority;

        public EditorGeneralMenuChoice SubMenuToFadeOut;


        public EditorGeneralMenu(Simulator simulator, Vector3 position, double visualPriority)
        {
            Simulator = simulator;
            Position = position;
            VisualPriority = visualPriority;

            Menus = new Dictionary<EditorGeneralMenuChoice, Image>(EditorGeneralMenuChoiceComparer.Default);


            Menus.Add(EditorGeneralMenuChoice.File, new Image("EditorFile", position) { SizeX = 4, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuChoice.Gameplay, new Image("EditorGameplay", position + new Vector3(50, 0, 0)) { SizeX = 4, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuChoice.Waves, new Image("EditorWaves", position + new Vector3(100, 0, 0)) { SizeX = 4, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuChoice.Battlefield, new Image("EditorBattlefield", position + new Vector3(150, 0, 0)) { SizeX = 4, VisualPriority = visualPriority });

            SubMenus = new Dictionary<EditorGeneralMenuChoice, ContextualMenu>(EditorGeneralMenuChoiceComparer.Default);

            //=================================================================

            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Load", Simulator.EditorCommands["ShowLoadPanel"]),
                new EditorTextContextualMenuChoice("Save", Simulator.EditorCommands["SaveLevel"]),
                new EditorTextContextualMenuChoice("Save as...", Simulator.EditorCommands["ShowSavePanel"]),
                new EditorToggleContextualMenuChoice(new List<string>() { "Playtest", "Edit" }, new List<EditorCommand>() { Simulator.EditorCommands["PlaytestState"], Simulator.EditorCommands["EditState"] }),
                new EditorTextContextualMenuChoice("Restart", Simulator.EditorCommands["RestartSimulation"]),
                new EditorToggleContextualMenuChoice(new List<string>() { "Pause", "Resume" }, new List<EditorCommand>() { new EditorCommand("PauseSimulation"), new EditorCommand("ResumeSimulation") }),
                new EditorTextContextualMenuChoice("Delete", Simulator.EditorCommands["ShowDeletePanel"])
            };

            var menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("File");
            SubMenus.Add(EditorGeneralMenuChoice.File, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Background", Simulator.EditorCommands["ShowBackgroundPanel"]),
                new EditorTextContextualMenuChoice("General", Simulator.EditorCommands["ShowGeneralPanel"]),
                new EditorTextContextualMenuChoice("Turrets", Simulator.EditorCommands["ShowTurretsPanel"]),
                new EditorTextContextualMenuChoice("Power-ups", Simulator.EditorCommands["ShowPowerUpsPanel"]),
                new EditorTextContextualMenuChoice("Player", Simulator.EditorCommands["ShowPlayerPanel"])
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("Gameplay");
            SubMenus.Add(EditorGeneralMenuChoice.Gameplay, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Quick generate", Simulator.EditorCommands["QuickGeneratePlanetarySystem"]),
                new EditorTextContextualMenuChoice("Generate", Simulator.EditorCommands["ShowGeneratePlanetarySystemPanel"]),
                new EditorTextContextualMenuChoice("Add a planet", Simulator.EditorCommands["AddPlanet"]),
                new EditorTextContextualMenuChoice("Validate", Simulator.EditorCommands["ValidatePlanetarySystem"]),
                new EditorTextContextualMenuChoice("Clear", new EditorCelestialBodyCommand("Clear")),
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("Battlefield");
            SubMenus.Add(EditorGeneralMenuChoice.Battlefield, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Edit...", Simulator.EditorCommands["ShowWavesPanel"]),
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("Waves");
            SubMenus.Add(EditorGeneralMenuChoice.Waves, menu);

            //=================================================================

            SubMenuToFadeOut = EditorGeneralMenuChoice.None;
        }


        public void Initialize()
        {
            //foreach (var subMenu in SubMenus.Values)
            //    if (subMenu.Visible)
            //        subMenu.Fade(255, 0, 500, null);
        }


        public void Draw()
        {
            foreach (var menu in Menus.Values)
                Simulator.Scene.Add(menu);
        }


        public EditorGeneralMenuChoice GetSelection(Circle circle)
        {
            foreach (var kvp in Menus)
                if (Physics.CircleRectangleCollision(circle, kvp.Value.GetRectangle()))
                    return kvp.Key;

            return EditorGeneralMenuChoice.None;
        }


        public void DoMenuChanged(EditorGeneralMenuChoice before, EditorGeneralMenuChoice now, Color color)
        {
            if (before != EditorGeneralMenuChoice.None && before != now)
            {
                Simulator.Scene.VisualEffects.Add(Menus[before], VisualEffects.ChangeSize(5, 4, 0, 250));
                //SubMenus[before].Fade(255, 0, 2500, FadeCompleted);
                //SubMenuToFadeOut = before;
                SubMenus[before].Visible = false;
            }


            if (now != EditorGeneralMenuChoice.None)
            {
                Simulator.Scene.VisualEffects.Add(Menus[now], VisualEffects.ChangeSize(4, 5, 0, 250));
                //SubMenus[now].Fade(0, 255, 2500, null);
                Menus[now].Color = color;
                SubMenus[now].Color = color;
                SubMenus[now].Visible = true;
            }
        }


        private void FadeCompleted()
        {
            SubMenuToFadeOut = EditorGeneralMenuChoice.None;
        }
    }
}
