namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldMenu
    {
        public CelestialBody CelestialBody;
        private Dictionary<string, LevelDescriptor> AvailableLevels;
        public PausedGameChoice PausedGameChoice;

        private Text Title;
        private Text Difficulty;
        private Text Highscore;
        private double VisualPriority;
        private ScoreStars Stars;

        public bool PausedGameMenuCheckedIn;
        public ContextualMenu PausedGameMenu;

        private List<ContextualMenuChoice> PausedGameChoices;

        private Simulator Simulator;


        public WorldMenu(Simulator simulator, double visualPriority, Dictionary<string, LevelDescriptor> availableLevels, Color color)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            AvailableLevels = availableLevels;

            PausedGameChoices = new List<ContextualMenuChoice>()
            {
                new TextContextualMenuChoice("resume", new Text("resume game", "Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice("new", new Text("new game", "Pixelite") { SizeX = 2 })
            };

            PausedGameMenu = new ContextualMenu(simulator, visualPriority, color, PausedGameChoices, 15);

            Title = new Text("Pixelite")
            {
                SizeX = 3,
                VisualPriority = VisualPriorities.Default.LevelNumber,
                Alpha = 200
            };

            Difficulty = new Text("Pixelite")
            {
                SizeX = 3,
                VisualPriority = VisualPriorities.Default.LevelNumber,
                Alpha = 200
            };

            Highscore = new Text("Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.LevelHighScore,
                Alpha = 200
            };

            Stars = new ScoreStars(Simulator.Scene, 0, VisualPriorities.Default.LevelHighScore).CenterIt();

            PausedGameMenuCheckedIn = false;
        }


        public bool PausedGameMenuVisible
        {
            get
            {
                return
                    CelestialBody != null &&
                    Main.GamePausedToWorld &&
                    Main.GameInProgress.Simulator.LevelDescriptor.Infos.Mission == CelestialBody.Name;
            }
        }


        public Vector3 Position
        {
            set { PausedGameMenu.Position = value; }
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

            if (PausedGameMenuCheckedIn && PausedGameMenuVisible)
            {
                PausedGameMenu.SelectedIndex = (int) PausedGameChoice;
                PausedGameMenu.Draw();
            }

            else
            {
                DrawHighScore();
            }
        }


        private void DrawInfos()
        {
            LevelDescriptor descriptor = AvailableLevels[CelestialBody.Name];

            Title.Data = descriptor.Infos.Mission;
            Title.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y - CelestialBody.Circle.Radius - 20, 0);
            Title.Origin = Title.Center;
            //Difficulty.Data = descriptor.Infos.Difficulty;
            //Difficulty.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y + CelestialBody.Circle.Radius + 16, 0);
            //Difficulty.Origin = Difficulty.Center;


            Simulator.Scene.Add(Title);
            //Simulator.Scene.Add(Difficulty);

        }


        private void DrawHighScore()
        {
            LevelDescriptor descriptor = AvailableLevels[CelestialBody.Name];

            int score = Main.SaveGameController.GetPlayerHighScore(descriptor.Infos.Id);

            Stars.Position = CelestialBody.Position + new Vector3(5, CelestialBody.Circle.Radius + 20, 0);
            Stars.BrightCount = descriptor.GetStarsCount(score);
            Stars.Draw();
        }
    }
}
