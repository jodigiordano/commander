namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class HiddenEnemies
    {
        public Dictionary<Enemy, CelestialBody> Output;

        public GridWorld EnemiesGrid;

        private CelestialBody CurrentCelestialBody;
        private IntegerHandler Handler;

        private Simulator Simulator;
        
        
        public HiddenEnemies(Simulator simulator)
        {
            Simulator = simulator;
            Output = new Dictionary<Enemy, CelestialBody>();
            Handler = new IntegerHandler(CheckEnemyIsHidden);
            CurrentCelestialBody = null;
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < Simulator.Data.Level.PlanetarySystem.Count; i++)
            {
                CurrentCelestialBody = Simulator.Data.Level.PlanetarySystem[i];

                EnemiesGrid.GetItems(CurrentCelestialBody.Circle.Rectangle, Handler);
            }
        }


        private bool CheckEnemyIsHidden(int index)
        {
            Enemy e = Simulator.Data.Enemies[index];

            if (!Output.ContainsKey(e) &&
                CurrentCelestialBody.VisualPriority < e.VisualPriority &&
                Physics.CircleCicleCollision(CurrentCelestialBody.Circle, e.Circle))
            {
                Output.Add(e, CurrentCelestialBody);
            }

            return true;
        }
    }
}
