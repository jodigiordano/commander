namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Spaceship : ICollidable
    {
        public event NoneHandler Bounced;

        // Movement
        
        public Vector3 Position                 { get; set; }
        public float Speed                      { get { return SteeringBehavior.Speed; } set { SteeringBehavior.Speed = value; } }
        public Vector3 Direction                { get; set; }
        private Vector3 LastPosition;


        // Visual
        public Image Image;
        private Particle TrailEffect;
        protected bool ShowTrail;

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

        protected Simulator Simulator;

        public SpaceshipSteeringBehavior SteeringBehavior;
        public SpaceshipWeapon Weapon;


        public Spaceship(Simulator simulator)
        {
            Simulator = simulator;
            Image = new Image("Vaisseau", Position)
            {
                SizeX = 4
            };
            
            Position = Vector3.Zero;
            SteeringBehavior = new SpaceshipNoneBehavior(this);
            
            Direction = new Vector3(1, 0, 0);

            Shape = Shape.Circle;
            Circle = new Circle(Position, Image.TextureSize.X * Image.SizeX / 2);

            Weapon = new NoWeapon(Simulator, this);

            SfxOut = "";
            SfxIn = "";
            Active = true;

            ShowTrail = false;
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


        public virtual void Update()
        {
            LastPosition = Position;

            Circle.Position = Position;

            if (Weapon != null)
                Weapon.Update();

            SteeringBehavior.Update();
        }


        public double VisualPriority
        {
            set { Image.VisualPriority = value; }
        }


        public List<Bullet> Fire()
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


        public void NotifyBounced()
        {
            if (Bounced != null)
                Bounced();
        }


        //useless
        public float Rotation { get; set; }
    }
}
