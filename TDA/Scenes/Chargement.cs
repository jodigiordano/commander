namespace TDA
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Core.Input;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

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
        private Translator TraductionChargement;
        private Translator PressStart;
        private Objets.AnimationTransition AnimationTransition;

        private Sablier Sablier;
        private Etat EtatScene;

        private PlayerIndex ConnectingPlayer = PlayerIndex.One;
        private bool WaitingForPlayerToConnect = true;

        private Thread ThreadChargementScenes;
        private bool ThreadChargementScenesTermine;

        private ValidationServeur ValidationServeur;

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

            Logo = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Logo"), new Vector3(0, -100, 0));
            Logo.Taille = 16;
            Logo.Origine = Logo.Centre;
            Logo.PrioriteAffichage = 0.3f;

            Core.Persistance.Facade.charger("principal");

            EtatScene = Etat.CHARGEMENT_ASSETS;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero);
            FondEcran.Couleur = Color.Black;
            FondEcran.TailleVecteur = new Vector2(1280, 720);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = 1f;
            
            ThreadChargementScenes = new Thread(doChargerScenes);
            ThreadChargementScenes.IsBackground = true;
            ThreadChargementScenesTermine = false;
        }

        private void initTraductionChargement()
        {
            TraductionChargement = new Translator
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
            PressStart = new Translator
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 255),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 255),
                (Preferences.Target == Setting.Xbox360) ? "Press a button, Commander" : "Click a button, Commander",
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

                    WaitingForPlayerToConnect = !(WaitingForPlayerToConnect && Main.PlayersController.IsConnected(ConnectingPlayer));

                    if (!WaitingForPlayerToConnect)
                    {
                        if (!Core.Persistance.Facade.donneeEstCharge("savePlayer"))
                        {
                            Core.Persistance.Facade.initialiserDonneesJoueur("savePlayer", ConnectingPlayer);
                            Core.Persistance.Facade.chargerDonnee("savePlayer");
                        }

                        EtatScene = Etat.CHARGEMENT_SAUVEGARDE;
                    }
                    break;


                case Etat.CHARGEMENT_SAUVEGARDE:
                    if (Core.Persistance.Facade.donneeEstCharge("savePlayer"))
                    {
                        if (!ThreadChargementScenesTermine)
                            ThreadChargementScenes.Start();

                        ValidationServeur = new ValidationServeur(Preferences.ProductName, Main.SaveGame.ProductKey);
                        ValidationServeur.valider();

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
                    ValidationServeur.Update(gameTime);

                    if (ValidationServeur.DelaiExpire)
                        ValidationServeur.canceler();

                    if (ThreadChargementScenesTermine &&
                       (Main.TrialMode.Active || ValidationServeur.ValidationTerminee))
                    {
                        Core.Visuel.Facade.mettreAJourScene("Menu", SceneMenu);
                        Core.Visuel.Facade.mettreAJourScene("NouvellePartie", SceneNouvellePartie);
                        Core.Visuel.Facade.mettreAJourScene("Aide", SceneAide);
                        Core.Visuel.Facade.mettreAJourScene("Options", SceneOptions);
                        Core.Visuel.Facade.mettreAJourScene("Editeur", SceneEditeur);
                        Core.Visuel.Facade.mettreAJourScene("Acheter", SceneAcheter);

                        if (!ValidationServeur.ErreurSurvenue && !ValidationServeur.Valide)
                            Main.SaveGame.ProductKey = "";

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
                        if (!Main.TrialMode.Active && !ValidationServeur.Valide)
                            Core.Visuel.Facade.effectuerTransition("ChargementVersValidation");
                        else
                            Core.Visuel.Facade.effectuerTransition("ChargementVersMenu");
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

            Main.PlayersController.Initialize();
            Effets.stop();
            Effets.vider();
            ConnectingPlayer = PlayerIndex.One;
            WaitingForPlayerToConnect = true;
            initPressStart();
            TraductionChargement.PartieTraduite.Couleur.A = 0;
            TraductionChargement.PartieNonTraduite.Couleur.A = 0;

            EtatScene = Etat.TRANSITION;

            AnimationTransition.In = true;
            AnimationTransition.Initialize();
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            doConnectPlayer(inputIndex);
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            doConnectPlayer(inputIndex);
        }


        private void doConnectPlayer(PlayerIndex inputIndex)
        {
            if (EtatScene != Etat.CONNEXION_JOUEUR)
                return;

            ConnectingPlayer = inputIndex;
            Main.PlayersController.Connect(inputIndex);
            WaitingForPlayerToConnect = true;
        }
    }
}
