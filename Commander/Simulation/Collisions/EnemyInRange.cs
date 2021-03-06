﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class EnemyInRange
    {
        public List<KeyValuePair<Turret, Enemy>> Output;

        public GridWorld EnemiesGrid;
        public List<Turret> Turrets;
        public List<Enemy> Enemies;
        public HiddenEnemies HiddenEnemies;

        private IntegerHandler Handler;
        private Turret CurrentTurret;
        private PhysicalRectangle CurrentTurretRectangle;
        private Circle CurrentTurretRange;
        private Enemy NearestEnemy;


        public EnemyInRange()
        {
            Output = new List<KeyValuePair<Turret, Enemy>>();
            Handler = new IntegerHandler(CheckEnemyIsInRange);
            CurrentTurret = null;
            CurrentTurretRectangle = new PhysicalRectangle();
            CurrentTurretRange = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Turrets.Count; i++)
            {
                NearestEnemy = null;

                CurrentTurret = Turrets[i];

                if (CurrentTurret.Type == TurretType.Gravitational || CurrentTurret.Type == TurretType.Booster)
                    continue;

                SyncRectangleAndCircle();

                EnemiesGrid.GetItems(CurrentTurretRectangle, Handler);

                if (NearestEnemy != null)
                    Output.Add(new KeyValuePair<Turret, Enemy>(CurrentTurret, NearestEnemy));
            }
        }


        private bool CheckEnemyIsInRange(int index)
        {
            Enemy e = Enemies[index];

            if (HiddenEnemies.Output.ContainsKey(e))
                return true;

            if (NearestEnemy != null && NearestEnemy.Displacement > e.Displacement)
                return true;

            if (Physics.CircleCicleCollision(CurrentTurretRange, e.Circle))
                NearestEnemy = e;

            return true;
        }


        private void SyncRectangleAndCircle()
        {
            float range = CurrentTurret.Range;
            CurrentTurretRectangle.X = (int) (CurrentTurret.Position.X - range);
            CurrentTurretRectangle.Y = (int) (CurrentTurret.Position.Y - range);
            CurrentTurretRectangle.Width = (int) (range * 2);
            CurrentTurretRectangle.Height = (int) (range * 2);

            CurrentTurretRange.Position = CurrentTurret.Position;
            CurrentTurretRange.Radius = range;
        }
    }
}
