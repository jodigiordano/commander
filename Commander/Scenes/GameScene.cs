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
        public MusicController MusicController;

        private LevelDescriptor Level;
        private string TransitingTo;
        private FutureJobsController FutureJobs;


        public GameScene(string name, LevelDescriptor level)
            : base(1280, 720)
        {
            Level = level;

            Name = name;
            TransitingTo = "";

            MusicController = new MusicController() { SwitchMusicRandomly = false };

            Simulator = new Simulator(this, level);
            Simulator.Initialize();
            Simulator.AddNewGameStateListener(DoNewGameState);
            Inputs.AddListener(Simulator);

            FutureJobs = new FutureJobsController();
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
            FutureJobs.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            EnableUpdate = true;

            MusicController.PlayMusic(false);

            Simulator.EnableInputs = true;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            if (TransitingTo == Main.SelectedWorld.Name || TransitingTo == "Menu")
                MusicController.PauseMusic();

            Simulator.EnableInputs = false;
        }


        public void DoNewGameState(GameState newState)
        {
            if (newState == GameState.Won)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameWon, Inputs.MasterPlayer.InputType);
                MusicController.SwitchTo(MusicContext.Won);
                Inputs.Active = false;

                FutureJobs.Add(ReactiveInputs, 500);
                FutureJobs.Add(StopVibrations, 250);
            }

            else if (newState == GameState.Lost)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameLost, Inputs.MasterPlayer.InputType);
                MusicController.SwitchTo(MusicContext.Lost);
                Inputs.Active = false;

                FutureJobs.Add(ReactiveInputs, 500);
                FutureJobs.Add(StopVibrations, 250);
            }

            else if (newState == GameState.Restart)
            {
                RetryLevel();
            }

            else if (newState == GameState.PausedToWorld)
            {
                TransiteToWorld();
            }

            if (newState == GameState.Won || newState == GameState.Lost)
                Simulator.EnableInputs = false;
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (Simulator.State == GameState.Won)
            {
                if (button == MouseConfiguration.Select)
                    NextLevel();

                else if (button == MouseConfiguration.Cancel)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == MouseConfiguration.Select)
                    RetryLevel();

                else if (button == MouseConfiguration.Cancel)
                    TransiteToWorld();
            }
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (Simulator.State == GameState.Won || Simulator.State == GameState.Lost)
            {
                if (key == KeyboardConfiguration.RetryLevel)
                    RetryLevel();

                else if (key == KeyboardConfiguration.Back)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Running)
            {
                if (key == KeyboardConfiguration.ChangeMusic)
                    MusicController.ChangeMusic(false);
            }
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (Simulator.State == GameState.Won)
            {
                if (button == GamePadConfiguration.Select)
                    NextLevel();

                else if (button == GamePadConfiguration.RetryLevel)
                    RetryLevel();
                else if (button == GamePadConfiguration.Cancel)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == GamePadConfiguration.Select)
                    RetryLevel();
                else if (button == GamePadConfiguration.Cancel)
                    TransiteToWorld();
            }

            else
            {
                if (button == GamePadConfiguration.ChangeMusic)
                    MusicController.ChangeMusic(false);
            }
        }


        public override void DoPlayerConnected(Core.Input.Player p)
        {
            var player = (Player) p;

            player.ChooseAssets();
        }

        
        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
            {
                TransitingTo = "Menu";
                TransiteTo(TransitingTo);
            }
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


        private void RetryLevel()
        {
            TransiteToNewGame(Level);
        }


        private void NextLevel()
        {
            var nextLevelId = Main.LevelsFactory.GetNextLevel(Main.SelectedWorld.WorldId, Level.Infos.Id);

            if (nextLevelId == -1)
            {
                TransiteToWorld();
                return;
            }

            TransiteToNewGame(Main.LevelsFactory.GetLevelDescriptor(nextLevelId));
        }


        private void TransiteToNewGame(LevelDescriptor level)
        {
            MusicController.StopMusic(false);

            var newGame = new GameScene(Name == "Game1" ? "Game2" : "Game1", level);
            Main.GameInProgress = newGame;
            newGame.Simulator.AddNewGameStateListener(Main.SelectedWorld.DoNewGameState);

            if (Visuals.GetScene(newGame.Name) == null)
                Visuals.AddScene(newGame);
            else
                Visuals.UpdateScene(newGame.Name, newGame);

            TransitingTo = newGame.Name;
            TransiteTo(TransitingTo);
        }


        private void TransiteToWorld()
        {
            TransitingTo = Main.SelectedWorld.Name;
            TransiteTo(TransitingTo);
        }


        private void ReactiveInputs()
        {
            Inputs.Active = true;
        }


        private void StopVibrations()
        {
            Inputs.StopAllVibrators();
        }
    }
}
