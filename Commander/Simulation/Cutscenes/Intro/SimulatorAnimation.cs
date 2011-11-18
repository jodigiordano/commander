namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SimulatorAnimation
    {
        public Simulator Simulator;

        private CommanderScene Scene;
        private double TimeBeforeIn;
        private Image Background;


        public SimulatorAnimation(CommanderScene scene)
        {
            Scene = scene;

            Background = new Image("PixelBlanc")
            {
                Color = Color.White,
                Size = Preferences.BackBuffer,
                VisualPriority = VisualPriorities.Cutscenes.IntroSimulatorBackground
            };

            TimeBeforeIn = IntroCutscene.Timing["SimulatorIn"];

            Simulator = new Simulator(scene, Main.WorldsFactory.CutsceneDescriptors[1])
            {
                CutsceneMode = true,
                AsteroidBeltOverride = true
            };
            Simulator.Initialize();

            Scene.VisualEffects.Add(Background, VisualEffects.Fade(Background.Alpha, 0, 0, 4000));
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
