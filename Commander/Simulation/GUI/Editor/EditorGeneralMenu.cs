﻿namespace EphemereGames.Commander.Simulation
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
                new EditorTextContextualMenuChoice("New", "New", 2, new EditorCommand("NewLevel")),
                new EditorTextContextualMenuChoice("Load", "Load", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Load, true)),
                new EditorTextContextualMenuChoice("Save", "Save", 2, new EditorCommand("SaveLevel")),
                new EditorTextContextualMenuChoice("Delete", "Delete", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Delete, true)),
                new EditorToggleContextualMenuChoice("Playtest", new List<string>() { "Playtest", "Edit" }, 2, new List<EditorCommand>() { new EditorCommand("PlaytestState"), new EditorCommand("EditState") }),
                new EditorTextContextualMenuChoice("Restart", "Restart", 2, new EditorCommand("RestartSimulation")),
                new EditorToggleContextualMenuChoice("Pause", new List<string>() { "Pause", "Resume" }, 2, new List<EditorCommand>() { new EditorCommand("PauseSimulation"), new EditorCommand("ResumeSimulation") }),
            };

            var menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("File");
            SubMenus.Add(EditorGeneralMenuChoice.File, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Background", "Background", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Background, true)),
                new EditorTextContextualMenuChoice("General", "General", 2, new EditorPanelCommand("ShowPanel", EditorPanel.General, true)),
                new EditorTextContextualMenuChoice("Turrets", "Turrets", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Turrets, true)),
                new EditorTextContextualMenuChoice("PowerUps", "Power-ups", 2, new EditorPanelCommand("ShowPanel", EditorPanel.PowerUps, true)),
                new EditorTextContextualMenuChoice("Player", "Player", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Player, true))
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("Gameplay");
            SubMenus.Add(EditorGeneralMenuChoice.Gameplay, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add a planet", 2, new EditorCelestialBodyCommand("AddPlanet")),
                new EditorToggleContextualMenuChoice("ShowPaths", new List<string>() { "Show paths", "Hide paths" }, 2, new List<EditorCommand>() { new EditorCommand("ShowCelestialBodiesPaths"), new EditorCommand("HideCelestialBodiesPaths") }),
                new EditorTextContextualMenuChoice("Clear", "Clear", 2, new EditorCelestialBodyCommand("Clear")),
            };

            menu = new ContextualMenu(simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, Color.White, choices, 5);
            menu.SetTitle("Battlefield");
            SubMenus.Add(EditorGeneralMenuChoice.Battlefield, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Edit", "Edit...", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Waves, true)),
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
            if (Simulator.EditorState == EditorState.Playtest)
                return Physics.CircleRectangleCollision(circle, Menus[EditorGeneralMenuChoice.File].GetRectangle()) ?
                    EditorGeneralMenuChoice.File :
                    EditorGeneralMenuChoice.None;

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
