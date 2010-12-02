namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    
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

            Bouton = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("emplacement"), Position, Scene);
            Bouton.PrioriteAffichage = priorite;
            Bouton.Origine = Bouton.Centre;

            BoutonCercle = new Cercle(Position, 16);
        }

        public override void Update(GameTime gameTime)
        {
            Pressed = false;

            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, Scene.Nom) &&
                Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, BoutonCercle))
            {
                Pressed = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Scene.ajouterScenable(Bouton);
        }
    }
}
