namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CelestialBodyAttributesPanel : VerticalPanel
    {
        public CelestialBody CelestialBody;

        private CheckBox HasMoons;
        private CheckBox FollowPath;
        private CheckBox CanSelect;
        private CheckBox StraightLine;
        private CheckBox Invincible;


        public CelestialBodyAttributesPanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(600, 600), VisualPriorities.Default.EditorPanel, Color.White)
        {
            SetTitle("Celestial body attributes");

            HasMoons = new CheckBox("has moons")            { SpaceForLabel = 350 };
            FollowPath = new CheckBox("follow the path")    { SpaceForLabel = 350 };
            CanSelect = new CheckBox("can select")          { SpaceForLabel = 350 };
            StraightLine = new CheckBox("straight line")    { SpaceForLabel = 350 };
            Invincible = new CheckBox("invincible")         { SpaceForLabel = 350 };

            AddWidget("HasMoons", HasMoons);
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

            HasMoons.Value = CelestialBody.HasMoons;
            FollowPath.Value = CelestialBody.FollowPath;
            CanSelect.Value = CelestialBody.CanSelect;
            StraightLine.Value = CelestialBody.StraightLine;
            Invincible.Value = CelestialBody.Invincible;
        }
    }
}
