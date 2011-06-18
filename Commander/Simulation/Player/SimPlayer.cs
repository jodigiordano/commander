namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SimPlayer
    {
        public List<CelestialBody> CelestialBodies;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;

        private Vector3 position;
        public PlayerSelection ActualSelection;

        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        public event SimPlayerHandler Changed;
        public event SimPlayerHandler Moved;

        public CommonStash CommonStash;

        public Circle Cercle;
        public PowerUpType PowerUpInUse;

        private Simulator Simulation;

        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;


        public SimPlayer(Simulator simulation)
        {
            Simulation = simulation;
            Cercle = new Circle(Position, 8);
            PowerUpInUse = PowerUpType.None;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection(CommonStash);
            TurretToBuyController = new SelectedTurretToBuyController(AvailableTurrets);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Cercle);
            SelectedPowerUpController = new SelectedPowerUpController(Simulation.PowerUpsFactory.Availables, Cercle);
            PowerUpInUse = PowerUpType.None;
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                VerifyFrame();
                Cercle.Position = position;
            }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            Position += delta * speed;
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
            ActualSelection.CheckAvailableTurretOptions();

            Position += SelectedCelestialBodyController.DoGlueMode();

            NotifyChanged();
            NotifyMoved();
        }


        public void DoCelestialBodyDestroyed()
        {
            SelectedCelestialBodyController.DoCelestialBodyDestroyed();
            ActualSelection.TurretToBuy = TurretType.None;
        }


        private void NotifyChanged()
        {
            if (Changed != null)
                Changed(this);
        }


        private void NotifyMoved()
        {
            if (Moved != null)
                Moved(this);
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.Xbox360DeadZoneV2.X + Cercle.Radius, 640 - Preferences.Xbox360DeadZoneV2.X - Cercle.Radius);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Cercle.Radius, 370 - Preferences.Xbox360DeadZoneV2.Y - Cercle.Radius);
        }
    }
}