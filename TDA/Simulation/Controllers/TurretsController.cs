namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;


    class TurretsController
    {
        public event TurretHandler TurretBought;
        public event TurretHandler TurretSold;
        public event TurretHandler TurretUpdated;
        public event TurretHandler TurretReactivated;
        public event PhysicalObjectHandler ObjectCreated;
        
        public List<Turret> StartingTurrets;
        public PlanetarySystemController PlanetarySystemController;
        public List<Turret> Turrets { get; private set; }

        private Simulation Simulation;
        private Dictionary<Turret, Ennemi> AssociationsThisTick;
        private bool demoMode;


        public TurretsController(Simulation simulation)
        {
            Simulation = simulation;
            Turrets = new List<Turret>();
            AssociationsThisTick = new Dictionary<Turret, Ennemi>();
            DemoMode = false;
        }


        public void Initialize()
        {
            for (int i = 0; i < StartingTurrets.Count; i++)
                Turrets.Add(StartingTurrets[i]);
        }


        public bool DemoMode
        {
            get { return demoMode; }
            set
            {
                demoMode = value;

                foreach (var turret in Turrets)
                    turret.ShowForm = false;
            }
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Turrets.Count; i++)
            {
                Turret t = Turrets[i];
                t.Position = t.CelestialBody.Position + t.RelativePosition;

                t.Update(gameTime);

                if (t.BackActiveThisTick)
                    NotifyTurretReactivated(t);

                Ennemi enemyAttacked;

                if (t.TimeLastBullet <= 0 && AssociationsThisTick.TryGetValue(t, out enemyAttacked))
                {
                    t.EnemyWatched = enemyAttacked;
                    t.Watcher = false;
                }
                else if (t.EnemyWatched != null && !t.EnemyWatched.EstVivant)
                {
                    t.EnemyWatched = null;
                    t.Watcher = true;
                }

                else
                    t.Watcher = true;
            }

            for (int i = 0; i < Turrets.Count; i++)
            {
                List<Projectile> projectiles = Turrets[i].BulletsThisTick(gameTime);

                for (int j = 0; j < projectiles.Count; j++)
                    NotifyObjectCreated(projectiles[j]);
            }

            // Pour l'instant, à chaque tick, les tourelles sont dissociées de leurs ennemis
            // et le système de collision fait en sorte que les ennemis sont réassocié de nouveau
            // Une tourelle est associé avec le premier ennemi qui entre en collision avec sa zone d'activation
            // Une meilleure implémentation serait un événement EntreDansZoneActivation et SortDeZoneActivation
            // lancé par le ControleurCollisions et capturé par ce controleur, qui ensuite associerait/dissocierait
            // l'ennemi d'une tourelle. Résultat: moins d'événements lancés à chaque tick.
            AssociationsThisTick.Clear();
        }


        public void Draw()
        {
            for (int i = 0; i < Turrets.Count; i++)
                if (Turrets[i].Visible)
                    Turrets[i].Draw();
        }


        public void DoBuyTurret(Turret turret)
        {
            turret.RelativePosition = turret.Position - turret.CelestialBody.position;

            PlanetarySystemController.AddTurret(turret);
            Turrets.Add(turret);
            NotifyTurretBought(turret);
        }


        public void DoSellTurret(Turret turret)
        {
            PlanetarySystemController.RemoveTurret(turret);

            turret.DoDie();
            Turrets.Remove(turret);
            NotifyTurretSold(turret);
        }


        public void DoUpgradeTurret(Turret turret)
        {
            if (turret.Upgrade())
                NotifyTurretUpgraded(turret);
        }


        public void DoInRangeTurret(Turret turret, IObjetPhysique obj)
        {
            Ennemi enemy = obj as Ennemi;

            if (enemy == null)
                return;

            if (!AssociationsThisTick.ContainsKey(turret))
                AssociationsThisTick.Add(turret, null);

            AssociationsThisTick[turret] = enemy;
        }


        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            CorpsCeleste corpsCeleste = obj as CorpsCeleste;

            if (corpsCeleste == null)
                return;

            foreach (var turret in corpsCeleste.Turrets)
                    Turrets.Remove(turret);
        }


        private void NotifyTurretBought(Turret turret)
        {
            if (TurretBought != null)
                TurretBought(turret);
        }


        private void NotifyTurretSold(Turret turret)
        {
            if (TurretSold != null)
                TurretSold(turret);
        }


        private void NotifyTurretUpgraded(Turret turret)
        {
            if (TurretUpdated != null)
                TurretUpdated(turret);
        }


        private void NotifyObjectCreated(IObjetPhysique obj)
        {
            if (ObjectCreated != null)
                ObjectCreated(obj);
        }


        private void NotifyTurretReactivated(Turret turret)
        {
            if (TurretReactivated != null)
                TurretReactivated(turret);
        }
    }
}
