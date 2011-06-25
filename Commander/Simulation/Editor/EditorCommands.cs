namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorCommand : IEqualityComparer<EditorCommand>
    {
        public string Name;


        public EditorCommand(string name)
        {
            Name = name;
        }


        public bool Equals(EditorCommand x, EditorCommand y)
        {
            return x.Name == y.Name;
        }


        public int GetHashCode(EditorCommand obj)
        {
            return base.GetHashCode();
        }
    }


    class EditorPanelCommand : EditorCommand
    {
        public EditorPanel Panel;
        public bool Show;


        public EditorPanelCommand(string name, EditorPanel panel, bool show)
            : base(name)
        {
            Panel = panel;
            Show = show;
        }
    }


    class EditorCelestialBodyCommand : EditorCommand
    {
        public CelestialBody CelestialBody;
        public int Speed;
        public Size Size;


        public EditorCelestialBodyCommand(string name)
            : base(name)
        {

        }
    }
}
