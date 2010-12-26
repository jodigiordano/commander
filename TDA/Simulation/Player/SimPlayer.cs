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


        public void UpdateSelection()
        {
            if (InSpacehip)
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.CelestialBodyOption = PowerUp.Aucune;
                ActualSelection.TurretOption = TurretAction.None;

                return;
            }


            //PreviousSelection.SynchronizeFrom(ActualSelection);

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

                if (ActualSelection.CelestialBodyOption == PowerUp.Aucune ||
                    !ActualSelection.AvailableCelestialBodyOptions[ActualSelection.CelestialBodyOption])
                    ActualSelection.NextCelestialBodyOption();
            }

            else
            {
                ActualSelection.TurretToBuy = null;
                ActualSelection.CelestialBodyOption = PowerUp.Aucune;
                ActualSelection.TurretOption = TurretAction.None;
            }

            //if (!ActualSelection.Equals(PreviousSelection))
            //    notifyChanged();
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


        public void NextPowerUpToBuy()
        {
            ActualSelection.NextCelestialBodyOption();
            //notifyChanged();
        }


        public void PreviousPowerUpToBuy()
        {
            ActualSelection.PreviousCelestialBodyOption();
            //notifyChanged();
        }


        public void NextTurretOption()
        {
            ActualSelection.NextTurretOption();
            //notifyChanged();
        }


        public void PreviousTurretOption()
        {
            ActualSelection.PreviousTurretOption();
            //notifyChanged();
        }


        public void Update(GameTime gameTime)
        {
            //Vector3 delta = CelestialBodyController.doGlueMode();

            //if (delta != Vector3.Zero)
            //{
            //    Position += delta;
            //    notifyMoved();
            //}

            Position += CelestialBodyController.doGlueMode();

            notifyChanged();
            notifyMoved();
        }


        public void DoCelestialBodyDestroyed()
        {
            CelestialBodyController.doCelestialBodyDestroyed();
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
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + Cercle.Rayon, 640 - Preferences.DeadZoneXbox.X - Cercle.Rayon);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + Cercle.Rayon, 370 - Preferences.DeadZoneXbox.Y - Cercle.Rayon);
        }


        private void checkAvailableTurretOptions()
        {
            bool majEtaitIndisponible = !ActualSelection.AvailableTurretOptions[TurretAction.Update];

            ActualSelection.AvailableTurretOptions[TurretAction.Sell] =
                ActualSelection.TurretSpot.Tourelle.PeutVendre;

            ActualSelection.AvailableTurretOptions[TurretAction.Update] =
                ActualSelection.TurretSpot.Tourelle.PeutMettreAJour &&
                ActualSelection.TurretSpot.Tourelle.PrixMiseAJour <= CommonStash.Cash;


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
                ActualSelection.CelestialBody.PrixDoItYourself <= CommonStash.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.FinalSolution] =
                ActualSelection.CelestialBody.PeutDetruire &&
                ActualSelection.CelestialBody.PrixDestruction <= CommonStash.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.CollectTheRent] =
                AvailableSpaceships[PowerUp.CollectTheRent] &&
                ActualSelection.CelestialBody.PeutAvoirCollecteur &&
                ActualSelection.CelestialBody.PrixCollecteur <= CommonStash.Cash;

            ActualSelection.AvailableCelestialBodyOptions[PowerUp.TheResistance] =
                AvailableSpaceships[PowerUp.TheResistance] &&
                ActualSelection.CelestialBody.PeutAvoirTheResistance &&
                ActualSelection.CelestialBody.PrixTheResistance <= CommonStash.Cash;
        }
    }
}