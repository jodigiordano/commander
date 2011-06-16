namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : Scene
    {
        public Main Main;
        public Simulation Simulation;
        protected ScenarioDescriptor Scenario;
        protected GameTime GameTime = new GameTime();

        public string MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;

        public GameScene(Main main, ScenarioDescriptor scenario)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;
            Scenario = scenario;

            Name = "Partie";

            MusiqueSelectionnee = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(MusiqueSelectionnee);

            Simulation = new Simulation(Main, this, scenario);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Visuals.Transite("PartieToChargement");
        }


        public override bool IsFinished
        {
            get
            {
                return Simulation.Etat == GameState.Lost || Simulation.Etat == GameState.Won;
            }
        }


        public GameState State
        {
            get { return Simulation.Etat; }
            set { Simulation.Etat = value; }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulation.Update(gameTime);
            this.GameTime = gameTime;
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        protected override void UpdateVisual()
        {
            Simulation.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            EnableUpdate = true;

            if (!Audio.IsMusicPlaying(MusiqueSelectionnee))
                Audio.PlayMusic(MusiqueSelectionnee, true, 1000, true);
            else
                Audio.UnpauseMusic(MusiqueSelectionnee, true, 1000);

            Inputs.AddListener(Simulation);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EnableUpdate = false;

            Audio.PauseMusic(MusiqueSelectionnee, true, 1000);

            Inputs.RemoveListener(Simulation);
        }


        public void doNouvelEtatPartie(GameState nouvelEtat)
        {
            if (nouvelEtat == GameState.Won || nouvelEtat == GameState.Lost)
            {
                Audio.StopMusic(MusiqueSelectionnee, true, 500);
                Main.AvailableMusics.Add(MusiqueSelectionnee);
                MusiqueSelectionnee = ((nouvelEtat == GameState.Won) ? "win" : "gameover") + Main.Random.Next(1, 3);
                Audio.PlayMusic(MusiqueSelectionnee, true, 1000, true);
            }
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if ((Simulation.Etat == GameState.Won || Simulation.Etat == GameState.Lost) &&
                (button == p.MouseConfiguration.Select || button == p.MouseConfiguration.Back))
                BeginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if ((key == p.KeyboardConfiguration.Cancel || key == p.KeyboardConfiguration.Back) && Simulation.HelpMode)
                return;

            if (key == p.KeyboardConfiguration.Back || key == p.KeyboardConfiguration.Cancel)
                BeginTransition();

            if (key == p.KeyboardConfiguration.ChangeMusic)
                BeginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Back || button == p.GamePadConfiguration.Back2)
                BeginTransition();

            if ((Simulation.Etat == GameState.Won || Simulation.Etat == GameState.Lost) &&
                (button == p.GamePadConfiguration.Select || button == p.GamePadConfiguration.Cancel))
                BeginTransition();

            if (button == p.GamePadConfiguration.ChangeMusic)
                BeginChangeMusic();
        }


        private void BeginTransition()
        {
            Visuals.Transite("PartieToNouvellePartie");
        }


        private void BeginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Audio.StopMusic(MusiqueSelectionnee, true, Preferences.TimeBetweenTwoMusics - 50);
            string ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(MusiqueSelectionnee);
            Main.AvailableMusics.Add(ancienneMusique);
            Audio.PlayMusic(MusiqueSelectionnee, true, 1000, true);

            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }
    }
}
