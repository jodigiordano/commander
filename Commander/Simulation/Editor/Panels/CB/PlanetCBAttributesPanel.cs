namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;

    
    class PlanetCBAttributesPanel : CelestialBodyAttributesPanel
    {
        private CheckBox HasMoons;


        public PlanetCBAttributesPanel(Simulator simulator)
            : base(simulator, new Vector2(600, 600))
        {
            HasMoons = new CheckBox("has moons") { SpaceForLabel = 350, ClickHandler = DoHasMoons };

            AddWidget("HasMoons", HasMoons);
        }


        public override void Open()
        {
            base.Open();

            HasMoons.Value = ((Planet) CelestialBody).HasMoons;
        }


        private void DoHasMoons(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("HasMoons")
            {
                HasMoons = HasMoons.Value,
                CelestialBody = CelestialBody
            });
        }
    }
}
