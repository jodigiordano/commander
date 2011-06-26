namespace EphemereGames.Core.Visual
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    public class Camera
    {
        public string Name              { get; set; }
        public Matrix Transform;
        public Path2D SpeedMovement   { get; set; }
        public Path2D SpeedZoom       { get; set; }
        public Path2D SpeedRotation   { get; set; }
        public bool Manual              { get; set; }

        private Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            set
            {
                origin.X = value.X;
                origin.Y = value.Y;

                updateTransform();
            }
        }

        private Vector3 position;
        private Vector3 positionEnd;
        private Vector3 positionDelta;
        private bool initializeMovement = false;
        private double movementDelay = 0;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                positionEnd.X = value.X;
                positionEnd.Y = value.Y;
                positionEnd.Z = value.Z;

                bool zoom = position.Z != value.Z;

                if (Manual)
                {    
                    position.X = positionEnd.X;
                    position.Y = positionEnd.Y;
                    position.Z = positionEnd.Z;
                    positionDelta.X = positionDelta.Y = positionDelta.Z = 0;
                    updateTransform();
                }
                else
                {
                    initializeMovement = true;
                    initializeZoom = zoom;
                    positionDelta.X = positionEnd.X - position.X;
                    positionDelta.Y = positionEnd.Y - position.Y;
                    positionDelta.Z = positionEnd.Z - position.Z;
                }
            }
        }


        private float rotation;
        private float rotationEnd;
        private float rotationDelta;
        private bool initializeRotation = false;
        private double rotationDelay = 0;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotationEnd = value;

                if (Manual)
                {
                    rotation = rotationEnd;
                    rotationDelta = 0.0f;
                    updateTransform();
                }
                else
                {
                    initializeRotation = true;
                    rotationDelta = rotationEnd - rotation;
                }
            }
        }

        private bool initializeZoom = false;
        private double zoomDelay = 0;


        public Camera()
        {
            Init();
        }


        public Camera(Camera other)
        {
            if (other != null)
            {
                Manual = true;
                Position = other.position;
                Rotation = other.Rotation;
                SpeedMovement = other.SpeedMovement;
                SpeedRotation = other.SpeedRotation;
                SpeedZoom = other.SpeedZoom;
                Transform = other.Transform;
                Origin = other.Origin;
                Name = "Inconnue";
                Manual = other.Manual;
            }

            else
            {
                Init();
            }
        }


        private void Init()
        {
            Manual = true;
            Position = new Vector3(0, 0, 500);
            Rotation = 0.0f;
            SpeedMovement = Path2D.CreerVitesse(Path3D.CurveType.Linear, 6000);
            SpeedRotation = Path2D.CreerVitesse(Path3D.CurveType.Linear, 9000);
            SpeedZoom = Path2D.CreerVitesse(Path3D.CurveType.Linear, 1500);
            Transform = Matrix.Identity;
            Origin = Vector2.Zero;
            Name = "Unknown";
        }


        public virtual void Update(GameTime gameTime)
        {
            if (Manual)
                return;

            float tmpZoomDelta;
            float tmpRotation;

            // position
            float multiplier = 1 - SpeedMovement.GetPosition(gameTime.ElapsedGameTime.TotalMilliseconds).Y;
            position.X = positionEnd.X - positionDelta.X * multiplier;
            position.Y = positionEnd.Y - positionDelta.Y * multiplier;

            // zoom
            tmpZoomDelta = positionDelta.Z * (1 - SpeedZoom.GetPosition(gameTime.ElapsedGameTime.TotalMilliseconds).Y);
            position.Z = positionEnd.Z - tmpZoomDelta;

            // rotation
            tmpRotation = rotation;
            rotation = rotationEnd - (rotationDelta * (1 - SpeedRotation.GetPosition(gameTime.ElapsedGameTime.TotalMilliseconds).Y));

            updateTransform();
        }


        private void updateTransform()
        {
            Transform =   
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(origin.X - position.X, origin.Y - position.Y, 0));
        }
    }
}