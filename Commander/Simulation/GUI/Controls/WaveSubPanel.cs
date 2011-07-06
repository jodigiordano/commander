﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class WaveSubPanel : VerticalPanel
    {
        private NumericHorizontalSlider StartingTime;
        private EnemiesWidget Enemies;
        private NumericHorizontalSlider Level;
        private NumericHorizontalSlider CashValue;
        private NumericHorizontalSlider Quantity;
        private ChoicesHorizontalSlider WavesTypes;

        private Dictionary<string, Panel> WavesPanels;
        private string CurrentWaveType;


        public WaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            ShowFrame = false;
            DistanceBetweenTwoChoices = 15;
                
            StartingTime = new NumericHorizontalSlider("Starting time", 0, 1000, 0, 10, 50);
            Enemies = new EnemiesWidget(simulator.EnemiesFactory.All, (int) size.X, 5);
            Level = new NumericHorizontalSlider("Level", 1, 100, 1, 1, 50);
            CashValue = new NumericHorizontalSlider("Cash", 0, 100, 0, 5, 100);
            Quantity = new NumericHorizontalSlider("Quantity", 0, 500, 0, 5, 50);
            WavesTypes = new ChoicesHorizontalSlider("Wave type", WaveGenerator.WavesTypesStrings, 0);

            AddWidget("StartingTime", StartingTime);
            AddWidget("Enemies", Enemies);
            AddWidget("Level", Level);
            AddWidget("CashValue", CashValue);
            AddWidget("Quantity", Quantity);
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
            get { return Enemies.SelectedCount; }
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
                Quantity = Quantity.Value
            };

            if (CurrentWaveType == WaveType.Homogene.ToString("g"))
            {
                d.Distance = ((HomogeneWaveSubPanel) WavesPanels[CurrentWaveType]).GetDistance();
            }

            else if (CurrentWaveType == WaveType.DistinctFollow.ToString("g"))
            {
                var panel = ((DistinctFollowWaveSubPanel) WavesPanels[CurrentWaveType]);

                d.Distance = panel.GetDistance();
                d.Delay = panel.GetDelay();
            }

            else if (CurrentWaveType == WaveType.PackedH.ToString("g"))
            {
                var panel = ((PackedHomogeneWaveSubPanel) WavesPanels[CurrentWaveType]);

                d.Delay = panel.GetDelay();
                d.SwitchEvery = panel.GetSwitchEvery();
            }


            return d;
        }
    }
}