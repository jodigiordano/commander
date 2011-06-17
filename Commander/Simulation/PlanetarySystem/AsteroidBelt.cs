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
            Simulator simulation,
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
            Asteroids = new List<Asteroid>(NbAsteroids);
 
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
                asteroid.Image.SizeX = (Main.Random.Next(20, 70) / 30.0f) * 3;

                CelestialBody.Move(this.RotationTime, (this.ActualRotationTime + asteroid.TimeOffset) % this.RotationTime, ref this.PositionBase, ref asteroid.Offset, ref asteroid.Image.position);

                Asteroids.Add(asteroid);
            }
        }


        //public override void Show()
        //{
        //    for (int i = 0; i < NbAsteroids; i++)
        //        Simulation.Scene.Add(Asteroids[i].Image);
        //}


        //public override void Hide()
        //{
        //    for (int i = 0; i < NbAsteroids; i++)
        //        Simulation.Scene.Remove(Asteroids[i].Image);
        //}

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < NbAsteroids; i++)
            {
                CelestialBody.Move(RotationTime, (ActualRotationTime + Asteroids[i].TimeOffset) % RotationTime, ref this.PositionBase, ref Asteroids[i].Offset, ref Asteroids[i].Image.position);
                Asteroids[i].Image.Rotation += Asteroids[i].RotationSpeed;
            }
        }


        public override void Draw()
        {
            for (int i = 0; i < NbAsteroids; i++)
            {
                //CorpsCeleste.Move(RotationTime, (ActualRotationTime + Asteroids[i].TimeOffset) % RotationTime, ref this.PositionBase, ref Asteroids[i].Offset, ref Asteroids[i].Image.position);
                //Asteroids[i].Image.Rotation += Asteroids[i].RotationSpeed;
                Simulation.Scene.Add(Asteroids[i].Image);
            }

            //Task t = Parallel.Start(Move);

            //AddToScene();

            //t.Wait();
        }


        //private void Move()
        //{
        //    foreach (var asteroid in Asteroids)
        //    {
        //        CorpsCeleste.Move(RotationTime, (ActualRotationTime + asteroid.TimeOffset) % RotationTime, ref this.PositionBase, ref asteroid.Offset, ref asteroid.Image.position);
        //        asteroid.Rotate();
        //    }
        //}


        //private void AddToScene()
        //{
        //    foreach (var asteroid in Asteroids)
        //        Simulation.Scene.Add(asteroid.Image);
        //}
    }
}
