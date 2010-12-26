﻿namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Partie : Scene
    {
        public Main Main;
        public Simulation Simulation;
        protected DescripteurScenario Scenario;
        protected GameTime GameTime = new GameTime();

        private AnimationTransition AnimationTransition;
        public String MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;
        private String ChoixTransition;

        public Partie(Main main, DescripteurScenario scenario)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;
            Scenario = scenario;

            Nom = "Partie";

            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);

            Simulation = new Simulation(Main, this, scenario);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            ChoixTransition = "chargement";
        }


        public override bool EstTerminee
        {
            get
            {
                return Simulation.Etat != GameState.Running;
            }
        }


        protected override void UpdateLogique(GameTime gameTime)
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
                    case "menu": EphemereGames.Core.Visuel.Facade.Transite("PartieToNouvellePartie"); break;
                    case "chargement": EphemereGames.Core.Visuel.Facade.Transite("PartieToChargement"); break;
                }

            Transition = TransitionType.None;
        }


        protected override void UpdateVisuel()
        {
            Simulation.Draw(GameTime);

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;

            if (!EphemereGames.Core.Audio.Facade.musiqueJoue(MusiqueSelectionnee))
                EphemereGames.Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            else
                EphemereGames.Core.Audio.Facade.reprendreMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.AddListener(Simulation);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            EphemereGames.Core.Audio.Facade.pauserMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.RemoveListener(Simulation);
        }

        public void doNouvelEtatPartie(GameState nouvelEtat)
        {
            if (nouvelEtat == GameState.Won || nouvelEtat == GameState.Lost)
            {
                EphemereGames.Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, 500);
                Main.MusiquesDisponibles.Add(MusiqueSelectionnee);
                MusiqueSelectionnee = ((nouvelEtat == GameState.Won) ? "win" : "gameover") + Main.Random.Next(1, 3);
                EphemereGames.Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            }
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            if (Simulation.Etat != GameState.Running && (button == MouseButton.Right || button == MouseButton.Left))
                beginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            if (key == Keys.Enter || key == Keys.Escape)
                beginTransition();

            if (key == Keys.RightShift)
                beginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            if (button == Buttons.Start)
                beginTransition();

            if (Simulation.Etat != GameState.Running && (button == Buttons.A || button == Buttons.B))
                beginTransition();

            if (button == Buttons.DPadUp)
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

            EphemereGames.Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, Preferences.TempsEntreDeuxChangementMusique - 50);
            String ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            Main.MusiquesDisponibles.Add(ancienneMusique);
            EphemereGames.Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);

            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }
    }
}
