namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class StartingPathCollisions
    {
        public List<KeyValuePair<CelestialBody, Bullet>> Output;

        private Circle StartingPathObjectCircle;
        private CelestialBody StartingPathObject;

        private Simulator Simulator;


        public StartingPathCollisions(Simulator simulator)
        {
            Simulator = simulator;

            Output = new List<KeyValuePair<CelestialBody, Bullet>>();
            StartingPathObjectCircle = new Circle();
        }


        public void Sync()
        {
            Output.Clear();

            StartingPathObject = Simulator.Data.Path.FirstCelestialBody;

            if (StartingPathObject == null)
                return;

            StartingPathObjectCircle.Radius = StartingPathObject.Circle.Radius + 30;
            StartingPathObjectCircle.Position = StartingPathObject.Position;

            foreach (var b in Simulator.Data.Bullets)
            {
                if (!(b is BasicBullet))
                    continue;

                if (Physics.CircleRectangleCollision(StartingPathObjectCircle, b.Rectangle))
                    Output.Add(new KeyValuePair<CelestialBody, Bullet>(StartingPathObject, b));
            }
        }
    }
}
