namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class OutOfBounds
    {
        public List<Bullet> Output;

        private Simulator Simulator;


        public OutOfBounds(Simulator simulator)
        {
            Simulator = simulator;
            Output = new List<Bullet>();
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = Simulator.Data.Bullets.Count - 1; i > -1; i--)
            {
                Bullet bullet = Simulator.Data.Bullets[i];

                if (bullet.Shape == Shape.Line || bullet.Shape == Shape.Circle)
                    continue;

                if (!Physics.RectangleRectangleCollision(bullet.Rectangle, Simulator.Data.Battlefield))
                    Output.Add(bullet);
            }
        }
    }
}
