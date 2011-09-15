namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EnemiesData
    {
        public double EnemyNearHitPerc;
        public double EnemyCountPerc;

        public List<Enemy> Enemies;
        public Path Path;
        public int MaxEnemiesForCountPerc;

        private double MaxEnemyNearHitPercDelta;


        public EnemiesData()
        {
            EnemyNearHitPerc = 0;
            EnemyCountPerc = 0;

            MaxEnemiesForCountPerc = int.MaxValue;
            MaxEnemyNearHitPercDelta = 0.01;
        }


        public void Update()
        {
            ComputeNearestEnemyPerc();
            ComputeEnemyCountPerc();
        }


        private void ComputeNearestEnemyPerc()
        {
            double currentEnemyNearHitPerc = 0;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy e = Enemies[i];

                double displacementPerc = Path.GetPercentage(e.Displacement);

                if (displacementPerc > currentEnemyNearHitPerc)
                    currentEnemyNearHitPerc = displacementPerc;
            }

            EnemyNearHitPerc = Core.Physics.Utilities.LerpMax(EnemyNearHitPerc, currentEnemyNearHitPerc, MaxEnemyNearHitPercDelta);
        }


        private void ComputeEnemyCountPerc()
        {
            EnemyCountPerc = (float) Enemies.Count / MaxEnemiesForCountPerc;
        }
    }
}
