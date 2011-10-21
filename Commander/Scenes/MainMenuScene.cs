﻿namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : CommanderScene
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
            : base("Menu")
        {
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

                        Simulator.ShowHelpBarMessage((Commander.Player) Inputs.MasterPlayer, HelpBarMessage.MoveYourSpaceship);
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

            Main.MusicController.PlayOrResume("MainMenuMusic");

            Simulator.OnFocus();
            Simulator.TeleportPlayers(false);

            if (Inputs.ConnectedPlayers.Count == 0) //must be done after Simulator.OnFocus() to set back no input
                InitConnectFirstPlayer();
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Simulator.OnFocusLost();
            Simulator.TeleportPlayers(true);
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }


        #region Input Handling

        public override void PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
                return;

            if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();

            else if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


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
            var player = (Commander.Player) p;

            if (button == player.MouseConfiguration.Select)
                BeginTransition(player);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (button == player.GamepadConfiguration.Select)
                BeginTransition(player);

            else if (button == player.GamepadConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Select)
                BeginTransition(player);

            else if (key == player.KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();

            else if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();
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

            if (Main.SelectedWorld != null)
                Main.SelectedWorld.GameInProgress = null;
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
                        case -1:
                        case 1:
                            Main.SaveGameController.PlayerSaveGame.ClearAndSave();
                            //TransiteTo(Main.LevelsFactory.GetWorldAnnounciationStringId(1));
                            TransiteTo("Cutscene1");
                            break;
                        case 0:
                            TransiteTo(Main.LevelsFactory.GetWorldAnnounciationStringId(Main.SaveGameController.PlayerSaveGame.CurrentWorld));
                            break;
                        default:
                            TransiteTo(Main.LevelsFactory.GetWorldAnnounciationStringId(Main.LevelsFactory.GetUnlockedWorldIdByIndex(Simulator.NewGameChoice - 2)));
                            break;
                    }
                    break;

                case "how to play": Simulator.ShowPanel(PanelType.Help, true); break;
                case "options": Simulator.ShowPanel(PanelType.Options, true); break;
                case "editor": if (Preferences.Debug) { TransiteTo(Main.LevelsFactory.GetWorldAnnounciationStringId(999)); } break;
                case "credits": Simulator.ShowPanel(PanelType.Credits, true); break;
                case "quit": Main.Instance.Exit(); break;
            }
        }

        #endregion
    }
}
