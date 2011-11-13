namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class EditorTextContextualMenuChoice : TextContextualMenuChoice
    {
        public EditorTextContextualMenuChoice(string name, string label, float textSize, NoneHandler handler)
            : base(name, new Text(label, @"Pixelite") { SizeX = textSize })
        {
            DoClick = handler;
        }
    }
}
