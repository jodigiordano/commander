namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    abstract class CelestialBodyAttributesPanel : VerticalPanel, IEditorCBPanel
    {
        public CelestialBody CelestialBody  { get; set; }
        public Simulator Simulator          { get; set; }

        private CheckBox FollowPath;
        private CheckBox CanSelect;
        private CheckBox StraightLine;
        private CheckBox Invincible;


        public CelestialBodyAttributesPanel(Simulator simulator, Vector2 size)
            : base(simulator.Scene, Vector3.Zero, size, VisualPriorities.Default.EditorPanel, Color.White)
        {
            SetTitle("Celestial body attributes");

            Simulator = simulator;

            FollowPath = new CheckBox("follow the path")    { SpaceForLabel = 350, ClickHandler = DoFollowPath };
            CanSelect = new CheckBox("can select")          { SpaceForLabel = 350, ClickHandler = DoCanSelect };
            StraightLine = new CheckBox("straight line")    { SpaceForLabel = 350, ClickHandler = DoStraightLine };
            Invincible = new CheckBox("invincible")         { SpaceForLabel = 350, ClickHandler = DoInvincible };

            AddWidget("FollowPath", FollowPath);
            AddWidget("CanSelect", CanSelect);
            AddWidget("StraightLine", StraightLine);
            AddWidget("Invincible", Invincible);

            Padding = new Vector2(30, 50);

            Alpha = 0;
        }


        public override void Open()
        {
            base.Open();

            FollowPath.Value = CelestialBody.FollowPath;
            CanSelect.Value = CelestialBody.canSelect;
            StraightLine.Value = CelestialBody.StraightLine;
            Invincible.Value = CelestialBody.Invincible;
        }


        private void DoFollowPath(PanelWidget widget, Commander.Player player)
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodyFollowPathCommand(Simulator.Data.Players[player], CelestialBody, FollowPath.Value));
        }


        private void DoCanSelect(PanelWidget widget, Commander.Player player)
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodyCanSelectCommand(Simulator.Data.Players[player], CelestialBody, CanSelect.Value));
        }


        private void DoStraightLine(PanelWidget widget, Commander.Player player)
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodyStraightLineCommand(Simulator.Data.Players[player], CelestialBody, StraightLine.Value));
        }


        private void DoInvincible(PanelWidget widget, Commander.Player player)
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodyInvincibleCommand(Simulator.Data.Players[player], CelestialBody, Invincible.Value));
        }
    }
}
