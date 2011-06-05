namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    class Cursor : IObjetPhysique
    {
        public float Vitesse { get; set; }
        public Cercle Cercle { get; set; }
        public Forme Forme { get; set; }
        public bool Active;
 
        private Vector3 position;
        private Scene Scene;
        private Main Main;
        private Image representation;
        private Vector2 Size;
        private float VisualPriority;
        private Color Color;


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority)
            : this(main, scene, initialPosition, speed, visualPriority, "Curseur", true)
        {

        }


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority, string imageName, bool visible)
        {
            Main = main;
            Scene = scene;
            Vitesse = speed;
            VisualPriority = visualPriority;
            Color = new Color(255, 255, 255, 0);
            Representation = imageName;
            Forme = Forme.Cercle;
            Cercle = new Cercle(initialPosition, Size.X / 4);
            Position = initialPosition;

            if (visible)
                DoShow();
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
            set { position = value; Cercle.Position = position; }
        }


        public int Width
        {
            get { return (int) (Size.X); }
        }


        public int Height
        {
            get { return (int) (Size.Y); }
        }


        public void DoShow()
        {
            Scene.Effets.Add(representation, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 250));
            Active = true;
        }


        public void DoHide()
        {
            Scene.Effets.Add(representation, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, 250));
            Active = false;
        }


        public void Draw()
        {
            representation.Position = this.Position;

            Scene.ajouterScenable(representation);
        }


        #region IObjetPhysique Membres
        //not implemented
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Ligne { get; set; }

        #endregion
    }
}
