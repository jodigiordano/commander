namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    abstract class Moon
    {
        protected Simulator Simulation;
        protected CelestialBody CelestialBody;
        protected Matrix RotationMatrix;
        protected Vector3 Position;
        protected Vector3 relativePosition;
        protected double RotationTime;
        protected double ActualRotationTime;
        public Image Representation;
        protected bool Inversed;


        public Moon(Simulator simulation, CelestialBody celestialBody, int alpha, string imageName, int size)
        {
            Simulation = simulation;
            CelestialBody = celestialBody;

            Representation = new Image(imageName)
            {
                SizeX = size,
                VisualPriority = CelestialBody.VisualPriority + 0.000001f,
            };

            Representation.Color.A = (byte) alpha;

            Inversed = Main.Random.Next(0, 2) == 0;
            RotationTime = Main.Random.Next(3000, 10000);
            ActualRotationTime = Main.Random.Next(0, (int)RotationTime);
        }


        public virtual void Update()
        {
            if (Inversed)
            {
                ActualRotationTime -= Preferences.TargetElapsedTimeMs;

                if (ActualRotationTime < 0)
                    ActualRotationTime = RotationTime + ActualRotationTime;
            }

            else
            {
                ActualRotationTime += Preferences.TargetElapsedTimeMs;
                ActualRotationTime %= RotationTime;
            }
        }


        //public void Show()
        //{
        //    Simulation.Scene.Add(Representation);
        //}


        //public void Hide()
        //{
        //    Simulation.Scene.Remove(Representation);
        //}


        public void Draw()
        {
            Representation.Position = this.Position;
            Simulation.Scene.Add(Representation);
        }
    }
}
