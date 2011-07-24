﻿namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class HomogeneWaveSubPanel : VerticalPanel
    {
        private ChoicesHorizontalSlider Distances;


        public HomogeneWaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            OnlyShowWidgets = true;

            Distances = new ChoicesHorizontalSlider("Distance", WaveGenerator.DistancesStrings, 0);

            AddWidget("Distances", Distances);
        }


        public Distance Distance
        {
            get { return (Distance) Enum.Parse(typeof(Distance), Distances.Value, false); }
            set { Distances.Value = value.ToString("g"); }
        }
    }
}