namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class WaveSubPanel : VerticalPanel
    {
        private NumericHorizontalSlider StartingTime;
        private EnemiesWidget Enemies;
        private NumericHorizontalSlider Level;
        private NumericHorizontalSlider CashValue;
        private NumericHorizontalSlider QuantityWidget;

        private ChoicesHorizontalSlider Distances;
        private NumericHorizontalSlider DelayWidget;
        private NumericHorizontalSlider ApplyDelayWidget;
        private NumericHorizontalSlider SwitchEveryWidget;


        public WaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            OnlyShowWidgets = true;
            DistanceBetweenTwoChoices = 15;
                
            StartingTime = new NumericHorizontalSlider("Starting time", 0, 500, 0, 10, 50);
            Enemies = new EnemiesWidget(simulator.EnemiesFactory.All, (int) size.X, 5);
            Level = new NumericHorizontalSlider("Level", 1, 100, 1, 1, 50);
            CashValue = new NumericHorizontalSlider("Cash", 0, 100, 0, 5, 100);
            QuantityWidget = new NumericHorizontalSlider("Quantity", 0, 500, 0, 5, 50);

            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 0);
            DelayWidget = new NumericHorizontalSlider("Delay", 0, 20, 0, 1, 100);
            ApplyDelayWidget = new NumericHorizontalSlider("Apply Delay", -1, 20, 0, 1, 100);
            SwitchEveryWidget = new NumericHorizontalSlider("Switch every", -1, 50, 5, 5, 100);
            
            AddWidget("StartingTime", StartingTime);
            AddWidget("Enemies", Enemies);
            AddWidget("Level", Level);
            AddWidget("CashValue", CashValue);
            AddWidget("Quantity", QuantityWidget);

            AddWidget("Distances", Distances);
            AddWidget("Delay", DelayWidget);
            AddWidget("ApplyDelay", ApplyDelayWidget);
            AddWidget("SwitchEvery", SwitchEveryWidget);
        }


        public int EnemiesCount
        {
            get { return Enemies.ClickedCount; }
        }


        public int Quantity
        {
            get { return QuantityWidget.Value; }
        }


        public void Sync(WaveDescriptor descriptor)
        {
            StartingTime.Value = (int) descriptor.StartingTime / 1000;
            Enemies.Sync(descriptor.Enemies);
            Level.Value = descriptor.LivesLevel;
            CashValue.Value = descriptor.CashValue;
            QuantityWidget.Value = descriptor.Quantity;

            Distance = descriptor.Distance;
            Delay = (int) (descriptor.Delay / 1000);
            ApplyDelayEvery = descriptor.ApplyDelayEvery / 1000;
            SwitchEvery = descriptor.SwitchEvery;
        }


        public WaveDescriptor GenerateDescriptor()
        {
            return new WaveDescriptor()
            {
                StartingTime = StartingTime.Value * 1000,
                Enemies = Enemies.GetEnemies(),
                LivesLevel = Level.Value,
                SpeedLevel = Level.Value,
                CashValue = CashValue.Value,
                Quantity = QuantityWidget.Value,
                Distance = Distance,
                Delay = Delay * 1000,
                ApplyDelayEvery = ApplyDelayEvery * 1000,
                SwitchEvery = SwitchEvery
            };
        }


        private Distance Distance
        {
            get { return (Distance) Enum.Parse(typeof(Distance), Distances.Value, false); }
            set { Distances.Value = value.ToString("g"); }
        }


        private int Delay
        {
            get { return DelayWidget.Value; }
            set { DelayWidget.Value = value; }
        }


        private int ApplyDelayEvery
        {
            get { return ApplyDelayWidget.Value; }
            set { ApplyDelayWidget.Value = value; }
        }


        private int SwitchEvery
        {
            get { return SwitchEveryWidget.Value; }
            set { SwitchEveryWidget.Value = value; }
        }
    }
}
