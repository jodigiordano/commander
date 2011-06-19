namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EnemiesLives
    {
        private List<List<Image>> Lives;
        private List<Enemy> Enemies;
        private Simulator Simulator;


        public EnemiesLives(Simulator simulator, List<Enemy> enemies)
        {
            Enemies = enemies;
            Simulator = simulator;

            Lives = new List<List<Image>>();

            for (int i = 0; i < 100; i++)
            {
                List<Image> visuals = new List<Image>();

                for (int j = 1; j < 7; j++)
                {
                    Image life = new Image("ViesEnnemis" + j)
                    {
                        VisualPriority = Preferences.PrioriteGUIVUeAvanceePointsVieEnnemis
                    };

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
                Enemy enemy = Enemies[i];

                float LivesRatio = enemy.LifePoints / enemy.StartingLifePoints;

                int index = (int)Math.Round((1 - LivesRatio) * 5);

                Lives[livesIndex][index].Position = enemy.Position - new Vector3(0, enemy.Image.AbsoluteSize.Y, 0);

                Simulator.Scene.Add(Lives[livesIndex][index]);
                livesIndex++;

                if (livesIndex >= 100)
                    return;
            }
        }
    }
}
