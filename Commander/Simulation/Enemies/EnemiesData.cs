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


        public EnemiesData()
        {
            EnemyNearHitPerc = 0;
            EnemyCountPerc = 0;

            MaxEnemiesForCountPerc = int.MaxValue;
        }


        public void Update()
        {
            ComputeNearestEnemyPerc();
            ComputeEnemyCountPerc();
        }


        private void ComputeNearestEnemyPerc()
        {
            EnemyNearHitPerc = 0;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy e = Enemies[i];

                double displacementPerc = Path.GetPercentage(e.Displacement);

                if (displacementPerc > EnemyNearHitPerc)
                    EnemyNearHitPerc = displacementPerc;
            }
        }


        private void ComputeEnemyCountPerc()
        {
            EnemyCountPerc = (float) Enemies.Count / MaxEnemiesForCountPerc;
        }
    }
}
