namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SimulatorAnimation
    {
        public Simulator Simulator;

        private Scene Scene;
        private double TimeBeforeIn;
        private Image Background;


        public SimulatorAnimation(Scene scene)
        {
            Scene = scene;

            Background = new Image("PixelBlanc")
            {
                Color = Color.White,
                Size = new Vector2(1280, 720),
                VisualPriority = VisualPriorities.Cutscenes.IntroSimulatorBackground
            };

            TimeBeforeIn = IntroCutscene.Timing["SimulatorIn"];

            Simulator = new Simulator(scene, Main.LevelsFactory.CutsceneDescriptors[1])
            {
                DemoMode = true,
                CutsceneMode = true
            };
            Simulator.Initialize();

            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.ChangeColor(Color.Black, 0, 2000));
            Scene.VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeOutTo0(255, TimeBeforeIn, 5000));
        }


        public void Update()
        {
            TimeBeforeIn -= Preferences.TargetElapsedTimeMs;

            if (TimeBeforeIn < 0)
                Simulator.Update();
        }


        public void Draw()
        {
            Scene.Add(Background);

            if (TimeBeforeIn < 0)
                Simulator.Draw();
        }
    }
}
