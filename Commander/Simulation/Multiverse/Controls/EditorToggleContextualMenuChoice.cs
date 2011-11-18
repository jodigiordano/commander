namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class EditorToggleContextualMenuChoice : ToggleContextualMenuChoice
    {
        public EditorToggleContextualMenuChoice(string name, List<string> labels, float textSize, NoneHandler handler)
            : base(name, labels, new Text(labels[0], @"Pixelite") { SizeX = textSize })
        {
            DoClick = handler;
        }
    }
}
