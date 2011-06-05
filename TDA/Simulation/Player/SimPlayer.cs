namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;


    class SimPlayer
    {
        public List<CorpsCeleste> CelestialBodies;
        public Dictionary<PowerUpType, bool> ActivesPowerUps;

        private Vector3 position;
        private PlayerSelection PreviousSelection;
        public PlayerSelection ActualSelection;

        private SelectedCelestialBodyController CelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;
        private SelectedPowerUpController SelectedPowerUpController;

        public event SimPlayerHandler Changed;
        public event SimPlayerHandler Moved;

        public CommonStash CommonStash;

        public Cercle Cercle;
        public PowerUpType PowerUpInUse;

        private Simulation Simulation;


        public SimPlayer(Simulation simulation)
        {
            Simulation = simulation;
            Cercle = new Cercle(Position, 8);
            PowerUpInUse = PowerUpType.None;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection(Simulation);
            PreviousSelection = new PlayerSelection(Simulation);
            TurretToBuyController = new SelectedTurretToBuyController(Simulation, CommonStash);
            CelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Cercle);
            SelectedPowerUpController = new SelectedPowerUpController(Simulation.PowerUpsFactory.Availables, Cercle);
            ActualSelection.AvailableTurrets = TurretToBuyController.AvailableTurrets;
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
            CelestialBodyController.UpdateSelection();

            ActualSelection.CelestialBody = CelestialBodyController.CelestialBody;

            if (CelestialBodyController.CelestialBody != null &&
                CelestialBodyController.SelectedCelestialBodyChanged)
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
            if (PowerUpInUse != PowerUpType.None)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretOption = TurretAction.None;

                if (PowerUpInUse == PowerUpType.FinalSolution)
                {
                    CelestialBodyController.UpdateSelection();
                    ActualSelection.CelestialBody = CelestialBodyController.CelestialBody;
                }

                return;
            }

            else if (ActualSelection.TurretToPlace != null)
            {
                CelestialBodyController.Initialize();

                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }

            CelestialBodyController.UpdateSelection();
            SelectedPowerUpController.UpdateSelection();

            ActualSelection.CelestialBody = CelestialBodyController.CelestialBody;
            ActualSelection.Turret = CelestialBodyController.Turret;
            ActualSelection.PowerUpToBuy = SelectedPowerUpController.PowerUpToBuy;

            if (ActualSelection.PowerUpToBuy != PowerUpType.None)
            {
                CheckAvailablePowerUps();

                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = null;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }

            if (ActualSelection.CelestialBody != null)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                CheckAvailablePowerUps();

                if (TurretToBuyController.TurretToBuy != null)
                    ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            }
            
            if (ActualSelection.Turret != null)
                CheckAvailableTurretOptions();

            if (CelestialBodyController.CelestialBody != null &&
                CelestialBodyController.Turret == null &&
                CelestialBodyController.SelectedCelestialBodyChanged)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
                ActualSelection.TurretOption = TurretAction.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                CheckAvailablePowerUps();

                //if (ActualSelection.TurretToBuy == null)
                //    ActualSelection.NextPowerUpToBuy();
            }

            else if (CelestialBodyController.Turret != null)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                if (ActualSelection.TurretOption == TurretAction.None)
                    ActualSelection.TurretOption = TurretAction.Update;

                CheckAvailableTurretOptions();
            }

            else if (CelestialBodyController.CelestialBody == null &&
                     CelestialBodyController.Turret == null)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretOption = TurretAction.None;
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


        public void Update(GameTime gameTime)
        {
            Position += CelestialBodyController.DoGlueMode();

            NotifyChanged();
            NotifyMoved();
        }


        public void DoCelestialBodyDestroyed()
        {
            CelestialBodyController.DoCelestialBodyDestroyed();
        }


        public void CheckAvailablePowerUps()
        {
            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Values)
                ActualSelection.AvailablePowerUpsToBuy[powerUp.Type] =
                    powerUp.BuyPrice <= CommonStash.Cash &&
                    ActivesPowerUps[powerUp.Type];
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
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + Cercle.Radius, 640 - Preferences.DeadZoneXbox.X - Cercle.Radius);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + Cercle.Radius, 370 - Preferences.DeadZoneXbox.Y - Cercle.Radius);
        }


        private void CheckAvailableTurretOptions()
        {
            bool majEtaitIndisponible = !ActualSelection.AvailableTurretOptions[TurretAction.Update];

            ActualSelection.AvailableTurretOptions[TurretAction.Sell] =
                ActualSelection.Turret.CanSell &&
                !ActualSelection.Turret.Disabled;

            ActualSelection.AvailableTurretOptions[TurretAction.Update] =
                ActualSelection.Turret.CanUpdate &&
                ActualSelection.Turret.UpdatePrice <= CommonStash.Cash;


            //des que l'option de maj redevient disponible, elle est selectionnee
            if (majEtaitIndisponible && ActualSelection.AvailableTurretOptions[TurretAction.Update])
                ActualSelection.TurretOption = TurretAction.Update;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            if (!ActualSelection.AvailableTurretOptions[TurretAction.Update] && ActualSelection.TurretOption == TurretAction.Update)
                ActualSelection.TurretOption = TurretAction.Sell;
        }
    }
}