namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CheckBox
    {
        public bool Checked;

        private Scene Scene;
        private Cursor Curseur;
        private Image Box;
        private Text CheckedRep;
        private Circle BoxCercle;
        private Vector3 Position;
        

        public CheckBox(Scene scene, Cursor curseur, Vector3 position, float priorite)
        {
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Box = new Image("checkbox", Position);
            Box.VisualPriority = priorite;
            Box.SizeX = 4;

            CheckedRep = new Text("X", "Pixelite", Color.White, Position);
            CheckedRep.SizeX = 2;
            CheckedRep.VisualPriority = priorite;

            BoxCercle = new Circle(Position, 16);
        }


        public void doClick()
        {
            if (Physics.CircleCicleCollision(Curseur.Circle, BoxCercle))
                Checked = !Checked;
        }


        public void Draw()
        {
            Scene.Add(Box);

            if (Checked)
                Scene.Add(CheckedRep);
        }
    }
}
