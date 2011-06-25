﻿namespace EphemereGames.Commander.Simulation
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
        private Image[] HighscoreStars;
        private double VisualPriority;

        private ContextualMenu PausedGameMenu;
        private List<ContextualMenuChoice> PausedGameChoices;

        private Simulator Simulator;


        public WorldMenu(Simulator simulator, double visualPriority, Dictionary<string, LevelDescriptor> availableLevels, Color color)
        {
            Simulator = simulator;
            VisualPriority = visualPriority;
            AvailableLevels = availableLevels;

            PausedGameChoices = new List<ContextualMenuChoice>()
            {
                new TextContextualMenuChoice(new Text("resume game", "Pixelite") { SizeX = 2 }),
                new TextContextualMenuChoice(new Text("new game", "Pixelite") { SizeX = 2 })
            };

            PausedGameMenu = new ContextualMenu(simulator, visualPriority, color, PausedGameChoices, 15);

            Title = new Text("Pixelite");
            Title.SizeX = 4;
            Title.VisualPriority = Preferences.PrioriteFondEcran - 0.00001;
            Title.Color.A = 200;

            Difficulty = new Text("Pixelite");
            Difficulty.SizeX = 3;
            Difficulty.VisualPriority = Preferences.PrioriteFondEcran - 0.00001;
            Difficulty.Color.A = 200;

            Highscore = new Text("Pixelite");
            Highscore.SizeX = 2;
            Highscore.VisualPriority = Preferences.PrioriteFondEcran - 0.00001;
            Highscore.Color.A = 200;

            HighscoreStars = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                var star = new Image("Star", Vector3.Zero);
                star.SizeX = 0.25f;
                star.VisualPriority = Preferences.PrioriteFondEcran - 0.00001;
                star.Color.A = 200;
                HighscoreStars[i] = star;
            }
        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            DrawInfos();

            if (CelestialBody is PinkHole)
                return;

            DrawHighScore();


            if (Main.GameInProgress != null && !Main.GameInProgress.IsFinished &&
                Main.GameInProgress.Simulator.LevelDescriptor.Mission == CelestialBody.Name &&
                Simulator.Scene.EnableInputs)
            {
                PausedGameMenu.Position = CelestialBody.Position;
                PausedGameMenu.SelectedIndex = (int) PausedGameChoice;
                PausedGameMenu.Draw();
            }
        }


        private void DrawInfos()
        {
            LevelDescriptor descriptor = AvailableLevels[CelestialBody.Name];

            Title.Data = descriptor.Mission;
            Title.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y - CelestialBody.Circle.Radius - 32, 0);
            Title.Origin = Title.Center;
            Difficulty.Data = descriptor.Difficulty;
            Difficulty.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y + CelestialBody.Circle.Radius + 16, 0);
            Difficulty.Origin = Difficulty.Center;


            Simulator.Scene.Add(Title);
            Simulator.Scene.Add(Difficulty);

        }


        private void DrawHighScore()
        {
            LevelDescriptor descriptor = AvailableLevels[CelestialBody.Name];
            HighScores highscores = null;

            Main.SaveGame.HighScores.TryGetValue(descriptor.Id, out highscores);

            Highscore.Data = (highscores == null) ? "highscore: 0" : "highscore: " + highscores.Scores[0].Value;
            Highscore.Origin = Highscore.Center;
            Highscore.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y + CelestialBody.Circle.Radius + 40, 0);

            int nbStars = (highscores == null) ? 0 : descriptor.NbStars(highscores.Scores[0].Value);

            for (int i = 0; i < 3; i++)
            {
                HighscoreStars[i].Position = new Vector3(CelestialBody.Position.X - 50 + i * 50, CelestialBody.Position.Y + CelestialBody.Circle.Radius + 70, 0);
                HighscoreStars[i].Color.A = (i < nbStars) ? (byte) 200 : (byte) 50;
            }


            Simulator.Scene.Add(Highscore);
            Simulator.Scene.Add(HighscoreStars[0]);
            Simulator.Scene.Add(HighscoreStars[1]);
            Simulator.Scene.Add(HighscoreStars[2]);
        }
    }
}
