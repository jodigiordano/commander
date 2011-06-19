namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    public enum TurretAction
    {
        None = -1,
        Sell = 0,
        Update = 1
    }


    public enum GameAction
    {
        None = -1,
        Resume = 0,
        New = 1
    }


    class PlayerSelection
    {
        public CelestialBody CelestialBody;

        public PowerUpType PowerUpToBuy;

        public Turret Turret;
        public TurretAction TurretOption;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;

        public TurretType TurretToBuy;
        public Turret TurretToPlace;

        public GameAction GameAction;

        private CommonStash CommonStash;


        public PlayerSelection(CommonStash commonStash)
        {
            CommonStash = commonStash;

            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
            Turret = null;
            TurretToBuy = TurretType.None;
            TurretOption = TurretAction.None;
            TurretToPlace = null;
            GameAction = GameAction.None;

            AvailableTurretOptions = new Dictionary<TurretAction, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretAction.Sell, false);
            AvailableTurretOptions.Add(TurretAction.Update, false);
        }


        public void NextGameAction()
        {
            int actual = (int) GameAction;
            int nbChoices = 2;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            GameAction = (GameAction) actual;
        }


        public void PreviousGameAction()
        {
            int actual = (int) GameAction;
            int nbChoices = 2;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            GameAction = (GameAction) actual;
        }


        public void NextTurretOption()
        {
            int actual = (int) TurretOption;
            int nbChoices = AvailableTurretOptions.Count;
            int next = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual += 1;

                if (actual >= nbChoices)
                    actual = 0;

                if (AvailableTurretOptions[(TurretAction)actual])
                {
                    next = actual;
                    break;
                }
            }

            TurretOption = (TurretAction)next;
        }


        public void PreviousTurretOption()
        {
            int actual = (int) TurretOption;
            int nbChoices = AvailableTurretOptions.Count;
            int previous = actual;

            for (int i = 1; i < nbChoices; i++)
            {
                actual -= 1;

                if (actual < 0)
                    actual = nbChoices - 1;

                if (AvailableTurretOptions[(TurretAction) actual])
                {
                    previous = actual;
                    break;
                }
            }

            TurretOption = (TurretAction)previous;
        }


        public void CheckAvailableTurretOptions()
        {
            if (Turret == null)
                return;

            bool majEtaitIndisponible = !AvailableTurretOptions[TurretAction.Update];

            AvailableTurretOptions[TurretAction.Sell] =
                Turret.CanSell &&
                !Turret.Disabled;

            AvailableTurretOptions[TurretAction.Update] =
                Turret.CanUpdate &&
                Turret.UpdatePrice <= CommonStash.Cash;

            //des que l'option de maj redevient disponible, elle est selectionnee
            if (majEtaitIndisponible && AvailableTurretOptions[TurretAction.Update])
                TurretOption = TurretAction.Update;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            if (!AvailableTurretOptions[TurretAction.Update] && TurretOption == TurretAction.Update)
                TurretOption = TurretAction.Sell;
        }
    }
}
