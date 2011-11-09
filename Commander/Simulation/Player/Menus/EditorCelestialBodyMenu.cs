namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;


    class EditorCelestialBodyMenu : ContextualMenu
    {
        SimPlayer Owner;


        public EditorCelestialBodyMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 5)
        {
            Owner = owner;

            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Move", "Move", 2, new EditorCelestialBodyCommand("Move")),
                new EditorTextContextualMenuChoice("Rotate", "Rotate", 2, new EditorCelestialBodyCommand("Rotate")),
                new EditorTextContextualMenuChoice("Trajectory", "Trajectory", 2, new EditorCelestialBodyCommand("Trajectory")),
                new EditorTextContextualMenuChoice("Remove", "Remove", 2, new EditorCelestialBodyCommand("Remove")),
                new EditorToggleContextualMenuChoice("Speed",
                    new List<string>() { "Speed: 0", "Speed: 1", "Speed: 2", "Speed: 3", "Speed: 4", "Speed: 5", "Speed: 6", "Speed: 7", "Speed: 8", "Speed: 9", "Speed: 10" },
                    2,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[1] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[2] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[3] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[4] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[5] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[6] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[7] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[8] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[9] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[10] },
                        new EditorCelestialBodyCommand("ToggleSpeed") { Speed = EditorLevelGenerator.PossibleRotationTimes[0] }
                    }),
                new EditorTextContextualMenuChoice("PushFirst", "Push first on path", 2, new EditorCelestialBodyCommand("PushFirst")),
                new EditorTextContextualMenuChoice("PushLast", "Push last on path", 2, new EditorCelestialBodyCommand("PushLast")),
                new EditorToggleContextualMenuChoice("Size",
                    new List<string>() { "Size: small", "Size: medium", "Size: big" },
                    2,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Normal },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Big },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Small },
                    }),
                new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, new EditorShowPanelCommand(PanelType.EditorCelestialBodyAssets)),
                new EditorTextContextualMenuChoice("Attributes", "Attributes", 2, new EditorShowPanelCommand(PanelType.EditorCelestialBodyAttributes))
            };

            foreach (var c in choices)
                AddChoice(c);
        }


        public EditorCommand Selection
        {
            get
            {
                var choice = GetCurrentChoice();
                EditorCommand command = null;

                if (choice is EditorTextContextualMenuChoice)
                    command = ((EditorTextContextualMenuChoice) choice).Command;
                else
                    command = ((EditorToggleContextualMenuChoice) choice).Command;

                command.Owner = Owner;
                return command;
            }
        }


        public override void UpdateSelection()
        {
            Owner.ActualSelection.EditorCelestialBodyMenuCommand = Visible ? Selection : null;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorEditingMode &&
                    Owner.ActualSelection.CelestialBody != null;
            }
            set { base.Visible = value; }
        }
    }
}
