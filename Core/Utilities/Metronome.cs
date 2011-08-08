using System.Collections.Generic;
namespace EphemereGames.Core.Utilities
{
    public class Metronome
    {
        public Path Path; 

        private double TargetElapsedTimeMs;
        private double TimeElapsedLastCycleMs;
        private int nbCycles;

        public double FrequencyMs { get; set; }


        public Metronome(double targetElapsedTimeMs, double frequencyMs)
        {
            TargetElapsedTimeMs = targetElapsedTimeMs;
            FrequencyMs = frequencyMs;

            Path = new Path();

            Initialize();
        }


        public void Initialize()
        {
            TimeElapsedLastCycleMs = 0;
        }


        public double FrequencyHz
        {
            get
            {
                return FrequencyMs / 1000;
            }

            set
            {
                FrequencyMs = 1000 / value;
            }
        }


        public int CyclesCountThisTick
        {
            get
            {
                return nbCycles;
            }
        }


        public float CurvePercThisTick
        {
            get
            {
                return Path.GetPosition((float) (TimeElapsedLastCycleMs / FrequencyMs));
            }
        }


        public void Update()
        {
            TimeElapsedLastCycleMs += TargetElapsedTimeMs;

            nbCycles = (int) (TimeElapsedLastCycleMs / FrequencyMs);

            if (nbCycles > 0)
                TimeElapsedLastCycleMs %= nbCycles;
        }


        public static Metronome Create(CurveType type, double targetElapsedTimeMs, double frequencyMs)
        {
            Metronome metronome = new Metronome(targetElapsedTimeMs, frequencyMs);

            switch (type)
            {
                case CurveType.Linear:
                    metronome.Path.Initialize(new List<float>() { 0, 1 }, new List<float> { 0, 1 });
                    break;

                case CurveType.Exponential:
                    metronome.Path.Initialize(new List<float>() { 0, 0.1f, 1 }, new List<float> { 0, 0.8f, 1 });
                    break;

                case CurveType.Log:
                    metronome.Path.Initialize(new List<float>() { 0, 0.8f, 1 }, new List<float> { 0, 0.1f, 1 });
                    break;

                case CurveType.Sine:
                    metronome.Path.Initialize(new List<float>() { 0, 1f, 0 }, new List<float> { 0, 0.5f, 1 });
                    break;
            }

            return metronome;
        }
    }
}
