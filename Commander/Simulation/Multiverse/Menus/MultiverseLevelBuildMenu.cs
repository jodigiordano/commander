namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class MultiverseLevelBuildMenu : MultiverseContextualMenu
    {
        public MultiverseLevelBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add planet", 2, DoAddPlanet),
                new EditorTextContextualMenuChoice("AddPinkHole", "Add pink hole", 2, DoAddPinkHole),
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
                    Simulator.EditingMode &&
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
                MultiverseLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody));

            ((Planet) command.CelestialBody).IsALevel = false;

            Simulator.MultiverseController.ExecuteCommand(command);

            Visible = false;
        }


        private void DoAddPinkHole()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                MultiverseLevelGenerator.GeneratePinkHoleCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.MultiverseController.ExecuteCommand(command);

            Visible = false;
        }


        private void DoBackground()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorBackground"));

            Visible = false;
        }


        private void DoWaves()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorWaves"));

            Visible = false;
        }


        private void DoPlaytest()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPlaytestCommand(Owner));

            Simulator.EditingMode = false;
            Simulator.Data.Level.SyncDescriptor();
            Simulator.Initialize();
            Simulator.SyncPlayers();

            Visible = false;
        }


        private void DoTurrets()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorTurrets"));

            Visible = false;
        }


        private void DoPowerUps()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorPowerUps"));

            Visible = false;
        }


        private void DoPlayer()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorPlayer"));

            Visible = false;
        }
    }
}
