namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    public enum Distance
    {
        Colles = 0,
        Proche = 10,
        Normal = 60,
        Eloigne = 110
    }


    public enum Taille
    {
        Petite = 28,
        Moyenne = 50,
        Grande = 68
    }


    public class WorldDescriptor
    {
        public int Id;
        public List<int> Levels;
        public List<KeyValuePair<int, int>> Warps;
        public DescripteurScenario SimulationDescription;
        public List<int> UnlockedCondition;
        public string WarpBlockedMessage;


        public WorldDescriptor()
        {
            Id = -1;
            Levels = new List<int>();
            Warps = new List<KeyValuePair<int, int>>();
            SimulationDescription = new DescripteurScenario();
            UnlockedCondition = new List<int>();
            WarpBlockedMessage = "";
        }
    }


    public class DescripteurScenario
    {
        public int Numero;
        public String Mission;
        public String Annee;
        public String Lieu;
        public String Objectif;
        public String Description;
        public String Image;
        public String Difficulte;
        public List<DescripteurCorpsCeleste> SystemePlanetaire;
        [ContentSerializer(Optional = true)]
        public DescripteurVaguesInfinies VaguesInfinies;
        public List<WaveDescriptor> Waves;
        public DescripteurJoueur Joueur;
        public String CorpsCelesteAProteger;
        public String FondEcran;
        [ContentSerializer(Optional = true)]
        public int ValeurMinerauxDonnes;
        [ContentSerializer(Optional = true)]
        public Vector3 PourcentageMinerauxDonnes;
        [ContentSerializer(Optional = true)]
        public int NbPackViesDonnes;
        [ContentSerializer(Optional = true)]
        public List<string> HelpTexts;


        public DescripteurScenario()
        {
            Numero = -1;
            Mission = "test";
            Annee = "test";
            Lieu = "test";
            Objectif = "test";
            Description = "test";
            Image = "scenario1";
            Difficulte = "test";


            SystemePlanetaire = new List<DescripteurCorpsCeleste>();
            Waves = new List<WaveDescriptor>();
            VaguesInfinies = null;
            Joueur = new DescripteurJoueur();
            CorpsCelesteAProteger = null;
            FondEcran = "fond1";

            ValeurMinerauxDonnes = 500;
            PourcentageMinerauxDonnes = new Vector3(0.6f, 0.3f, 0.1f);
            NbPackViesDonnes = 5;

            HelpTexts = new List<string>();
        }


        public void ajouterCorpsCeleste(Taille taille, Vector3 position, String nom, String representation, int vitesse, int priorite)
        {
            DescripteurCorpsCeleste d = new DescripteurCorpsCeleste();
            d.Nom = nom;
            d.Invincible = true;
            d.Position = position;
            d.PositionDepart = 0;
            d.Priorite = priorite;
            d.Taille = taille;
            d.TourellesPermises = null;
            d.Vitesse = vitesse;
            d.Representation = representation;
            d.Emplacements = new List<DescripteurEmplacement>();

            SystemePlanetaire.Add(d);
        }


        public void ajouterTrouRose(Vector3 position, String nom, int vitesse, int priorite)
        {
            DescripteurCorpsCeleste d = new DescripteurCorpsCeleste();
            d.Nom = nom;
            d.Invincible = true;
            d.Position = position;
            d.PositionDepart = 0;
            d.Priorite = priorite;
            d.Taille = Taille.Petite;
            d.TourellesPermises = null;
            d.Vitesse = vitesse;
            d.RepresentationParticules = "trouRose";
            d.Emplacements = new List<DescripteurEmplacement>();

            SystemePlanetaire.Add(d);
        }


        public double ParTime
        {
            get
            {
                if (VaguesInfinies != null)
                    return 0;

                double parTime = 0;

                foreach (var wave in Waves)
                    parTime += wave.StartingTime;

                return parTime;
            }
        }


        public int NbStars(int score)
        {
            int maxCash = 0;

            foreach (var wave in Waves)
                maxCash += wave.Quantity * wave.CashValue;

            int maxLives = Joueur.PointsDeVie + NbPackViesDonnes;

            int bestScore = (maxLives * 50) + maxCash + (int)(ParTime / 100);

            bestScore = (int) (bestScore * 0.75);

            return (score >= bestScore) ? 3 :
                   (score >= bestScore * 0.75) ? 2 :
                   (score > -bestScore * 0.5) ? 1 : 0;
        }
    }


    public class DescripteurVaguesInfinies
    {
        public List<EnemyType> EnnemisPresents;
        public int DifficulteDepart;
        public int IncrementDifficulte;
        public Vector2 MinMaxEnnemisParVague;
        public int MinerauxParVague;
        public bool FirstOneStartNow;

        public DescripteurVaguesInfinies()
        {
            EnnemisPresents = new List<EnemyType>();
            DifficulteDepart = 1;
            IncrementDifficulte = 0;
            MinMaxEnnemisParVague = new Vector2(1, 1);
            FirstOneStartNow = false;
        }
    }


    public class DescripteurCorpsCeleste
    {
        public String Nom;
        public List<DescripteurEmplacement> Emplacements;
        [ContentSerializer(Optional = true)]
        public String Representation;
        public Taille Taille;
        [ContentSerializer(Optional = true)]
        public String RepresentationParticules;
        public int Vitesse;
        public int Priorite;
        public Vector3 Position;
        [ContentSerializer(Optional = true)]
        public Vector3 Offset;
        [ContentSerializer(Optional = true)]
        public int Rotation;
        [ContentSerializer(Optional = true)]
        public bool EnBackground;
        [ContentSerializer(Optional = true)]
        public bool PeutAvoirCollecteur;
        [ContentSerializer(Optional = true)]
        public bool PeutAvoirDoItYourself;
        [ContentSerializer(Optional = true)]
        public bool PeutAvoirTheResistance;
        [ContentSerializer(Optional = true)]
        public bool PeutDetruire;
        public bool Selectionnable;
        public bool Invincible;
        [ContentSerializer(Optional = true)]
        public List<TurretType> TourellesPermises;
        public int PositionDepart;
        public List<String> Representations;


        public DescripteurCorpsCeleste()
        {
            Nom = "CorpsCeleste";
            Emplacements = new List<DescripteurEmplacement>();
            Representation = null;
            RepresentationParticules = null;
            Vitesse = 0;
            Priorite = -1;
            Position = Vector3.Zero;
            Offset = Vector3.Zero;
            Selectionnable = true;
            Invincible = false;
            TourellesPermises = null;
            Taille = Taille.Moyenne;
            PositionDepart = 0;
            PeutAvoirCollecteur = false;
            PeutAvoirDoItYourself = false;
            PeutAvoirTheResistance = false;
            PeutDetruire = false;
            Representations = new List<String>();
            EnBackground = false;
            Rotation = 0;
        }


        public void ajouterTourelle(TurretType typeTourelle, int niveau, Vector3 position, bool visible)
        {
            DescripteurEmplacement e = new DescripteurEmplacement();
            e.Position = position;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = typeTourelle;
            e.Tourelle.Niveau = niveau;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            e.Tourelle.Visible = visible;

            Emplacements.Add(e);
        }
    }


    public class WaveDescriptor
    {
        public double StartingTime;
        public List<EnemyType> Enemies;
        public int SpeedLevel;
        public int LivesLevel;
        public int CashValue;
        public int Quantity;
        public Distance Distance;
        public double Delay;
        public int ApplyDelayEvery;
        public int SwitchEvery;


        public WaveDescriptor()
        {
            StartingTime = 0;
            Enemies = new List<EnemyType>();
            SpeedLevel = 1;
            LivesLevel = 1;
            CashValue = 1;
            Quantity = 1;
            Distance = Distance.Colles;
            Delay = 0;
            ApplyDelayEvery = -1;
            SwitchEvery = -1;
        }


        public List<EnemyDescriptor> GetEnemiesToCreate()
        {
            var results = new List<EnemyDescriptor>();
            int typeIndex = 0;
            double lastTimeCreated = 0;

            for (int i = 0; i < Quantity; i++)
            {
                if ((i + 1) % SwitchEvery == 0)
                    typeIndex = (typeIndex + 1) % Enemies.Count;

                var type = Enemies[typeIndex];

                double delay = ((i + 1) % ApplyDelayEvery == 0) ? Delay : 0;

                double frequency =
                    FactoryEnnemis.Instance.getTaille(type) + (int)Distance /
                    FactoryEnnemis.Instance.getVitesse(type, SpeedLevel) * (1000f / 60f);

                var e = new EnemyDescriptor()
                {
                    Type = type,
                    CashValue = this.CashValue,
                    LivesLevel = this.LivesLevel,
                    SpeedLevel = this.SpeedLevel,
                    StartingTime = lastTimeCreated + frequency + delay
                };

                results.Add(e);

                lastTimeCreated = e.StartingTime;
            }


            return results;
        }
    }


    public class DescripteurJoueur
    {
        public int ReserveUnites;
        public int PointsDeVie;

        public DescripteurJoueur()
        {
            ReserveUnites = 0;
            PointsDeVie = 1;
        }
    }


    public class DescripteurTourelle
    {
        public TurretType Type;
        public bool PeutVendre;
        public bool PeutMettreAJour;
        public bool Visible;


        [ContentSerializer(Optional = true)]
        public int Niveau;

        public DescripteurTourelle()
        {
            Type = TurretType.Unknown;
            PeutVendre = true;
            PeutMettreAJour = true;
            Niveau = 1;
            Visible = true;
        }
    }


    public class DescripteurEmplacement
    {
        [ContentSerializer(Optional = true)]
        public DescripteurTourelle Tourelle;

        public Vector3 Position;
        public String Representation;

        public DescripteurEmplacement()
        {
            Tourelle = null;
            Position = Vector3.Zero;
            Representation = "emplacement";
        }
    }


    public class EnemyDescriptor
    {
        public EnemyType Type;
        public int SpeedLevel;
        public int LivesLevel;
        public int CashValue;
        public double StartingTime;
    }
}
