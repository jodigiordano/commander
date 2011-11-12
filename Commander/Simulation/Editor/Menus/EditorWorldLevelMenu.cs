namespace EphemereGames.Commander.Simulation
{
    class EditorWorldLevelMenu : EditorContextualMenu
    {
        public EditorWorldLevelMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("edit", "edit", 2, new EditorSimpleCommand("EditLevel")));
            AddChoice(new EditorTextContextualMenuChoice("playtest", "playtest", 2, new EditorSimpleCommand("PlaytestLevel")));
        }


        protected override EditorCommand Selection
        {
            get { return ((EditorTextContextualMenuChoice) Choices[SelectedIndex]).Command; }
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorPlaytestingMode &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody != null;
            }

            set { base.Visible = value; }
        }
    }
}
