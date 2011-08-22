namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PlayerLivesLiteral
    {
        private CelestialBody celestialBody;

        private Simulator Simulator;
        private Image LifeImage;
        private Text LifesCount;


        public PlayerLivesLiteral(Simulator simulator)
        {
            Simulator = simulator;

            LifeImage = new Image("ViesEnnemis5")
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.PlayerLives,
                Origin = Vector2.Zero
            };

            LifesCount = new Text(@"Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.PlayerLives
            };
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set
            {
                celestialBody = value;
            }
        }


        public void Update()
        {
            if (CelestialBody == null)
                return;

            LifesCount.Data = CelestialBody.LifePoints.ToString();
        }


        public void Draw()
        {
            if (CelestialBody == null || CelestialBody.LifePoints <= 0)
                return;

            LifeImage.Position = CelestialBody.Position + new Vector3(-LifeImage.AbsoluteSize.X, CelestialBody.Circle.Radius + 10, 0);
            LifesCount.Position = CelestialBody.Position + new Vector3(5, CelestialBody.Circle.Radius + 10, 0);

            Simulator.Scene.Add(LifeImage);
            Simulator.Scene.Add(LifesCount);
        }
    }
}
