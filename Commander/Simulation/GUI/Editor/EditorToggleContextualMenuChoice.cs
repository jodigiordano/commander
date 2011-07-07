namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class EditorToggleContextualMenuChoice : ToggleContextualMenuChoice
    {
        public EditorCommand Command;
        private List<EditorCommand> Commands;


        public EditorToggleContextualMenuChoice(List<string> names, float textSize, List<EditorCommand> commands)
            : base(names, new Text(names[0], "Pixelite") { SizeX = textSize })
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
