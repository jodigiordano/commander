namespace EphemereGames.Commander.Simulation
{
    abstract class EditorContextualMenu : CommanderContextualMenu
    {
        public EditorContextualMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }


        protected abstract EditorCommand Selection { get; }


        public override void UpdateSelection()
        {
            if (Visible)
            {
                var selection = Selection;
                selection.Owner = Owner;

                Owner.ActualSelection.EditorCommand = Selection;
            }
        }
    }
}
