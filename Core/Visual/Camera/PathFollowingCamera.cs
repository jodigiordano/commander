namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physics;


    public class PathFollowingCamera : FollowingCamera
    {
        public Path2D Path { get; private set; }

        new private IPhysicalObject Followed   { get; set; }
        new private Vector3 Speed       { get; set; }
        private double Length           { get; set; }


        public PathFollowingCamera(
            Path2D path,
            float zPosition,
            Vector2 origin,
            Camera other)
            : base(null, new Vector3(0, 0, zPosition), Vector3.Zero, Vector3.Zero, true, origin, other)
        {
            this.Path = path;
            this.Position = new Vector3(Path.positionDepart(), this.Position.Z);
            this.Manual = true;
            this.Length = 0;
        }


        public PathFollowingCamera(PathFollowingCamera other, Path2D path) :
            base(other, other.Position, other.MaxDistance, other.Speed, other.Manual, other.Origin)
        {
            this.Path = path;
            this.Length = Length;
        }


        public bool DestinationReached
        {
            get
            {
                return this.Path.position(Length) == this.Path.positionFin();
            }
        }


        protected override void moveManually(GameTime gameTime)
        {
            this.Length += gameTime.ElapsedGameTime.TotalMilliseconds;

            Vector2 posPath = Path.position(this.Length);
            this.Position = new Vector3(posPath.X, posPath.Y, this.Position.Z);
        }
    }
}