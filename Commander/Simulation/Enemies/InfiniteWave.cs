namespace EphemereGames.Commander.Simulation
{
    class InfiniteWave
    {
        protected WaveGenerator Generator;
        protected int NbWavesAsked;
        protected DescriptorInfiniteWaves Descriptor;

        private Simulator Simulation;
        private int ActualDifficulty;


        public InfiniteWave(Simulator simulation, DescriptorInfiniteWaves descriptor)
        {
            Simulation = simulation;
            Descriptor = descriptor;
            NbWavesAsked = 0;
            ActualDifficulty = Descriptor.StartingDifficulty - Descriptor.DifficultyIncrement;

            Generator = new WaveGenerator();
            Generator.NbWaves = 1;
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

            return new Wave(Simulation, Generator.Waves[0]);
        }
    }
}
