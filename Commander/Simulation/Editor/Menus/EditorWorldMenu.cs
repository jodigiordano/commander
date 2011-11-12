namespace EphemereGames.Commander.Simulation
{
    class EditorWorldMenu : EditorContextualMenu
    {
        public EditorWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("EditLayout", "edit layout", 2, new EditorSimpleCommand("Edit")));
            AddChoice(new EditorTextContextualMenuChoice("ChangeName", "change name", 2, new EditorShowPanelCommand("EditorWorldName")));

            Visible = false;
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
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }
    }
}
