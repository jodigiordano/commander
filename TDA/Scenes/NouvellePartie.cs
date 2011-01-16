namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class NouvellePartie : SceneMenu
    {
        private Main Main;
        private Dictionary<int, World> Mondes;
        private int IndiceMondeSelectionne;
        private AnimationTransition AnimationTransition;
        private String ChoixTransition;
        private DescripteurScenario ChoixScenario;
        private double TempsEntreDeuxChangementMusique;
        private Menu Menu;
        //private List<AideNiveau> AidesNiveaux;
        private String MessagePause;
        private AnimationLieutenant AnimationFinMonde1 = null;
        private AnimationLieutenant AnimationFinDemo = null;


        public NouvellePartie(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "NouvellePartie";

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            Mondes = new Dictionary<int, World>();
            Mondes.Add(1, new World(main, this, FactoryScenarios.GetWorldDescriptor(1)));
            Mondes.Add(2, new World(main, this, FactoryScenarios.GetWorldDescriptor(2)));
            Mondes.Add(3, new World(main, this, FactoryScenarios.GetWorldDescriptor(3)));

            IndiceMondeSelectionne = 1;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);

//            AidesNiveaux = new List<AideNiveau>();

//            AidesNiveaux.Add(new AideNiveau
//            (
//                Main,
//                0,
//                new List<KeyValuePair<string, string>>()
//                {
//                    new KeyValuePair<string, string>("PAP", "The goal of this game is to\n\nprotect me against asteroids."),
//                    new KeyValuePair<string, string>("Ceinture", "Asteroids will come from here.\n\nThey follow the white path."),
//                    new KeyValuePair<string, string>("PAP", "You see my moons ?\n\nA maximum of 5 asteroids\n\ncan reach me."),
//                    new KeyValuePair<string, string>("Grav", "The path is the\n\nwhite dotted line."),
//                    new KeyValuePair<string, string>("Emplacement", "This is where you buy\n\nmercenaries to kill the asteroids."),
//                    new KeyValuePair<string, string>("Emplacement", "Only the green\n\nmercenary is\n\navailable right now."),
//                    new KeyValuePair<string, string>("Emplacement", "You can upgrade or sell a\n\nmercenary. Use LT/RT to\n\nnavigate the choices."),
//                    new KeyValuePair<string, string>("Sablier", "When I'm depleted, a wave\n\nof asteroids will come."),
//                    new KeyValuePair<string, string>("Sablier", "The upper-right number is the\n\nnumber of waves remaining."),
//                    new KeyValuePair<string, string>("Sablier", "The bottom-right number\n\nis your cash flow. Kill\n\nasteroids to get more cash!"),
//                    new KeyValuePair<string, string>("Sablier", "Put your cursor over\n\nme to see the next wave."),
//                    new KeyValuePair<string, string>("Sablier", "If you're bored,\n\nclick on me to call\n\nthe next wave earlier."),
//#if WINDOWS && !MANETTE_WINDOWS
//                    new KeyValuePair<string, string>("Curseur", "Maintain the middle mouse button\n\npressed to see the enemies' lifes\n\nand the mercenaries' sight."),
//#else
//                    new KeyValuePair<string, string>("Curseur", "Maintain the X button pressed\n\nto see the enemies' lifes\n\nand the mercenaries' sight."),
//#endif
//                    new KeyValuePair<string, string>("PAP", "The help in the main\n\nmenu have all the answers.\n\nThese annoying hints\n\nwill repeat 2 times.")
//                }
//            ));

//            AidesNiveaux.Add(new AideNiveau
//            (
//                Main,
//                1,
//                new List<KeyValuePair<string, string>>()
//                {
//                    new KeyValuePair<string, string>("Emplacement", "A new mercenary is available.\n\nThe pink one. It's a she.\n\nThreat her right."),
//                    new KeyValuePair<string, string>("Curseur", "Every level is possible. This\n\ngame is hard. It's normal to\n\nredo 5-10 times a level."),
//#if WINDOWS && !MANETTE_WINDOWS
//                    new KeyValuePair<string, string>("Curseur", "Don't forget to use the middle\n\nmouse button to know if your\n\nmercenary is well placed.")
//#else
//                    new KeyValuePair<string, string>("Curseur", "Don't forget to use the X\n\nbutton to know if your\n\nmercenary is well placed.")
//#endif
//                }
//            ));

//            AidesNiveaux.Add(new AideNiveau
//            (
//                Main,
//                2,
//                new List<KeyValuePair<string, string>>()
//                {
//                    new KeyValuePair<string, string>("Grav", "Hello I'm the\n\nwhite mercenary."),
//                    new KeyValuePair<string, string>("Grav", "The path cross the\n\nplanet where I'm installed."),
//                    new KeyValuePair<string, string>("Emplacement", "The white mercenary is\n\navailable. It will\n\nmodify the path."),
//                    new KeyValuePair<string, string>("Emplacement", "You must lengthen the\n\npath to survive."),
//                    new KeyValuePair<string, string>("Emplacement", "Upgrade the white mercenary\n\nto see what will occur!")
//                }
//            ));

//            AidesNiveaux.Add(new AideNiveau
//            (
//                Main,
//                3,
//                new List<KeyValuePair<string, string>>()
//                {
//                    new KeyValuePair<string, string>("Planete", "Select me (without touching\n\na square) to grab the collector."),
//                    new KeyValuePair<string, string>("Planete", "In some levels, check the\n\nplanets for useful powerups"),
//                    new KeyValuePair<string, string>("Planete", "When an asteroid explode,\n\nit will drop extra cash. Use\n\nthe collector to collect them.")
//                }
//            ));
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            AnimationTransition.Initialize();
        }


        private World MondeSelectionne
        {
            get { return Mondes[IndiceMondeSelectionne]; }
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            MondeSelectionne.Update(gameTime);
            jouerAnimationFinMonde1(gameTime);
            jouerAnimationFinDemo(gameTime);
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (var warp in MondeSelectionne.WarpsCelestialBodies)
                ((TrouRose) warp.Key).Couleur = (Mondes[warp.Value].Unlocked) ? new Color(255, 0, 255) : new Color(255, 0, 0);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            if (Transition == TransitionType.Out)
                if (ChoixTransition.StartsWith("World"))
                {
                    Transition = TransitionType.In;
                    AnimationTransition.Initialize();
                    EphemereGames.Core.Input.Facade.RemoveListener(MondeSelectionne.Simulation);
                    Int32.TryParse(ChoixTransition.Remove(0, 5), out IndiceMondeSelectionne);
                    EphemereGames.Core.Input.Facade.AddListener(MondeSelectionne.Simulation);

                    if (IndiceMondeSelectionne == 1 && AnimationFinMonde1 != null)
                        AnimationFinMonde1.doShow();
                }

                else if (ChoixTransition == "menu")
                    EphemereGames.Core.Visuel.Facade.Transite("NouvellePartieToMenu");
                else if (ChoixTransition == "chargement")
                    EphemereGames.Core.Visuel.Facade.Transite("NouvellePartieToChargement");
                else
                {
                    if (Main.GameInProgress != null &&
                        !Main.GameInProgress.EstTerminee &&
                        Main.GameInProgress.Simulation.DescriptionScenario.Mission == ChoixTransition &&
                        MondeSelectionne.Simulation.GameAction == GameAction.Resume)
                    {
                        Main.GameInProgress.State = GameState.Running;
                        EphemereGames.Core.Visuel.Facade.Transite("NouvellePartieToPartie");
                    }
                    else
                    {
                        if (Main.GameInProgress != null)
                        {
                            EphemereGames.Core.Audio.Facade.arreterMusique(Main.GameInProgress.MusiqueSelectionnee, false, 0);

                            if (!Main.GameInProgress.EstTerminee)
                                Main.MusiquesDisponibles.Add(Main.GameInProgress.MusiqueSelectionnee);
                        }

                        Main.GameInProgress = new Partie(Main, ChoixScenario);
                        Main.GameInProgress.Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);
                        MondeSelectionne.Simulation.ControleurMessages.StopPausedMessage();

                        if (ChoixScenario.Numero <= 3)
                        {
                            //AidesNiveaux[ChoixScenario.Numero].QuotesObjets.Clear();
                            //Main.GameInProgress.Simulation.ControleurMessages.AideNiveau = AidesNiveaux[ChoixScenario.Numero];
                            Main.GameInProgress.Simulation.ControleurMessages.Initialize();
                        }

                        EphemereGames.Core.Visuel.Facade.UpdateScene("Partie", Main.GameInProgress);
                        EphemereGames.Core.Visuel.Facade.Transite("NouvellePartieToPartie");
                    }
                }
            else
                Transition = TransitionType.None;
        }


        private void jouerAnimationFinMonde1(GameTime gameTime)
        {
            if (AnimationFinMonde1 != null && AnimationFinMonde1.Finished(gameTime))
                AnimationFinMonde1 = null;
            else if (AnimationFinMonde1 != null && MondeSelectionne.Id == 2)
                AnimationFinMonde1.Update(gameTime);
        }

        private void jouerAnimationFinDemo(GameTime gameTime)
        {
            if (AnimationFinDemo != null && AnimationFinDemo.Finished(gameTime))
                AnimationFinDemo = null;
            else if (AnimationFinDemo != null && MondeSelectionne.Id == 1)
                AnimationFinDemo.Update(gameTime);
        }


        protected override void UpdateVisuel()
        {
            MondeSelectionne.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);

            if (AnimationFinMonde1 != null && MondeSelectionne.Id == 2)
                this.ajouterScenable(AnimationFinMonde1);

            if (AnimationFinDemo != null && MondeSelectionne.Id == 1)
                this.ajouterScenable(AnimationFinDemo);
        }


        public override void onFocus()
        {
 	        base.onFocus();

            Menu = (Menu)EphemereGames.Core.Visuel.Facade.GetScene("Menu");

            Transition = TransitionType.In;

            if (AnimationFinMonde1 != null)
                AnimationFinMonde1.doShow();

            if (AnimationFinDemo != null)
                AnimationFinDemo.doShow();

            if (!EphemereGames.Core.Audio.Facade.musiqueJoue(Menu.MusiqueSelectionnee))
                EphemereGames.Core.Audio.Facade.jouerMusique(Menu.MusiqueSelectionnee, true, 1000, true);
            else
                EphemereGames.Core.Audio.Facade.reprendreMusique(Menu.MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.AddListener(MondeSelectionne.Simulation);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            if (!ChoixTransition.StartsWith("World") && ChoixTransition != "menu")
                EphemereGames.Core.Audio.Facade.pauserMusique(Menu.MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.RemoveListener(MondeSelectionne.Simulation);
        }


        private void doNouvelEtatPartie(GameState etat)
        {
            foreach (var monde in Mondes)
                monde.Value.InitLevelsStates();

            //if (etat == GameState.Paused)
            //{
            //    MondeSelectionne.Simulation.CelestialBodyPausedGame = MondeSelectionne.Simulation.ControleurSystemePlanetaire.CelestialBodies.Find(e => e.Nom == Main.GameInProgress.Simulation.DescriptionScenario.Mission);
            //    MondeSelectionne.Simulation.ControleurMessages.DisplayPausedMessage();
            //}

            //else
            //{
            //    MondeSelectionne.Simulation.ControleurMessages.StopPausedMessage();
            //}
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Cancel)
                beginTransition("menu");

            if (button == p.MouseConfiguration.Select)
                doSelectAction();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.Cancel)
                beginTransition("menu");

            if (key == p.KeyboardConfiguration.ChangeMusic)
                changeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Cancel)
                beginTransition("menu");

            if (button == p.GamePadConfiguration.ChangeMusic)
                changeMusic();

            if (button == p.GamePadConfiguration.Select)
                doSelectAction();
        }


        private void beginTransition(string choice)
        {
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = choice;

            if (AnimationFinMonde1 != null)
                AnimationFinMonde1.doHide();

            if (AnimationFinDemo != null)
                AnimationFinDemo.doHide();
        }


        private void changeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Menu menu = (Menu)EphemereGames.Core.Visuel.Facade.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }


        private void doSelectAction()
        {
            if (MondeSelectionne.WorldSelected != -1)
            {
                if (Mondes[MondeSelectionne.WorldSelected].Unlocked)
                {
                    Transition = TransitionType.Out;
                    ChoixTransition = "World" + MondeSelectionne.WorldSelected;
                }

                else
                {
                    MondeSelectionne.ShowWarpBlockedMessage();
                }
            }

            else if (MondeSelectionne.Id == 2 &&
                     MondeSelectionne.LevelSelected != null &&
                     AnimationFinMonde1 == null)
            {
                AnimationFinMonde1 = new AnimationLieutenant(
                    Main,
                    this,
                    "This is the end of World 1, Commander, and what you see right now is World 2, which will be available if I sell enough of World 1 ;) You can expect more mercenaries, enemies and power ups in this World! \n\nVisit ephemeregames.com to know when World 2 will be available.", 25000);
            }

            else if (MondeSelectionne.LevelSelected != null)
            {
                ChoixScenario = MondeSelectionne.LevelSelected;

                beginTransition(MondeSelectionne.LevelSelected.Mission);
            }


            //else if (Main.TrialMode.Active &&
            //         MondeSelectionne.CorpsSelectionne != "" &&
            //         MondeSelectionne.CorpsSelectionne[2] != '1' &&
            //         MondeSelectionne.CorpsSelectionne[2] != '2' &&
            //         MondeSelectionne.CorpsSelectionne[2] != '3' &&
            //         AnimationFinDemo == null)
            //{
            //    AnimationFinDemo = new AnimationLieutenant(
            //        Main,
            //        this,
            //        "Only the levels 1-1, 1-2 and 1-3 are available in this demo, Commander! If you want to finish the fight and save humanity, visit ephemeregames.com to buy all the levels for only 5$! By unlocking the 9 levels, you will be able to take the warp to World 2 ! Keep my website in your bookmarks if you want more infos on me, my games and my future projects.", 25000);
            //}

//#if XBOX
            //            else if (button == p.MouseConfiguration.Select &&
            //                     AnimationFinDemo != null)
            //            {
            //                try
            //                {
            //                    Guide.ShowMarketplace(Main.JoueursConnectes[0].Manette);
            //                }

//                catch (GamerPrivilegeException)
            //                {
            //                    Guide.BeginShowMessageBox
            //                    (
            //                        "Oh no!",
            //                        "You must be signed in with an Xbox Live enabled profile to buy the game. You can either:\n\n1. Go buy it directly on the marketplace (suggested).\n\n2. Restart the game and sign in with an Xbox Live profile.\n\n\nThank you for your support, commander!",
            //                        new List<string> { "Ok" },
            //                        0,
            //                        MessageBoxIcon.Warning,
            //                        null,
            //                        null);
            //            }
            //#endif
        }
    }
}
