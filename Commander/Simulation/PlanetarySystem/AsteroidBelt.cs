namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class AsteroidBelt : CelestialBody
    {
        private class Asteroid
        {
            public Image Image;
            public float RotationSpeed;
            public int MovingSpeed;
            public Vector3 Offset;
            public double TimeOffset;


            public void Rotate()
            {
                Image.Rotation += RotationSpeed;
            }
        }

        private const int NbAsteroids = 150;
        private List<Asteroid> Asteroids;


        public AsteroidBelt(
            Simulator simulator,
            string name,
            Vector3 path,
            Vector3 basePosition,
            Size size,
            float speed,
            List<string> images,
            int startingPourcentage)
            : base(
                simulator,
                name,
                path,
                basePosition,
                0,
                size,
                speed,
                null,
                startingPourcentage,
                Preferences.PrioriteSimulationCeintureAsteroides,
                false)
        {
            List<Image> representations = new List<Image>();

            foreach (var image in images)
                representations.Add(new Image(image));

            if (representations.Count == 0)
                representations.Add(new Image("Asteroid"));

            Asteroids = new List<Asteroid>(NbAsteroids);

            var sizeMultiplier = Math.Max(path.X, path.Y) / Math.Min(Preferences.BackBuffer.X, Preferences.BackBuffer.Y);

            for (int i = 0; i < NbAsteroids; i++)
            {
                Asteroid asteroid = new Asteroid()
                {
                    Image = representations[Main.Random.Next(0, representations.Count)].Clone(),
                    TimeOffset = Main.Random.Next((int)(-speed/2), (int)(speed/2)),
                    Offset = basePosition + new Vector3(Main.Random.Next(-50,50),Main.Random.Next(-50,50 ),0),
                    RotationSpeed = Main.Random.Next(-100, 100) / 10000.0f,
                    MovingSpeed = Main.Random.Next(1, 10)
                };

                asteroid.Image.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                asteroid.Image.Alpha = 60;
                asteroid.Image.SizeX = (Main.Random.Next(20, 70) / 30.0f) * 3 * sizeMultiplier;

                CelestialBody.Move(Speed, (ActualRotationTime + asteroid.TimeOffset) % Speed, ref Path, ref asteroid.Offset, ref RotationMatrix, ref asteroid.Image.position);

                Asteroids.Add(asteroid);
            }
        }


        public override void Update()
        {
            base.Update();

            foreach (var a in Asteroids)
            {
                CelestialBody.Move(Speed, (ActualRotationTime + a.TimeOffset) % Speed, ref Path, ref a.Offset, ref RotationMatrix, ref a.Image.position);
                a.Image.Rotation += a.RotationSpeed;
            }
        }


        public override void Draw()
        {
            foreach (var a in Asteroids)
                Simulator.Scene.Add(a.Image);
        }
    }
}
