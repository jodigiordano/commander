namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AsteroidBelt : CelestialBody
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

        private List<string> ImagesNames;
        private List<Image> Images;


        public AsteroidBelt(string name, List<string> imagesNames, double visualPriority)
            : base(name, visualPriority)
        {
            ImagesNames = imagesNames;

            Images = new List<Image>();

            foreach (var image in imagesNames)
                Images.Add(new Image(image));

            if (Images.Count == 0)
                Images.Add(new Image("Asteroid"));

            Asteroids = new List<Asteroid>(NbAsteroids);

            CanSelect = false;
        }


        public override void Initialize()
        {
            base.Initialize();

            Asteroids.Clear();

            var sizeMultiplier =
                Math.Max(SteeringBehavior.Path.X, SteeringBehavior.Path.Y) /
                Math.Min(Preferences.BackBuffer.X, Preferences.BackBuffer.Y);

            for (int i = 0; i < NbAsteroids; i++)
            {
                Asteroid asteroid = new Asteroid()
                {
                    Image = Images[Main.Random.Next(0, Images.Count)].Clone(),
                    TimeOffset = Main.Random.Next((int) (-SteeringBehavior.Speed / 2), (int) (SteeringBehavior.Speed / 2)),
                    Offset = SteeringBehavior.BasePosition + new Vector3(Main.Random.Next(-50, 50), Main.Random.Next(-50, 50), 0),
                    RotationSpeed = Main.Random.Next(-100, 100) / 10000.0f,
                    MovingSpeed = Main.Random.Next(1, 10)
                };

                asteroid.Image.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                asteroid.Image.Alpha = 60;
                asteroid.Image.SizeX = (Main.Random.Next(20, 70) / 30.0f) * 3 * sizeMultiplier;

                MoveAsteroid(asteroid);
                //Planet.Move(Speed, (ActualRotationTime + asteroid.TimeOffset) % Speed, ref Path, ref asteroid.Offset, ref RotationMatrix, ref asteroid.Image.position);

                Asteroids.Add(asteroid);
            }
        }


        public override void Update()
        {
            base.Update();

            foreach (var a in Asteroids)
                MoveAsteroid(a);
        }


        public override void Draw()
        {
            foreach (var a in Asteroids)
                Simulator.Scene.Add(a.Image);
        }


        public override CelestialBodyDescriptor GenerateDescriptor()
        {
            return new AsteroidBeltCBDescriptor()
            {
                Name = Name,
                Images = ImagesNames
            };
        }


        private void MoveAsteroid(Asteroid asteroid)
        {
            SteeringBehavior.Move(Speed, asteroid.TimeOffset, ref asteroid.Offset, ref asteroid.Image.position);

            asteroid.Image.Rotation += asteroid.RotationSpeed;
        }
    }
}
