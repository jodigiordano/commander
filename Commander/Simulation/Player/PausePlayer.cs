namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class PausePlayer
    {
        public SpaceshipSpaceship SpaceshipMove;
        public OptionsPanel OptionsPanel;
        public PausePanel PausePanel;

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

            SpaceshipMove = new SpaceshipSpaceship(simulator)
            {
                ApplyAutomaticBehavior = false,
                Speed = 4,
                ShootingFrequency = 100,
                BulletHitPoints = 1,
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
            get { return SpaceshipMove.CurrentSpeed; }
        }


        public Vector3 NextInput
        {
            get { return SpaceshipMove.NextMovement; }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            SpaceshipMove.NextMovement = delta;
        }


        public void Rotate(ref Vector3 delta, float speed)
        {
            SpaceshipMove.NextRotation = delta;
        }



        public void Update()
        {
            // More friction on a celestial body and a turret
            if (SpaceshipMove.NextMovement == Vector3.Zero)
            {
                if (OptionsPanel.Visible && OptionsPanel.DoHover(Circle) && OptionsPanel.LastHoverWidget.Sticky)
                    SpaceshipMove.Friction = 0.1f;

                if (PausePanel.Visible && PausePanel.DoHover(Circle) && PausePanel.LastHoverWidget.Sticky)
                    SpaceshipMove.Friction = 0.1f;
            }


            SpaceshipMove.Update();
            Position = SpaceshipMove.Position;
            Direction = SpaceshipMove.Direction;


            SpaceshipMove.NextMovement = Vector3.Zero;
            SpaceshipMove.NextRotation = Vector3.Zero;
        }


        private void VerifyFrame()
        {
            position = new Vector3
            (
                MathHelper.Clamp(Position.X, -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius, 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius),
                MathHelper.Clamp(Position.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius, 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius),
                0
            );
        }
    }
}