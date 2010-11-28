//=============================================================================
//
// Cam�ra
// Par d�faut, pointe au centre du buffer qui la contient. C'est son origine.
//
//=============================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    public class Camera : ICamera
    {
        //=============================================================================
        // �v�nements
        //=============================================================================

        public virtual event EventHandler CameraZoomee;

        protected virtual void OnCameraZoomee(EventArgs e)
        {
            if (CameraZoomee != null)
                CameraZoomee(this, e);
        }


        //=============================================================================
        // Attributs / Getters / Setters
        //=============================================================================

        /// <summary>
        /// Nom de la cam�ra. Utile pour le d�bogage
        /// </summary>
        public String Nom                   { get; set; }

        /// <summary>
        /// Transformation appliqu�e aux objets vues par la cam�ra
        /// D�placement et Rotation
        /// </summary>
        public Matrix Transformee;

        /// <summary>
        /// Vitesse de d�placement de la cam�ra en mode automatique
        /// </summary>
        public Trajet2D VitesseDeplacement    { get; set; }

        /// <summary>
        /// Vitesse de zoom de la cam�ra en mode automatique
        /// </summary>
        public Trajet2D VitesseZoom           { get; set; }

        /// <summary>
        /// Vitesse de rotation de la cam�ra en mode automatique
        /// </summary>
        public Trajet2D VitesseRotation       { get; set; }

        /// <summary>
        /// Mode de la cam�ra
        /// Vrai : un changement de position/rotation/etc. se fait tout de suite
        /// Faux : le changement est contr�l� par les vitesses
        /// </summary>
        public bool Manuelle                { get; set; }
        //public float FacteurZ               { get; set; }
        //public float DistanceFocus          { get; set; }

        /// <summary>
        /// Taille near/focus/far d'un objet vue par la cam�ra
        /// </summary>
        public Vector3 FacteurTaille        { get; set; }

        /// <summary>
        /// Opacit� near/focus/far d'un objet vue par la cam�ra
        /// </summary>
        public Vector3 FacteurOpacite       { get; set; }

        /// <summary>
        /// Flou near/focus/far d'un objet vue par la cam�ra
        /// </summary>
        public Vector3 FacteurDarker          { get; set; }

        /// <summary>
        /// Position absolue du focus
        /// </summary>
        public float PositionFocusZ
        {
            get { return position.Z - ChampVision.Y; }
        }

        /// <summary>
        /// Champ de vision de la cam�ra
        /// (Valeurs relatives � la position Z de la cam�ra)
        /// 
        /// X : Point de visibilit� le plus proche de la cam�ra (point o� la cam�ra ne voit plus rien)
        /// Y : Point de focus
        /// Z : Point de visibilit� le plus loin de la cam�ra (point o� la cam�ra ne voit plus rien)
        /// </summary>
        private Vector3 champVision;
        public Vector3 ChampVision
        {
            get
            {
                return champVision;
            }

            set
            {
#if DEBUG
                if (value.Z < value.Y || value.Y < value.X)
                    throw new Exception("il faut que X <= Y <= Z");
#endif

                champVision.X = value.X;
                champVision.Y = value.Y;
                champVision.Z = value.Z;
            }
        }


        /// <summary>
        /// Point de fuite qui donne l'effet que les objets fuient la cam�ra ou se rassemble � un point
        /// d�pendemment de leur position en Z par rapport � la cam�ra
        /// </summary>
        public Vector3 PointDeFuite
        {
            get
            {
                return new Vector3(position.X, position.Y, position.Z - champVision.Z);
            }
        }

        /// <summary>
        /// Dimension de la "lentille" de la cam�ra
        /// </summary>
        private Vector2 dimension;
        public Vector2 Dimension
        {
            get
            {
                return dimension;
            }

            set
            {
                dimension.X = value.X;
                dimension.Y = value.Y;
                maxDeplacementDeprecated = true;
            }
        }

        /// <summary>
        /// Rectangle qui fait le contour de la "lentille" de la cam�ra
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                    (int)(position.X - (dimension.X / 2.0f)),
                    (int)(position.Y - (dimension.Y / 2.0f)),
                    (int)dimension.X,
                    (int)dimension.Y);
            }
        }

        /// <summary>
        /// Longueur maximale du vecteur de d�placement vers ou fuyant le point de fuite
        /// </summary>
        private bool maxDeplacementDeprecated;
        private Vector2 maxDeplacement;
        public Vector2 MaxDeplacement
        {
            get
             {
                if (maxDeplacementDeprecated)
                {
                    float max = (float)Math.Sqrt(dimension.X * dimension.X + dimension.Y * dimension.Y) / 2.0f;
                    maxDeplacement.X = maxDeplacement.Y = max;
                    maxDeplacementDeprecated = false;
                }

                return maxDeplacement;
            }
        }


        /// <summary>
        /// Position de la cam�ra relative au buffer
        /// Ce buffer contient ce que voit la cam�ra
        /// Ex: si l'origine est Buffer.Centre et que la cam�ra est � la position (0,0), le centre de la cam�ra pointe au centre du buffer
        /// </summary>
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


        /// <summary>
        /// Position de la cam�ra relative � l'origine
        /// </summary>
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

                    if (zoom)
                        OnCameraZoomee(EventArgs.Empty);
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


        /// <summary>
        /// Rotation de la cam�ra sur elle-m�me
        /// </summary>
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


        //=============================================================================
        // Constructeurs
        //=============================================================================

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
                champVision.X = ancienneCamera.champVision.X;
                champVision.Y = ancienneCamera.champVision.Y;
                champVision.Z = ancienneCamera.champVision.Z;
                FacteurTaille = ancienneCamera.FacteurTaille;
                FacteurOpacite = ancienneCamera.FacteurOpacite;
                FacteurDarker = ancienneCamera.FacteurDarker;
                Dimension = ancienneCamera.dimension;
                VitesseDeplacement = ancienneCamera.VitesseDeplacement;
                VitesseRotation = ancienneCamera.VitesseRotation;
                VitesseZoom = ancienneCamera.VitesseZoom;
                Transformee = ancienneCamera.Transformee;
                Origine = ancienneCamera.Origine;
                Nom = "Inconnue";
                Manuelle = ancienneCamera.Manuelle;

                transfererListeners(ancienneCamera);
            }

            else
            {
                Init();
            }
        }

        /// <summary>
        /// Initialise une nouvelle cam�ra
        /// </summary>
        private void Init()
        {
            Manuelle = true;
            Position = new Vector3(0, 0, 500);
            Rotation = 0.0f;
            champVision.X = 0;
            champVision.Y = 500;
            champVision.Z = 1000;
            FacteurTaille = new Vector3(2, 1, 0.50f);
            FacteurOpacite = new Vector3(0, 1, 0);
            FacteurDarker = new Vector3(0, 0, 0);
            Dimension = new Vector2(Preferences.FenetreLargeur, Preferences.FenetreHauteur);
            VitesseDeplacement = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 6000);
            VitesseRotation = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 9000);
            VitesseZoom = Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, 1500);
            Transformee = Matrix.Identity;
            Origine = Vector2.Zero;
            Nom = "Inconnue";
        }


        //=============================================================================
        // Logique
        //=============================================================================

        /// <summary>
        /// Calcul le facteur taille d'un objet en fonction de sa distance � la cam�ra
        /// </summary>
        /// <param name="position">Position de l'objet</param>
        /// <returns></returns>
        public float calculerFacteurTaille(ref Vector3 position)
        {
            float deltaFocus = position.Z - this.PositionFocusZ;

            float facteurTaille;

            // proche de la cam�ra
            if (deltaFocus > 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Y - champ.X;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                //facteurTaille = 1.0f + pourc;
                Vector3 facteurT = FacteurTaille;
                facteurTaille = facteurT.Y + (facteurT.X - facteurT.Y) * pourc;
            }

            // loin de la cam�ra
            else if (deltaFocus < 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Z - champ.Y;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                //facteurTaille = 1.0f - pourc;
                facteurTaille = MathHelper.Clamp(1.0f - pourc, this.FacteurTaille.Z, 1.0f);
                //facteurTaille = MathHelper.Clamp(1.5f - pourc, 0.0f, 1.0f);
                //facteurTaille = this.FacteurTaille.Y - this.FacteurTaille.Z * pourc;
            }

            // en focus
            else 
            {
                facteurTaille = this.FacteurTaille.Y;
            }

            return facteurTaille;
        }

        /// <summary>
        /// Calcul le facteur d'opacit� d'un objet en fonction de sa distance � la cam�ra
        /// </summary>
        /// <param name="position">Position de l'objet</param>
        /// <returns></returns>
        public virtual float calculerFacteurOpacite(ref Vector3 position)
        {
            float deltaFocus = position.Z - this.PositionFocusZ;

            float facteurOpacite;

            // proche de la cam�ra
            if (deltaFocus > 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Y - champ.X;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                Vector3 facteurO = FacteurOpacite;
                facteurOpacite = Math.Max(facteurO.Y * (1.0f - pourc), facteurO.X);
            }

            // loin de la cam�ra
            else if (deltaFocus < 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Z - champ.Y;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                facteurOpacite = this.FacteurOpacite.Y;
            }

            // en focus
            else
            {
                facteurOpacite = this.FacteurOpacite.Y;
            }

            return facteurOpacite;
        }

        /// <summary>
        /// Calcul le facteur de flou d'un objet en fonction de sa distance � la cam�ra
        /// </summary>
        /// <param name="position">Position de l'objet</param>
        /// <returns></returns>
        public virtual float calculerFacteurDarker(ref Vector3 position)
        {
            float deltaFocus = position.Z - this.PositionFocusZ;

            float facteurDarker;

            // proche de la cam�ra
            if (deltaFocus > 0)
            {
                //float range = this.ChampVision.Y - this.ChampVision.X;
                //float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                //facteurFlou = Math.Min(this.FacteurFlou.X * pourc, this.FacteurFlou.X);
                facteurDarker = this.FacteurDarker.Y;
            }

            // loin de la cam�ra
            else if (deltaFocus < 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Z - champ.Y;
                float pourc = MathHelper.Clamp((Math.Abs(deltaFocus) / range) * 2, 0.0f, 1.0f);

                float facteurDZ = FacteurDarker.Z;
                facteurDarker = Math.Min(facteurDZ * pourc, facteurDZ);
            }

            // en focus
            else
            {
                facteurDarker = this.FacteurDarker.Y;
            }

            return facteurDarker;
        }

        /// <summary>
        /// Calcul le facteur de d�placement d'un objet en fonction de sa distance � la cam�ra
        /// </summary>
        /// <param name="position">Position de l'objet</param>
        /// <param name="facteurDeplacement">Facteur de d�placement de l'objet</param>
        public Vector2 calculerFacteurDeplacement(ref Vector3 position)
        {
            Vector2 facteurDeplacement;

            float deltaFocus = position.Z - this.PositionFocusZ;

            // proche de la cam�ra
            if (deltaFocus > 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Y - champ.X;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                Vector3 pdf = PointDeFuite;
                facteurDeplacement = new Vector2(position.X - pdf.X, position.Y - pdf.Y);
                Vector2.Multiply(ref facteurDeplacement, pourc, out facteurDeplacement);
            }

            // loin de la cam�ra
            else if (deltaFocus < 0)
            {
                Vector3 champ = ChampVision;
                float range = champ.Z - champ.Y;
                float pourc = MathHelper.Clamp(Math.Abs(deltaFocus) / range, 0.0f, 1.0f);

                Vector3 pdf = PointDeFuite;
                facteurDeplacement = new Vector2(pdf.X - position.X, pdf.Y - position.Y);
                Vector2.Multiply(ref facteurDeplacement, pourc, out facteurDeplacement);
            }

            // en focus
            else
            {
                facteurDeplacement = Vector2.Zero;
            }

            return facteurDeplacement;
        }

        public Vector2 calculerFacteurOrigine()
        {
            return origine;
        }

        /// <summary>
        /// Met � jour la cam�ra. Doit �tre appel� pour une cam�ra automatique.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (!Manuelle)
            {
                double tempsActuel = gameTime.TotalGameTime.TotalMilliseconds;

                if (initDeplacement)
                {
                    tempsDebutDeplacement = tempsActuel;
                    initDeplacement = false;
                }

                if (initRotation)
                {
                    tempsDebutRotation = tempsActuel;
                    initRotation = false;
                }

                if (initZoom)
                {
                    tempsDebutZoom = tempsActuel;
                    initZoom = false;
                }


                float tmpZoomDelta;
                float tmpRotation;

                // position
                float multiplicateur = 1 - VitesseDeplacement.position(tempsActuel - tempsDebutDeplacement).Y;
                position.X = positionFinale.X - positionDelta.X * multiplicateur;
                position.Y = positionFinale.Y - positionDelta.Y * multiplicateur;
                
                // zoom
                tmpZoomDelta = positionDelta.Z * (1 - VitesseZoom.position(tempsActuel - tempsDebutZoom).Y);
                position.Z = positionFinale.Z - tmpZoomDelta;
                
                // rotation
                tmpRotation = rotation;
                rotation = rotationFinale - (rotationDelta * (1 - VitesseRotation.position(tempsActuel - tempsDebutRotation).Y));

                majTransformee();

                if (tmpZoomDelta != 0)
                    OnCameraZoomee(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Met � jour la cam�ra. Doit �tre appel� pour une cam�ra automatique.
        /// </summary>
        /// <param name="tempsRelatif"></param>
        public virtual void Update(double tempsRelatif)
        {
            if (!Manuelle)
            {
                float tmpZoomDelta;
                float tmpRotation;

                // position
                float multiplicateur = 1 - VitesseDeplacement.position(tempsRelatif).Y;
                position.X = positionFinale.X - positionDelta.X * multiplicateur;
                position.Y = positionFinale.Y - positionDelta.Y * multiplicateur;

                // zoom
                tmpZoomDelta = positionDelta.Z * (1 - VitesseZoom.position(tempsRelatif).Y);
                position.Z = positionFinale.Z - tmpZoomDelta;

                // rotation
                tmpRotation = rotation;
                rotation = rotationFinale - (rotationDelta * (1 - VitesseRotation.position(tempsRelatif).Y));

                majTransformee();

                if (tmpZoomDelta != 0)
                    OnCameraZoomee(EventArgs.Empty);
            }
        }


        //=============================================================================
        // Helpers
        //=============================================================================

        /// <summary>
        /// Transform�e de la cam�ra pour la translation et la rotation. Le zoom d�pend de la position des objets.
        /// </summary>
        private void majTransformee()
        {
            Transformee =   
                //Matrix.CreateTranslation(new Vector3(-Origine, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(origine.X - position.X, origine.Y - position.Y, 0));
        }

        /// <summary>
        /// Transfert les listeners d'une cam�ra � une autre
        /// </summary>
        /// <remarks>
        /// Utile pour switcher d'une cam�ra � une autre � l'int�rieur d'une m�me sc�ne.
        ///  L'ancienne cam�ra perd ses listeners.
        /// </remarks>
        /// <param name="ancienneCamera"></param>
        protected void transfererListeners(Camera ancienneCamera)
        {
            if (ancienneCamera.CameraZoomee != null)
            {
                this.CameraZoomee += (EventHandler)ancienneCamera.CameraZoomee.Clone();
                ancienneCamera.CameraZoomee = null;
            }
        }


        /// <summary>
        /// D�termine si l'objet � la position +position+ est en focus
        /// </summary>
        /// <param name="position">Position d'un objet</param>
        /// <returns>En focus ou non</returns>
        public bool EnFocus(Vector3 position)
        {
            return position.Z == this.PositionFocusZ;
        }

        /// <summary>
        /// Est-ce qu'on doit appliquer le flou ou non
        /// </summary>
        public bool AppliquerFlou
        {
            get
            {
                return FacteurDarker != Vector3.Zero;
            }
        }
    }
}