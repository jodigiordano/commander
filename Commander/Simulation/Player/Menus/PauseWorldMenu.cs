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


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.Scene is WorldScene &&
                    !Simulator.EditorMode &&
                    ((WorldScene) Simulator.Scene).GetGamePausedSelected(Owner.InnerPlayer);
            }

            set
            {
                base.Visible = value;
            }
        }
    }
}
