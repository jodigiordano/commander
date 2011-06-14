namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class GameMenu
    {
        public int Score;
        public int Cash;
        public int RemainingWaves;
        public double TimeNextWave;

        private Simulation Simulation;
        private Text WidgetScore;
        private Text WidgetCash;
        private Text WidgetRemainingWaves;
        private Vector3 Position;
        public NextWaveMenu MenuNextWave;
        public SandGlass SandGlass;
        public Cursor Cursor;


        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave
        {
            set
            {
                MenuNextWave = new NextWaveMenu(Simulation, value, this.Position - new Vector3(150, 30, 0), Preferences.PrioriteGUIPanneauGeneral + 0.049f);
            }
        }


        public GameMenu(Simulation simulation, Vector3 position)
        {
            this.Simulation = simulation;
            this.Position = position;

            this.SandGlass = new SandGlass(simulation.Main, simulation.Scene, 50000, this.Position, Preferences.PrioriteGUIPanneauGeneral + 0.05f);
            this.Cash = 0;


            WidgetCash = new Text
            (
                Cash + "M$",
                "Pixelite",
                Color.White,
                Position + new Vector3(30, 0, 0)
            );
            WidgetCash.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetCash.SizeX = 3;

            WidgetRemainingWaves = new Text
            (
                RemainingWaves.ToString(),
                "Pixelite",
                Color.White,
                Position + new Vector3(30, -40, 0)
            );
            WidgetRemainingWaves.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetRemainingWaves.SizeX = 3;

            WidgetScore = new Text
            (
                Score.ToString(),
                "Pixelite",
                Color.White,
                Position + new Vector3(30, 40, 0)
            );
            WidgetScore.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetScore.SizeX = 3;
        }


        public void Update()
        {
            this.SandGlass.RemainingTime = this.TimeNextWave;
            this.SandGlass.Update();
        }


        //public void Show()
        //{
        //    Simulation.Scene.Add(WidgetCash);
        //    Simulation.Scene.Add(WidgetRemainingWaves);
        //    Simulation.Scene.Add(WidgetScore);

        //    SandGlass.Show();
        //}


        //public void Hide()
        //{
        //    Simulation.Scene.Remove(WidgetCash);
        //    Simulation.Scene.Remove(WidgetRemainingWaves);
        //    Simulation.Scene.Remove(WidgetScore);

        //    SandGlass.Hide();
        //}


        public void Draw()
        {

            WidgetCash.Data = Cash + "M$";
            WidgetRemainingWaves.Data = (RemainingWaves == -1) ? "Inf." : RemainingWaves.ToString();
            WidgetScore.Data = Score.ToString();

            Simulation.Scene.Add(WidgetCash);
            Simulation.Scene.Add(WidgetRemainingWaves);
            Simulation.Scene.Add(WidgetScore);

            SandGlass.Draw();
            MenuNextWave.Draw();
        }
    }
}
