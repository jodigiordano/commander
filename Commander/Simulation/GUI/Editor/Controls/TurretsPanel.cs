namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class TurretsPanel : GridPanel
    {
        private Simulator Simulator;


        public TurretsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Availables Turrets");
            NbColumns = 4;

            foreach (var turret in Simulator.TurretsFactory.All)
                AddWidget(turret.Key.ToString(), new TurretCheckBox(Simulator.TurretsFactory.Create(turret.Key)));

            Initialize();
        }


        public override void Initialize()
        {
            base.Initialize();

            foreach (var turret in Simulator.TurretsFactory.All)
            {
                var widget = (TurretCheckBox) GetWidgetByName(turret.Key.ToString());

                widget.Value = Simulator.TurretsFactory.Availables.ContainsKey(turret.Key);
            }
        }
    }
}
