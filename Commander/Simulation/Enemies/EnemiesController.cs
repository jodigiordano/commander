namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using ParallelTasks;
    using System;


    class EnemiesController
    {
        public List<Enemy> Enemies                                         { get; private set; }
        public LinkedList<Wave> Waves                                      { get; set; }
        public Dictionary<EnemyType, EnemyDescriptor> NextWaveData         { get; set; }
        public InfiniteWave InfiniteWaves                                  { get; set; }
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
        private Simulator Simulation;
        private LinkedListNode<Wave> NextWave { get; set; }
        private List<Wave> ActiveWaves { get; set; }
        private double WaveCounter { get; set; }

        private Action SyncRemoveEnemies;
        private Action SyncRemoveMinerals;
        private Action SyncProcessDeadEnemies;
        private Action SyncUpdateEnemies;
        private Action SyncUpdateMinerals;

        private int EnemiesCreatedCounter;


        public EnemiesController(Simulator simulation)
        {
            Simulation = simulation;

            Enemies = new List<Enemy>();
            Waves = new LinkedList<Wave>();
            ActiveWaves = new List<Wave>();
            Minerals = new List<Mineral>();
            MineralsDistribution = new List<KeyValuePair<int, MineralType>>();
            NextWaveData = new Dictionary<EnemyType, EnemyDescriptor>(EnemyTypeComparer.Default);

            SyncRemoveEnemies = new Action(RemoveEnemies);
            SyncRemoveMinerals = new Action(RemoveMinerals);
            SyncProcessDeadEnemies = new Action(ProcessDeadEnemies);
            SyncUpdateEnemies = new Action(UpdateEnemies);
            SyncUpdateMinerals = new Action(UpdateMinerals);
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
                return (m1.Key > m2.Key) ? -1 : (m1.Key < m2.Key) ? 1 : 0;
            });
        }




        public void Update()
        {
            Task t1, t2;

            t1 = Parallel.Start(SyncProcessDeadEnemies); //very tricky, risky one
            AddNewEnemies();
            t1.Wait();

            t1 = Parallel.Start(SyncRemoveEnemies);
            t2 = Parallel.Start(SyncRemoveMinerals);
            RemoveWaves();
            t1.Wait();
            t2.Wait();

            t1 = Parallel.Start(SyncUpdateEnemies);
            t2 = Parallel.Start(SyncUpdateMinerals);
            UpdateWaves();
            t1.Wait();
            t2.Wait();

            AddWave();
        }

        private void ProcessDeadEnemies()
        {
            for (int i = Enemies.Count - 1; i > -1; i--)
            {
                Enemy e = Enemies[i];

                if (!e.Alive)
                {
                    if (e.Type == EnemyType.Damacloid && !e.EndOfPathReached)
                        ExtractSwarm(e); //Add a descriptor to a Wave.ToCreate

                    RemoveFromWave(e); //Cycle in Waves; Wave.Alives--
                    ExtractMinerals(e); //Add minerals
                }
            }
        }

        private void AddNewEnemies()
        {
            foreach (var w in ActiveWaves)
                foreach (var e in w.NewEnemies)
                    AddEnemy(e, w);
        }


        public void Draw()
        {
            foreach (var e in Enemies)
                e.Draw();
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
                min.LifePoints = 0; 

                return;
            }
        }


        public void DoNextWaveAsked()
        {
            if (NextWave == null)
                return;

            NextWave.Value.StartingTime = 0;
        }


        private void RemoveWaves()
        {
            for (int i = ActiveWaves.Count - 1; i > -1; i--)
            {
                if (ActiveWaves[i].IsFinished)
                {
                    ActiveWaves.RemoveAt(i);
                    NotifyWaveEnded();
                }
            }
        }


        private void RemoveMinerals()
        {
            for (int i = Minerals.Count - 1; i > -1; i--)
            {
                Mineral m = Minerals[i];

                if (!m.Alive)
                {
                    m.DoDie();
                    Simulation.MineralsFactory.Return(m);

                    NotifyObjectDestroyed(m);

                    Minerals.RemoveAt(i);
                }
            }
        }


        private void RemoveEnemies()
        {
            for (int i = Enemies.Count - 1; i > -1; i--)
            {
                Enemy e = Enemies[i];

                if (!e.Alive)
                {
                    e.DoDie();
                    Simulation.EnemiesFactory.Return(e);

                    if (e.EndOfPathReached)
                        NotifyEnemyReachedEndOfPath(e, Path.LastCelestialBody);

                    NotifyObjectDestroyed(e);

                    Enemies.RemoveAt(i);
                }
            }
        }


        private void UpdateWaves()
        {
            foreach (var w in ActiveWaves)
                w.Update();
        }


        private void UpdateMinerals()
        {
            foreach (var m in Minerals)
                m.Update();
        }


        private void UpdateEnemies()
        {
            foreach (var e in Enemies)
                e.Update();
        }


        private void RecalculateCompositionNextWave()
        {
            NextWaveData.Clear();

            if (NextWave != null)
                foreach (var kvp in NextWave.Value.Composition)
                    NextWaveData.Add(kvp.Key, kvp.Value);
        }


        private void AddWave()
        {
            WaveCounter += Preferences.TargetElapsedTimeMs;

            if (NextWave == null || NextWave.Value.StartingTime >= WaveCounter)
                return;

            WaveCounter = 0;

            ActiveWaves.Add(NextWave.Value);

            if (InfiniteWaves == null)
                NextWave = NextWave.Next;
            else
                NextWave.Value = InfiniteWaves.GetNextWave();

            RecalculateCompositionNextWave();

            NotifyWaveStarted();
        }


        private void AddEnemy(Enemy e, Wave w)
        {
            e.Path = this.Path;
            e.PathPreview = this.PathPreview;
            e.Translation.Y = Main.Random.Next(-20, 20);
            e.WaveId = w.GetHashCode();

            e.Initialize();

            if (e.Type != EnemyType.Swarm)
            {
                while (MineralsDistribution.Count > 0 && MineralsDistribution[MineralsDistribution.Count - 1].Key == EnemiesCreatedCounter)
                {
                    e.Minerals.Add(Simulation.MineralsFactory.Get(MineralsDistribution[MineralsDistribution.Count - 1].Value, e.Image.VisualPriority + 0.01));

                    MineralsDistribution.RemoveAt(MineralsDistribution.Count - 1);
                }

                EnemiesCreatedCounter++;
            }

            Simulation.Scene.VisualEffects.Add(e.Image, Core.Visual.VisualEffects.FadeInFrom0(255, 0, e.FadeInTime));

            Enemies.Add(e);

            NotifyObjectCreated(e);
        }


        private void ExtractSwarm(Enemy enemy)
        {
            var e = EnemyDescriptor.Pool.Get();
            
            e.Type = EnemyType.Swarm;
            e.CashValue = enemy.CashValue / 10;
            e.LivesLevel = enemy.Level;
            e.SpeedLevel = enemy.Level;
            e.StartingPosition = enemy.Displacement;

            for (int j = 0; j < 3; j++)
                ActiveWaves[0].AddEnemy(e);
        }


        private void ExtractMinerals(Enemy enemy)
        {
            foreach (var m in enemy.Minerals)
            {
                m.Position = enemy.Position;

                Vector3 direction;

                Path.Direction(enemy.Displacement, out direction);

                m.Direction = direction;

                Minerals.Add(m);
            }
        }


        private void RemoveFromWave(Enemy enemy)
        {
            foreach (var w in Waves)
                if (w.GetHashCode() == enemy.WaveId)
                {
                    w.DoEnemyDestroyed();
                    break;
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


        private void NotifyEnemyReachedEndOfPath(Enemy enemy, CelestialBody celestialBody)
        {
            if (EnemyReachedEndOfPath != null)
                EnemyReachedEndOfPath(enemy, celestialBody);
        }
    }
}
