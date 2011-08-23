namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : Scene
    {
        private Simulator Simulator;

        private enum State
        {
            Transition,
            ConnectPlayer,
            ConnectingPlayer,
            LoadSaveGame,
            PlayerConnected
        }

        private State SceneState;
        private MainMenuChoices Choices;
        private CommanderTitle Title;


        public MainMenuScene()
            : base(Preferences.BackBuffer)
        {
            Name = "Menu";

            Title = new CommanderTitle(this, new Vector3(0, -10, 0), VisualPriorities.Default.Title);
            Title.Initialize();

            LevelDescriptor levelDescriptor = Main.LevelsFactory.Menu;

            Simulator = new Simulator(this, levelDescriptor)
            {
                DemoMode = true,
                EnableInputs = false
            };
            Simulator.Initialize();
            Main.NewsController.LoadNewsAsync(NewsType.General);
            Inputs.AddListener(Simulator);

            SceneState = State.Transition;

            Choices = new MainMenuChoices(Simulator, VisualPriorities.Default.MenuChoices);
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();
            Title.Update();
            
            switch (SceneState)
            {
                case State.ConnectPlayer:
                    Title.Show();
                    Choices.Hide();

                    SceneState = State.ConnectingPlayer;
                    break;

                case State.ConnectingPlayer:
                    break;


                case State.LoadSaveGame:
                    if (Main.SaveGameController.IsPlayerSaveGameLoaded)
                    {
                        Title.Hide();
                        Choices.Show();

                        if (!Simulator.EnableInputs)
                            Simulator.SyncPlayers();

                        Simulator.EnableInputs = true;
                        SceneState = State.PlayerConnected;

                        Simulator.ShowHelpBarMessage(HelpBarMessage.MoveYourSpaceship, Inputs.MasterPlayer.InputType);
                        Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 1000);
                    }
                    break;
            }
        }


        protected override void UpdateVisual()
        {
            Choices.Draw();
            Title.Draw();
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            Main.MusicController.PlayMusic(false);

            if (Inputs.ConnectedPlayers.Count == 0)
                InitConnectFirstPlayer();
            else
            {
                Simulator.EnableInputs = true;
            }

            Simulator.TeleportPlayers(false);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Main.MusicController.PauseMusic();

            Simulator.EnableInputs = false;
            Simulator.TeleportPlayers(true);
        }


        #region Input Handling

        public override void PlayerMouseConnectionRequested(Core.Input.Player p, MouseButton button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void PlayerGamePadConnectionRequested(Core.Input.Player p, Buttons button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Player) p;

            if (Inputs.ConnectedPlayers.Count == 1)
                ReloadPlayerData(player);

            player.ChooseAssets();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
                BeginTransition((Player) p);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Select)
                BeginTransition((Player) p);

            else if (button == GamePadConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                InitConnectFirstPlayer();
            else if (Main.SaveGameController.CurrentPlayer == player)
            {
                ReloadPlayerData((Player) Inputs.MasterPlayer);
                SceneState = State.LoadSaveGame;
            }
        }


        private void ReloadPlayerData(Player p)
        {
            Main.SaveGameController.ReloadPlayerData(p);
            SceneState = State.LoadSaveGame;
        }


        private void InitConnectFirstPlayer()
        {
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 0, 1000);
            Title.Initialize();
            SceneState = State.ConnectPlayer;
            Simulator.EnableInputs = false;
            Simulator.SyncPlayers();
            Main.GameInProgress = null;
        }


        private void BeginTransition(Player player)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(player);

            if (c == null)
                return;

#if !DEBUG
            if (c.Name == "editor")
                return;
#endif

            switch (c.Name)
            {
                case "save the world":
                    switch (Simulator.NewGameChoice)
                    {
                        case NewGameChoice.None:
                        case NewGameChoice.NewGame:
                            Main.SaveGameController.PlayerSaveGame.ClearAndSave();
                            TransiteTo("Cutscene1");
                            break;
                        case NewGameChoice.Continue:
                            TransiteTo("World" + Main.SaveGameController.PlayerSaveGame.CurrentWorld + "Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld1:
                            TransiteTo("World1Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld2:
                            TransiteTo("World2Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld3:
                            TransiteTo("World3Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld4:
                            TransiteTo("World4Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld5:
                            TransiteTo("World5Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld6:
                            TransiteTo("World6Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld7:
                            TransiteTo("World7Annunciation");
                            break;
                        case NewGameChoice.WrapToWorld8:
                            TransiteTo("World8Annunciation");
                            break;
                    }
                    break;

                case "how to play": Simulator.ShowPanel(PanelType.Help, true); break;
                case "options": Simulator.ShowPanel(PanelType.Options, true); break;
                case "editor": TransiteTo("Editeur"); break;
                case "credits": Simulator.ShowPanel(PanelType.Credits, true); break;
                case "quit": Main.Instance.Exit(); break;
            }
        }

        #endregion
    }
}
