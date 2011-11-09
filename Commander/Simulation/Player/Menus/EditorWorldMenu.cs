namespace EphemereGames.Commander.Simulation.Player
{
    using EphemereGames.Core.Visual;

    
    class EditorWorldMenu : WorldMenu
    {
        public EditorWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new TextContextualMenuChoice("edit", new Text("edit", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("playtest", new Text("playtest", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("save", new Text("save", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("reset", new Text("reset", @"Pixelite") { SizeX = 2 }));
        }


        public EditorWorldChoice EditorChoice
        {
            get { return (EditorWorldChoice) SelectedIndex; }
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorEditingMode &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody != null;
            }

            set { base.Visible = value; }
        }
    }
}
