﻿namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PushButton
    {
        public bool Pressed;

        private Scene Scene; 
        private Cursor Curseur;
        public Image Bouton;
        private Circle BoutonCercle;
        private Vector3 Position;


        public PushButton(Scene scene, Cursor curseur, Vector3 position, float priorite)
        {
            Scene = scene;
            Curseur = curseur;
            Position = position;

            Bouton = new Image("checkbox", Position);
            Bouton.VisualPriority = priorite;
            Bouton.SizeX = 4;

            BoutonCercle = new Circle(Position, 16);
        }


        public void doClick()
        {
            Pressed = Physics.CircleCicleCollision(Curseur.Circle, BoutonCercle);
        }


        public void Update(GameTime gameTime)
        {
            Pressed = false;
        }


        public void Draw()
        {
            Scene.Add(Bouton);
        }
    }
}