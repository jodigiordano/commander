namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class WaveSubPanel : VerticalPanel
    {
        private NumericHorizontalSlider StartingTime;
        private EnemiesWidget Enemies;
        private NumericHorizontalSlider Level;
        private NumericHorizontalSlider CashValue;
        private NumericHorizontalSlider QuantityWidget;
        private ChoicesHorizontalSlider WavesTypes;

        private Dictionary<string, Panel> WavesPanels;
        private string CurrentWaveType;


        public WaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            OnlyShowWidgets = true;
            DistanceBetweenTwoChoices = 15;
                
            StartingTime = new NumericHorizontalSlider("Starting time", 0, 1000, 0, 10, 50);
            Enemies = new EnemiesWidget(simulator.EnemiesFactory.All, (int) size.X, 5);
            Level = new NumericHorizontalSlider("Level", 1, 100, 1, 1, 50);
            CashValue = new NumericHorizontalSlider("Cash", 0, 100, 0, 5, 100);
            QuantityWidget = new NumericHorizontalSlider("Quantity", 0, 500, 0, 5, 50);
            WavesTypes = new ChoicesHorizontalSlider("Wave type", WaveGenerator.WavesTypesStrings, 0);

            AddWidget("StartingTime", StartingTime);
            AddWidget("Enemies", Enemies);
            AddWidget("Level", Level);
            AddWidget("CashValue", CashValue);
            AddWidget("Quantity", QuantityWidget);
            AddWidget("WavesTypes", WavesTypes);

            WavesPanels = new Dictionary<string, Panel>();
            WavesPanels.Add(WaveType.Homogene.ToString("g"), new HomogeneWaveSubPanel(simulator, size, visualPriority, color));
            WavesPanels.Add(WaveType.DistinctFollow.ToString("g"), new DistinctFollowWaveSubPanel(simulator, size, visualPriority, color));
            WavesPanels.Add(WaveType.PackedH.ToString("g"), new PackedHomogeneWaveSubPanel(simulator, size, visualPriority, color));

            CurrentWaveType = WavesTypes.Value;
            AddWidget("SubWaveType", WavesPanels[WavesTypes.Value]);
        }


        protected override bool Click(Core.Physics.Circle circle)
        {
            if (!base.Click(circle))
                return false;

            if (CurrentWaveType != WavesTypes.Value)
            {
                RemoveWidget("SubWaveType");
                AddWidget("SubWaveType", WavesPanels[WavesTypes.Value]);

                CurrentWaveType = WavesTypes.Value;
            }

            return true;
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

            if (descriptor.SwitchEvery != -1) //must be checked first
            {
                WavesTypes.Value = WaveType.PackedH.ToString("g");

                var panel = ((PackedHomogeneWaveSubPanel) WavesPanels[WavesTypes.Value]);

                panel.Delay = (int) descriptor.Delay;
                panel.SwitchEvery = descriptor.SwitchEvery;
            }

            else if (descriptor.Delay != -1)
            {
                WavesTypes.Value = WaveType.DistinctFollow.ToString("g");

                var panel = ((DistinctFollowWaveSubPanel) WavesPanels[WavesTypes.Value]);

                panel.Distance = descriptor.Distance;
                panel.Delay = (int) descriptor.Delay;
            }

            else
            {
                WavesTypes.Value = WaveType.Homogene.ToString("g");

                ((HomogeneWaveSubPanel) WavesPanels[WavesTypes.Value]).Distance = descriptor.Distance;
            }

            // Sync Generator Sub Panel
            RemoveWidget("SubWaveType");
            AddWidget("SubWaveType", WavesPanels[WavesTypes.Value]);

            CurrentWaveType = WavesTypes.Value;
        }


        public WaveDescriptor GenerateDescriptor()
        {
            WaveDescriptor d = new WaveDescriptor()
            {
                StartingTime = StartingTime.Value * 1000,
                Enemies = Enemies.GetEnemies(),
                LivesLevel = Level.Value,
                SpeedLevel = Level.Value,
                CashValue = CashValue.Value,
                Quantity = QuantityWidget.Value
            };

            if (CurrentWaveType == WaveType.Homogene.ToString("g"))
            {
                d.Distance = ((HomogeneWaveSubPanel) WavesPanels[CurrentWaveType]).Distance;
            }

            else if (CurrentWaveType == WaveType.DistinctFollow.ToString("g"))
            {
                var panel = ((DistinctFollowWaveSubPanel) WavesPanels[CurrentWaveType]);

                d.ApplyDelayEvery = d.Quantity / d.Enemies.Count;
                d.SwitchEvery = d.ApplyDelayEvery;
                d.Distance = panel.Distance;
                d.Delay = panel.Delay;
            }

            else if (CurrentWaveType == WaveType.PackedH.ToString("g"))
            {
                var panel = ((PackedHomogeneWaveSubPanel) WavesPanels[CurrentWaveType]);

                d.Delay = panel.Delay;
                d.SwitchEvery = panel.SwitchEvery;
                d.ApplyDelayEvery = d.SwitchEvery;
                d.Distance = Distance.Near;
            }


            return d;
        }
    }
}
