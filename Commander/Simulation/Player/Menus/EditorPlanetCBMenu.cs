namespace EphemereGames.Commander.Simulation.Player
{
    class EditorPlanetCBMenu : EditorCelestialBodyMenu
    {
        public EditorPlanetCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, new EditorShowPanelCommand(PanelType.EditorCelestialBodyAssets)));
        }


        public override bool Visible
        {
            get { return base.Visible && Owner.ActualSelection.CelestialBody is Planet; }
            set { base.Visible = value; }
        }
    }
}
