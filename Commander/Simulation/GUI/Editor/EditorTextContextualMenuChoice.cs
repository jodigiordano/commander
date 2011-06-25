namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class EditorTextContextualMenuChoice : TextContextualMenuChoice
    {
        public EditorCommand Command;


        public EditorTextContextualMenuChoice(string name, EditorCommand command)
            : base(new Text(name, "Pixelite") { SizeX = 2 })
        {
            Command = command;
        }
    }
}
