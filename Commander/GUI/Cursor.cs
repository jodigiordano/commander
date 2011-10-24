namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Cursor : ICollidable, IVisual
    {
        public float Speed { get; set; }
        public Circle Circle { get; set; }
        public Shape Shape { get; set; }
        public double FadeTime = 250;

        protected Scene Scene;
        public Image FrontImage;
        public Image BackImage;

        private Vector3 position;
        private Vector2 Size;
        private double VisualPriority;
        private bool Active;

        public byte MaxAlpha;


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
            MaxAlpha = 255;

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


        public virtual byte Alpha
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


        public virtual void Fade(int from, int to, double time)
        {
            var effect = Core.Visual.VisualEffects.Fade(from, to, 0, time);

            FrontImage.Alpha = (byte) from;

            Scene.VisualEffects.Add(FrontImage, effect);

            if (BackImage != null)
            {
                BackImage.Alpha = (byte) from;
                Scene.VisualEffects.Add(BackImage, effect);
            }

            Active = to >= from && to != 0;
        }


        public virtual void FadeIn()
        {
            Fade(Alpha, MaxAlpha, FadeTime);
        }


        public virtual void FadeOut()
        {
            Fade(Alpha, 0, FadeTime);
        }


        public virtual void FadeOut(byte to)
        {
            Fade(Alpha, to, FadeTime);
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


        #region ICollidable Membres
        //not implemented
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        PhysicalRectangle ICollidable.Rectangle { get; set; }
        Line ICollidable.Line { get; set; }

        #endregion

        Rectangle IVisual.VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }

        Vector2 IVisual.Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        Vector2 IVisual.Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        Color IVisual.Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
