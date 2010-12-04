namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    class Emplacement : DrawableGameComponent, IObjetPhysique
    {
        public static Color[] CouleursDisponibles = new Color[]
        {
            new Color(255, 121, 33),
            new Color(255, 106, 0),
            new Color(255, 216, 0),
            new Color(255, 238, 58),
            new Color(76, 255, 0),
            new Color(0, 255, 33),
            new Color(0, 255, 144),
            new Color(0, 148, 255),
            new Color(0, 38, 255),
            new Color(72, 0, 255),
            new Color(178, 0, 255),
            new Color(255, 0, 220),
            new Color(255, 0, 110),
        };

        private Vector3 positionRelative;
        public Vector3 Position { get; set; }
        public Forme Forme { get; set; }
        private Scene Scene;
        private CorpsCeleste corpsCeleste;

        private Tourelle tourelle;
        public IVisible representation { get; set; }
        public IVisible Filtre;
        public RectanglePhysique Rectangle { get; set; }

        public float PrioriteAffichage
        {
            set
            {
                this.representation.PrioriteAffichage = value - 0.001f;
                this.Filtre.PrioriteAffichage = this.representation.PrioriteAffichage + 0.0001f;

                if (EstOccupe)
                    Tourelle.PrioriteAffichage = value;
            }
        }


        public Emplacement(Simulation simulation, Vector3 positionRelative, IVisible representation, CorpsCeleste corpsCeleste)
            : base(simulation.Main)
        {
            this.positionRelative = positionRelative;
            this.representation = representation;
            this.representation.origine = this.representation.Centre;

            this.corpsCeleste = corpsCeleste;
            this.Scene = simulation.Scene;
            this.Position = corpsCeleste.Position + positionRelative;

            this.Filtre = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), this.representation.Position, simulation.Scene);
            this.Filtre.TailleVecteur = new Vector2(this.representation.Rectangle.Width, this.representation.rectangle.Height);
            this.Filtre.Origine = this.Filtre.Centre;
            this.Filtre.Couleur = new Color(CouleursDisponibles[Main.Random.Next(0, CouleursDisponibles.Length)], 200);

            this.PrioriteAffichage = corpsCeleste.PrioriteAffichage;

            this.Forme = Forme.Rectangle;
            this.Rectangle = new RectanglePhysique
            (
                (int) this.Position.X - this.representation.Rectangle.Width,
                (int) this.Position.Y - this.representation.Rectangle.Height,
                this.representation.Rectangle.Width,
                this.representation.Rectangle.Height
            );
                
        }

        public bool EstOccupe { get { return tourelle != null; } }

        public Tourelle Tourelle
        {
            get { return tourelle; }

            set
            {
                tourelle = value;

                if (tourelle != null)
                {
                    tourelle.Position = this.Position;
                    tourelle.PrioriteAffichage = this.representation.PrioriteAffichage;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.Position = corpsCeleste.Position + positionRelative;

            this.Rectangle.X = (int) this.Position.X - this.representation.Rectangle.Width / 2;
            this.Rectangle.Y = (int) this.Position.Y - this.representation.Rectangle.Height / 2;

            if (tourelle != null)
                tourelle.Position = Position;
        }

        public override void Draw(GameTime gameTime)
        {
            if (tourelle == null)
            {
                representation.position = this.Position;
                Filtre.position = representation.position;
                Scene.ajouterScenable(representation);
                Scene.ajouterScenable(Filtre);
            }
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
