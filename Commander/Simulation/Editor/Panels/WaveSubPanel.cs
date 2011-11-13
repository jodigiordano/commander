namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WaveSubPanel : HorizontalPanel
    {
        public int Id;
        public List<EnemyType> Enemies;

        private Label WaveEmitterLabel;
        private NumericHorizontalSlider StartingTime;
        private ChoicesHorizontalSlider Distances;
        private NumericHorizontalSlider DelayWidget;
        private NumericHorizontalSlider ApplyDelayWidget;
        private NumericHorizontalSlider SwitchEveryWidget;

        private Label EnemiesLabel;
        private NumericHorizontalSlider QuantityWidget;
        private NumericHorizontalSlider Level;
        private NumericHorizontalSlider CashValue;
        private PushButton EnemiesButton;

        private VerticalPanel SideA;
        private VerticalPanel SideB;

        private Simulator Simulator;

        public WaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color, int id)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            Simulator = simulator;
            OnlyShowWidgets = true;
            Id = id;

            Enemies = new List<EnemyType>();

            var spaceAvailable = size - new Vector2(50, 200);

            SideA = new VerticalPanel(simulator.Scene, new Vector3(), new Vector2(spaceAvailable.X / 2, spaceAvailable.Y), visualPriority, color)
            {
                ShowCloseButton = false,
                ShowFrame = false,
                ShowBackground = true,
                DistanceBetweenTwoChoices = 15
            };

            SideB = new VerticalPanel(simulator.Scene, new Vector3(), new Vector2(spaceAvailable.X / 2, spaceAvailable.Y), visualPriority, color)
            {
                ShowCloseButton = false,
                ShowFrame = false,
                ShowBackground = true,
                DistanceBetweenTwoChoices = 15
            };

            WaveEmitterLabel = new Label(new Text("Wave emitter", "Pixelite") { SizeX = 2 });
            StartingTime = new NumericHorizontalSlider("Starting time", 0, 500, 0, 10, 250, 150);
            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 250, 150, 0);
            DelayWidget = new NumericHorizontalSlider("Delay", 0, 20, 0, 1, 250, 150);
            ApplyDelayWidget = new NumericHorizontalSlider("Apply Delay", -1, 20, 0, 1, 250, 150);
            ApplyDelayWidget.AddAlias(-1, "None");
            SwitchEveryWidget = new NumericHorizontalSlider("Switch every", -1, 50, 5, 5, 250, 150);
            SwitchEveryWidget.AddAlias(-1, "None");

            EnemiesLabel = new Label(new Text("Enemies", "Pixelite") { SizeX = 2 });
            QuantityWidget = new NumericHorizontalSlider("Quantity", 0, 500, 0, 5, 250, 150);
            Level = new NumericHorizontalSlider("Level", 1, 100, 1, 1, 250, 150);
            CashValue = new NumericHorizontalSlider("Cash", 0, 100, 0, 5, 250, 150);
            EnemiesButton = new PushButton(new Text("Enemies", "Pixelite") { SizeX = 2 }, 250) { ClickHandler = DoEnemiesPanel };

            SideA.AddWidget("WaveEmitterLabel", WaveEmitterLabel);
            SideA.AddWidget("StartingTime", StartingTime);
            SideA.AddWidget("Distances", Distances);
            SideA.AddWidget("Delay", DelayWidget);
            SideA.AddWidget("ApplyDelay", ApplyDelayWidget);
            SideA.AddWidget("SwitchEvery", SwitchEveryWidget);

            SideB.AddWidget("EnemiesLabel", EnemiesLabel);
            SideB.AddWidget("Quantity", QuantityWidget);
            SideB.AddWidget("Enemies", EnemiesButton);
            SideB.AddWidget("Level", Level);
            SideB.AddWidget("CashValue", CashValue);

            AddWidget("SideA", SideA);
            AddWidget("SideB", SideB);
        }


        public int EnemiesCount
        {
            get { return Enemies.Count; }
        }


        public int Quantity
        {
            get { return QuantityWidget.Value; }
        }


        public void Sync(WaveDescriptor descriptor)
        {
            StartingTime.Value = (int) descriptor.StartingTime / 1000;
            Enemies = descriptor.Enemies;
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
                Enemies = Enemies,
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


        private void DoEnemiesPanel(PanelWidget widget, Commander.Player player)
        {
            var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];
            enemiesAssets.PanelToOpenOnClose = Name;

            if (Id < Simulator.Data.Level.Descriptor.Waves.Count)
                enemiesAssets.Enemies = Simulator.Data.Level.Descriptor.Waves[Id].Enemies;
            else
                enemiesAssets.Enemies = new List<EnemyType>();

            Simulator.EditorController.ExecuteCommand(
                new EditorPanelShowCommand(Simulator.Data.Players[player], "EditorEnemies")
                {
                    UsePosition = true,
                    Position = Position + new Vector3(Size, 0) 
                });
        }
    }
}
