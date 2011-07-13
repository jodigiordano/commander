namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : Scene
    {
        public Simulator Simulator;
        protected LevelDescriptor Level;
        protected GameTime GameTime = new GameTime();

        public string SelectedMusic;
        private double TempsEntreDeuxChangementMusique;

        public GameScene(LevelDescriptor level)
            : base(1280, 720)
        {
            Level = level;

            Name = "Partie";

            SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(SelectedMusic);

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
            Simulator.Update(gameTime);
            this.GameTime = gameTime;
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
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

            if (!Audio.IsMusicPlaying(SelectedMusic))
                Audio.PlayMusic(SelectedMusic, true, 1000, true);
            else
                Audio.ResumeMusic(SelectedMusic, true, 1000);

            Simulator.EnableInputs = true;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Audio.PauseMusic(SelectedMusic, true, 1000);

            Simulator.EnableInputs = false;
        }


        public void DoNewGameState(GameState newState)
        {
            if (newState == GameState.Won)
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameWon);
            else if (newState == GameState.Lost)
                Simulator.ShowHelpBarMessage(HelpBarMessage.GameLost);

            if (newState == GameState.Won || newState == GameState.Lost)
            {
                Audio.StopMusic(SelectedMusic, true, 500);
                Main.AvailableMusics.Add(SelectedMusic);
                SelectedMusic = ((newState == GameState.Won) ? "win" : "gameover") + Main.Random.Next(1, 3);
                Audio.PlayMusic(SelectedMusic, true, 1000, true);

                Simulator.EnableInputs = false;
            }
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
                    BeginChangeMusic();
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
                else if (button == GamePadConfiguration.Back)
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
                    BeginChangeMusic();
            }
        }

        
        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        private void BeginTransition()
        {
            TransiteTo(Main.SelectedWorld);
        }


        private void RetryLevel()
        {
            Simulator.Initialize();
            Simulator.SyncPlayers();
            Simulator.EnableInputs = true;
        }


        private void BeginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Audio.StopMusic(SelectedMusic, true, Preferences.TimeBetweenTwoMusics - 50);
            string ancienneMusique = SelectedMusic;
            SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(SelectedMusic);
            Main.AvailableMusics.Add(ancienneMusique);
            Audio.PlayMusic(SelectedMusic, true, 1000, true);

            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }
    }
}
