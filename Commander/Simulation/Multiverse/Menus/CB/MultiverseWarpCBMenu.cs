namespace EphemereGames.Commander.Simulation
{
    class MultiverseWarpCBMenu : MultiverseCelestialBodyMenu
    {
        public MultiverseWarpCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoiceFirst(new EditorTextContextualMenuChoice("editWarp", "edit warp", 2, DoEditWarp)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseEditWarp) });
            
            AddChoiceFirst(new EditorTextContextualMenuChoice("jumptoWarp", "jump to warp", 2, DoJumpToWarp)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseJumpToWorld) });

            
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody is PinkHole;
            }

            set { base.Visible = value; }
        }


        private void DoEditWarp()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelCBShowCommand(Owner, "EditorEditWarp", Owner.ActualSelection.CelestialBody, Simulator));
        }


        private void DoJumpToWarp()
        {
            Main.MultiverseController.JumpToWorld(
                Main.CurrentWorld.CBtoWarp[Owner.ActualSelection.CelestialBody], "World");
        }


        protected override void DoAttributes()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPinkHoleCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoRemove()
        {
            base.DoRemove();

            Main.CurrentWorld.RemoveWarp(Owner.ActualSelection.CelestialBody);
        }
    }
}
