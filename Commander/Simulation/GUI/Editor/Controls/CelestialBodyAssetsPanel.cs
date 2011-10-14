namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Microsoft.Xna.Framework;


    class CelestialBodyAssetsPanel : SlideshowPanel
    {
        public CelestialBody CelestialBody;

        private string AssetsDirectory = @".\Content\";
        private Simulator Simulator;


        public CelestialBodyAssetsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Assets");
            Slider.SpaceForLabel = 100;
            Slider.SetLabel("Set");

            Initialize();

            Alpha = 0;
        }


        public override void Initialize()
        {
            base.Initialize();

            ClearWidgets();

            var assets = GetAssets();

            for (int i = 0; i < Math.Ceiling(assets.Count / 12f); i++)
                AddWidget("asset" + i, CreateSubPanel(assets, i * 12, Math.Min(i * 12 + 11, assets.Count - 1)));
        }


        private GridPanel CreateSubPanel(List<string> assets, int begin, int end)
        {
            GridPanel assetsPanel = new GridPanel(Simulator.Scene, new Vector3(30, 0, 0), new Vector2(700, 500), VisualPriority, Color.White)
            {
                NbColumns = 4,
                OnlyShowWidgets = true
            };

            for (int i = begin; i <= end; i++)
            {
                var name = assets[i];
                var widget = new ImageWidget(name, 4f)
                {
                    Sticky = true
                };
                assetsPanel.AddWidget(name, widget);
            }

            return assetsPanel;
        }
        

        private List<string> GetAssets()
        {
            List<string> results = new List<string>();

            Regex regex = new Regex(@"(planete[0-9]+[3]<)|(vaisseauAlien[0-9]+[3]<)|(stationSpatiale[0-9]+[3]<)");

            using (StreamReader r = new StreamReader(AssetsDirectory + @"\spritesheet.xml"))
            {
                string line;

                while ((line = r.ReadLine()) != null)
                {
                    Match m = regex.Match(line);

                    if (m.Success)
                        results.Add(m.Groups[0].Value.Remove(m.Groups[0].Value.Length - 1));
                }
            }

            return results;
        }
    }
}
