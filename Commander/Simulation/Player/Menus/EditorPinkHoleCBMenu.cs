namespace EphemereGames.Commander.Simulation.Player
{
    class EditorPinkHoleCBMenu : EditorCelestialBodyMenu
    {
        public EditorPinkHoleCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }


        public override bool Visible
        {
            get { return base.Visible && Owner.ActualSelection.CelestialBody is PinkHole; }
            set { base.Visible = value; }
        }
    }
}
