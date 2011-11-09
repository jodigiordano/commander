namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CelestialBodyAttributesPanel : VerticalPanel
    {
        public CelestialBody CelestialBody;

        private Simulator Simulator;

        private CheckBox HasMoons;
        private CheckBox FollowPath;
        private CheckBox CanSelect;
        private CheckBox StraightLine;
        private CheckBox Invincible;


        public CelestialBodyAttributesPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("Celestial body attributes");

            HasMoons = new CheckBox("has moons")            { SpaceForLabel = 200 };
            FollowPath = new CheckBox("follow the path")    { SpaceForLabel = 200 };
            CanSelect = new CheckBox("can select")          { SpaceForLabel = 200 };
            StraightLine = new CheckBox("straight line")    { SpaceForLabel = 200 };
            Invincible = new CheckBox("invincible")         { SpaceForLabel = 200 };

            AddWidget("HasMoons", HasMoons);
            AddWidget("FollowPath", FollowPath);
            AddWidget("CanSelect", CanSelect);
            AddWidget("StraightLine", StraightLine);
            AddWidget("Invincible", Invincible);

            Alpha = 0;

            Initialize();
        }


        public override void Initialize()
        {
            base.Initialize();

            HasMoons.Value = CelestialBody.HasMoons;
            FollowPath.Value = CelestialBody.FollowPath;
            CanSelect.Value = CelestialBody.CanSelect;
            StraightLine.Value = CelestialBody.StraightLine;
            Invincible.Value = CelestialBody.Invincible;
        }
    }
}
