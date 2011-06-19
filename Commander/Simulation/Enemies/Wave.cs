namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class Wave
    {
        public double StartingTime;
        public List<Enemy> NewEnemies;
        public Dictionary<EnemyType, EnemyDescriptor> Composition;
        public int EnemiesCount;

        private double TempsDebut;
        private List<EnemyDescriptor> EnemiesToCreate;
        private Simulator Simulator;
        private int EnemiesAlive;


        public Wave(Simulator simulator, WaveDescriptor descriptor)
        {
            Simulator = simulator;

            EnemiesToCreate = new List<EnemyDescriptor>();
            NewEnemies = new List<Enemy>();
            Composition = new Dictionary<EnemyType, EnemyDescriptor>();

            EnemiesToCreate = descriptor.GetEnemiesToCreate();

            EnemiesToCreate.Sort(delegate(EnemyDescriptor e1, EnemyDescriptor e2)
            {
                return e2.StartingTime.CompareTo(e1.StartingTime);
            });

            StartingTime = descriptor.StartingTime;

            foreach (var enemy in EnemiesToCreate)
            {
                if (Composition.ContainsKey(enemy.Type))
                    Composition[enemy.Type].CashValue++;
                else
                {
                    EnemyDescriptor desc = new EnemyDescriptor();
                    desc.Type = enemy.Type;
                    desc.CashValue = 1;
                    desc.LivesLevel = enemy.LivesLevel;
                    desc.SpeedLevel = enemy.SpeedLevel;

                    Composition.Add(enemy.Type, desc);
                }
            }

            EnemiesCount = EnemiesToCreate.Count;
            EnemiesAlive = 0;
        }


        public void Update()
        {
            TempsDebut += Preferences.TargetElapsedTimeMs;

            NewEnemies.Clear();

            if (EnemiesToCreate.Count == 0)
                return;

            int i = EnemiesToCreate.Count - 1;

            while (i >= 0 && EnemiesToCreate[i].StartingTime <= TempsDebut)
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
            e.StartingTime = TempsDebut;

            EnemiesToCreate.Add(e);
            EnemiesCount++;
        }
    }
}
