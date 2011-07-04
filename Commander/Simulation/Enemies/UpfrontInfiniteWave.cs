namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class UpfrontInfiniteWave : InfiniteWave
    {
        private List<Wave> Waves;
        private int MaxWaves;


        public UpfrontInfiniteWave(Simulator simulator, DescriptorInfiniteWaves descriptor, int nbWaves)
            : base(simulator, descriptor)
        {
            Waves = new List<Wave>();
            MaxWaves = nbWaves;

            Generator.WavesCount = nbWaves;
            Generator.DifficultyStart = Descriptor.StartingDifficulty;
            Generator.DifficultyEnd = Descriptor.StartingDifficulty + Descriptor.DifficultyIncrement * nbWaves;

            Generator.Generate();

            if (NbWavesAsked == 0 && Descriptor.FirstOneStartNow)
                Generator.Waves[0].StartingTime = 0;

            foreach (var wave in Generator.Waves)
                Waves.Add(new Wave(simulator, wave));
        }


        public override Wave GetNextWave()
        {
            if (NbWavesAsked >= MaxWaves)
                NbWavesAsked = 0;

            return Waves[NbWavesAsked++];
        }
    }
}
