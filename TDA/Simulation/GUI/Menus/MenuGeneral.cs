namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MenuGeneral
    {
        public int Score;
        public int Cash;
        public int RemainingWaves;
        public double TimeNextWave;

        private Simulation Simulation;
        private IVisible WidgetScore;
        private IVisible WidgetCash;
        private IVisible WidgetRemainingWaves;
        private Vector3 Position;
        public MenuProchaineVague MenuNextWave;
        public SandGlass SandGlass;
        public Cursor Cursor;


        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave
        {
            set
            {
                MenuNextWave = new MenuProchaineVague(Simulation, value, this.Position - new Vector3(150, 30, 0), Preferences.PrioriteGUIPanneauGeneral + 0.049f);
            }
        }


        public MenuGeneral(Simulation simulation, Vector3 position)
        {
            this.Simulation = simulation;
            this.Position = position;

            this.SandGlass = new SandGlass(simulation.Main, simulation.Scene, 50000, this.Position, Preferences.PrioriteGUIPanneauGeneral + 0.05f);
            this.Cash = 0;


            WidgetCash = new IVisible
            (
                Cash + "M$",
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, 0, 0)
            );
            WidgetCash.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetCash.Taille = 3;

            WidgetRemainingWaves = new IVisible
            (
                RemainingWaves.ToString(),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, -40, 0)
            );
            WidgetRemainingWaves.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetRemainingWaves.Taille = 3;

            WidgetScore = new IVisible
            (
                Score.ToString(),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                Color.White,
                Position + new Vector3(30, 40, 0)
            );
            WidgetScore.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetScore.Taille = 3;
        }


        public void Update()
        {
            this.SandGlass.RemainingTime = this.TimeNextWave;
            this.SandGlass.Update();
        }


        public void Show()
        {
            Simulation.Scene.Add(WidgetCash);
            Simulation.Scene.Add(WidgetRemainingWaves);
            Simulation.Scene.Add(WidgetScore);

            SandGlass.Show();
        }


        public void Hide()
        {
            Simulation.Scene.Remove(WidgetCash);
            Simulation.Scene.Remove(WidgetRemainingWaves);
            Simulation.Scene.Remove(WidgetScore);

            SandGlass.Hide();
        }


        public void Draw()
        {

            WidgetCash.Texte = Cash + "M$";
            WidgetRemainingWaves.Texte = (RemainingWaves == -1) ? "Inf." : RemainingWaves.ToString();
            WidgetScore.Texte = Score.ToString();

            SandGlass.Draw();
            MenuNextWave.Draw(null);
        }
    }
}
