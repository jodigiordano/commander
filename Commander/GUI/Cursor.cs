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
        public bool Active;

        protected Scene Scene;
        protected Image Representation;

        private Vector3 position;
        private Vector2 Size;
        private double VisualPriority;
        private Color Color;


        public Cursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority)
            : this(scene, initialPosition, speed, visualPriority, "Curseur", true)
        {

        }


        public Cursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority, string imageName, bool visible)
        {
            Scene = scene;
            Speed = speed;
            VisualPriority = visualPriority;
            Color = new Color(255, 255, 255, 0);
            SetRepresentation(imageName, 1);
            Shape = Shape.Circle;
            Circle = new Circle(initialPosition, Size.X / 4);
            Position = initialPosition;

            if (visible)
                FadeIn();
        }


        public void SetRepresentation(string imageName, float size)
        {
            Representation = new Image(imageName, position)
            {
                VisualPriority = VisualPriority,
                Color = Color,
                SizeX = size
            };
            Size = Representation.AbsoluteSize;
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
            Scene.VisualEffects.Add(Representation, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 250));
            Active = true;
        }


        public virtual void FadeOut()
        {
            Scene.VisualEffects.Add(Representation, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 250));
            Active = false;
        }


        public virtual void Draw()
        {
            Representation.Position = this.Position;
            Scene.Add(Representation);
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
