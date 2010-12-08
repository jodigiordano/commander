namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Input;
    using Microsoft.Xna.Framework.Input;



    class Partie : Scene
    {
        public Main Main;
        public Simulation Simulation;
        protected DescripteurScenario Scenario;
        protected GameTime GameTime = new GameTime();

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        public String MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;
        private String ChoixTransition;

        public Partie(Main main, DescripteurScenario scenario)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;
            Scenario = scenario;

            Nom = "Partie";

            EnPause = false;
            EstVisible = true;
            EnFocus = false;

            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);

            Simulation = new Simulation(Main, this, scenario);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            ChoixTransition = "chargement";
            effectuerTransition = true;
        }


        public override bool EstTerminee
        {
            get
            {
                return Simulation.Etat != EtatPartie.EnCours;
            }
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                {
                    switch (ChoixTransition)
                    {
                        case "menu": Core.Visuel.Facade.effectuerTransition("PartieVersNouvellePartie"); break;
                        case "chargement": Core.Visuel.Facade.effectuerTransition("PartieVersChargement"); break;
                    }
                }
            }

            else
            {
                Simulation.Update(gameTime);

                this.GameTime = gameTime;

                TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }


        protected override void UpdateVisuel()
        {
            Simulation.Draw(GameTime);

            if (effectuerTransition)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            effectuerTransition = true;
            AnimationTransition.In = true;
            AnimationTransition.Initialize();

            if (!Core.Audio.Facade.musiqueJoue(MusiqueSelectionnee))
                Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            else
                Core.Audio.Facade.reprendreMusique(MusiqueSelectionnee, true, 1000);

            Core.Input.Facade.AddListener(Simulation);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            Core.Audio.Facade.pauserMusique(MusiqueSelectionnee, true, 1000);

            Core.Input.Facade.RemoveListener(Simulation);
        }

        public void doNouvelEtatPartie(EtatPartie nouvelEtat)
        {
            if (nouvelEtat == EtatPartie.Gagnee || nouvelEtat == EtatPartie.Perdue)
            {
                Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, 500);
                Main.MusiquesDisponibles.Add(MusiqueSelectionnee);
                MusiqueSelectionnee = ((nouvelEtat == EtatPartie.Gagnee) ? "win" : "gameover") + Main.Random.Next(1, 3);
                Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            }
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            if (Simulation.Etat != EtatPartie.EnCours && (button == MouseButton.Right || button == MouseButton.Left))
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

            if (Simulation.Etat != EtatPartie.EnCours && (button == Buttons.A || button == Buttons.B))
                beginTransition();

            if (button == Buttons.DPadUp)
                beginChangeMusic();
        }


        private void beginTransition()
        {
            if (effectuerTransition)
                return;

            effectuerTransition = true;
            ChoixTransition = "menu";
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
        }


        private void beginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, Preferences.TempsEntreDeuxChangementMusique - 50);
            String ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            Main.MusiquesDisponibles.Add(ancienneMusique);
            Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);

            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }
    }
}
