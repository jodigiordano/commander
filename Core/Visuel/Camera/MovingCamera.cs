namespace EphemereGames.Core.Visuel
{
    using Microsoft.Xna.Framework;


    public class MovingCamera : Camera
    {
        public virtual Vector3 Speed    { get; set; }

        private static double TargetGameTime = (1.0 / 60) * 1000;


        public MovingCamera(
            Vector3 initialPosition,
            Vector3 speed,
            Vector2 origin,
            Camera other)
            : base(other)
        {
            this.Speed = speed;
            this.Origin = origin;
            this.Manual = true;
            this.Position = initialPosition;
        }


        public MovingCamera(
            FollowingCamera other,
            Vector3 initialPosition,
            Vector3 speed,
            Vector2 origin)
            : base(other)
        {
            this.Speed = speed;
            this.Origin = origin;
            this.Manual = true;
            this.Position = initialPosition;
        }


        public override void Update(GameTime gameTime)
        {
            float multiplier = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / TargetGameTime);

            Vector3 displacement = new Vector3(
                Speed.X * multiplier,
                Speed.Y * multiplier,
                Speed.Z * multiplier);

            this.Manual = true;

            Vector3 pos = Position;
            this.Position = new Vector3(pos.X + displacement.X, pos.Y + displacement.Y, pos.Z + displacement.Z);
            this.Manual = false;
        }
    }
}
