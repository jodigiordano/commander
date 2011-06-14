namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physics;


    public class FollowingCamera : Camera
    {
        public virtual IPhysicalObject FollowedObject  { get; set; }
        public Vector3 MaxDistance              { get; set; }
        public virtual Vector3 Speed            { get; set; }

        private static double TargetGameTime = (1.0 / 60) * 1000;


        public FollowingCamera(
            IPhysicalObject followed,
            Vector3 initialPosition,
            Vector3 maxDistance,
            Vector3 speed,
            bool manual,
            Vector2 origin,
            Camera ancienneCamera)
            : base(ancienneCamera)
        {
            this.FollowedObject = followed;
            this.MaxDistance = maxDistance;
            this.Speed = speed;
            this.Origin = origin;
            this.Manual = true;
            this.Position = initialPosition;
            this.Manual = manual;
        }


        public FollowingCamera(
            FollowingCamera other,
            Vector3 initialPosition,
            Vector3 maxDistance,
            Vector3 speed,
            bool manual,
            Vector2 origin)
            : base(other)
        {
            this.FollowedObject = other.FollowedObject;
            this.MaxDistance = maxDistance;
            this.Speed = speed;
            this.Origin = origin;
            this.Manual = true;
            this.Position = initialPosition;
            this.Manual = manual;
        }


        public override void Update(GameTime gameTime)
        {
            if (Manual)
                moveManually(gameTime);
            else
                moveAutomatically(gameTime);
        }


        protected virtual void moveManually(GameTime gameTime)
        {
            this.Position = new Vector3(this.FollowedObject.Position.X, this.FollowedObject.Position.Y, this.Position.Z);
        }


        private void moveAutomatically(GameTime gameTime)
        {
            Vector3 posCam = Position;
            Vector3 displacement = new Vector3(
                this.FollowedObject.Position.X - posCam.X,
                this.FollowedObject.Position.Y - posCam.Y,
                this.FollowedObject.Position.Z - posCam.Z);
            displacement.Normalize();

            float multiplicateur = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / TargetGameTime);
            displacement.X = displacement.X * Speed.X * multiplicateur;
            displacement.Y = displacement.Y * Speed.Y * multiplicateur;
            displacement.Z = displacement.Z * Speed.Z * multiplicateur;

            Vector3 totalDistance = new Vector3(
                this.FollowedObject.Position.X - posCam.X,
                this.FollowedObject.Position.Y - posCam.Y,
                this.FollowedObject.Position.Z - posCam.Z);

            displacement.X = (totalDistance.X < 0) ?
                Math.Max(displacement.X, totalDistance.X) :
                Math.Min(displacement.X, totalDistance.X);

            displacement.Y = (totalDistance.Y < 0) ?
                Math.Max(displacement.Y, totalDistance.Y) :
                Math.Min(displacement.Y, totalDistance.Y);

            displacement.Z = 0;

            this.Manual = true;
            this.Position = new Vector3(posCam.X + displacement.X, posCam.Y + displacement.Y, posCam.Z + displacement.Z);
            this.Manual = false;
        }
    }
}