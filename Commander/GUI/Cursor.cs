namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Cursor : IObjetPhysique
    {
        public float Speed { get; set; }
        public Circle Circle { get; set; }
        public Shape Shape { get; set; }

        protected Scene Scene;
        protected Image FrontImage;
        protected Image BackImage;

        private Vector3 position;
        private Vector2 Size;
        private double VisualPriority;
        private bool Active;


        public Cursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority)
            : this(scene, initialPosition, speed, visualPriority, "Curseur", true)
        {

        }


        public Cursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority, string imageName, bool visible)
        {
            Scene = scene;
            Speed = speed;
            VisualPriority = visualPriority;
            SetFrontImage(imageName, 1, new Color(255, 255, 255, 0));
            Shape = Shape.Circle;
            Circle = new Circle(initialPosition, Size.X / 4);
            Position = initialPosition;
            BackImage = null;

            if (visible)
                FadeIn();
        }


        public void SetFrontImage(string imageName, float size, Color color)
        {
            FrontImage = new Image(imageName, position)
            {
                VisualPriority = VisualPriority,
                Color = color,
                SizeX = size
            };
            Size = FrontImage.AbsoluteSize;
        }

        public void SetBackImage(string imageName, float size, Color color)
        {
            BackImage = new Image(imageName, position)
            {
                VisualPriority = VisualPriority + 0.00001,
                Color = color,
                SizeX = size
            };
            Size = BackImage.AbsoluteSize;
        }


        public byte Alpha
        {
            get { return FrontImage.Alpha; }
            set
            {
                FrontImage.Alpha = value;

                if (BackImage != null)
                    BackImage.Alpha = value;
            }
        }


        public Vector3 Position
        {
            get { return position; }
            set { position = value; Circle.Position = position; }
        }


        public int Width
        {
            get { return (int) (Size.X); }
        }


        public int Height
        {
            get { return (int) (Size.Y); }
        }


        public virtual void FadeIn()
        {
            var effect = Core.Visual.VisualEffects.Fade(0, 255, 0, 250);

            FrontImage.Alpha = 0;

            Scene.VisualEffects.Add(FrontImage, effect);

            if (BackImage != null)
            {
                BackImage.Alpha = 0;
                Scene.VisualEffects.Add(BackImage, effect);
            }

            Active = true;
        }


        public virtual void FadeOut()
        {
            var effect = Core.Visual.VisualEffects.Fade(0, 0, 0, 250);

            Scene.VisualEffects.Add(FrontImage, effect);

            if (BackImage != null)
            {
                Scene.VisualEffects.Add(BackImage, effect);
            }
            
            Active = false;
        }


        public virtual void Draw()
        {
            FrontImage.Position = Position;
            Scene.Add(FrontImage);

            if (BackImage != null)
            {
                BackImage.Position = Position;
                Scene.Add(BackImage);
            }
        }


        #region IObjetPhysique Membres
        //not implemented
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public PhysicalRectangle Rectangle { get; set; }
        public Line Line { get; set; }

        #endregion
    }
}
