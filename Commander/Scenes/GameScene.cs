﻿namespace EphemereGames.Commander
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
            : base(Vector2.Zero, 1280, 720)
        {
            Level = level;

            Name = "Partie";

            SelectedMusic = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(SelectedMusic);

            Simulator = new Simulator(this, level);
            Simulator.Initialize();
            Simulator.AddNewGameStateListener(doNouvelEtatPartie);
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

            EnableUpdate = true;

            if (!Audio.IsMusicPlaying(SelectedMusic))
                Audio.PlayMusic(SelectedMusic, true, 1000, true);
            else
                Audio.ResumeMusic(SelectedMusic, true, 1000);

            Inputs.AddListener(Simulator);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Audio.PauseMusic(SelectedMusic, true, 1000);

            Inputs.RemoveListener(Simulator);
        }


        public void doNouvelEtatPartie(GameState nouvelEtat)
        {
            if (nouvelEtat == GameState.Won || nouvelEtat == GameState.Lost)
            {
                Audio.StopMusic(SelectedMusic, true, 500);
                Main.AvailableMusics.Add(SelectedMusic);
                SelectedMusic = ((nouvelEtat == GameState.Won) ? "win" : "gameover") + Main.Random.Next(1, 3);
                Audio.PlayMusic(SelectedMusic, true, 1000, true);
            }
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (!p.Master)
                return;

            if ((Simulator.State == GameState.Won || Simulator.State == GameState.Lost) &&
                (button == MouseConfiguration.Select || button == MouseConfiguration.Back))
                BeginTransition();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (!p.Master)
                return;

            if ((key == KeyboardConfiguration.Cancel || key == KeyboardConfiguration.Back) && Simulator.HelpMode)
                return;

            if (key == KeyboardConfiguration.Back || key == KeyboardConfiguration.Cancel)
                BeginTransition();

            if (key == KeyboardConfiguration.ChangeMusic)
                BeginChangeMusic();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (!p.Master)
                return;

            if (button == GamePadConfiguration.Back || button == GamePadConfiguration.Back2)
                BeginTransition();

            if ((Simulator.State == GameState.Won || Simulator.State == GameState.Lost) &&
                (button == GamePadConfiguration.Select || button == GamePadConfiguration.Cancel))
                BeginTransition();

            if (button == GamePadConfiguration.ChangeMusic)
                BeginChangeMusic();
        }

        
        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (player.Master)
                TransiteTo("Chargement");
        }


        private void BeginTransition()
        {
            TransiteTo(Main.SelectedWorld);
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
