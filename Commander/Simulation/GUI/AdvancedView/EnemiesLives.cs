namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EnemiesLives
    {
        public List<Enemy> Enemies;
        public bool ShowAll;

        private List<List<Image>> Lives;
        private Simulator Simulator;

        private int MaxImages;


        public EnemiesLives(Simulator simulator)
        {
            Simulator = simulator;

            MaxImages = 100;

            Lives = new List<List<Image>>();

            for (int i = 0; i < MaxImages; i++)
            {
                List<Image> visuals = new List<Image>();

                for (int j = 1; j < 7; j++)
                    visuals.Add(new Image("ViesEnnemis" + j));

                Lives.Add(visuals);
            }

            ShowAll = false;
        }


        public void Draw()
        {
            int livesIndex = 0;

            DoShowIndividual(ref livesIndex);

            if (ShowAll)
                DoShowAll(ref livesIndex);


        }


        private void DoShowIndividual(ref int imagesIndex)
        {
            if (imagesIndex >= MaxImages)
                return;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy enemy = Enemies[i];

                if (enemy.BeingHit)
                    ShowLife(enemy, imagesIndex, true);

                imagesIndex++;

                if (imagesIndex >= MaxImages)
                    return;
            }
        }


        private void DoShowAll(ref int imagesIndex)
        {
            if (imagesIndex >= MaxImages)
                return;

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy enemy = Enemies[i];

                ShowLife(enemy, imagesIndex, false);

                imagesIndex++;

                if (imagesIndex >= MaxImages)
                    return;
            }
        }


        private void ShowLife(Enemy e, int imagesIndex, bool applyApha)
        {
            float LivesRatio = e.LifePoints / e.StartingLifePoints;

            int statusIndex = (int) Math.Round((1 - LivesRatio) * 5);

            var image = Lives[imagesIndex][statusIndex];

            image.Position = e.Position - new Vector3(0, e.Image.AbsoluteSize.Y, 0);
            image.VisualPriority = e.VisualPriority - 0.000001f;

            if (applyApha)
                image.Alpha = (byte) Math.Max(0, e.BeingHitPourc * 255);
            else
                image.Alpha = 255;

            Simulator.Scene.Add(Lives[imagesIndex][statusIndex]);
        }
    }
}
