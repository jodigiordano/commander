namespace EphemereGames.Commander.Simulation
{
    class EditorPinkHoleCBMenu : EditorCelestialBodyMenu
    {
        public EditorPinkHoleCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
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
            Simulator.EditorController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPinkHoleCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }
    }
}
