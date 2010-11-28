namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Microsoft.Xna.Framework.GamerServices;
    using System.Threading;

    class Chargement : Scene
    {
        private enum Etat
        {
            CHARGEMENT_ASSETS,
            CONNEXION_JOUEUR,
            CHARGEMENT_SAUVEGARDE,
            CHARGEMENT_SCENES,
            TRANSITION
        }

        public Main Main;

        private IVisible FondEcran;
        private IVisible Logo;
        private Traducteur TraductionChargement;
        private Traducteur PressStart;
        private Objets.AnimationTransition AnimationTransition;

        private Sablier Sablier;
        private Etat EtatScene;

        private bool JoueurSeConnecte = false;

        private Thread ThreadChargementScenes;
        private bool ThreadChargementScenesTermine;

        PlayerIndex manettePrincipale;

#if WINDOWS
        private ValidationServeur ValidationServeur;
#endif

        private static List<String> QuotesChargement = new List<string>()
        {
            "Building a better world. Please wait.",
            "Counting pixels by hand. Please wait.",
            "Downloading the Internet. Please wait.",
            "Waiting for christmas. Please wait.",
            "Looking for Ben Laden. Please wait.",
            "Applying a Windows patch. Please wait.",
            "Logging in the Matrix. Please wait.",
            "Organizing the resistance. Please wait.",
            "Generating the universe. Please wait.",
            "Understanding my girlfriend. Please wait.",
            "Getting rid of poverty. Please wait."
        };

        public Chargement(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Chargement";
            EnPause = false;
            EstVisible = true;
            EnFocus = true;

            initTraductionChargement();

            initPressStart();

            Sablier = new Sablier(Main, this, 5000, new Vector3(0, 250, 0), 0.3f);
            Sablier.TempsRestant = 5000;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            Logo = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Logo"), new Vector3(0, -100, 0), this);
            Logo.Taille = 16;
            Logo.Origine = Logo.Centre;
            Logo.PrioriteAffichage = 0.3f;

            Core.Persistance.Facade.charger("principal");

            EtatScene = Etat.CHARGEMENT_ASSETS;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero, this);
            FondEcran.Couleur = Color.Black;
            FondEcran.TailleVecteur = new Vector2(1280, 720);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = 1f;
            
            ThreadChargementScenes = new Thread(doChargerScenes);
            ThreadChargementScenes.IsBackground = true;
            ThreadChargementScenesTermine = false;

            manettePrincipale = PlayerIndex.One;
        }

        private void initTraductionChargement()
        {
            TraductionChargement = new Traducteur
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 255),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 255),
                QuotesChargement[Main.Random.Next(0, QuotesChargement.Count)],
                3,
                true,
                3000,
                250
            );
            TraductionChargement.Centre = true;
            TraductionChargement.PartieNonTraduite.PrioriteAffichage = 0.3f;
            TraductionChargement.PartieTraduite.PrioriteAffichage = 0.3f;
        }

        private void initPressStart()
        {
            PressStart = new Traducteur
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 255),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 255),
#if WINDOWS && !MANETTE_WINDOWS
                "Click a button, Commander",
#else
                "Press a button, Commander.",
#endif
                3,
                true,
                3000,
                250
            );
            PressStart.Centre = true;
            PressStart.PartieNonTraduite.PrioriteAffichage = 0.3f;
            PressStart.PartieTraduite.PrioriteAffichage = 0.3f;
            PressStart.PartieNonTraduite.Couleur.A = 0;
            PressStart.PartieTraduite.Couleur.A = 0;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            switch (EtatScene)
            {
                case Etat.CHARGEMENT_ASSETS:

                    if (!Sablier.Tourne)
                        Sablier.TempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (Sablier.TempsRestant <= 0)
                    {
                        Sablier.TempsRestant = 5000;
                        Sablier.tourner();
                    }

                    Sablier.Update(gameTime);
                    TraductionChargement.Update(gameTime);

                    if (Core.Persistance.Facade.estCharge("principal") && TraductionChargement.Termine)
                    {
                        EtatScene = Etat.CONNEXION_JOUEUR;
                        Effets.ajouter(TraductionChargement.PartieTraduite, EffetsPredefinis.fadeOutTo0(255, 0, 1000));
                        Effets.ajouter(TraductionChargement.PartieNonTraduite, EffetsPredefinis.fadeOutTo0(255, 0, 1000));
                        Sablier.doHide(1000);
                        Effets.ajouter(PressStart.PartieTraduite, EffetsPredefinis.fadeInFrom0(255, 500, 1000));
                        Effets.ajouter(PressStart.PartieNonTraduite, EffetsPredefinis.fadeInFrom0(255, 500, 1000));

                    }
                    break;
                case Etat.CONNEXION_JOUEUR:
                    PressStart.Update(gameTime);

                    if (!JoueurSeConnecte)
                    {
                        for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
                        {
                            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheDebug, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheProchaineVague, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionPrecedent, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionSuivant, i, this.Nom) ||
                                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheVueAvancee, i, this.Nom))
                            {
                                manettePrincipale = i;
                                JoueurSeConnecte = true;
                                break;
                            }
                        }
                    }

                    else
                    {
#if XBOX
                        if (Core.Input.Facade.getJoueurConnecte(manettePrincipale) == null)
                        {
                            Core.Input.Facade.connecterJoueur(manettePrincipale);
                        }
                        else
#endif
                        {
                            Main.JoueursConnectes.Add(new Joueur(manettePrincipale));

                            if (!Core.Persistance.Facade.donneeEstCharge("savePlayer"))
                            {
                                Core.Persistance.Facade.initialiserDonneesJoueur("savePlayer", manettePrincipale);
                                Core.Persistance.Facade.chargerDonnee("savePlayer");
                            }

                            EtatScene = Etat.CHARGEMENT_SAUVEGARDE;
                        }
                    }
                    break;
                case Etat.CHARGEMENT_SAUVEGARDE:
                    if (Core.Persistance.Facade.donneeEstCharge("savePlayer"))
                    {
                        if (!ThreadChargementScenesTermine)
                            ThreadChargementScenes.Start();

#if WINDOWS
                        ValidationServeur = new ValidationServeur("commanderworld1", Main.Sauvegarde.ProductKey);
                        ValidationServeur.valider();
#endif

                        Effets.ajouter(PressStart.PartieTraduite, EffetsPredefinis.fadeOutTo0(255, 0, 1000));
                        Effets.ajouter(PressStart.PartieNonTraduite, EffetsPredefinis.fadeOutTo0(255, 0, 1000));

                        initTraductionChargement();

                        Effets.ajouter(TraductionChargement.PartieTraduite, EffetsPredefinis.fadeInFrom0(255, 500, 1000));
                        Effets.ajouter(TraductionChargement.PartieNonTraduite, EffetsPredefinis.fadeInFrom0(255, 500, 1000));

                        Sablier.doShow(1500);

                        EtatScene = Etat.CHARGEMENT_SCENES;
                    }
                    break;
                case Etat.CHARGEMENT_SCENES:
                    if (!Sablier.Tourne)
                        Sablier.TempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (Sablier.TempsRestant <= 0)
                    {
                        Sablier.TempsRestant = 5000;
                        Sablier.tourner();
                    }

                    TraductionChargement.Update(gameTime);
                    Sablier.Update(gameTime);

#if WINDOWS
                    ValidationServeur.Update(gameTime);

                    if (ValidationServeur.DelaiExpire)
                        ValidationServeur.canceler();
#endif

#if WINDOWS
                    if (ThreadChargementScenesTermine &&
                       (Main.ModeTrial.Actif || (!Main.ModeTrial.Actif && ValidationServeur.ValidationTerminee)))
#else
                    if (ThreadChargementScenesTermine)
#endif
                    {
                        Core.Visuel.Facade.mettreAJourScene("Menu", SceneMenu);
                        Core.Visuel.Facade.mettreAJourScene("NouvellePartie", SceneNouvellePartie);
                        Core.Visuel.Facade.mettreAJourScene("Aide", SceneAide);
                        Core.Visuel.Facade.mettreAJourScene("Options", SceneOptions);
                        Core.Visuel.Facade.mettreAJourScene("Editeur", SceneEditeur);
                        Core.Visuel.Facade.mettreAJourScene("Acheter", SceneAcheter);

#if WINDOWS
                        if (!ValidationServeur.ErreurSurvenue && !ValidationServeur.Valide)
                            Main.Sauvegarde.ProductKey = "";
#endif

                        EtatScene = Etat.TRANSITION;
                    }
                    break;
                case Etat.TRANSITION:
                    AnimationTransition.suivant(gameTime);

                    if (AnimationTransition.estTerminee(gameTime) && AnimationTransition.In)
                    {
                        AnimationTransition.In = false;
                        AnimationTransition.Initialize();

                        Effets.ajouter(PressStart.PartieTraduite, EffetsPredefinis.fadeInFrom0(255, 1000, 1000));
                        Effets.ajouter(PressStart.PartieNonTraduite, EffetsPredefinis.fadeInFrom0(255, 1000, 1000));

                        EtatScene = Etat.CONNEXION_JOUEUR;
                    }

                    else if (AnimationTransition.estTerminee(gameTime) && !AnimationTransition.In)
                    {
#if WINDOWS
                        if (!Main.ModeTrial.Actif && Main.Sauvegarde.ProductKey.Length != 16)
                            Core.Visuel.Facade.effectuerTransition("ChargementVersValidation");
                        else
                            Core.Visuel.Facade.effectuerTransition("ChargementVersMenu");
#else
                        Core.Visuel.Facade.effectuerTransition("ChargementVersMenu");
#endif
                    }
                    break;

            }
        }

        private Scene SceneMenu, SceneNouvellePartie, SceneAide, SceneOptions, SceneEditeur, SceneAcheter;
        public void doChargerScenes()
        {
            SceneMenu = new Menu(Main);
            SceneNouvellePartie = new NouvellePartie(Main);
            SceneAide = new Aide(Main);
            SceneOptions = new Options(Main);
            SceneEditeur = new Editeur(Main);
            SceneAcheter = new Acheter(Main);

            ThreadChargementScenesTermine = true;
        }

        protected override void UpdateVisuel()
        {
            if (Core.Persistance.Facade.estCharge("principal") && TraductionChargement.Termine)
                AnimationTransition.Draw(null);

            TraductionChargement.Draw(null);
            Sablier.Draw(null);
            PressStart.Draw(null);

            this.ajouterScenable(Logo);
        }

        public override void onFocus()
        {
            base.onFocus();

            Main.ControleurJoueursConnectes.Initialize();
            Effets.stop();
            Effets.vider();
            JoueurSeConnecte = false;
            initPressStart();
            TraductionChargement.PartieTraduite.Couleur.A = 0;
            TraductionChargement.PartieNonTraduite.Couleur.A = 0;

            EtatScene = Etat.TRANSITION;

            AnimationTransition.In = true;
            AnimationTransition.Initialize();
        }
    }
}
