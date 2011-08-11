namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SimPlayer
    {
        public List<CelestialBody> CelestialBodies;
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public SimPlayerSelection ActualSelection;
        public CommonStash CommonStash;
        public bool UpdateSelectionz;
        public string ImageName;
        public PowerUpType PowerUpInUse;
        public Player BasePlayer;
        public PausePlayer PausePlayer;
        public SpaceshipSpaceship SpaceshipMove;
        public bool TurretToPlaceChanged;
        public bool GameOver;

        // for keyboard/mouse
        public bool MovingLeft;
        public bool MovingRight;
        public bool MovingUp;
        public bool MovingDown;
        public Vector3 LastMouseDirection;

        private Vector3 direction;
        private Simulator Simulator;

        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        private bool firing;


        public SimPlayer(Simulator simulator, Player player)
        {
            Simulator = simulator;
            BasePlayer = player;
            PowerUpInUse = PowerUpType.None;

            SpaceshipMove = new SpaceshipSpaceship(simulator)
            {
                Speed = 4,
                VisualPriority = VisualPriorities.Default.PlayerCursor
            };

            SpaceshipMove.Bounced += new NoneHandler(DoBouncing);

            TurretToPlaceChanged = false;
            GameOver = false;
        }


        public void Initialize()
        {
            ActualSelection = new SimPlayerSelection();
            TurretToBuyController = new SelectedTurretToBuyAllController(AvailableTurrets);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(Simulator, this, CelestialBodies);
            SelectedPowerUpController = new SelectedPowerUpController(Simulator.PowerUpsFactory.Availables, Circle);
            PowerUpInUse = PowerUpType.None;

            MovingLeft = MovingRight = MovingUp = MovingDown = false;
            LastMouseDirection = Vector3.Zero;

            GameOver = false;
            Firing = false;

            if (BasePlayer.InputType == Core.Input.InputType.Mouse)
            {
                SpaceshipMove.SteeringBehavior = new SpaceshipMouseMBehavior(SpaceshipMove);
                PausePlayer.SpaceshipMove.SteeringBehavior = new SpaceshipMouseMBehavior(PausePlayer.SpaceshipMove);
            }
            else if (BasePlayer.InputType == Core.Input.InputType.Gamepad)
            {
                SpaceshipMove.SteeringBehavior = new SpaceshipGamePadMBehavior(SpaceshipMove);
                PausePlayer.SpaceshipMove.SteeringBehavior = new SpaceshipGamePadMBehavior(PausePlayer.SpaceshipMove);
            }
        }


        public Vector3 Position
        {
            get { return BasePlayer.Position; }
            set
            {
                BasePlayer.Position = SpaceshipMove.Position = value;
            }
        }


        public Circle Circle
        {
            get { return BasePlayer.Circle; }
            set
            {
                BasePlayer.Circle = value;
            }
        }


        public Color Color
        {
            get { return BasePlayer.Color; }
            set { BasePlayer.Color = value; }
        }


        public Vector3 NinjaPosition
        {
            set
            {
                SpaceshipMove.NinjaPosition = value;
                Position = SpaceshipMove.Position;
                VerifyFrame();
            }
        }


        public bool Firing
        {
            get { return firing; }

            set
            {
                if (!firing && value && BasePlayer.InputType == Core.Input.InputType.Mouse)
                {
                    LastMouseDirection = SpaceshipMove.Direction;
                }

                firing = value;
            }
        }


        public float BulletAttackPoints
        {
            set
            {
                SpaceshipMove.BulletAttackPoints = value;
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
            get { return SpaceshipMove.SteeringBehavior.CurrentSpeed; }
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


        public void UpdateWorldSelection()
        {
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (ActualSelection.CelestialBody != null && Main.GameInProgress != null &&
                !Main.GameInProgress.IsFinished &&
                Main.GameInProgress.Simulator.LevelDescriptor.Infos.Mission == ActualSelection.CelestialBody.Name &&
                Simulator.Scene.EnableInputs &&
                ActualSelection.PausedGameChoice == PausedGameChoice.None) //todo: take it from WorldMenu
            {
                NextPausedGameChoice();
            }

            else if (Main.GameInProgress == null || ActualSelection.CelestialBody == null)
            {
                ActualSelection.PausedGameChoice = PausedGameChoice.None;
            }
        }


        public void UpdateMainMenuSelection()
        {
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (ActualSelection.CelestialBody != null &&
                ActualSelection.CelestialBody.Name == @"save the world" &&
                ActualSelection.NewGameChoice == NewGameChoice.None)
            {
                NextNewGameChoice();
            }

            else if (ActualSelection.CelestialBody == null || ActualSelection.CelestialBody.Name != @"save the world")
                ActualSelection.NewGameChoice = NewGameChoice.None;
        }


        public void UpdateSelection()
        {
            if (GameOver)
            {
                ActualSelection.Initialize();

                return;
            }

            // In a power-up
            if (PowerUpInUse != PowerUpType.None)
            {
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretChoice = TurretChoice.None;
                ActualSelection.CelestialBody = null;

                if (PowerUpInUse == PowerUpType.FinalSolution)
                {
                    SelectedCelestialBodyController.Update();
                    ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;
                }

                return;
            }


            // Placing a turret
            if (ActualSelection.TurretToPlace != null)
            {
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretChoice = TurretChoice.None;

                return;
            }

            SelectedCelestialBodyController.Update();
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;
            ActualSelection.Turret = SelectedCelestialBodyController.Turret;

            SelectedPowerUpController.Update();
            ActualSelection.PowerUpToBuy = SelectedPowerUpController.PowerUpToBuy;

            TurretToBuyController.Update(ActualSelection.CelestialBody);
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;


            if (ActualSelection.PowerUpToBuy != PowerUpType.None)
            {
                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.TurretChoice = TurretChoice.None;

                return;
            }


            if (SelectedCelestialBodyController.CelestialBody != null &&
                SelectedCelestialBodyController.Turret == null &&
                SelectedCelestialBodyController.SelectedCelestialBodyChanged)
            {
                ActualSelection.TurretChoice = TurretChoice.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                return;
            }


            if (SelectedCelestialBodyController.Turret != null)
            {
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                if (ActualSelection.TurretChoice == TurretChoice.None)
                    ActualSelection.TurretChoice = TurretChoice.Update;

                return;
            }


            if (SelectedCelestialBodyController.CelestialBody == null &&
                SelectedCelestialBodyController.Turret == null)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretChoice = TurretChoice.None;

                return;
            }
        }


        public void NextTurretToBuy()
        {
            TurretToBuyController.Next();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
        }


        public void PreviousTurretToBuy()
        {
            TurretToBuyController.Previous();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
        }


        public void NextPausedGameChoice()
        {
            int actual = (int) ActualSelection.PausedGameChoice;
            int nbChoices = 2;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            ActualSelection.PausedGameChoice = (PausedGameChoice) actual;
        }


        public void PreviousPausedGameChoice()
        {
            int actual = (int) ActualSelection.PausedGameChoice;
            int nbChoices = 2;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            ActualSelection.PausedGameChoice = (PausedGameChoice) actual;
        }


        public void NextNewGameChoice()
        {
            int actual = (int) ActualSelection.NewGameChoice;
            int nbChoices = ActualSelection.AvailableNewGameChoices.Count;
            int next = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual += 1;

                if (actual >= nbChoices)
                    actual = 0;

                if (ActualSelection.AvailableNewGameChoices[(NewGameChoice) actual])
                {
                    next = actual;
                    break;
                }
            }

            ActualSelection.NewGameChoice = (NewGameChoice) next;
        }


        public void PreviousNewGameChoice()
        {
            int actual = (int) ActualSelection.NewGameChoice;
            int nbChoices = ActualSelection.AvailableNewGameChoices.Count;
            int previous = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual -= 1;

                if (actual < 0)
                    actual = nbChoices - 1;

                if (ActualSelection.AvailableNewGameChoices[(NewGameChoice) actual])
                {
                    previous = actual;
                    break;
                }
            }

            ActualSelection.NewGameChoice = (NewGameChoice) previous;
        }


        public void NextTurretOption()
        {
            int actual = (int) ActualSelection.TurretChoice;
            int nbChoices = ActualSelection.AvailableTurretOptions.Count;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            ActualSelection.TurretChoice = (TurretChoice) actual;
        }


        public void PreviousTurretOption()
        {
            int actual = (int) ActualSelection.TurretChoice;
            int nbChoices = ActualSelection.AvailableTurretOptions.Count;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            ActualSelection.TurretChoice = (TurretChoice) actual;
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

            CheckAvailableTurretOptions();

            if (Simulator.DemoMode)
            {
                CheckAvailableNewGameChoices();
            }

            Position += SelectedCelestialBodyController.DoGlueMode();

            // Manage turret to place (to always has a valid position)
            if (ActualSelection.TurretToPlace != null)
            {
                Turret turretToPlace = ActualSelection.TurretToPlace;
                CelestialBody celestialBody = ActualSelection.TurretToPlace.CelestialBody;
                turretToPlace.Position = Position;

                if (celestialBody.OuterTurretZone.Outside(Position))
                    Position = celestialBody.OuterTurretZone.NearestPointToCircumference(Position);

                turretToPlace.CanPlace = celestialBody.InnerTurretZone.Outside(turretToPlace.Position);

                if (turretToPlace.CanPlace)
                    foreach (var turret in celestialBody.Turrets)
                    {
                        turretToPlace.CanPlace = !turret.Visible ||
                            !Physics.CircleCicleCollision(turret.Circle, turretToPlace.Circle);

                        if (!turretToPlace.CanPlace)
                            break;
                    }
            }

            TurretToPlaceChanged = false;

            if (UpdateSelectionz)
                UpdateSelection();

            if (Simulator.WorldMode)
                UpdateWorldSelection();

            if (Simulator.DemoMode)
                UpdateMainMenuSelection();
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyController.DoCelestialBodyDestroyed();
            ActualSelection.TurretToBuy = TurretType.None;
        }


        private void VerifyFrame()
        {
            Position = new Vector3
            (
                MathHelper.Clamp(Position.X, -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius, 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius),
                MathHelper.Clamp(Position.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius, 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius),
                0
            );
        }


        private void CheckAvailableTurretOptions()
        {
            if (ActualSelection.Turret == null)
                return;

            bool majEtaitIndisponible = !ActualSelection.AvailableTurretOptions[TurretChoice.Update];

            ActualSelection.AvailableTurretOptions[TurretChoice.Sell] =
                ActualSelection.Turret.CanSell &&
                !ActualSelection.Turret.Disabled;

            ActualSelection.AvailableTurretOptions[TurretChoice.Update] =
                ActualSelection.Turret.CanUpdate &&
                ActualSelection.Turret.UpdatePrice <= CommonStash.Cash;

            //des que l'option de maj redevient disponible, elle est selectionnee
            //if (majEtaitIndisponible && ActualSelection.AvailableTurretOptions[TurretChoice.Update])
            //    ActualSelection.TurretChoice = TurretChoice.Update;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            //if (!ActualSelection.AvailableTurretOptions[TurretChoice.Update] && ActualSelection.TurretChoice == TurretChoice.Update)
            //    ActualSelection.TurretChoice = TurretChoice.Sell;
        }


        private void CheckAvailableNewGameChoices()
        {
            ActualSelection.AvailableNewGameChoices[NewGameChoice.Continue] = Main.PlayerSaveGame.CurrentWorld > 0;

            var maxWorld = Main.PlayerSaveGame.LastUnlockedWorld;

            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld1] = maxWorld > 1;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld2] = maxWorld >= 2;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld3] = maxWorld >= 3;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld4] = maxWorld >= 4;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld5] = maxWorld >= 5;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld6] = maxWorld >= 6;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld7] = maxWorld >= 7;
            ActualSelection.AvailableNewGameChoices[NewGameChoice.WrapToWorld8] = maxWorld >= 8;

        }


        private void DoBouncing()
        {
            Core.Input.Inputs.VibrateController(BasePlayer, 150f, 0.5f, 0.5f);
        }
    }
}