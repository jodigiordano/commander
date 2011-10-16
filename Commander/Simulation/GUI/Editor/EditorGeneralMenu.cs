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

            Menus.Add(EditorGeneralMenuChoice.Gameplay, new Image("EditorGameplay", position + new Vector3(0, 0, 0)) { SizeX = 4, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuChoice.Battlefield, new Image("EditorBattlefield", position + new Vector3(50, 0, 0)) { SizeX = 4, VisualPriority = visualPriority });

            SubMenus = new Dictionary<EditorGeneralMenuChoice, ContextualMenu>(EditorGeneralMenuChoiceComparer.Default);

            //=================================================================

            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Background", "Background", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Background, true)),
                new EditorTextContextualMenuChoice("Turrets", "Turrets", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Turrets, true)),
                new EditorTextContextualMenuChoice("PowerUps", "Power-ups", 2, new EditorPanelCommand("ShowPanel", EditorPanel.PowerUps, true)),
                new EditorTextContextualMenuChoice("Player", "Player", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Player, true)),
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, new EditorPanelCommand("ShowPanel", EditorPanel.Waves, true)),
            };

            var menu = new ContextualMenu(simulator, VisualPriority - 0.00001, Color.White, choices, 5);
            menu.SetTitle("Gameplay");
            SubMenus.Add(EditorGeneralMenuChoice.Gameplay, menu);

            //=================================================================

            choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add a planet", 2, new EditorCelestialBodyCommand("AddPlanet")),
                new EditorToggleContextualMenuChoice("ShowPaths", new List<string>() { "Show paths", "Hide paths" }, 2, new List<EditorCommand>() { new EditorCommand("ShowCelestialBodiesPaths"), new EditorCommand("HideCelestialBodiesPaths") }),
                new EditorTextContextualMenuChoice("Clear", "Clear", 2, new EditorCelestialBodyCommand("Clear")),
            };

            menu = new ContextualMenu(simulator, VisualPriority - 0.00001, Color.White, choices, 5);
            menu.SetTitle("Battlefield");
            SubMenus.Add(EditorGeneralMenuChoice.Battlefield, menu);

            //=================================================================

            SubMenuToFadeOut = EditorGeneralMenuChoice.None;
        }


        public void Initialize()
        {

        }


        public void Draw()
        {
            foreach (var menu in Menus.Values)
                Simulator.Scene.Add(menu);
        }


        public EditorGeneralMenuChoice GetSelection(Circle circle)
        {
            if (Simulator.EditorState == EditorState.Playtest)
                return EditorGeneralMenuChoice.None;

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
                SubMenus[before].Visible = false;
            }


            if (now != EditorGeneralMenuChoice.None)
            {
                Simulator.Scene.VisualEffects.Add(Menus[now], VisualEffects.ChangeSize(4, 5, 0, 250));
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
