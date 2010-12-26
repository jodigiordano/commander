namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;
    using Wintellect.PowerCollections;

    class GenerateurScenario
    {
        private static int[] TempsRotationPossible = new int[]
        {
            10000,
            30000,
            60000,
            120000,
            240000,
            360000,
            480000,
            600000,
            900000,
            1200000
        };

        private static Dictionary<Taille, RectanglePhysique> Cadres = new Dictionary<Taille, RectanglePhysique>()
        {
            { Taille.Petite,  new RectanglePhysique(-640 + 123, -370 + 112, 1098, 560) },
            { Taille.Moyenne, new RectanglePhysique(-640 + 147, -370 + 136, 1050, 512) },
            { Taille.Grande,  new RectanglePhysique(-640 + 179, -370 + 168,  986, 448) }
        };

        private static Dictionary<Taille, RectanglePhysique> EspaceGenerationEmplacements = new Dictionary<Taille,RectanglePhysique>()
        {
            { Taille.Petite,  new RectanglePhysique( -6,  -6, 12, 12) },
            { Taille.Moyenne, new RectanglePhysique(-9, -9, 18, 18) },
            { Taille.Grande,  new RectanglePhysique(-12, -12, 24, 24) }
        };

        private static Dictionary<Taille, Vector3> DistancesMin = new Dictionary<Taille, Vector3>()
        {
            { Taille.Petite,  new Vector3( 64,  92, 120) },
            { Taille.Moyenne, new Vector3( 92, 120, 144) },
            { Taille.Grande,  new Vector3(120, 144, 176) }
        };

        private static Dictionary<Taille, int> MaxEmplacements = new Dictionary<Taille, int>()
        {
            { Taille.Petite,  4 },
            { Taille.Moyenne, 10 },
            { Taille.Grande,  20 }
        };

        private static Dictionary<EnemyType, string> RepresentationsEnnemis = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Asteroid, "Asteroid" },
            { EnemyType.Centaur, "Centaur" },
            { EnemyType.Plutoid, "Plutoid" },
            { EnemyType.Comet, "Comet" },
            { EnemyType.Trojan, "Trojan" },
            { EnemyType.Meteoroid, "Meteoroid" }
        };


        public int NbCorpsCelestes;
        public int NbCorpsCelestesFixes;
        public List<TypeTourelle> TourellesDisponibles;
        public List<PowerUp> PowerUpsDisponibles;
        public bool AvecEtoileAuMilieu;
        public int NbPlanetesCheminDeDepart;
        public int ViesPlaneteAProteger;
        public int ArgentExtra;
        public int NbPacksVie;
        public int NbEmplacements;
        public int ArgentDepart;
        public List<EnemyType> EnnemisPresents;
        public DescripteurScenario DescripteurScenario;
        public bool SystemeCentre;

        private SimulationLite SimulationLite;
        private List<DescripteurCorpsCeleste> CorpsCelestes;

        public GenerateurScenario()
        {
            SimulationLite = new SimulationLite();
            CorpsCelestes = new List<DescripteurCorpsCeleste>();
        }

        public void genererGameplay()
        {
            DescripteurScenario.Annee = Main.Random.Next(2100, 2201).ToString();
            DescripteurScenario.FondEcran = "fondecran" + Main.Random.Next(1, 23);
            DescripteurScenario.Joueur.PointsDeVie = ViesPlaneteAProteger;
            DescripteurScenario.Joueur.ReserveUnites = ArgentDepart;
            DescripteurScenario.ValeurMinerauxDonnes = ArgentExtra;
            DescripteurScenario.NbPackViesDonnes = NbPacksVie;
            DescripteurScenario.Objectif = "Protect " + DescripteurScenario.CorpsCelesteAProteger;
        }

        public void genererSystemePlanetaire()
        {
            DescripteurScenario = FactoryScenarios.getDescripteurBidon();

            CorpsCelestes.Clear();

            genererCorpsCelestes();
            genererEmplacements();
            genererCheminDepart(); //doit etre fait avant les priorites pour un peu plus de random(awsom)nesssssssss
            genererPriorites();
            genererPlaneteAProteger();
            genererCeintureAsteroides();

            if (AvecEtoileAuMilieu)
                genererEtoile();
        }

        private void genererCorpsCelestes()
        {
            for (int i = 0; i < NbCorpsCelestes; i++)
            {
                DescripteurCorpsCeleste dcc = genererCorpsCeleste(i < NbCorpsCelestesFixes);

                dcc.PeutAvoirCollecteur = PowerUpsDisponibles.Contains(PowerUp.CollectTheRent);
                dcc.PeutAvoirDoItYourself = PowerUpsDisponibles.Contains(PowerUp.DoItYourself);
                dcc.PeutAvoirTheResistance = PowerUpsDisponibles.Contains(PowerUp.TheResistance);
                dcc.PeutDetruire = PowerUpsDisponibles.Contains(PowerUp.FinalSolution);

                dcc.TourellesPermises = TourellesDisponibles;

                CorpsCelestes.Add(dcc);

                DescripteurScenario.SystemePlanetaire.Add(dcc);
            }
        }

        public void genererCeintureAsteroides()
        {
            DescripteurCorpsCeleste ceinture = DescripteurScenario.SystemePlanetaire[0];

            ceinture.Position = new Vector3((Main.Random.Next(0, 2) == 0) ? 700 : -700, -400, 0);
            ceinture.Priorite = 0;
            ceinture.Vitesse = TempsRotationPossible[Main.Random.Next(TempsRotationPossible.Length / 2, TempsRotationPossible.Length)];
            ceinture.Representations = new List<string>();
            ceinture.PositionDepart = Main.Random.Next(0, 100);

            foreach (var ennemi in EnnemisPresents)
                ceinture.Representations.Add(RepresentationsEnnemis[ennemi]);
        }

        private void genererEtoile()
        {
            DescripteurCorpsCeleste c = new DescripteurCorpsCeleste();
            c.Nom = "Etoile";
            c.Position = Vector3.Zero;
            c.Taille = Taille.Grande;
            c.Representation = "planete2";
            c.RepresentationParticules = "etoile";
            c.Priorite = -1;
            c.TourellesPermises = new List<TypeTourelle>();
            c.EnBackground = true;
            c.Invincible = false;
            c.Selectionnable = false;
            c.Vitesse = 0;

            DescripteurScenario.SystemePlanetaire.Add(c);
        }

        private void genererPlaneteAProteger()
        {
            DescripteurCorpsCeleste aProteger = DescripteurScenario.SystemePlanetaire[DescripteurScenario.SystemePlanetaire.Count - 1];

            aProteger.Priorite = int.MaxValue;

            if (aProteger.Emplacements.Count == 0)
            {
                DescripteurEmplacement dE = new DescripteurEmplacement();
                dE.Representation = "emplacement";

                RectanglePhysique espaceGeneration = EspaceGenerationEmplacements[aProteger.Taille];

                dE.Position = new Vector3
                (
                    Main.Random.Next(espaceGeneration.X, espaceGeneration.X + espaceGeneration.Width),
                    Main.Random.Next(espaceGeneration.Y, espaceGeneration.Y + espaceGeneration.Height),
                    0
                );

                aProteger.Emplacements.Add(dE);
            }

            if (aProteger.Emplacements[0].Tourelle == null || aProteger.Emplacements[0].Tourelle.Type != TypeTourelle.Gravitationnelle)
            {
                DescripteurTourelle dT = new DescripteurTourelle();
                dT.Niveau = 1;
                dT.PeutMettreAJour = false;
                dT.PeutVendre = false;
                dT.Type = TypeTourelle.Gravitationnelle;
                //dT.Visible = false;

                aProteger.Emplacements[0].Tourelle = dT;
            }

            else
            {
                aProteger.Emplacements[0].Tourelle.PeutMettreAJour = false;
                aProteger.Emplacements[0].Tourelle.PeutVendre = false;
                //aProteger.Emplacements[0].Tourelle.Visible = false;
            }

            DescripteurScenario.CorpsCelesteAProteger = aProteger.Nom;
        }

        public List<Vector3> pointsConsideres()
        {
            return SimulationLite.pointsConsideres;
        }

        public List<Cercle> cerclesConsideres()
        {
            return SimulationLite.cerclesConsideres;
        }

        private DescripteurCorpsCeleste genererCorpsCeleste(bool fixe)
        {
            DescripteurCorpsCeleste d = new DescripteurCorpsCeleste();

            d.Nom = "Planete" + Main.Random.Next(0, int.MaxValue); //todo
            d.Invincible = false;
            d.EnBackground = false;
            d.PeutAvoirCollecteur = false;
            d.PeutAvoirDoItYourself = false;
            d.PeutAvoirTheResistance = false;
            d.PeutDetruire = false;

            d.Invincible = false;
            d.Selectionnable = true;
            d.TourellesPermises = null;
            d.Priorite = -1;

            d.Emplacements = new List<DescripteurEmplacement>();

            int nbTentatives = 0;

            RectanglePhysique rp = null;

            do
            {
                nbTentatives++;

                int taille = Main.Random.Next(0, 3);
                d.Taille = (taille == 0) ? Taille.Petite : (taille == 1) ? Taille.Moyenne : Taille.Grande;
                d.Representation = (fixe) ? "stationSpatiale" + Main.Random.Next(1, 3).ToString() :
                                            "planete" + Main.Random.Next(2, 8).ToString();

                //RepresentationParticules = null; //todo

                d.Vitesse = (fixe) ? 0 : TempsRotationPossible[Main.Random.Next(0, TempsRotationPossible.Length)];

                rp = Cadres[d.Taille];

                d.Position = new Vector3(Main.Random.Next(rp.X, rp.X + rp.Width), Main.Random.Next(rp.Y, rp.Y + rp.Height), 0);

                d.Offset = (SystemeCentre) ?
                    Vector3.Zero :
                    new Vector3(Main.Random.Next((int)(rp.X - d.Position.X + 200), (int)(rp.X + rp.Width - d.Position.X - 200)), Main.Random.Next((int)(rp.Y - d.Position.Y + 100), (int)(rp.Y + rp.Width - d.Position.Y - 100)), 0);

                d.Rotation = Main.Random.Next(-360, 360);
                d.PositionDepart = Main.Random.Next(0, 100);
            }
            while (!SimulationLite.dansLesBornes(d, rp) || SimulationLite.collisionPlanetaire(d, CorpsCelestes));

            Console.WriteLine("nb tentatives: " + nbTentatives);
            
            return d;
        }


        private void genererEmplacements()
        {

            // Distribuer les emplacements aux divers corps célestes

            Dictionary<int, int> IndiceQte = new Dictionary<int, int>();

            int nbEmplacementsEffectif = 0;

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                IndiceQte.Add(i, 0);
                nbEmplacementsEffectif += MaxEmplacements[CorpsCelestes[i].Taille];
            }

            nbEmplacementsEffectif = Math.Min(nbEmplacementsEffectif, NbEmplacements);

            for (int i = 0; i < nbEmplacementsEffectif; i++)
            {
                bool place = false;

                do
                {
                    int indice = Main.Random.Next(0, CorpsCelestes.Count);

                    if (IndiceQte[indice] < MaxEmplacements[CorpsCelestes[indice].Taille])
                    {
                        IndiceQte[indice] += 1;
                        place = true;
                    }
                }
                while (!place);
            }


            // Pour un corps céleste, distribuer ses emplacements en terme de positions relatives
            //List<DescripteurEmplacement> emplacements = new List<DescripteurEmplacement>();

            for (int i = 0; i < CorpsCelestes.Count; i++)
            {
                for (int j = 0; j < IndiceQte[i]; j++)
                {
                    DescripteurEmplacement emplacement = new DescripteurEmplacement();
                    emplacement.Representation = "emplacement";
                    
                    RectanglePhysique espaceGeneration = EspaceGenerationEmplacements[CorpsCelestes[i].Taille];

                    do
                    {
                        emplacement.Position = new Vector3
                        (
                            Main.Random.Next(espaceGeneration.X, espaceGeneration.X + espaceGeneration.Width),
                            Main.Random.Next(espaceGeneration.Y, espaceGeneration.Y + espaceGeneration.Height),
                            0
                        );
                    }
                    while (SimulationLite.emplacementTropProche(emplacement, CorpsCelestes[i].Emplacements));

                    CorpsCelestes[i].Emplacements.Add(emplacement);
                }
            }
        }

        private void genererPriorites()
        {
            int priorite = 1;

            CorpsCelestes.Sort(delegate(DescripteurCorpsCeleste corps1, DescripteurCorpsCeleste corps2)
            {
                if (corps1.Position.Length() > corps2.Position.Length())
                    return 1;

                if (corps1.Position.Length() < corps2.Position.Length())
                    return -1;

                return 0;
            });

            foreach (var corpsCeleste in CorpsCelestes)
                corpsCeleste.Priorite = priorite++;
        }

        private void genererCheminDepart()
        {
            int nbPlanetesCheminDepartEffectif = 0;

            foreach (var planete in CorpsCelestes)
            {
                if (planete.Emplacements.Count > 0)
                    nbPlanetesCheminDepartEffectif++;
            }

            nbPlanetesCheminDepartEffectif = Math.Min(nbPlanetesCheminDepartEffectif, NbPlanetesCheminDeDepart);

            List<DescripteurCorpsCeleste> corps = new List<DescripteurCorpsCeleste>(CorpsCelestes);

            while (corps.Count > 0 && nbPlanetesCheminDepartEffectif > 0)
            {
                int indice = Main.Random.Next(0, corps.Count);

                DescripteurCorpsCeleste corpsSelectionne = corps[indice];

                corps.RemoveAt(indice);

                if (corpsSelectionne.Emplacements.Count > 0)
                {
                    DescripteurTourelle dt = new DescripteurTourelle();
                    dt.Niveau = 1;
                    dt.PeutMettreAJour = true;
                    dt.PeutVendre = true;
                    dt.Type = TypeTourelle.Gravitationnelle;

                    corpsSelectionne.Emplacements[0].Tourelle = dt;

                    nbPlanetesCheminDepartEffectif--;
                }
            }

            //for (int i = 0; i < CorpsCelestes.Count && nbPlanetesCheminDepartEffectif > 0; i++)
            //{
            //    if (CorpsCelestes[i].Emplacements.Count > 0)
            //    {
            //        DescripteurTourelle dt = new DescripteurTourelle();
            //        dt.Niveau = 1;
            //        dt.PeutMettreAJour = true;
            //        dt.PeutVendre = true;
            //        dt.Type = TypeTourelle.Gravitationnelle;

            //        CorpsCelestes[i].Emplacements[0].Tourelle = dt;

            //        nbPlanetesCheminDepartEffectif--;
            //    }
            //}
        }
    }
}
