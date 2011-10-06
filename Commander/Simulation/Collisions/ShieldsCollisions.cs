namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class SpaceshipShieldsCollisions
    {
        public List<KeyValuePair<ICollidable, Bullet>> Output;

        public List<Bullet> Bullets;
        public List<Spaceship> Spaceships;


        public SpaceshipShieldsCollisions()
        {
            Spaceships = new List<Spaceship>();
            Output = new List<KeyValuePair<ICollidable, Bullet>>();
        }


        public void Sync()
        {
            Output.Clear();

            foreach (var o in Spaceships)
            {
                foreach (var b in Bullets)
                {
                    if (!(b is BasicBullet || b is MissileBullet || b is RailGunBullet))
                        continue;

                    if (b.FiredBy == o.Id)
                        continue;

                    if (Physics.CircleRectangleCollision(o.ShieldCircle, b.Rectangle))
                        Output.Add(new KeyValuePair<ICollidable, Bullet>(o, b));
                }
            }
        }
    }
}
