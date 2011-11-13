namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorWorldBuildMenu : EditorContextualMenu
    {
        public EditorWorldBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add level", 2, DoAddLevel),
                new EditorTextContextualMenuChoice("AddPinkHole", "Add warp", 2, DoAddWarp),
                new EditorTextContextualMenuChoice("Background", "Background", 2, DoBackground),
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, DoWaves),
                new EditorTextContextualMenuChoice("ChangeName", "change name", 2, DoChangeName)
                //new EditorTextContextualMenuChoice("Playtest", "Playtest", 2, DoPlaytest),
            };

            foreach (var c in choices)
                AddChoice(c);

            Visible = false;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    Simulator.EditorMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }


        public override void OnClose()
        {
            base.OnClose();

            Visible = false;
        }


        private void DoAddLevel()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                EditorLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.EditorController.ExecuteCommand(command);

            Main.CurrentWorld.AddLevel(command.CelestialBody);
            Visible = false;
        }


        private void DoAddWarp()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                EditorLevelGenerator.GeneratePinkHoleCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.EditorController.ExecuteCommand(command);
            Visible = false;
        }


        private void DoBackground()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorBackground"));
            Visible = false;
        }


        private void DoWaves()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorInfiniteWaves"));
            Visible = false;
        }


        //private void DoPlaytest()
        //{
        //    Simulator.EditorController.ExecuteCommand(new EditorPlaytestCommand(Owner));

        //    Main.CurrentWorld.World.Editing = false;
        //    Simulator.Data.Level.SyncDescriptor();
        //    Main.CurrentWorld.Initialize();
        //    Visible = false;
        //}


        private void DoChangeName()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorWorldName"));
            Visible = false;
        }
    }
}
