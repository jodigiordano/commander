namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class EnemiesController
    {
        public List<Enemy> Enemies                                         { get; private set; }
        public LinkedList<Wave> Waves                                      { get; set; }
        public Dictionary<EnemyType, EnemyDescriptor> NextWaveData         { get; set; }
        public VaguesInfinies InfiniteWaves                                { get; set; }
        public Path Path                                                   { get; set; }
        public Path PathPreview                                            { get; set; }
        public Vector3 MineralsPercentages;
        public int MineralsValue;
        public int LifePacksGiven;
        public List<Mineral> Minerals;
        public event NoneHandler WaveEnded;
        public event NoneHandler WaveStarted;
        public event PhysicalObjectHandler ObjectDestroyed;
        public event PhysicalObjectHandler ObjectCreated;
        public event EnemyCelestialBodyHandler EnemyReachedEndOfPath;

        private List<KeyValuePair<int, MineralType>> MineralsDistribution;
        private Simulation Simulation;
        private LinkedListNode<Wave> NextWave { get; set; }
        private List<Wave> ActiveWaves { get; set; }
        private double WaveCounter { get; set; }

        private int EnemiesCreatedCounter;


        public EnemiesController(Simulation simulation)
        {
            Simulation = simulation;

            Enemies = new List<Enemy>();
            Waves = new LinkedList<Wave>();
            ActiveWaves = new List<Wave>();
            Minerals = new List<Mineral>();
            MineralsDistribution = new List<KeyValuePair<int, MineralType>>();
            NextWaveData = new Dictionary<EnemyType, EnemyDescriptor>();
        }


        public void Initialize()
        {
            WaveCounter = 0;
            EnemiesCreatedCounter = 0;

            NextWave = Waves.First;

            RecalculateCompositionNextWave();

            if (InfiniteWaves != null)
                return;

            // Minerals Distribution
            MineralsDistribution.Clear();

            int enemiesCnt = 0;

            foreach (var wave in Waves)
                enemiesCnt += wave.EnemiesCount;

            Vector3 unitValue = new Vector3(
                Simulation.MineralsFactory.GetValue(MineralType.Cash10),
                Simulation.MineralsFactory.GetValue(MineralType.Cash25),
                Simulation.MineralsFactory.GetValue(MineralType.Cash150));
            Vector3 valuePerType = MineralsPercentages * MineralsValue; //atention: float
            Vector3 qtyPerType = valuePerType / unitValue;


            for (int i = 0; i < qtyPerType.X; i++)
                MineralsDistribution.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, enemiesCnt), MineralType.Cash10));

            for (int i = 0; i < qtyPerType.Y; i++)
                MineralsDistribution.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, enemiesCnt), MineralType.Cash25));

            for (int i = 0; i < qtyPerType.Z; i++)
                MineralsDistribution.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, enemiesCnt), MineralType.Cash150));

            for (int i = 0; i < LifePacksGiven; i++)
                MineralsDistribution.Add(new KeyValuePair<int, MineralType>(Main.Random.Next(0, enemiesCnt), MineralType.Life1));

            MineralsDistribution.Sort(delegate(KeyValuePair<int, MineralType> m1, KeyValuePair<int, MineralType> m2)
            {
                return m1.Key.CompareTo(m2.Key);
            });

            MineralsDistribution.Reverse();
        }


        public void Update()
        {
            for (int i = Enemies.Count - 1; i > -1; i--)
                if (!Enemies[i].Alive)
                {
                    Enemy enemy = Enemies[i];

                    enemy.DoDie();

                    if (enemy.Type == EnemyType.Damacloid)
                    {
                        EnemyDescriptor e = new EnemyDescriptor()
                        {
                            Type = EnemyType.Swarm,
                            CashValue = enemy.CashValue / 10,
                            LivesLevel = enemy.Level,
                            SpeedLevel = enemy.Level,
                            StartingPosition = enemy.Displacement
                        };

                        for (int j = 0; j < 10; j++)
                            ActiveWaves[0].AddEnemy(e);
                    }

                    foreach (var wave in ActiveWaves)
                        wave.DoEnemyDestroyed(enemy);

                    List<Mineral> minerals = enemy.Minerals;

                    for (int j = 0; j < minerals.Count; j++)
                    {
                        Mineral mineral = minerals[j];

                        mineral.Position = enemy.Position;

                        Vector3 direction;

                        Path.Direction(enemy.Displacement, out direction);

                        mineral.Direction = direction;
                    }

                    Minerals.AddRange(minerals);

                    NotifyObjectDestroyed(enemy);

                    Enemies.RemoveAt(i);
                }

            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].Update();

            for (int i = Minerals.Count - 1; i > -1; i--)
            {
                Minerals[i].Update();

                if (!Minerals[i].Alive)
                    Minerals.RemoveAt(i);

            }

            UpdateWaves();
        }


        public void Draw()
        {
            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].Draw();
        }


        public void DoObjectHit(IObjetPhysique obj, IObjetPhysique by)
        {
            Enemy enemy = obj as Enemy;

            if (enemy != null && enemy.Alive)
            {
                enemy.DoHit((ILivingObject)by);

                return;
            }


            Mineral min = obj as Mineral;

            if (min != null)
            {
                min.DoDie();

                Minerals.Remove(min);

                NotifyObjectDestroyed(min);

                return;
            }
        }


        public void DoNextWaveAsked()
        {
            if (NextWave != null)
            {
                WaveCounter = 0;

                ActiveWaves.Add(NextWave.Value);
                NextWave = (InfiniteWaves == null) ? NextWave.Next : new LinkedListNode<Wave>(InfiniteWaves.getProchaineVague());

                RecalculateCompositionNextWave();

                NotifyWaveStarted();
            }
        }


        public void ListenToEndOfPathReached(Enemy enemy)
        {
            enemy.DoDie();

            foreach (var wave in ActiveWaves)
                wave.DoEnemyDestroyed(enemy);

            NotifyEnemyReachedEndOfPath(enemy, Path.DernierRelais);
        }


        private void RecalculateCompositionNextWave()
        {
            NextWaveData.Clear();

            if (NextWave != null)
                foreach (var kvp in NextWave.Value.Composition)
                    NextWaveData.Add(kvp.Key, kvp.Value);
        }


        private void UpdateWaves()
        {

            WaveCounter += 16.66;

            if (NextWave != null && NextWave.Value.StartingTime <= WaveCounter)
            {
                WaveCounter = 0;

                ActiveWaves.Add(NextWave.Value);

                if (InfiniteWaves == null)
                    NextWave = NextWave.Next;
                else
                    NextWave.Value = InfiniteWaves.getProchaineVague();

                RecalculateCompositionNextWave();

                NotifyWaveStarted();
            }

            for (int i = ActiveWaves.Count - 1; i > -1; i--)
            {
                ActiveWaves[i].Update();

                List<Enemy> newEnemies = ActiveWaves[i].Enemies;

                Enemies.AddRange(newEnemies);

                for (int j = 0; j < newEnemies.Count; j++)
                {
                    Enemy e = newEnemies[j];

                    e.Path = this.Path;
                    e.PathPreview = this.PathPreview;
                    e.PathEndReached += new Enemy.EnemyHandler(this.ListenToEndOfPathReached);
                    e.Translation.Y = Main.Random.Next(-20, 20);

                    e.Initialize();

                    if (e.Type != EnemyType.Swarm)
                    {
                        while (MineralsDistribution.Count > 0 && MineralsDistribution[MineralsDistribution.Count - 1].Key == EnemiesCreatedCounter)
                        {
                            e.Minerals.Add(Simulation.MineralsFactory.CreateMineral(MineralsDistribution[MineralsDistribution.Count - 1].Value, e.Image.VisualPriority + 0.01f));

                            MineralsDistribution.RemoveAt(MineralsDistribution.Count - 1);
                        }
                    }

                    Simulation.Scene.Effects.Add(e.Image, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, e.FadeInTime));

                    EnemiesCreatedCounter++;

                    NotifyObjectCreated(e);
                }

                if (ActiveWaves[i].IsFinished)
                {
                    ActiveWaves.RemoveAt(i);
                    NotifyWaveEnded();
                }
            }
        }


        private void NotifyWaveEnded()
        {
            if (WaveEnded != null)
                WaveEnded();
        }


        private void NotifyWaveStarted()
        {
            if (WaveStarted != null)
                WaveStarted();
        }


        private void NotifyObjectDestroyed(IObjetPhysique obj)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(obj);
        }

        private void NotifyObjectCreated(IObjetPhysique obj)
        {
            if (ObjectCreated != null)
                ObjectCreated(obj);
        }


        private void NotifyEnemyReachedEndOfPath(Enemy enemy, CorpsCeleste celestialBody)
        {
            if (EnemyReachedEndOfPath != null)
                EnemyReachedEndOfPath(enemy, celestialBody);
        }
    }
}
