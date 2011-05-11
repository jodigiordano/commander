namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class VaguesInfinies
    {
        private Simulation Simulation;
        private DescriptorInfiniteWaves Descripteur;
        private GenerateurVagues Generateur;
        private int DifficulteActuelle;
        private int NbVaguesGenerees;

        public VaguesInfinies(Simulation simulation, DescriptorInfiniteWaves descripteur)
        {
            Simulation = simulation;
            Descripteur = descripteur;
            NbVaguesGenerees = 0;
            DifficulteActuelle = Descripteur.StartingDifficulty - Descripteur.DifficultyIncrement;

            Generateur = new GenerateurVagues();
            Generateur.NbVagues = 1;
            Generateur.EnnemisPresents = Descripteur.Enemies;
            Generateur.ArgentEnnemis = Descripteur.MineralsPerWave;
            Generateur.QteEnnemis = Main.Random.Next((int) Descripteur.MinMaxEnemiesPerWave.X, (int) Descripteur.MinMaxEnemiesPerWave.Y);
        }

        public Wave getProchaineVague()
        {
            DifficulteActuelle += Descripteur.DifficultyIncrement;

            Generateur.DifficulteDebut = DifficulteActuelle;
            Generateur.DifficulteFin = DifficulteActuelle;

            Generateur.generer();

            if (NbVaguesGenerees == 0 && Descripteur.FirstOneStartNow)
                Generateur.Vagues[0].StartingTime = 0;

            NbVaguesGenerees++;

            return new Wave(Simulation, Generateur.Vagues[0]);
        }
    }
}
