namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;


    class EditorBuildMenu : ContextualMenu
    {
        private SimPlayer Owner;


        public EditorBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 5)
        {
            Owner = owner;

            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add a planet", 2, new EditorCelestialBodyCommand("AddPlanet")),
                new EditorTextContextualMenuChoice("AddPinkHole", "Add a pink hole", 2, new EditorCelestialBodyCommand("AddPinkHole")),
                new EditorTextContextualMenuChoice("Background", "Background", 2, new EditorShowPanelCommand(PanelType.EditorBackground)),
                new EditorTextContextualMenuChoice("Turrets", "Turrets", 2, new EditorShowPanelCommand(PanelType.EditorTurrets)),
                new EditorTextContextualMenuChoice("PowerUps", "Power-ups", 2, new EditorShowPanelCommand(PanelType.EditorPowerUps)),
                new EditorTextContextualMenuChoice("Player", "Player", 2, new EditorShowPanelCommand(PanelType.EditorPlayer)),
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, new EditorShowPanelCommand(PanelType.EditorWaves)),
                new EditorTextContextualMenuChoice("Playtest", "Playtest", 2, new EditorSimpleCommand("Playtest")),
            };

            foreach (var c in choices)
                AddChoice(c);

            Visible = false;
        }


        public EditorCommand Selection
        {
            get
            {
                var command = ((EditorTextContextualMenuChoice) Choices[SelectedIndex]).Command;

                command.Owner = Owner;

                return command;
            }
        }


        public override void UpdateSelection()
        {
            Owner.ActualSelection.EditorBuildMenuCommand = Visible ? Selection : null;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorEditingMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }
    }
}
