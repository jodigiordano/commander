namespace EphemereGames.Commander.Simulation
{
    class EditorPinkHoleCBMenu : EditorCelestialBodyMenu
    {
        public EditorPinkHoleCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("Attributes", "Attributes", 2, new EditorShowCBPanelCommand("PinkHoleCBAttributesPanel")));
        }


        public override bool Visible
        {
            get { return base.Visible && Owner.ActualSelection.CelestialBody is PinkHole; }
            set { base.Visible = value; }
        }
    }
}
