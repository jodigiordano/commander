namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PowerUpsAssetsPanel : AssetsPanel
    {

        public PowerUpsAssetsPanel(Simulator simulator)
            : base("Availables Power-Ups", simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White)
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


        protected override void DoClick(PanelWidget widget, Commander.Player player)
        {
            var checkbox = (PowerUpCheckBox) widget;

            if (checkbox.Value)
                Simulator.Data.Level.AvailablePowerUps.Add(checkbox.PowerUp, Simulator.PowerUpsFactory.Create(checkbox.PowerUp));
            else
                Simulator.Data.Level.AvailablePowerUps.Remove(checkbox.PowerUp);

            //Simulator.EditorController.ExecuteCommand(
            //    new EditorSimpleCommand("AddOrRemovePowerUp"));
        }
    }
}
