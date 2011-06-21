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

        public PlayerSelection ActualSelection;

        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        public CommonStash CommonStash;

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
                AutomaticMode = false
            };

            TurretToPlaceChanged = false;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection(CommonStash);
            TurretToBuyController = new SelectedTurretToBuyController(AvailableTurrets);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Circle);
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
            SelectedCelestialBodyController.Update();

            ActualSelection.CelestialBody = SelectedCelestialBodyController.CelestialBody;

            if (SelectedCelestialBodyController.CelestialBody != null &&
                SelectedCelestialBodyController.SelectedCelestialBodyChanged)
            {
                ActualSelection.GameAction = GameAction.None;
                ActualSelection.NextGameAction();
            }

            else if (ActualSelection.CelestialBody == null)
            {
                ActualSelection.GameAction = GameAction.None;
            }
        }


        public void UpdateSelection()
        {
            // In a power-up
            if (PowerUpInUse != PowerUpType.None)
            {
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretOption = TurretAction.None;
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
                ActualSelection.TurretOption = TurretAction.None;

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
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }


            if (SelectedCelestialBodyController.CelestialBody != null &&
                SelectedCelestialBodyController.Turret == null &&
                SelectedCelestialBodyController.SelectedCelestialBodyChanged)
            {
                ActualSelection.TurretOption = TurretAction.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                return;
            }


            if (SelectedCelestialBodyController.Turret != null)
            {
                ActualSelection.TurretToBuy = TurretType.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                if (ActualSelection.TurretOption == TurretAction.None)
                    ActualSelection.TurretOption = TurretAction.Update;

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
                ActualSelection.TurretOption = TurretAction.None;

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


        public void NextShitToBuy()
        {
            TurretToBuyController.Next();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
        }


        public void PreviousShitToBuy()
        {
            TurretToBuyController.Previous();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
        }


        public void NextGameAction()
        {
            ActualSelection.NextGameAction();
        }


        public void PreviousGameAction()
        {
            ActualSelection.PreviousGameAction();
        }


        public void NextTurretOption()
        {
            ActualSelection.NextTurretOption();
        }


        public void PreviousTurretOption()
        {
            ActualSelection.PreviousTurretOption();
        }


        public void Update()
        {
            if (ActualSelection.TurretToPlace == null)
            {
                SpaceshipMove.Update();
                Position = SpaceshipMove.Position;
                Direction = SpaceshipMove.Direction;
                SpaceshipMove.NextInput = Vector3.Zero;
            }

            else
            {
                SpaceshipMove.Position = Position;
            }

            ActualSelection.CheckAvailableTurretOptions();

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
    }
}