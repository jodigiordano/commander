namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class TurretsAssetsPanel : AssetsPanel
    {

        public TurretsAssetsPanel(Simulator simulator)
            : base("Availables Turrets", simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White)
        {

        }


        public override void Sync()
        {
            foreach (var turret in Simulator.TurretsFactory.All)
            {
                var widget = (TurretCheckBox) GetWidgetByName(turret.Key.ToString());

                widget.Value = Simulator.Data.Level.AvailableTurrets.ContainsKey(turret.Key);
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


        protected override void DoClick(PanelWidget widget, Commander.Player player)
        {
            var checkbox = (TurretCheckBox) widget;

            if (checkbox.Value)
                Simulator.Data.Level.AvailableTurrets.Add(checkbox.Turret.Type, checkbox.Turret);
            else
                Simulator.Data.Level.AvailableTurrets.Remove(checkbox.Turret.Type);
        }
    }
}
