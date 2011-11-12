namespace EphemereGames.Commander.Simulation
{
    class EditorPlanetCBMenu : EditorCelestialBodyMenu
    {
        public EditorPlanetCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, new EditorShowCBPanelCommand("EditorPlanetCBAssets")));
            AddChoice(new EditorTextContextualMenuChoice("Attributes", "Attributes", 2, new EditorShowCBPanelCommand("PlanetCBAttributesPanel")));
        }


        public override bool Visible
        {
            get { return base.Visible && Owner.ActualSelection.CelestialBody is Planet; }
            set { base.Visible = value; }
        }
    }
}
