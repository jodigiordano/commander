namespace EphemereGames.Commander.Simulation
{
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
            Vector3 basePosition,
            Size size,
            float speed,
            List<string> images,
            int startingPourcentage)
            : base(
                simulator,
                name,
                basePosition,
                Vector3.Zero,
                size,
                speed,
                null,
                startingPourcentage,
                Preferences.PrioriteSimulationCeintureAsteroides)
        {
            List<Image> representations = new List<Image>();

            for (int j = 0; j < images.Count; j++)
                representations.Add(new Image(images[j]));


            Asteroids = new List<Asteroid>(NbAsteroids);

            if (representations.Count == 0)
                representations.Add(new Image("Asteroid"));

            for (int i = 0; i < NbAsteroids; i++)
            {
                Asteroid asteroid = new Asteroid()
                {
                    Image = representations[Main.Random.Next(0, representations.Count)].Clone(),
                    TimeOffset = Main.Random.Next((int)(-speed/2), (int)(speed/2)),
                    Offset = new Vector3(Main.Random.Next(-50,50),Main.Random.Next(-50,50 ),0),
                    RotationSpeed = Main.Random.Next(-100, 100) / 10000.0f,
                    MovingSpeed = Main.Random.Next(1, 10)
                };

                asteroid.Image.VisualPriority = Preferences.PrioriteSimulationCeintureAsteroides;
                asteroid.Image.Color.A = 60;
                asteroid.Image.SizeX = (Main.Random.Next(20, 70) / 30.0f) * 3;

                CelestialBody.Move(Speed, (ActualRotationTime + asteroid.TimeOffset) % Speed, ref BasePosition, ref asteroid.Offset, ref asteroid.Image.position);

                Asteroids.Add(asteroid);
            }
        }


        public override void Update()
        {
            base.Update();

            foreach (var a in Asteroids)
            {
                CelestialBody.Move(Speed, (ActualRotationTime + a.TimeOffset) % Speed, ref BasePosition, ref a.Offset, ref a.Image.position);
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
