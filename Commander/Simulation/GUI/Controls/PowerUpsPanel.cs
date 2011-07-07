namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PowerUpsPanel : GridPanel
    {
        private Simulator Simulator;


        public PowerUpsPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Availables Power-Ups");

            foreach (var powerUp in Simulator.PowerUpsFactory.All)
                AddWidget(powerUp.Key.ToString(),
                    new PowerUpCheckBox(powerUp.Value.Category == PowerUpCategory.Turret ?
                        Simulator.TurretsFactory.All[powerUp.Value.AssociatedTurret].BaseImage.TextureName : powerUp.Value.BuyImage,
                        powerUp.Key));

            Initialize();
        }


        public void Initialize()
        {
            foreach (var powerUp in Simulator.PowerUpsFactory.All)
                ((PowerUpCheckBox) Widgets[powerUp.Key.ToString()]).Checked =
                    Simulator.PowerUpsFactory.Availables.ContainsKey(powerUp.Key);
        }
    }
}
