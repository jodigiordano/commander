namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    public class Camera
    {
        public String Nom                   { get; set; }
        public Matrix Transformee;
        public Trajet2D VitesseDeplacement    { get; set; }
        public Trajet2D VitesseZoom           { get; set; }
        public Trajet2D VitesseRotation       { get; set; }
        public bool Manuelle                { get; set; }

        private Vector2 origine;
        public Vector2 Origine
        {
            get { return origine; }
            set
            {
                origine.X = value.X;
                origine.Y = value.Y;

                majTransformee();
            }
        }

        private Vector3 position;
        private Vector3 positionFinale;
        private Vector3 positionDelta;
        private bool initDeplacement = false;
        private double tempsDebutDeplacement = 0;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                positionFinale.X = value.X;
                positionFinale.Y = value.Y;
                positionFinale.Z = value.Z;

                bool zoom = position.Z != value.Z;

                if (Manuelle)
                {    
                    position.X = positionFinale.X;
                    position.Y = positionFinale.Y;
                    position.Z = positionFinale.Z;
                    positionDelta.X = positionDelta.Y = positionDelta.Z = 0;
                    majTransformee();
                }
                else
                {
                    initDeplacement = true;
                    initZoom = zoom;
                    positionDelta.X = positionFinale.X - position.X;
                    positionDelta.Y = positionFinale.Y - position.Y;
                    positionDelta.Z = positionFinale.Z - position.Z;
                }
            }
        }


        private float rotation;
        private float rotationFinale;
        private float rotationDelta;
        private bool initRotation = false;
        private double tempsDebutRotation = 0;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotationFinale = value;

                if (Manuelle)
                {
                    rotation = rotationFinale;
                    rotationDelta = 0.0f;
                    majTransformee();
                }
                else
                {
                    initRotation = true;
                    rotationDelta = rotationFinale - rotation;
                }
            }
        }

        private bool initZoom = false;
        private double tempsDebutZoom = 0;


        public Camera()
        {
            Init();
        }


        public Camera(Camera ancienneCamera)
        {
            if (ancienneCamera != null)
            {
                Manuelle = true;
                Position = ancienneCamera.position;
                Rotation = ancienneCamera.Rotation;
                VitesseDeplacement = ancienneCamera.VitesseDeplacement;
                VitesseRotation = ancienneCamera.VitesseRotation;
                VitesseZoom = ancienneCamera.VitesseZoom;
                Transformee = ancienneCamera.Transformee;
                Origine = ancienneCamera.Origine;
                Nom = "Inconnue";
                Manuelle = ancienneCamera.Manuelle;
            }

            else
            {
                Init();
            }
        }


        private void Init()
        {
            Manuelle = true;
            Position = new Vector3(0, 0, 500);
            Rotation = 0.0f;
            VitesseDeplacement = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 6000);
            VitesseRotation = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 9000);
            VitesseZoom = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 1500);
            Transformee = Matrix.Identity;
            Origine = Vector2.Zero;
            Nom = "Inconnue";
        }


        public virtual void Update(GameTime gameTime)
        {
            if (Manuelle)
                return;

            float tmpZoomDelta;
            float tmpRotation;

            // position
            float multiplicateur = 1 - VitesseDeplacement.position(gameTime.ElapsedGameTime.TotalMilliseconds).Y;
            position.X = positionFinale.X - positionDelta.X * multiplicateur;
            position.Y = positionFinale.Y - positionDelta.Y * multiplicateur;

            // zoom
            tmpZoomDelta = positionDelta.Z * (1 - VitesseZoom.position(gameTime.ElapsedGameTime.TotalMilliseconds).Y);
            position.Z = positionFinale.Z - tmpZoomDelta;

            // rotation
            tmpRotation = rotation;
            rotation = rotationFinale - (rotationDelta * (1 - VitesseRotation.position(gameTime.ElapsedGameTime.TotalMilliseconds).Y));

            majTransformee();
        }


        private void majTransformee()
        {
            Transformee =   
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(origine.X - position.X, origine.Y - position.Y, 0));
        }
    }
}