namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class ObjectsCollisions
    {
        public List<KeyValuePair<IObjetPhysique, IObjetPhysique>> Output;

        public Simulator Simulator;
        public HiddenEnemies HiddenEnemies;
        public bool DarkSide;
        public bool DeadlyShootingStars;
        public GridWorld EnemiesGrid;
        public List<Turret> Turrets;
        public List<Bullet> Bullets;
        public List<Enemy> Enemies;
        public List<Mineral> Minerals;
        public List<ShootingStar> ShootingStars;
        public Spaceship Collector;
        public Spaceship AutomaticCollector;

        private GridWorld.IntegerHandler HandlerBulletEnemy;
        private GridWorld.IntegerHandler HandlerBulletExplosion;
        private GridWorld.IntegerHandler HandlerShootingStars;
        private GridWorld.IntegerHandler HandlerShootingStarExplosion;
        private Bullet CurrentBullet;
        private ShootingStar CurrentShootingStar;
        private PhysicalRectangle Rectangle;
        private Circle Circle;
        private Dictionary<int, int> TmpObjects;


        public ObjectsCollisions()
        {
            Output = new List<KeyValuePair<IObjetPhysique, IObjetPhysique>>();
            HandlerBulletEnemy = new GridWorld.IntegerHandler(CheckBulletEnemy);
            HandlerBulletExplosion = new GridWorld.IntegerHandler(CheckBulletExplosion);
            HandlerShootingStars = new GridWorld.IntegerHandler(CheckShootingStars);
            HandlerShootingStarExplosion = new GridWorld.IntegerHandler(CheckShootingStarExplosion);
            TmpObjects = new Dictionary<int, int>();
            Rectangle = new PhysicalRectangle();
            Circle = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            SyncDarkSide();
            SyncBulletsEnemies();
            SyncMineralsCollector(Collector);
            SyncMineralsCollector(AutomaticCollector);
            SyncShootingStars();
        }


        private void SyncShootingStars()
        {
            if (!DeadlyShootingStars)
                return;

            for (int i = 0; i < ShootingStars.Count; i++)
            {
                CurrentShootingStar = ShootingStars[i];

                Rectangle.X = (int) (CurrentShootingStar.Position.X - CurrentShootingStar.Circle.Radius);
                Rectangle.Y = (int) (CurrentShootingStar.Position.Y - CurrentShootingStar.Circle.Radius);
                Rectangle.Width = (int) (CurrentShootingStar.Circle.Radius * 2);
                Rectangle.Height = (int) (CurrentShootingStar.Circle.Radius * 2);

                TmpObjects.Clear();

                EnemiesGrid.GetItems(Rectangle, HandlerShootingStars);
            }
        }


        private void SyncDarkSide()
        {
            if (!DarkSide)
                return;

            foreach (var kvp in HiddenEnemies.Output)
            {
                if (kvp.Value.FirstOnPath || (kvp.Value.LastOnPath && kvp.Key.Path.Pourc(kvp.Key.Displacement) > 0.98))
                    continue;

                Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(kvp.Key, ((PowerUpDarkSide) Simulator.PowerUpsFactory.Availables[PowerUpType.DarkSide]).CorpsCeleste));
            }
        }


        private void SyncMineralsCollector(Spaceship Collector)
        {
            if (Collector == null)
                return;

            for (int i = 0; i < Minerals.Count; i++)
            {
                Mineral mineral = Minerals[i];

                if (Physics.CircleCicleCollision(mineral.Circle, Collector.Circle))
                {
                    Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(mineral, Collector));
                    Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(Collector, mineral));
                }
            }
        }

        
        private void SyncBulletsEnemies()
        {
            TmpObjects.Clear();

            for (int i = Bullets.Count - 1; i > -1; i--)
            {
                CurrentBullet = Bullets[i];

                if (CurrentBullet.Type == BulletType.LaserMultiple || CurrentBullet.Type == BulletType.LaserSimple)
                    EnemiesGrid.GetItems(CurrentBullet.Line, HandlerBulletEnemy);
                else
                    EnemiesGrid.GetItems(CurrentBullet.Rectangle, HandlerBulletEnemy);
            }
        }


        private bool CheckShootingStars(int index)
        {
            if (TmpObjects.ContainsKey(index))
                return true;
            else
                TmpObjects.Add(index, 0);

            Enemy e = Enemies[index];

            bool collision = false;

            if (e.Shape == Shape.Rectangle)
                collision = Physics.RectangleRectangleCollision(Rectangle, e.Rectangle);
            else if (e.Shape == Shape.Circle)
                collision = Physics.CircleCicleCollision(CurrentShootingStar.Circle, e.Circle);

            if (collision)
            {
                Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, CurrentShootingStar));

                Circle.Position = CurrentShootingStar.Position;
                Circle.Radius = CurrentShootingStar.ZoneImpact;

                TmpObjects.Clear();


                EnemiesGrid.GetItems(Circle.Rectangle, HandlerShootingStarExplosion);

                return true;
            }

            return false;
        }


        private bool CheckBulletEnemy(int index)
        {
            if (TmpObjects.ContainsKey(index))
                return true;
            else
                TmpObjects.Add(index, 0);

            Enemy e = Enemies[index];

            bool collision = false;

            
            //Degeux
            if (CurrentBullet.Shape == Shape.Rectangle && e.Shape == Shape.Rectangle)
            {
                //Physics.TransformRectangle(CurrentBullet.Rectangle, CurrentBullet.Rotation, Rectangle);

                collision = Physics.RectangleRectangleCollision(CurrentBullet.Rectangle, e.Rectangle);
            }

            else if (CurrentBullet.Shape == Shape.Circle && e.Shape == Shape.Rectangle)
            {
                collision = Physics.CircleRectangleCollision(CurrentBullet.Circle, e.Rectangle);
            }

            else if (CurrentBullet.Shape == Shape.Rectangle && e.Shape == Shape.Circle)
            {
                //Physics.TransformRectangle(CurrentBullet.Rectangle, CurrentBullet.Rotation, Rectangle);

                collision = Physics.CircleRectangleCollision(e.Circle, CurrentBullet.Rectangle);
            }

            else if (CurrentBullet.Shape == Shape.Line && e.Shape == Shape.Rectangle)
            {
                collision = Physics.LineRectangleCollision(CurrentBullet.Line, e.Rectangle);
            }

            if (collision)
            {
                Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, CurrentBullet));

                if (CurrentBullet.Type == BulletType.LaserMultiple || CurrentBullet.Type == BulletType.SlowMotion)
                    return true;

                if (CurrentBullet.Explosive)
                {
                    Circle.Position = CurrentBullet.Position;
                    Circle.Radius = CurrentBullet.ExplosionRange;

                    TmpObjects.Clear();

                    EnemiesGrid.GetItems(Circle.Rectangle, HandlerBulletExplosion);
                }

                return false;
            }

            return true;
        }


        private bool CheckBulletExplosion(int index)
        {
            if (TmpObjects.ContainsKey(index))
                return true;
            else
                TmpObjects.Add(index, 0);

            Enemy e = Enemies[index];

            if (Physics.CircleCicleCollision(Circle, e.Circle))
                Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, CurrentBullet));

            return true;
        }


        private bool CheckShootingStarExplosion(int index)
        {
            if (TmpObjects.ContainsKey(index))
                return true;
            else
                TmpObjects.Add(index, 0);

            Enemy e = Enemies[index];

            if (Physics.CircleCicleCollision(Circle, e.Circle))
                Output.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, CurrentShootingStar));

            return true;
        }
    }
}
