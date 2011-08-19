namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GameMenu
    {
        public int Cash;
        public int Lives;

        private Simulator Simulator;
        private Image CashImage;
        private Text CashText;
        private Image LivesImage;
        private Text LivesText;
        private Vector3 Position;

        private bool Faded;


        public GameMenu(Simulator simulator, Vector3 position)
        {
            Simulator = simulator;
            Position = position;

            Cash = 0;

            CashImage = new Image("ScoreMoney")
            {
                SizeX = 4,
                Alpha = 0,
                VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05,
                Position = Position,
                Origin = Vector2.Zero
            };

            CashText = new Text("Pixelite")
            {
                Alpha = 0,
                VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05,
                SizeX = 3,
                Position = CashImage.Position + new Vector3(CashImage.AbsoluteSize.X + 5, 0, 0)
            };


            LivesImage = new Image("ScoreLives")
            {
                SizeX = 4,
                Alpha = 0,
                VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05,
                Position = Position + new Vector3(0, CashImage.AbsoluteSize.Y, 0),
                Origin = Vector2.Zero
            };


            LivesText = new Text("Pixelite")
            {
                Alpha = 0,
                VisualPriority = Preferences.PrioriteGUIPanneauGeneral + 0.05,
                SizeX = 3,
                Position = LivesImage.Position + new Vector3(LivesImage.AbsoluteSize.X + 5, 0, 0)
            };


            Faded = true;
            FadeIn(255, 3000);
        }


        public void Update()
        {

        }


        public void FadeIn(int to, double length)
        {
            if (!Faded)
                return;

            var effect = Core.Visual.VisualEffects.Fade(CashImage.Alpha, to, 0, length);

            Simulator.Scene.VisualEffects.Add(CashImage, effect);
            Simulator.Scene.VisualEffects.Add(CashText, effect);
            Simulator.Scene.VisualEffects.Add(LivesImage, effect);
            Simulator.Scene.VisualEffects.Add(LivesText, effect);
            Faded = false;
        }


        public void FadeOut(int to, double length)
        {
            if (Faded)
                return;

            var effect = Core.Visual.VisualEffects.Fade(CashText.Alpha, to, 0, length);

            Simulator.Scene.VisualEffects.Add(CashImage, effect);
            Simulator.Scene.VisualEffects.Add(CashText, effect);
            Simulator.Scene.VisualEffects.Add(LivesImage, effect);
            Simulator.Scene.VisualEffects.Add(LivesText, effect);
            Faded = true;
        }


        public void Draw()
        {
            CashText.Data = Cash.ToString();
            LivesText.Data = Lives.ToString();

            Simulator.Scene.Add(CashImage);
            Simulator.Scene.Add(CashText);
            Simulator.Scene.Add(LivesImage);
            Simulator.Scene.Add(LivesText);
        }
    }
}
