namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class EditorTextContextualMenuChoice : TextContextualMenuChoice
    {
        public EditorCommand Command;


        public EditorTextContextualMenuChoice(string name, float textSize, EditorCommand command)
            : base(new Text(name, "Pixelite") { SizeX = textSize })
        {
            Command = command;
        }
    }
}
