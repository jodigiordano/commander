namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Spaceship : IDestroyable
    {
        public event DirectionHandler Bounced;
        public event NoneHandler Rotated;

        private static int NextId = 0;
        
        public int Id                           { get; set; }

        // Movement
        public virtual Vector3 Position         { get; set; }
        public float Speed                      { get { return SteeringBehavior.Speed; } set { SteeringBehavior.Speed = value; } }
        public Vector3 Direction                { get; set; }
        private Vector3 LastPosition;

        // Visual
        public Image Image;
        private Particle TrailEffect;
        protected bool ShowTrail;
        public string ShieldImageName;
        public Color ShieldColor;
        public byte ShieldAlpha;
        public float ShieldDistance;
        public float ShieldSize;
        public bool ShowShield;
        public Circle ShieldCircle              { get; set; }

        // Collision
        public Shape Shape                      { get; set; }
        public Circle Circle                    { get; set; }
        public PhysicalRectangle Rectangle      { get; set; }
        public Line Line                        { get; set; }

        // Power-Up
        public string SfxOut                    { get; protected set; }
        public string SfxIn                     { get; protected set; }
        public int BuyPrice;
        public IPhysical StartingObject;
        public virtual bool Active { get; set; }

        public bool Alive { get; set; }

        public Simulator Simulator;

        public SpaceshipSteeringBehavior SteeringBehavior;
        public SpaceshipWeapon Weapon;


        public Spaceship(Simulator simulator) : this(simulator, "Vaisseau") {}


        public Spaceship(Simulator simulator, string imageName)
        {
            Simulator = simulator;
            Image = new Image(imageName, Position);
            Position = Vector3.Zero;
            SteeringBehavior = new SpaceshipNoneBehavior(this);
            
            Direction = new Vector3(1, 0, 0);

            Shape = Shape.Circle;
            Circle = new Circle();
            ShieldCircle = new Circle();

            SizeX = 4;

            Weapon = new NoWeapon(Simulator, this);

            SfxOut = "";
            SfxIn = "";
            Active = true;

            ShowTrail = false;
            ShowShield = false;

            Alive = true;
            Id = NextId++;
        }


        public virtual void Initialize()
        {
            TrailEffect = Simulator.Scene.Particles.Get(@"spaceshipTrail");
            TrailEffect.VisualPriority = Image.VisualPriority + 0.00001f;
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


        public virtual float SizeX
        {
            get { return Image.SizeX; }
            set
            {
                Image.SizeX = value;
                Circle.Radius = Image.AbsoluteSize.X / 2;
                ShieldCircle.Radius = Circle.Radius + 10;
            }
        }


        public virtual BlendType Blend
        {
            set { Image.Blend = value; }
        }


        public virtual void Update()
        {
            LastPosition = Position;

            Circle.Position = Position;
            ShieldCircle.Position = Position;

            if (Weapon != null)
                Weapon.Update();

            SteeringBehavior.Update();
        }


        public double VisualPriority
        {
            get { return Image.VisualPriority;  }
            set { Image.VisualPriority = value; }
        }


        public virtual List<Bullet> Fire()
        {
            return Weapon.Fire();
        }


        public virtual void Draw()
        {
            Image.Position = Position;

            Image.Rotation = (MathHelper.PiOver2) + (float) Math.Atan2(Direction.Y, Direction.X);

            Simulator.Scene.Add(Image);

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

            Simulator.Scene.VisualEffects.Add(Image, Core.Visual.VisualEffects.FadeOutTo0(Image.Color.A, 0, tempsRequis));
        }


        public void NotifyBounced(Direction d)
        {
            if (Bounced != null)
                Bounced(d);
        }


        public void NotifyRotated()
        {
            if (Rotated != null)
                Rotated();
        }


        public void DoShieldHit(Vector3 hitPosition)
        {
            if (!ShowShield)
                return;

            Vector3 direction = hitPosition - Position;
            float lengthSquared = direction.LengthSquared();
            float cbLengthSquared = ShieldCircle.Radius * ShieldCircle.Radius;

            if (lengthSquared >= cbLengthSquared)
                Simulator.Scene.Add(new ShieldHitAnimation(ShieldImageName, this, hitPosition, ShieldColor, ShieldSize, VisualPriority, ShieldDistance, ShieldAlpha));
        }


        public float Rotation
        {
            get { return Core.Physics.Utilities.VectorToAngle(Direction); }
            set { Direction = Core.Physics.Utilities.AngleToVector(value); }
        }
    }
}
