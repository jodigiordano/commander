namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class Wave
    {
        public double StartingTime;
        public Vector3 StartingPosition;
        public List<Ennemi> Enemies;
        public Dictionary<EnemyType, EnemyDescriptor> Composition;
        public int NbEnemies;

        private double TempsDebut;
        private List<EnemyDescriptor> EnemiesToCreate;
        private Dictionary<int, Ennemi> EnemiesCreated;
        private Simulation Simulation;


        public Wave(Simulation simulation, WaveDescriptor descriptor)
        {
            Simulation = simulation;

            EnemiesToCreate = new List<EnemyDescriptor>();
            EnemiesCreated = new Dictionary<int, Ennemi>();
            Enemies = new List<Ennemi>();
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

            NbEnemies = EnemiesToCreate.Count;
        }


        public void Update(GameTime gameTime)
        {
            TempsDebut += gameTime.ElapsedGameTime.TotalMilliseconds;

            Enemies.Clear();

            if (EnemiesToCreate.Count == 0)
                return;

            int i = EnemiesToCreate.Count - 1;

            while (i >= 0 && EnemiesToCreate[i].StartingTime <= TempsDebut)
            {
                var desc = EnemiesToCreate[i];

                Ennemi e = FactoryEnnemis.Instance.creerEnnemi
                (
                    Simulation,
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


        public void doEnemyDestroyed(Ennemi ennemi)
        {
            EnemiesCreated.Remove(ennemi.GetHashCode());
        }
    }
}
