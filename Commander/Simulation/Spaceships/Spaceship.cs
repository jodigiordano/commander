namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Spaceship : IObjetPhysique
    {
        public static List<int> SafeBouncing = new List<int>() { -20, -18, -16, -14, -10, 10, 14, 16, 18, 20 };


        public float Speed                      { get; set; }
        public float Masse                      { get; set; }
        public Vector3 Direction                { get; set; }
        public float Rotation                   { get; set; }
        public float ResistanceRotation         { get; set; }
        public Shape Shape                      { get; set; }
        public Circle Circle                    { get; set; }
        public PhysicalRectangle Rectangle      { get; set; }
        public Line Line                       { get; set; }

        public IObjetPhysique StartingObject    { get; set; }
        public virtual bool TargetReached       { get; set; }

        private Vector3 targetPosition;
        public bool InCombat;
        public Vector3 TargetPosition           { get { return targetPosition; } set { targetPosition = value; InCombat = true; TargetReached = false; } }
        public float BulletHitPoints;

        protected Simulator Simulation;
        public Image Image;
        public float RotationMaximaleRad;

        protected List<Bullet> Bullets;
        public double ShootingFrequency;
        private double LastFireCounter;

        public string SfxOut                 { get; protected set; }
        public string SfxIn                     { get; protected set; }
        public virtual bool Active              { get; set; }
        public bool GoBackToStartingObject      { get; set; }
        public bool AutomaticMode;
        public Vector3 NextInput;

        protected Matrix RotationMatrix;

        private Particle TrailEffect;
        protected bool ShowTrail;

        private Vector3 position;
        public Vector3 LastPosition;


        public Spaceship(Simulator simulator)
        {
            Simulation = simulator;
            Image = new Image("Vaisseau", Position)
            {
                SizeX = 4
            };
            
            Position = Vector3.Zero;
            Speed = 4;
            Masse = 1;
            Direction = new Vector3(1, 0, 0);
            Rotation = 0;
            ResistanceRotation = 0;
            Shape = Shape.Circle;
            Circle = new Circle(Position, Image.TextureSize.X * Image.SizeX / 2);

            RotationMaximaleRad = 0.10f;
            ShootingFrequency = 300;
            LastFireCounter = 0;

            Bullets = new List<Bullet>();

            SfxOut = "";
            SfxIn = "";
            Active = true;
            GoBackToStartingObject = false;
            AutomaticMode = true;
            NextInput = Vector3.Zero;

            ShowTrail = false;
        }


        public virtual void Initialize()
        {
            if (StartingObject != null)
                Position = StartingObject.Position;

            TrailEffect = Simulation.Scene.Particles.Get(@"spaceshipTrail");
            TrailEffect.VisualPriority = Image.VisualPriority + 0.00001f;
        }



        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
            }
        }


        public Vector3 NinjaPosition
        {
            set
            {
                Position = value;
                LastPosition = value;
            }
        }


        public Vector3 DeltaPosition
        {
            get { return Position - LastPosition; }
        }


        public virtual void Update()
        {
            LastPosition = position;

            if (StartingObject != null && GoBackToStartingObject)
                TargetPosition = StartingObject.Position;

            this.Circle.Position = this.Position;

            LastFireCounter += Preferences.TargetElapsedTimeMs;
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

            Position += Direction * Speed;

            if ((TargetPosition - Position).LengthSquared() <= 600)
            {
                InCombat = false;
                TargetReached = true;
            }
        }


        public virtual List<Bullet> BulletsThisTick()
        {
            Bullets.Clear();


            if (LastFireCounter >= ShootingFrequency)
            {
                LastFireCounter = 0;

                Matrix matriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(Direction, matriceRotation);
                directionUnitairePerpendiculaire.Normalize();

                Vector3 translation = directionUnitairePerpendiculaire * Main.Random.Next(-17, 17);

                BasicBullet p = (BasicBullet) Simulation.BulletsFactory.Get(BulletType.Base);

                p.Position = Position + translation;
                p.Direction = Direction;
                p.AttackPoints = BulletHitPoints;
                p.VisualPriority = Image.VisualPriority + 0.001;
                p.Speed = 10;

                Bullets.Add(p);
            }

            if (Bullets.Count != 0)
                Audio.PlaySfx(@"sfxPowerUpResistanceTire" + Main.Random.Next(1, 4));

            return Bullets;
        }


        public virtual void Draw()
        {
            Image.Position = Position;
            Image.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);

            Simulation.Scene.Add(Image);

            if (ShowTrail)
            {
                Vector3 p = Image.Position;
                TrailEffect.Trigger(ref p);
            }
        }


        public virtual void DoHide()
        {
            float distance = (StartingObject.Position - Position).Length();

            double tempsRequis = (distance / Speed) * 16.33f;

            Simulation.Scene.VisualEffects.Add(Image, Core.Visual.VisualEffects.FadeOutTo0(Image.Color.A, 0, tempsRequis));
        }


        protected static float SignedAngle(ref Vector3 vecteur1, ref Vector3 vecteur2)
        {
            float perpDot = vecteur1.X * vecteur2.Y - vecteur1.Y * vecteur2.X;

            return (float)Math.Atan2(perpDot, Vector3.Dot(vecteur1, vecteur2));
        }
    }
}
