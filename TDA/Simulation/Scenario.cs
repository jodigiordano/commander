namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Scenario
    {
        public int Numero;
        public string Mission;
        public string Annee;
        public string Lieu;
        public string Objectif;
        public string Description;
        public string Difficulte;
        public double ParTime;

        public List<CorpsCeleste> SystemePlanetaire;
        public VaguesInfinies VaguesInfinies;
        public LinkedList<Wave> Vagues;
        public CommonStash CommonStash;
        public List<Turret> Tourelles;
        public CorpsCeleste CorpsCelesteAProteger;
        public IVisible FondEcran;

        public int ValeurMinerauxDonnes;
        public Vector3 PourcentageMinerauxDonnes;
        public int NbPackViesDonnes;
        public List<string> HelpTexts;

        private Simulation Simulation;
        private float ProchainePrioriteAffichageCorpsCeleste = Preferences.PrioriteSimulationCorpsCeleste;
        private DescripteurScenario Descriptor;


        public Scenario(Simulation simulation, DescripteurScenario descripteur)
        {
            Simulation = simulation;
            Descriptor = descripteur;

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
            Tourelles = new List<Turret>();
            Vagues = new LinkedList<Wave>();

            CommonStash = new CommonStash();
            CommonStash.Lives = descripteur.Joueur.PointsDeVie;
            CommonStash.Cash = descripteur.Joueur.ReserveUnites;

            FondEcran = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(descripteur.FondEcran), Vector3.Zero);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.VisualPriority = Preferences.PrioriteFondEcran;

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
                    c.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(nomRepresentation(corpsCeleste.Taille, corpsCeleste.Representation)), Vector3.Zero);
                    c.Representation.Origine = c.Representation.Centre;
                    c.Representation.VisualPriority = c.ParticulesRepresentation.VisualPriority + 0.001f;

                    if (corpsCeleste.EnBackground)
                        c.Representation.Couleur.A = 60;
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
                       new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(nomRepresentation(corpsCeleste.Taille, corpsCeleste.Representation)), Vector3.Zero),
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
                        IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>(corpsCeleste.Representations[j]), Vector3.Zero);
                        iv.Origine = iv.Centre;
                        representations.Add(iv);
                    }

                    c = new AsteroidBelt
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
                    c.TourellesPermises = new List<Turret>();

                    for (int j = 0; j < corpsCeleste.TourellesPermises.Count; j++)
                        c.TourellesPermises.Add(Simulation.TurretFactory.CreateTurret(corpsCeleste.TourellesPermises[j]));
                }


                for (int j = 0; j < corpsCeleste.Emplacements.Count; j++)
                {
                    DescripteurEmplacement emplacement = corpsCeleste.Emplacements[j];

                    if (emplacement.Tourelle != null)
                    {
                        Turret turret = Simulation.TurretFactory.CreateTurret(emplacement.Tourelle.Type);
                        turret.CanSell = emplacement.Tourelle.PeutVendre;
                        turret.CanUpdate = emplacement.Tourelle.PeutMettreAJour;
                        turret.Level = emplacement.Tourelle.Niveau;
                        turret.BackActiveThisTickOverride = true;
                        turret.Visible = emplacement.Tourelle.Visible;
                        turret.CelestialBody = c;
                        turret.RelativePosition = emplacement.Position * 8;
                        turret.Position = c.Position + turret.RelativePosition;

                        c.Turrets.Add(turret);

                        Tourelles.Add(turret);
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
                for (int i = 0; i < descripteur.Waves.Count; i++)
                    Vagues.AddLast(new Wave(Simulation, descripteur.Waves[i]));

            ParTime = descripteur.ParTime;

            HelpTexts = descripteur.HelpTexts;
        }


        private String nomRepresentation(Taille taille, String nomBase)
        {
            return nomBase + ((taille == Taille.Petite) ? 1 : (taille == Taille.Moyenne) ? 2 : 3).ToString();
        }


        public int NbStars(int score)
        {
            return Descriptor.NbStars(score);
        }
    }
}
