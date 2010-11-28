namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    [Serializable]
    public enum Distance
    {
        Colles = 0,
        Proche = 10,
        Normal = 60,
        Eloigne = 110
    }


    [Serializable]
    public enum Taille
    {
        Petite = 32,
        Moyenne = 60,
        Grande = 88
    }


    [Serializable]
    public class DescripteurScenario
    {
        public int Numero { get; set; }
        public String Mission { get; set; }
        public String Annee { get; set; }
        public String Lieu { get; set; }
        public String Objectif { get; set; }
        public String Description { get; set; }
        public String Image { get; set; }
        public String Difficulte { get; set; }

        public List<DescripteurCorpsCeleste> SystemePlanetaire { get; set; }

        [ContentSerializer(Optional = true)]
        public DescripteurVaguesInfinies VaguesInfinies { get; set; }

        public List<DescripteurVague> Vagues { get; set; }
        
        public DescripteurJoueur Joueur { get; set; }

        public String CorpsCelesteAProteger { get; set; }

        public String FondEcran { get; set; }

        [ContentSerializer(Optional = true)]
        public int ValeurMinerauxDonnes { get; set; }

        [ContentSerializer(Optional = true)]
        public Vector3 PourcentageMinerauxDonnes { get; set; }

        [ContentSerializer(Optional = true)]
        public int NbPackViesDonnes { get; set; }

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
            Vagues = new List<DescripteurVague>();
            VaguesInfinies = null;
            Joueur = new DescripteurJoueur();
            CorpsCelesteAProteger = null;
            FondEcran = "fond1";

            ValeurMinerauxDonnes = 500;
            PourcentageMinerauxDonnes = new Vector3(0.6f, 0.3f, 0.1f);
            NbPackViesDonnes = 5;
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


        public override string ToString()
        {
            String s =
                "Scenario. " +
                "Numero: " + Numero +
                ", Mission: " + Mission +
                ", Annee: " + Annee +
                ", Lieu: " + Lieu +
                ", Objectif: " + Objectif +
                ", Image: " + Image +
                ", Difficulte: " + Difficulte +
                ", FondEcran: " + FondEcran +
                ", Joueur: " + Joueur +
                ", CorpsCelesteAProteger: " + ((CorpsCelesteAProteger == null) ? "Aucune" : CorpsCelesteAProteger);

            foreach (var c in SystemePlanetaire)
                s += c;

            foreach (var v in Vagues)
                s += v;

            return s;
        }
    }


    [Serializable]
    public class DescripteurVaguesInfinies
    {
        public List<TypeEnnemi> EnnemisPresents     { get; set; }
        public int DifficulteDepart                 { get; set; }
        public int IncrementDifficulte              { get; set; }
        public Vector2 MinMaxEnnemisParVague        { get; set; }
        public int MinerauxParVague                 { get; set; }
        public bool FirstOneStartNow                { get; set; }

        public DescripteurVaguesInfinies()
        {
            EnnemisPresents = new List<TypeEnnemi>();
            DifficulteDepart = 1;
            IncrementDifficulte = 0;
            MinMaxEnnemisParVague = new Vector2(1, 1);
            FirstOneStartNow = false;
        }
    }


    [Serializable]
    public class DescripteurCorpsCeleste
    {
        public String Nom { get; set; }
        public List<DescripteurEmplacement> Emplacements { get; set; }

        [ContentSerializer(Optional = true)]
        public String Representation { get; set; }
        public Taille Taille { get; set; }

        [ContentSerializer(Optional = true)]
        public String RepresentationParticules { get; set; }

        public int Vitesse { get; set; }
        public int Priorite { get; set; }
        public Vector3 Position { get; set; }

        [ContentSerializer(Optional = true)]
        public Vector3 Offset { get; set; }

        [ContentSerializer(Optional = true)]
        public int Rotation { get; set; }

        [ContentSerializer(Optional = true)]
        public bool EnBackground { get; set; }

        [ContentSerializer(Optional = true)]
        public bool PeutAvoirCollecteur { get; set; }

        [ContentSerializer(Optional = true)]
        public bool PeutAvoirDoItYourself { get; set; }

        [ContentSerializer(Optional = true)]
        public bool PeutAvoirTheResistance { get; set; }

        [ContentSerializer(Optional = true)]
        public bool PeutDetruire { get; set; }

        public bool Selectionnable { get; set; }
        public bool Invincible { get; set; }

        [ContentSerializer(Optional = true)]
        public List<TypeTourelle> TourellesPermises { get; set; }

        public int PositionDepart { get; set; }
        
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


        public override string ToString()
        {
            String s =
                "Corps Celeste. " +
                "Nom: " + Nom +
                ", Representation: " + ((Representation == null) ? "Aucune" : Representation) +
                ", RepresentationParticules: " + ((RepresentationParticules == null) ? "Aucune" : RepresentationParticules) +
                ", Vitesse: " + Vitesse +
                ", Priorite: " + Priorite +
                ", Position: " + Position +
                ", " + ((PeutAvoirCollecteur) ? "PeutAvoirCollecteur" : "NePeutPasAvoirCollecteur") +
                ", " + ((Selectionnable) ? "Selectionnable" : "NonSelectionnable") +
                ", " + ((Invincible) ? "Invincible" : "PasInvincible") +
                ", Taille: " + Taille.ToString("g") +
                ", PositionDepart: " + PositionDepart +
                ", Representations: ";

                if (Representations.Count == 0)
                    s += "Aucune";
                else
                    foreach (var rep in Representations)
                        s += rep + ", ";

                foreach (var emplacement in Emplacements)
                    s += emplacement;

                return s;
        }

        public void ajouterTourelle(TypeTourelle typeTourelle, int niveau, Vector3 position, bool visible)
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


    [Serializable]
    public class DescripteurVague
    {
        public double TempsDepart                       { get; set; }
        public List<DescripteurEnnemi> Ennemis          { get; set; }
        public List<Distance> Distances                 { get; set; }

        public DescripteurVague()
        {
            TempsDepart = 0;
            Ennemis = new List<DescripteurEnnemi>();
            Distances = new List<Distance>();
        }

        public void ajouter(double tempsDepart, DescripteurEnnemi ennemi, Distance distance, int quantite)
        {
            TempsDepart = tempsDepart;

            for (int i = 0; i < quantite; i++)
            {
                Ennemis.Add(ennemi);
                Distances.Add(distance);
            }
        }
    }


    [Serializable]
    public class DescripteurJoueur
    {
        public int ReserveUnites { get; set; }
        public int PointsDeVie { get; set; }

        public DescripteurJoueur()
        {
            ReserveUnites = 0;
            PointsDeVie = 1;
        }

        public override string ToString()
        {
            return
                "Joueur. " +
                "ReserveUnites: " + ReserveUnites +
                ", PointsDeVie: " + PointsDeVie + "\n";
        }
    }


    [Serializable]
    public class DescripteurTourelle
    {
        public TypeTourelle Type { get; set; }
        public bool PeutVendre { get; set; }
        public bool PeutMettreAJour { get; set; }
        public bool Visible { get; set; }


        [ContentSerializer(Optional = true)]
        public int Niveau { get; set; }

        public DescripteurTourelle()
        {
            Type = TypeTourelle.Inconnu;
            PeutVendre = true;
            PeutMettreAJour = true;
            Niveau = 1;
            Visible = true;
        }

        public override string ToString()
        {
            return
                "Tourelle. " +
                "Type: " + Type.ToString("g") +
                ", " + ((PeutVendre) ? "PeutVendre" : "NePeutPasVendre") +
                ", " + ((PeutMettreAJour) ? "PeutMettreAJour" : "NePeutPasMettreAJour") + "\n";
        }
    }


    [Serializable]
    public class DescripteurEmplacement
    {
        [ContentSerializer(Optional = true)]
        public DescripteurTourelle Tourelle { get; set; }

        public Vector3 Position { get; set; }
        public String Representation { get; set; }

        public DescripteurEmplacement()
        {
            Tourelle = null;
            Position = Vector3.Zero;
            Representation = "emplacement";
        }

        public override string ToString()
        {
            return
                "\nEmplacement. " +
                "Position: " + Position +
                ", Representation: " + Representation +
                ", " + ((Tourelle == null) ? "Aucune" : Tourelle.ToString()) + "\n";
        }
    }


    [Serializable]
    public class DescripteurEnnemi
    {
        public TypeEnnemi Type { get; set; }
        public int NiveauVitesse { get; set; }
        public int NiveauPointsVie { get; set; }
        public int Valeur { get; set; }
        public double PauseApres { get; set; }

        public DescripteurEnnemi()
        {
            Type = TypeEnnemi.Inconnu;
            NiveauVitesse = 1;
            NiveauPointsVie = 1;
            Valeur = 1;
        }

        public override string ToString()
        {
            return
                "\nEnnemi. " +
                "Type: " + Type.ToString("g") +
                ", Valeur: " + Valeur + "\n";
        }
    }
}
