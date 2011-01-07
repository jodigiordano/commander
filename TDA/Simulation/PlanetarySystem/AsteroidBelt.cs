namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class AsteroidBelt : CorpsCeleste
    {
        private class Asteroid
        {
            public IVisible Image;
            public float RotationSpeed;
            public int MovingSpeed;
            public Vector3 Offset;
            public double TimeOffset;
        }

        private const int NbAsteroids = 200;
        private Asteroid[] Asteroids;


        public AsteroidBelt(
            Simulation simulation,
            string name,
            Vector3 basePosition,
            float radius,
            double rotationTime,
            List<IVisible> images,
            int startingPourcentage)
            : base(
                simulation,
                name,
                basePosition,
                Vector3.Zero,
                radius,
                rotationTime,
                images[0],
                startingPourcentage,
                Preferences.PrioriteSimulationCeintureAsteroides,
                false,
                0)
        {
            Asteroids = new Asteroid[NbAsteroids];
 
            for (int i = 0; i < NbAsteroids; i++)
            {
                Asteroid asteroid = new Asteroid();
                asteroid.Image = (IVisible) images[Main.Random.Next(0, images.Count)].Clone();
                asteroid.Image.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                asteroid.TimeOffset = Main.Random.Next((int)(-rotationTime/2), (int)(rotationTime/2));
                asteroid.Offset = new Vector3(Main.Random.Next(-50,50),Main.Random.Next(-50,50 ),0);
                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + asteroid.TimeOffset) % this.TempsRotation, ref this.PositionBase, ref asteroid.Offset, ref asteroid.Image.position);
                asteroid.Image.Couleur.A = 60;
                asteroid.RotationSpeed = Main.Random.Next(-100, 100) / 10000.0f;
                asteroid.MovingSpeed = Main.Random.Next(1, 10);
                asteroid.Image.Taille = (Main.Random.Next(10, 70) / 30.0f) * 3;

                Asteroids[i] = asteroid;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NbAsteroids; i++)
            {
                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + Asteroids[i].TimeOffset) % TempsRotation, ref this.PositionBase, ref Asteroids[i].Offset, ref Asteroids[i].Image.position);
                Asteroids[i].Image.Rotation += Asteroids[i].RotationSpeed;

                Simulation.Scene.ajouterScenable(Asteroids[i].Image);
            }
        }
    }
}
