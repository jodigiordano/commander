namespace EphemereGames.Commander.Simulation
{
    class EditorLevelCBMenu : EditorCelestialBodyMenu
    {
        public EditorLevelCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoiceFirst(new EditorTextContextualMenuChoice("playtest", "playtest level", 2, DoPlaytestLevel));
            AddChoiceFirst(new EditorTextContextualMenuChoice("edit", "edit level", 2, DoEditLevel));

            AddChoice(new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, DoCelestialBodyAssets));
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody is Planet;
            }

            set { base.Visible = value; }
        }


        private void DoEditLevel()
        {
            Simulator.EditorController.ExecuteCommand(new EditorEditLevelCommand(Owner));

            Main.CurrentWorld.World.EditorMode = true;
            Main.CurrentWorld.World.Editing = true;
            Main.CurrentWorld.DoSelectActionEditor(Owner.InnerPlayer);
        }


        private void DoPlaytestLevel()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPlaytestLevelCommand(Owner));

            Main.CurrentWorld.World.EditorMode = true;
            Main.CurrentWorld.World.Editing = false;
            Main.CurrentWorld.DoSelectActionEditor(Owner.InnerPlayer);
        }


        private void DoCelestialBodyAssets()
        {
            Simulator.EditorController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAssets", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoAttributes()
        {
            Simulator.EditorController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoRemove()
        {
            base.DoRemove();

            Main.CurrentWorld.RemoveLevel(Owner.ActualSelection.CelestialBody);
        }
    }
}
