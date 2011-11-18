namespace EphemereGames.Commander.Simulation
{
    class MultiversePlanetCBMenu : MultiverseCelestialBodyMenu
    {
        public MultiversePlanetCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
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
                    Owner.ActualSelection.CelestialBody is Planet &&
                    !((Planet) Owner.ActualSelection.CelestialBody).IsALevel;
            }

            set { base.Visible = value; }
        }


        private void DoCelestialBodyAssets()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAssets", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoAttributes()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }
    }
}
