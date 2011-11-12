namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;

    
    class InfiniteWave
    {
        protected int NbWavesAsked;
        protected InfiniteWavesDescriptor Descriptor;

        private Simulator Simulator;
        private int ActualDifficulty;


        public InfiniteWave(Simulator simulator, InfiniteWavesDescriptor descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;
            NbWavesAsked = 0;
            ActualDifficulty = Descriptor.StartingDifficulty - Descriptor.DifficultyIncrement;
        }


        public virtual Wave GetNextWave()
        {
            ActualDifficulty += Descriptor.DifficultyIncrement;

            var descriptor = WaveGenerator.Generate(
                ActualDifficulty,
                Main.Random.Next((int) Descriptor.MinMaxEnemiesPerWave.X, (int) Descriptor.MinMaxEnemiesPerWave.Y),
                Descriptor.Enemies);

            if (NbWavesAsked == 0 && Descriptor.FirstOneStartNow)
                descriptor.StartingTime = 0;
            else
                descriptor.StartingTime = Main.Random.Next(30000, 80000);

            NbWavesAsked++;

            var wave = new Wave(Simulator, descriptor);

            wave.Initialize();

            return wave;
        }


        public double GetAverageLife()
        {
            int averageDifficulty = Descriptor.StartingDifficulty + Descriptor.DifficultyIncrement / 2;

            double average = 0;

            foreach (var e in Descriptor.Enemies)
                average += Simulator.TweakingController.EnemiesFactory.GetLives(e, averageDifficulty);

            average = Descriptor.Enemies.Count == 0 ? average : average / Descriptor.Enemies.Count;

            return average;
        }


        public List<EnemyType> Enemies
        {
            get { return Descriptor.Enemies; }
        }


        public InfiniteWavesDescriptor GenerateDescriptor()
        {
            return Descriptor;
        }
    }
}
