namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorLevelBuildMenu : EditorContextualMenu
    {
        public EditorLevelBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add a planet", 2, DoAddPlanet),
                new EditorTextContextualMenuChoice("AddPinkHole", "Add a pink hole", 2, DoAddPinkHole),
                new EditorTextContextualMenuChoice("Background", "Background", 2, DoBackground),
                new EditorTextContextualMenuChoice("Turrets", "Turrets", 2, DoTurrets),
                new EditorTextContextualMenuChoice("Player", "Player", 2, DoPlayer),
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, DoWaves),
                new EditorTextContextualMenuChoice("Playtest", "Playtest", 2, DoPlaytest),
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
                    Simulator.GameMode &&
                    Simulator.EditorEditingMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }


        public override void OnClose()
        {
            base.OnClose();

            Visible = false;
        }


        private void DoAddPlanet()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                EditorLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.EditorController.ExecuteCommand(command);

            Visible = false;
        }


        private void DoAddPinkHole()
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
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorWaves"));

            Visible = false;
        }


        private void DoPlaytest()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPlaytestCommand(Owner));

            Simulator.EditMode = false;
            Simulator.Data.Level.SyncDescriptor();
            Simulator.Initialize();
            Simulator.SyncPlayers();

            Visible = false;
        }


        private void DoTurrets()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorTurrets"));

            Visible = false;
        }


        private void DoPowerUps()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorPowerUps"));

            Visible = false;
        }


        private void DoPlayer()
        {
            Simulator.EditorController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorPlayer"));

            Visible = false;
        }
    }
}
