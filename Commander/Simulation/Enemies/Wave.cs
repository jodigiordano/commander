namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class Wave
    {
        public double TimeElapsed;
        public double StartingTime;

        public List<Enemy> NewEnemies;
        public int EnemiesCount;

        private List<EnemyDescriptor> EnemiesToCreate;
        private Simulator Simulator;
        private int EnemiesAlive;

        public WaveDescriptor Descriptor;


        public Wave(Simulator simulator, WaveDescriptor descriptor)
        {
            Simulator = simulator;
            Descriptor = descriptor;

            NewEnemies = new List<Enemy>();
        }


        public void Initialize()
        {
            NewEnemies.Clear();

            EnemiesToCreate = Descriptor.GetEnemiesToCreate(Simulator);

            EnemiesToCreate.Sort(delegate(EnemyDescriptor e1, EnemyDescriptor e2)
            {
                return e2.StartingTime.CompareTo(e1.StartingTime);
            });

            StartingTime = Descriptor.StartingTime;

            EnemiesCount = EnemiesToCreate.Count;
            EnemiesAlive = 0;
        }


        public void Update()
        {
            TimeElapsed += Preferences.TargetElapsedTimeMs;

            NewEnemies.Clear();

            if (EnemiesToCreate.Count == 0)
                return;

            int i = EnemiesToCreate.Count - 1;

            while (i >= 0 && EnemiesToCreate[i].StartingTime <= TimeElapsed)
            {
                var desc = EnemiesToCreate[i];

                Enemy e = Simulator.EnemiesFactory.Get
                (
                    desc.Type,
                    desc.SpeedLevel,
                    desc.LivesLevel,
                    desc.CashValue
                );

                e.Displacement = desc.StartingPosition;

                EnemyDescriptor.Pool.Return(desc);
                EnemiesToCreate.RemoveAt(i);
                NewEnemies.Add(e);
                EnemiesAlive++;

                i--;
            }
        }


        public int EnemiesToCreateCount
        {
            get { return EnemiesToCreate.Count; }
        }


        public bool IsFinished
        {
            get
            {
                return EnemiesToCreate.Count == 0 && EnemiesAlive == 0;
            }
        }


        public void DoEnemyDestroyed()
        {
            EnemiesAlive--;
        }


        public void AddEnemy(EnemyDescriptor e)
        {
            e.StartingTime = TimeElapsed;

            EnemiesToCreate.Add(e);
            EnemiesCount++;
        }
    }
}
