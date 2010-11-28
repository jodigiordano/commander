namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Microsoft.Xna.Framework.Input;

    class Curseur : IObjetPhysique
    {
        public Vector3 Position { get; set; }
        public float Vitesse { get; set; }
        public Cercle Cercle { get; set; }
        public Forme Forme { get; set; }
        public bool Actif;

        private Scene Scene;
        private Main Main;
        private IVisible Representation;

        private Vector2 AnciennePositionSouris;

        public Curseur(Main main, Scene scene, Vector3 positionInitiale, float vitesse, float prioriteAffichage)
        {
            Main = main;
            Scene = scene;
            Position = positionInitiale;
            AnciennePositionSouris = new Vector2(positionInitiale.X, positionInitiale.Y);

            Vitesse = vitesse;

            Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Curseur"), positionInitiale, Scene);
            Representation.Origine = Representation.Centre;
            Representation.PrioriteAffichage = prioriteAffichage;
            Representation.Couleur = new Color(Color.White, 0);

            Forme = Forme.Cercle;
            Cercle = new Cercle(this.Position, Representation.Rectangle.Width / 4);

            doShow();
        }

        public void Update(GameTime gameTime)
        {
            if (!Actif)
                return;

#if XBOX || MANETTE_WINDOWS
            Vector2 positionThumb = Core.Input.Facade.positionThumbstick(Main.JoueursConnectes[0].Manette, true, Scene.Nom);

            Position += new Vector3(positionThumb.X * Vitesse, -positionThumb.Y * Vitesse, 0);
            //Position.X += positionThumb.X * Vitesse;
            //Position.Y -= positionThumb.Y * Vitesse;
#else
            Vector2 positionSouris = Core.Input.Facade.positionDeltaSouris(Main.JoueursConnectes[0].Manette, Scene.Nom);

            //if (positionSouris != Vector2.Zero)
            //    positionSouris.Normalize();

            //positionSouris *= Vitesse;

            Position += new Vector3(positionSouris, 0);
            //Position.X += positionSouris.X;
            //Position.Y += positionSouris.Y;

            //Vector2 nouvellePositionSouris = Core.Input.Facade.positionSouris(PlayerIndex.One, Scene.Nom);
            //Vector2 deltaPositionSouris = nouvellePositionSouris - AnciennePositionSouris;
            //Position += new Vector3(deltaPositionSouris, 0);
            //AnciennePositionSouris = nouvellePositionSouris;
#endif
            Position = new Vector3
            (
                MathHelper.Clamp(Position.X, -640 + Preferences.DeadZoneXbox.X + Representation.Rectangle.Width / 2, 640 - Preferences.DeadZoneXbox.X - Representation.Rectangle.Width / 2),
                MathHelper.Clamp(Position.Y, -370 + Preferences.DeadZoneXbox.Y + Representation.Rectangle.Height / 2, 370 - Preferences.DeadZoneXbox.Y - Representation.Rectangle.Height / 2),
                0
            );

            //Position.X = MathHelper.Clamp(Position.X, -640 + Preferences.DeadZoneXbox.X + Representation.Rectangle.Width / 2, 640 - Preferences.DeadZoneXbox.X - Representation.Rectangle.Width / 2);
            //Position.Y = MathHelper.Clamp(Position.Y, -370 + Preferences.DeadZoneXbox.Y + Representation.Rectangle.Height / 2, 370 - Preferences.DeadZoneXbox.Y - Representation.Rectangle.Height / 2);

            Cercle.Position = Position;
        }

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

        public Vector3 Direction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public float Rotation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public RectanglePhysique Rectangle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public Ligne Ligne
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
