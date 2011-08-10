using System.Collections.Generic;
namespace EphemereGames.Commander
{
    class FutureJobsController
    {
        private double CurrentTime;
        private List<KeyValuePair<NoneHandler, double>> Jobs;


        public FutureJobsController()
        {
            CurrentTime = 0;
            Jobs = new List<KeyValuePair<NoneHandler, double>>();
        }


        public void Update()
        {
            CurrentTime += Preferences.TargetElapsedTimeMs;

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
