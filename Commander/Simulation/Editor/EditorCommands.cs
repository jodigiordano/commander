namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class EditorCommand : IEqualityComparer<EditorCommand>
    {
        public string Name;
        public SimPlayer Owner;


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


    class EditorSimpleCommand : EditorCommand
    {
        public EditorSimpleCommand(string name)
            : base(name)
        {

        }
    }


    class EditorShowPanelCommand : EditorCommand
    {
        public PanelType Panel;


        public EditorShowPanelCommand(PanelType panel)
            : base("ShowPanel")
        {
            Panel = panel;
        }
    }


    class EditorCelestialBodyCommand : EditorCommand
    {
        public CelestialBody CelestialBody;
        public float Speed;
        public Size Size;
        public bool ShowPath;
        public string AssetName;
        public bool HasMoons;
        public bool FollowPath;
        public bool CanSelect;
        public bool StraightLine;
        public bool Invincible;


        public EditorCelestialBodyCommand(string name)
            : base(name)
        {

        }
    }


    class EditorPlayerCommand : EditorCommand
    {
        public int LifePoints;
        public int Cash;
        public int Minerals;
        public int LifePacks;
        public double BulletDamage;


        public EditorPlayerCommand(string name)
            : base(name)
        {

        }
    }
}
