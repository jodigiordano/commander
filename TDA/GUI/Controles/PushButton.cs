namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    
    class PushButton : DrawableGameComponent
    {
        private Main Main;
        private Scene Scene; 
        private Cursor Curseur;
        public IVisible Bouton;
        private Cercle BoutonCercle;
        private Vector3 Position;

        public bool Pressed;

        public PushButton(Main main, Scene scene, Cursor curseur, Vector3 position, float priorite)
            : base(main)
        {
            Main = main;
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Bouton = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("emplacement"), Position);
            Bouton.VisualPriority = priorite;
            Bouton.Origine = Bouton.Centre;

            BoutonCercle = new Cercle(Position, 16);
        }


        public void doClick()
        {
            Pressed = EphemereGames.Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, BoutonCercle);
        }


        public override void Update(GameTime gameTime)
        {
            Pressed = false;
        }


        public override void Draw(GameTime gameTime)
        {
            Scene.ajouterScenable(Bouton);
        }
    }
}
