namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class TurretsController
    {
        public List<Turret> StartingTurrets;
        public PlanetarySystemController PlanetarySystemController;

        public List<Turret> Turrets;

        public event TurretHandler TurretBought;
        public event TurretHandler TurretSold;
        public event TurretHandler TurretUpdated;
        public event TurretHandler TurretReactivated;
        public event PhysicalObjectHandler ObjectCreated;

        private Simulator Simulator;
        private Dictionary<Turret, Enemy> AssociationsThisTick;
        private Dictionary<Turret, int> BoostedTurretsThisTick;


        public TurretsController(Simulator simulator)
        {
            Simulator = simulator;
            Turrets = new List<Turret>();
            AssociationsThisTick = new Dictionary<Turret, Enemy>();
            BoostedTurretsThisTick = new Dictionary<Turret, int>();
        }


        public void Initialize()
        {
            Turrets.Clear();
            AssociationsThisTick.Clear();
            BoostedTurretsThisTick.Clear();

            for (int i = 0; i < StartingTurrets.Count; i++)
                Turrets.Add(StartingTurrets[i]);

            if (Simulator.DemoMode)
                foreach (var turret in Turrets)
                    turret.ShowForm = false;
        }


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Turrets.Count; i++)
            {
                Turret t = Turrets[i];

                if (t.UpdatePosition)
                    t.Position = t.CelestialBody.Position + t.RelativePosition;

                t.Update();

                if (t.BackActiveThisTick)
                    NotifyTurretReactivated(t);

                if (t.PlayerControlled)
                    continue;

                Enemy enemyAttacked;

                if (t.TimeLastBullet <= 0 && AssociationsThisTick.TryGetValue(t, out enemyAttacked))
                {
                    t.EnemyWatched = enemyAttacked;
                    t.Watcher = false;
                }
                else if (t.EnemyWatched != null && !t.EnemyWatched.Alive)
                {
                    t.EnemyWatched = null;
                    t.Watcher = true;
                }

                else
                    t.Watcher = true;
            }

            for (int i = 0; i < Turrets.Count; i++)
            {
                List<Bullet> projectiles = Turrets[i].BulletsThisTick();

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

            BoostTurrets();
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
            //turret.Show();

            PlanetarySystemController.AddTurret(turret);
            Turrets.Add(turret);
            NotifyTurretBought(turret);
        }


        public void DoSellTurret(Turret turret)
        {
            PlanetarySystemController.RemoveTurret(turret);

            turret.DoDie();
            //turret.Hide();

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
            Enemy enemy = obj as Enemy;

            if (enemy == null)
                return;

            if (!AssociationsThisTick.ContainsKey(turret))
                AssociationsThisTick.Add(turret, null);

            AssociationsThisTick[turret] = enemy;
        }


        public void DoTurretBoosted(Turret boosting, Turret boosted)
        {
            int value = -1;
            bool contained = BoostedTurretsThisTick.TryGetValue(boosted, out value);

            if (!contained)
                BoostedTurretsThisTick.Add(boosted, boosting.Level);
            else if (value < boosting.Level)
                BoostedTurretsThisTick[boosted] = boosted.Level;
        }


        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            CelestialBody corpsCeleste = obj as CelestialBody;

            if (corpsCeleste != null)
            {

                foreach (var turret in corpsCeleste.Turrets)
                    Turrets.Remove(turret);

                return;
            }

            LaserBullet bullet = obj as LaserBullet;

            if (bullet != null)
            {
                bullet.Turret.EnemyWatched = null;
                bullet.Turret.Watcher = true;
            }
        }


        private void BoostTurrets()
        {
            foreach (var turret in Turrets)
                turret.BoostMultiplier = 0;

            foreach (var kvp in BoostedTurretsThisTick)
                kvp.Key.BoostMultiplier = kvp.Value;

            BoostedTurretsThisTick.Clear();
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
