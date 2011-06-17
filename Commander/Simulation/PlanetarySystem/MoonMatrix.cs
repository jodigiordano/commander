namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class MoonMatrix : Moon
    {
        public MoonMatrix(Simulator simulation, CelestialBody celestialBody, int alpha)
            : this(simulation, celestialBody, alpha, "lune" + Main.Random.Next(1, 5), Main.Random.Next(2, 4)) { }


        public MoonMatrix(Simulator simulation, CelestialBody celestialBody, int alpha, string imageName, int size)
            : base(simulation, celestialBody, alpha, imageName, size)
        {
            relativePosition = new Vector3(celestialBody.Circle.Radius + Main.Random.Next(10, 30), 0, 0);

            Matrix.CreateRotationZ((float) (MathHelper.TwoPi / ActualRotationTime), out RotationMatrix);
            Vector3.Transform(ref relativePosition, ref RotationMatrix, out Position);
            Vector3.Add(ref Position, ref CelestialBody.position, out Position);
        }


        public override void Update()
        {
            base.Update();

            float radians = (float) (ActualRotationTime * (MathHelper.TwoPi / RotationTime));

            Matrix.CreateRotationZ(radians, out RotationMatrix);
            Vector3.Transform(ref relativePosition, ref RotationMatrix, out Position);
            Vector3.Add(ref Position, ref CelestialBody.position, out Position);
        }
    }
}
