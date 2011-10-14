namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class WaveSubPanel : HorizontalPanel
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

        private VerticalPanel SideA;
        private VerticalPanel SideB;


        public WaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            OnlyShowWidgets = true;

            SideA = new VerticalPanel(simulator.Scene, new Vector3(), new Vector2(size.X / 2, size.Y), visualPriority, color)
            {
                OnlyShowWidgets = true,
                DistanceBetweenTwoChoices = 15
            };

            SideB = new VerticalPanel(simulator.Scene, new Vector3(), new Vector2(size.X / 2, size.Y), visualPriority, color)
            {
                OnlyShowWidgets = true,
                DistanceBetweenTwoChoices = 15
            };

            StartingTime = new NumericHorizontalSlider("Starting time", 0, 500, 0, 10, 50, 50);
            Enemies = new EnemiesWidget(simulator.EnemiesFactory.All, (int) size.X / 2, 3);
            Level = new NumericHorizontalSlider("Level", 1, 100, 1, 1, 50, 50);
            CashValue = new NumericHorizontalSlider("Cash", 0, 100, 0, 5, 100, 100);
            QuantityWidget = new NumericHorizontalSlider("Quantity", 0, 500, 0, 5, 50, 50);

            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 0);
            DelayWidget = new NumericHorizontalSlider("Delay", 0, 20, 0, 1, 100, 100);
            ApplyDelayWidget = new NumericHorizontalSlider("Apply Delay", -1, 20, 0, 1, 100, 100);
            SwitchEveryWidget = new NumericHorizontalSlider("Switch every", -1, 50, 5, 5, 100, 100);

            SideA.AddWidget("StartingTime", StartingTime);
            SideA.AddWidget("Enemies", Enemies);
            SideA.AddWidget("Level", Level);
            SideA.AddWidget("CashValue", CashValue);
            
            SideB.AddWidget("Quantity", QuantityWidget);
            SideB.AddWidget("Distances", Distances);
            SideB.AddWidget("Delay", DelayWidget);
            SideB.AddWidget("ApplyDelay", ApplyDelayWidget);
            SideB.AddWidget("SwitchEvery", SwitchEveryWidget);

            AddWidget("SideA", SideA);
            AddWidget("SideB", SideB);
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
            ApplyDelayEvery = descriptor.ApplyDelayEvery;
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
                ApplyDelayEvery = ApplyDelayEvery,
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
