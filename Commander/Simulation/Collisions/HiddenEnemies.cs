namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class HiddenEnemies
    {
        public Dictionary<Enemy, CelestialBody> Output;

        public List<CelestialBody> CelestialBodies;
        public GridWorld EnemiesGrid;
        public List<Enemy> Enemies;

        private CelestialBody CurrentCelestialBody;
        private GridWorld.IntegerHandler Handler;
        
        
        public HiddenEnemies()
        {
            Output = new Dictionary<Enemy, CelestialBody>();
            Handler = new GridWorld.IntegerHandler(CheckEnemyIsHidden);
            CurrentCelestialBody = null;
        }


        public void Sync()
        {
            Output.Clear();

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                CurrentCelestialBody = CelestialBodies[i];

                EnemiesGrid.GetItems(CurrentCelestialBody.Circle.Rectangle, Handler);
            }
        }


        private bool CheckEnemyIsHidden(int index)
        {
            Enemy e = Enemies[index];

            if (!Output.ContainsKey(e) &&
                Physics.CircleCicleCollision(CurrentCelestialBody.Circle, e.Circle))
            {
                Output.Add(e, CurrentCelestialBody);
            }

            return true;
        }
    }
}
