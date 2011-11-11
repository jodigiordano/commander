namespace EphemereGames.Commander.Simulation
{
    abstract class CommanderContextualMenu : ContextualMenu
    {
        protected SimPlayer Owner;


        public CommanderContextualMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 5)
        {
            Owner = owner;
        }


        public override void NextChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.NextChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }


        public override void PreviousChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.PreviousChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }
    }
}
