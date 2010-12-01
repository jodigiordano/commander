namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class VaguesInfinies
    {
        private Simulation Simulation;
        private DescripteurVaguesInfinies Descripteur;
        private GenerateurVagues Generateur;
        private int DifficulteActuelle;
        private int NbVaguesGenerees;

        public VaguesInfinies(Simulation simulation, DescripteurVaguesInfinies descripteur)
        {
            Simulation = simulation;
            Descripteur = descripteur;
            NbVaguesGenerees = 0;
            DifficulteActuelle = Descripteur.DifficulteDepart - Descripteur.IncrementDifficulte;

            Generateur = new GenerateurVagues();
            Generateur.NbVagues = 1;
            Generateur.EnnemisPresents = Descripteur.EnnemisPresents;
            Generateur.ArgentEnnemis = Descripteur.MinerauxParVague;
            Generateur.QteEnnemis = Main.Random.Next((int) Descripteur.MinMaxEnnemisParVague.X, (int) Descripteur.MinMaxEnnemisParVague.Y);
        }

        public Vague getProchaineVague()
        {
            DifficulteActuelle += Descripteur.IncrementDifficulte;

            Generateur.DifficulteDebut = DifficulteActuelle;
            Generateur.DifficulteFin = DifficulteActuelle;

            Generateur.generer();

            if (NbVaguesGenerees == 0 && Descripteur.FirstOneStartNow)
                Generateur.Vagues[0].TempsDepart = 0;

            NbVaguesGenerees++;

            return new Vague(Simulation, Generateur.Vagues[0]);
        }
    }
}
