namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;


    class ControleurCollisions : DrawableGameComponent
    {
        public event PhysicalObjectHandler ObjetDeserte;
        public event PhysicalObjectPhysicalObjectHandler ObjetTouche;
        public event TurretTurretHandler TurretBoosted;

        public delegate void DansZoneActivationHandler(Turret tourelle, IObjetPhysique objet);
        public event DansZoneActivationHandler DansZoneActivation;

        private Scene Scene;
        private Simulation Simulation;
        private RectanglePhysique Terrain;
        private SpaceshipCollector Collecteur;
        private GridWorld EnemiesGrid;
        private GridWorld TurretsGrid;

        public List<Projectile> Projectiles;
        public List<Ennemi> Ennemis;
        public List<Turret> Tourelles;
        public List<CorpsCeleste> CorpsCelestes;
        public List<Mineral> Mineraux;
        public bool Debug;


        public ControleurCollisions(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Scene = simulation.Scene;
            this.Projectiles = new List<Projectile>();
            this.Ennemis = new List<Ennemi>();
            this.CorpsCelestes = new List<CorpsCeleste>();
            this.Tourelles = new List<Turret>();
            this.Mineraux = new List<Mineral>();
            this.Terrain = simulation.Terrain;
            this.EnemiesGrid = new GridWorld(this.Terrain, 50);
            this.TurretsGrid = new GridWorld(this.Terrain, 50);
            this.Debug = false;
        }


        private void notifyObjetDeserte(IObjetPhysique deserteur)
        {
            if (ObjetDeserte != null)
                ObjetDeserte(deserteur);
        }


        private void notifyObjetTouche(IObjetPhysique objet, IObjetPhysique par)
        {
            if (ObjetTouche != null)
                ObjetTouche(objet, par);
        }


        private void notifyDansZoneActivation(Turret tourelle, IObjetPhysique objet)
        {
            if (DansZoneActivation != null)
                DansZoneActivation(tourelle, objet);
        }


        private void notifyTurretBoosted(Turret boostingTurret, Turret boostedTurret)
        {
            if (TurretBoosted != null)
                TurretBoosted(boostingTurret, boostedTurret);
        }


        public void Update()
        {
//#if XBOX
//            Simulation.Main.Threads[2].AddTask(new ThreadTask(doDeserteursCeTick));
//            Simulation.Main.Threads[2].AddTask(new ThreadTask(doCollisionsCeTick));

//            doDansZoneActivationCeTick();

//            while (Simulation.Main.Threads[2].Travaille()) { }
//#else
            doDeserteursCeTick();
            doCollisionsCeTick();
            doDansZoneActivationCeTick();
            doBoostCollisionsThisTick();
//#endif
        }

        public void notifyAll()
        {
            for (int i = 0; i < deserteurs.Count; i++)
                notifyObjetDeserte(deserteurs[i]);

            for (int i = 0; i < collisionsCeTick.Count; i++)
                notifyObjetTouche(collisionsCeTick[i].Key, collisionsCeTick[i].Value);

            for (int i = 0; i < collisionsDansZoneActivationCeTick.Count; i++)
                notifyDansZoneActivation(collisionsDansZoneActivationCeTick[i].Key, collisionsDansZoneActivationCeTick[i].Value);

            for (int i = 0; i < boostCollisionsThisTick.Count; i++)
                notifyTurretBoosted(boostCollisionsThisTick[i].Key, boostCollisionsThisTick[i].Value);
        }

        public override void Update(GameTime gameTime)
        {
            EnemiesGrid.Clear();
            for (int i = 0; i < Ennemis.Count; i++)
                EnemiesGrid.Add(i, Ennemis[i]);

            TurretsGrid.Clear();
            for (int i = 0; i < Tourelles.Count; i++)
                TurretsGrid.Add(i, Tourelles[i]);

            Update();
            notifyAll();
        }


        public override void Draw(GameTime gameTime)
        {
            if (Debug)
            {
                Cercle c = new Cercle(Vector3.Zero, 0);

                foreach (var p in Projectiles)
                {
                    dessinerRectangle(p, Color.Red);

                    if (p is ProjectileMissile)
                    {
                        c.Position = p.Position;
                        c.Radius = ((ProjectileMissile) p).ZoneImpact;

                        Scene.ajouterScenable(new RectangleVisuel(c.Rectangle, Color.Cyan));
                    }
                }

                foreach (var corpsCeleste in CorpsCelestes)
                    dessinerRectangle(corpsCeleste, Color.Yellow);

                foreach (var e in Ennemis)
                {
                    Scene.ajouterScenable(new RectangleVisuel(e.Cercle.Rectangle, Color.Yellow));
                    dessinerRectangle(e, Color.Orange);
                }

                foreach (var e in Mineraux)
                    dessinerRectangle(e, Color.Yellow);

                if (Collecteur != null)
                    dessinerRectangle(Collecteur, Color.Yellow);
            }
        }

        private void dessinerRectangle(IObjetPhysique objet, Color couleur)
        {
            if (objet.Forme == Forme.Rectangle)
                Scene.ajouterScenable(new RectangleVisuel(objet.Rectangle.RectanglePrimitif, couleur));
            else if (objet.Forme == Forme.Cercle)
                Scene.ajouterScenable(new RectangleVisuel(objet.Cercle.Rectangle, couleur));
        }

        private List<IObjetPhysique> deserteurs = new List<IObjetPhysique>();
        private void doDeserteursCeTick()
        {
            deserteurs.Clear();

            // Pour des raisons de performance, on enleve le projectile de la liste ici et non dans ControleurProjectiles
            // (Ca fait mal au coeur mais performance oblige)
            for (int i = Projectiles.Count - 1; i > -1; i--)
            {
                Projectile projectile = Projectiles[i];

                if (projectile.Forme == Forme.Ligne || projectile.Forme == Forme.Cercle)
                    continue;

                if (!EphemereGames.Core.Physique.Facade.collisionRectangleRectangle(projectile.Rectangle, this.Terrain))
                {
                    Projectiles[i].DoDieSilent();
                    Projectiles.RemoveAt(i);
                }
            }
        }


        private List<KeyValuePair<IObjetPhysique, IObjetPhysique>> collisionsCeTick = new List<KeyValuePair<IObjetPhysique, IObjetPhysique>>();

        private Dictionary<int, int> tmpObjetsTraites = new Dictionary<int, int>();
        //private Bag<int> tmpObjetsTraites = new Bag<int>(); // Ne fonctionne pas sur la Xbox 360

        private void doCollisionsCeTick()
        {
            collisionsCeTick.Clear();

            Cercle c = new Cercle(Vector3.Zero, 0);

            // Collisions Projectiles <--> Ennemis

            for (int i = Projectiles.Count - 1; i > -1; i--)
            {
                Projectile projectile = Projectiles[i];

                tmpObjetsTraites.Clear();

                IEnumerable<int> candidats;

                if (projectile is ProjectileLaserMultiple || projectile is ProjectileLaserSimple)
                    candidats = EnemiesGrid.GetItems(projectile.Ligne);
                else
                    candidats = EnemiesGrid.GetItems(projectile.Rectangle);

                int nbCollisions = 0;

                foreach (var indice in candidats)
                {
                    bool collision = false;

                    if (tmpObjetsTraites.ContainsKey(indice))
                        continue;
                    else
                        tmpObjetsTraites.Add(indice, 0);

                    Ennemi e = Ennemis[indice];

                    //Degeux
                    if (projectile.Forme == Forme.Rectangle && e.Forme == Forme.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionRectangleRectangle(projectile.Rectangle, e.Rectangle);
                    }

                    else if (projectile.Forme == Forme.Cercle && e.Forme == Forme.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionCercleRectangle(projectile.Cercle, e.Rectangle);
                    }

                    else if (projectile.Forme == Forme.Rectangle && e.Forme == Forme.Cercle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionCercleRectangle(e.Cercle, projectile.Rectangle);
                    }

                    else if (projectile.Forme == Forme.Ligne && e.Forme == Forme.Rectangle)
                    {
                        collision = EphemereGames.Core.Physique.Facade.collisionLigneRectangle(projectile.Ligne, e.Rectangle);
                    }

                    if (collision)
                    {
                        nbCollisions++;

                        collisionsCeTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(e, projectile));

                        if (!(projectile is ProjectileLaserMultiple || projectile is ProjectileSlowMotion))
                        {
                            projectile.DoHit(e);

                            if (!projectile.Alive)
                            {
                                projectile.DoDie();
                                Projectiles.RemoveAt(i);

                                if (projectile is ProjectileMissile)
                                {
                                    ProjectileMissile projectileMissile = (ProjectileMissile) projectile;

                                    c.Position = projectileMissile.Position;
                                    c.Radius = projectileMissile.ZoneImpact;

                                    tmpObjetsTraites.Clear();
                                    IEnumerable<int> candidats2 = EnemiesGrid.GetItems(c.Rectangle);

                                    foreach (var indice2 in candidats2)
                                    {
                                        if (tmpObjetsTraites.ContainsKey(indice2))
                                            continue;
                                        else
                                            tmpObjetsTraites.Add(indice2, 0);

                                        Ennemi ennemi = Ennemis[indice2];

                                        if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, ennemi.Cercle))
                                            collisionsCeTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(ennemi, projectile));
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }


            // Collisions Mineraux <--> Collecteur

            if (Collecteur != null)
            {
                for (int i = 0; i < Mineraux.Count; i++)
                {
                    Mineral mineral = Mineraux[i];

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(mineral.Cercle, Collecteur.Cercle))
                    {
                        collisionsCeTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(mineral, Collecteur));
                        collisionsCeTick.Add(new KeyValuePair<IObjetPhysique, IObjetPhysique>(Collecteur, mineral));
                    }
                }
            }
        }


        private Dictionary<int, Ennemi> ennemisCaches = new Dictionary<int, Ennemi>();

        private void doEnnemisCachesParCorpsCelestes()
        {
            ennemisCaches.Clear();

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                CorpsCeleste corpsCeleste = CorpsCelestes[i];

                if (corpsCeleste.EstDernierRelais)
                    continue;

                IEnumerable<int> candidats = EnemiesGrid.GetItems(corpsCeleste.Cercle.Rectangle);

                foreach (var indice in candidats)
                {
                    Ennemi e = Ennemis[indice];

                    if (!ennemisCaches.ContainsKey(e.GetHashCode()) &&
                        EphemereGames.Core.Physique.Facade.collisionCercleCercle(corpsCeleste.Cercle, e.Cercle))
                    {
                        ennemisCaches.Add(e.GetHashCode(), e);
                    }
                }
            }
        }


        private List<KeyValuePair<Turret, IObjetPhysique>> collisionsDansZoneActivationCeTick = new List<KeyValuePair<Turret, IObjetPhysique>>();
        private void doDansZoneActivationCeTick()
        {
            doEnnemisCachesParCorpsCelestes();

            collisionsDansZoneActivationCeTick.Clear();

            RectanglePhysique r = new RectanglePhysique();
            Cercle c = new Cercle(Vector3.Zero, 1);

            // Collisions Tourelle.ZoneActivation <--> Ennemis
            for (int i = 0; i < Tourelles.Count; i++)
            {
                Turret tourelle = Tourelles[i];

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
                    Ennemi e = Ennemis[indice];

                    if (ennemisCaches.ContainsKey(e.GetHashCode()))
                        continue;

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(c, e.Cercle))
                    {
                        collisionsDansZoneActivationCeTick.Add(new KeyValuePair<Turret, IObjetPhysique>(tourelle, e));
                        break; // passe à la prochaine tourelle
                    }
                }
            }
        }


        private List<KeyValuePair<Turret, Turret>> boostCollisionsThisTick = new List<KeyValuePair<Turret, Turret>>();
        private void doBoostCollisionsThisTick()
        {
            boostCollisionsThisTick.Clear();

            RectanglePhysique r = new RectanglePhysique();
            Cercle c = new Cercle(Vector3.Zero, 1);

            // Collisions BoosterTurret <--> Others Turrets
            for (int i = 0; i < Tourelles.Count; i++)
            {
                Turret turret = Tourelles[i];

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
                    Turret boostedTurret = Tourelles[index];

                    if (boostedTurret.Type == TurretType.Booster)
                        continue;

                    if (Core.Physique.Facade.collisionCercleCercle(turret.Cercle, c))
                        boostCollisionsThisTick.Add(new KeyValuePair<Turret, Turret>(turret, boostedTurret));
                }
            }
        }


        public void doObjetDetruit(IObjetPhysique objet)
        {
            CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            if (corpsCeleste != null)
            {
                tmpObjetsTraites.Clear();

                IEnumerable<int> candidats = EnemiesGrid.GetItems(corpsCeleste.ZoneImpactDestruction.Rectangle);

                foreach (var indice in candidats)
                {
                    if (tmpObjetsTraites.ContainsKey(indice))
                        continue;
                    else
                        tmpObjetsTraites.Add(indice, 0);

                    if (indice >= Ennemis.Count)
                        continue;

                    Ennemi ennemi = Ennemis[indice];

                    if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(corpsCeleste.ZoneImpactDestruction, ennemi.Cercle))
                        notifyObjetTouche(ennemi, corpsCeleste);
                }

                return;
            }


            SpaceshipCollector collecteur = objet as SpaceshipCollector;

            if (collecteur != null)
            {
                Collecteur = null;

                return;
            }
        }


        public void doObjetCree(IObjetPhysique objet)
        {
            SpaceshipCollector collecteur = objet as SpaceshipCollector;

            if (collecteur != null)
                Collecteur = collecteur;
        }
    }
}
