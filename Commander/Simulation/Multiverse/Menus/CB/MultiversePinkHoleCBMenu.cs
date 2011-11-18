namespace EphemereGames.Commander.Simulation
{
    class MultiversePinkHoleCBMenu : MultiverseCelestialBodyMenu
    {
        public MultiversePinkHoleCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.GameMode &&
                    Owner.ActualSelection.CelestialBody is PinkHole;
            }

            set { base.Visible = value; }
        }


        protected override void DoAttributes()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPinkHoleCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }
    }
}
