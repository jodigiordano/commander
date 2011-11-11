namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Microsoft.Xna.Framework;


    class CelestialBodyAssetsPanel : AssetsPanel
    {
        public CelestialBody CelestialBody;

        private string AssetsDirectory = @".\Content\";


        public CelestialBodyAssetsPanel(Simulator simulator)
            : base("Assets", simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White)
        {
            AssetsPerPanel = 12;
        }


        protected override List<string> GetAssets()
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


        protected override PanelWidget GetWidget(string assetName)
        {
            return new ImageWidget(assetName, 4f);
        }
    }
}
