namespace EphemereGames.Commander.Simulation
{
    class InfiniteWave
    {
        protected WaveGenerator Generator;
        protected int NbWavesAsked;
        protected DescriptorInfiniteWaves Descriptor;

        private Simulator Simulator;
        private int ActualDifficulty;


        public InfiniteWave(Simulator simulator, DescriptorInfiniteWaves descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;
            NbWavesAsked = 0;
            ActualDifficulty = Descriptor.StartingDifficulty - Descriptor.DifficultyIncrement;

            Generator = new WaveGenerator();
            Generator.WavesCount = 1;
            Generator.Enemies = Descriptor.Enemies;
            Generator.MineralsPerWave = Descriptor.MineralsPerWave;
            Generator.QtyEnemies = Main.Random.Next((int) Descriptor.MinMaxEnemiesPerWave.X, (int) Descriptor.MinMaxEnemiesPerWave.Y);
        }


        public virtual Wave GetNextWave()
        {
            ActualDifficulty += Descriptor.DifficultyIncrement;

            Generator.DifficultyStart = ActualDifficulty;
            Generator.DifficultyEnd = ActualDifficulty;

            Generator.Generate();

            if (NbWavesAsked == 0 && Descriptor.FirstOneStartNow)
                Generator.Waves[0].StartingTime = 0;

            NbWavesAsked++;

            var wave = new Wave(Simulator, Generator.Waves[0]);

            wave.Initialize();

            return wave;
        }


        public double GetAverageLife()
        {
            int averageDifficulty = (Generator.DifficultyEnd + Generator.DifficultyStart) / 2;

            double average = 0;

            foreach (var e in Generator.Enemies)
                average += Simulator.TweakingController.EnemiesFactory.GetLives(e, averageDifficulty);

            average = Generator.Enemies.Count == 0 ? average : average / Generator.Enemies.Count;

            return average;
        }
    }
}
