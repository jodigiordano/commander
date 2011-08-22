namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PlayerCash
    {
        public CelestialBody CelestialBody;

        private Simulator Simulator;
        private CommonStash Stash;
        private Text Cash;


        public PlayerCash(Simulator simulator, CommonStash stash)
        {
            Simulator = simulator;
            Stash = stash;

            Cash = new Text(Stash.Cash.ToString(), @"Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.PlayerCash
            };
        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            Cash.Data = Stash.Cash + "$";
            Cash.CenterIt();
            Cash.Position = CelestialBody.Position + new Vector3(0, CelestialBody.Circle.Radius + 10, 0);

            Simulator.Scene.Add(Cash);
        }
    }
}
