namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;

    class Sablier : DrawableGameComponent, IObjetPhysique
    {
        public double TempsRestant;

        public Vector3 Position { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Forme Forme { get; set; }

        private Scene Scene;
        private IVisible Representation;
        private List<IVisible> Pixels;
        private int Progression;
        private double TempsMinimum;
        private EffetRotation EffetRotation;

        private const int NB_PIXELS = 48;
        private const int TAILLE_PIXEL = 4;

        private static List<Vector3> PositionsRelatives = new List<Vector3>()
        {
            new Vector3(2, 2, 0),
            new Vector3(3, 2, 0),
            new Vector3(4, 2, 0),
            new Vector3(5, 2, 0),
            new Vector3(6, 2, 0),
            new Vector3(7, 2, 0),
            new Vector3(2, 3, 0),
            new Vector3(3, 3, 0),
            new Vector3(4, 3, 0),
            new Vector3(5, 3, 0),
            new Vector3(6, 3, 0),
            new Vector3(7, 3, 0),
            new Vector3(2, 4, 0),
            new Vector3(3, 4, 0),
            new Vector3(4, 4, 0),
            new Vector3(5, 4, 0),
            new Vector3(6, 4, 0),
            new Vector3(7, 4, 0),
            new Vector3(3, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(5, 5, 0),
            new Vector3(6, 5, 0),
            new Vector3(4, 6, 0),
            new Vector3(5, 6, 0),


            new Vector3(4, 14, 0),
            new Vector3(6, 14, 0),
            new Vector3(3, 14, 0),
            new Vector3(5, 14, 0),
            new Vector3(4, 13, 0),
            new Vector3(5, 13, 0),
            new Vector3(6, 13, 0),
            new Vector3(5, 12, 0),
            new Vector3(7, 14, 0),
            new Vector3(3, 13, 0),
            new Vector3(6, 12, 0),
            new Vector3(2, 14, 0),
            new Vector3(7, 13, 0),
            new Vector3(7, 12, 0),
            new Vector3(4, 12, 0),
            new Vector3(6, 11, 0),
            new Vector3(2, 13, 0),
            new Vector3(2, 12, 0),
            new Vector3(5, 11, 0),
            new Vector3(3, 12, 0),
            new Vector3(4, 11, 0),
            new Vector3(3, 11, 0),
            new Vector3(4, 10, 0),
            new Vector3(5, 10, 0)
        };


        public Sablier(Main main, Scene scene, double tempsMinimum, Vector3 position, float prioriteAffichage)
            : base(main)
        {
            this.Scene = scene;
            this.TempsMinimum = tempsMinimum;
            this.Position = position;

            Pixels = new List<IVisible>(NB_PIXELS);

            Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("sablier"), position);
            Representation.Taille = 4;
            Representation.Origine = Representation.Centre;
            Representation.VisualPriority = prioriteAffichage;

            for (int i = 0; i < NB_PIXELS; i++)
            {
                IVisible iv = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
                iv.Couleur = new Color(255, 0, 220, 255);
                iv.Taille = TAILLE_PIXEL;
                iv.Position = this.Position + (PositionsRelatives[i] - new Vector3(Representation.Origine.X, Representation.Origine.Y, 0)) * TAILLE_PIXEL;

                Vector3 origine3 = (this.Position - iv.Position);

                iv.Origine = new Vector2(origine3.X, origine3.Y) / TAILLE_PIXEL;
                iv.Position += origine3;
                iv.VisualPriority = Representation.VisualPriority - 0.01f;

                Pixels.Add(iv);
            }

            Progression = 0;

            Forme = Forme.Rectangle;
            Rectangle = new RectanglePhysique(Representation.Rectangle);
        }

        public bool Tourne
        {
            get
            {
                return (EffetRotation != null && !EffetRotation.Finished);
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (EffetRotation != null && EffetRotation.Finished)
            {
                Representation.Rotation = 0;
                Progression = 0;

                EffetRotation = null;
            }

            if (TempsRestant >= TempsMinimum)
                return;

            Progression = (int)Math.Ceiling(((TempsMinimum - TempsRestant) / TempsMinimum) * (NB_PIXELS / 2));
        }


        public override void Draw(GameTime gameTime)
        {
            for (int i = Progression; i < NB_PIXELS / 2; i++)
                Scene.ajouterScenable(Pixels[i]);

            for (int i = 0; i < Progression; i++)
                Scene.ajouterScenable(Pixels[NB_PIXELS / 2 + i]);

            for (int i = 0; i < NB_PIXELS; i++)
                Pixels[i].Rotation = Representation.Rotation;

            Scene.ajouterScenable(Representation);
        }

        public void tourner()
        {
            if (EffetRotation != null && !EffetRotation.Finished)
                return;

            EffetRotation = new EffetRotation();
            EffetRotation.Length = 500;
            EffetRotation.Progress = AbstractEffect.ProgressType.Linear;
            EffetRotation.Quantite = MathHelper.Pi;

            Scene.Effets.Add(Representation, EffetRotation);
        }

        public void doHide(double temps)
        {
            Scene.Effets.Add(Representation, PredefinedEffects.FadeOutTo0(255, 0, temps));

            foreach (var pixel in Pixels)
                Scene.Effets.Add(pixel, PredefinedEffects.FadeOutTo0(255, 0, temps));
        }

        public void doShow(double temps)
        {
            Scene.Effets.Add(Representation, PredefinedEffects.FadeInFrom0(255, 0, temps));

            foreach (var pixel in Pixels)
                Scene.Effets.Add(pixel, PredefinedEffects.FadeInFrom0(255, 0, temps));
        }

        #region IObjetPhysique Membres

        public float Vitesse
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
        public Cercle Cercle
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
