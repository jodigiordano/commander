namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EnemiesData
    {
        public double EnemyNearHitPerc;
        public bool EnemyNearHitPercChanged;

        public double EnemyCountPerc;
        public bool EnemyCountPercChanged;

        public List<Enemy> Enemies;
        public Path Path;
        public int MaxEnemiesForCountPerc;

        private double MaxEnemyNearHitPercDelta;


        public EnemiesData()
        {
            EnemyNearHitPerc = 0;
            EnemyCountPerc = 0;

            EnemyNearHitPercChanged = true;
            EnemyCountPercChanged = true;

            MaxEnemiesForCountPerc = int.MaxValue;
            MaxEnemyNearHitPercDelta = 0.01;
        }


        public void Update()
        {
            if (Path == null)
                return;

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

            double newEnemyNearHitPerc = Core.Physics.Utilities.LerpMax(EnemyNearHitPerc, currentEnemyNearHitPerc, MaxEnemyNearHitPercDelta);

            EnemyNearHitPercChanged = EnemyNearHitPerc != newEnemyNearHitPerc;

            EnemyNearHitPerc = newEnemyNearHitPerc;
        }


        private void ComputeEnemyCountPerc()
        {
            float newEnemyCountPerc = (float) Enemies.Count / MaxEnemiesForCountPerc;

            newEnemyCountPerc = MathHelper.Clamp(newEnemyCountPerc, 0, 1);

            EnemyCountPercChanged = EnemyCountPerc - newEnemyCountPerc != 0;

            var delta = MathHelper.Clamp(newEnemyCountPerc - (float) EnemyCountPerc, -0.02f, 0.02f);

            EnemyCountPerc = MathHelper.Clamp((float) EnemyCountPerc + delta, 0, 1);
        }
    }
}
