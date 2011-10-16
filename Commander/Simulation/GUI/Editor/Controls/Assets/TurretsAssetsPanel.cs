namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class TurretsAssetsPanel : AssetsPanel
    {

        public TurretsAssetsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base("Availables Turrets", simulator, position, size, visualPriority, color)
        {

        }


        public override void Sync()
        {
            foreach (var turret in Simulator.TurretsFactory.All)
            {
                var widget = (TurretCheckBox) GetWidgetByName(turret.Key.ToString());

                widget.Value = Simulator.TurretsFactory.Availables.ContainsKey(turret.Key);
            }
        }


        protected override List<string> GetAssets()
        {
            List<string> results = new List<string>();

            foreach (var turret in Simulator.TurretsFactory.All)
                results.Add(turret.Key.ToString());

            return results;
        }


        protected override PanelWidget GetWidget(string assetName)
        {
            return new TurretCheckBox(Simulator.TurretsFactory.Create((TurretType) Enum.Parse(typeof(TurretType), assetName)));
        }
    }
}
