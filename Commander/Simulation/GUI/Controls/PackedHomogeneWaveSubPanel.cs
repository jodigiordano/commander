namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PackedHomogeneWaveSubPanel : VerticalPanel
    {
        private NumericHorizontalSlider Delay;
        private NumericHorizontalSlider SwitchEvery;


        public PackedHomogeneWaveSubPanel(Simulator simulator, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, Vector3.Zero, size, visualPriority, color)
        {
            ShowFrame = false;

            Delay = new NumericHorizontalSlider("Delay", 2000, 10000, 2000, 100, 100);
            SwitchEvery = new NumericHorizontalSlider("Switch every", 5, 20, 5, 1, 100);

            AddWidget("Delay", Delay);
            AddWidget("SwitchEvery", SwitchEvery);
        }


        public int GetDelay()
        {
            return Delay.Value;
        }


        public int GetSwitchEvery()
        {
            return SwitchEvery.Value;
        }
    }
}
