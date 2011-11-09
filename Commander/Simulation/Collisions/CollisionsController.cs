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
        public List<Turret> Turrets;
        public List<Mineral> Minerals;
        public bool Debug;
        

        public event PhysicalObjectHandler ObjectOutOfBounds;
        public event PhysicalObjectPhysicalObjectHandler ObjectHit;
        public event TurretTurretHandler TurretBoosted;
        public event TurretPhysicalObjectHandler InTurretRange;
        public event EnemyBulletHandler BulletDeflected;
        public event SimPlayerSimPlayerHandler PlayersCollided;
        public event BulletCelestialBodyHandler StartingPathCollision;
        public event CollidableBulletHandler ShieldCollided;

        private Simulator Simulator;
        private GridWorld EnemiesGrid;
        private GridWorld TurretsGrid;

        private HiddenEnemies HiddenEnemies;
        private BoostedTurrets BoostedTurrets;
        private BulletsDeflected BulletsDeflected;
        private OutOfBounds OutOfBounds;
        private EnemyInRange EnemyInRange;
        private CelestialBodyExplosion CelestialBodyExplosion;
        private ObjectsCollisions ObjectsCollisions;
        private PlayersCollisions PlayersCollisions;
        private StartingPathCollisions StartingPathCollisions;
        private SpaceshipShieldsCollisions ShieldsCollisions;

        private Action SyncOutOfBounds;
        private Action SyncEnemyInRange;
        private Action SyncBoostedTurrets;
        private Action SyncBulletsDeflected;


        public CollisionsController(Simulator simulator)
        {
            Simulator = simulator;
            
            HiddenEnemies = new HiddenEnemies(Simulator);
            BoostedTurrets = new BoostedTurrets();
            BulletsDeflected = new BulletsDeflected(Simulator);
            OutOfBounds = new OutOfBounds(Simulator);
            EnemyInRange = new EnemyInRange(Simulator);
            ObjectsCollisions = new ObjectsCollisions(Simulator);
            PlayersCollisions = new PlayersCollisions(Simulator);
            CelestialBodyExplosion = new CelestialBodyExplosion(Simulator);
            StartingPathCollisions = new StartingPathCollisions(Simulator);
            ShieldsCollisions = new SpaceshipShieldsCollisions(Simulator);

            SyncOutOfBounds = new Action(OutOfBounds.Sync);
            SyncEnemyInRange = new Action(EnemyInRange.Sync);
            SyncBoostedTurrets = new Action(BoostedTurrets.Sync);
            SyncBulletsDeflected = new Action(BulletsDeflected.Sync);
        }


        public void Initialize()
        {
            Debug = false;

            EnemiesGrid = new GridWorld(Simulator.Data.Battlefield.Inner, 50);
            TurretsGrid = new GridWorld(Simulator.Data.Battlefield.Inner, 50);

            HiddenEnemies.EnemiesGrid = EnemiesGrid;
            BoostedTurrets.Turrets = Turrets;
            BoostedTurrets.TurretsGrid = TurretsGrid;
            BulletsDeflected.EnemiesGrid = EnemiesGrid;
            EnemyInRange.EnemiesGrid = EnemiesGrid;
            EnemyInRange.HiddenEnemies = HiddenEnemies;
            EnemyInRange.Turrets = Turrets;
            CelestialBodyExplosion.EnemiesGrid = EnemiesGrid;
            ObjectsCollisions.EnemiesGrid = EnemiesGrid;
            ObjectsCollisions.HiddenEnemies = HiddenEnemies;
            ObjectsCollisions.Minerals = Minerals;
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
            PlayersCollisions.Sync();
            StartingPathCollisions.Sync();
            ShieldsCollisions.Sync();

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

            foreach (var d in PlayersCollisions.Output)
                NotifyPlayersCollided(d.Key, d.Value);

            foreach (var d in StartingPathCollisions.Output)
                NotifyStartingPathCollision(d.Key, d.Value);

            foreach (var d in ShieldsCollisions.Output)
                NotifyShieldCollided(d.Key, d.Value);
        }


        public void Draw()
        {
            if (!Debug)
                return;

            Circle c = new Circle(Vector3.Zero, 0);

            foreach (var p in Simulator.Data.Bullets)
            {
                DrawRectangle(p, Color.Red);

                if (p.Explosive)
                {
                    c.Position = p.Position;
                    c.Radius = p.ExplosionRange;

                    Simulator.Scene.Add(new VisualRectangle(c.Rectangle, Color.Cyan));
                }
            }

            foreach (var corpsCeleste in Simulator.Data.Level.PlanetarySystem)
                DrawRectangle(corpsCeleste, Color.Yellow);

            foreach (var e in Simulator.Data.Enemies)
            {
                Simulator.Scene.Add(new VisualRectangle(e.Circle.Rectangle, e.Color));
                DrawRectangle(e, e.Color);
            }

            foreach (var e in Minerals)
                DrawRectangle(e, Color.Yellow);

            EnemiesGrid.Draw(Simulator.Scene, Simulator.Data.Enemies);
        }


        private void DrawRectangle(ICollidable objet, Color couleur)
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
            for (int i = 0; i < Simulator.Data.Enemies.Count; i++)
                EnemiesGrid.Add(i, Simulator.Data.Enemies[i]);
        }


        public void DoObjectDestroyed(ICollidable obj)
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


            if (obj is Spaceship)
                ShieldsCollisions.Spaceships.Remove((Spaceship) obj);
        }


        public void DoObjectCreated(ICollidable obj)
        {
            if (obj is SpaceshipCollector)
                ObjectsCollisions.Collector = (Spaceship) obj;

            else if (obj is SpaceshipAutomaticCollector)
                ObjectsCollisions.AutomaticCollector = (SpaceshipAutomaticCollector) obj;

            else if (obj is Spaceship)
                ShieldsCollisions.Spaceships.Add((Spaceship) obj);
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp is PowerUpDeadlyShootingStars)
                ObjectsCollisions.DeadlyShootingStars = true;
            else if (powerUp is PowerUpDarkSide)
                ObjectsCollisions.DarkSide = true;
        }


        private void NotifyObjectOutOfBounds(ICollidable obj)
        {
            if (ObjectOutOfBounds != null)
                ObjectOutOfBounds(obj);
        }


        private void NotifyObjectHit(ICollidable obj, ICollidable by)
        {
            if (ObjectHit != null)
                ObjectHit(obj, by);
        }


        private void NotifyInTurretRange(Turret turret, ICollidable obj)
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


        private void NotifyPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            if (PlayersCollided != null)
                PlayersCollided(p1, p2);
        }


        private void NotifyStartingPathCollision(CelestialBody cb, Bullet b)
        {
            if (StartingPathCollision != null)
                StartingPathCollision(b, cb);
        }


        private void NotifyShieldCollided(ICollidable i, Bullet b)
        {
            if (ShieldCollided != null)
                ShieldCollided(i, b);
        }
    }
}
