namespace EphemereGames.Commander.Simulation.Player
{
    using EphemereGames.Core.Visual;

    
    class PauseWorldMenu : CommanderContextualMenu
    {
        public PauseWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new TextContextualMenuChoice("resume", new Text("resume game", @"Pixelite") { SizeX = 2 }) { DoClick = DoResume });
            AddChoice(new TextContextualMenuChoice("new", new Text("new game", @"Pixelite") { SizeX = 2 }) { DoClick = DoNewGame });
        }


        public override void OnOpen()
        {
            SelectedIndex = 0;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    !Simulator.MultiverseMode &&
                    Simulator.WorldMode &&
                    ((WorldScene) Simulator.Scene).GetGamePausedSelected(Owner.InnerPlayer);
            }

            set
            {
                base.Visible = value;
            }
        }


        private void DoNewGame()
        {
            Main.CurrentWorld.StartNewGame(Owner.InnerPlayer);
        }


        private void DoResume()
        {
            Main.CurrentWorld.ResumeGame(Owner.InnerPlayer);
        }


        public override System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            if (SelectedIndex == 0)
                return Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.WorldToggleResume);
            else
                return Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.WorldToggleNewGame);
        }
    }
}
