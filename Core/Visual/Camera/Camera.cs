namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    public class Camera
    {
        public string Name;
        public Matrix Transform;

        public PhysicalRectangle Rectangle { get; private set; }

        private Vector2 origin;
        private Vector3 position;
        private float rotation;
        private float zoom = 1;
        private Vector2 Size;


        public Camera(Vector2 size)
        {
            Size = size;

            Initialize();
        }


        public Camera(Camera other)
        {
            if (other != null)
            {
                Position = other.position;
                Rotation = other.Rotation;
                Origin = other.Origin;
                Zoom = other.zoom;
                Name = "Unknown";
            }

            else
            {
                Initialize();
            }
        }


        private void Initialize()
        {
            Rectangle = new PhysicalRectangle();
            Position = new Vector3(0, 0, 500);
            Rotation = 0.0f;
            Origin = Vector2.Zero;
            Zoom = 1;
            Name = "Unknown";
        }


        public Vector2 Origin
        {
            get { return origin; }
            set
            {
                origin.X = value.X;
                origin.Y = value.Y;

                UpdateTransform();
            }
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;

                UpdateTransform();
            }
        }


        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;

                UpdateTransform();
            }
        }


        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;

                UpdateTransform();
            }
        }


        private void UpdateTransform()
        {
            Transform =
                Matrix.CreateScale(zoom) *
                Matrix.CreateTranslation(origin.X - position.X * zoom, origin.Y - position.Y * zoom, 0);

            Rectangle.X = (int) (position.X - Size.X / zoom / 2);
            Rectangle.Y = (int) (position.Y - Size.Y / zoom / 2);
            Rectangle.Width = (int) (Size.X / zoom);
            Rectangle.Height = (int) (Size.Y / zoom);
        }
    }
}