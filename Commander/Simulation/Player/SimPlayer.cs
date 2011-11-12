namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation.Player;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class SimPlayer
    {
        public Commander.Player InnerPlayer;
        public VisualPlayer VisualPlayer;

        // Panel mode
        private Vector3 PositionBackup;
        private Vector3 DirectionBackup;
        private SimPlayerSelection SelectionBackup;

        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public Dictionary<TurretChoice, bool> AvailableTurretOptions;

        public SimPlayerSelection ActualSelection;
        public SimPlayerSelection LastSelection;
        
        public PowerUpType PowerUpInUse;
        public Spaceship SpaceshipMove;
        public SimPlayerDirectionHandler BouncedHandler;
        public SimPlayerHandler RotatedHandler;

        // for keyboard/mouse
        public bool MovingLeft;
        public bool MovingRight;
        public bool MovingUp;
        public bool MovingDown;
        public Vector3 LastMouseDirection;

        private Vector3 direction;
        private Simulator Simulator;

        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private SelectedPowerUpController SelectedPowerUpController;

        private bool firing;
        private Metronome VibrationMetronome;


        public SimPlayer(Simulator simulator, Commander.Player player)
        {
            Simulator = simulator;
            InnerPlayer = player;

            SpaceshipMove = new Spaceship(Simulator)
            {
                Speed = 4,
                VisualPriority = VisualPriorities.Default.VisualPlayer,
                ShowShield = true,
                ShieldImageName = "SpaceshipHitMask",
                ShieldColor = InnerPlayer.Color,
                ShieldAlpha = 255,
                ShieldDistance = 32,
                ShieldSize = 4
            };
            SpaceshipMove.Weapon = new BasicBulletWeapon(Simulator, SpaceshipMove, 100, 1);
            SpaceshipMove.SizeX = 4;

            SpaceshipMove.Bounced += new DirectionHandler(DoBounced);
            SpaceshipMove.Rotated += new NoneHandler(DoRotated);

            if (InnerPlayer.InputType == Core.Input.InputType.MouseAndKeyboard)
                SpaceshipMove.SteeringBehavior = new SpaceshipMouseMBehavior(SpaceshipMove);
            else if (InnerPlayer.InputType == Core.Input.InputType.KeyboardOnly)
                SpaceshipMove.SteeringBehavior = new SpaceshipKeyboardMBehavior(SpaceshipMove);
            else if (InnerPlayer.InputType == Core.Input.InputType.Gamepad)
                SpaceshipMove.SteeringBehavior = new SpaceshipGamePadMBehavior(SpaceshipMove);

            ActualSelection = new SimPlayerSelection();
            LastSelection = new SimPlayerSelection();
            SelectionBackup = new SimPlayerSelection();
            PowerUpInUse = PowerUpType.None;
            MovingLeft = MovingRight = MovingUp = MovingDown = false;
            LastMouseDirection = Vector3.Zero;
            Firing = false;
            PowerUpInUse = PowerUpType.None;
            VibrationMetronome = new Metronome(Preferences.TargetElapsedTimeMs, 100);

            VisualPlayer = new VisualPlayer(Simulator, this);

            AvailableTurretOptions = new Dictionary<TurretChoice, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretChoice.Sell, false);
            AvailableTurretOptions.Add(TurretChoice.Update, false);
        }


        public void Initialize()
        {
            InitializePlayer();
            InitializeVisualPlayer();
        }


        private void InitializePlayer()
        {
            SelectedCelestialBodyController = new SelectedCelestialBodyController(Simulator, this);
            SelectedPowerUpController = new SelectedPowerUpController(Simulator.Data.Level.AvailablePowerUps, Circle);

            VisualPlayer.AvailableTurrets = AvailableTurrets;
        }


        private void InitializeVisualPlayer()
        {
            VisualPlayer.Initialize();

            VisualPlayer.CurrentVisual.Position = Position;

            VisualPlayer.CurrentVisual.TeleportIn();
            VisualPlayer.CurrentVisual.FadeIn();
        }


        public void DoDisconnect()
        {
            VisualPlayer.CurrentVisual.TeleportOut();
        }


        public void Draw()
        {
            VisualPlayer.Draw();
        }


        public Vector3 Position
        {
            get { return InnerPlayer.Position; }
            set
            {
                InnerPlayer.Position = SpaceshipMove.Position = value;
                InnerPlayer.Position = Simulator.Data.Battlefield.Clamp(InnerPlayer.Position, InnerPlayer.Circle.Radius);
                VisualPlayer.CurrentVisual.Position = InnerPlayer.Position;
                VisualPlayer.Crosshair.Position = InnerPlayer.Position;
            }
        }


        public void SwitchToPanelMode()
        {
            SelectionBackup.Sync(ActualSelection);
            PositionBackup = Position;
            DirectionBackup = Direction;
            VisualPlayer.SwitchToPanelVisual();
        }


        public void SwitchToNormalMode()
        {
            NinjaPosition = PositionBackup;
            Direction = DirectionBackup;
            ActualSelection.Sync(SelectionBackup);
            VisualPlayer.SwitchToVisual();
        }


        public Circle Circle
        {
            get { return InnerPlayer.Circle; }
            set { InnerPlayer.Circle = value; }
        }


        public Color Color
        {
            get { return InnerPlayer.Color; }
            set { InnerPlayer.Color = value; }
        }


        public string ImageName
        {
            get { return InnerPlayer.ImageName; }
            set { InnerPlayer.ImageName = value; }
        }



        public Vector3 NinjaPosition
        {
            set
            {
                SpaceshipMove.NinjaPosition = value;
                Position = SpaceshipMove.Position;
            }
        }


        public bool Firing
        {
            get { return firing; }

            set
            {
                if (!firing && value && (InnerPlayer.InputType == Core.Input.InputType.MouseAndKeyboard || InnerPlayer.InputType == Core.Input.InputType.KeyboardOnly))
                {
                    LastMouseDirection = SpaceshipMove.Direction;
                }

                firing = value;
            }
        }


        public float BulletAttackPoints
        {
            set { SpaceshipMove.Weapon.AttackPoints = value; }
        }


        public Vector3 DeltaPosition
        {
            get { return SpaceshipMove.DeltaPosition; }
        }


        public Vector3 Direction
        {
            get { return direction; }
            set { direction = SpaceshipMove.Direction = VisualPlayer.CurrentVisual.Direction = value; }
        }


        public Vector3 CurrentSpeed
        {
            get { return SpaceshipMove.SteeringBehavior.Momentum; }
        }


        public float MaximumSpeed
        {
            get { return SpaceshipMove.SteeringBehavior.MaximumMomentum; }
        }


        public bool MovementInputThisTick
        {
            get { return SpaceshipMove.SteeringBehavior.ManualMovementInputThisTick; }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            SpaceshipMove.SteeringBehavior.NextMovement = delta;
        }


        public void Rotate(ref Vector3 delta, float speed)
        {
            SpaceshipMove.SteeringBehavior.NextRotation = delta;
        }


        public void NextContextualMenuSelection()
        {
            var openedMenu = VisualPlayer.GetOpenedMenu();

            if (openedMenu != null)
                openedMenu.NextChoice();
        }


        public void PreviousContextualMenuSelection()
        {
            var openedMenu = VisualPlayer.GetOpenedMenu();

            if (openedMenu != null)
                openedMenu.PreviousChoice();
        }


        public void Update()
        {
            // More friction on a celestial body and a turret
            if (SpaceshipMove.SteeringBehavior.NextMovement == Vector3.Zero)
            {
                if (ActualSelection.TurretToPlace == null && (ActualSelection.CelestialBody != null || ActualSelection.Turret != null))
                    SpaceshipMove.SteeringBehavior.Friction = 0.1f;
            }


            SpaceshipMove.Update();
            Position = SpaceshipMove.Position;
            Direction = SpaceshipMove.Direction;

            Position += SelectedCelestialBodyController.DoGlueMode();

            // Manage turret to place (to always have a valid position)
            if (ActualSelection.TurretToPlace != null)
            {
                Turret turretToPlace = ActualSelection.TurretToPlace;
                CelestialBody celestialBody = ActualSelection.TurretToPlace.CelestialBody;
                turretToPlace.Position = Position;

                if (celestialBody.TurretsController.OuterTurretZone.Outside(Position))
                    Position = celestialBody.TurretsController.OuterTurretZone.NearestPointToCircumference(Position);

                turretToPlace.CanPlace = celestialBody.TurretsController.InnerTurretZone.Outside(turretToPlace.Position);
                turretToPlace.CanPlace = turretToPlace.CanPlace && turretToPlace.BuyPrice <= Simulator.Data.Level.CommonStash.Cash; //multiplayer failsafe.

                if (turretToPlace.CanPlace)
                    foreach (var turret in celestialBody.TurretsController.Turrets)
                    {
                        turretToPlace.CanPlace =
                            !turret.Visible ||
                            !Physics.CircleCicleCollision(turret.Circle, turretToPlace.Circle);

                        if (!turretToPlace.CanPlace)
                            break;
                    }
            }

            UpdateSelection();

            if (Firing)
            {
                VibrationMetronome.Update();
            
                if (VibrationMetronome.CyclesCountThisTick != 0)
                    Core.Input.Inputs.VibrateControllerHighFrequency(InnerPlayer, 80, 0.4f);

                LastMouseDirection = SpaceshipMove.Direction;
            }

            VisualPlayer.Update();
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyController.DoCelestialBodyDestroyed();
            ActualSelection.TurretToBuy = TurretType.None;
        }


        private void UpdateSelection()
        {
            if (Simulator.State == GameState.Lost || Simulator.State == GameState.Won)
            {
                ActualSelection.Initialize();
                return;
            }

            LastSelection.Sync(ActualSelection);

            SelectedCelestialBodyController.Update();
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;
            ActualSelection.Turret = SelectedCelestialBodyController.Turret;
            CheckAvailableTurretOptions();

            ActualSelection.OpenedMenu = VisualPlayer.GetOpenedMenu();

            if (ActualSelection.OpenedMenu != null && ActualSelection.OpenedMenuChanged)
                ActualSelection.OpenedMenu.OnOpen();

            if (ActualSelection.OpenedMenu != null)
                ActualSelection.OpenedMenu.UpdateSelection();
        }


        private void DoBounced(Direction d)
        {
            if (BouncedHandler != null)
                BouncedHandler(this, d);
        }


        private void DoRotated()
        {
            if (RotatedHandler != null)
                RotatedHandler(this);
        }


        private void CheckAvailableTurretOptions()
        {
            if (ActualSelection.Turret == null)
                return;

            AvailableTurretOptions[TurretChoice.Sell] =
                ActualSelection.Turret.CanSell &&
                !ActualSelection.Turret.Disabled;

            AvailableTurretOptions[TurretChoice.Update] =
                ActualSelection.Turret.CanUpdate &&
                ActualSelection.Turret.UpgradePrice <= Simulator.Data.Level.CommonStash.Cash;
        }
    }
}