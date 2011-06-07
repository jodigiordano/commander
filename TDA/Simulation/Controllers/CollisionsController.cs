namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;


    class CollisionsController
    {
        public event PhysicalObjectHandler ObjectOutOfBounds;
        public event PhysicalObjectPhysicalObjectHandler ObjectHit;
        public event TurretTurretHandler TurretBoosted;
        public event TurretPhysicalObjectHandler InTurretRange;
        public event EnemyBulletHandler BulletDeflected;

        private Simulation Simulation;
        private RectanglePhysique Battlefield;
        private Spaceship Collector;
        private Spaceship AutomaticCollector;
        private bool DeadlyShootingStars;
        private bool DarkSide;
        private GridWorld EnemiesGrid;
        private GridWorld TurretsGrid;

        public List<Bullet> Bullets;
        public List<Enemy> Enemies;
        public List<Turret> Turrets;
        public List<CorpsCeleste> CelestialBodies;
        public List<Mineral> Minerals;
        public List<ShootingStar> ShootingStars;
        public bool Debug;


        public CollisionsController(Simulation simulation)
        {
            this.Simulation = simulation;
            this.Bullets = new List<Bullet>();
            this.Enemies = new List<Enemy>();
            this.CelestialBodies = new List<CorpsCeleste>();
            this.Turrets = new List<Turret>();
            this.Minerals = new List<Mineral>();
            this.Battlefield = simulation.Terrain;
            this.EnemiesGrid = new GridWorld(this.Battlefield, 50);
            this.TurretsGrid = new GridWorld(this.Battlefield, 50);
            this.Debug = false;
        }


        public void NotifyAll()
        {
            for (int i = 0; i < outOfBounds.Count; i++)
                NotifyObjectOutOfBounds(outOfBounds[i]);

            for (int i = 0; i < CollisionsThisTick.Count; i++)
                NotifyObjectHit(CollisionsThisTick[i].Key, CollisionsThisTick[i].Value);

            for (int i = 0; i < InTurretRangeThisTick.Count; i++)
                NotifyInTurretRange(InTurretRangeThisTick[i].Key, InTurretRangeThisTick[i].Value);

            for (int i = 0; i < BoostedThisTick.Count; i++)
                NotifyTurretBoosted(BoostedThisTick[i].Key, BoostedThisTick[i].Value);

            for (int i = 0; i < DeflectThisTick.Count; i++)
                NotifyBulletDeflected(DeflectThisTick[i].Key, DeflectThisTick[i].Value);
        }


        public void Update()
        {
            EnemiesGrid.Clear();
            for (int i = 0; i < Enemies.Count; i++)
                EnemiesGrid.Add(i, Enemies[i]);

            TurretsGrid.Clear();
            for (int i = 0; i < Turrets.Count; i++)
                TurretsGrid.Add(i, Turrets[i]);

            DoOutOfBoundsThisTick();
            DoHiddenEnemies();
            DoCollisionsThisTick();
            DoInTurretRangeThisTick();
            DoBoostedThisTick();
            DoDeflectThisTick();

            NotifyAll();
        }


        public void Draw()
        {
            if (Debug)
            {
                Cercle c = new Cercle(Vector3.Zero, 0);

                foreach (var p in Bullets)
                {
                    DrawRectangle(p, Color.Red);

                    if (p is MissileBullet)
                    {
                        c.Position = p.Position;
                        c.Radius = ((MissileBullet) p).ZoneImpact;

                        Simulation.Scene.ajouterScenable(new RectangleVisuel(c.Rectangle, Color.Cyan));
                    }
                }

                foreach (var corpsCeleste in CelestialBodies)
                    DrawRectangle(corpsCeleste, Color.Yellow);

                foreach (var e in Enemies)
                {
                    Simulation.Scene.ajouterScenable(new RectangleVisuel(e.Circle.Rectangle, Color.Yellow));
                    DrawRectangle(e, Color.Orange);
                }

                foreach (var e in Minerals)
                    DrawRectangle(e, Color.Yellow);

                if (Collector != null)
                    DrawRectangle(Collector, Color.Yellow);
            }
        }


        private void DrawRectangle(IObjetPhysique objet, Color couleur)
        {
            if (objet.Shape == Shape.Rectangle)
                Simulation.Scene.ajouterScenable(new RectangleVisuel(objet.Rectangle.RectanglePrimitif, couleur));
            else if (objet.Shape == Shape.Circle)
                Simulation.Scene.ajouterScenable(new RectangleVisuel(objet.Circle.Rectangle, couleur));
        }


        private List<IObjetPhysique> outOfBounds = new List<IObjetPhysique>();
        private void DoOutOfBoundsThisTick()
        {
            outOfBounds.Clear();

            // Pour des raisons de performance, on enleve le projectile de la liste ici et non dans ControleurProjectiles
            // (Ca fait mal au coeur mais performance oblige)
            for (int i = Bullets.Count - 1; i > -1; i--)
            {
                Bullet projectile = Bullets[i];

                if (projectile.Shape == Shape.Line || projectile.Shape == Shape.Circle)
                    continue;

                if (!EphemereGames.Core.Physique.Facade.collisionRectangleRectangle(projectile.Rectangle, this.Battlefield))
                {
                    Bullets[i].DoDieSilent();
                    Bullets.RemoveAt(i);
                }
            }
        }


        private List<KeyValuePair<IObjetPhysique, IObjetPhysique>> CollisionsThisTick = new List<KeyValuePair<IObjetPhysique, IObjetPhysique>>();
        private Dictionary<int, int> TmpObjects = new Dictionary<int, int>();
        //private Bag<int> tmpObjetsTraites = new Bag<int>(); // Ne fonctionne pas sur la Xbox 360

        private void DoCollisionsThisTick()
        {
            CollisionsThisTick.Clear();

            Cercle c = new Cercle(Vector3.Zero, 0);
            RectanglePhysique r = new RectanglePhysique();

            // Collisions Enemies <--> Celestial Bodies
            if (DarkSide)
            {
                foreach (var enemy in hiddenEnemies.Values)
                    CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(enemy, ((PowerUpDarkSide) Simulation.PowerUpsFactory.Availables[PowerUpType.DarkSide]).CorpsCeleste));
            }


            // Collisions Projectiles <--> Ennemis

            for (int i = Bullets.Count - 1; i > -1; i--)
            {
                Bullet bullet = Bullets[i];

                TmpObjects.Clear();

                IEnumerable<int> candidates;

                if (bullet is MultipleLasersBullet || bullet is LaserBullet)
                    candidates = EnemiesGrid.GetItems(bullet.Line);
                else
                    candidates = EnemiesGrid.GetItems(bullet.Rectangle);

                int nbCollisions = 0;

                foreach (var index in candidates)
                {
                    bool collision = false;

                    if (TmpObjects.ContainsKey(index))
                        continue;
                    else
                        TmpObjects.Add(index, 0);

                    Enemy e = Enemies[index];

                    //Degeux
                    if (bullet.Shape == Shape.Rectangle && e.Shape == Shape.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionRectangleRectangle(bullet.Rectangle, e.Rectangle);
                    }

                    else if (bullet.Shape == Shape.Circle && e.Shape == Shape.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionCercleRectangle(bullet.Circle, e.Rectangle);
                    }

                    else if (bullet.Shape == Shape.Rectangle && e.Shape == Shape.Circle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionCercleRectangle(e.Circle, bullet.Rectangle);
                    }

                    else if (bullet.Shape == Shape.Line && e.Shape == Shape.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionLigneRectangle(bullet.Line, e.Rectangle);
                    }

                    if (collision)
                    {
                        nbCollisions++;

                        CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, bullet));

                        if (!(bullet is MultipleLasersBullet || bullet is SlowMotionBullet))
                        {
                            bullet.DoHit(e);

                            if (!bullet.Alive)
                            {
                                bullet.DoDie();
                                Bullets.RemoveAt(i);

                                if (bullet is MissileBullet || bullet is NanobotsBullet || bullet is RailGunBullet)
                                {
                                    c.Position = bullet.Position;
                                    c.Radius = bullet is MissileBullet ? ((MissileBullet) bullet).ZoneImpact :
                                               bullet is NanobotsBullet ? ((NanobotsBullet) bullet).ZoneImpact :
                                               ((RailGunBullet) bullet).ZoneImpact;

                                    TmpObjects.Clear();
                                    IEnumerable<int> candidats2 = EnemiesGrid.GetItems(c.Rectangle);

                                    foreach (var indice2 in candidats2)
                                    {
                                        if (TmpObjects.ContainsKey(indice2))
                                            continue;
                                        else
                                            TmpObjects.Add(indice2, 0);

                                        Enemy ennemi = Enemies[indice2];

                                        if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, ennemi.Circle))
                                            CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(ennemi, bullet));
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }


            // Collisions Mineraux <--> Collecteur

            if (Collector != null)
            {
                for (int i = 0; i < Minerals.Count; i++)
                {
                    Mineral mineral = Minerals[i];

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(mineral.Circle, Collector.Circle))
                    {
                        CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(mineral, Collector));
                        CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(Collector, mineral));
                    }
                }
            }


            // Collisions Mineraux <--> Collecteur

            if (AutomaticCollector != null)
            {
                for (int i = 0; i < Minerals.Count; i++)
                {
                    Mineral mineral = Minerals[i];

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(mineral.Circle, AutomaticCollector.Circle))
                    {
                        CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(mineral, AutomaticCollector));
                        CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(AutomaticCollector, mineral));
                    }
                }
            }


            // Collisions Shooting Stars <--> Enemies
            if (DeadlyShootingStars)
            {
                for (int i = 0; i < ShootingStars.Count; i++)
                {
                    ShootingStar star = ShootingStars[i];

                    r.X = (int) (star.Position.X - star.Circle.Radius);
                    r.Y = (int) (star.Position.Y - star.Circle.Radius);
                    r.Width = (int) (star.Circle.Radius * 2);
                    r.Height = (int) (star.Circle.Radius * 2);

                    TmpObjects.Clear();

                    IEnumerable<int> candidates = EnemiesGrid.GetItems(r);

                    int nbCollisions = 0;

                    foreach (var index in candidates)
                    {
                        bool collision = false;

                        if (TmpObjects.ContainsKey(index))
                            continue;
                        else
                            TmpObjects.Add(index, 0);

                        Enemy e = Enemies[index];

                        if (e.Shape == Shape.Rectangle)
                        {
                            collision = EphemereGames.Core.Physique.Facade.collisionRectangleRectangle(r, e.Rectangle);
                        }

                        else if (e.Shape == Shape.Circle)
                        {
                            collision = EphemereGames.Core.Physique.Facade.collisionCercleCercle(star.Circle, e.Circle);
                        }

                        if (collision)
                        {
                            nbCollisions++;

                            CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, star));

                            c.Position = star.Position;
                            c.Radius = star.ZoneImpact;

                            TmpObjects.Clear();
                            IEnumerable<int> candidates2 = EnemiesGrid.GetItems(c.Rectangle);

                            foreach (var index2 in candidates2)
                            {
                                if (TmpObjects.ContainsKey(index2))
                                    continue;
                                else
                                    TmpObjects.Add(index2, 0);

                                Enemy ennemi = Enemies[index2];

                                if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, ennemi.Circle))
                                    CollisionsThisTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(ennemi, star));
                            }

                            break;
                        }
                    }
                }
            }
        }


        private Dictionary<int, Enemy> hiddenEnemies = new Dictionary<int, Enemy>();
        private void DoHiddenEnemies()
        {
            hiddenEnemies.Clear();

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                CorpsCeleste corpsCeleste = CelestialBodies[i];

                if (corpsCeleste.EstDernierRelais)
                    continue;

                IEnumerable<int> candidats = EnemiesGrid.GetItems(corpsCeleste.Circle.Rectangle);

                foreach (var indice in candidats)
                {
                    Enemy e = Enemies[indice];

                    if (!hiddenEnemies.ContainsKey(e.GetHashCode()) &&
                        EphemereGames.Core.Physique.Facade.collisionCercleCercle(corpsCeleste.Circle, e.Circle))
                    {
                        hiddenEnemies.Add(e.GetHashCode(), e);
                    }
                }
            }
        }


        private List<KeyValuePair<Turret, IObjetPhysique>> InTurretRangeThisTick = new List<KeyValuePair<Turret, IObjetPhysique>>();
        private void DoInTurretRangeThisTick()
        {
            InTurretRangeThisTick.Clear();

            RectanglePhysique r = new RectanglePhysique();
            Cercle c = new Cercle(Vector3.Zero, 1);

            // Collisions Tourelle.ZoneActivation <--> Ennemis
            for (int i = 0; i < Turrets.Count; i++)
            {
                Turret tourelle = Turrets[i];

                if (tourelle.Type == TurretType.Gravitational || tourelle.Type == TurretType.Booster)
                    continue;

                float range = tourelle.Range;
                r.X = (int) (tourelle.Position.X - range);
                r.Y = (int) (tourelle.Position.Y - range);
                r.Width = (int) (range * 2);
                r.Height = (int) (range * 2);

                c.Position = tourelle.Position;
                c.Radius = range;

                IEnumerable<int> candidats = EnemiesGrid.GetItems(r);

                foreach (var indice in candidats)
                {
                    Enemy e = Enemies[indice];

                    if (hiddenEnemies.ContainsKey(e.GetHashCode()))
                        continue;

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, e.Circle))
                    {
                        InTurretRangeThisTick.Add(new KeyValuePair<Turret, IObjetPhysique>(tourelle, e));
                        break; // passe à la prochaine tourelle
                    }
                }
            }
        }


        private List<KeyValuePair<Enemy, Bullet>> DeflectThisTick = new List<KeyValuePair<Enemy, Bullet>>();
        private void DoDeflectThisTick()
        {
            DeflectThisTick.Clear();

            RectanglePhysique r = new RectanglePhysique();
            Cercle c = new Cercle(Vector3.Zero, 1);

            // Collisions Normal Bullets, Projectiles <--> Deflecting Enemies
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullet b = Bullets[i];

                if (!(b is BasicBullet || b is MissileBullet ))
                    continue;

                float range = b.DeflectZone;
                r.X = (int) (b.Position.X - range);
                r.Y = (int) (b.Position.Y - range);
                r.Width = (int) (range * 2);
                r.Height = (int) (range * 2);

                c.Position = b.Position;
                c.Radius = range;

                IEnumerable<int> candidates = EnemiesGrid.GetItems(r);

                foreach (var index in candidates)
                {
                    Enemy e = Enemies[index];

                    if (e.Type != EnemyType.Vulcanoid)
                        continue;

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, e.Circle))
                    {
                        DeflectThisTick.Add(new KeyValuePair<Enemy, Bullet>(e, b));
                        break;
                    }
                }
            }
        }


        private List<KeyValuePair<Turret, Turret>> BoostedThisTick = new List<KeyValuePair<Turret, Turret>>();
        private void DoBoostedThisTick()
        {
            BoostedThisTick.Clear();

            RectanglePhysique r = new RectanglePhysique();
            Cercle c = new Cercle(Vector3.Zero, 1);

            // Collisions BoosterTurret <--> Others Turrets
            for (int i = 0; i < Turrets.Count; i++)
            {
                Turret turret = Turrets[i];

                if (turret.Type != TurretType.Booster)
                    continue;

                float range = turret.Range;
                r.X = (int) (turret.Position.X - range);
                r.Y = (int) (turret.Position.Y - range);
                r.Width = (int) (range * 2);
                r.Height = (int) (range * 2);

                c.Position = turret.Position;
                c.Radius = range;

                IEnumerable<int> candidates = TurretsGrid.GetItems(r);

                foreach (var index in candidates)
                {
                    Turret boostedTurret = Turrets[index];

                    if (boostedTurret.Type == TurretType.Booster)
                        continue;

                    if (Core.Physique.Facade.collisionCercleCercle(turret.Circle, c))
                        BoostedThisTick.Add(new KeyValuePair<Turret, Turret>(turret, boostedTurret));
                }
            }
        }


        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            CorpsCeleste celestialBody = obj as CorpsCeleste;

            if (celestialBody != null)
            {
                TmpObjects.Clear();

                RectanglePhysique r = new RectanglePhysique(
                    (int) (celestialBody.position.X - celestialBody.ZoneImpactDestruction),
                    (int) (celestialBody.position.Y - celestialBody.ZoneImpactDestruction),
                    (int) (celestialBody.ZoneImpactDestruction * 2),
                    (int) (celestialBody.ZoneImpactDestruction * 2));
                Cercle c = new Cercle(celestialBody.position, celestialBody.ZoneImpactDestruction);

                IEnumerable<int> candidates = EnemiesGrid.GetItems(r);

                foreach (var index in candidates)
                {
                    if (TmpObjects.ContainsKey(index))
                        continue;
                    else
                        TmpObjects.Add(index, 0);

                    if (index >= Enemies.Count)
                        continue;

                    Enemy enemy = Enemies[index];

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, enemy.Circle))
                        NotifyObjectHit(enemy, celestialBody);
                }

                return;
            }


            SpaceshipCollector collector = obj as SpaceshipCollector;

            if (collector != null)
            {
                Collector = null;

                return;
            }


            SpaceshipAutomaticCollector acollector = obj as SpaceshipAutomaticCollector;

            if (acollector != null)
            {
                AutomaticCollector = null;

                return;
            }
        }


        public void DoObjectCreated(IObjetPhysique obj)
        {
            if (obj is SpaceshipCollector)
                Collector = (Spaceship) obj;

            else if (obj is SpaceshipAutomaticCollector)
                AutomaticCollector = (SpaceshipAutomaticCollector) obj;
        }


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp is PowerUpDeadlyShootingStars)
                DeadlyShootingStars = true;
            else if (powerUp is PowerUpDarkSide)
                DarkSide = true;
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
