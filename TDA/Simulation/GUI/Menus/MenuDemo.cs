namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuDemo : MenuAbstract
    {
        public CorpsCeleste CelestialBody;
        public int Scenario;
        
        private List<IVisible> NamesScores;
        private float PrioriteAffichage;


        public MenuDemo(Simulation simulation, float prioriteAffichage)
            : base(simulation)
        {
            PrioriteAffichage = prioriteAffichage;

            NamesScores = new List<IVisible>();

            for (int i = 0; i < 3; i++)
            {
                var namesScores = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Vector3.Zero);
                namesScores.Taille = 2;
                namesScores.VisualPriority = this.PrioriteAffichage;

                NamesScores.Add(namesScores);
            }
        }


        protected override Vector2 MenuSize
        {
            get
            {
                if (CelestialBody == null)
                    return Vector2.Zero;

                return new Vector2(190, 65);
            }
        }


        protected override Vector3 BasePosition
        {
            get
            {
                return (CelestialBody == null) ? Vector3.Zero : CelestialBody.Position - new Vector3(0, CelestialBody.Cercle.Rayon / 4, 0);
            }
        }


        public override void Draw()
        {
            if (CelestialBody == null || Scenario == -1)
                return;

            base.Draw();

            HighScores highscores = null;

            Simulation.Main.SaveGame.HighScores.TryGetValue(Scenario, out highscores);

            for (int i = 0; i < 3; i++)
            {
                if (highscores != null && highscores.Scores.Count > i)
                    NamesScores[i].Texte = highscores.Scores[i].Key.PadRight(6) + highscores.Scores[i].Value;
                else
                    NamesScores[i].Texte = "none  0";

                NamesScores[i].Position = this.Position + new Vector3(0, i * 20, 0);
                Simulation.Scene.ajouterScenable(NamesScores[i]);
            }


            Bulle.Draw(null);
        }
    }
}
