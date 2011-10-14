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
        private NumericHorizontalSlider BulletDamage;


        public PlayerPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Player");
            
            Lives = new NumericHorizontalSlider("Starting lives", 0, 50, 0, 1, 100, 100);
            Cash = new NumericHorizontalSlider("Starting money", 0, 50000, 0, 100, 100, 100);
            Minerals = new NumericHorizontalSlider("Minerals", 0, 50000, 0, 100, 100, 100);
            LifePacks = new NumericHorizontalSlider("Life packs", 0, 50, 0, 1, 100, 100);
            BulletDamage = new NumericHorizontalSlider("Bullet damage", -1, 1000, 0, 5, 100, 100);

            AddWidget("Lives", Lives);
            AddWidget("Cash", Cash);
            AddWidget("Minerals", Minerals);
            AddWidget("LifePacks", LifePacks);
            AddWidget("BulletDamage", BulletDamage);

            Initialize();
        }


        public void Initialize()
        {
            base.Initialize();

            Lives.Value = Simulator.LevelDescriptor.Player.Lives;
            Cash.Value = Simulator.LevelDescriptor.Player.Money;
            Minerals.Value = Simulator.LevelDescriptor.Minerals.Cash;
            LifePacks.Value = Simulator.LevelDescriptor.Minerals.LifePacks;
            BulletDamage.Value = (int) (Simulator.LevelDescriptor.Player.BulletDamage * 10);
        }
    }
}
