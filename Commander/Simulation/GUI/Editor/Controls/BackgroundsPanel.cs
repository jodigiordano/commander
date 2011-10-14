namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Xna.Framework;


    class BackgroundsPanel : SlideshowPanel
    {
        private string BackgroundsDirectory = @".\Content\backgrounds";
        private Simulator Simulator;


        public BackgroundsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Backgrounds");
            Slider.SpaceForLabel = 100;
            Slider.SetLabel("Set");

            Initialize();

            Alpha = 0;
        }


        public override void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            var backgrounds = GetBackgrounds();

            for (int i = 0; i < Math.Ceiling(backgrounds.Count / 16f); i++)
                AddWidget("background" + i, CreateSubPanel(backgrounds, i * 16, Math.Min(i * 16 + 15, backgrounds.Count - 1)));
        }


        private GridPanel CreateSubPanel(List<string> backgrounds, int begin, int end)
        {
            GridPanel backgroundPanel = new GridPanel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), VisualPriority, Color.White)
            {
                NbColumns = 4,
                OnlyShowWidgets = true
            };

            for (int i = begin; i <= end; i++)
            {
                var name = backgrounds[i];
                var widget = new ImageWidget(name, 0.09f)
                {
                    Sticky = true
                };
                backgroundPanel.AddWidget(name, widget);
            }

            return backgroundPanel;
        }
        

        private List<string> GetBackgrounds()
        {
            List<string> results = new List<string>(Directory.GetFiles(BackgroundsDirectory + @"\cutscenes"));

            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\story1"));
            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\story2"));
            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\worlds"));

            for (int i = 0; i < results.Count; i++)
                results[i] = System.IO.Path.GetFileNameWithoutExtension(results[i]);

            return results;
        }
    }
}
