namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    static class FactoryScenarios
    {
        public static WorldDescriptor GetWorldDescriptor(int id)
        {
            WorldDescriptor wd;

            switch (id)
            {
                case 1:
                default:
                    wd = new WorldDescriptor()
                    {
                        Id = 1,
                        Levels = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        Warps = new List<KeyValuePair<int,int>>() { new KeyValuePair<int, int>(9, 2) },
                        SimulationDescription = FactoryScenarios.getDescripteurMonde1(),
                        UnlockedCondition = new List<int>(),
                        WarpBlockedMessage = "You're not Commander\n\nenough to ascend to\n\na higher level."
                    };
                    break;

                case 2:
                    wd = new WorldDescriptor()
                    {
                        Id = 2,
                        Levels = new List<int>() { 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                        Warps = new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(19, 3), new KeyValuePair<int, int>(20, 1) },
                        SimulationDescription = FactoryScenarios.getDescripteurMonde2(),
                        UnlockedCondition = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        WarpBlockedMessage = "Only a true Commander\n\nmay enjoy a better world."
                    };
                    break;

                case 3:
                    wd = new WorldDescriptor()
                    {
                        Id = 3,
                        Levels = new List<int>() { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                        Warps = new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(31, 2) },
                        SimulationDescription = FactoryScenarios.getDescripteurMonde3(),
                        UnlockedCondition = new List<int>() { -1 },
                        WarpBlockedMessage = ""
                    };
                    break;
            }


            return wd;
        }


        public static DescripteurScenario GetLevelScenario(int id)
        {
            DescripteurScenario d;

            if (id == 9)
            {
                d = new DescripteurScenario();
                d.Numero = 9;
                d.Mission = "Go to World 2!";
                d.Difficulte = "";

                return d;
            }

            else if (id == 19)
            {
                d = new DescripteurScenario();
                d.Numero = 19;
                d.Mission = "Go to World 3!";
                d.Difficulte = "";

                return d;
            }

            else if (id == 20)
            {
                d = new DescripteurScenario();
                d.Numero = 20;
                d.Mission = "Go back\nto World 1!";
                d.Difficulte = "";

                return d;
            }

            else if (id == 31)
            {
                d = new DescripteurScenario();
                d.Numero = id;
                d.Mission = "Go back\nto World\n2!";
                d.Difficulte = "";

                return d;
            }

            else if (id >= 10 && id <= 18)
            {
                d = new DescripteurScenario();
                d.Numero = id;
                d.Mission = "2-" + (id - 9);
                d.Difficulte = (id == 10 || id == 17) ? "Easy" : (id == 11 || id == 12 || id == 13 || id == 14) ? "Normal" : "Hard";

                return d;
            }

            else if (id >= 21 && id <= 30)
            {
                d = new DescripteurScenario();
                d.Numero = id;
                d.Mission = "3-" + (id - 20);
                d.Difficulte = (id == 10 || id == 17) ? "Easy" : (id == 11 || id == 12 || id == 13 || id == 14) ? "Normal" : "Hard";

                return d;
            }

            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DescripteurScenario));

                using (StreamReader reader = new StreamReader(".\\Content\\scenarios\\scenario" + id + ".xml"))
                    d = (DescripteurScenario) serializer.Deserialize(reader.BaseStream);

                return d;
            }

            return null;
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
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Basic, 5, new Vector3(10, -14, 0), true);

            d.CorpsCelesteAProteger = d.SystemePlanetaire[0].Nom;

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(300, -220, 0), "options", "planete4", 0, 99);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(3, 2, 0), false);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Basic, 8, new Vector3(-20, -5, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-50, 150, 0), "editor", "planete3", 0, 98);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(4, 2, 0), false);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Laser, 7, new Vector3(3, -7, 0), true);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Missile, 3, new Vector3(-8, 0, 0), true);


            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-400, 200, 0), "help", "planete6", 0, 97);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(2, 1, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(350, 200, 0), "quit", "planete5", 0, 96);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(-5, 3, 0), false);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.SlowMotion, 6, new Vector3(-10, -3, 0), true);

            c = new DescripteurCorpsCeleste();
            c.Nom = "whatever";
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
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Alien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);


            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 12;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<EnemyType>();
            v.EnnemisPresents.Add(EnemyType.Asteroid);
            v.EnnemisPresents.Add(EnemyType.Comet);
            v.EnnemisPresents.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }


        public static DescripteurScenario getDescripteurTestsPerformance()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            WaveDescriptor v;

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
                e.Tourelle.Type = TurretType.Gravitational;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.Basic;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.Basic;
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
                e.Tourelle.Type = TurretType.Gravitational;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.MultipleLasers;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.MultipleLasers;
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
                e.Tourelle.Type = TurretType.Gravitational;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, 6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.Basic;
                e.Tourelle.Niveau = 10;
                c.Emplacements.Add(e);
                e = new DescripteurEmplacement();
                e.Position = new Vector3(0, -6, 0);
                e.Tourelle = new DescripteurTourelle();
                e.Tourelle.Type = TurretType.Basic;
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
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = true;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Alien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Plutoid };
            v.LivesLevel = 150;
            d.Waves.Add(v);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Comet };
            v.LivesLevel = 150;
            v.Quantity = 100;
            d.Waves.Add(v);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Meteoroid };
            v.LivesLevel = 150;
            v.Quantity = 100;
            d.Waves.Add(v);

            return d;
        }


        public static DescripteurScenario getDescripteurBidon()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;
            WaveDescriptor v;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran19";

            c = new DescripteurCorpsCeleste();
            c.Nom = "CeintureAsteroide";
            c.Invincible = true;
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 50000;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Priorite = 1;
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Gravitational;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);

            d.CorpsCelesteAProteger = c.Nom;

            d.SystemePlanetaire.Add(c);

            d.Waves.Add(new WaveDescriptor());
            d.Waves[0].Enemies.Add(EnemyType.Asteroid);
            d.Waves[0].Quantity = 50;

            return d;
        }


        public static DescripteurScenario getDescripteurMonde1()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran17";

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-300, -200, 0), "1-1", "planete6", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Basic, 5, new Vector3(5, -4, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-450, -50, 0), "1-2", "planete7", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Basic, 3, new Vector3(0, 8, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Basic, 3, new Vector3(6, 16, 0), true);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Basic, 3, new Vector3(-6, 24, 0), true);
            
            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-400, 150, 0), "1-3", "planete1", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            
            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-150, 30, 0), "1-4", "planete2", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Laser, 5, new Vector3(-3, 10, 0), true);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Laser, 5, new Vector3(10, -6, 0), true);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Laser, 5, new Vector3(0, -10, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(0, 200, 0), "1-5", "planete3", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(100, 75, 0), "1-6", "planete4", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[5].ajouterTourelle(TurretType.MultipleLasers, 1, new Vector3(5, 5, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(400, 150, 0), "1-7", "planete5", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.Gravitational, 2, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.Missile, 2, new Vector3(-14, -9, 0), true);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.SlowMotion, 4, new Vector3(0, -12, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(450, -150, 0), "1-8", "planete6", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            
            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(200, -200, 0), "1-9", "planete7", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.SlowMotion, 4, new Vector3(12, 2, 0), true);

            d.ajouterTrouRose(new Vector3(-50, -220, 0), "Go to World 2!", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

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
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Alien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 10;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<EnemyType>();
            v.EnnemisPresents.Add(EnemyType.Asteroid);
            v.EnnemisPresents.Add(EnemyType.Comet);
            v.EnnemisPresents.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }


        public static DescripteurScenario getDescripteurMonde2()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran16";

            d.ajouterTrouRose(new Vector3(0, 300, 0), "Go back\nto World 1!", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(-150, 200, 0), "2-1", "planete6", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(250, 250, 0), "2-2", "planete7", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Basic, 6, new Vector3(-10, -5, 0), true);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Basic, 7, new Vector3(20, 6, 0), true);


            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(0, 100, 0), "2-3", "planete1", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-300, 175, 0), "2-4", "planete2", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Missile, 5, new Vector3(-18, -20, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.MultipleLasers, 6, new Vector3(-18, -8, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.SlowMotion, 4, new Vector3(17, -15, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(0, -75, 0), "2-5", "planete3", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(250, 50, 0), "2-6", "planete4", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.Missile, 8, new Vector3(-6, -25, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(400, -150, 0), "2-7", "planete5", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 5, new Vector3(24, 0, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 6, new Vector3(20, 8, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 7, new Vector3(18, 16, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 8, new Vector3(10, 24, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 9, new Vector3(0, 32, 0), true);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Laser, 10, new Vector3(0, 40, 0), true);

            d.ajouterCorpsCeleste(Taille.Petite, new Vector3(100, -220, 0), "2-8", "planete6", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-300, -100, 0), "2-9", "planete7", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterTrouRose(new Vector3(-125, -200, 0), "Go to World 3!", 0, 11);
            d.SystemePlanetaire[10].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.CorpsCelesteAProteger = "Go to World 3!";

            c = new DescripteurCorpsCeleste();
            c.Nom = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 2560000;
            c.PositionDepart = 75;
            c.Taille = Taille.Petite;
            c.Representations.Add("Plutoid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Alien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 40;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<EnemyType>();
            v.EnnemisPresents.Add(EnemyType.Asteroid);
            v.EnnemisPresents.Add(EnemyType.Comet);
            v.EnnemisPresents.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }


        public static DescripteurScenario getDescripteurMonde3()
        {
            DescripteurScenario d = new DescripteurScenario();

            DescripteurCorpsCeleste c;
            DescripteurEmplacement e;

            d.Joueur.PointsDeVie = 1;
            d.Joueur.ReserveUnites = 100;

            d.FondEcran = "fondecran15";

            d.ajouterTrouRose(new Vector3(500, 0, 0), "Go back\nto World\n2!", 0, 1);
            d.SystemePlanetaire[0].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(450, -200, 0), "3-1", "stationSpatiale1", 0, 2);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[1].ajouterTourelle(TurretType.MultipleLasers, 10, new Vector3(-20, -15, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(350, 200, 0), "3-2", "stationSpatiale1", 0, 3);
            d.SystemePlanetaire[2].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(250, 0, 0), "3-3", "stationSpatiale2", 0, 4);
            d.SystemePlanetaire[3].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(150, -200, 0), "3-4", "stationSpatiale1", 0, 5);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Basic, 10, new Vector3(-17, -13, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Basic, 10, new Vector3(-19, 0, 0), true);
            d.SystemePlanetaire[4].ajouterTourelle(TurretType.Basic, 10, new Vector3(-3, 15, 0), true);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(50, 200, 0), "3-5", "stationSpatiale2", 0, 6);
            d.SystemePlanetaire[5].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-50, 0, 0), "3-6", "stationSpatiale2", 0, 7);
            d.SystemePlanetaire[6].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Moyenne, new Vector3(-150, -200, 0), "3-7", "stationSpatiale1", 0, 8);
            d.SystemePlanetaire[7].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-200, 225, 0), "3-8", "stationSpatiale1", 0, 9);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.Missile, 10, new Vector3(-25, 0, 0), true);
            d.SystemePlanetaire[8].ajouterTourelle(TurretType.Missile, 10, new Vector3(-18, -8, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-300, 30, 0), "3-9", "stationSpatiale2", 0, 10);
            d.SystemePlanetaire[9].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.SystemePlanetaire[9].ajouterTourelle(TurretType.MultipleLasers, 10, new Vector3(-20, 0, 0), true);

            d.ajouterCorpsCeleste(Taille.Grande, new Vector3(-400, -160, 0), "3-10", "planete1", 0, 11);
            d.SystemePlanetaire[10].ajouterTourelle(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);


            d.CorpsCelesteAProteger = "3-10";

            c = new DescripteurCorpsCeleste();
            c.Nom = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Vitesse = 2560000;
            c.PositionDepart = 0;
            c.Taille = Taille.Petite;
            c.Representations.Add("Asteroid");
            c.Priorite = 0;
            c.TourellesPermises = new List<TurretType>();
            c.Selectionnable = false;
            e = new DescripteurEmplacement();
            e.Position = Vector3.Zero;
            e.Tourelle = new DescripteurTourelle();
            e.Tourelle.Type = TurretType.Alien;
            e.Tourelle.PeutVendre = false;
            e.Tourelle.PeutMettreAJour = false;
            c.Emplacements.Add(e);
            d.SystemePlanetaire.Add(c);

            DescripteurVaguesInfinies v = new DescripteurVaguesInfinies();
            v.DifficulteDepart = 70;
            v.IncrementDifficulte = 0;
            v.MinerauxParVague = 0;
            v.MinMaxEnnemisParVague = new Vector2(10, 30);
            v.EnnemisPresents = new List<EnemyType>();
            v.EnnemisPresents.Add(EnemyType.Asteroid);
            v.EnnemisPresents.Add(EnemyType.Comet);
            v.EnnemisPresents.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.VaguesInfinies = v;

            return d;
        }
    }
}
