namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class StartingPathCollisions
    {
        public List<KeyValuePair<CelestialBody, Bullet>> Output;

        public List<Bullet> Bullets;
        public Path Path;

        private Circle StartingPathObjectCircle;
        private CelestialBody StartingPathObject;


        public StartingPathCollisions()
        {
            Output = new List<KeyValuePair<CelestialBody, Bullet>>();
            StartingPathObjectCircle = new Circle();
        }


        public void Sync()
        {
            Output.Clear();

            StartingPathObject = Path.FirstCelestialBody;

            if (StartingPathObject == null)
                return;

            StartingPathObjectCircle.Radius = StartingPathObject.Circle.Radius + 30;
            StartingPathObjectCircle.Position = StartingPathObject.Position;

            foreach (var b in Bullets)
            {
                if (!(b is BasicBullet))
                    continue;

                if (Physics.CircleRectangleCollision(StartingPathObjectCircle, b.Rectangle))
                    Output.Add(new KeyValuePair<CelestialBody, Bullet>(StartingPathObject, b));
            }
        }
    }
}
