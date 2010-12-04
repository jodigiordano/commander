namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;

    enum EtatPartie
    {
        EnCours,
        Gagnee,
        Perdue
    }

    class ControleurScenario : DrawableGameComponent
    {
        public delegate void NouvelEtatPartieHandler(EtatPartie etat);
        public event NouvelEtatPartieHandler NouvelEtatPartie;

        public bool ModeDemo = false;
        public bool ModeEditeur = false;

        public List<CorpsCeleste> CorpsCelestes     { get { return Scenario.SystemePlanetaire; } }
        public VaguesInfinies VaguesInfinies        { get { return Scenario.VaguesInfinies; } }
        public LinkedList<Vague> Vagues             { get { return Scenario.Vagues; } }
        public Player Player                        { get { return Scenario.Player; } }
        public List<Tourelle> TourellesDepart       { get { return Scenario.Tourelles; } }
        public CorpsCeleste CorpsCelesteAProteger   { get { return Scenario.CorpsCelesteAProteger; } }

        public EtatPartie Etat                                          { get; private set; }
        private Simulation Simulation;
        private int CompteurVagues;
        private ParticuleEffectWrapper Etoiles;
        private double EmetteurEtoiles;
        public Scenario Scenario;


        public ControleurScenario(Simulation simulation, Scenario scenario)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Scenario = scenario;

            Etoiles = Simulation.Scene.Particules.recuperer("etoilesScintillantes");
            Etoiles.PrioriteAffichage = Preferences.PrioriteGUIEtoiles;
            EmetteurEtoiles = 0;

            CompteurVagues = 0;
        }


        protected virtual void notifyNouvelEtatPartie(EtatPartie nouvelEtat)
        {
            if (NouvelEtatPartie != null)
                NouvelEtatPartie(nouvelEtat);
        }


        public override void Update(GameTime gameTime)
        {
            EmetteurEtoiles += gameTime.ElapsedGameTime.TotalMilliseconds;

            //tmp
            //compteurTmp -= gameTime.ElapsedGameTime.TotalMilliseconds;

            //if (compteurTmp < 0 && !ModeDemo)
            //{
            //    notifyNouvelEtatPartie(EtatPartie.Perdue);
            //    compteurTmp = double.NaN;
            //}

            if (EmetteurEtoiles >= 100)
            {
                Vector2 v2 = Vector2.Zero;
                Etoiles.Emettre(ref v2);
                EmetteurEtoiles = 0;
            }

            if (Main.Random.Next(0, 1000) == 0)
                Simulation.Scene.Animations.inserer(Simulation.Scene, new AnimationEtoileFilante(Simulation));
        }


        public override void Draw(GameTime gameTime)
        {
            Simulation.Scene.ajouterScenable(Scenario.FondEcran);
        }


        public void doVagueTerminee()
        {
            CompteurVagues++;

            if (CompteurVagues == Vagues.Count && Etat == EtatPartie.EnCours && !ModeDemo && !ModeEditeur)
            {
                Etat = EtatPartie.Gagnee;

                if (Simulation.Main.Sauvegarde.Progression[Scenario.Numero] < 0)
                    Simulation.Main.Sauvegarde.Progression[Scenario.Numero] = Math.Abs(Simulation.Main.Sauvegarde.Progression[Scenario.Numero]);
                else if (Simulation.Main.Sauvegarde.Progression[Scenario.Numero] == 0)
                    Simulation.Main.Sauvegarde.Progression[Scenario.Numero] = 1;

                Core.Persistance.Facade.sauvegarderDonnee("savePlayer");

                notifyNouvelEtatPartie(Etat);
            }
        }


        public void doEnnemiAtteintFinTrajet(Ennemi ennemi, CorpsCeleste corpsCeleste)
        {
            if (Etat == EtatPartie.Gagnee)
                return;

            if (!(corpsCeleste is CeintureAsteroide))
                corpsCeleste.doTouche(ennemi);

            if (!Simulation.ModeDemo && Simulation.Etat != EtatPartie.Perdue)
            {
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteTouche");
            }

            if (corpsCeleste == CorpsCelesteAProteger)
                Player.Lives = (int) CorpsCelesteAProteger.PointsVie; //correct de caster?

            if (Player.Lives <= 0 && Etat == EtatPartie.EnCours && !ModeDemo && !ModeEditeur)
            {
                CorpsCelesteAProteger.doMeurt();
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");
                Etat = EtatPartie.Perdue;

                if ((Simulation.Main.Sauvegarde.Progression[Scenario.Numero] <= 0))
                    Simulation.Main.Sauvegarde.Progression[Scenario.Numero] -= 1;
                
                Core.Persistance.Facade.sauvegarderDonnee("savePlayer");

                notifyNouvelEtatPartie(Etat);
            }
        }

        public void doObjetDetruit(IObjetPhysique objet)
        {
            if (Etat == EtatPartie.Gagnee || Etat == EtatPartie.Perdue)
                return;

            CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            if (corpsCeleste == null)
                return;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");

            if (corpsCeleste == CorpsCelesteAProteger && !ModeDemo && !ModeEditeur)
            {
                Etat = EtatPartie.Perdue;

                notifyNouvelEtatPartie(Etat);
            }
        }
    }
}
