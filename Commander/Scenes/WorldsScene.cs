namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class WorldsScene : Scene
    {
        private Main Main;
        private Dictionary<int, World> Mondes;
        private int IndiceMondeSelectionne;
        private ScenarioDescriptor ChoixScenario;
        private double TempsEntreDeuxChangementMusique;
        private MainMenuScene Menu;
        private string MessagePause;
        private AnimationCommodore AnimationFinDemo = null;


        public WorldsScene(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Name = "NouvellePartie";

            Mondes = new Dictionary<int, World>();
            Mondes.Add(1, new World(main, this, ScenariosFactory.GetWorldDescriptor(1)));
            Mondes.Add(2, new World(main, this, ScenariosFactory.GetWorldDescriptor(2)));
            Mondes.Add(3, new World(main, this, ScenariosFactory.GetWorldDescriptor(3)));

            IndiceMondeSelectionne = 1;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(DoPlayerDisconnected);
        }


        private void DoPlayerDisconnected()
        {
            Visuals.Transite("NouvellePartieToChargement");
        }


        private World MondeSelectionne
        {
            get { return Mondes[IndiceMondeSelectionne]; }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            MondeSelectionne.Update(gameTime);
            jouerAnimationFinDemo(gameTime);
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (var warp in MondeSelectionne.WarpsCelestialBodies)
                ((PinkHole) warp.Key).Couleur = (Mondes[warp.Value].Unlocked) ? new Color(255, 0, 255) : new Color(255, 0, 0);
        }


        private void jouerAnimationFinDemo(GameTime gameTime)
        {
            if (AnimationFinDemo != null && AnimationFinDemo.IsFinished)
                AnimationFinDemo = null;
            else if (AnimationFinDemo != null && MondeSelectionne.Id == 1)
                AnimationFinDemo.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            MondeSelectionne.Draw();
        }


        public override void OnFocus()
        {
 	        base.OnFocus();

            EnableUpdate = true;

            Menu = (MainMenuScene)Visuals.GetScene("Menu");

            if (AnimationFinDemo != null)
                AnimationFinDemo.FadeIn();

            if (!Audio.IsMusicPlaying(Menu.SelectedMusic))
                Audio.PlayMusic(Menu.SelectedMusic, true, 1000, true);
            else
                Audio.UnpauseMusic(Menu.SelectedMusic, true, 1000);

            Input.AddListener(MondeSelectionne.Simulation);
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Audio.PauseMusic(Menu.SelectedMusic, true, 1000);

            Input.RemoveListener(MondeSelectionne.Simulation);
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
                BeginTransition("menu");

            if (button == p.MouseConfiguration.Select)
                doSelectAction();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.Cancel)
                BeginTransition("menu");

            if (key == p.KeyboardConfiguration.ChangeMusic)
                changeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Cancel)
                BeginTransition("menu");

            if (button == p.GamePadConfiguration.ChangeMusic)
                changeMusic();

            if (button == p.GamePadConfiguration.Select)
                doSelectAction();
        }


        private void BeginTransition(string choice)
        {
            //if (ChoixTransition.StartsWith("World"))
            //{
            //    Input.RemoveListener(MondeSelectionne.Simulation);
            //    Int32.TryParse(ChoixTransition.Remove(0, 5), out IndiceMondeSelectionne);
            //    Input.AddListener(MondeSelectionne.Simulation);
            //}

            if (choice == "menu")
                Visuals.Transite("NouvellePartieToMenu");
            else
            {
                if (Main.GameInProgress != null &&
                    !Main.GameInProgress.IsFinished &&
                    Main.GameInProgress.Simulation.DescriptionScenario.Mission == choice &&
                    MondeSelectionne.Simulation.GameAction == GameAction.Resume)
                {
                    Main.GameInProgress.State = GameState.Running;
                    Visuals.Transite("NouvellePartieToPartie");
                }
                else
                {
                    if (Main.GameInProgress != null)
                    {
                        Audio.StopMusic(Main.GameInProgress.MusiqueSelectionnee, false, 0);

                        if (!Main.GameInProgress.IsFinished)
                            Main.AvailableMusics.Add(Main.GameInProgress.MusiqueSelectionnee);
                    }

                    Main.GameInProgress = new GameScene(Main, ChoixScenario);
                    Main.GameInProgress.Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);
                    MondeSelectionne.Simulation.MessagesController.StopPausedMessage();

                    Visuals.UpdateScene("Partie", Main.GameInProgress);
                    Visuals.Transite("NouvellePartieToPartie");
                }
            }

            if (AnimationFinDemo != null)
                AnimationFinDemo.FadeOut();
        }


        private void changeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            MainMenuScene menu = (MainMenuScene)Visuals.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }


        private void doSelectAction()
        {
            if (MondeSelectionne.WorldSelected != -1)
            {
                if (Mondes[MondeSelectionne.WorldSelected].Unlocked)
                {
                    //Transition = TransitionType.Out;
                    //ChoixTransition = "World" + MondeSelectionne.WorldSelected;
                }

                else
                {
                    MondeSelectionne.ShowWarpBlockedMessage();
                }
            }

            else if (Main.TrialMode.Active && MondeSelectionne.LevelSelected != null && MondeSelectionne.LevelSelected.Id > 2)
            {
                if (AnimationFinDemo == null)
                AnimationFinDemo = new AnimationCommodore(
                    "Only the levels 1-1, 1-2 and 1-3 are available in this demo, Commander! If you want to finish the fight and save humanity, visit ephemeregames.com to buy all the levels for only 5$! By unlocking the 9 levels, you will be able to take the warp to World 2 ! Keep my website in your bookmarks if you want more infos on me, my games and my future projects.", 25000, Preferences.PrioriteGUIPanneauGeneral );
            }

            else if ( MondeSelectionne.LevelSelected != null )
            {
                ChoixScenario = MondeSelectionne.LevelSelected;

                BeginTransition(MondeSelectionne.LevelSelected.Mission);
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
