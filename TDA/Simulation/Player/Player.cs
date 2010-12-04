namespace TDA
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;


    class Player
    {
        public Cursor Cursor;
        public List<CorpsCeleste> CelestialBodies;
        public Dictionary<PowerUp, bool> AvailableSpaceships;

        public int Score;
        public int Cash;
        public int Lives;
        private Vector3 position;
        public PlayerSelection ActualSelection;

        private SelectedCelestialBodyController CelestialBodyController;
        private SelectedTurretToBuyController TurretToBuyController;

        public delegate void PlayerHandler(Player player);
        public event PlayerHandler SelectionChanged;


        public Player()
        {
            Score = 0;
            Cash = 0;
            Lives = 0;
        }


        public void Initialize()
        {
            ActualSelection = new PlayerSelection();
            TurretToBuyController = new SelectedTurretToBuyController(this);
            CelestialBodyController = new SelectedCelestialBodyController(CelestialBodies, Cursor);
            ActualSelection.AvailableTurretsToBuy = TurretToBuyController.AvailableTurretsInScenario;
        }


        private void notifySelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this);
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                VerifyFrame();
                Cursor.Position = position;
            }
        }


        public void UpdateSelection()
        {
            CelestialBodyController.UpdateSelection();

            ActualSelection.CelestialBody = CelestialBodyController.CelestialBody;
            ActualSelection.TurretSpot = CelestialBodyController.TurretSpot;

            if (CelestialBodyController.TurretSpot != null && !CelestialBodyController.TurretSpot.EstOccupe)
            {
                TurretToBuyController.Update(ActualSelection.CelestialBody, ActualSelection.TurretSpot);
                ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
                ActualSelection.TurretOption = TurretAction.None;
                ActualSelection.CelestialBodyOption = PowerUp.Aucune;
            }

            else if (CelestialBodyController.TurretSpot != null && CelestialBodyController.TurretSpot.EstOccupe)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.CelestialBodyOption = PowerUp.Aucune;

                if (ActualSelection.TurretOption == TurretAction.None)
                    ActualSelection.TurretOption = TurretAction.Update;

                checkAvailableTurretOptions();
            }

            else if (CelestialBodyController.CelestialBody != null)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.TurretOption = TurretAction.None;
                ActualSelection.TurretSpot = null;

                checkAvailableCelestialBodyOptions();

                if (ActualSelection.CelestialBodyOption == PowerUp.Aucune)
                    ActualSelection.NextCelestialBodyOption();
            }

            else
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.CelestialBodyOption = PowerUp.Aucune;
                ActualSelection.TurretOption = TurretAction.None;
            }

            //if (CelestialBodyController.SelectedCelestialBodyChanged ||
            //    CelestialBodyController.SelectedTurretSpotChanged)
                notifySelectionChanged();
        }


        public void NextTurretToBuy()
        {
            TurretToBuyController.Next();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            notifySelectionChanged();
        }


        public void PreviousTurretToBuy()
        {
            TurretToBuyController.Previous();
            ActualSelection.TurretToBuy = TurretToBuyController.TurretToBuy;
            notifySelectionChanged();
        }


        public void NextPowerUpToBuy()
        {
            ActualSelection.NextCelestialBodyOption();
            notifySelectionChanged();
        }


        public void PreviousPowerUpToBuy()
        {
            ActualSelection.PreviousCelestialBodyOption();
            notifySelectionChanged();
        }


        public void NextTurretOption()
        {
            ActualSelection.NextTurretOption();
            notifySelectionChanged();
        }


        public void PreviousTurretOption()
        {
            ActualSelection.PreviousTurretOption();
            notifySelectionChanged();
        }


        public void Update(GameTime gameTime)
        {
            Position += CelestialBodyController.doGlueMode();
        }


        public void doTurretReactivated(Tourelle turret)
        {
            if (ActualSelection.Turret == turret)
                ActualSelection.TurretOption = TurretAction.Update;
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + Cursor.Width / 2, 640 - Preferences.DeadZoneXbox.X - Cursor.Width / 2);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + Cursor.Height / 2, 370 - Preferences.DeadZoneXbox.Y - Cursor.Height / 2);
        }


        private void checkAvailableTurretOptions()
        {
            bool majEtaitIndisponible = !ActualSelection.AvailableTurretOptions[TurretAction.Update];

            ActualSelection.AvailableTurretOptions[TurretAction.Sell] =
                ActualSelection.TurretSpot.Tourelle.PeutVendre;

            ActualSelection.AvailableTurretOptions[TurretAction.Update] =
                ActualSelection.TurretSpot.Tourelle.PeutMettreAJour &&
                ActualSelection.TurretSpot.Tourelle.PrixMiseAJour <= this.Cash;


            //des que l'option de maj redevient disponible, elle est selectionnee
            if (majEtaitIndisponible && ActualSelection.AvailableTurretOptions[TurretAction.Update])
                ActualSelection.TurretOption = TurretAction.Update;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            if (!ActualSelection.AvailableTurretOptions[TurretAction.Update] && ActualSelection.TurretOption == TurretAction.Update)
                ActualSelection.TurretOption = TurretAction.Sell;
        }


        private void checkAvailableCelestialBodyOptions()
        {
            ActualSelection.AvailableCelestialBodyOptions[PowerUp.DoItYourself] =
                AvailableSpaceships[PowerUp.DoItYourself] &&
                ActualSelection.CelestialBody.PeutAvoirDoItYourself &&
                ActualSelection.CelestialBody.PrixDoItYourself <= this.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.FinalSolution] =
                ActualSelection.CelestialBody.PeutDetruire &&
                ActualSelection.CelestialBody.PrixDestruction <= this.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.CollectTheRent] =
                AvailableSpaceships[PowerUp.CollectTheRent] &&
                ActualSelection.CelestialBody.PeutAvoirCollecteur &&
                ActualSelection.CelestialBody.PrixCollecteur <= this.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.TheResistance] =
                AvailableSpaceships[PowerUp.TheResistance] &&
                ActualSelection.CelestialBody.PeutAvoirTheResistance &&
                ActualSelection.CelestialBody.PrixTheResistance <= this.Cash;
        }
    }
}