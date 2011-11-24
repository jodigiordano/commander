namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;

    
    abstract class MultiverseContextualMenu : CommanderContextualMenu
    {
        public MultiverseContextualMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

        }


        public override List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            return Choices[SelectedIndex].HelpBarMessage;
        }
    }
}
