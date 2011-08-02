namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class MoonPath : Moon
    {
        private Path3D InnerPath;
        private double TimeFrontStart;
        private double TimeFrontEnd;


        public MoonPath(Simulator simulator, CelestialBody celestialBody, int alpha)
            : this(simulator, celestialBody, alpha, "lune" + Main.Random.Next(1, 5), Main.Random.Next(2, 4)) { }


        public MoonPath(Simulator simulator, CelestialBody celestialBody, int alpha, string imageName, int size)
            : base(simulator, celestialBody, alpha, imageName, size)
        {
            relativePosition = new Vector3(celestialBody.Circle.Radius + Main.Random.Next(10, 30), 0, 0);

            float relativePositionY = Main.Random.Next(10, 30);

            Vector3 relativePositionInverse = relativePosition;
            relativePositionInverse.X = -relativePositionInverse.X;

            double step = RotationTime / 6;

            InnerPath = new Path3D
            (
                new List<Vector3>()
                {
                    relativePosition + new Vector3(0, -relativePositionY, 0),
                    relativePosition + new Vector3(30, 0, 0),
                    relativePosition + new Vector3(0, relativePositionY, 0),
                    relativePositionInverse + new Vector3(0, relativePositionY, 0),
                    relativePositionInverse + new Vector3(-30, 0, 0),
                    relativePositionInverse + new Vector3(0, -relativePositionY, 0),
                    relativePosition + new Vector3(0, -relativePositionY, 0)
                },
                new List<double>()
                {
                    step * 0,
                    step * 1,
                    step * 2,
                    step * 3,
                    step * 4,
                    step * 5,
                    step * 6
                }
            );

            TimeFrontStart = step * 1;
            TimeFrontEnd = step * 4;
            Representation.VisualPriority = (ActualRotationTime >= TimeFrontStart && ActualRotationTime <= TimeFrontEnd) ?
                VisualPriorities.Default.Moon :
                CelestialBody.VisualPriority + 0.01f;

            float rotation = MathHelper.ToRadians(Main.Random.Next(0, 180));

            Matrix.CreateRotationZ(rotation, out RotationMatrix);

            InnerPath.GetPosition(ActualRotationTime, ref Position);
            Vector3.Transform(ref Position, ref RotationMatrix, out Position);
            Vector3.Add(ref Position, ref CelestialBody.position, out Position);
        }


        public override void Update()
        {
            base.Update();

            Representation.VisualPriority = (ActualRotationTime >= TimeFrontStart && ActualRotationTime <= TimeFrontEnd) ?
                VisualPriorities.Default.Moon :
                CelestialBody.VisualPriority + 0.01f;

            InnerPath.GetPosition(ActualRotationTime, ref Position);
            Vector3.Transform(ref Position, ref RotationMatrix, out Position);
            Vector3.Add(ref Position, ref CelestialBody.position, out Position);
        }
    }
}
