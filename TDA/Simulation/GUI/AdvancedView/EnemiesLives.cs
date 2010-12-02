namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class EnemiesLives
    {
        private List<List<IVisible>> Lives;
        private List<Ennemi> Enemies;
        private Simulation Simulation;

        public EnemiesLives(Simulation simulation, List<Ennemi> enemies)
        {
            Enemies = enemies;
            Simulation = simulation;

            Lives = new List<List<IVisible>>();

            for (int i = 0; i < 100; i++)
            {
                List<IVisible> visuals = new List<IVisible>();

                for (int j = 1; j < 7; j++)
                {
                    IVisible life = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("ViesEnnemis" + j), Vector3.Zero, Simulation.Scene);
                    life.PrioriteAffichage = Preferences.PrioriteGUIVUeAvanceePointsVieEnnemis;
                    life.Origine = life.Centre;

                    visuals.Add(life);
                }

                Lives.Add(visuals);
            }
        }

        public void Draw()
        {
            int livesIndex = 0;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Ennemi enemy = Enemies[i];

                float LivesRatio = enemy.PointsVie / enemy.PointsVieDepart;

                int index = (int)Math.Round((1 - LivesRatio) * 5);

                Lives[livesIndex][index].Position = enemy.Position - new Vector3(0, enemy.RepresentationVivant.Texture.Height * enemy.RepresentationVivant.TailleVecteur.Y, 0);

                Simulation.Scene.ajouterScenable(Lives[livesIndex][index]);
                livesIndex++;

                if (livesIndex >= 100)
                    return;
            }
        }
    }
}
