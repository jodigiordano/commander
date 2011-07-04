namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class WaveGenerator
    {
        private const int MinEnemiesPerWave = 10;


        private static List<double> TimeBetweenTwoWaves = new List<double>()
        {
            30000,
            60000,
            80000
        };


        private static List<double> TimePause = new List<double>()
        {
            5000,
            10000,
            15000
        };


        public static List<string> WavesTypesStrings = new List<string>()
        {
            WaveType.Homogene.ToString("g"),
            WaveType.DistinctFollow.ToString("g"),
            WaveType.PackedH.ToString("g")
        };


        public static List<string> DistancesStrings = new List<string>()
        {
            Distance.Joined.ToString("g"),
            Distance.Near.ToString("g"),
            Distance.Normal.ToString("g"),
            Distance.Far.ToString("g")
        };


        private static List<Distance> Distances = new List<Distance>()
        {
            Distance.Joined,
            Distance.Near,
            Distance.Normal,
            Distance.Far
        };


        private static Dictionary<WaveType, Vector2> MinMaxEnnemisTypeVague = new Dictionary<WaveType,Vector2>()
        {
            { WaveType.Homogene, new Vector2(1, 1) },
            { WaveType.DistinctFollow, new Vector2(2, int.MaxValue) },
            { WaveType.PackedH, new Vector2(1, 1) }
        };


        public int DifficultyStart;
        public int DifficultyEnd;
        public int QtyEnemies;
        public List<EnemyType> Enemies;
        public int WavesCount;
        public List<WaveDescriptor> Waves;
        public int MineralsPerWave;

        public WaveGenerator()
        {
            DifficultyStart = 1;
            DifficultyEnd = 1;
            QtyEnemies = 1;
            Enemies = new List<EnemyType>();
            WavesCount = 1;
        }

        public void Generate()
        {
            Waves = new List<WaveDescriptor>();

            // Ajustement dans la quantité d'ennemis
            QtyEnemies = Math.Max(QtyEnemies, WavesCount * MinEnemiesPerWave + MinEnemiesPerWave*2);

            List<WaveType> typesVaguesGenerees = new List<WaveType>();
            List<int> qtesParVague = new List<int>();
            List<List<EnemyType>> enemiesPerWave = new List<List<EnemyType>>();

            // Déterminer les types de vagues utilisées            
            do
            {
                typesVaguesGenerees.Clear();

                for (int i = 0; i < WavesCount; i++)
                {
                    WaveType typeVague = (WaveType)Main.Random.Next(0, MinMaxEnnemisTypeVague.Count);

                    if (MinMaxEnnemisTypeVague[typeVague].X > Enemies.Count)
                        i--;
                    else
                        typesVaguesGenerees.Add(typeVague);
                }

            } while (!AreAppropriateWaveTypes(typesVaguesGenerees));

            // Déterminer les quantitées allouées à chaque vague
            do
            {
                qtesParVague.Clear();

                for (int i = 0; i < WavesCount; i++)
                    qtesParVague.Add(0);

                for (int i = 0; i < QtyEnemies; i++)
                    qtesParVague[Main.Random.Next(0, WavesCount)]++;
            } while (!AreAppropriateQtyPerWave(qtesParVague));


            // Déterminer les ennemis utilisés dans chaque vague
            do
            {
                enemiesPerWave.Clear();

                for (int i = 0; i < WavesCount; i++)
                    enemiesPerWave.Add(new List<EnemyType>());

                //pour chaque vague => utiliser x ennemis où  x <= Min(TypeVague.MaxEnnemis, EnnemisDispo.Count)
                for (int i = 0; i < WavesCount; i++)
                {
                    int nbEnnemisDansCetteVague = (int) Math.Min(MinMaxEnnemisTypeVague[typesVaguesGenerees[i]].Y, Enemies.Count);

                    while (enemiesPerWave[i].Count < nbEnnemisDansCetteVague)
                    {
                        int type = Main.Random.Next(0, Enemies.Count);

                        if (!enemiesPerWave[i].Contains(Enemies[type]))
                            enemiesPerWave[i].Add(Enemies[type]);
                    }
                }

            } while (!AreAllEnemiesUsed(enemiesPerWave));


            // Generate the waves
            for (int i = 0; i < WavesCount; i++)
            {
                WaveType type = typesVaguesGenerees[i];
                int qty = qtesParVague[i];
                List<EnemyType> enemies = enemiesPerWave[i];

                WaveDescriptor descriptor = new WaveDescriptor();

                switch (type)
                {
                    case WaveType.Homogene: GenerateHomogene(descriptor); break;
                    case WaveType.DistinctFollow: GenerateDistinctsWhichFollow(descriptor, qty, enemies.Count); break;
                    case WaveType.PackedH: GeneratePackedHomogene(descriptor, qty); break;
                }

                descriptor.Enemies = enemies;
                descriptor.Quantity = qty;
                descriptor.StartingTime = TimeBetweenTwoWaves[Main.Random.Next(0, TimeBetweenTwoWaves.Count)];
                Waves.Add(descriptor);
            }

            AdjustDifficulty(Waves);
            DistributeMoney(Waves);
        }


        private void DistributeMoney(List<WaveDescriptor> vagues)
        {
            for (int i = 0; i < WavesCount; i++)
            {
                int valeurVague = (int) Math.Max(MineralsPerWave * (1.0/2.0/(WavesCount-i)), vagues[i].Quantity);
                int valeurEnnemi = (int)Math.Ceiling((double)valeurVague / vagues[i].Quantity);

                vagues[i].CashValue = valeurEnnemi;
            }
        }


        private void AdjustDifficulty(List<WaveDescriptor> vagues)
        {
            float step = (DifficultyEnd - DifficultyStart) / (float) ((WavesCount == 1) ? 1 : (WavesCount - 1));

            for (int i = 0; i < WavesCount; i++)
            {
                int difficulte = (int) (DifficultyStart + step * i);

                vagues[i].SpeedLevel = difficulte;
                vagues[i].LivesLevel = difficulte;
            }
        }


        private void GenerateDistinctsWhichFollow(WaveDescriptor description, int qty, int enemiesCount)
        {
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
            description.SwitchEvery = qty / enemiesCount;
            description.Delay = Main.Random.Next(2000, 4000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }


        private void GenerateHomogene(WaveDescriptor description)
        {
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
        }


        private void GeneratePackedHomogene(WaveDescriptor description, int qty)
        {
            description.Distance = Distance.Near;
            description.SwitchEvery = qty / Main.Random.Next(5, 10);
            description.Delay = Main.Random.Next(4000, 8000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }


        private bool AreAllEnemiesUsed(List<List<EnemyType>> ennemisParVague)
        {
            List<EnemyType> ennemisDispo = new List<EnemyType>(Enemies);

            for (int i = 0; i < ennemisParVague.Count; i++)
                for (int j = 0; j < ennemisParVague[i].Count; j++)
                    if (ennemisDispo.Contains(ennemisParVague[i][j]))
                        ennemisDispo.Remove(ennemisParVague[i][j]);

            return (ennemisDispo.Count == 0);
        }


        private bool AreAppropriateWaveTypes(List<WaveType> typesVaguesGenerees)
        {
            // Pas trop d'ennemis pour les sortes de vagues
            float sommeTypesEnnemisRequis = 0;

            for (int i = typesVaguesGenerees.Count - 1; i > -1; i--)
                sommeTypesEnnemisRequis += MinMaxEnnemisTypeVague[typesVaguesGenerees[i]].Y;

            if (sommeTypesEnnemisRequis < Enemies.Count)
                return false;

            return true;
        }


        private bool AreAppropriateQtyPerWave(List<int> qtesParVague)
        {
            foreach (var qte in qtesParVague)
                if (qte < MinEnemiesPerWave)
                    return false;

            return true;
        }
    }
}
