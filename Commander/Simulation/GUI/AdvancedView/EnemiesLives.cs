namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EnemiesLives
    {
        public List<Enemy> Enemies;

        private List<List<Image>> Lives;
        private Simulator Simulator;


        public EnemiesLives(Simulator simulator)
        {
            Simulator = simulator;

            Lives = new List<List<Image>>();

            for (int i = 0; i < 100; i++)
            {
                List<Image> visuals = new List<Image>();

                for (int j = 1; j < 7; j++)
                    visuals.Add(new Image("ViesEnnemis" + j));

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

                var image = Lives[livesIndex][index];

                image.Position = enemy.Position - new Vector3(0, enemy.Image.AbsoluteSize.Y, 0);
                image.VisualPriority = enemy.VisualPriority - 0.000001f;

                Simulator.Scene.Add(Lives[livesIndex][index]);
                livesIndex++;

                if (livesIndex >= 100)
                    return;
            }
        }
    }
}
