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
        private IVisible Representation;
        private Text Text;


        public Cursor(Main main, Scene scene, Vector3 initialPosition, float speed, float visualPriority)
        {
            Main = main;
            Scene = scene;
            
            Vitesse = speed;

            Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Curseur"), initialPosition);
            Representation.Origine = Representation.Centre;
            Representation.VisualPriority = visualPriority;
            Representation.Couleur = new Color(255, 255, 255, 0);

            Forme = Forme.Cercle;
            Cercle = new Cercle(initialPosition, Representation.Rectangle.Width / 4);

            Position = initialPosition;

            doShow();

            Text = new Text("Pixelite");
            Text.SizeX = 1;
            Text.Color = Color.Black;
            Text.VisualPriority = visualPriority - 0.0000001f;
        }


        public int Width { get { return this.Representation.Rectangle.Width; } }
        public int Height { get { return this.Representation.Rectangle.Height; } }


        public void doShow()
        {
            Scene.Effets.Add(Representation, PredefinedEffects.FadeInFrom0(255, 0, 250));
            Actif = true;
        }


        public void doHide()
        {
            Scene.Effets.Add(Representation, PredefinedEffects.FadeOutTo0(255, 0, 250));
            Actif = false;
        }


        public void Draw()
        {
            Representation.Position = this.Position;

            Scene.ajouterScenable(Representation);

//#if DEBUG
//            Text.Data = Position.ToString();
//            Text.Position = Position;
//            Scene.ajouterScenable(Text);
//#endif
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
