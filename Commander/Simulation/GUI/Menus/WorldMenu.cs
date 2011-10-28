namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldMenu
    {
        public CelestialBody CelestialBody;
        public PausedGameChoice PausedGameChoice;
        public EditorWorldChoice EditorChoice;

        private Text Title;
        private Text Difficulty;
        private Text Highscore;
        private double VisualPriority;
        private ScoreStars Stars;

        public bool MenuCheckedIn;
        public ContextualMenu PausedGameMenu;
        public ContextualMenu EditorMenu;

        private List<ContextualMenuChoice> PausedGameChoices;
        private List<ContextualMenuChoice> EditorChoices;

        private Simulator Simulator;
        private bool AlternateSelectedText;


        public WorldMenu(Simulator simulator, double visualPriority, Color color)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;

            AlternateSelectedText = color == Colors.Spaceship.Yellow;


            PausedGameChoices = new List<ContextualMenuChoice>()
            {
                new TextContextualMenuChoice("resume", new Text("resume game", @"Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice("new", new Text("new game", @"Pixelite") { SizeX = 2 })
            };

            PausedGameMenu = new ContextualMenu(simulator, visualPriority, color, PausedGameChoices, 15);

            EditorChoices = new List<ContextualMenuChoice>()
            {
                new TextContextualMenuChoice("edit", new Text("edit", @"Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice("playtest", new Text("playtest", @"Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice("save", new Text("save", @"Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice("reset", new Text("reset", @"Pixelite") { SizeX = 2 }),
            };

            EditorMenu = new ContextualMenu(simulator, visualPriority, color, EditorChoices, 15);

            Title = new Text(@"Pixelite")
            {
                SizeX = 3,
                VisualPriority = VisualPriorities.Default.LevelNumber,
                Alpha = 200
            };

            Difficulty = new Text(@"Pixelite")
            {
                SizeX = 3,
                VisualPriority = VisualPriorities.Default.LevelNumber,
                Alpha = 200
            };

            Highscore = new Text(@"Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.LevelHighScore,
                Alpha = 200
            };

            Stars = new ScoreStars(Simulator.Scene, 0, VisualPriorities.Default.LevelHighScore).CenterIt();

            MenuCheckedIn = false;
        }


        public bool PausedGameMenuVisible
        {
            get
            {
                if (Main.CurrentWorld == null)
                    return false;

                foreach (var p in Inputs.ConnectedPlayers)
                    if (Main.CurrentWorld.GetGamePausedSelected((Commander.Player) p))
                        return true;

                return false;
            }
        }


        public bool EditorMenuVisible
        {
            get
            {
                return
                    CelestialBody != null &&
                    Simulator.EditorWorldMode;
            }
        }


        public Vector3 Position
        {
            set
            {
                PausedGameMenu.Position = value;
                EditorMenu.Position = value;
            }
        }


        public void Update()
        {

        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            DrawInfos();

            if (CelestialBody is PinkHole)
                return;

            if (EditorMenuVisible)
            {
                int newIndex = (int) EditorChoice;

                //if (AlternateSelectedText && EditorMenu.Choices.Count > 0 && newIndex != EditorMenu.SelectedIndex)
                //{
                //    if (EditorMenu.SelectedIndex >= 0)
                //        ((TextContextualMenuChoice) PausedGameMenu.Choices[PausedGameMenu.SelectedIndex]).SetColor(Color.White);

                //    if (newIndex >= 0)
                //        ((TextContextualMenuChoice) PausedGameMenu.Choices[newIndex]).SetColor(Colors.Spaceship.Selected);
                //}

                EditorMenu.SelectedIndex = newIndex;
                EditorMenu.Draw();
            }

            else if (MenuCheckedIn && PausedGameMenuVisible)
            {
                int newIndex = (int) PausedGameChoice;

                if (AlternateSelectedText && PausedGameMenu.Choices.Count > 0 && newIndex != PausedGameMenu.SelectedIndex)
                {
                    if (PausedGameMenu.SelectedIndex >= 0)
                        ((TextContextualMenuChoice) PausedGameMenu.Choices[PausedGameMenu.SelectedIndex]).SetColor(Color.White);

                    if (newIndex >= 0)
                        ((TextContextualMenuChoice) PausedGameMenu.Choices[newIndex]).SetColor(Colors.Spaceship.Selected);
                }

                PausedGameMenu.SelectedIndex = newIndex;
                PausedGameMenu.Draw();
            }

            else
            {
                DrawHighScore();
            }
        }


        private void DrawInfos()
        {
            if (Simulator.AvailableWarpsWorldMode.ContainsKey(CelestialBody))
            {
                Title.Data = LevelsFactory.GetWorldStringId(Simulator.AvailableWarpsWorldMode[CelestialBody]);
            }

            else if (Simulator.AvailableLevelsWorldMode.ContainsKey(CelestialBody))
            {
                var descriptor = CurrentLevelDescriptor;

                Title.Data = Main.CurrentWorld.World.GetLevelStringId(descriptor.Infos.Id);
            }

            Title.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y - CelestialBody.Circle.Radius - 20, 0);
            Title.Origin = Title.Center;

            Simulator.Scene.Add(Title);
        }


        private void DrawHighScore()
        {
            LevelDescriptor descriptor = CurrentLevelDescriptor;

            int score = Main.SaveGameController.GetPlayerHighScore(descriptor.Infos.Id);

            Stars.Position = CelestialBody.Position + new Vector3(5, CelestialBody.Circle.Radius + 20, 0);
            Stars.BrightCount = descriptor.GetStarsCount(score);
            Stars.Draw();

            if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
            {
                Highscore.Data = score.ToString();
                Highscore.CenterIt();
                Highscore.Position = CelestialBody.Position + new Vector3(0, CelestialBody.Circle.Radius + 50, 0);
                Simulator.Scene.Add(Highscore);
            }
        }


        private LevelDescriptor CurrentLevelDescriptor
        {
            get
            {
                return Main.CurrentWorld.World.GetLevelDescriptor(Simulator.AvailableLevelsWorldMode[CelestialBody]);
            }
        }
    }
}
