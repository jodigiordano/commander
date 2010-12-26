namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;

    class GenerateurVagues
    {
        private const int MinEnnemisVague = 10;

        private static List<double> TempsEntreDeuxVagues = new List<double>()
        {
            30000,
            60000,
            80000
        };

        private static List<double> TempsMiniPause = new List<double>()
        {
            5000,
            10000,
            15000
        };

        private static List<Distance> DistancesDisponibles = new List<Distance>()
        {
            Distance.Colles,
            Distance.Proche,
            Distance.Normal,
            Distance.Eloigne
        };

        private enum TypeVague
        {
            Homogene = 0,
            /*Entrecroisement,
            Heterogene,
            MiniVaguesHeterogenes,
            MiniVaguesHomogenes,*/
            DistinctesQuiSeSuivent,
            /*PackHeterogene,*/
            PackHomogene
        }

        private static Dictionary<TypeVague, Vector2> MinMaxEnnemisTypeVague = new Dictionary<TypeVague,Vector2>()
        {
            { TypeVague.Homogene, new Vector2(1, 1) },
            /*{ TypeVague.Entrecroisement, new Vector2(2, int.MaxValue) },
            { TypeVague.Heterogene, new Vector2(2, int.MaxValue) },*/
            /*{ TypeVague.MiniVaguesHeterogenes, new Vector2(3, int.MaxValue) },
            { TypeVague.MiniVaguesHomogenes, new Vector2(1, 1) },*/
            { TypeVague.DistinctesQuiSeSuivent, new Vector2(2, int.MaxValue) },
            { TypeVague.PackHomogene, new Vector2(1, 1) }/*,
            { TypeVague.PackHeterogene, new Vector2(2, int.MaxValue) }*/
        };

        public int DifficulteDebut;
        public int DifficulteFin;
        public int QteEnnemis;
        public List<EnemyType> EnnemisPresents;
        public int NbVagues;
        public List<WaveDescriptor> Vagues;
        public int ArgentEnnemis;

        public GenerateurVagues()
        {
            DifficulteDebut = 1;
            DifficulteFin = 1;
            QteEnnemis = 1;
            EnnemisPresents = new List<EnemyType>();
            NbVagues = 1;
        }

        public void generer()
        {
            Vagues = new List<WaveDescriptor>();

            // Ajustement dans la quantité d'ennemis
            QteEnnemis = Math.Max(QteEnnemis, NbVagues * MinEnnemisVague + MinEnnemisVague*2);

            List<TypeVague> typesVaguesGenerees = new List<TypeVague>();
            List<int> qtesParVague = new List<int>();
            List<List<EnemyType>> ennemisParVague = new List<List<EnemyType>>();

            // Déterminer les types de vagues utilisées            
            do
            {
                typesVaguesGenerees.Clear();

                for (int i = 0; i < NbVagues; i++)
                {
                    TypeVague typeVague = (TypeVague)Main.Random.Next(0, MinMaxEnnemisTypeVague.Count);

                    if (MinMaxEnnemisTypeVague[typeVague].X > EnnemisPresents.Count)
                        i--;
                    else
                        typesVaguesGenerees.Add(typeVague);
                }

            } while (!typesVaguesAppropriees(typesVaguesGenerees));

            // Déterminer les quantitées allouées à chaque vague
            do
            {
                qtesParVague.Clear();

                for (int i = 0; i < NbVagues; i++)
                    qtesParVague.Add(0);

                for (int i = 0; i < QteEnnemis; i++)
                    qtesParVague[Main.Random.Next(0, NbVagues)]++;
            } while (!qteParVagueApproprie(qtesParVague));


            // Déterminer les ennemis utilisés dans chaque vague
            do
            {
                ennemisParVague.Clear();

                for (int i = 0; i < NbVagues; i++)
                    ennemisParVague.Add(new List<EnemyType>());

                //pour chaque vague => utiliser x ennemis où  x <= Min(TypeVague.MaxEnnemis, EnnemisDispo.Count)
                for (int i = 0; i < NbVagues; i++)
                {
                    int nbEnnemisDansCetteVague = (int) Math.Min(MinMaxEnnemisTypeVague[typesVaguesGenerees[i]].Y, EnnemisPresents.Count);

                    while (ennemisParVague[i].Count < nbEnnemisDansCetteVague)
                    {
                        int type = Main.Random.Next(0, EnnemisPresents.Count);

                        if (!ennemisParVague[i].Contains(EnnemisPresents[type]))
                            ennemisParVague[i].Add(EnnemisPresents[type]);
                    }
                }

            } while (!tousEnnemisUtilises(ennemisParVague));


            // Générer les vagues (enfin!)
            for (int i = 0; i < NbVagues; i++)
            {
                TypeVague type = typesVaguesGenerees[i];
                int quantite = qtesParVague[i];
                List<EnemyType> ennemis = ennemisParVague[i];

                WaveDescriptor description = new WaveDescriptor();

                switch (type)
                {
                    case TypeVague.Homogene: genererHomogene(description, quantite, ennemis); break;
                    //case TypeVague.Heterogene: genererHeterogene(description, quantite, ennemis); break;
                    //case TypeVague.Entrecroisement: genererEntrecroisement(description, quantite, ennemis); break;
                    case TypeVague.DistinctesQuiSeSuivent: genererDistinctesQuiSeSuivent(description, quantite, ennemis); break;
                    //case TypeVague.MiniVaguesHeterogenes: genererHomogene(description, quantite, ennemis[0]); break;
                    //case TypeVague.MiniVaguesHomogenes: genererHomogene(description, quantite, ennemis[0]); break;
                    //case TypeVague.PackHeterogene: genererPackHeterogene(description, quantite, ennemis); break;
                    case TypeVague.PackHomogene: genererPackHomogene(description, quantite, ennemis); break;
                }

                description.StartingTime = TempsEntreDeuxVagues[Main.Random.Next(0, TempsEntreDeuxVagues.Count)];
                Vagues.Add(description);
            }

            ajusterDifficulte(Vagues);
            distribuerArgent(Vagues);
        }


        private void distribuerArgent(List<WaveDescriptor> vagues)
        {
            for (int i = 0; i < NbVagues; i++)
            {
                int valeurVague = (int) Math.Max(ArgentEnnemis * (1.0/2.0/(NbVagues-i)), vagues[i].Quantity);
                int valeurEnnemi = (int)Math.Ceiling((double)valeurVague / vagues[i].Quantity);

                vagues[i].CashValue = valeurEnnemi;
            }
        }


        private void ajusterDifficulte(List<WaveDescriptor> vagues)
        {
            float step = (DifficulteFin - DifficulteDebut) / (float) ((NbVagues == 1) ? 1 : (NbVagues - 1));

            for (int i = 0; i < NbVagues; i++)
            {
                int difficulte = (int) (DifficulteDebut + step * i);

                vagues[i].SpeedLevel = difficulte;
                vagues[i].LivesLevel = difficulte;
            }
        }


        private void genererDistinctesQuiSeSuivent(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Enemies = ennemis;
            description.Distance = DistancesDisponibles[Main.Random.Next(2, DistancesDisponibles.Count)];
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


        private void genererHomogene(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Quantity = qte;
            description.Enemies = ennemis;
            description.Distance = DistancesDisponibles[Main.Random.Next(2, DistancesDisponibles.Count)];
        }


        private void genererPackHomogene(WaveDescriptor description, int qte, List<EnemyType> ennemis)
        {
            description.Enemies = ennemis;
            description.Distance = Distance.Proche;
            description.Quantity = qte;
            description.SwitchEvery = qte / Main.Random.Next(5, 10);
            description.Delay = Main.Random.Next(4000, 8000);
            description.ApplyDelayEvery = description.SwitchEvery;
        }


        private bool tousEnnemisUtilises(List<List<EnemyType>> ennemisParVague)
        {
            List<EnemyType> ennemisDispo = new List<EnemyType>(EnnemisPresents);

            for (int i = 0; i < ennemisParVague.Count; i++)
                for (int j = 0; j < ennemisParVague[i].Count; j++)
                    if (ennemisDispo.Contains(ennemisParVague[i][j]))
                        ennemisDispo.Remove(ennemisParVague[i][j]);

            return (ennemisDispo.Count == 0);
        }


        private bool typesVaguesAppropriees(List<TypeVague> typesVaguesGenerees)
        {
            // Pas trop d'ennemis pour les sortes de vagues
            float sommeTypesEnnemisRequis = 0;

            for (int i = typesVaguesGenerees.Count - 1; i > -1; i--)
                sommeTypesEnnemisRequis += MinMaxEnnemisTypeVague[typesVaguesGenerees[i]].Y;

            if (sommeTypesEnnemisRequis < EnnemisPresents.Count)
                return false;

            return true;
        }


        private bool qteParVagueApproprie(List<int> qtesParVague)
        {
            foreach (var qte in qtesParVague)
                if (qte < MinEnnemisVague)
                    return false;

            return true;
        }
    }
}
