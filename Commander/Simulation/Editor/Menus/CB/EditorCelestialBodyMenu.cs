namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class EditorCelestialBodyMenu : EditorContextualMenu
    {
        public EditorCelestialBodyMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
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
                new EditorTextContextualMenuChoice("RemoveFromPath", "Remove from path", 2, new EditorCelestialBodyCommand("RemoveFromPath")),
                new EditorToggleContextualMenuChoice("Size",
                    new List<string>() { "Size: small", "Size: normal", "Size: big" },
                    2,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Normal },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Big },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Small },
                    })
            };

            foreach (var c in choices)
                AddChoice(c);
        }


        public override void OnOpen()
        {
            SelectedIndex = 0;

            var speed = (EditorToggleContextualMenuChoice) GetChoiceByName("Speed");

            for (int i = 0; i < EditorLevelGenerator.PossibleRotationTimes.Count; i++)
                if (Owner.ActualSelection.CelestialBody.Speed == EditorLevelGenerator.PossibleRotationTimes[i])
                {
                    speed.SetChoice(i);
                    break;
                }
            
            var size = (EditorToggleContextualMenuChoice) GetChoiceByName("Size");

            size.SetChoice(
                Owner.ActualSelection.CelestialBody.Size == Size.Small ? 0 :
                Owner.ActualSelection.CelestialBody.Size == Size.Normal ? 1 : 2);
        }


        protected override EditorCommand Selection
        {
            get
            {
                var choice = GetCurrentChoice();

                if (choice is EditorTextContextualMenuChoice)
                    return ((EditorTextContextualMenuChoice) choice).Command;
                else
                    return ((EditorToggleContextualMenuChoice) choice).Command;
            }
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
