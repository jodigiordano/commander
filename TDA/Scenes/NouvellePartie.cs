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
        private String MessagePause;
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
                            Main.GameInProgress.Simulation.ControleurMessages.Initialize();
                        }

                        EphemereGames.Core.Visuel.Facade.UpdateScene("Partie", Main.GameInProgress);
                        EphemereGames.Core.Visuel.Facade.Transite("NouvellePartieToPartie");
                    }
                }
            else
                Transition = TransitionType.None;
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

            if (AnimationFinDemo != null && MondeSelectionne.Id == 1)
                this.ajouterScenable(AnimationFinDemo);
        }


        public override void onFocus()
        {
 	        base.onFocus();

            Menu = (Menu)EphemereGames.Core.Visuel.Facade.GetScene("Menu");

            Transition = TransitionType.In;

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

            else if (Main.TrialMode.Active && MondeSelectionne.LevelSelected != null && MondeSelectionne.LevelSelected.Numero > 2)
            {
                if (AnimationFinDemo == null)
                AnimationFinDemo = new AnimationLieutenant(
                    Main,
                    this,
                    "Only the levels 1-1, 1-2 and 1-3 are available in this demo, Commander! If you want to finish the fight and save humanity, visit ephemeregames.com to buy all the levels for only 5$! By unlocking the 9 levels, you will be able to take the warp to World 2 ! Keep my website in your bookmarks if you want more infos on me, my games and my future projects.", 25000 );
            }

            else if ( MondeSelectionne.LevelSelected != null )
            {
                ChoixScenario = MondeSelectionne.LevelSelected;

                beginTransition( MondeSelectionne.LevelSelected.Mission );
            }




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
