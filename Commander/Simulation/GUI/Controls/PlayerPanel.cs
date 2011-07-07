namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PlayerPanel : VerticalPanel
    {
        private Simulator Simulator;

        private NumericHorizontalSlider Lives;
        private NumericHorizontalSlider Cash;


        public PlayerPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Player");
            
            Lives = new NumericHorizontalSlider("Starting lives", 0, 50, 0, 1, 100);
            Cash = new NumericHorizontalSlider("Starting money", 0, 50000, 0, 100, 100);

            AddWidget("Lives", Lives);
            AddWidget("Cash", Cash);

            Initialize();
        }


        public void Initialize()
        {
            Lives.Value = Simulator.LevelDescriptor.Player.Lives;
            Cash.Value = Simulator.LevelDescriptor.Player.Money;
        }
    }
}
