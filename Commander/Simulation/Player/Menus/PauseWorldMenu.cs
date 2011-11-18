namespace EphemereGames.Commander.Simulation.Player
{
    using EphemereGames.Core.Visual;

    
    class PauseWorldMenu : CommanderContextualMenu
    {
        public PauseWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new TextContextualMenuChoice("resume", new Text("resume game", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("new", new Text("new game", @"Pixelite") { SizeX = 2 }));
        }


        public PauseChoice Selection
        {
            get { return (PauseChoice) SelectedIndex; }
        }


        public override void OnOpen()
        {
            Owner.ActualSelection.PausedGameChoice = Selection;
        }


        public override void OnClose()
        {
            Owner.ActualSelection.PausedGameChoice = PauseChoice.None;
        }


        public override void NextChoice()
        {
            base.NextChoice();

            Owner.ActualSelection.PausedGameChoice = Selection;
        }


        public override void PreviousChoice()
        {
            base.PreviousChoice();

            Owner.ActualSelection.PausedGameChoice = Selection;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    !Simulator.EditingMode &&
                    ((WorldScene) Simulator.Scene).GetGamePausedSelected(Owner.InnerPlayer);
            }

            set
            {
                base.Visible = value;
            }
        }
    }
}
