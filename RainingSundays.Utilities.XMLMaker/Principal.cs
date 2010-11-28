namespace Utilities.XMLTester
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
    using Core.Visuel;
    using EndOfCivilizations;

    class Principal
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Asset a generer: ");
            Console.WriteLine("[0] FondEcran");
            Console.WriteLine("[1] DescriptionTransition");

            char choix = (char)Console.Read();
            Console.ReadLine();
            bool valide = true;

            switch (choix)
            {
                case '0':
                    Console.WriteLine("Generation en cours.");
                    ecrireXML<FondEcran2>("fondEcran.xml", genererFondEcran());
                break;

                case '1':
                    Console.WriteLine("Generation en cours.");
                    ecrireXML<DescriptionTransition>("descriptionTransition.xml", genererDescriptionTransition());
                break;

                default: valide = false;
                break;
            }

            Console.WriteLine("Termine.");

            if (valide)
                Console.WriteLine("Le fichier a ete genere dans le dossier " + AppDomain.CurrentDomain.BaseDirectory);

            Console.Read();

            //// Test Dialogue
            //EndOfCivilizations.Dialogue dialogue = new EndOfCivilizations.Dialogue();
            //dialogue.Nom = "DialogueNiveau1FR";
            //dialogue.Duree = 18000;
            ////dialogue.Monologues = new List<EndOfCivilizations.Monologue>();

            ////EndOfCivilizations.Monologue monologue = new EndOfCivilizations.Monologue();
            ////monologue.Visage = new EndOfCivilizations.IVisible();
            ////monologue.Visage.TextureNom = "visageLia";
            ////monologue.Visage.Position = new Vector3(50, 50, 0);
            ////monologue.Visage.Couleur = Color.TransparentWhite;
            ////monologue.Texte = new EndOfCivilizations.IVisible();
            ////monologue.Texte.Texte = "Felicitation, tu es\n" + "maintenant immortel\n" + "et bientot le\n" + "dernier survivant\n" + "de cette planete.";
            ////monologue.Texte.PoliceNom = "policeMenu";
            ////monologue.Texte.Position = new Vector3(150, 75, 0);
            ////monologue.EffetsTexte = new List<EndOfCivilizations.Effet>();
            ////monologue.EffetsTexte.Add(EndOfCivilizations.EffetsPredefinis.fadeInFrom0(255, 1500, 4000));

            ////monologue.EffetsVisage = new List<EndOfCivilizations.Effet>();
            ////monologue.EffetsVisage.Add(EndOfCivilizations.EffetsPredefinis.fadeInFrom0(255, 1000, 4000));

            ////dialogue.Monologues.Add(monologue);

            //XmlWriterSettings settings = ecrireXML(dialogue);

            //// Test transition

            //EndOfCivilizations.Transition transition = new EndOfCivilizations.Transition();
            //transition.Duree = 1000;
            //transition.NomTransition = "IntroductionToMenu";
            //transition.Descriptions = new List<EndOfCivilizations.DescriptionTransition>();

            //EndOfCivilizations.DescriptionTransition s;

            //s = new EndOfCivilizations.DescriptionTransition();
            //s.NomScene = "Menu";
            //s.EnPausePendant = false;
            //s.VisibleApres = true;
            //s.EnPauseApres = false;
            //s.FocusApres = true;
            //s.PrioriteAffichageApres = 1.0f;
            //s.PrioriteAffichagePendant = 1.0f;

            //EndOfCivilizations.EffetCamera effetCamera = new EndOfCivilizations.EffetCamera();
            //effetCamera.Duree = transition.Duree;
            //effetCamera.Position = new Vector3(0, 0, 888.3f);
            //effetCamera.Rotation = 0.0f;
            ////effetCamera.VitesseDeplacement = EndOfCivilizations.Trajet.CreerVitesse(EndOfCivilizations.Trajet.Type.Logarithmique, transition.Duree);
            ////effetCamera.VitesseRotation = EndOfCivilizations.Trajet.CreerVitesse(EndOfCivilizations.Trajet.Type.Lineaire, transition.Duree);
            ////effetCamera.VitesseZoom = EndOfCivilizations.Trajet.CreerVitesse(EndOfCivilizations.Trajet.Type.Logarithmique, transition.Duree);

            //s.Effets = new List<EndOfCivilizations.Effet>();
            //s.Effets.Add(effetCamera);

            //transition.Descriptions.Add(s);

            //s = new EndOfCivilizations.DescriptionTransition();
            //s.NomScene = "Introduction";
            //s.EnPausePendant = true;
            //s.EnPauseApres = true;
            //s.FocusApres = false;
            //s.PrioriteAffichageApres = 0.5f;
            //s.PrioriteAffichagePendant = 0.5f;
            //s.VisibleApres = true;

            //EndOfCivilizations.EffetDeplacement effetDeplacement = EndOfCivilizations.EffetsPredefinis.deplacerMaintenant(new Vector3(57, 839, 0));

            //EndOfCivilizations.EffetFadeCouleur effetFade = new EndOfCivilizations.EffetFadeCouleur();
            //effetFade.Duree = transition.Duree;
            //effetFade.Delai = 0;
            //effetFade.Progression = EndOfCivilizations.Effet.TypeProgression.Lineaire;
            //effetFade.Trajet = new EndOfCivilizations.Trajet(
            //    new Vector2[]
            //    {
            //        new Vector2(0.5f),
            //        new Vector2(1)
            //    },
            //    new double[]
            //    {
            //        0,
            //        transition.Duree
            //    }
            //);

            //s.Effets = new List<EndOfCivilizations.Effet>();
            //s.Effets.Add(effetDeplacement);
            //s.Effets.Add(effetFade);

            //transition.Descriptions.Add(s);

            //using (XmlWriter writer = XmlWriter.Create("transitionIntroductionToMenu.xml", settings))
            //{
            //    IntermediateSerializer.Serialize(writer, transition, null);
            //}

            //// Test assets

            //List<EndOfCivilizations.DescriptionPackage> listePackages = new List<EndOfCivilizations.DescriptionPackage>();


            //EndOfCivilizations.DescriptionPackage descPackage = new EndOfCivilizations.DescriptionPackage()
            //{
            //    BanquesSonores = new Dictionary<string, string[]>()
            //    {
            //        {
            //            "jeu",
            //            new String[] { "ennemiMeurt", "Audio/Environnement/ennemiMeurt" }
            //        },
            //        {
            //            "lia",
            //            new String[] {}
            //        }
            //    },
            //    Textures = new List<String>()
            //    {
            //        "ennemi1", "ennemi2", "ennemi3", "ennemi4", "ennemi6",
            //        "ennemiAsteroide", "ennemiAsteroideMorceau1", "ennemiAsteroideMorceau2", "ennemiAsteroideOmbre",
            //        "ennemiEtoileFilante", "ennemiEtoileFilante2",
            //        "fire", "fumee", "damage", "etoile",
            //        "projectile", "projectileLaser", "missileCercle", "projectileVaisseau",
            //        "Medailles/medailleBlouson", "Medailles/medaillePoints1Blouson", "Medailles/medaillePoints1Notification",
            //        "vaisseau2",
            //        "PUvies", "PUcanonSup", "PUmissile", "PUbase", "PUmitrailleuse", "PUcash", "PUaveugle", "PUaide", "PUflipecran",
            //        "back", "back2",
            //        "feuTire1", "feuTire2", "feuTire3",
            //        "deplacement", "blind", "vaisseauAide",
            //        "vide",
            //        "menu", "bip", "fleche", "bgOptions", "bgTop",
            //        "gameOver", "blackScreen",
            //        "bgMenuFull", "bgJeuFull",
            //        "visageHumain", "visageLia",
            //        "mire",
            //        "cercle",
            //        "indicateurTeleport",
            //        "Backgrounds/bgNiveau1Calque0Texture1", "Backgrounds/bgNiveau1Calque0Texture2",
            //        "Backgrounds/bgNiveau1Calque1Texture1", "Backgrounds/bgNiveau1Calque1Texture2",
            //        "Backgrounds/bgNiveau1Calque2Texture1", "Backgrounds/bgNiveau1Calque2Texture2", "Backgrounds/bgNiveau1Calque2Texture3", "Backgrounds/bgNiveau1Calque2Texture4",
            //        "Backgrounds/bgNiveau1Calque2Texture5", "Backgrounds/bgNiveau1Calque2Texture6", "Backgrounds/bgNiveau1Calque2Texture7", "Backgrounds/bgNiveau1Calque2Texture8",
            //        "Backgrounds/bgNiveau1Calque2Texture9", "Backgrounds/bgNiveau1Calque2Texture10", "Backgrounds/bgNiveau1Calque2Texture11", "Backgrounds/bgNiveau1Calque2Texture12",
            //        "Backgrounds/bgNiveau1Calque2Texture13",
            //        "Backgrounds/bgNiveau1Calque3Texture1", "Backgrounds/bgNiveau1Calque3Texture2", "Backgrounds/bgNiveau1Calque3Texture3", "Backgrounds/bgNiveau1Calque3Texture4",
            //        "Backgrounds/bgNiveau1Calque3Texture5", "Backgrounds/bgNiveau1Calque3Texture6", "Backgrounds/bgNiveau1Calque3Texture7", "Backgrounds/bgNiveau1Calque3Texture8",
            //        "Backgrounds/bgNiveau1Calque3Texture9", "Backgrounds/bgNiveau1Calque3Texture10", "Backgrounds/bgNiveau1Calque3Texture11", "Backgrounds/bgNiveau1Calque3Texture12",
            //        "Backgrounds/bgNiveau1Calque3Texture13", "Backgrounds/bgNiveau1Calque3Texture14", "Backgrounds/bgNiveau1Calque3Texture15", "Backgrounds/bgNiveau1Calque3Texture16",
            //        "Backgrounds/bgNiveau1Calque3Texture17", "Backgrounds/bgNiveau1Calque3Texture18", "Backgrounds/bgNiveau1Calque3Texture19", "Backgrounds/bgNiveau1Calque3Texture20",
            //        "Aliens/alien1", "Aliens/alien2", "Aliens/alien3", "Aliens/alien4", "Aliens/alien5",
            //        "TachesDeSang/tache1", "TachesDeSang/tache2", "TachesDeSang/tache3", "TachesDeSang/tache4",
            //        "pixelBlanc",
            //        "ennemiPatrouilleur", "ennemiPatrouilleurPhares",
            //        "Sprites/explosion",
            //        "Boutons/A",
            //        "menuTopLeft", "menuTopRight", "menuBottomLeft", "menuBottomRight"
            //    },
            //    Musiques = new Dictionary<string, string>()
            //    {
            //        { "bossIntro", "Audio/Musiques/Paradisejp mp3" },
            //        { "boss", "Audio/Musiques/chiasse2" },
            //        { "powerUp", "Audio/Musiques/chiasse2" },
            //    },
            //    Particules = new Dictionary<string, int>()
            //    {
            //        {"Particules/ennemiEtoileFilanteTrainee", 30},
            //        {"Particules/ennemiAsteroideOmbre", 100},
            //        {"Particules/ennemiAsteroideTouche", 100},
            //        {"Particules/tachesDeSang", 30},
            //        {"Particules/teleportation", 30},
            //        {"Particules/teleportationInverse", 30},
            //        {"Particules/armeBaseTrainee", 100},
            //        {"Particules/selectionMenu", 30},
            //        {"Particules/bip", 10}
            //    },
            //    Transitions = new List<String>()
            //    {
            //        "Transitions/transitionSplashScreenToMenu",
            //        "Transitions/transitionMenuToSplashScreen",
            //        "Transitions/transitionMenuToIntroduction",
            //        "Transitions/transitionMenuToMap",
            //        "Transitions/transitionMapToMenu",
            //        "Transitions/transitionOptionsToMenu",
            //        "Transitions/transitionMenuToOptions",
            //        "Transitions/transitionIntroductionToDialogue",
            //        "Transitions/transitionDialogueToJeu",
            //        "Transitions/transitionMenuToHighScore",
            //        "Transitions/transitionHighScoreToMenu",
            //        "Transitions/transitionHighScoreToDialogue"
            //    },
            //    Temporaire = false,
            //    Nom = "principal"
            //};

            //EndOfCivilizations.DescriptionPackage descPack2 = new EndOfCivilizations.DescriptionPackage()
            //{
            //    Musiques = new Dictionary<string, string>() { { "menu", "Audio/Musiques/Sans titre 2 Rendered2" } },
            //    Niveau = -1,
            //    EffetsFX = new List<string>() { "PostProcessing" },
            //    Textures = new List<String>()
            //    {
            //        "SplashScreen",
            //        "loading",
            //        "Boutons/start"
            //    },
            //    Polices = new Dictionary<string, string>()
            //    {
            //        { "police", "Score" },
            //        { "policeMenu", "PoliceMenu" },
            //        { "policePowerUpPoints", "PowerUpPoints" },
            //        { "policeRusse", "PoliceRusse" },
            //        { "policePixel", "PolicePixel" }
            //    },
            //    Nom = "splashScreen",
            //    Temporaire = false
            //};

            //EndOfCivilizations.DescriptionPackage descPack3 = new EndOfCivilizations.DescriptionPackage()
            //{
            //    BanquesSonores = new Dictionary<string, string[]>()
            //    {
            //        {
            //            "lia",
            //            new String[]
            //            {
            //                "quote1", "Audio/QuotesLia/LiaQuote1",
            //                "quote2", "Audio/QuotesLia/LiaQuote2",
            //                "quote3", "Audio/QuotesLia/LiaQuote3"
            //            }
            //        }
            //    },
            //    Musiques = new Dictionary<string, string>() { { "jeu", "Audio/Musiques/x16.2" } },
            //    Niveau = 1,
            //};

            //listePackages.Add(descPackage);
            //listePackages.Add(descPack2);
            //listePackages.Add(descPack3);

            //using (XmlWriter writer = XmlWriter.Create("assets.xml", settings))
            //{
            //    IntermediateSerializer.Serialize(writer, listePackages, null);
            //}


            //// Test Sprite

            //EndOfCivilizations.Sprite sprite = new EndOfCivilizations.Sprite()
            //{
            //    NomSpriteSheet = "Sprites/explosion",
            //    Cyclique = false,
            //    VitesseDefilement = 50,
            //    Rectangles = new Rectangle[]
            //    {
            //        new Rectangle(0,    0,      64,     65),
            //        new Rectangle(64,   0,      65,     65),
            //        new Rectangle(129,  0,      63,     65),
            //        new Rectangle(193,  0,      63,     65),
            //        new Rectangle(0,    66,     64,     65),
            //        new Rectangle(64,   66,     65,     65),
            //        new Rectangle(129,  66,     63,     65),
            //        new Rectangle(193,  66,     63,     65),
            //        new Rectangle(0,    132,    64,     65),
            //        new Rectangle(64,   132,    65,     65),
            //        new Rectangle(129,  132,    63,     65),
            //        new Rectangle(193,  132,    63,     65),
            //        new Rectangle(0,    197,    64,     59),
            //        new Rectangle(64,   197,    65,     59),
            //        new Rectangle(129,  197,    63,     59),
            //        new Rectangle(193,  197,    63,     59)
            //    }
            //};

            //// Test Donnees Niveau
            //EndOfCivilizations.DonneesNiveau donneesNiveau = new EndOfCivilizations.DonneesNiveau()
            //{
            //    FrequencesEnnemis = new Dictionary<string, int>()
            //    {
            //        {"Asteroide",           80},
            //        {"EtoileFilante",       10},
            //        {"AnneauAsteroides",    10}
            //    },

            //    NombreMaximumEnnemis = new Dictionary<string, int>()
            //    {
            //        {"Asteroide",           int.MaxValue},
            //        {"EtoileFilante",       3},
            //        {"AnneauAsteroides",    1}
            //    },

            //    PowerUpsTemporairesActives = new List<string>()
            //    {
            //        "Missile",
            //        "Aveugle",
            //        "FlipEcran"
            //    },

            //    PowerUpsPermanentsActives = new List<string>()
            //    {
            //        "Vie",
            //        "Canons"
            //    },

            //    HabiletesActivees = new List<string>()
            //    {

            //    },

            //    Teleportation = 0,
            //    TempsApparitionEnnemis = 333
            //};

            //using (XmlWriter writer = XmlWriter.Create("niveau1.xml", settings))
            //{
            //    IntermediateSerializer.Serialize(writer, donneesNiveau, null);
            //}
        }

        private static DescriptionTransition genererDescriptionTransition()
        {
            DescriptionTransition description = new DescriptionTransition();

            description.NomScene = "NomDeLaScene";
            description.PrioriteAffichageApres = 1;
            description.PrioriteAffichagePendant = 1;
            description.VisibleApres = false;
            description.FocusApres = false;
            description.EnPausePendant = false;
            description.EnPauseApres = true;
            description.Effets = new List<Core.Utilities.AbstractEffet>();
            description.Effets.Add(EffetsPredefinis.fadeOutTo0(255, 0, 3000));
            description.Effets.Add(EffetsPredefinis.fadeInFrom0(255, 0, 3000));
            description.Animations.Add(new TDA.Objets.AnimationTransitionOut());

            return description;
        }

        private static FondEcran2 genererFondEcran()
        {
            FondEcran2 fondEcran = new FondEcran2();

            fondEcran.Position = Vector3.Zero;
            fondEcran.Layers = new List<FondEcran2.Layer>();
            fondEcran.Layers.Add(new FondEcran2.Layer());
            fondEcran.Layers.Add(new FondEcran2.Layer());

            fondEcran.Layers[0].MosaiquesStatiques = new Mosaique2[]
            {
                new Mosaique2()
                {
                    NomsTextures = new String[][]
                    {
                        new String[] { "layer1", "layer1", "layer1" },
                        new String[] { "layer1", "layer1", "layer1" },
                        new String[] { "layer1", "layer1", "layer1" },
                        new String[] { "layer1", "layer1", "layer1" }
                    }
                }
            };

            fondEcran.Layers[1].MosaiquesStatiques = new Mosaique2[]
            {
                new Mosaique2()
                {
                    NomsTextures = new String[][]
                    {
                        new String[] { "layer2", "layer2", "layer2" },
                        new String[] { "layer2", "layer2", "layer2" },
                        new String[] { "layer2", "layer2", "layer2" },
                        new String[] { "layer2", "layer2", "layer2" }
                    }
                }
            };

            return fondEcran;
        }

        private static void ecrireXML<T>(String nomFichier, T objet)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(nomFichier, settings))
            {
                IntermediateSerializer.Serialize(writer, objet, null);
            }
        }
    }
}
