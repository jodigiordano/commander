namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    abstract class EditorPanelCommand : EditorCommand
    {
        public string Panel;


        public EditorPanelCommand(SimPlayer owner, string panel)
            : base(owner)
        {
            Panel = panel;
        }
    }


    class EditorPanelShowCommand : EditorPanelCommand
    {
        public Vector3 Position;
        public bool UsePosition;


        public EditorPanelShowCommand(SimPlayer owner, string panel)
            : base(owner, panel)
        {
            Position = Vector3.Zero;
            UsePosition = false;
        }
    }


    class EditorPanelCBShowCommand : EditorPanelShowCommand
    {
        public EditorPanelCBShowCommand(SimPlayer owner, string panel, CelestialBody cb, Simulator simulator)
            : base(owner, panel)
        {
            ((IEditorCBPanel) simulator.Data.Panels[panel]).CelestialBody = cb;
        }
    }
}
