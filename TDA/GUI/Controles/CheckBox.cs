namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class CheckBox
    {
        public bool Checked;

        private Scene Scene;
        private Cursor Curseur;
        private IVisible Box;
        private IVisible CheckedRep;
        private Cercle BoxCercle;
        private Vector3 Position;
        

        public CheckBox(Scene scene, Cursor curseur, Vector3 position, float priorite)
        {
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Box = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("emplacement"), Position);
            Box.VisualPriority = priorite;
            Box.Origine = Box.Centre;
            Box.Taille = 4;

            CheckedRep = new IVisible("X", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position);
            CheckedRep.Taille = 2;
            CheckedRep.VisualPriority = priorite;
            CheckedRep.Origine = CheckedRep.Centre;

            BoxCercle = new Cercle(Position, 16);
        }


        public void doClick()
        {
            if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, BoxCercle))
                Checked = !Checked;
        }


        public void Draw()
        {
            Scene.ajouterScenable(Box);

            if (Checked)
                Scene.ajouterScenable(CheckedRep);
        }
    }
}
