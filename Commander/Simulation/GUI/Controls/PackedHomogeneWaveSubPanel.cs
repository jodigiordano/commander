namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PackedHomogeneWaveSubPanel : VerticalPanel
    {
        private NumericHorizontalSlider DelayWidget;
        private NumericHorizontalSlider SwitchEveryWidget;


        public PackedHomogeneWaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            ShowFrame = false;

            DelayWidget = new NumericHorizontalSlider("Delay", 2000, 10000, 2000, 100, 100);
            SwitchEveryWidget = new NumericHorizontalSlider("Switch every", 5, 20, 5, 1, 100);

            AddWidget("Delay", DelayWidget);
            AddWidget("SwitchEvery", SwitchEveryWidget);
        }


        public int Delay
        {
            get { return DelayWidget.Value; }
            set { DelayWidget.Value = value; }
        }


        public int SwitchEvery
        {
            get { return SwitchEveryWidget.Value; }
            set { SwitchEveryWidget.Value = value; }
        }
    }
}
