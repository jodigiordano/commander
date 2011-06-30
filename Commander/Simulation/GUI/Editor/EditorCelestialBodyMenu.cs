namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorCelestialBodyMenu
    {
        public CelestialBody CelestialBody;

        public ContextualMenu Menu;

        private Simulator Simulator;
        private List<ContextualMenuChoice> Choices;


        public EditorCelestialBodyMenu(Simulator simulator, Color color)
        {
            Simulator = simulator;

            Choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Remove", new EditorCelestialBodyCommand("Remove")),
                new EditorToggleContextualMenuChoice(
                    new List<string>() { "Speed: 0", "Speed: 1", "Speed: 2", "Speed: 3", "Speed: 4", "Speed: 5", "Speed: 6", "Speed: 7", "Speed: 8", "Speed: 9", "Speed: 10" },
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
                new EditorTextContextualMenuChoice("Push first on path", new EditorCelestialBodyCommand("PushFirst")),
                new EditorTextContextualMenuChoice("Push last on path", new EditorCelestialBodyCommand("PushLast")),
                new EditorToggleContextualMenuChoice(
                    new List<string>() { "Add to starting path", "Remove from starting path" },
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("AddGravitationalTurret"),
                        new EditorCelestialBodyCommand("RemoveGravitationalTurret")
                    }),
                new EditorToggleContextualMenuChoice(
                    new List<string>() { "Size: small", "Size: medium", "Size: big" },
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Normal },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Big },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Small },
                    }),
                new EditorTextContextualMenuChoice("Change Asset", new EditorCelestialBodyCommand("ChangeAsset")),
                new EditorTextContextualMenuChoice("Modify Trajectory", new EditorCelestialBodyCommand("ModifyTrajectory")),
                new EditorTextContextualMenuChoice("Move along trajectory", new EditorCelestialBodyCommand("MoveAlongTrajectory")),
                new EditorTextContextualMenuChoice("Verify", new EditorCelestialBodyCommand("Verify")),

            };

            Menu = new ContextualMenu(Simulator, Preferences.PrioriteGUIPanneauGeneral - 0.001, color, Choices, 5);
        }


        public bool Visible
        {
            get { return Menu.Visible && CelestialBody != null; }
            set { Menu.Visible = value; }
        }


        public void SyncData()
        {
            if (CelestialBody == null)
                return;

            for (int i = 0; i < EditorLevelGenerator.PossibleRotationTimes.Count; i++)
            {
                if (CelestialBody.Speed == EditorLevelGenerator.PossibleRotationTimes[i])
                {
                    ((EditorToggleContextualMenuChoice) Choices[1]).SetChoice(i);
                    break;
                }
            }

            ((EditorToggleContextualMenuChoice) Choices[4]).SetChoice(CelestialBody.StartingPathTurret == null ? 0 : 1);

            for (int i = 0; i < EditorLevelGenerator.PossibleSizes.Count; i++)
            {
                if ((int) CelestialBody.Circle.Radius == (int) EditorLevelGenerator.PossibleSizes[i])
                {
                    ((EditorToggleContextualMenuChoice) Choices[5]).SetChoice(i);
                    break;
                }
            }


        }


        public void Update()
        {
            if (CelestialBody != null)
                Menu.Position = CelestialBody.Position;
        }


        public void Draw()
        {
            if (!Visible)
                return;

            Menu.Draw();
        }
    }
}
