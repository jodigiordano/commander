namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class UpfrontInfiniteWave : InfiniteWave
    {
        private List<Wave> Waves;
        private int MaxWaves;


        public UpfrontInfiniteWave(Simulator simulator, InfiniteWavesDescriptor descriptor, int nbWaves)
            : base(simulator, descriptor)
        {
            Waves = new List<Wave>();
            MaxWaves = nbWaves;

            var difficulty = Descriptor.StartingDifficulty;

            for (int i = 0; i < nbWaves; i++)
            {
                var waveDesc = WaveGenerator.Generate(
                    difficulty,
                    Main.Random.Next((int) Descriptor.MinMaxEnemiesPerWave.X, (int) Descriptor.MinMaxEnemiesPerWave.Y),
                    Descriptor.Enemies);

                if (i == 0 && Descriptor.FirstOneStartNow)
                    waveDesc.StartingTime = 0;

                Waves.Add(new Wave(simulator, waveDesc));

                difficulty += Descriptor.DifficultyIncrement;
            }
        }


        public override Wave GetNextWave()
        {
            if (NbWavesAsked >= MaxWaves)
                NbWavesAsked = 0;

            return Waves[NbWavesAsked++];
        }
    }
}
