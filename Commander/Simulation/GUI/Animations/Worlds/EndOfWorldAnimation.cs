namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    abstract class EndOfWorldAnimation : Animation
    {
        private WorldScene WorldScene;
        protected Simulator Simulator;


        public EndOfWorldAnimation(WorldScene worldScene, double duration)
            : base(duration, VisualPriorities.Default.EndOfWorldAnimation)
        {
            WorldScene = worldScene;
            Simulator = worldScene.Simulator;

            Main.GameInProgress = null;
            WorldScene.CanSelectCelestialBodies = false;
            WorldScene.CanGoBackToMainMenu = false;
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 0, 500);
            Simulator.Scene.VisualEffects.Add(WorldScene.LevelStates, Core.Visual.VisualEffects.Fade(WorldScene.LevelStates.Alpha, 0, 0, 1000));
        }


        public override void Stop()
        {
            base.Stop();

            WorldScene.CanSelectCelestialBodies = true;
            WorldScene.CanGoBackToMainMenu = true;
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);
        }
    }
}
