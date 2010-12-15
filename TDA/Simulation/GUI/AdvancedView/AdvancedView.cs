﻿namespace TDA
{
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class AdvancedView
    {
        public bool Visible;

        private Simulation Simulation;
        private EnemiesLives EnemiesLives;
        private List<CorpsCeleste> CelestialBodies;
        private List<IVisible> Circles;

        private int CirclesCount = 50;

        public AdvancedView(Simulation simulation, List<Ennemi> enemies, List<CorpsCeleste> celestialBodies)
        {
            Simulation = simulation;
            Visible = false;
            CelestialBodies = celestialBodies;
            EnemiesLives = new EnemiesLives(simulation, enemies);

            Circles = new List<IVisible>();

            for (int i = 0; i < CirclesCount; i++)
            {
                IVisible iv = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("CercleBlanc"), Vector3.Zero);
                iv.Couleur = new Color(Color.Black, 200);
                iv.Origine = iv.Centre;
                iv.PrioriteAffichage = Preferences.PrioriteGUIEtoiles - 0.001f;

                Circles.Add(iv);
            }
        }

        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();

            int circlesCount = 0;

            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                CorpsCeleste corps = CelestialBodies[i];

                for (int j = 0; j < corps.Emplacements.Count; j++)
                {
                    Emplacement emplacement = corps.Emplacements[j];

                    if (emplacement.EstOccupe)
                    {
                        Circles[circlesCount].Position = emplacement.Tourelle.Position;
                        Circles[circlesCount].Couleur = new Color(emplacement.Tourelle.Couleur, 100);
                        Circles[circlesCount].Taille = (emplacement.Tourelle.ZoneActivation.Rayon / 100) * 2;
                        Simulation.Scene.ajouterScenable(Circles[circlesCount]);

                        circlesCount++;

                        if (circlesCount > CirclesCount)
                            return;
                    }
                }
            }
        }
    }
}