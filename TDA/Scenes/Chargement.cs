namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class Chargement : Scene
    {
        private enum Etat
        {
            FINISHED,
            CHARGEMENT_ASSETS,
            CONNEXION_JOUEUR,
            CHARGEMENT_SAUVEGARDE,
            CHARGEMENT_SCENES
        }

        public Main Main;

        private Image Logo;
        private Image Background;
        private Translator TraductionChargement;
        private Translator PressStart;
        private AnimationTransition AnimationTransition;

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
            Active = true;

            initTraductionChargement();

            initPressStart();

            Sablier = new Sablier(Main, this, 5000, new Vector3(0, 250, 0), 0.3f);
            Sablier.TempsRestant = 5000;

            Logo = new Image("Logo", new Vector3(0, -100, 0));
            Logo.SizeX = 16;
            Logo.VisualPriority = 0.3f;

            EphemereGames.Core.Persistance.Facade.LoadPackage("principal");

            EtatScene = Etat.CHARGEMENT_ASSETS;

            Background = new Image("SplashScreenBg", Vector3.Zero);
            Background.VisualPriority = 1f;
            
            ThreadChargementScenes = new Thread(doChargerScenes);
            ThreadChargementScenes.IsBackground = true;
            ThreadChargementScenesTermine = false;

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (AnimationTransition.Finished(gameTime))
            {
                if (Transition == TransitionType.In)
                {
                    EtatScene = Etat.CONNEXION_JOUEUR;
                }

                else
                {
                    if (!Main.TrialMode.Active && !ValidationServeur.Valide)
                        EphemereGames.Core.Visuel.Facade.Transite("ChargementToValidation");
                    else
                        EphemereGames.Core.Visuel.Facade.Transite("ChargementToMenu");
                }

                Transition = TransitionType.None;
            }
        }


        private void initTraductionChargement()
        {
            TraductionChargement = new Translator
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 255),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 255),
                QuotesChargement[Main.Random.Next(0, QuotesChargement.Count)],
                3,
                true,
                3000,
                250,
                0.3f
            );
            TraductionChargement.Centre = true;
        }

        private void initPressStart()
        {
            PressStart = new Translator
            (
                Main,
                this,
                new Vector3(0, 150, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 0),
                (Preferences.Target == Setting.Xbox360) ? "Press a button, Commander" : "Click a button, Commander",
                3,
                true,
                3000,
                250,
                0.3f
            );
            PressStart.Centre = true;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

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

                    if (EphemereGames.Core.Persistance.Facade.PackageLoaded("principal") && TraductionChargement.Termine)
                    {
                        EtatScene = Etat.CONNEXION_JOUEUR;
                        Effets.Add(TraductionChargement.PartieTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        Effets.Add(TraductionChargement.PartieNonTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        Sablier.doHide(1000);
                        Effets.Add(PressStart.PartieTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));
                        Effets.Add(PressStart.PartieNonTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));

                    }
                    break;


                case Etat.CONNEXION_JOUEUR:
                    PressStart.Update(gameTime);

                    WaitingForPlayerToConnect = !(WaitingForPlayerToConnect && Main.PlayersController.IsConnected(ConnectingPlayer));

                    if (!WaitingForPlayerToConnect)
                    {
                        if (!EphemereGames.Core.Persistance.Facade.DataLoaded("savePlayer"))
                            EphemereGames.Core.Persistance.Facade.LoadData("savePlayer");

                        EtatScene = Etat.CHARGEMENT_SAUVEGARDE;
                    }
                    break;


                case Etat.CHARGEMENT_SAUVEGARDE:
                    if (EphemereGames.Core.Persistance.Facade.DataLoaded("savePlayer"))
                    {
                        if (!ThreadChargementScenesTermine)
                            ThreadChargementScenes.Start();

                        ValidationServeur = new ValidationServeur(Preferences.ProductName, Main.SaveGame.ProductKey);
                        ValidationServeur.valider();

                        Effets.Add(PressStart.PartieTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));
                        Effets.Add(PressStart.PartieNonTraduite, PredefinedEffects.FadeOutTo0(255, 0, 1000));

                        initTraductionChargement();

                        Effets.Add(TraductionChargement.PartieTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));
                        Effets.Add(TraductionChargement.PartieNonTraduite, PredefinedEffects.FadeInFrom0(255, 500, 1000));

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
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Menu", SceneMenu);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("NouvellePartie", SceneNouvellePartie);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Aide", SceneAide);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Options", SceneOptions);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Editeur", SceneEditeur);
                        EphemereGames.Core.Visuel.Facade.UpdateScene("Acheter", SceneAcheter);

                        if (!ValidationServeur.ErreurSurvenue && !ValidationServeur.Valide)
                            Main.SaveGame.ProductKey = "";

                        EtatScene = Etat.FINISHED;
                        Transition = TransitionType.Out;
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
            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);

            TraductionChargement.Draw(null);
            Sablier.Draw(null);
            PressStart.Draw(null);

            this.ajouterScenable(Background);
            this.ajouterScenable(Logo);
        }


        public override void onFocus()
        {
            base.onFocus();

            Main.PlayersController.Initialize();
            Effets.Stop();
            Effets.Clear();
            ConnectingPlayer = PlayerIndex.One;
            WaitingForPlayerToConnect = true;
            initPressStart();
            TraductionChargement.PartieTraduite.Couleur.A = 0;
            TraductionChargement.PartieNonTraduite.Couleur.A = 0;

            Transition = TransitionType.In;
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
