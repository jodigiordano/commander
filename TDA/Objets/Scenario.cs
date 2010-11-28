namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;

    class Scenario
    {
        public int Numero;
        public String Mission;
        public String Annee;
        public String Lieu;
        public String Objectif;
        public String Description;
        public String Image;
        public String Difficulte;

        public List<CorpsCeleste> SystemePlanetaire;
        public VaguesInfinies VaguesInfinies;
        public LinkedList<Vague> Vagues;
        public JoueurCommun Joueur;
        public List<Tourelle> Tourelles;
        public CorpsCeleste CorpsCelesteAProteger;
        public IVisible FondEcran;

        public int ValeurMinerauxDonnes;
        public Vector3 PourcentageMinerauxDonnes;
        public int NbPackViesDonnes;

        private Simulation Simulation;
        private float ProchainePrioriteAffichageCorpsCeleste = Preferences.PrioriteSimulationCorpsCeleste;

        public Scenario(Simulation simulation, DescripteurScenario descripteur)
        {
            this.Simulation = simulation;

            Mission = descripteur.Mission;
            Annee = descripteur.Annee;
            Lieu = descripteur.Lieu;
            Objectif = descripteur.Objectif;
            Description = descripteur.Description;
            Difficulte = descripteur.Difficulte;
            Numero = descripteur.Numero;

            ValeurMinerauxDonnes = descripteur.ValeurMinerauxDonnes;
            PourcentageMinerauxDonnes = descripteur.PourcentageMinerauxDonnes;
            NbPackViesDonnes = descripteur.NbPackViesDonnes;

            SystemePlanetaire = new List<CorpsCeleste>();
            Tourelles = new List<Tourelle>();
            Vagues = new LinkedList<Vague>();

            Joueur = new JoueurCommun();
            Joueur.PointsDeVie = descripteur.Joueur.PointsDeVie;
            Joueur.ReserveUnites = descripteur.Joueur.ReserveUnites;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(descripteur.FondEcran), Vector3.Zero, Simulation.Scene);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = Preferences.PrioriteFondEcran;

            for (int i = 0; i < descripteur.SystemePlanetaire.Count; i++)
            {
                DescripteurCorpsCeleste corpsCeleste = descripteur.SystemePlanetaire[i];

                CorpsCeleste c;

                // Trou Rose
                if (corpsCeleste.Representation == null &&
                    corpsCeleste.RepresentationParticules != null &&
                    corpsCeleste.RepresentationParticules == "trouRose")
                {
                    c = new TrouRose
                    (
                       Simulation,
                       corpsCeleste.Nom,
                       corpsCeleste.Position,
                       corpsCeleste.Offset,
                       (int)corpsCeleste.Taille,
                       corpsCeleste.Vitesse,
                       Simulation.Scene.Particules.recuperer(corpsCeleste.RepresentationParticules),
                       corpsCeleste.PositionDepart,
                       ProchainePrioriteAffichageCorpsCeleste -= 0.001f,
                       corpsCeleste.EnBackground,
                       corpsCeleste.Rotation
                    );

                }

                // Soleil
                else if (corpsCeleste.Representation == null && corpsCeleste.RepresentationParticules != null)
                {
                    c = new CorpsCeleste
                    (
                       Simulation,
                       corpsCeleste.Nom,
                       corpsCeleste.Position,
                       corpsCeleste.Offset,
                       (int)corpsCeleste.Taille,
                       corpsCeleste.Vitesse,
                       Simulation.Scene.Particules.recuperer(corpsCeleste.RepresentationParticules),
                       corpsCeleste.PositionDepart,
                       ProchainePrioriteAffichageCorpsCeleste -= 0.001f,
                       corpsCeleste.EnBackground,
                       corpsCeleste.Rotation
                    );

                }

                // Corps gazeux
                else if (corpsCeleste.Representation != null && corpsCeleste.RepresentationParticules != null)
                {
                    c = new CorpsCeleste
                    (
                        Simulation,
                        corpsCeleste.Nom,
                        corpsCeleste.Position,
                        corpsCeleste.Offset,
                        (int)corpsCeleste.Taille,
                        corpsCeleste.Vitesse,
                        Simulation.Scene.Particules.recuperer(corpsCeleste.RepresentationParticules),
                        corpsCeleste.PositionDepart, ProchainePrioriteAffichageCorpsCeleste -= 0.001f,
                        corpsCeleste.EnBackground,
                       corpsCeleste.Rotation
                    );
                    c.representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(nomRepresentation(corpsCeleste.Taille, corpsCeleste.Representation)), Vector3.Zero, Simulation.Scene);
                    c.representation.Origine = c.representation.Centre;
                    c.representation.PrioriteAffichage = c.representationParticules.PrioriteAffichage + 0.001f;

                    if (corpsCeleste.EnBackground)
                        c.representation.Couleur.A = 60;
                }

                // Corps normal
                else if (corpsCeleste.Representation != null)
                {
                    c = new CorpsCeleste
                    (
                       Simulation,
                       corpsCeleste.Nom,
                       corpsCeleste.Position,
                       corpsCeleste.Offset,
                       (int)corpsCeleste.Taille,
                       corpsCeleste.Vitesse,
                       new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(nomRepresentation(corpsCeleste.Taille, corpsCeleste.Representation)), Vector3.Zero, Simulation.Scene),
                       corpsCeleste.PositionDepart,
                       ProchainePrioriteAffichageCorpsCeleste -= 0.001f,
                       corpsCeleste.EnBackground,
                       corpsCeleste.Rotation
                    );
                }

                // Ceinture d'asteroides
                else
                {
                    List<IVisible> representations = new List<IVisible>();

                    for (int j = 0; j < corpsCeleste.Representations.Count; j++)
                    {
                        IVisible iv = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(corpsCeleste.Representations[j]), Vector3.Zero, Simulation.Scene);
                        iv.Origine = iv.Centre;
                        representations.Add(iv);
                    }

                    c = new CeintureAsteroide
                    (
                        Simulation,
                        corpsCeleste.Nom,
                        corpsCeleste.Position,
                        (int)corpsCeleste.Taille,
                        corpsCeleste.Vitesse,
                        representations,
                        corpsCeleste.PositionDepart
                    );
                }

                c.PeutAvoirCollecteur = corpsCeleste.PeutAvoirCollecteur;
                c.PeutAvoirDoItYourself = corpsCeleste.PeutAvoirDoItYourself;
                c.PeutAvoirTheResistance = corpsCeleste.PeutAvoirTheResistance;
                c.PeutDetruire = corpsCeleste.PeutDetruire;
                c.Priorite = corpsCeleste.Priorite;
                c.Selectionnable = corpsCeleste.Selectionnable;
                c.Invincible = corpsCeleste.Invincible;

                if (corpsCeleste.TourellesPermises != null)
                {
                    c.TourellesPermises = new List<Tourelle>();

                    for (int j = 0; j < corpsCeleste.TourellesPermises.Count; j++)
                        c.TourellesPermises.Add(FactoryTourelles.creerTourelle(simulation, corpsCeleste.TourellesPermises[j]));
                }

                Color couleurEmplacement = new Color(Emplacement.CouleursDisponibles[Main.Random.Next(0, Emplacement.CouleursDisponibles.Length)], 200);

                for (int j = 0; j < corpsCeleste.Emplacements.Count; j++)
                {
                    DescripteurEmplacement emplacement = corpsCeleste.Emplacements[j];

                    Emplacement e = new Emplacement
                    (
                        Simulation,
                        emplacement.Position * 8,
                        new IVisible(Core.Persistance.Facade.recuperer<Texture2D>(emplacement.Representation), Vector3.Zero, Simulation.Scene),
                        c
                    );

                    e.Filtre.Couleur = couleurEmplacement;

                    c.Emplacements.Add(e);

                    if (emplacement.Tourelle != null)
                    {
                        e.Tourelle = FactoryTourelles.creerTourelle(simulation, emplacement.Tourelle.Type);
                        e.Tourelle.PeutVendre = emplacement.Tourelle.PeutVendre;
                        e.Tourelle.PeutMettreAJour = emplacement.Tourelle.PeutMettreAJour;
                        e.Tourelle.Niveau = emplacement.Tourelle.Niveau;
                        e.Tourelle.AnnonciationActiveDeNouveauOverride = true;
                        e.Tourelle.Visible = emplacement.Tourelle.Visible;

                        Tourelles.Add(e.Tourelle);
                    }
                }

                if (corpsCeleste.Nom == descripteur.CorpsCelesteAProteger)
                {
                    c.PointsVie = descripteur.Joueur.PointsDeVie;
                    CorpsCelesteAProteger = c;
                }

                SystemePlanetaire.Add(c);
            }



            if (descripteur.VaguesInfinies != null)
            {
                VaguesInfinies = new VaguesInfinies(Simulation, descripteur.VaguesInfinies);
                Vagues.AddLast(VaguesInfinies.getProchaineVague());
            }

            else
                for (int i = 0; i < descripteur.Vagues.Count; i++)
                    Vagues.AddLast(new Vague(Simulation, descripteur.Vagues[i]));
        }

        private String nomRepresentation(Taille taille, String nomBase)
        {
            return nomBase + ((taille == Taille.Petite) ? 1 : (taille == Taille.Moyenne) ? 2 : 3).ToString();
        }
    }
}
