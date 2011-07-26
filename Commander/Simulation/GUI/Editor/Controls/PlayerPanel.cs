namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PlayerPanel : VerticalPanel
    {
        private Simulator Simulator;

        private NumericHorizontalSlider Lives;
        private NumericHorizontalSlider Cash;
        private NumericHorizontalSlider Minerals;
        private NumericHorizontalSlider LifePacks;


        public PlayerPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Player");
            
            Lives = new NumericHorizontalSlider("Starting lives", 0, 50, 0, 1, 100);
            Cash = new NumericHorizontalSlider("Starting money", 0, 50000, 0, 100, 100);
            Minerals = new NumericHorizontalSlider("Minerals", 0, 50000, 0, 100, 100);
            LifePacks = new NumericHorizontalSlider("Life packs", 0, 50, 0, 1, 100);

            AddWidget("Lives", Lives);
            AddWidget("Cash", Cash);
            AddWidget("Minerals", Minerals);
            AddWidget("LifePacks", LifePacks);

            Initialize();
        }


        public void Initialize()
        {
            Lives.Value = Simulator.LevelDescriptor.Player.Lives;
            Cash.Value = Simulator.LevelDescriptor.Player.Money;
            Minerals.Value = Simulator.LevelDescriptor.Minerals.Cash;
            LifePacks.Value = Simulator.LevelDescriptor.Minerals.LifePacks;
        }
    }
}
