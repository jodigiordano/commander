namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class EnemiesData
    {
        public double EnemyNearHitPerc;
        public bool EnemyNearHitPercChanged;

        public double EnemyCountPerc;
        public bool EnemyCountPercChanged;

        public int MaxEnemiesForCountPerc;

        private double MaxEnemyNearHitPercDelta;
        private Simulator Simulator;

        public EnemiesData(Simulator simulator)
        {
            Simulator = simulator;

            EnemyNearHitPerc = 0;
            EnemyCountPerc = 0;

            EnemyNearHitPercChanged = true;
            EnemyCountPercChanged = true;

            MaxEnemiesForCountPerc = int.MaxValue;
            MaxEnemyNearHitPercDelta = 0.01;
        }


        public void Update()
        {
            if (Simulator.Data.Path == null)
                return;

            ComputeNearestEnemyPerc();
            ComputeEnemyCountPerc();
        }


        private void ComputeNearestEnemyPerc()
        {
            double currentEnemyNearHitPerc = 0;

            for (int i = 0; i < Simulator.Data.Enemies.Count; i++)
            {
                Enemy e = Simulator.Data.Enemies[i];

                double displacementPerc = Simulator.Data.Path.GetPercentage(e.Displacement);

                if (displacementPerc > currentEnemyNearHitPerc)
                    currentEnemyNearHitPerc = displacementPerc;
            }

            double newEnemyNearHitPerc = Core.Physics.Utilities.LerpMax(EnemyNearHitPerc, currentEnemyNearHitPerc, MaxEnemyNearHitPercDelta);

            EnemyNearHitPercChanged = EnemyNearHitPerc != newEnemyNearHitPerc;

            EnemyNearHitPerc = newEnemyNearHitPerc;
        }


        private void ComputeEnemyCountPerc()
        {
            float newEnemyCountPerc = (float) Simulator.Data.Enemies.Count / MaxEnemiesForCountPerc;

            newEnemyCountPerc = MathHelper.Clamp(newEnemyCountPerc, 0, 1);

            EnemyCountPercChanged = EnemyCountPerc - newEnemyCountPerc != 0;

            var delta = MathHelper.Clamp(newEnemyCountPerc - (float) EnemyCountPerc, -0.01f, 0.01f);

            EnemyCountPerc = MathHelper.Clamp((float) EnemyCountPerc + delta, 0, 1);
        }
    }
}
