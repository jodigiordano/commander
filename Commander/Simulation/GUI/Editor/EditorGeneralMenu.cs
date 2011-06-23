namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EditorGeneralMenu
    {
        public Dictionary<EditorGeneralMenuAction, Text> Menus;

        private Simulator Simulator;
        private Vector3 Position;
        private double VisualPriority;


        public EditorGeneralMenu(Simulator simulator, Vector3 position, double visualPriority)
        {
            Simulator = simulator;
            Position = position;
            VisualPriority = visualPriority;

            Menus = new Dictionary<EditorGeneralMenuAction, Text>(EditorGeneralMenuActionComparer.Default);

            Menus.Add(EditorGeneralMenuAction.Gameplay, new Text("G", "Pixelite", position) { SizeX = 3, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuAction.Waves, new Text("W", "Pixelite", position + new Vector3(50, 0, 0)) { SizeX = 3, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuAction.Battlefield, new Text("B", "Pixelite", position + new Vector3(100, 0, 0)) { SizeX = 3, VisualPriority = visualPriority });
            Menus.Add(EditorGeneralMenuAction.File, new Text("F", "Pixelite", position + new Vector3(150, 0, 0)) { SizeX = 3, VisualPriority = visualPriority });
        }


        public void Draw()
        {
            foreach (var menu in Menus.Values)
                Simulator.Scene.Add(menu);
        }


        public EditorGeneralMenuAction GetSelection(Circle circle)
        {
            foreach (var kvp in Menus)
                if (Physics.CircleRectangleCollision(circle, kvp.Value.GetRectangle()))
                    return kvp.Key;

            return EditorGeneralMenuAction.None;
        }
    }
}
