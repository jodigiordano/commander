﻿namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using ParallelTasks;


    class EnemiesController
    {
        public LinkedList<Wave> Waves;
        public InfiniteWave InfiniteWaves;
        public Path Path;
        public Path PathPreview;
        public int MineralsCash;
        public int LifePacksGiven;
        public List<Wave> ActiveWaves;
        public EnemiesData EnemiesData;
        
        public List<Enemy> Enemies;
        public List<Mineral> Minerals;
        public bool SpawnEnemies;

        public event NoneHandler WaveNearToStart;
        public event NoneHandler WaveStarted;
        public event NoneHandler WaveEnded;
        public event PhysicalObjectHandler ObjectHit;
        public event PhysicalObjectHandler ObjectDestroyed;
        public event PhysicalObjectHandler ObjectCreated;
        public event EnemyCelestialBodyHandler EnemyReachedEndOfPath;
        public event NextWaveHandler NextWaveCompositionChanged;

        private List<KeyValuePair<int, MineralType>> MineralsDistribution;
        private Simulator Simulator;
        private LinkedListNode<Wave> NextWave;
        private double TimeElapsedLastWave;
        private int EnemiesCreatedCounter;

        private Action SyncRemoveEnemies;
        private Action SyncRemoveMinerals;
        private Action SyncProcessDeadEnemies;
        private Action SyncUpdateEnemies;
        private Action SyncUpdateMinerals;

        private double MinTimeWaveNearToStart;


        public EnemiesController(Simulator simulator)
        {
            Simulator = simulator;

            Enemies = new List<Enemy>();
            ActiveWaves = new List<Wave>();
            Minerals = new List<Mineral>();
            MineralsDistribution = new List<KeyValuePair<int, MineralType>>();

            SyncRemoveEnemies = new Action(RemoveEnemies);
            SyncRemoveMinerals = new Action(RemoveMinerals);
            SyncProcessDeadEnemies = new Action(ProcessDeadEnemies);
            SyncUpdateEnemies = new Action(UpdateEnemies);
            SyncUpdateMinerals = new Action(UpdateMinerals);

            EnemiesData = new EnemiesData()
            {
                Enemies = Enemies
            };

            MinTimeWaveNearToStart = 10000;
        }


        public void Initialize()
        {
            Enemies.Clear();
            Minerals.Clear();
            MineralsDistribution.Clear();
            NextWave = Waves.First;
            ActiveWaves.Clear();

            TimeElapsedLastWave = 0;
            EnemiesCreatedCounter = 0;

            NotifyNextWaveCompositionChanged();

            if (InfiniteWaves != null)
                return;

            int enemiesCnt = 0;

            foreach (var wave in Waves)
            {
                wave.Initialize();
                enemiesCnt += wave.EnemiesCount;
            }

            EnemiesData.Path = Path;
            EnemiesData.MaxEnemiesForCountPerc = 20;

            // Minerals Distribution
            Vector3 unitValue = new Vector3(
                Simulator.MineralsFactory.GetValue(MineralType.Cash10),
                Simulator.MineralsFactory.GetValue(MineralType.Cash25),
                Simulator.MineralsFactory.GetValue(MineralType.Cash150));
            Vector3 valuePerType = new Vector3(0.6f, 0.3f, 0.1f) * MineralsCash; //atention: float
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

            SpawnEnemies = true;
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
            EnemiesData.Update();
        }


        public void Draw()
        {
            foreach (var e in Enemies)
                e.Draw();
        }


        public void DoObjectHit(ICollidable obj, ICollidable by)
        {
            Enemy enemy = obj as Enemy;

            if (enemy != null && enemy.Alive)
            {
                var living = (ILivingObject) by;

                enemy.DoHit(living);

                if (enemy.Alive && enemy.LastHitBy != living)
                {
                    enemy.LastHitBy = living;
                    NotifyObjectHit(enemy);
                }

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
            if (!SpawnEnemies)
                return;

            foreach (var w in ActiveWaves)
                foreach (var e in w.NewEnemies)
                    AddEnemy(e, w);
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
                    Simulator.MineralsFactory.Return(m);

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
                    Simulator.EnemiesFactory.Return(e);

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


        private void AddWave()
        {
            if (!SpawnEnemies)
                return;

            TimeElapsedLastWave += Preferences.TargetElapsedTimeMs;

            if (NextWave == null)
                return;

            if (NextWave.Value.StartingTime != 0 && !NextWave.Value.AnnouncedNearToStart &&
                NextWave.Value.StartingTime - TimeElapsedLastWave <= MinTimeWaveNearToStart)
            {
                NotifyWaveNearToStart();
                NextWave.Value.AnnouncedNearToStart = true;
            }

            if (NextWave.Value.StartingTime >= TimeElapsedLastWave)
                return;

            TimeElapsedLastWave = 0;

            ActiveWaves.Add(NextWave.Value);

            if (InfiniteWaves == null)
                NextWave = NextWave.Next;
            else
                NextWave.Value = InfiniteWaves.GetNextWave();

            NotifyNextWaveCompositionChanged();

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
                    e.Minerals.Add(Simulator.MineralsFactory.Get(MineralsDistribution[MineralsDistribution.Count - 1].Value, e.Image.VisualPriority + 0.01));

                    MineralsDistribution.RemoveAt(MineralsDistribution.Count - 1);
                }

                EnemiesCreatedCounter++;
            }

            var fadeInEffect = Core.Visual.VisualEffects.FadeInFrom0(Simulator.WorldMode? 150 : 255, 0, e.FadeInTime);

            Simulator.Scene.VisualEffects.Add(e.Image, fadeInEffect);

            Enemies.Add(e);

            NotifyObjectCreated(e);
        }


        private void ExtractSwarm(Enemy enemy)
        {
            for (int j = 0; j < 3; j++)
            {
                var e = EnemyDescriptor.Pool.Get();

                e.Type = EnemyType.Swarm;
                e.CashValue = enemy.CashValue / 10;
                e.LivesLevel = enemy.Level;
                e.SpeedLevel = enemy.Level;
                e.StartingPosition = enemy.Displacement;

                ActiveWaves[0].AddEnemy(e);
            }
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


        private void NotifyObjectHit(ICollidable obj)
        {
            if (ObjectHit != null)
                ObjectHit(obj);
        }


        private void NotifyObjectDestroyed(ICollidable obj)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(obj);
        }

        private void NotifyObjectCreated(ICollidable obj)
        {
            if (ObjectCreated != null)
                ObjectCreated(obj);
        }


        private void NotifyEnemyReachedEndOfPath(Enemy enemy, CelestialBody celestialBody)
        {
            if (EnemyReachedEndOfPath != null)
                EnemyReachedEndOfPath(enemy, celestialBody);
        }


        private void NotifyNextWaveCompositionChanged()
        {
            if (NextWaveCompositionChanged != null)
                NextWaveCompositionChanged(NextWave != null ? NextWave.Value.Descriptor : new WaveDescriptor());
        }


        private void NotifyWaveNearToStart()
        {
            if (WaveNearToStart != null)
                WaveNearToStart();
        }
    }
}
