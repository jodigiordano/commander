namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PowerUpsAssetsPanel : AssetsPanel
    {

        public PowerUpsAssetsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base("Availables Power-Ups", simulator, position, size, visualPriority, color)
        {

        }


        public override void Sync()
        {
            foreach (var p in Simulator.PowerUpsFactory.All)
            {
                var widget = (PowerUpCheckBox) GetWidgetByName(p.Key.ToString());

                widget.Value = Simulator.Data.Level.AvailablePowerUps.ContainsKey(p.Key);
            }
        }


        protected override List<string> GetAssets()
        {
            List<string> results = new List<string>();

            foreach (var p in Simulator.PowerUpsFactory.All)
                results.Add(p.Key.ToString());

            return results;
        }


        protected override PanelWidget GetWidget(string assetName)
        {
            var pType = (PowerUpType) Enum.Parse(typeof(PowerUpType), assetName);
            var p = Simulator.PowerUpsFactory.All[pType];

            return new PowerUpCheckBox(p.Category == PowerUpCategory.Turret ?
                Simulator.TurretsFactory.All[p.AssociatedTurret].BaseImage.TextureName :
                p.BuyImage, pType, 3);
        }
    }
}
