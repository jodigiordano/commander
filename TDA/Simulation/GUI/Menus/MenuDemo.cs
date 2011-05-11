namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class MenuDemo : MenuAbstract
    {
        public CorpsCeleste CelestialBody;
        public ScenarioDescriptor Scenario;
        public GameAction Action;

        private Text ResumeGame;
        private Text NewGame;
        private Text Title;
        private Text Difficulty;
        private Text Highscore;
        private Image Selector;
        private Image[] HighscoreStars;
        private float VisualPriority;


        public MenuDemo(Simulation simulation, float visualPriority)
            : base(simulation)
        {
            VisualPriority = visualPriority;

            ResumeGame = new Text("resume game", "Pixelite", Color.White, Vector3.Zero);
            ResumeGame.SizeX = 2;
            ResumeGame.VisualPriority = visualPriority;

            NewGame = new Text("new game", "Pixelite", Color.White, Vector3.Zero);
            NewGame.VisualPriority = visualPriority;
            NewGame.SizeX = 2;

            Selector = new Image("PixelBlanc", Vector3.Zero);
            Selector.Size = new Vector2(190, 30);
            Selector.Color = Color.Green;
            Selector.Color.A = 230;
            Selector.VisualPriority = visualPriority + 0.01f;
            Selector.Origin = Vector2.Zero;

            Title = new Text("Pixelite");
            Title.SizeX = 4;
            Title.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            Title.Color.A = 200;

            Difficulty = new Text("Pixelite");
            Difficulty.SizeX = 3;
            Difficulty.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            Difficulty.Color.A = 200;

            Highscore = new Text("Pixelite");
            Highscore.SizeX = 2;
            Highscore.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            Highscore.Color.A = 200;

            HighscoreStars = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                var star = new Image("Star", Vector3.Zero);
                star.SizeX = 0.25f;
                star.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
                star.Color.A = 200;
                HighscoreStars[i] = star;
            }
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (CelestialBody == null)
                    return Vector2.Zero;

                return new Vector2(190, 60);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position - new Vector3(0, CelestialBody.Cercle.Radius / 4, 0);
            }
        }


        public override void Draw()
        {
            if (CelestialBody == null || Scenario == null || Action == GameAction.None)
                return;


            DrawInfos();


            if (CelestialBody is TrouRose)
                return;


            DrawHighScore();


            if (Simulation.Main.GameInProgress != null &&
                !Simulation.Main.GameInProgress.EstTerminee &&
                Simulation.CelestialBodyPausedGame != null && Simulation.CelestialBodyPausedGame.Nom == CelestialBody.Nom)
            {
                base.Draw();
                DrawGameInProgress();
            }
        }


        private void DrawGameInProgress()
        {
            NewGame.Position = Position + new Vector3(5, 3, 0);
            ResumeGame.Position = Position + new Vector3(5, 33, 0);
            Selector.Position = (Action == GameAction.Resume) ? Position + new Vector3(0, 30, 0) : Position;


            Simulation.Scene.ajouterScenable(ResumeGame);
            Simulation.Scene.ajouterScenable(NewGame);
            Simulation.Scene.ajouterScenable(Selector);
            Bulle.Draw(null);
        }


        private void DrawInfos()
        {
            Title.Data = Scenario.Mission;
            Title.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y - CelestialBody.Cercle.Radius - 32, 0);
            Title.Origin = Title.Center;
            Difficulty.Data = Scenario.Difficulty;
            Difficulty.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y + CelestialBody.Cercle.Radius + 16, 0);
            Difficulty.Origin = Difficulty.Center;


            Simulation.Scene.ajouterScenable(Title);
            Simulation.Scene.ajouterScenable(Difficulty);

        }


        private void DrawHighScore()
        {
            HighScores highscores = null;

            Simulation.Main.SaveGame.HighScores.TryGetValue(Scenario.Id, out highscores);

            Highscore.Data = (highscores == null) ? "highscore: 0" : "highscore: " + highscores.Scores[0].Value;
            Highscore.Origin = Highscore.Center;
            Highscore.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y + CelestialBody.Cercle.Radius + 40, 0);

            int nbStars = (highscores == null) ? 0 : Scenario.NbStars(highscores.Scores[0].Value);

            for (int i = 0; i < 3; i++)
            {
                HighscoreStars[i].Position = new Vector3(CelestialBody.Position.X - 50 + i * 50, CelestialBody.Position.Y + CelestialBody.Cercle.Radius + 70, 0);
                HighscoreStars[i].Color.A = (i < nbStars) ? (byte) 200 : (byte) 50;
            }


            Simulation.Scene.ajouterScenable(Highscore);
            Simulation.Scene.ajouterScenable(HighscoreStars[0]);
            Simulation.Scene.ajouterScenable(HighscoreStars[1]);
            Simulation.Scene.ajouterScenable(HighscoreStars[2]);
        }
    }
}
