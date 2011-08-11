﻿namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Spaceship : ICollidable
    {
        public event NoneHandler Bounced;

        private static List<int> SafeBouncing = new List<int>() { -20, -18, -16, -14, -10, 10, 14, 16, 18, 20 };

        // Movement
        public float MaxRotationRad;
        private Matrix RotationMatrix;
        public Vector3 NextMovement;
        public Vector3 NextRotation;
        public Vector3 Position                 { get; set; }
        public float Speed                      { get; set; }
        public Vector3 Direction                { get; set; }
        public float Rotation                   { get; set; }
        private Vector3 Acceleration;
        public Vector3 LastPosition;
        private Vector3 Bouncing;
        public float Friction;

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

        protected Simulator Simulator;

        // Automatic mode
        public bool ApplyAutomaticBehavior;
        public SpaceshipBehavior AutomaticBehavior;

        // Weapon
        public SpaceshipWeapon Weapon;

        public virtual bool Active              { get; set; }

        public IPhysical StartingObject;


        public Spaceship(Simulator simulator)
        {
            Simulator = simulator;
            Image = new Image("Vaisseau", Position)
            {
                SizeX = 4
            };
            
            Position = Vector3.Zero;
            Speed = 4;
            Direction = new Vector3(1, 0, 0);
            Rotation = 0;
            Shape = Shape.Circle;
            Circle = new Circle(Position, Image.TextureSize.X * Image.SizeX / 2);

            MaxRotationRad = 0.10f;
            Weapon = new NoWeapon(Simulator, this);

            SfxOut = "";
            SfxIn = "";
            Active = true;
            ApplyAutomaticBehavior = true;
            NextMovement = Vector3.Zero;
            NextRotation = Vector3.Zero;


            Acceleration = Vector3.Zero;

            ShowTrail = false;
        }


        public virtual void Initialize()
        {
            TrailEffect = Simulator.Scene.Particles.Get(@"spaceshipTrail");
            TrailEffect.VisualPriority = Image.VisualPriority + 0.00001f;
        }



        public Vector3 CurrentSpeed
        {
            get { return Speed * Acceleration; }
        }


        public void Stop()
        {
            Bouncing = Vector3.Zero;
            Acceleration = Vector3.Zero;
            NextMovement = Vector3.Zero;
            NextRotation = Vector3.Zero;
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

            if (ApplyAutomaticBehavior)
                AutomaticBehavior.Update();
            else
                DoManualMode(ref NextMovement, ref NextRotation);

            ApplyBouncing();
            ApplyFriction();
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


        private void DoManualMode(ref Vector3 movement, ref Vector3 rotation)
        {
            movement /= 5;

            movement.X = MathHelper.Clamp(movement.X, -1, 1);
            movement.Y = MathHelper.Clamp(movement.Y, -1, 1);

            if (rotation != Vector3.Zero)
                ApplyRotation(ref rotation);
            else
                ApplyRotation(ref movement);

            ApplyAcceleration(ref movement);

            Position += Speed * Acceleration;
        }


        private void ApplyRotation(ref Vector3 movement)
        {
            // Movement direction
            Vector3 currentDirection = movement;

            // Current direction
            Vector3 direction = Direction;

            // Delta, Converted to angle
            float angle = Core.Physics.Utilities.SignedAngle(ref currentDirection, ref direction);

            // Clamp to maximum allowed by the ship
            float rotation = MathHelper.Clamp(MaxRotationRad, 0, Math.Abs(angle));

            if (angle > 0)
                rotation = -rotation;

            // Apply
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref direction, ref RotationMatrix, out direction);

            if (direction != Vector3.Zero)
                direction.Normalize();

            Direction = direction;
        }


        private void ApplyFriction()
        {
            if (Friction > 0)
                Friction = Math.Max(0, Friction - 0.01f); //lower == less on planets
        }


        private void ApplyBouncing()
        {
            bool bouncing = false;

            if (Position.X > 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Acceleration.X) * Speed * 2f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                bouncing = true;
            }

            if (Position.X < -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Acceleration.X) * Speed * 2f;
                Bouncing.Y = Bouncing.Y + Acceleration.Y * Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                bouncing = true;
            }

            if (Position.Y > 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 2f;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Acceleration.Y) * Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                bouncing = true;
            }

            if (Position.Y < -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Acceleration.X * Speed * 2f;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Acceleration.Y) * Speed * 2f;

                Acceleration.X = 0;
                Acceleration.Y = 0;

                bouncing = true;
            }

            Position += Bouncing;

            if (Bouncing.X > 0)
                Bouncing.X = Math.Max(0, Bouncing.X - 0.8f);
            else if (Bouncing.X < 0)
                Bouncing.X = Math.Min(0, Bouncing.X + 0.8f);

            if (Bouncing.Y > 0)
                Bouncing.Y = Math.Max(0, Bouncing.Y - 0.8f);
            else if (Bouncing.Y < 0)
                Bouncing.Y = Math.Min(0, Bouncing.Y + 0.8f);

            if (bouncing)
                NotifyBounced();
        }


        private void ApplyAcceleration(ref Vector3 input)
        {
            Acceleration += input / 10f;

            Acceleration.X = MathHelper.Clamp(Acceleration.X, -2, 2);
            Acceleration.Y = MathHelper.Clamp(Acceleration.Y, -2, 2);

            if (Acceleration.X > 0)
                Acceleration.X = Math.Max(0, Acceleration.X - (0.03f + Friction)); //lower literal == less in general
            else if (Acceleration.X < 0)
                Acceleration.X = Math.Min(0, Acceleration.X + (0.03f + Friction));

            if (Acceleration.Y > 0)
                Acceleration.Y = Math.Max(0, Acceleration.Y - (0.03f + Friction));
            else if (Acceleration.Y < 0)
                Acceleration.Y = Math.Min(0, Acceleration.Y + (0.03f + Friction));
        }


        public void ApplySafeBouncing()
        {
            Bouncing = new Vector3(Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], Spaceship.SafeBouncing[Main.Random.Next(0, Spaceship.SafeBouncing.Count)], 0);
        }


        private void NotifyBounced()
        {
            if (Bounced != null)
                Bounced();
        }
    }
}
