namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EnemiesAssetsPanel : AssetsPanel
    {
        public List<EnemyType> Enemies;


        public EnemiesAssetsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base("Availables enemies", simulator, position, size, visualPriority, color)
        {

        }


        public override void Sync()
        {
            foreach (var e in Simulator.EnemiesFactory.All)
            {
                var widget = (EnemyCheckBox) GetWidgetByName(e.Type.ToString());

                widget.Value = Enemies.Contains(e.Type);
            }
        }


        protected override bool Click(Core.Physics.Circle circle)
        {
            var clicked = base.Click(circle);

            if (clicked)
            {
                Enemies.Clear();

                foreach (var e in Simulator.EnemiesFactory.All)
                {
                    var widget = (EnemyCheckBox) GetWidgetByName(e.Type.ToString());

                    if (widget.Value)
                        Enemies.Add(widget.Enemy);
                }
            }

            return clicked;
        }


        protected override List<string> GetAssets()
        {
            List<string> results = new List<string>();

            foreach (var e in Simulator.EnemiesFactory.All)
                results.Add(e.Type.ToString());

            return results;
        }


        protected override PanelWidget GetWidget(string assetName)
        {
            EnemyType e = (EnemyType) Enum.Parse(typeof(EnemyType), assetName);

            return new EnemyCheckBox(EnemiesFactory.ImagesEnemies[e], e, 4);
        }
    }
}
