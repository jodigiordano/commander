namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class Vaisseau : DrawableGameComponent, IObjetPhysique
    {
        public Vector3 Position                 { get; set; }
        public float Vitesse                    { get; set; }
        public float Masse                      { get; set; }
        public Vector3 Direction                { get; set; }
        public float Rotation                   { get; set; }
        public float ResistanceRotation         { get; set; }
        public Forme Forme                      { get; set; }
        public Cercle Cercle                    { get; set; }
        public RectanglePhysique Rectangle      { get; set; }
        public Ligne Ligne                      { get; set; }
        public CorpsCeleste CorpsCelesteDepart;
        public bool CibleAtteinte;

        private Vector3 positionVisee;
        public bool EnAttaque;
        public Vector3 PositionVisee            { get { return positionVisee; } set { positionVisee = value; EnAttaque = true; CibleAtteinte = false; } }
        public float PuissanceProjectile;

        protected Simulation Simulation;
        public IVisible Representation;
        public float RotationMaximaleRad;

        private List<Projectile> Projectiles;
        public double CadenceTir;
        private double CompteurDernierTir;

        protected Matrix MatriceRotation;

        public Vaisseau(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Vaisseau"), this.Position);
            this.Representation.Taille = 4;
            this.Representation.Origine = this.Representation.Centre;
            
            this.Position = Vector3.Zero;
            this.Vitesse = 4;
            this.Masse = 1;
            this.Direction = new Vector3(1, 0, 0);
            this.Rotation = 0;
            this.ResistanceRotation = 0;
            this.Forme = Forme.Cercle;
            this.Cercle = new Cercle(this.Position, this.Representation.Rectangle.Width / 2);

            this.RotationMaximaleRad = 0.10f;
            this.CadenceTir = 300;
            this.CompteurDernierTir = 0;

            this.Projectiles = new List<Projectile>();
        }

        public override void Update(GameTime gameTime)
        {
            this.Cercle.Position = this.Position;

            CompteurDernierTir += gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public virtual void doModeAutomatique()
        {
            // Trouver la direction visée
            Vector3 directionVisee = PositionVisee - Position;
            Vector3 direction = Direction;

            // Trouver l'angle d'alignement
            float angle = signedAngle(ref directionVisee, ref direction);

            // Trouver la rotation nécessaire pour s'enligner
            float rotation = MathHelper.Clamp(RotationMaximaleRad, 0, Math.Abs(angle));

            if (angle > 0)
                rotation = -rotation;

            // Appliquer la rotation
            Matrix.CreateRotationZ(rotation, out MatriceRotation);
            Vector3.Transform(ref direction, ref MatriceRotation, out direction);

            if (direction != Vector3.Zero)
                direction.Normalize();

            Direction = direction;

            Position += Direction * Vitesse;

            if ((PositionVisee - Position).LengthSquared() <= 400)
            {
                EnAttaque = false;
                CibleAtteinte = true;
            }
        }


        public List<Projectile> ProjectilesCeTick(GameTime gameTime)
        {
            Projectiles.Clear();


            if (CompteurDernierTir >= CadenceTir)
            {
                CompteurDernierTir = 0;

                Matrix matriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(Direction, matriceRotation);
                directionUnitairePerpendiculaire.Normalize();

                Vector3 translation = directionUnitairePerpendiculaire * Main.Random.Next(-17, 17);

                ProjectileBase p = Projectile.PoolProjectilesBase.recuperer();
                p.Scene = Simulation.Scene;
                p.Position = this.Position + translation;
                p.Direction = this.Direction;
                p.PointsAttaque = PuissanceProjectile;
                p.PrioriteAffichage = this.Representation.VisualPriority + 0.001f;
                p.Initialize();

                Simulation.Scene.Particules.retourner(p.RepresentationDeplacement);
                p.RepresentationDeplacement = null;

                Projectiles.Add(p);
            }

            if (Projectiles.Count != 0)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceTire" + Main.Random.Next(1, 4));

            return Projectiles;
        }


        public override void Draw(GameTime gameTime)
        {
            Representation.Position = this.Position;
            Representation.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);

            Simulation.Scene.ajouterScenable(Representation);
            //Simulation.Scene.ajouterScenable(new RectangleVisuel(new Rectangle((int)PositionVisee.X - 10, (int)PositionVisee.Y - 10, 20, 20), Color.Red));
            //Simulation.Scene.ajouterScenable(new LigneVisuel(this.Position, this.PositionVisee, Color.Red, 4));
            //Simulation.Scene.ajouterScenable(new LigneVisuel(this.Position, this.Position + this.Direction * 50, Color.Green, 4));
        }


        public void doDisparaitre()
        {
            float distance = (CorpsCelesteDepart.Position - Position).Length();

            double tempsRequis = (distance / Vitesse) * 16.33f;

            Simulation.Scene.Effets.Add(this.Representation, Core.Visuel.PredefinedEffects.FadeOutTo0(this.Representation.Couleur.A, 0, tempsRequis));
        }


        private float signedAngle(ref Vector3 vecteur1, ref Vector3 vecteur2)
        {
            float perpDot = vecteur1.X * vecteur2.Y - vecteur1.Y * vecteur2.X;

            return (float)Math.Atan2(perpDot, Vector3.Dot(vecteur1, vecteur2));
        }
    }
}
