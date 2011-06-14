namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class GameScene : Scene
    {
        public Main Main;
        public Simulation Simulation;
        protected ScenarioDescriptor Scenario;
        protected GameTime GameTime = new GameTime();

        private AnimationTransition AnimationTransition;
        public string MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;
        private string ChoixTransition;

        public GameScene(Main main, ScenarioDescriptor scenario)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;
            Scenario = scenario;

            Nom = "Partie";

            MusiqueSelectionnee = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(MusiqueSelectionnee);

            Simulation = new Simulation(Main, this, scenario);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

            AnimationTransition = new AnimationTransition(500, Preferences.PrioriteTransitionScene)
            {
                Scene = this
            };

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            ChoixTransition = "chargement";
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
            if (Transition != TransitionType.None)
                return;

            Simulation.Update(gameTime);
            this.GameTime = gameTime;
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            if (Transition == TransitionType.Out)
                switch (ChoixTransition)
                {
                    case "menu": EphemereGames.Core.Visual.Visuals.Transite("PartieToNouvellePartie"); break;
                    case "chargement": EphemereGames.Core.Visual.Visuals.Transite("PartieToChargement"); break;
                }

            Transition = TransitionType.None;
        }


        protected override void UpdateVisual()
        {
            Simulation.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Transition = TransitionType.In;

            if (!EphemereGames.Core.Audio.Audio.musiqueJoue(MusiqueSelectionnee))
                EphemereGames.Core.Audio.Audio.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            else
                EphemereGames.Core.Audio.Audio.reprendreMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Input.AddListener(Simulation);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            EphemereGames.Core.Audio.Audio.pauserMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Input.RemoveListener(Simulation);
        }


        public void doNouvelEtatPartie(GameState nouvelEtat)
        {
            if (nouvelEtat == GameState.Won || nouvelEtat == GameState.Lost)
            {
                EphemereGames.Core.Audio.Audio.arreterMusique(MusiqueSelectionnee, true, 500);
                Main.AvailableMusics.Add(MusiqueSelectionnee);
                MusiqueSelectionnee = ((nouvelEtat == GameState.Won) ? "win" : "gameover") + Main.Random.Next(1, 3);
                EphemereGames.Core.Audio.Audio.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            }
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if ((Simulation.Etat == GameState.Won || Simulation.Etat == GameState.Lost) &&
                (button == p.MouseConfiguration.Select || button == p.MouseConfiguration.Back))
                beginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if ((key == p.KeyboardConfiguration.Cancel || key == p.KeyboardConfiguration.Back) && Simulation.HelpMode)
                return;

            if (key == p.KeyboardConfiguration.Back || key == p.KeyboardConfiguration.Cancel)
                beginTransition();

            if (key == p.KeyboardConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Back || button == p.GamePadConfiguration.Back2)
                beginTransition();

            if ((Simulation.Etat == GameState.Won || Simulation.Etat == GameState.Lost) &&
                (button == p.GamePadConfiguration.Select || button == p.GamePadConfiguration.Cancel))
                beginTransition();

            if (button == p.GamePadConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = "menu";
        }


        private void beginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            EphemereGames.Core.Audio.Audio.arreterMusique(MusiqueSelectionnee, true, Preferences.TimeBetweenTwoMusics - 50);
            string ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.AvailableMusics[Main.Random.Next(0, Main.AvailableMusics.Count)];
            Main.AvailableMusics.Remove(MusiqueSelectionnee);
            Main.AvailableMusics.Add(ancienneMusique);
            EphemereGames.Core.Audio.Audio.jouerMusique(MusiqueSelectionnee, true, 1000, true);

            TempsEntreDeuxChangementMusique = Preferences.TimeBetweenTwoMusics;
        }
    }
}
