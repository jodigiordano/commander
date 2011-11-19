namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HighscoresPanel : SlideshowPanel
    {
        public int WorldId;
        public int LevelId;

        private Simulator Simulator;

        public HighscoresPanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(600, 600), VisualPriorities.Default.Panel, Color.White)
        {
            Simulator = simulator;

            Slider.SpaceForLabel = 150;
            Slider.SpaceForValue = 100;
            Slider.SetLabel("Page #");

            Alpha = 0;
        }


        public override void Open()
        {
            ClearWidgets();
            SetTitle("Highscores for: " + WorldId + "-" + LevelId);

            var highscores = Main.WorldsFactory.GetWorld(WorldId).HighScores.GetAllHighScores(LevelId);

            for (int i = 0; i < MathHelper.Clamp(highscores.Scores.Count / 10, 1, 10); i++)
                AddWidget("scores" + i, new HighscoresSubPanel(
                    Simulator,
                    new Vector2(Dimension.X, Dimension.Y),
                    VisualPriority + 0.00001,
                    Color,
                    highscores,
                    i * 10,
                    Math.Min(i * 10 + 10, highscores.Scores.Count)));

            base.Open();
        }


        private class HighscoresSubPanel : GridPanel
        {
            public HighscoresSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color, LevelScores levelScores, int i, int j)
                : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
            {
                OnlyShowWidgets = true;
                NbColumns = 3;
                Alpha = 0;
                DistanceBetweenTwoChoices = 15;
                
                AddWidget("Number", new Label(new Text("#", "Pixelite") { SizeX = 2, Color = Colors.Default.AlienBright }));
                AddWidget("Player", new Label(new Text("Player", "Pixelite") { SizeX = 2, Color = Colors.Default.AlienBright }));
                AddWidget("Score", new Label(new Text("Score", "Pixelite") { SizeX = 2, Color = Colors.Default.AlienBright }));

                for (; i < j; i++)
                {
                    AddWidget("Number" + i, new Label(new Text((i+1).ToString(), "Pixelite") { SizeX = 2 }));
                    AddWidget("Player" + i, new Label(new Text(levelScores.Scores[i].Player, "Pixelite") { SizeX = 2 }));
                    AddWidget("Score" + i, new Label(new Text(levelScores.Scores[i].Score.ToString(), "Pixelite") { SizeX = 2 }));
                }
            }
        }
    }
}
