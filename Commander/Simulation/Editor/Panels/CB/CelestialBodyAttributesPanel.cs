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
            CanSelect.Value = CelestialBody.CanSelect;
            StraightLine.Value = CelestialBody.StraightLine;
            Invincible.Value = CelestialBody.Invincible;
        }


        private void DoFollowPath(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("FollowPath")
            {
                FollowPath = FollowPath.Value,
                CelestialBody = CelestialBody
            });
        }


        private void DoCanSelect(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("CanSelect")
            {
                CanSelect = CanSelect.Value,
                CelestialBody = CelestialBody
            });
        }


        private void DoStraightLine(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("StraightLine")
            {
                StraightLine = StraightLine.Value,
                CelestialBody = CelestialBody
            });
        }


        private void DoInvincible(PanelWidget widget)
        {
            Simulator.EditorController.NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("Invincible")
            {
                Invincible = Invincible.Value,
                CelestialBody = CelestialBody
            });
        }
    }
}
