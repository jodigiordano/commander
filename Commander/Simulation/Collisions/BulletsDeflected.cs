namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class BulletsDeflected
    {
        public List<KeyValuePair<Enemy, Bullet>> Output;

        public GridWorld EnemiesGrid;
        public List<Bullet> Bullets;
        public List<Enemy> Enemies;

        private GridWorld.IntegerHandler Handler;
        private Bullet CurrentBullet;
        private RectanglePhysique CurrentBulletRectangle;
        private Circle CurrentBulletDeflectRange;
        
        
        public BulletsDeflected()
        {
            Output = new List<KeyValuePair<Enemy, Bullet>>();
            Handler = new GridWorld.IntegerHandler(CheckBulletIsDeflected);
            CurrentBullet = null;
            CurrentBulletRectangle = new RectanglePhysique();
            CurrentBulletDeflectRange = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Bullets.Count; i++)
            {
                CurrentBullet = Bullets[i];

                if (!(CurrentBullet is BasicBullet || CurrentBullet is MissileBullet))
                    continue;

                SyncRectangleAndCircle();

                EnemiesGrid.GetItems(CurrentBulletRectangle, Handler);
            }
        }


        private bool CheckBulletIsDeflected(int index)
        {
            Enemy e = Enemies[index];

            if (e.Type != EnemyType.Vulcanoid)
                return true;

            if (Physics.collisionCercleCercle(CurrentBulletDeflectRange, e.Circle))
            {
                Output.Add(new KeyValuePair<Enemy, Bullet>(e, CurrentBullet));
                return false;
            }

            return true;
        }


        private void SyncRectangleAndCircle()
        {
            float range = CurrentBullet.DeflectZone;
            CurrentBulletRectangle.X = (int) (CurrentBullet.Position.X - range);
            CurrentBulletRectangle.Y = (int) (CurrentBullet.Position.Y - range);
            CurrentBulletRectangle.Width = (int) (range * 2);
            CurrentBulletRectangle.Height = (int) (range * 2);

            CurrentBulletDeflectRange.Position = CurrentBullet.Position;
            CurrentBulletDeflectRange.Radius = range;
        }
    }
}
