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

        private static List<Distance> Distances = new List<Distance>()
        {
            Distance.Joined,
            Distance.Near,
            Distance.Normal,
            Distance.Far
        };

        private enum WaveType
        {
            Homogene = 0,
            /*Entrecroisement,
            Heterogene,
            MiniVaguesHeterogenes,
            MiniVaguesHomogenes,*/
            DistinctWhichFollow,
            /*PackHeterogene,*/
            PackHomogene
        }

        private static Dictionary<WaveType, Vector2> MinMaxEnnemisTypeVague = new Dictionary<WaveType,Vector2>()
        {
            { WaveType.Homogene, new Vector2(1, 1) },
            /*{ TypeVague.Entrecroisement, new Vector2(2, int.MaxValue) },
            { TypeVague.Heterogene, new Vector2(2, int.MaxValue) },*/
            /*{ TypeVague.MiniVaguesHeterogenes, new Vector2(3, int.MaxValue) },
            { TypeVague.MiniVaguesHomogenes, new Vector2(1, 1) },*/
            { WaveType.DistinctWhichFollow, new Vector2(2, int.MaxValue) },
            { WaveType.PackHomogene, new Vector2(1, 1) }/*,
            { TypeVague.PackHeterogene, new Vector2(2, int.MaxValue) }*/
        };

        public int DifficultyStart;
        public int DifficultyEnd;
        public int QtyEnemies;
        public List<EnemyType> Enemies;
        public int NbWaves;
        public List<WaveDescriptor> Waves;
        public int MineralsPerWave;

        public WaveGenerator()
        {
            DifficultyStart = 1;
            DifficultyEnd = 1;
            QtyEnemies = 1;
            Enemies = new List<EnemyType>();
            NbWaves = 1;
        }

        public void Generate()
        {
            Waves = new List<WaveDescriptor>();

            // Ajustement dans la quantité d'ennemis
            QtyEnemies = Math.Max(QtyEnemies, NbWaves * MinEnemiesPerWave + MinEnemiesPerWave*2);

            List<WaveType> typesVaguesGenerees = new List<WaveType>();
            List<int> qtesParVague = new List<int>();
            List<List<EnemyType>> ennemisParVague = new List<List<EnemyType>>();

            // Déterminer les types de vagues utilisées            
            do
            {
                typesVaguesGenerees.Clear();

                for (int i = 0; i < NbWaves; i++)
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

                for (int i = 0; i < NbWaves; i++)
                    qtesParVague.Add(0);

                for (int i = 0; i < QtyEnemies; i++)
                    qtesParVague[Main.Random.Next(0, NbWaves)]++;
            } while (!AreAppropriateQtyPerWave(qtesParVague));


            // Déterminer les ennemis utilisés dans chaque vague
            do
            {
                ennemisParVague.Clear();

                for (int i = 0; i < NbWaves; i++)
                    ennemisParVague.Add(new List<EnemyType>());

                //pour chaque vague => utiliser x ennemis où  x <= Min(TypeVague.MaxEnnemis, EnnemisDispo.Count)
                for (int i = 0; i < NbWaves; i++)
                {
                    int nbEnnemisDansCetteVague = (int) Math.Min(MinMaxEnnemisTypeVague[typesVaguesGenerees[i]].Y, Enemies.Count);

                    while (ennemisParVague[i].Count < nbEnnemisDansCetteVague)
                    {
                        int type = Main.Random.Next(0, Enemies.Count);

                        if (!ennemisParVague[i].Contains(Enemies[type]))
                            ennemisParVague[i].Add(Enemies[type]);
                    }
                }

            } while (!AreAllEnemiesUsed(ennemisParVague));


            // Générer les vagues (enfin!)
            for (int i = 0; i < NbWaves; i++)
            {
                WaveType type = typesVaguesGenerees[i];
                int quantite = qtesParVague[i];
                List<EnemyType> ennemis = ennemisParVague[i];

                WaveDescriptor description = new WaveDescriptor();

                switch (type)
                {
                    case WaveType.Homogene: GenerateHomogene(description, quantite, ennemis); break;
                    //case TypeVague.Heterogene: genererHeterogene(description, quantite, ennemis); break;
                    //case TypeVague.Entrecroisement: genererEntrecroisement(description, quantite, ennemis); break;
                    case WaveType.DistinctWhichFollow: GenerateDistinctsWhichFollow(description, quantite, ennemis); break;
                    //case TypeVague.MiniVaguesHeterogenes: genererHomogene(description, quantite, ennemis[0]); break;
                    //case TypeVague.MiniVaguesHomogenes: genererHomogene(description, quantite, ennemis[0]); break;
                    //case TypeVague.PackHeterogene: genererPackHeterogene(description, quantite, ennemis); break;
                    case WaveType.PackHomogene: GeneratePackHomogene(description, quantite, ennemis); break;
                }

                description.StartingTime = TimeBetweenTwoWaves[Main.Random.Next(0, TimeBetweenTwoWaves.Count)];
                Waves.Add(description);
            }

            AdjustDifficulty(Waves);
            DistributeMoney(Waves);
        }


        private void DistributeMoney(List<WaveDescriptor> vagues)
        {
            for (int i = 0; i < NbWaves; i++)
            {
                int valeurVague = (int) Math.Max(MineralsPerWave * (1.0/2.0/(NbWaves-i)), vagues[i].Quantity);
                int valeurEnnemi = (int)Math.Ceiling((double)valeurVague / vagues[i].Quantity);

                vagues[i].CashValue = valeurEnnemi;
            }
        }


        private void AdjustDifficulty(List<WaveDescriptor> vagues)
        {
            float step = (DifficultyEnd - DifficultyStart) / (float) ((NbWaves == 1) ? 1 : (NbWaves - 1));

            for (int i = 0; i < NbWaves; i++)
            {
                int difficulte = (int) (DifficultyStart + step * i);

                vagues[i].SpeedLevel = difficulte;
                vagues[i].LivesLevel = difficulte;
            }
        }


        private void GenerateDistinctsWhichFollow(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Enemies = ennemis;
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
            description.Quantity = qte;
            description.SwitchEvery = qte / ennemis.Count;
            description.Delay = Main.Random.Next(2000, 4000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }


        //private void genererEntrecroisement(DescripteurVague description, int qte, List<EnemyType> ennemis)
        //{
        //    Distance distance = DistancesDisponibles[Main.Random.Next(2, DistancesDisponibles.Count)];

        //    int QteAvantSwitch = qte / Main.Random.Next(5, 10);

        //    QteAvantSwitch = (int)MathHelper.Clamp(QteAvantSwitch, 10, 30);

        //    for (int i = 0; i < qte; i++)
        //    {
        //        description.Distances.Add(distance);

        //        DescripteurEnnemi descEnnemi = new DescripteurEnnemi();
        //        descEnnemi.Type = ennemis[(i / QteAvantSwitch) % ennemis.Count];

        //        description.Ennemis.Add(descEnnemi);
        //    }
        //}


        private void GenerateHomogene(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Quantity = qte;
            description.Enemies = ennemis;
            description.Distance = Distances[Main.Random.Next(2, Distances.Count)];
        }


        private void GeneratePackHomogene(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Enemies = ennemis;
            description.Distance = Distance.Near;
            description.Quantity = qte;
            description.SwitchEvery = qte / Main.Random.Next(5, 10);
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
