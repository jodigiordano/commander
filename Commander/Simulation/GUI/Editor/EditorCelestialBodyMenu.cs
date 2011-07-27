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

        public static int NumberOfChoices;


        public EditorCelestialBodyMenu(Simulator simulator, Color color)
        {
            Simulator = simulator;

            Choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Move", "Move", 1, new EditorCelestialBodyCommand("Move")),
                new EditorTextContextualMenuChoice("Rotate", "Rotate", 1, new EditorCelestialBodyCommand("Rotate")),
                new EditorTextContextualMenuChoice("Shrink", "Shrink", 1, new EditorCelestialBodyCommand("Shrink")),
                new EditorTextContextualMenuChoice("Remove", "Remove", 1, new EditorCelestialBodyCommand("Remove")),
                new EditorToggleContextualMenuChoice("Speed",
                    new List<string>() { "Speed: 0", "Speed: 1", "Speed: 2", "Speed: 3", "Speed: 4", "Speed: 5", "Speed: 6", "Speed: 7", "Speed: 8", "Speed: 9", "Speed: 10" },
                    1,
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
                new EditorTextContextualMenuChoice("PushFirst", "Push first on path", 1, new EditorCelestialBodyCommand("PushFirst")),
                new EditorTextContextualMenuChoice("PushLast", "Push last on path", 1, new EditorCelestialBodyCommand("PushLast")),
                new EditorToggleContextualMenuChoice("AddToPath",
                    new List<string>() { "Add to starting path", "Remove from starting path" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("AddGravitationalTurret"),
                        new EditorCelestialBodyCommand("RemoveGravitationalTurret")
                    }),
                new EditorToggleContextualMenuChoice("Size",
                    new List<string>() { "Size: small", "Size: medium", "Size: big" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Normal },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Big },
                        new EditorCelestialBodyCommand("ToggleSize") { Size = Size.Small },
                    }),
                GetToggleAssets(),
                new EditorTextContextualMenuChoice("Verify", "Verify", 1, new EditorCelestialBodyCommand("Verify")),
                new EditorTextContextualMenuChoice("StartingPosition", "Starting Position", 1, new EditorCelestialBodyCommand("StartingPosition")),
                new EditorToggleContextualMenuChoice("ShowPath",
                    new List<string>() { "Show path", "Hide path" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("ShowPathPreview"),
                        new EditorCelestialBodyCommand("HidePathPreview")
                    }),
                new EditorToggleContextualMenuChoice("HasMoons",
                    new List<string>() { "Has moons: true", "Has moons: false" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("HasMoons") { HasMoons = false },
                        new EditorCelestialBodyCommand("HasMoons") { HasMoons = true }
                    }),
                new EditorToggleContextualMenuChoice("FollowPath",
                    new List<string>() { "Follow path: true", "Follow path: false" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("FollowPath") { FollowPath = false },
                        new EditorCelestialBodyCommand("FollowPath") { FollowPath = true }
                    }),
                new EditorToggleContextualMenuChoice("CanSelect",
                    new List<string>() { "Can select: true", "Can select: false" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("CanSelect") { CanSelect = false },
                        new EditorCelestialBodyCommand("CanSelect") { CanSelect = true }
                    }),
                new EditorToggleContextualMenuChoice("StraightLine",
                    new List<string>() { "Straight line: true", "Straight line: false" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("StraightLine") { StraightLine = false },
                        new EditorCelestialBodyCommand("StraightLine") { StraightLine = true }
                    }),
                new EditorToggleContextualMenuChoice("Invincible",
                    new List<string>() { "Invincible: true", "Invincible: false" },
                    1,
                    new List<EditorCommand>()
                    {
                        new EditorCelestialBodyCommand("Invincible") { Invincible = false },
                        new EditorCelestialBodyCommand("Invincible") { Invincible = true }
                    })
            };

            NumberOfChoices = Choices.Count;

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

            // sync speed
            for (int i = 0; i < EditorLevelGenerator.PossibleRotationTimes.Count; i++)
            {
                if (CelestialBody.Speed == EditorLevelGenerator.PossibleRotationTimes[i])
                {
                    ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("Speed")).SetChoice(i);
                    break;
                }
            }

            // sync add to path
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("AddToPath")).SetChoice(CelestialBody.StartingPathTurret == null ? 0 : 1);

            // sync sizes
            for (int i = 0; i < EditorLevelGenerator.PossibleSizes.Count; i++)
            {
                if ((int) CelestialBody.Circle.Radius == (int) EditorLevelGenerator.PossibleSizes[i])
                {
                    ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("Size")).SetChoice(i);
                    break;
                }
            }

            // sync assets
            for (int i = 0; i < EditorLevelGenerator.PossibleCelestialBodiesAssets.Count; i++)
            {
                if (CelestialBody.Image == null)
                    break;

                if (CelestialBody.PartialImageName == EditorLevelGenerator.PossibleCelestialBodiesAssets[i])
                {
                    ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("ToggleAsset")).SetChoice(i);
                    break;
                }
            }

            // sync show path
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("ShowPath")).SetChoice(CelestialBody.ShowPath ? 1 : 0);

            // sync has moons
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("HasMoons")).SetChoice(CelestialBody.HasMoons ? 0 : 1);

            // sync follow path
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("FollowPath")).SetChoice(CelestialBody.FollowPath ? 0 : 1);

            // sync can select
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("CanSelect")).SetChoice(CelestialBody.canSelect ? 0 : 1);

            // sync straight line
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("StraightLine")).SetChoice(CelestialBody.StraightLine ? 0 : 1);

            // sync invincible
            ((EditorToggleContextualMenuChoice) Menu.GetChoiceByName("Invincible")).SetChoice(CelestialBody.Invincible ? 0 : 1);
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


        private EditorToggleContextualMenuChoice GetToggleAssets()
        {
            List<string> names = new List<string>();

            foreach (var a in EditorLevelGenerator.PossibleCelestialBodiesAssets)
                names.Add("Asset: " + a);

            List<EditorCommand> commands = new List<EditorCommand>();

            for (int i = 0; i < EditorLevelGenerator.PossibleCelestialBodiesAssets.Count; i++)
            {
                var command = (i == EditorLevelGenerator.PossibleCelestialBodiesAssets.Count - 1) ?
                    new EditorCelestialBodyCommand("ToggleAsset") { AssetName = EditorLevelGenerator.PossibleCelestialBodiesAssets[0] } :
                    new EditorCelestialBodyCommand("ToggleAsset") { AssetName = EditorLevelGenerator.PossibleCelestialBodiesAssets[i+1] };

                commands.Add(command);
            }

            return new EditorToggleContextualMenuChoice("ToggleAsset", names, 1, commands);
        }
    }
}
