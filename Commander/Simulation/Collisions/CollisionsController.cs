namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ParallelTasks;


    class CollisionsController
    {
        public List<Bullet> Bullets;
        public List<Enemy> Enemies;
        public List<Turret> Turrets;
        public List<CelestialBody> CelestialBodies;
        public List<Mineral> Minerals;
        public List<ShootingStar> ShootingStars;
        public bool Debug;

        public event PhysicalObjectHandler ObjectOutOfBounds;
        public event PhysicalObjectPhysicalObjectHandler ObjectHit;
        public event TurretTurretHandler TurretBoosted;
        public event TurretPhysicalObjectHandler InTurretRange;
        public event EnemyBulletHandler BulletDeflected;

        private Simulator Simulator;
        private PhysicalRectangle Battlefield;
        private GridWorld EnemiesGrid;
        private GridWorld TurretsGrid;

        private HiddenEnemies HiddenEnemies;
        private BoostedTurrets BoostedTurrets;
        private BulletsDeflected BulletsDeflected;
        private OutOfBounds OutOfBounds;
        private EnemyInRange EnemyInRange;
        private CelestialBodyExplosion CelestialBodyExplosion;
        private ObjectsCollisions ObjectsCollisions;

        private Action SyncOutOfBounds;
        private Action SyncEnemyInRange;
        private Action SyncBoostedTurrets;
        private Action SyncBulletsDeflected;


        public CollisionsController(Simulator simulator)
        {
            Simulator = simulator;
            Battlefield = Simulator.Terrain;
            EnemiesGrid = new GridWorld(Battlefield, 50);
            TurretsGrid = new GridWorld(Battlefield, 50);
            
            HiddenEnemies = new HiddenEnemies();
            BoostedTurrets = new BoostedTurrets();
            BulletsDeflected = new BulletsDeflected();
            OutOfBounds = new OutOfBounds();
            EnemyInRange = new EnemyInRange();
            ObjectsCollisions = new ObjectsCollisions();
            CelestialBodyExplosion = new CelestialBodyExplosion();

            SyncOutOfBounds = new Action(OutOfBounds.Sync);
            SyncEnemyInRange = new Action(EnemyInRange.Sync);
            SyncBoostedTurrets = new Action(BoostedTurrets.Sync);
            SyncBulletsDeflected = new Action(BulletsDeflected.Sync);
        }


        public void Initialize()
        {
            Debug = false;

            HiddenEnemies.CelestialBodies = CelestialBodies;
            HiddenEnemies.Enemies = Enemies;
            HiddenEnemies.EnemiesGrid = EnemiesGrid;

            BoostedTurrets.Turrets = Turrets;
            BoostedTurrets.TurretsGrid = TurretsGrid;

            BulletsDeflected.Bullets = Bullets;
            BulletsDeflected.Enemies = Enemies;
            BulletsDeflected.EnemiesGrid = EnemiesGrid;

            OutOfBounds.Battlefield = Battlefield;
            OutOfBounds.Bullets = Bullets;

            EnemyInRange.Enemies = Enemies;
            EnemyInRange.EnemiesGrid = EnemiesGrid;
            EnemyInRange.HiddenEnemies = HiddenEnemies;
            EnemyInRange.Turrets = Turrets;

            CelestialBodyExplosion.Enemies = Enemies;
            CelestialBodyExplosion.EnemiesGrid = EnemiesGrid;

            ObjectsCollisions.Bullets = Bullets;
            ObjectsCollisions.Enemies = Enemies;
            ObjectsCollisions.EnemiesGrid = EnemiesGrid;
            ObjectsCollisions.HiddenEnemies = HiddenEnemies;
            ObjectsCollisions.Minerals = Minerals;
            ObjectsCollisions.ShootingStars = ShootingStars;
            ObjectsCollisions.Simulator = Simulator;
            ObjectsCollisions.Turrets = Turrets;
        }


        public void Update()
        {
            Task task = Parallel.Start(SyncEnemiesGrid);

            SyncTurretsGrid();

            task.Wait();

            HiddenEnemies.Sync(); // needed for other collisions (can't do in //)

            Task task3 = Parallel.Start(SyncOutOfBounds);
            Task task4 = Parallel.Start(SyncEnemyInRange);
            Task task5 = Parallel.Start(SyncBoostedTurrets);
            Task task6 = Parallel.Start(SyncBulletsDeflected);

            ObjectsCollisions.Sync();

            task3.Wait();
            task4.Wait();
            task5.Wait();
            task6.Wait();

            NotifyAll();
        }


        public void NotifyAll()
        {
            foreach (var b in OutOfBounds.Output)
                NotifyObjectOutOfBounds(b);

            foreach (var c in ObjectsCollisions.Output)
                NotifyObjectHit(c.Key, c.Value);

            foreach (var et in EnemyInRange.Output)
                NotifyInTurretRange(et.Key, et.Value);

            foreach (var b in BoostedTurrets.Output)
                NotifyTurretBoosted(b.Key, b.Value);

            foreach (var d in BulletsDeflected.Output)
                NotifyBulletDeflected(d.Key, d.Value);
        }


        public void Draw()
        {
            if (!Debug)
                return;

            Circle c = new Circle(Vector3.Zero, 0);

            foreach (var p in Bullets)
            {
                DrawRectangle(p, Color.Red);

                if (p.Explosive)
                {
                    c.Position = p.Position;
                    c.Radius = p.ExplosionRange;

                    Simulator.Scene.Add(new VisualRectangle(c.Rectangle, Color.Cyan));
                }
            }

            foreach (var corpsCeleste in CelestialBodies)
                DrawRectangle(corpsCeleste, Color.Yellow);

            foreach (var e in Enemies)
            {
                Simulator.Scene.Add(new VisualRectangle(e.Circle.Rectangle, e.Color));
                DrawRectangle(e, e.Color);
            }

            foreach (var e in Minerals)
                DrawRectangle(e, Color.Yellow);

            EnemiesGrid.Draw(Simulator.Scene, Enemies);
        }


        private void DrawRectangle(IObjetPhysique objet, Color couleur)
        {
            if (objet.Shape == Shape.Rectangle)
                Simulator.Scene.Add(new VisualRectangle(objet.Rectangle.RectanglePrimitif, couleur));
            else if (objet.Shape == Shape.Circle)
                Simulator.Scene.Add(new VisualRectangle(objet.Circle.Rectangle, couleur));
        }


        private void SyncTurretsGrid()
        {
            TurretsGrid.Clear();
            for (int i = 0; i < Turrets.Count; i++)
                TurretsGrid.Add(i, Turrets[i]);
        }

        private void SyncEnemiesGrid()
        {
            EnemiesGrid.Clear();
            for (int i = 0; i < Enemies.Count; i++)
                EnemiesGrid.Add(i, Enemies[i]);
        }


        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                CelestialBodyExplosion.CurrentCelestialBody = celestialBody;
                CelestialBodyExplosion.Sync();

                foreach (var e in CelestialBodyExplosion.Output.Values)
                    NotifyObjectHit(e, celestialBody);

                return;
            }


            SpaceshipCollector collector = obj as SpaceshipCollector;

            if (collector != null)
            {
                ObjectsCollisions.Collector = null;

                return;
            }


            SpaceshipAutomaticCollector acollector = obj as SpaceshipAutomaticCollector;

            if (acollector != null)
            {
                ObjectsCollisions.AutomaticCollector = null;

                return;
            }
        }


        public void DoObjectCreated(IObjetPhysique obj)
        {
            if (obj is SpaceshipCollector)
                ObjectsCollisions.Collector = (Spaceship) obj;

            else if (obj is SpaceshipAutomaticCollector)
                ObjectsCollisions.AutomaticCollector = (SpaceshipAutomaticCollector) obj;
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp is PowerUpDeadlyShootingStars)
                ObjectsCollisions.DeadlyShootingStars = true;
            else if (powerUp is PowerUpDarkSide)
                ObjectsCollisions.DarkSide = true;
        }


        private void NotifyObjectOutOfBounds(IObjetPhysique obj)
        {
            if (ObjectOutOfBounds != null)
                ObjectOutOfBounds(obj);
        }


        private void NotifyObjectHit(IObjetPhysique obj, IObjetPhysique by)
        {
            if (ObjectHit != null)
                ObjectHit(obj, by);
        }


        private void NotifyInTurretRange(Turret turret, IObjetPhysique obj)
        {
            if (InTurretRange != null)
                InTurretRange(turret, obj);
        }


        private void NotifyTurretBoosted(Turret boostingTurret, Turret boostedTurret)
        {
            if (TurretBoosted != null)
                TurretBoosted(boostingTurret, boostedTurret);
        }


        private void NotifyBulletDeflected(Enemy enemy, Bullet bullet)
        {
            if (BulletDeflected != null)
                BulletDeflected(enemy, bullet);
        }
    }
}
