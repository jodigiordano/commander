namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class BackgroundsPanel : SlideshowPanel
    {
        private Simulator Simulator;


        public BackgroundsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Backgrounds");
            Slider.SetLabel("Set");

            Initialize();
        }


        public void Initialize()
        {
            ClearWidgets();

            var nbBackgrounds = 47;

            for (int i = 0; i < Math.Ceiling(nbBackgrounds / 16f); i++)
                AddWidget("background" + i, CreateSubPanel(i * 16 + 1, Math.Min(i * 16 + 16, nbBackgrounds)));
        }


        private GridPanel CreateSubPanel(int begin, int end)
        {
            GridPanel backgroundPanel = new GridPanel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White)
            {
                NbColumns = 4,
                OnlyShowWidgets = true
            };

            for (int i = begin; i <= end; i++)
                backgroundPanel.AddWidget("background" + i, new ImageWidget("background" + i, 0.1f));

            return backgroundPanel;
        }
    }
}
