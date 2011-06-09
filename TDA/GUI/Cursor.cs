namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class Cursor : IObjetPhysique
    {
        public float Speed { get; set; }
        public Cercle Circle { get; set; }
        public Shape Shape { get; set; }
        public bool Active;
 
        private Vector3 position;
        private Scene Scene;
        private Main Main;
        private Image representation;
        private Vector2 Size;
        private double VisualPriority;
        private Color Color;


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, double visualPriority)
            : this(main, scene, initialPosition, speed, visualPriority, "Curseur", true)
        {

        }


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, double visualPriority, string imageName, bool visible)
        {
            Main = main;
            Scene = scene;
            Speed = speed;
            VisualPriority = visualPriority;
            Color = new Color(255, 255, 255, 0);
            Representation = imageName;
            Shape = Shape.Circle;
            Circle = new Cercle(initialPosition, Size.X / 4);
            Position = initialPosition;

            if (visible)
                FadeIn();
        }


        public string Representation
        {
            set
            {
                representation = new Image(value, position)
                {
                    VisualPriority = VisualPriority,
                    Color = Color
                };
                Size = representation.AbsoluteSize;
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


        public void FadeIn()
        {
            Scene.Effects.Add(representation, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 250));
            Active = true;
        }


        public void FadeOut()
        {
            Scene.Effects.Add(representation, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, 250));
            Active = false;
        }


        public void Show()
        {
            Scene.Add(representation);
        }


        public void Hide()
        {
            Scene.Remove(representation);
        }


        public void Draw()
        {
            representation.Position = this.Position;
        }


        #region IObjetPhysique Membres
        //not implemented
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Line { get; set; }

        #endregion
    }
}
