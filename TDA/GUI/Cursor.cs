namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class Cursor : IObjetPhysique
    {
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; Cercle.Position = position; }
        }

        public float Vitesse { get; set; }
        public Cercle Cercle { get; set; }
        public Forme Forme { get; set; }
        public bool Actif;

        private Scene Scene;
        private Main Main;
        private Image Representation;
        private Vector2 Size;
        private Text Text;


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority)
            : this(main, scene, initialPosition, speed, visualPriority, "Curseur", true)
        {

        }


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority, string imageName, bool visible)
        {
            Main = main;
            Scene = scene;

            Vitesse = speed;

            Representation = new Image(imageName, initialPosition)
            {
                VisualPriority = visualPriority,
                Color = new Color(255, 255, 255, 0)
            };
            Size = Representation.AbsoluteSize;

            Forme = Forme.Cercle;
            Cercle = new Cercle(initialPosition, Size.X / 4);

            Position = initialPosition;

            if (visible)
                doShow();

            Text = new Text("Pixelite")
            {
                SizeX = 1,
                Color = Color.Black,
                VisualPriority = visualPriority - 0.0000001f
            };
        }


        public int Width { get { return (int) (Size.X); } }
        public int Height { get { return (int) (Size.Y); } }


        public void doShow()
        {
            Scene.Effets.Add(Representation, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, 250));
            Actif = true;
        }


        public void doHide()
        {
            Scene.Effets.Add(Representation, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, 250));
            Actif = false;
        }


        public void Draw()
        {
            Representation.Position = this.Position;

            Scene.ajouterScenable(Representation);
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
