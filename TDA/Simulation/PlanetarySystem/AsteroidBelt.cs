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
            public Image Image;
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
            List<Image> images,
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
                Asteroid asteroid = new Asteroid()
                {
                    Image = images[Main.Random.Next(0, images.Count)].Clone(),
                    TimeOffset = Main.Random.Next((int)(-rotationTime/2), (int)(rotationTime/2)),
                    Offset = new Vector3(Main.Random.Next(-50,50),Main.Random.Next(-50,50 ),0),
                    RotationSpeed = Main.Random.Next(-100, 100) / 10000.0f,
                    MovingSpeed = Main.Random.Next(1, 10)
                };

                asteroid.Image.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                asteroid.Image.Color.A = 60;
                asteroid.Image.SizeX = (Main.Random.Next(10, 70) / 30.0f) * 3;

                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + asteroid.TimeOffset) % this.TempsRotation, ref this.PositionBase, ref asteroid.Offset, ref asteroid.Image.position);

                Asteroids[i] = asteroid;
            }
        }


        public override void Show()
        {
            for (int i = 0; i < NbAsteroids; i++)
                Simulation.Scene.Add(Asteroids[i].Image);
        }


        public override void Hide()
        {
            for (int i = 0; i < NbAsteroids; i++)
                Simulation.Scene.Remove(Asteroids[i].Image);
        }


        public override void Draw()
        {
            for (int i = 0; i < NbAsteroids; i++)
            {
                CorpsCeleste.Deplacer(this.TempsRotation, (this.TempsRotationActuel + Asteroids[i].TimeOffset) % TempsRotation, ref this.PositionBase, ref Asteroids[i].Offset, ref Asteroids[i].Image.position);
                Asteroids[i].Image.Rotation += Asteroids[i].RotationSpeed;
            }
        }
    }
}
