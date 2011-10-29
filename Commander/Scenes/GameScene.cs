namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : CommanderScene
    {
        public Simulator Simulator;
        public LevelDescriptor Level;

        public bool EditorMode;
        public EditorState EditorState;

        private FutureJobsController FutureJobs;

        private string MusicName;


        public GameScene(string name, LevelDescriptor level)
            : base(name)
        {
            Level = level;

            FutureJobs = new FutureJobsController();

            MusicName = "BattleMusic";

            EditorMode = false;
            EditorState = EditorState.Editing;
        }


        public override void Initialize()
        {
            Simulator = new Simulator(this, Level)
            {
                EditorMode = EditorMode,
                EditorState = EditorState
            };
            Simulator.Initialize();
            Simulator.AddNewGameStateListener(DoNewGameState);
            Inputs.AddListener(Simulator);
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
            XACTAudio.ChangeCategoryVolume("Turrets", 10);

            Simulator.OnFocus();
            Simulator.TeleportPlayers(false);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Simulator.OnFocusLost();
            Simulator.TeleportPlayers(true);
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }


        public void DoNewGameState(GameState newState)
        {
            if (newState == GameState.Won)
            {
                Simulator.ShowHelpBarMessage(Inputs.MasterPlayer, HelpBarMessage.GameWon);
                Main.MusicController.StopCurrentMusic();
                MusicName = "WinMusic";
                Main.MusicController.PlayOrResume(MusicName);
                Inputs.Active = false;

                FutureJobs.Add(ReactiveInputs, 750);
                //FutureJobs.Add(StopVibrations, 250);
            }

            else if (newState == GameState.Lost)
            {
                Simulator.ShowHelpBarMessage(Inputs.MasterPlayer, HelpBarMessage.GameLost);
                Main.MusicController.StopCurrentMusic();
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
            var player = (Commander.Player) p;

            if (Simulator.State == GameState.Won)
            {
                if (button == player.MouseConfiguration.Select)
                    NextLevel();

                else if (button == player.MouseConfiguration.AlternateSelect)
                    RetryLevel();

                else if (button == player.MouseConfiguration.Cancel)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == player.MouseConfiguration.Select)
                    RetryLevel();

                else if (button == player.MouseConfiguration.Cancel)
                    TransiteToWorld();
            }
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();

            if (Simulator.State == GameState.Won)
            {
                if (key == player.KeyboardConfiguration.NextLevel)
                    NextLevel();

                else if (key == player.KeyboardConfiguration.RetryLevel)
                    RetryLevel();

                else if (key == player.KeyboardConfiguration.GoBackToWorld)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (key == player.KeyboardConfiguration.NextLevel)
                    RetryLevel();

                else if (key == player.KeyboardConfiguration.GoBackToWorld)
                    TransiteToWorld();
            }
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (Simulator.State == GameState.Won)
            {
                if (button == player.GamepadConfiguration.Select)
                    NextLevel();

                else if (button == player.GamepadConfiguration.RetryLevel)
                    RetryLevel();
                else if (button == player.GamepadConfiguration.Cancel)
                    TransiteToWorld();
            }

            else if (Simulator.State == GameState.Lost)
            {
                if (button == player.GamepadConfiguration.Select)
                    RetryLevel();
                else if (button == player.GamepadConfiguration.Cancel)
                    TransiteToWorld();
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
                TransiteTo(Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? "World" : "Menu");
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.LeftCoin || key == player.KeyboardConfiguration.RightCoin)
                return;

            if (key == player.KeyboardConfiguration.Home)
                Main.Instance.Exit();

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
            var nextLevel = Main.CurrentWorld.World.GetNextLevel(Level.Infos.Id);

            if (nextLevel == null)
            {
                TransiteToWorld();
                return;
            }

            TransiteToNewGame(nextLevel);
        }


        private void TransiteToNewGame(LevelDescriptor level)
        {
            Main.MusicController.StopCurrentMusic();

            var newGame = new GameScene(Name == "Game1" ? "Game2" : "Game1", level)
            {
                EditorMode = EditorMode,
                EditorState = EditorState
            };
            newGame.Initialize();
            Main.CurrentGame = newGame;
            newGame.Simulator.AddNewGameStateListener(Main.CurrentWorld.DoNewGameState);

            if (Visuals.GetScene(newGame.Name) == null)
                Visuals.AddScene(newGame);
            else
                Visuals.UpdateScene(newGame.Name, newGame);

            TransiteTo(newGame.Name);
        }


        private void TransiteToWorld()
        {
            TransiteTo("World");
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
