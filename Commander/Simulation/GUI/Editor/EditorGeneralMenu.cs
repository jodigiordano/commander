namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EditorGeneralMenu
    {
        public Dictionary<EditorGeneralMenuChoice, Image> Menus;

        private Simulator Simulator;
        private Vector3 Position;
        private double VisualPriority;


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


        public void DoMenuChanged(EditorGeneralMenuChoice before, EditorGeneralMenuChoice now)
        {
            if (before != EditorGeneralMenuChoice.None)
                Simulator.Scene.VisualEffects.Add(Menus[before], VisualEffects.ChangeSize(5, 4, 0, 250));


            if (now != EditorGeneralMenuChoice.None)
                Simulator.Scene.VisualEffects.Add(Menus[now], VisualEffects.ChangeSize(4, 5, 0, 250));
        }
    }
}
