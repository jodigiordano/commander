namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Core.Utilities;
    using Microsoft.Xna.Framework.GamerServices;

    class NouvellePartie : SceneMenu
    {
        private Main Main;
        private List<Monde> Mondes;
        private int IndiceMondeSelectionne;
        private IVisible Titre;
        private IVisible Infos;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private String ChoixTransition;
        private DescripteurScenario ChoixScenario;
        private double TempsEntreDeuxChangementMusique;

        private Menu Menu;


        private static List<String> QuotesPause = new List<string>()
        {
            "Get back to work, commander!",
            "Finish your job, commander!",
            "This world needs\n\nto be saved, commander!",
            "Don't leave them\n\nbehind, commander!",
            "The Resistance needs you, commander!"
        };

        private List<AideNiveau> AidesNiveaux;

        private String MessagePause;

        //TMP
        private AnimationLieutenant AnimationFinMonde1 = null;
        private AnimationLieutenant AnimationFinDemo = null;

        public NouvellePartie(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "NouvellePartie";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;

            Mondes = new List<Monde>();
            Mondes.Add(new Monde(main, this, 1, FactoryScenarios.getDescripteurMonde1(), FactoryScenarios.getDescriptionsScenariosMonde1(), true));
            Mondes.Add(new Monde(main, this, 2, FactoryScenarios.getDescripteurMonde2(), FactoryScenarios.getDescriptionsScenariosMonde2(), false));
            Mondes.Add(new Monde(main, this, 3, FactoryScenarios.getDescripteurMonde3(), FactoryScenarios.getDescriptionsScenariosMonde3(), false));

            IndiceMondeSelectionne = 0;

            Main.ControleurJoueursConnectes.JoueurPrincipalDeconnecte += new ControleurJoueursConnectes.JoueurPrincipalDeconnecteHandler(doJoueurPrincipalDeconnecte);

            AidesNiveaux = new List<AideNiveau>();

            AidesNiveaux.Add(new AideNiveau
            (
                Main,
                0,
                190,
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("PAP", "The goal of this game is to\n\nprotect me against asteroids."),
                    new KeyValuePair<string, string>("Ceinture", "Asteroids will come from here.\n\nThey follow the white path."),
                    new KeyValuePair<string, string>("PAP", "You see my moons ?\n\nA maximum of 5 asteroids\n\ncan reach me."),
                    new KeyValuePair<string, string>("Grav", "The path is the\n\nwhite dotted line."),
                    new KeyValuePair<string, string>("Emplacement", "This is where you buy\n\nmercenaries to kill the asteroids."),
                    new KeyValuePair<string, string>("Emplacement", "Only the green\n\nmercenary is\n\navailable right now."),
                    new KeyValuePair<string, string>("Emplacement", "You can upgrade or sell a\n\nmercenary. Use LT/RT to\n\nnavigate the choices."),
                    new KeyValuePair<string, string>("Sablier", "When I'm depleted, a wave\n\nof asteroids will come."),
                    new KeyValuePair<string, string>("Sablier", "The upper-right number is the\n\nnumber of waves remaining."),
                    new KeyValuePair<string, string>("Sablier", "The bottom-right number\n\nis your cash flow. Kill\n\nasteroids to get more cash!"),
                    new KeyValuePair<string, string>("Sablier", "Put your cursor over\n\nme to see the next wave."),
                    new KeyValuePair<string, string>("Sablier", "If you're bored,\n\nclick on me to call\n\nthe next wave earlier."),
#if WINDOWS && !MANETTE_WINDOWS
                    new KeyValuePair<string, string>("Curseur", "Maintain the middle mouse button\n\npressed to see the enemies' lifes\n\nand the mercenaries' sight."),
#else
                    new KeyValuePair<string, string>("Curseur", "Maintain the X button pressed\n\nto see the enemies' lifes\n\nand the mercenaries' sight."),
#endif
                    new KeyValuePair<string, string>("PAP", "The help in the main\n\nmenu have all the answers.\n\nThese annoying hints\n\nwill repeat 2 times.")
                }
            ));

            AidesNiveaux.Add(new AideNiveau
            (
                Main,
                1,
                191,
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Emplacement", "A new mercenary is available.\n\nThe pink one. It's a she.\n\nThreat her right."),
                    new KeyValuePair<string, string>("Curseur", "Every level is possible. This\n\ngame is hard. It's normal to\n\nredo 5-10 times a level."),
#if WINDOWS && !MANETTE_WINDOWS
                    new KeyValuePair<string, string>("Curseur", "Don't forget to use the middle\n\nmouse button to know if your\n\nmercenary is well placed.")
#else
                    new KeyValuePair<string, string>("Curseur", "Don't forget to use the X\n\nbutton to know if your\n\nmercenary is well placed.")
#endif
                }
            ));

            AidesNiveaux.Add(new AideNiveau
            (
                Main,
                2,
                192,
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Grav", "Hello I'm the\n\nwhite mercenary."),
                    new KeyValuePair<string, string>("Grav", "The path cross the\n\nplanet where I'm installed."),
                    new KeyValuePair<string, string>("Emplacement", "The white mercenary is\n\navailable. It will\n\nmodify the path."),
                    new KeyValuePair<string, string>("Emplacement", "You must lengthen the\n\npath to survive."),
                    new KeyValuePair<string, string>("Emplacement", "Upgrade the white mercenary\n\nto see what will occur!")
                }
            ));

            AidesNiveaux.Add(new AideNiveau
            (
                Main,
                3,
                193,
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Planete", "Select me (without touching\n\na square) to grab the collector."),
                    new KeyValuePair<string, string>("Planete", "In some levels, check the\n\nplanets for useful powerups"),
                    new KeyValuePair<string, string>("Planete", "When an asteroid explode,\n\nit will drop extra cash. Use\n\nthe collector to collect them.")
                }
            ));

            verifierWarpZones();
        }


        private void doJoueurPrincipalDeconnecte()
        {
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            ChoixTransition = "chargement";
            effectuerTransition = true;
        }


        private Monde MondeSelectionne
        {
            get { return Mondes[IndiceMondeSelectionne]; }
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            MondeSelectionne.Update(gameTime);
            verifierWarpZones();
            jouerAnimationFinMonde1(gameTime);
            jouerAnimationFinDemo(gameTime);
            verifierPartieEnPause();


            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                {
                    switch (ChoixTransition)
                    {
                        case "Go to World 2!":
                            effectuerTransition = true;
                            AnimationTransition.In = true;
                            AnimationTransition.Initialize();
                            IndiceMondeSelectionne = 1;
                            break;
                        case "Go to World 3!":
                            effectuerTransition = true;
                            AnimationTransition.In = true;
                            AnimationTransition.Initialize();
                            IndiceMondeSelectionne = 2;
                            break;
                        case "Go back\nto World 1!":
                            effectuerTransition = true;
                            AnimationTransition.In = true;
                            AnimationTransition.Initialize();
                            IndiceMondeSelectionne = 0;
                            break;
                        case "Go back\nto World\n2!":
                            effectuerTransition = true;
                            AnimationTransition.In = true;
                            AnimationTransition.Initialize();
                            IndiceMondeSelectionne = 1;
                            break;
                        case "menu": Core.Visuel.Facade.effectuerTransition("NouvellePartieVersMenu"); break;
                        case "chargement": Core.Visuel.Facade.effectuerTransition("NouvellePartieVersChargement"); break;
                        default:
                            if (Menu.PartieEnCours != null && !Menu.PartieEnCours.EstTerminee && Menu.PartieEnCours.Simulation.DescriptionScenario.Mission == ChoixTransition)
                                Core.Visuel.Facade.effectuerTransition("NouvellePartieVersPartie");
                            else
                            {
                                if (Menu.PartieEnCours != null)
                                {
                                    Core.Audio.Facade.arreterMusique(Menu.PartieEnCours.MusiqueSelectionnee, false, 0);

                                    if (!Menu.PartieEnCours.EstTerminee)
                                        Main.MusiquesDisponibles.Add(Menu.PartieEnCours.MusiqueSelectionnee);
                                }

                                Menu.PartieEnCours = new Partie(Main, ChoixScenario);
                                Menu.PartieEnCours.Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

                                if (ChoixScenario.Numero <= 3)
                                {
                                    AidesNiveaux[ChoixScenario.Numero].QuotesObjets.Clear();
                                    Menu.PartieEnCours.Simulation.ControleurMessages.AideNiveau = AidesNiveaux[ChoixScenario.Numero];
                                    Menu.PartieEnCours.Simulation.ControleurMessages.Initialize();
                                }

                                MondeSelectionne.arreterMessagePause();

                                MessagePause = QuotesPause[Main.Random.Next(0, QuotesPause.Count)];

                                Core.Visuel.Facade.mettreAJourScene("Partie", Menu.PartieEnCours);
                                Core.Visuel.Facade.effectuerTransition("NouvellePartieVersPartie");
                            }
                            break;

                    }
                }
            }

            else
            {
                // mis ici pour différencié du cas général.
                if (MondeSelectionne.CorpsSelectionne == "Go to World 2!" || MondeSelectionne.CorpsSelectionne == "Go to World 3!")
                {
                    if (((MondeSelectionne.CorpsSelectionne == "Go to World 2!" && Mondes[1].Debloque) ||
                          MondeSelectionne.CorpsSelectionne == "Go to World 3!" && Mondes[2].Debloque))
                    {

                        if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                        {
                            ChoixTransition = MondeSelectionne.CorpsSelectionne;
                            effectuerTransition = true;
                            AnimationTransition.In = false;
                            AnimationTransition.Initialize();

                            if (AnimationFinMonde1 != null)
                                AnimationFinMonde1.doShow();
                        }
                    }

                    else
                    {
                        MondeSelectionne.afficherMessageBloque(
                            MondeSelectionne.CorpsSelectionne == "Go to World 2!" ?
                                "You're not Commander\n\nenough to ascend to\n\na higher level." :
                                "Only a true Commander\n\nmay enjoy a better world.");
                    }
                }

                //TMP: Tous les niveaux du monde 2 == animation de "fin de monde 1"
                else if (MondeSelectionne.CorpsSelectionne != "" && MondeSelectionne.CorpsSelectionne[0] == '2' && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                {
                    if (AnimationFinMonde1 == null)
                        AnimationFinMonde1 = new AnimationLieutenant(
                            Main,
                            this,
                            "This is the end of World 1, Commander, and what you see right now is World 2, which will be available if I sell enough of World 1 ;) You can expect more mercenaries, enemies and power ups in this World! \n\nVisit ephemeregames.com to know when World 2 will be available.", 25000);
                }

                //TMP: En mode trial, si c'est la fin de la demo == tous les niveaux du monde 1 deviennent inacessibles
#if WINDOWS
                else if (Main.ModeTrial.Actif &&
                         MondeSelectionne.CorpsSelectionne != "" &&
                         (MondeSelectionne.CorpsSelectionne[2] != '1' && MondeSelectionne.CorpsSelectionne[2] != '2' && MondeSelectionne.CorpsSelectionne[2] != '3') &&
                         Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                {
                    if (AnimationFinDemo == null)
                    {
                        AnimationFinDemo = new AnimationLieutenant(
                            Main,
                            this,
                            "Only the levels 1-1, 1-2 and 1-3 are available in this demo, Commander! If you want to finish the fight and save humanity, visit ephemeregames.com to buy all the levels for only 5$! By unlocking the 9 levels, you will be able to take the warp to World 2 ! Keep my website in your bookmarks if you want more infos on me, my games and my future projects.", 25000);
                    }
                }
#else
                
                else if (Main.ModeTrial.Actif &&
                         Main.ModeTrial.FinDemo &&
                         MondeSelectionne.CorpsSelectionne != "" &&
                         MondeSelectionne.CorpsSelectionne[0] == '1' &&
                         Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                {
                    if (AnimationFinDemo == null)
                    {
                        AnimationFinDemo = new AnimationLieutenant(
                            Main,
                            this,
                            "This is the end of the demo, Commander! If you want to finish the fight and save humanity, press A right now. 9 levels of insanity for only 3 bucks are awaiting you and I bet you want to discover World 2 ! \n\nVisit ephemeregames.com for more infos on me, my games and my future projects.", 25000);
                    }
                }
#endif
                else if (AnimationFinDemo != null && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                {
                    try
                    {
    #if XBOX
                        Guide.ShowMarketplace(Main.JoueursConnectes[0].Manette);
    #endif
                    }

                    catch (GamerPrivilegeException)
                    {
    #if XBOX
                        Guide.BeginShowMessageBox
                        (
                            "Oh no!",
                            "You must be signed in with an Xbox Live enabled profile to buy the game. You can either:\n\n1. Go buy it directly on the marketplace (suggested).\n\n2. Restart the game and sign in with an Xbox Live profile.\n\n\nThank you for your support, commander!",
                            new List<string> { "Ok" },
                            0,
                            MessageBoxIcon.Warning,
                            null,
                            null);
    #endif
                    }

                }


                else if (MondeSelectionne.CorpsSelectionne != "" && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
                {
                    ChoixTransition = MondeSelectionne.CorpsSelectionne;
                    ChoixScenario = MondeSelectionne.ScenarioSelectionne;
                    effectuerTransition = true;
                    AnimationTransition.In = false;
                    AnimationTransition.Initialize();

                    MondeSelectionne.arreterMessageBloque();

                    if (ChoixTransition == "Go back\nto World 1!" && AnimationFinMonde1 != null)
                        AnimationFinMonde1.doHide();

                    if (AnimationFinDemo != null)
                        AnimationFinDemo.doHide();
                }

                else
                {
                    MondeSelectionne.arreterMessageBloque();
                }

                doRetourMenu();
                doChangerMusique(gameTime);
            }
        }

        private void doChangerMusique(GameTime gameTime)
        {
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, Main.JoueursConnectes[0].Manette, this.Nom) && TempsEntreDeuxChangementMusique <= 0)
            {
                Menu menu = (Menu)Core.Visuel.Facade.recupererScene("Menu");
                menu.changerMusique();
                TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
            }

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void doRetourMenu()
        {
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Main.JoueursConnectes[0].Manette, this.Nom) ||
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu2, Main.JoueursConnectes[0].Manette, this.Nom))
            {
                ChoixTransition = "menu";
                effectuerTransition = true;
                AnimationTransition.In = false;
                AnimationTransition.Initialize();

                if (AnimationFinMonde1 != null)
                    AnimationFinMonde1.doHide();

                if (AnimationFinDemo != null)
                    AnimationFinDemo.doHide();
            }
        }

        private void verifierPartieEnPause()
        {
            if (Menu.PartieEnCours != null && !Menu.PartieEnCours.EstTerminee)
            {
                MondeSelectionne.CorpsCelestePartieEnPause = Menu.PartieEnCours.Simulation.DescriptionScenario.Mission;
                MondeSelectionne.afficherMessagePause(MessagePause);
            }

            else
            {
                MondeSelectionne.arreterMessagePause();
                MondeSelectionne.CorpsCelestePartieEnPause = null;
            }
        }

        private void jouerAnimationFinMonde1(GameTime gameTime)
        {
            if (AnimationFinMonde1 != null && AnimationFinMonde1.estTerminee(gameTime))
                AnimationFinMonde1 = null;
            else if (AnimationFinMonde1 != null && MondeSelectionne.Numero == 2)
                AnimationFinMonde1.suivant(gameTime);
        }

        private void jouerAnimationFinDemo(GameTime gameTime)
        {
            if (AnimationFinDemo != null && AnimationFinDemo.estTerminee(gameTime))
                AnimationFinDemo = null;
            else if (AnimationFinDemo != null && MondeSelectionne.Numero == 1)
                AnimationFinDemo.suivant(gameTime);
        }


        protected override void UpdateVisuel()
        {
            MondeSelectionne.Draw(null);

            if (effectuerTransition)
                AnimationTransition.Draw(null);

            if (AnimationFinMonde1 != null && MondeSelectionne.Numero == 2)
                this.ajouterScenable(AnimationFinMonde1);

            if (AnimationFinDemo != null && MondeSelectionne.Numero == 1)
                this.ajouterScenable(AnimationFinDemo);
        }


        public override void onFocus()
        {
 	        base.onFocus();

            Menu = (Menu)Core.Visuel.Facade.recupererScene("Menu");

            //Mondes = new List<Monde>();
            //Mondes.Add(new Monde(Main, this, FactoryScenarios.getDescripteurMonde1(), FactoryScenarios.getDescriptionsScenariosMonde1(), true));
            //Mondes.Add(new Monde(Main, this, FactoryScenarios.getDescripteurMonde2(), FactoryScenarios.getDescriptionsScenariosMonde2(), false));
            //Mondes.Add(new Monde(Main, this, FactoryScenarios.getDescripteurMonde3(), FactoryScenarios.getDescriptionsScenariosMonde3(), false));

            effectuerTransition = true;
            AnimationTransition.In = true;
            AnimationTransition.Initialize();

            if (AnimationFinMonde1 != null)
                AnimationFinMonde1.doShow();

            if (AnimationFinDemo != null)
                AnimationFinDemo.doShow();

            if (!Core.Audio.Facade.musiqueJoue(Menu.MusiqueSelectionnee))
                Core.Audio.Facade.jouerMusique(Menu.MusiqueSelectionnee, true, 1000, true);
            else
                Core.Audio.Facade.reprendreMusique(Menu.MusiqueSelectionnee, true, 1000);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            if (ChoixTransition != "Go to World 2!" && ChoixTransition != "Go to World 3!" &&
                ChoixTransition != "Go back\nto World 1!" && ChoixTransition != "Go back\nto World\n2!" &&
                ChoixTransition != "menu")
            {
                Core.Audio.Facade.pauserMusique(Menu.MusiqueSelectionnee, true, 1000);
            }
        }

        private void verifierWarpZones()
        {
            if (IndiceMondeSelectionne == 0)
            {
                if (!Main.ModeTrial.Actif)
                    Mondes[1].Debloque = Main.Sauvegarde.Progression[0] > 0 && Main.Sauvegarde.Progression[1] > 0 && Main.Sauvegarde.Progression[2] > 0 && Main.Sauvegarde.Progression[3] > 0 &&
                                         Main.Sauvegarde.Progression[4] > 0 && Main.Sauvegarde.Progression[5] > 0 && Main.Sauvegarde.Progression[6] > 0 && Main.Sauvegarde.Progression[7] > 0 &&
                                         Main.Sauvegarde.Progression[8] > 0;
                else
                    Mondes[1].Debloque = Main.Sauvegarde.Progression[0] > 0 && Main.Sauvegarde.Progression[1] > 0 && Main.Sauvegarde.Progression[4] > 0 && Main.Sauvegarde.Progression[5] > 0;

                MondeSelectionne.TrousRoses[0].Couleur = (Mondes[1].Debloque) ? new Color(255, 0, 255) : new Color(255, 0, 0);
            }

            else if (IndiceMondeSelectionne == 1)
            {
                if (Main.ModeTrial.Actif)
                    Mondes[2].Debloque = false;
                else
                    Mondes[2].Debloque = Main.Sauvegarde.Progression[0] > 0 && Main.Sauvegarde.Progression[1] > 0 && Main.Sauvegarde.Progression[4] > 0 && Main.Sauvegarde.Progression[5] > 0 &&
                                         Main.Sauvegarde.Progression[2] > 0 && Main.Sauvegarde.Progression[3] > 0 && Main.Sauvegarde.Progression[8] > 0 && Main.Sauvegarde.Progression[10] > 0 &&
                                         Main.Sauvegarde.Progression[11] > 0 && Main.Sauvegarde.Progression[12] > 0 && Main.Sauvegarde.Progression[13] > 0 && Main.Sauvegarde.Progression[14] > 0 &&
                                         Main.Sauvegarde.Progression[17] > 0;

                MondeSelectionne.TrousRoses[1].Couleur = (Mondes[2].Debloque) ? new Color(255, 0, 255) : new Color(255, 0, 0);
            }
        }

        private void doNouvelEtatPartie(EtatPartie etat)
        {
            foreach (var monde in Mondes)
                monde.initLunes();
        }
    }
}
