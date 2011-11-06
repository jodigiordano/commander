﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class BulletsDeflected
    {
        public List<KeyValuePair<Enemy, Bullet>> Output;

        public GridWorld EnemiesGrid;

        private IntegerHandler Handler;
        private Bullet CurrentBullet;
        private PhysicalRectangle CurrentBulletRectangle;
        private Circle CurrentBulletDeflectRange;

        private Simulator Simulator;
        

        public BulletsDeflected(Simulator simulator)
        {
            Simulator = simulator;

            Output = new List<KeyValuePair<Enemy, Bullet>>();
            Handler = new IntegerHandler(CheckBulletIsDeflected);
            CurrentBullet = null;
            CurrentBulletRectangle = new PhysicalRectangle();
            CurrentBulletDeflectRange = new Circle(Vector3.Zero, 0);
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Simulator.Data.Bullets.Count; i++)
            {
                CurrentBullet = Simulator.Data.Bullets[i];

                if (!CurrentBullet.Deflectable)
                    continue;

                SyncRectangleAndCircle();

                EnemiesGrid.GetItems(CurrentBulletRectangle, Handler);
            }
        }


        private bool CheckBulletIsDeflected(int index)
        {
            Enemy e = Simulator.Data.Enemies[index];

            if (e.Type != EnemyType.Vulcanoid)
                return true;

            if (Physics.CircleCicleCollision(CurrentBulletDeflectRange, e.Circle))
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
