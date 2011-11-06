namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class SpaceshipShieldsCollisions
    {
        public List<KeyValuePair<ICollidable, Bullet>> Output;

        public List<Spaceship> Spaceships;

        private Simulator Simulator;


        public SpaceshipShieldsCollisions(Simulator simulator)
        {
            Simulator = simulator;

            Spaceships = new List<Spaceship>();
            Output = new List<KeyValuePair<ICollidable, Bullet>>();
        }


        public void Sync()
        {
            Output.Clear();

            foreach (var o in Spaceships)
            {
                foreach (var b in Simulator.Data.Bullets)
                {
                    if (!(b is BasicBullet || b is MissileBullet || b is RailGunBullet))
                        continue;

                    if (b.Owner == o.Id)
                        continue;

                    if (Physics.CircleRectangleCollision(o.ShieldCircle, b.Rectangle))
                        Output.Add(new KeyValuePair<ICollidable, Bullet>(o, b));
                }
            }
        }
    }
}
