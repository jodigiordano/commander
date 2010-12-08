namespace TDA
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

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
        private IVisible Representation;


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority)
        {
            Main = main;
            Scene = scene;
            
            Vitesse = speed;

            Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Curseur"), initialPosition);
            Representation.Origine = Representation.Centre;
            Representation.PrioriteAffichage = visualPriority;
            Representation.Couleur = new Color(Color.White, 0);

            Forme = Forme.Cercle;
            Cercle = new Cercle(initialPosition, Representation.Rectangle.Width / 4);

            Position = initialPosition;

            doShow();
        }


        public int Width { get { return this.Representation.Rectangle.Width; } }
        public int Height { get { return this.Representation.Rectangle.Height; } }


        public void doShow()
        {
            Scene.Effets.ajouter(Representation, EffetsPredefinis.fadeInFrom0(255, 0, 250));
            Actif = true;
        }


        public void doHide()
        {
            Scene.Effets.ajouter(Representation, EffetsPredefinis.fadeOutTo0(255, 0, 250));
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
