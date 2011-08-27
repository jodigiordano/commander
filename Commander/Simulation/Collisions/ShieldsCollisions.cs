namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class ShieldsCollisions
    {
        public List<KeyValuePair<ICollidable, Bullet>> Output;

        public List<Bullet> Bullets;
        public List<ICollidable> Objects;

        private Circle ObjectCircle;


        public ShieldsCollisions()
        {
            Objects = new List<ICollidable>();
            Output = new List<KeyValuePair<ICollidable, Bullet>>();
            ObjectCircle = new Circle();
        }


        public void Sync()
        {
            Output.Clear();

            foreach (var o in Objects)
            {
                ObjectCircle.Radius = o.Circle.Radius + 10;
                ObjectCircle.Position = o.Position;

                foreach (var b in Bullets)
                {
                    if (!(b is BasicBullet || b is MissileBullet || b is RailGunBullet))
                        continue;

                    if (Physics.CircleRectangleCollision(ObjectCircle, b.Rectangle))
                        Output.Add(new KeyValuePair<ICollidable, Bullet>(o, b));
                }
            }
        }
    }
}
