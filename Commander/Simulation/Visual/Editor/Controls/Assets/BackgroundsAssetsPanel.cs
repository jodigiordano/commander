namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Xna.Framework;


    class BackgroundsAssetsPanel : AssetsPanel
    {
        private string BackgroundsDirectory = @".\Content\backgrounds";


        public BackgroundsAssetsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base("Backgrounds", simulator, position, size, visualPriority, color)
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
    }
}
