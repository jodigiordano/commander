﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class OutOfBounds
    {
        public List<Bullet> Output;

        public List<Bullet> Bullets;
        public PhysicalRectangle Battlefield;


        public OutOfBounds()
        {
            Output = new List<Bullet>();
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = Bullets.Count - 1; i > -1; i--)
            {
                Bullet bullet = Bullets[i];

                if (bullet.Shape == Shape.Line || bullet.Shape == Shape.Circle)
                    continue;

                if (!Physics.RectangleRectangleCollision(bullet.Rectangle, this.Battlefield))
                    Output.Add(bullet);
            }
        }
    }
}
