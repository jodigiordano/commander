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
 
        private Vector3 position;
        private Scene Scene;
        private Image representation;
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
            representation = new Image(imageName, position)
            {
                VisualPriority = VisualPriority,
                Color = Color,
                SizeX = size
            };
            Size = representation.AbsoluteSize;
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


        public void FadeIn()
        {
            Scene.VisualEffects.Add(representation, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 250));
            Active = true;
        }


        public void FadeOut()
        {
            Scene.VisualEffects.Add(representation, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 250));
            Active = false;
        }


        public void Draw()
        {
            representation.Position = this.Position;
            Scene.Add(representation);
        }


        #region IObjetPhysique Membres
        //not implemented
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Line Line { get; set; }

        #endregion
    }
}
