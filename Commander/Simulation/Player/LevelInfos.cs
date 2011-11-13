namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    
    class LevelInfos
    {
        private Text Title;
        private Text Highscore;
        private ScoreStars Stars;

        private Simulator Simulator;
        private SimPlayer Owner;


        public LevelInfos(Simulator simulator, SimPlayer owner)
        {
            Simulator = simulator;
            Owner = owner;

            Title = new Text(@"Pixelite")
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
        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            DrawInfos();

            if (!(CelestialBody is Planet))
                return;

            DrawHighScore();
        }


        private void DrawInfos()
        {
            if (Simulator.AvailableWarpsWorldMode.ContainsKey(CelestialBody))
            {
                Title.Data = WorldsFactory.GetWorldStringId(Simulator.AvailableWarpsWorldMode[CelestialBody]);
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

            if (descriptor == null)
                return;

            var highscore = Main.CurrentWorld.World.HighScores.GetHighScore(descriptor.Infos.Id);
            int score = highscore == null ? 0 : highscore.Score;

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
                if (!Simulator.AvailableLevelsWorldMode.ContainsKey(CelestialBody))
                    return null;

                return Main.CurrentWorld.World.GetLevelDescriptor(Simulator.AvailableLevelsWorldMode[CelestialBody]);
            }
        }


        private CelestialBody CelestialBody
        {
            get { return Owner.ActualSelection.CelestialBody; }
        }
    }
}
