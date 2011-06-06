namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class Wave
    {
        public double StartingTime;
        public Vector3 StartingPosition;
        public List<Enemy> Enemies;
        public Dictionary<EnemyType, EnemyDescriptor> Composition;
        public int EnemiesCount;

        private double TempsDebut;
        private List<EnemyDescriptor> EnemiesToCreate;
        private Dictionary<int, Enemy> EnemiesCreated;
        private Simulation Simulation;


        public Wave(Simulation simulation, WaveDescriptor descriptor)
        {
            Simulation = simulation;

            EnemiesToCreate = new List<EnemyDescriptor>();
            EnemiesCreated = new Dictionary<int, Enemy>();
            Enemies = new List<Enemy>();
            Composition = new Dictionary<EnemyType, EnemyDescriptor>();

            EnemiesToCreate = descriptor.GetEnemiesToCreate();

            EnemiesToCreate.Sort(delegate(EnemyDescriptor e1, EnemyDescriptor e2)
            {
                return e2.StartingTime.CompareTo(e1.StartingTime);
            });

            StartingTime = descriptor.StartingTime;

            foreach (var enemy in EnemiesToCreate)
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

            EnemiesCount = EnemiesToCreate.Count;
        }


        public void Update()
        {
            TempsDebut += 16.66;

            Enemies.Clear();

            if (EnemiesToCreate.Count == 0)
                return;

            int i = EnemiesToCreate.Count - 1;

            while (i >= 0 && EnemiesToCreate[i].StartingTime <= TempsDebut)
            {
                var desc = EnemiesToCreate[i];

                Enemy e = Simulation.EnemiesFactory.CreateEnemy
                (
                    desc.Type,
                    desc.SpeedLevel,
                    desc.LivesLevel,
                    desc.CashValue
                );

                e.Position = StartingPosition;

                EnemiesToCreate.RemoveAt(i);
                Enemies.Add(e);
                EnemiesCreated.Add(e.GetHashCode(), e);

                i--;
            }
        }


        public bool IsFinished
        {
            get
            {
                return EnemiesToCreate.Count == 0 && EnemiesCreated.Count == 0;
            }
        }


        public void DoEnemyDestroyed(Enemy enemy)
        {
            EnemiesCreated.Remove(enemy.GetHashCode());
        }
    }
}
