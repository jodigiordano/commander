namespace EphemereGames.Commander.Simulation
{
    class MultiverseWorldMenu : MultiverseContextualMenu
    {
        public MultiverseWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("JumpToWorld", "jump to world", 2, DoJumpToWorld) {
                HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseJumpToWorld) });

            Visible = false;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.MultiverseMode &&
                    !Simulator.GameMode &&
                    !Simulator.EditingMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }


        private void DoJumpToWorld()
        {
            if (Main.PlayersController.MultiverseData.IsLoggedIn)
            {
                ((JumpToWorldPanel) Simulator.Data.Panels["JumpToWorld"]).From = Simulator.Scene.Name;
                Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "JumpToWorld"));
            }

            else
            {
                Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "Login"));
            }

            Visible = false;
        }
    }
}
