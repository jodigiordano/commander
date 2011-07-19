namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : Scene
    {
        public Simulator Simulator;
        private LevelDescriptor Level;
        public MusicController MusicController;


        public GameScene(LevelDescriptor level)
            : base(1280, 720)
        {
            Level = level;

            Name = "Partie";

            MusicController = new MusicController() { SwitchMusicRandomly = false };

            Simulator = new Simulator(this, level);
            Simulator.Initialize();
            Simulator.AddNewGameStateListener(DoNewGameState);
            Inputs.AddListener(Simulator);
        }


        public override bool IsFinished
        {
            get
            {
                return Simulator.State == GameState.Lost || Simulator.State == GameState.Won;
            }
        }


        public GameState State
        {
            get { return Simulator.State; }
            set { Simulator.State = value; }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();
            MusicController.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            EnableUpdate = true;

            MusicController.PlayMusic(false);

            Simulator.EnableInputs = true;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            MusicController.PauseMusic();

            Simulator.EnableInputs = false;
        }


        public void DoNewGameState(GameState newState)
        {
            if (newState == GameState.Won)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameWon);
                MusicController.SwitchTo(MusicContext.Won);
            }

            else if (newState == GameState.Lost)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameLost);
                MusicController.SwitchTo(MusicContext.Lost);
            }

            if (newState == GameState.Won || newState == GameState.Lost)
                Simulator.EnableInputs = false;
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (Simulator.State == GameState.Won)
            {
                if (button == MouseConfiguration.Select)
                {    //todo: go to next level
                }

                else if (button == MouseConfiguration.Back)
                    BeginTransition();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == MouseConfiguration.Select)
                    RetryLevel();

                else if (button == MouseConfiguration.Back)
                    BeginTransition();
            }
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (Simulator.State == GameState.Won || Simulator.State == GameState.Lost)
            {
                if (key == KeyboardConfiguration.RetryLevel)
                    RetryLevel();

                else if (key == KeyboardConfiguration.Back)
                    BeginTransition();
            }

            else if (Simulator.State == GameState.Running)
            {
                if ((key == KeyboardConfiguration.Cancel || key == KeyboardConfiguration.Back) && Simulator.HelpMode)
                    return;

                if (key == KeyboardConfiguration.Back)
                    BeginTransition();

                if (key == KeyboardConfiguration.ChangeMusic)
                    MusicController.ChangeMusic(false);
            }
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (Simulator.State == GameState.Won)
            {
                if (button == GamePadConfiguration.Select)
                {
                    //todo: next level
                }

                else if (button == GamePadConfiguration.RetryLevel)
                    RetryLevel();
                else if (button == GamePadConfiguration.Cancel)
                    BeginTransition();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == GamePadConfiguration.Select)
                    RetryLevel();
                else if (button == GamePadConfiguration.Cancel)
                    BeginTransition();
            }

            else
            {
                if (button == GamePadConfiguration.Back)
                    BeginTransition();

                if (button == GamePadConfiguration.ChangeMusic)
                    MusicController.ChangeMusic(false);
            }
        }

        
        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            if (p.State == PlayerState.Disconnected)
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


        private void BeginTransition()
        {
            TransiteTo(Main.SelectedWorld);
        }


        private void RetryLevel()
        {
            MusicController.ChangeMusic(true);

            Simulator.Initialize();
            Simulator.SyncPlayers();
            Simulator.EnableInputs = true;
        }
    }
}
