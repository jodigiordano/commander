namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;


    class SimPlayer
    {
        public List<CorpsCeleste> CelestialBodies;
        public Dictionary<PowerUp, bool> AvailableSpaceships;

        private Vector3 position;
        private PlayerSelection PreviousSelection;
        public PlayerSelection ActualSelection;

        private SelectedCelestialBodyController CelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;

        public event SimPlayerHandler Changed;
        public event SimPlayerHandler Moved;

        public CommonStash CommonStash;

        public Cercle Cercle;
        public bool InSpacehip;


        public SimPlayer()
        {
            Cercle = new Cercle(Position, 8);
            InSpacehip = false;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection();
            PreviousSelection = new PlayerSelection();
            TurretToBuyController = new SelectedTurretToBuyController(CommonStash);
            CelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Cercle);
            ActualSelection.AvailableTurretsToBuy = TurretToBuyController.AvailableTurretsInScenario;
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
                ActualSelection.PowerUpToBuy = PowerUp.None;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }

            else if (ActualSelection.TurretToPlace != null)
            {
                CelestialBodyController.Initialize();

                ActualSelection.CelestialBody = null;
                ActualSelection.Turret = null;
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUp.None;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }

            CelestialBodyController.UpdateSelection();

            ActualSelection.CelestialBody = CelestialBodyController.CelestialBody;
            ActualSelection.Turret = CelestialBodyController.Turret;

            if (ActualSelection.CelestialBody != null)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody);
                checkAvailableCelestialBodyOptions();
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
                ActualSelection.PowerUpToBuy = PowerUp.None;

                checkAvailableCelestialBodyOptions();

                if (ActualSelection.TurretToBuy == null)
                    ActualSelection.NextPowerUpToBuy();
            }

            else if (CelestialBodyController.Turret != null)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.PowerUpToBuy = PowerUp.None;

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
                ActualSelection.PowerUpToBuy = PowerUp.None;
                ActualSelection.TurretOption = TurretAction.None;
            }
        }


        public void NextTurretToBuy()
        {
            TurretToBuyController.Next();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            //notifyChanged();
        }


        public void PreviousTurretToBuy()
        {
            TurretToBuyController.Previous();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            //notifyChanged();
        }


        public void NextShitToBuy()
        {
            if (ActualSelection.TurretToBuy != null)
            {
                ActualSelection.PowerUpToBuy = PowerUp.None;

                TurretToBuyController.Next();
                ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;

                if (TurretToBuyController.TurretToBuy == TurretToBuyController.FirstAvailable &&
                    ActualSelection.FirstPowerUpToBuyAvailable != PowerUp.None)
                {
                    ActualSelection.TurretToBuy = null;
                    ActualSelection.PowerUpToBuy = ActualSelection.FirstPowerUpToBuyAvailable;

                    if (ActualSelection.PowerUpToBuy == PowerUp.None)
                    {
                        TurretToBuyController.Next();
                        ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
                    }
                }
            }

            else
            {
                ActualSelection.TurretToBuy = null;

                ActualSelection.NextPowerUpToBuy();

                if (ActualSelection.PowerUpToBuy == PowerUp.None ||
                    ActualSelection.PowerUpToBuy == ActualSelection.FirstPowerUpToBuyAvailable)
                {
                    ActualSelection.PowerUpToBuy = PowerUp.None;
                    ActualSelection.TurretToBuy = TurretToBuyController.FirstAvailable;
                    TurretToBuyController.SetSelectedTurret(ActualSelection.TurretToBuy);

                    if (ActualSelection.TurretToBuy == null)
                        ActualSelection.NextPowerUpToBuy();
                }
            }
        }


        public void PreviousShitToBuy()
        {
            if (ActualSelection.TurretToBuy != null)
            {
                ActualSelection.PowerUpToBuy = PowerUp.None;

                TurretToBuyController.Previous();
                ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;

                if (TurretToBuyController.TurretToBuy == TurretToBuyController.LastAvailable &&
                    ActualSelection.LastPowerUpToBuyAvailable != PowerUp.None)
                {
                    ActualSelection.TurretToBuy = null;
                    ActualSelection.PowerUpToBuy = ActualSelection.LastPowerUpToBuyAvailable;

                    if (ActualSelection.PowerUpToBuy == PowerUp.None)
                    {
                        TurretToBuyController.Previous();
                        ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
                    }
                }
            }

            else
            {
                ActualSelection.TurretToBuy = null;

                ActualSelection.PreviousPowerUpToBuy();

                if (ActualSelection.PowerUpToBuy == PowerUp.None ||
                    ActualSelection.PowerUpToBuy == ActualSelection.LastPowerUpToBuyAvailable)
                {
                    ActualSelection.PowerUpToBuy = PowerUp.None;
                    ActualSelection.TurretToBuy = TurretToBuyController.LastAvailable;
                    TurretToBuyController.SetSelectedTurret(ActualSelection.TurretToBuy);

                    if (ActualSelection.TurretToBuy == null)
                        ActualSelection.PreviousPowerUpToBuy();
                }
            }
        }


        public void NextGameAction()
        {
            ActualSelection.NextGameAction();
        }


        public void PreviousGameAction()
        {
            ActualSelection.PreviousGameAction();
        }


        public void NextPowerUpToBuy()
        {
            ActualSelection.NextPowerUpToBuy();
        }


        public void PreviousPowerUpToBuy()
        {
            ActualSelection.PreviousPowerUpToBuy();
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
                ActualSelection.Turret.CanSell;

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


        private void checkAvailableCelestialBodyOptions()
        {
            ActualSelection.AvailablePowerUpsToBuy[PowerUp.DoItYourself] =
                AvailableSpaceships[PowerUp.DoItYourself] &&
                ActualSelection.CelestialBody.PeutAvoirDoItYourself &&
                ActualSelection.CelestialBody.PrixDoItYourself <= CommonStash.Cash;

            ActualSelection.AvailablePowerUpsToBuy[PowerUp.FinalSolution] =
                ActualSelection.CelestialBody.PeutDetruire &&
                ActualSelection.CelestialBody.PrixDestruction <= CommonStash.Cash;

            ActualSelection.AvailablePowerUpsToBuy[PowerUp.CollectTheRent] =
                AvailableSpaceships[PowerUp.CollectTheRent] &&
                ActualSelection.CelestialBody.PeutAvoirCollecteur &&
                ActualSelection.CelestialBody.PrixCollecteur <= CommonStash.Cash;

            ActualSelection.AvailablePowerUpsToBuy[PowerUp.TheResistance] =
                AvailableSpaceships[PowerUp.TheResistance] &&
                ActualSelection.CelestialBody.PeutAvoirTheResistance &&
                ActualSelection.CelestialBody.PrixTheResistance <= CommonStash.Cash;
        }
    }
}