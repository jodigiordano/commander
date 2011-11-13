namespace EphemereGames.Commander.Simulation
{
    abstract class EditorContextualMenu : CommanderContextualMenu
    {
        public EditorContextualMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }
    }
}
