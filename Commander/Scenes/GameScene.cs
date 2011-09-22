namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : CommanderScene
    {
        public Simulator Simulator;
        public LevelDescriptor Level;

        private string TransitingTo;
        private FutureJobsController FutureJobs;

        private string MusicName;


        public GameScene(string name, LevelDescriptor level)
            : base(name)
        {
            Level = level;

            TransitingTo = "";

            Simulator = new Simulator(this, level);
            Simulator.Initialize();
            Simulator.AddNewGameStateListener(DoNewGameState);
            Inputs.AddListener(Simulator);

            FutureJobs = new FutureJobsController();

            MusicName = "Raindrop";
        }


        public override void CleanUp()
        {
            base.CleanUp();
            Simulator.CleanUp();
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
            FutureJobs.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 500);

            EnableUpdate = true;

            Main.MusicController.PlayOrResume(MusicName);

            Simulator.OnFocus();
            Simulator.TeleportPlayers(false);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Simulator.OnFocusLost();
            Simulator.TeleportPlayers(true);
        }


        public void DoNewGameState(GameState newState)
        {
            if (newState == GameState.Won)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameWon, Inputs.MasterPlayer.InputType);
                MusicName = "WinMusic";
                Main.MusicController.PlayOrResume(MusicName);
                Inputs.Active = false;

                FutureJobs.Add(ReactiveInputs, 750);
                //FutureJobs.Add(StopVibrations, 250);
            }

            else if (newState == GameState.Lost)
            {
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameLost, Inputs.MasterPlayer.InputType);
                MusicName = "LoseMusic";
                Main.MusicController.PlayOrResume(MusicName);
                Inputs.Active = false;

                FutureJobs.Add(ReactiveInputs, 750);
                //FutureJobs.Add(StopVibrations, 250);
            }

            else if (newState == GameState.Restart)
            {
                RetryLevel();
            }

            else if (newState == GameState.PausedToWorld)
            {
                TransiteToWorld();
            }
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (Simulator.State == GameState.Won)
            {
                if (button == MouseConfiguration.Select)
                    NextLevel();

                else if (button == MouseConfiguration.AlternateSelect)
                    RetryLevel();

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
                //if (key == KeyboardConfiguration.ChangeMusic)
                //    MusicController.ChangeMusic(false);
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
                //if (button == GamePadConfiguration.ChangeMusic)
                //    MusicController.ChangeMusic(false);
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


        public void StopMusic()
        {
            Main.MusicController.Stop(MusicName);
        }


        private void RetryLevel()
        {
            TransiteToNewGame(Level);
        }


        private void NextLevel()
        {
            var nextLevelId = Main.LevelsFactory.GetNextLevel(Main.SelectedWorld.Id, Level.Infos.Id);

            if (nextLevelId == -1)
            {
                TransiteToWorld();
                return;
            }

            TransiteToNewGame(Main.LevelsFactory.GetLevelDescriptor(nextLevelId));
        }


        private void TransiteToNewGame(LevelDescriptor level)
        {
            Main.MusicController.StopCurrentMusic();

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
