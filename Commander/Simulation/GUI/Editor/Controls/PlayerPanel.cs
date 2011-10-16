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

            Lives = new NumericHorizontalSlider("Starting lives", 0, 50, 0, 1, 350, 200);
            Cash = new NumericHorizontalSlider("Starting money", 0, 50000, 0, 100, 350, 200);
            Minerals = new NumericHorizontalSlider("Minerals", 0, 50000, 0, 100, 350, 200);
            LifePacks = new NumericHorizontalSlider("Life packs", 0, 50, 0, 1, 350, 200);

            BulletDamage = new NumericHorizontalSlider("Bullet damage", -1, 100, 0, 1, 350, 200);
            BulletDamage.AddAlias(-1, "automatic");

            AddWidget("Lives", Lives);
            AddWidget("Cash", Cash);
            AddWidget("Minerals", Minerals);
            AddWidget("LifePacks", LifePacks);
            AddWidget("BulletDamage", BulletDamage);

            Alpha = 0;

            Initialize();
        }


        public override void Initialize()
        {
            base.Initialize();

            Lives.Value = Simulator.LevelDescriptor.Player.Lives;
            Cash.Value = Simulator.LevelDescriptor.Player.Money;
            Minerals.Value = Simulator.LevelDescriptor.Minerals.Cash;
            LifePacks.Value = Simulator.LevelDescriptor.Minerals.LifePacks;
            BulletDamage.Value = (int) (Simulator.LevelDescriptor.Player.BulletDamage);
        }
    }
}
