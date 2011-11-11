namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class EditorTextContextualMenuChoice : TextContextualMenuChoice
    {
        public EditorCommand Command;


        public EditorTextContextualMenuChoice(string name, string label, float textSize, EditorCommand command)
            : base(name, new Text(label, @"Pixelite") { SizeX = textSize })
        {
            Command = command;
        }
    }
}
