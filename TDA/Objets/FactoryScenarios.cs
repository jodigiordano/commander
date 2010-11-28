namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using System.Xml.Serialization;
    using System.IO;
    using System.Reflection;
    using Microsoft.Xna.Framework.Storage;

    static class FactoryScenarios
    {
        public static Dictionary<String, DescripteurScenario> getDescriptionsScenariosMonde1()
        {
            //todo
            Dictionary<String, DescripteurScenario> resultats = new Dictionary<String, DescripteurScenario>();

            DescripteurScenario d;

            XmlSerializer serializer = new XmlSerializer(typeof(DescripteurScenario));

            for (int i = 1; i < 10; i++)
                using (StreamReader reader = new StreamReader(StorageContainer.TitleLocation + "\\Content\\scenarios\\scenario" + i + ".xml"))
                {
                    d = (DescripteurScenario)serializer.Deserialize(reader.BaseStream);
                    resultats.Add(d.Mission, d);
                }

            d = new DescripteurScenario();
            d.Numero = 9;
            d.Mission = "Go to World 2!";
            d.Difficulte = "";
            resultats.Add(d.Mission, d);

            return resultats;
        }

        public static Dictionary<String, DescripteurScenario> getDescriptionsScenariosMonde2()
        {
            //todo
            Dictionary<String, DescripteurScenario> resultats = new Dictionary<String, DescripteurScenario>();

            DescripteurScenario d;

            d = new DescripteurScenario();
            d.Numero = 10;
            d.Mission = "2-1";
            d.Difficulte = "Easy";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 11;
            d.Mission = "2-2";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 12;
            d.Mission = "2-3";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 13;
            d.Mission = "2-4";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 14;
            d.Mission = "2-5";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 15;
            d.Mission = "2-6";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 16;
            d.Mission = "2-7";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 17;
            d.Mission = "2-8";
            d.Difficulte = "Easy";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 18;
            d.Mission = "2-9";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 19;
            d.Mission = "Go to World 3!";
            d.Difficulte = "";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 20;
            d.Mission = "Go back\nto World 1!";
            d.Difficulte = "";
            resultats.Add(d.Mission, d);

            return resultats;
        }

        public static Dictionary<String, DescripteurScenario> getDescriptionsScenariosMonde3()
        {
            //todo
            Dictionary<String, DescripteurScenario> resultats = new Dictionary<String, DescripteurScenario>();

            DescripteurScenario d;

            d = new DescripteurScenario();
            d.Numero = 21;
            d.Mission = "3-1";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 22;
            d.Mission = "3-2";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 23;
            d.Mission = "3-3";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 24;
            d.Mission = "3-4";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 25;
            d.Mission = "3-5";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 26;
            d.Mission = "3-6";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 27;
            d.Mission = "3-7";
            d.Difficulte = "Normal";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 28;
            d.Mission = "3-8";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 29;
            d.Mission = "3-9";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 30;
            d.Mission = "3-10";
            d.Difficulte = "Hard";
            resultats.Add(d.Mission, d);

            d = new DescripteurScenario();
            d.Numero = 31;
            d.Mission = "Go back\nto World\n2!";
            d.Difficulte = "";
            resultats.Add(d.Mission, d);

            return resultats;
        }


        public static LinkedList<DescripteurScenario> getDescriptionsScenarios()
        {
            LinkedList<DescripteurScenario> resultats = new LinkedList<DescripteurScenario>();

            XmlSerializer serializer = new XmlSerializer(typeof(DescripteurScenario));

            String repertoire =
                Path.Combine(StorageContainer.TitleLocation + "\\",
                Path.Combine("Content", "scenarios"));

            String[] fichiers = Directory.GetFiles(repertoire, "*.xml");

            foreach(var fichier in fichiers)
                using (StreamReader reader = new StreamReader(fichier))
                    resultats.AddLast((DescripteurScenario)serializer.Deserialize(reader.BaseStream));

            return resultats;
        }


        public static DescripteurScenario getDescripteurMenu()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran19";
            //d.FondEcran = "fondecran23";

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-300, -150, 0), "save the\nworld", "planete2", 0, 100);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), true);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Base, 5, new Vector3(10, -8, 0), true);

            d.CorpsCelesteAProteger = d.SystemePlanetaire[0].Nom;

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(300, -220, 0), "options", "planete4", 0, 99);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(3, 2, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Base, 8, new Vector3(-9, -5, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.LaserMultiple, 4, new Vector3(22, 0, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-50, 150, 0), "resume game", "planete3", 0, 98);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(4, 2, 0), true);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.LaserSimple, 7, new Vector3(3, -7, 0), true);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Missile, 3, new Vector3(-6, 14, 0), true);


            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-400, 200, 0), "help", "planete6", 0, 97);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(2, 1, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(350, 200, 0), "quit", "planete5", 0, 96);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(-5, 3, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.SlowMotion, 6, new Vector3(6, 3, 0), true);

            c = new DescripteurCorpsCeleste();
            c.Nom = "editor";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 320000;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Representations.Add("Plutoid");
            c.Representations.Add("Comet");
            c.Representations.Add("Centaur");
            c.Representations.Add("Trojan");
            c.Representations.Add("Meteoroid");
            c.Priorite = 1;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = true;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.GravitationnelleAlien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);


            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 12;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<TypeEnnemi>();
            v.EnnemisPresents.Add(TypeEnnemi.Asteroid);
            v.EnnemisPresents.Add(TypeEnnemi.Comet);
            v.EnnemisPresents.Add(TypeEnnemi.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }


        public static DescripteurScenario getDescripteurTestsPerformance()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            DescripteurVague v;
            DescripteurEnnemi en;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran14";

            for (int i = 0; i < 6; i++)
            {
                c = new DescripteurCorpsCeleste();
                c.Nom = i.ToString();
                c.Position = new Vector3(150, 100, 0);
                c.PositionDepart = (int) (i * 14f);
                c.Taille = Taille.Petite;
                c.Vitesse = 120000;
                c.Representation = "stationSpatiale1";
                c.Priorite = 30 - i;
                c.Invincible = true;
                e = new DescripteurEmplacement();
                e.Position = new Vector3(-6, 0, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Gravitationnelle;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Base;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Base;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                d.SystemePlanetaire.Add(c);
            }

            d.CorpsCelesteAProteger = d.SystemePlanetaire[0].Nom;

            for (int i = 0; i < 8; i++)
            {
                c = new DescripteurCorpsCeleste();
                c.Nom = (i+8).ToString();
                c.Position = new Vector3(300, 200, 0);
                c.PositionDepart = (int)(i * 11f);
                c.Taille = Taille.Petite;
                c.Vitesse = 120000;
                c.Representation = "stationSpatiale2";
                c.Priorite = 24 - i;
                c.Invincible = true;
                e = new DescripteurEmplacement();
                e.Position = new Vector3(-6, 0, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Gravitationnelle;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.LaserMultiple;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.LaserMultiple;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                d.SystemePlanetaire.Add(c);
            }

            for (int i = 0; i < 16; i++)
            {
                c = new DescripteurCorpsCeleste();
                c.Nom = (i+16).ToString();
                c.Position = new Vector3(450, 300, 0);
                c.PositionDepart = (int)(i * 5f);
                c.Taille = Taille.Petite;
                c.Vitesse = 120000;
                c.Representation = "stationSpatiale1";
                c.Priorite = 16 - i;
                c.Invincible = true;
                e = new DescripteurEmplacement();
                e.Position = new Vector3(-6, 0, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Gravitationnelle;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Base;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TypeTourelle.Base;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                d.SystemePlanetaire.Add(c);
            }


            c = new DescripteurCorpsCeleste();
            c.Nom = "1111";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 320000;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Representations.Add("Plutoid");
            c.Representations.Add("Comet");
            c.Representations.Add("Centaur");
            c.Representations.Add("Trojan");
            c.Representations.Add("Meteoroid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = true;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.GravitationnelleAlien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            v = new DescripteurVague();
            en = new DescripteurEnnemi();
            en.Type = TypeEnnemi.Plutoid;
            en.NiveauPointsVie = 150;
            v.ajouter(0, en, Distance.Proche, 100);
            d.Vagues.Add(v);

            v = new DescripteurVague();
            en = new DescripteurEnnemi();
            en.Type = TypeEnnemi.Comet;
            en.NiveauPointsVie = 150;
            v.ajouter(0, en, Distance.Proche, 100);
            d.Vagues.Add(v);

            v = new DescripteurVague();
            en = new DescripteurEnnemi();
            en.Type = TypeEnnemi.Meteoroid;
            en.NiveauPointsVie = 150;
            v.ajouter(0, en, Distance.Proche, 100);
            d.Vagues.Add(v);

            return d;
        }


        public static DescripteurScenario getDescripteurBidon()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            DescripteurVague v;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran19";

            //c = new DescripteurCorpsCeleste();
            //c.Nom = "Etoile";
            //c.Position = Vector3.Zero;
            //c.Taille = Taille.Grande;
            //c.Representation = "planete2";
            //c.RepresentationParticules = "etoile";
            //c.Priorite = -1;
            //c.TourellesPermises = new List<TypeTourelle>();
            //d.SystemePlanetaire.Add(c);

            //c = new DescripteurCorpsCeleste();
            //c.Nom = "Planete";
            //c.Position = new Vector3(150, 120, 0);
            //c.PositionDepart = 40;
            //c.Taille = Taille.Moyenne;
            //c.Vitesse = 80000;
            //c.Representation = "planete3";
            //c.Priorite = 100;
            //c.Invincible = false;
            //e = new DescripteurEmplacement();
            //e.Position = new Vector3(3, -2, 0);
            //e.Tourelle = new DescripteurTourelle();
            //e.Tourelle.Type = TypeTourelle.Gravitationnelle;
            //e.Tourelle.PeutVendre = false;
            //e.Tourelle.PeutMettreAJour = false;
            //c.Emplacements.Add(e);
            //d.SystemePlanetaire.Add(c);

            //d.CorpsCelesteAProteger = c.Nom;

            c = new DescripteurCorpsCeleste();
            c.Nom = "CeintureAsteroide";
            c.Invincible = true;
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 50000;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Priorite = 1;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.Gravitationnelle;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);

            d.CorpsCelesteAProteger = c.Nom;

            d.SystemePlanetaire.Add(c);

            v = new DescripteurVague();
            v.ajouter(double.MaxValue, new DescripteurEnnemi(), Distance.Normal, 30);

            d.Vagues.Add(v);

            return d;
        }

        public static DescripteurScenario getDescripteurMonde1()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            DescripteurEnnemi en;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran17";

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-300, -200, 0), "1-1", "planete6", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Base, 5, new Vector3(15, -8, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-500, -50, 0), "1-2", "planete7", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Base, 3, new Vector3(-10, 8, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Base, 3, new Vector3(-8, 16, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Base, 3, new Vector3(-4, 24, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-400, 150, 0), "1-3", "planete1", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            
            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-150, 30, 0), "1-4", "planete2", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.LaserSimple, 5, new Vector3(-12, 10, 0), true);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.LaserSimple, 5, new Vector3(15, -12, 0), true);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.LaserSimple, 5, new Vector3(0, -18, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(0, 250, 0), "1-5", "planete3", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(100, 125, 0), "1-6", "planete4", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[5].ajouterTourelle(TypeTourelle.LaserMultiple, 1, new Vector3(5, 5, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(400, 200, 0), "1-7", "planete5", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.Gravitationnelle, 2, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.Missile, 2, new Vector3(-20, -12, 0), true);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.SlowMotion, 4, new Vector3(-8, -22, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(450, -100, 0), "1-8", "planete6", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            
            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(200, -150, 0), "1-9", "planete7", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.SlowMotion, 4, new Vector3(12, -5, 0), true);

            d.ajouterTrouRose(new Vector3(-50, -220, 0), "Go to World 2!", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.CorpsCelesteAProteger = "Go to World 2!";

            c = new DescripteurCorpsCeleste();
            c.Nom = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 2560000;
            c.PositionDepart = 40;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Representations.Add("Plutoid");
            c.Representations.Add("Comet");
            c.Representations.Add("Centaur");
            c.Representations.Add("Trojan");
            c.Representations.Add("Meteoroid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.GravitationnelleAlien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 10;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<TypeEnnemi>();
            v.EnnemisPresents.Add(TypeEnnemi.Asteroid);
            v.EnnemisPresents.Add(TypeEnnemi.Comet);
            v.EnnemisPresents.Add(TypeEnnemi.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }

        public static DescripteurScenario getDescripteurMonde2()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            DescripteurEnnemi en;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran16";

            d.ajouterTrouRose(new Vector3(0, 300, 0), "Go back\nto World 1!", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-150, 200, 0), "2-1", "planete6", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(250, 250, 0), "2-2", "planete7", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Base, 6, new Vector3(-10, -5, 0), true);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Base, 7, new Vector3(20, 6, 0), true);


            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(0, 100, 0), "2-3", "planete1", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-300, 175, 0), "2-4", "planete2", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Missile, 5, new Vector3(-18, -20, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.LaserMultiple, 6, new Vector3(-18, -8, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.SlowMotion, 4, new Vector3(17, -15, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(0, -75, 0), "2-5", "planete3", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(250, 50, 0), "2-6", "planete4", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.Missile, 8, new Vector3(-6, -25, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(400, -150, 0), "2-7", "planete5", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 5, new Vector3(24, 0, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 6, new Vector3(20, 8, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 7, new Vector3(18, 16, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 8, new Vector3(10, 24, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 9, new Vector3(0, 32, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.LaserSimple, 10, new Vector3(0, 40, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(100, -220, 0), "2-8", "planete6", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-300, -100, 0), "2-9", "planete7", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterTrouRose(new Vector3(-125, -200, 0), "Go to World 3!", 0, 11);
            d.SystemePlanetaire[10].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.CorpsCelesteAProteger = "Go to World 3!";

            c = new DescripteurCorpsCeleste();
            c.Nom = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 2560000;
            c.PositionDepart = 75;
            c.Taille = Taille.Petite;
            c.Representations.Add("Plutoid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.GravitationnelleAlien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 40;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<TypeEnnemi>();
            v.EnnemisPresents.Add(TypeEnnemi.Asteroid);
            v.EnnemisPresents.Add(TypeEnnemi.Comet);
            v.EnnemisPresents.Add(TypeEnnemi.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }

        public static DescripteurScenario getDescripteurMonde3()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            DescripteurEnnemi en;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran15";

            d.ajouterTrouRose(new Vector3(500, 0, 0), "Go back\nto World\n2!", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(450, -200, 0), "3-1", "stationSpatiale1", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[1].ajouterTourelle(TypeTourelle.LaserMultiple, 10, new Vector3(-20, -15, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(350, 200, 0), "3-2", "stationSpatiale1", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(250, 0, 0), "3-3", "stationSpatiale2", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(150, -200, 0), "3-4", "stationSpatiale1", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Base, 10, new Vector3(-17, -13, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Base, 10, new Vector3(-19, 0, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TypeTourelle.Base, 10, new Vector3(-3, 15, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(50, 200, 0), "3-5", "stationSpatiale2", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-50, 0, 0), "3-6", "stationSpatiale2", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-150, -200, 0), "3-7", "stationSpatiale1", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-200, 225, 0), "3-8", "stationSpatiale1", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.Missile, 10, new Vector3(-25, 0, 0), true);
            d.SystemePlanetaire[8].ajouterTourelle(TypeTourelle.Missile, 10, new Vector3(-18, -8, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-300, 30, 0), "3-9", "stationSpatiale2", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[9].ajouterTourelle(TypeTourelle.LaserMultiple, 10, new Vector3(-20, 0, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-400, -160, 0), "3-10", "planete1", 0, 11);
            d.SystemePlanetaire[10].ajouterTourelle(TypeTourelle.Gravitationnelle, 1, new Vector3(1, -2, 0), false);


            d.CorpsCelesteAProteger = "3-10";

            c = new DescripteurCorpsCeleste();
            c.Nom = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 2560000;
            c.PositionDepart = 0;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TypeTourelle>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TypeTourelle.GravitationnelleAlien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 70;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<TypeEnnemi>();
            v.EnnemisPresents.Add(TypeEnnemi.Asteroid);
            v.EnnemisPresents.Add(TypeEnnemi.Comet);
            v.EnnemisPresents.Add(TypeEnnemi.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }
    }
}
