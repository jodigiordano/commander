namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class MultiverseScene : CommanderScene
    {
        private LevelDescriptor Layout;
        private Simulator Simulator;
        private CBBigLabels Choices;


        public MultiverseScene() :
            base("Multiverse")
        {
            Layout = Main.WorldsFactory.Multiverse;
        }


        public override void Initialize()
        {
            if (Simulator != null)
                Simulator.CleanUp();

            InitializeCBStates();

            // Initialize the simulator
            Simulator = new Simulator(this, Layout)
            {
                DemoMode = true,
                MultiverseMode = true,
                EnableInputs = true
            };

            Simulator.Initialize();
            Inputs.AddListener(Simulator);
            Simulator.EnableInputs = false;
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            Choices = new CBBigLabels(Simulator, VisualPriorities.Default.MenuChoices);
            Choices.Show();
        }


        private void InitializeCBStates()
        {
            Layout.PlanetarySystem[1].Name = "jump to world";
            Layout.PlanetarySystem[2].Name = "";
            Layout.PlanetarySystem[3].Name = "my world";
            Layout.PlanetarySystem[4].Name = Main.PlayersController.MultiverseData.Registered ? "" : "register";
        }


        #region Scene Handling

        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
            Choices.Draw();
        }


        public override void OnFocus()
        {
            Simulator.OnFocus();
        }


        public override void OnFocusLost()
        {
            Simulator.OnFocusLost();
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }

        #endregion

        #region Input

        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            var player = (Commander.Player) p;

            if (button == player.MouseConfiguration.Select)
                DoSelectAction(player);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Select)
                DoSelectAction(player);

            else if (key == player.KeyboardConfiguration.Back)
                DoBackAction();

            else if (key == player.KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();

            else if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (button == player.GamepadConfiguration.Back)
                DoBackAction();

            else if (button == player.GamepadConfiguration.ChangeMusic)
                Main.MusicController.ToggleCurrentMusic();

            else if (button == player.GamepadConfiguration.Select)
                DoSelectAction(player);
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
                return;

            if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();

            if (player.State == PlayerState.Disconnected)
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

            player.ChooseAssets();
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }




        #endregion

        #region Input Handling

        private void DoBackAction()
        {
            TransiteTo("Menu");
        }


        private void DoSelectAction(Player player)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(player);

            if (c == null)
                return;

            switch (c.Name)
            {
                case "register": DoRegister(player); break;
                case "jump to world": DoJumpToWorld(player); break;
                case "my world": DoMyWorld(player);
                    break;
            }
        }


        private void DoJumpToWorld(Player player)
        {
            Simulator.ShowPanel(PanelType.JumpToWorld, player.Position, true);
        }


        private void DoRegister(Player player)
        {
            Simulator.ShowPanel(PanelType.Register, player.Position, true);
        }


        private void DoMyWorld(Player player)
        {
            if (!Main.PlayersController.MultiverseData.Registered)
                Simulator.ShowPanel(PanelType.Login, player.Position, true);
            else
                Main.MultiverseController.JumpToWorld(Main.PlayersController.MultiverseData.WorldId, "Multiverse");
        }

        #endregion
    }
}
