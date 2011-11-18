namespace EphemereGames.Commander.Simulation
{
    abstract class MultiverseContextualMenu : CommanderContextualMenu
    {
        public MultiverseContextualMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }
    }
}
