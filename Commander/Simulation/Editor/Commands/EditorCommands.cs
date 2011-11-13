namespace EphemereGames.Commander.Simulation
{
    abstract class EditorCommand
    {
        public SimPlayer Owner;


        public EditorCommand(SimPlayer owner)
        {
            Owner = owner;
        }
    }


    class EditorEditCommand : EditorCommand
    {
        public EditorEditCommand(SimPlayer owner)
            : base(owner)
        {

        }
    }


    class EditorEditLevelCommand : EditorCommand
    {
        public EditorEditLevelCommand(SimPlayer owner)
            : base(owner)
        {

        }
    }


    class EditorPlaytestLevelCommand : EditorCommand
    {
        public EditorPlaytestLevelCommand(SimPlayer owner)
            : base(owner)
        {

        }
    }


    class EditorPlaytestCommand : EditorCommand
    {
        public EditorPlaytestCommand(SimPlayer owner)
            : base(owner)
        {

        }
    }
}
