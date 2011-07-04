namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorCommand : IEqualityComparer<EditorCommand>
    {
        public string Name;
        public EditorCommandType Type;
        public EditorPlayer Owner;


        public EditorCommand(string name)
        {
            Name = name;
            Type = EditorCommandType.Basic;
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
            Type = EditorCommandType.Panel;
            Panel = panel;
            Show = show;
        }
    }


    class EditorCelestialBodyCommand : EditorCommand
    {
        public CelestialBody CelestialBody;
        public float Speed;
        public Size Size;
        public bool ShowPath;


        public EditorCelestialBodyCommand(string name)
            : base(name)
        {
            Type = EditorCommandType.CelestialBody;
        }
    }


    class EditorPlayerCommand : EditorCommand
    {
        public int LifePoints;
        public int Cash;


        public EditorPlayerCommand(string name)
            : base(name)
        {
            Type = EditorCommandType.Player;
        }
    }
}
