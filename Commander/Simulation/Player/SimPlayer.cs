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

        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        public CommonStash CommonStash;

        public bool UpdateSelectionz;

        public Circle Circle;
        public Color Color;
        public string Representation;
        public PowerUpType PowerUpInUse;

        private Vector3 position;
        private Vector3 direction;
        private Simulator Simulator;

        private SpaceshipSpaceship SpaceshipMove;

        public bool TurretToPlaceChanged;


        public SimPlayer(Simulator simulator)
        {
            Simulator = simulator;
            Circle = new Circle(Position, 8);
            PowerUpInUse = PowerUpType.None;

            SpaceshipMove = new SpaceshipSpaceship(simulator)
            {
                AutomaticMode = false,
                Speed = 3
            };

            TurretToPlaceChanged = false;
        }


        public void Initialize()
        {
            ActualSelection = new SimPlayerSelection();
            TurretToBuyController = new SelectedTurretToBuyController(AvailableTurrets);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(Simulator, this, CelestialBodies);
            SelectedPowerUpController = new SelectedPowerUpController(Simulator.PowerUpsFactory.Availables, Circle);
            PowerUpInUse = PowerUpType.None;
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
            get { return SpaceshipMove.NextInput; }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            if (ActualSelection.TurretToPlace != null)
            {
                Position += delta * speed;
                SpaceshipMove.Stop();
            }

            else
            {
                SpaceshipMove.NextInput = delta;
            }
        }


        public void UpdateDemoSelection()
        {
            //SelectedCelestialBodyController.Update();

            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (SelectedCelestialBodyController.CelestialBody != null &&
                SelectedCelestialBodyController.SelectedCelestialBodyChanged)
            {
                ActualSelection.GameChoice = PausedGameChoice.None;
                NextGameAction();
            }

            else if (ActualSelection.CelestialBody == null)
            {
                ActualSelection.GameChoice = PausedGameChoice.None;
            }
        }


        public void UpdateSelection()
        {
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
                SelectedCelestialBodyController.Initialize();

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


        public void NextGameAction()
        {
            int actual = (int) ActualSelection.GameChoice;
            int nbChoices = 2;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            ActualSelection.GameChoice = (PausedGameChoice) actual;
        }


        public void PreviousGameAction()
        {
            int actual = (int) ActualSelection.GameChoice;
            int nbChoices = 2;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            ActualSelection.GameChoice = (PausedGameChoice) actual;
        }


        public void NextTurretOption()
        {
            int actual = (int) ActualSelection.TurretChoice;
            int nbChoices = ActualSelection.AvailableTurretOptions.Count;
            int next = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual += 1;

                if (actual >= nbChoices)
                    actual = 0;

                if (ActualSelection.AvailableTurretOptions[(TurretChoice) actual])
                {
                    next = actual;
                    break;
                }
            }

            ActualSelection.TurretChoice = (TurretChoice) next;
        }


        public void PreviousTurretOption()
        {
            int actual = (int) ActualSelection.TurretChoice;
            int nbChoices = ActualSelection.AvailableTurretOptions.Count;
            int previous = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual -= 1;

                if (actual < 0)
                    actual = nbChoices - 1;

                if (ActualSelection.AvailableTurretOptions[(TurretChoice) actual])
                {
                    previous = actual;
                    break;
                }
            }

            ActualSelection.TurretChoice = (TurretChoice) previous;
        }


        public void Update()
        {
            if (ActualSelection.TurretToPlace == null)
            {
                if (SpaceshipMove.NextInput == Vector3.Zero &&
                    (ActualSelection.CelestialBody != null || ActualSelection.Turret != null))
                    SpaceshipMove.Friction = 0.05f;

                SpaceshipMove.Update();
                Position = SpaceshipMove.Position;
                Direction = SpaceshipMove.Direction;
            }

            else
            {
                SpaceshipMove.Position = Position;
            }

            CheckAvailableTurretOptions();

            Position += SelectedCelestialBodyController.DoGlueMode();

            // Manage turret to place (to always has a valid position)
            if (ActualSelection.TurretToPlace != null)
            {
                Turret turretToPlace = ActualSelection.TurretToPlace;
                CelestialBody celestialBody = ActualSelection.TurretToPlace.CelestialBody;
                turretToPlace.Position = position;

                if (celestialBody.OuterTurretZone.Outside(ref position))
                    Position = celestialBody.OuterTurretZone.NearestPointToCircumference(ref position);

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

            if (Simulator.DemoMode)
                UpdateDemoSelection();

            SpaceshipMove.NextInput = Vector3.Zero;
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyController.DoCelestialBodyDestroyed();
            ActualSelection.TurretToBuy = TurretType.None;
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(position.X, -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius, 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius);
            position.Y = MathHelper.Clamp(position.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius, 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius);
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
            if (majEtaitIndisponible && ActualSelection.AvailableTurretOptions[TurretChoice.Update])
                ActualSelection.TurretChoice = TurretChoice.Update;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            if (!ActualSelection.AvailableTurretOptions[TurretChoice.Update] && ActualSelection.TurretChoice == TurretChoice.Update)
                ActualSelection.TurretChoice = TurretChoice.Sell;
        }
    }
}