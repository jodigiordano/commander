namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class BackgroundsAssetsPanel : AssetsPanel
    {
        private string BackgroundsDirectory = @".\Content\backgrounds";


        public BackgroundsAssetsPanel(Simulator simulator)
            : base("Backgrounds", simulator, Vector3.Zero, new Vector2(500, 500), VisualPriorities.Default.EditorPanel, Color.White)
        {

        }
        

        protected override List<string> GetAssets()
        {
            List<string> results = new List<string>(Directory.GetFiles(BackgroundsDirectory + @"\cutscenes"));

            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\story1"));
            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\story2"));
            results.AddRange(Directory.GetFiles(BackgroundsDirectory + @"\worlds"));

            for (int i = 0; i < results.Count; i++)
                results[i] = System.IO.Path.GetFileNameWithoutExtension(results[i]);

            return results;
        }


        protected override PanelWidget GetWidget(string assetName)
        {
            return new ImageWidget(assetName, 0.09f);
        }


        protected override void DoClick(PanelWidget widget)
        {
            var img = (ImageWidget) widget;

            Simulator.Data.Level.Background = new Image(img.Image.TextureName) { VisualPriority = VisualPriorities.Default.Background };
        }
    }
}
