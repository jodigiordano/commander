namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GameMenu
    {
        public int Score;
        public int Cash;
        public int RemainingWaves;
        public double TimeNextWave;

        public NextWaveMenu MenuNextWave;
        public SandGlass SandGlass;

        public PhysicalRectangle Rectangle;

        private Simulator Simulator;
        private Text WidgetScore;
        private Text WidgetCash;
        private Text WidgetRemainingWaves;
        private Vector3 Position;

        private bool Faded;


        public GameMenu(Simulator simulator, Vector3 position)
        {
            Simulator = simulator;
            Position = position;

            SandGlass = new SandGlass(simulator.Scene, 50000, this.Position, Preferences.PrioriteGUIPanneauGeneral + 0.05f)
            {
                Alpha = 0
            };
            
            Cash = 0;


            WidgetCash = new Text
            (
                Cash + "M$",
                "Pixelite",
                new Color(255, 255, 255, 0),
                Position + new Vector3(30, 0, 0)
            );
            WidgetCash.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetCash.SizeX = 3;

            WidgetRemainingWaves = new Text
            (
                RemainingWaves.ToString(),
                "Pixelite",
                new Color(255, 255, 255, 0),
                Position + new Vector3(30, -40, 0)
            );
            WidgetRemainingWaves.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetRemainingWaves.SizeX = 3;

            WidgetScore = new Text
            (
                Score.ToString(),
                "Pixelite",
                new Color(255, 255, 255, 0),
                Position + new Vector3(30, 40, 0)
            );
            WidgetScore.VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05f;
            WidgetScore.SizeX = 3;

            Faded = true;
            FadeIn(255, 3000);

            Rectangle = new PhysicalRectangle(SandGlass.Rectangle.Left, SandGlass.Rectangle.Top, SandGlass.Rectangle.Width + 200, SandGlass.Rectangle.Height + 30);
        }


        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave
        {
            set
            {
                MenuNextWave = new NextWaveMenu(Simulator, value, this.Position - new Vector3(150, 30, 0), Preferences.PrioriteGUIPanneauGeneral + 0.049f);
            }
        }


        public void Update()
        {
            this.SandGlass.RemainingTime = this.TimeNextWave;
            this.SandGlass.Update();
        }


        public void FadeIn(int to, double length)
        {
            if (!Faded)
                return;

            Simulator.Scene.VisualEffects.Add(WidgetScore, Core.Visual.VisualEffects.Fade(WidgetScore.Alpha, to, 0, length));
            Simulator.Scene.VisualEffects.Add(WidgetCash, Core.Visual.VisualEffects.Fade(WidgetCash.Alpha, to, 0, length));
            Simulator.Scene.VisualEffects.Add(WidgetRemainingWaves, Core.Visual.VisualEffects.Fade(WidgetRemainingWaves.Alpha, to, 0, length));
            SandGlass.Fade(to, 0, length);
            Faded = false;
        }


        public void FadeOut(int to, double length)
        {
            if (Faded)
                return;

            Simulator.Scene.VisualEffects.Add(WidgetScore, Core.Visual.VisualEffects.Fade(WidgetScore.Alpha, to, 0, length));
            Simulator.Scene.VisualEffects.Add(WidgetCash, Core.Visual.VisualEffects.Fade(WidgetCash.Alpha, to, 0, length));
            Simulator.Scene.VisualEffects.Add(WidgetRemainingWaves, Core.Visual.VisualEffects.Fade(WidgetRemainingWaves.Alpha, to, 0, length));
            SandGlass.Fade(to, 0, length);
            Faded = true;
        }


        public void Draw()
        {

            WidgetCash.Data = Cash + "M$";
            WidgetRemainingWaves.Data = (RemainingWaves == -1) ? "Inf." : RemainingWaves.ToString();
            WidgetScore.Data = Score.ToString();

            Simulator.Scene.Add(WidgetCash);
            Simulator.Scene.Add(WidgetRemainingWaves);
            Simulator.Scene.Add(WidgetScore);

            SandGlass.Draw();
            MenuNextWave.Draw();
        }
    }
}
