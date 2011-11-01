namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class SimPlayer
    {
        public List<CelestialBody> CelestialBodies;
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public SimPlayerSelection ActualSelection;
        public SimPlayerSelection LastSelection;
        public CommonStash CommonStash;
        public bool UpdateSelectionz;
        public PowerUpType PowerUpInUse;
        public Player InnerPlayer;
        public PausePlayer PausePlayer;
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
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        private bool firing;
        private Metronome VibrationMetronome;


        public SimPlayer(Simulator simulator, Player player)
        {
            Simulator = simulator;
            InnerPlayer = player;
            PowerUpInUse = PowerUpType.None;

            SpaceshipMove = new Spaceship(simulator)
            {
                Speed = 4,
                VisualPriority = VisualPriorities.Default.PlayerCursor,
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

            VibrationMetronome = new Metronome(Preferences.TargetElapsedTimeMs, 100);
        }


        public void Initialize()
        {
            ActualSelection = new SimPlayerSelection();
            LastSelection = new SimPlayerSelection();
            TurretToBuyController = new SelectedTurretToBuyAllController(AvailableTurrets);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(Simulator, this, CelestialBodies);
            SelectedPowerUpController = new SelectedPowerUpController(Simulator.PowerUpsFactory.Availables, Circle);
            PowerUpInUse = PowerUpType.None;

            MovingLeft = MovingRight = MovingUp = MovingDown = false;
            LastMouseDirection = Vector3.Zero;

            Firing = false;

            if (InnerPlayer.InputType == Core.Input.InputType.MouseAndKeyboard)
            {
                SpaceshipMove.SteeringBehavior = new SpaceshipMouseMBehavior(SpaceshipMove);
                PausePlayer.SpaceshipMove.SteeringBehavior = new SpaceshipMouseMBehavior(PausePlayer.SpaceshipMove);
            }
            else if (InnerPlayer.InputType == Core.Input.InputType.KeyboardOnly)
            {
                SpaceshipMove.SteeringBehavior = new SpaceshipKeyboardMBehavior(SpaceshipMove);
                PausePlayer.SpaceshipMove.SteeringBehavior = new SpaceshipKeyboardMBehavior(PausePlayer.SpaceshipMove);
            }
            else if (InnerPlayer.InputType == Core.Input.InputType.Gamepad)
            {
                SpaceshipMove.SteeringBehavior = new SpaceshipGamePadMBehavior(SpaceshipMove);
                PausePlayer.SpaceshipMove.SteeringBehavior = new SpaceshipGamePadMBehavior(PausePlayer.SpaceshipMove);
            }
        }


        public Vector3 Position
        {
            get { return InnerPlayer.Position; }
            set
            {
                InnerPlayer.Position = SpaceshipMove.Position = value;
                VerifyFrame();
            }
        }


        public Circle Circle
        {
            get { return InnerPlayer.Circle; }
            set
            {
                InnerPlayer.Circle = value;
            }
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
                VerifyFrame();
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
            set
            {
                SpaceshipMove.Weapon.AttackPoints = value;
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


        public void UpdateWorldSelection()
        {
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (Simulator.EditorWorldMode)
            {
                if (ActualSelection.CelestialBody != null &&
                    Simulator.Scene.EnableInputs &&
                    ActualSelection.EditorWorldChoice == EditorWorldChoice.None)
                {
                    NextEditorWorldChoice();
                }

                else if (ActualSelection.CelestialBody == null || !Simulator.Scene.EnableInputs)
                {
                    ActualSelection.EditorWorldChoice = EditorWorldChoice.None;
                }
            }

            else if (
                Simulator.Scene.EnableInputs &&
                Main.CurrentWorld.GetGamePausedSelected(InnerPlayer) &&
                ActualSelection.PausedGameChoice == PausedGameChoice.None)
            {
                NextPausedGameChoice();
            }

            else if (Main.CurrentGame == null ||
                     ActualSelection.CelestialBody == null ||
                    !Main.CurrentWorld.GetGamePausedSelected(InnerPlayer))
            {
                ActualSelection.PausedGameChoice = PausedGameChoice.None;
            }
        }


        public void UpdateMainMenuSelection()
        {
            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (ActualSelection.CelestialBody != null &&
                LevelsFactory.IsCampaignCB(ActualSelection.CelestialBody) &&
                ActualSelection.NewGameChoice == -1)
            {
                NextNewGameChoice();
            }

            else if (ActualSelection.CelestialBody == null || !LevelsFactory.IsCampaignCB(ActualSelection.CelestialBody))
                ActualSelection.NewGameChoice = -1;
        }


        public void UpdateSelection()
        {
            LastSelection.Sync(ActualSelection);

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


        public void NextEditorWorldChoice()
        {
            int actual = (int) ActualSelection.EditorWorldChoice;
            int nbChoices = 4;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            ActualSelection.EditorWorldChoice = (EditorWorldChoice) actual;
        }


        public void PreviousEditorWorldChoice()
        {
            int actual = (int) ActualSelection.EditorWorldChoice;
            int nbChoices = 4;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            ActualSelection.EditorWorldChoice = (EditorWorldChoice) actual;
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

                if (ActualSelection.AvailableNewGameChoices[actual])
                {
                    next = actual;
                    break;
                }
            }

            ActualSelection.NewGameChoice = next;
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

                if (ActualSelection.AvailableNewGameChoices[actual])
                {
                    previous = actual;
                    break;
                }
            }

            ActualSelection.NewGameChoice = previous;
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

            // Manage turret to place (to always have a valid position)
            if (ActualSelection.TurretToPlace != null)
            {
                Turret turretToPlace = ActualSelection.TurretToPlace;
                CelestialBody celestialBody = ActualSelection.TurretToPlace.CelestialBody;
                turretToPlace.Position = Position;

                if (celestialBody.OuterTurretZone.Outside(Position))
                    Position = celestialBody.OuterTurretZone.NearestPointToCircumference(Position);

                turretToPlace.CanPlace = celestialBody.InnerTurretZone.Outside(turretToPlace.Position);
                turretToPlace.CanPlace = turretToPlace.CanPlace && turretToPlace.BuyPrice <= CommonStash.Cash; //multiplayer failsafe.

                if (turretToPlace.CanPlace)
                    foreach (var turret in celestialBody.Turrets)
                    {
                        turretToPlace.CanPlace =
                            !turret.Visible ||
                            !Physics.CircleCicleCollision(turret.Circle, turretToPlace.Circle);

                        if (!turretToPlace.CanPlace)
                            break;
                    }
            }

            if (UpdateSelectionz)
                UpdateSelection();

            if (Simulator.WorldMode)
                UpdateWorldSelection();

            if (Simulator.DemoMode)
                UpdateMainMenuSelection();

            if (Firing)
            {
                VibrationMetronome.Update();
            
                if (VibrationMetronome.CyclesCountThisTick != 0)
                    Core.Input.Inputs.VibrateControllerHighFrequency(InnerPlayer, 80, 0.4f);

                LastMouseDirection = SpaceshipMove.Direction;
            }
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyController.DoCelestialBodyDestroyed();
            ActualSelection.TurretToBuy = TurretType.None;
        }


        private void VerifyFrame()
        {
            InnerPlayer.Position = new Vector3
            (
                MathHelper.Clamp(Position.X, Simulator.Battlefield.Left + Circle.Radius, Simulator.Battlefield.Right - Circle.Radius),
                MathHelper.Clamp(Position.Y, Simulator.Battlefield.Top + Circle.Radius, Simulator.Battlefield.Bottom - Circle.Radius),
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
                ActualSelection.Turret.UpgradePrice <= CommonStash.Cash;
        }


        private void CheckAvailableNewGameChoices()
        {
            ActualSelection.AvailableNewGameChoices.Clear();
            ActualSelection.AvailableNewGameChoices.Add(0, Main.PlayersController.CampaignData.CurrentWorld > 0);
            ActualSelection.AvailableNewGameChoices.Add(1, true);

            int index = 2;

            foreach (var w in Main.LevelsFactory.Worlds.Values)
                if (w.Unlocked)
                    ActualSelection.AvailableNewGameChoices.Add(index++, true);
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
    }
}