namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorToggleContextualMenuChoice : ToggleContextualMenuChoice
    {
        public EditorCommand Command;
        private List<EditorCommand> Commands;


        public EditorToggleContextualMenuChoice(List<string> names, List<EditorCommand> commands)
            : base(names)
        {
            if (commands.Count != names.Count)
            {
                Commands = new List<EditorCommand>();

                for (int i = 0; i < names.Count; i++)
                    Commands.Add(commands[0]);
            }

            else
            {
                Commands = commands;
            }

            Command = Commands[CurrentIndex];
        }


        public override void Previous()
        {
            base.Previous();

            Command = Commands[CurrentIndex];
        }


        public override void Next()
        {
            base.Next();

            Command = Commands[CurrentIndex];
        }


        public override void SetChoice(int index)
        {
            base.SetChoice(index);

            Command = Commands[CurrentIndex];
        }
    }
}
