namespace EphemereGames.Commander.Simulation
{
    class EditorPlanetCBMenu : EditorCelestialBodyMenu
    {
        public EditorPlanetCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, DoCelestialBodyAssets));
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.GameMode &&
                    Owner.ActualSelection.CelestialBody is Planet;
            }

            set { base.Visible = value; }
        }


        private void DoCelestialBodyAssets()
        {
            Simulator.EditorController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAssets", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoAttributes()
        {
            Simulator.EditorController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }
    }
}
