namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    class Spaceship : IObjetPhysique
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

        public IObjetPhysique StartingObject;
        public virtual bool TargetReached       { get; set; }

        private Vector3 targetPosition;
        public bool InCombat;
        public Vector3 TargetPosition           { get { return targetPosition; } set { targetPosition = value; InCombat = true; TargetReached = false; } }
        public float BulletHitPoints;

        protected Simulation Simulation;
        public Image Image;
        public float RotationMaximaleRad;

        private List<Projectile> Bullets;
        public double ShootingFrequency;
        private double LastFireCounter;

        public string SfxGoHome                 { get; protected set; }
        public string SfxIn                     { get; protected set; }
        public virtual bool Active              { get; set; }
        public bool GoBackToStartingObject      { get; set; }
        public bool AutomaticMode;
        public Vector3 NextInput;

        protected Matrix RotationMatrix;


        public Spaceship(Simulation simulation)
        {
            Simulation = simulation;
            Image = new Image("Vaisseau", Position)
            {
                SizeX = 4
            };
            
            Position = Vector3.Zero;
            Vitesse = 4;
            Masse = 1;
            Direction = new Vector3(1, 0, 0);
            Rotation = 0;
            ResistanceRotation = 0;
            Forme = Forme.Cercle;
            Cercle = new Cercle(Position, Image.TextureSize.X * Image.SizeX / 2);

            RotationMaximaleRad = 0.10f;
            ShootingFrequency = 300;
            LastFireCounter = 0;

            Bullets = new List<Projectile>();

            SfxGoHome = "";
            SfxIn = "";
            Active = true;
            GoBackToStartingObject = false;
            AutomaticMode = true;
            NextInput = Vector3.Zero;
        }


        public virtual void Update()
        {
            this.Cercle.Position = this.Position;

            LastFireCounter += 16.66f;
        }


        public virtual void DoAutomaticMode()
        {
            // Trouver la direction visée
            Vector3 directionVisee = TargetPosition - Position;
            Vector3 direction = Direction;

            // Trouver l'angle d'alignement
            float angle = SignedAngle(ref directionVisee, ref direction);

            // Trouver la rotation nécessaire pour s'enligner
            float rotation = MathHelper.Clamp(RotationMaximaleRad, 0, Math.Abs(angle));

            if (angle > 0)
                rotation = -rotation;

            // Appliquer la rotation
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref direction, ref RotationMatrix, out direction);

            if (direction != Vector3.Zero)
                direction.Normalize();

            Direction = direction;

            Position += Direction * Vitesse;

            if ((TargetPosition - Position).LengthSquared() <= 400)
            {
                InCombat = false;
                TargetReached = true;
            }
        }


        public virtual List<Projectile> BulletsThisTick()
        {
            Bullets.Clear();


            if (LastFireCounter >= ShootingFrequency)
            {
                LastFireCounter = 0;

                Matrix matriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(Direction, matriceRotation);
                directionUnitairePerpendiculaire.Normalize();

                Vector3 translation = directionUnitairePerpendiculaire * Main.Random.Next(-17, 17);

                ProjectileBase p = Projectile.PoolProjectilesBase.recuperer();
                p.Scene = Simulation.Scene;
                p.Position = Position + translation;
                p.Direction = Direction;
                p.PointsAttaque = BulletHitPoints;
                p.PrioriteAffichage = Image.VisualPriority + 0.001f;
                p.Initialize();

                Simulation.Scene.Particules.retourner(p.RepresentationDeplacement);
                p.RepresentationDeplacement = null;

                Bullets.Add(p);
            }

            if (Bullets.Count != 0)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceTire" + Main.Random.Next(1, 4));

            return Bullets;
        }


        public virtual void Draw()
        {
            Image.Position = Position;
            Image.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);

            Simulation.Scene.ajouterScenable(Image);
        }


        public virtual void DoHide()
        {
            float distance = (StartingObject.Position - Position).Length();

            double tempsRequis = (distance / Vitesse) * 16.33f;

            Simulation.Scene.Effets.Add(Image, Core.Visuel.PredefinedEffects.FadeOutTo0(Image.Color.A, 0, tempsRequis));
        }


        protected static float SignedAngle(ref Vector3 vecteur1, ref Vector3 vecteur2)
        {
            float perpDot = vecteur1.X * vecteur2.Y - vecteur1.Y * vecteur2.X;

            return (float)Math.Atan2(perpDot, Vector3.Dot(vecteur1, vecteur2));
        }
    }
}
