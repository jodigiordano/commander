﻿namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class PausePlayer
    {
        public Spaceship SpaceshipMove;

        // for keyboard
        public bool MovingLeft;
        public bool MovingRight;
        public bool MovingUp;
        public bool MovingDown;

        private Vector3 direction;
        private Simulator Simulator;

        private Vector3 position;
        public Circle Circle;


        public PausePlayer(Simulator simulator)
        {
            Simulator = simulator;

            SpaceshipMove = new Spaceship(simulator)
            {
                Speed = 4,
                VisualPriority = VisualPriorities.Default.PlayerPanelCursor
            };

            Circle = new Circle(Vector3.Zero, 8);
        }


        public void Initialize()
        {
            MovingLeft = MovingRight = MovingUp = MovingDown = false;
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = SpaceshipMove.Position = value;
                VerifyFrame();
                Circle.Position = position;
            }
        }


        public Vector3 NinjaPosition
        {
            set
            {
                SpaceshipMove.NinjaPosition = value;
                Position = SpaceshipMove.Position;
            }
        }


        public Vector3 DeltaPosition
        {
            get { return SpaceshipMove.DeltaPosition; }
        }


        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = SpaceshipMove.Direction = value;
            }
        }


        public Vector3 CurrentSpeed
        {
            get { return SpaceshipMove.SteeringBehavior.Momentum; }
        }


        public Vector3 NextInput
        {
            get { return SpaceshipMove.SteeringBehavior.NextMovement; }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            SpaceshipMove.SteeringBehavior.NextMovement = delta;
        }


        public void Rotate(ref Vector3 delta, float speed)
        {
            SpaceshipMove.SteeringBehavior.NextRotation = delta;
        }



        public void Update()
        {
            SpaceshipMove.Update();
            Position = SpaceshipMove.Position;
            Direction = SpaceshipMove.Direction;


            SpaceshipMove.SteeringBehavior.NextMovement = Vector3.Zero;
            SpaceshipMove.SteeringBehavior.NextRotation = Vector3.Zero;
        }


        private void VerifyFrame()
        {
            position = new Vector3
            (
                MathHelper.Clamp(Position.X, Simulator.Battlefield.Left + Circle.Radius, Simulator.Battlefield.Right - Circle.Radius),
                MathHelper.Clamp(Position.Y, Simulator.Battlefield.Top + Circle.Radius, Simulator.Battlefield.Bottom - Circle.Radius),
                0
            );
        }
    }
}