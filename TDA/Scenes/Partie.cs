namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;



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
            Simulation.Initialize();
            Simulation.EtreNotifierNouvelEtatPartie(doNouvelEtatPartie);

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            Main.ControleurJoueursConnectes.JoueurPrincipalDeconnecte += new ControleurJoueursConnectes.JoueurPrincipalDeconnecteHandler(doJoueurPrincipalDeconnecte);
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

            else if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu, Main.JoueursConnectes[0].Manette, this.Nom) ||
                     Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu2, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    (Simulation.Etat != EtatPartie.EnCours && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Main.JoueursConnectes[0].Manette, this.Nom)) ||
                    (Simulation.Etat != EtatPartie.EnCours && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom)))
            {
                effectuerTransition = true;
                ChoixTransition = "menu";
                AnimationTransition.In = false;
                AnimationTransition.Initialize();
            }

            else
            {
                Simulation.Update(gameTime);

                this.GameTime = gameTime;

                if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, Main.JoueursConnectes[0].Manette, this.Nom) && TempsEntreDeuxChangementMusique <= 0)
                {
                    changerMusique();
                    TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
                }

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
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            Core.Audio.Facade.pauserMusique(MusiqueSelectionnee, true, 1000);
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

        private void changerMusique()
        {
            Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, Preferences.TempsEntreDeuxChangementMusique - 50);
            String ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            Main.MusiquesDisponibles.Add(ancienneMusique);
            Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
        }
    }
}
