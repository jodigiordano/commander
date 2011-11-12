namespace EphemereGames.Core.Utilities
{
    using System.Collections.Generic;


    public class FutureJobsController
    {
        private double CurrentTime;
        private List<KeyValuePair<NoneHandler, double>> Jobs;
        private float TargetElapsedTimeMs;


        public FutureJobsController(float targetElapsedTimeMs)
        {
            CurrentTime = 0;
            TargetElapsedTimeMs = targetElapsedTimeMs;
            Jobs = new List<KeyValuePair<NoneHandler, double>>();
        }


        public void Update()
        {
            CurrentTime += TargetElapsedTimeMs;

            for (int i = Jobs.Count - 1; i > -1; i--)
            {
                var job = Jobs[i];

                if (job.Value <= CurrentTime)
                {
                    job.Key();
                    Jobs.RemoveAt(i);
                }
            }
        }


        public void Add(NoneHandler job, double timeMs)
        {
            Jobs.Add(new KeyValuePair<NoneHandler, double>(job, CurrentTime + timeMs));
        }
    }
}
