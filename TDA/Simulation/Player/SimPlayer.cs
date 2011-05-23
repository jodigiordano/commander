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
        public bool InSpacehip;

        private Simulation Simulation;


        public SimPlayer(Simulation simulation)
        {
            Simulation = simulation;
            Cercle = new Cercle(Position, 8);
            InSpacehip = false;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection(Simulation);
            PreviousSelection = new PlayerSelection(Simulation);
            TurretToBuyController = new SelectedTurretToBuyController(Simulation, CommonStash);
            CelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Cercle);
            SelectedPowerUpController = new SelectedPowerUpController(Simulation.PowerUpsFactory.Availables, Cercle);
            ActualSelection.AvailableTurrets = TurretToBuyController.AvailableTurrets;
            InSpacehip = false;
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
            if (InSpacehip)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;
                ActualSelection.TurretOption = TurretAction.None;

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
                checkAvailablePowerUps();

                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = null;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }

            if (ActualSelection.CelestialBody != null)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                checkAvailablePowerUps();

                if (TurretToBuyController.TurretToBuy != null)
                    ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            }
            
            if (ActualSelection.Turret != null)
                checkAvailableTurretOptions();

            if (CelestialBodyController.CelestialBody != null &&
                CelestialBodyController.Turret == null &&
                CelestialBodyController.SelectedCelestialBodyChanged)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
                ActualSelection.TurretOption = TurretAction.None;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                checkAvailablePowerUps();

                //if (ActualSelection.TurretToBuy == null)
                //    ActualSelection.NextPowerUpToBuy();
            }

            else if (CelestialBodyController.Turret != null)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUpType.None;

                if (ActualSelection.TurretOption == TurretAction.None)
                    ActualSelection.TurretOption = TurretAction.Update;

                checkAvailableTurretOptions();
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

            notifyChanged();
            notifyMoved();
        }


        public void DoCelestialBodyDestroyed()
        {
            CelestialBodyController.DoCelestialBodyDestroyed();
        }


        private void notifyChanged()
        {
            if (Changed != null)
                Changed(this);
        }


        private void notifyMoved()
        {
            if (Moved != null)
                Moved(this);
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + Cercle.Radius, 640 - Preferences.DeadZoneXbox.X - Cercle.Radius);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + Cercle.Radius, 370 - Preferences.DeadZoneXbox.Y - Cercle.Radius);
        }


        private void checkAvailableTurretOptions()
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


        private void checkAvailablePowerUps()
        {
            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Values)
                ActualSelection.AvailablePowerUpsToBuy[powerUp.Type] =
                    powerUp.BuyPrice <= CommonStash.Cash &&
                    ActivesPowerUps[powerUp.Type];
        }
    }
}