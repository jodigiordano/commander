namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class WavesPanel : SlideshowPanel
    {
        private Simulator Simulator;


        public WavesPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Waves");

            Initialize();
        }


        public void Initialize()
        {
            ClearWidgets();

            for (int i = 0; i < 20; i++)
                AddWidget("wave" + i, new WaveSubPanel(Simulator, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White));

            for (int i = 0; i < Simulator.LevelDescriptor.Waves.Count; i++)
                ((WaveSubPanel) Widgets["wave" + i]).Sync(Simulator.LevelDescriptor.Waves[i]);
        }
    }
}
