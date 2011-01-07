namespace EphemereGames.Commander
{
    using System.Collections.Generic;


    public enum PowerUp
    {
        None = -1,
        DoItYourself = 0,
        CollectTheRent = 1,
        FinalSolution = 2,
        TheResistance = 3
    }


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
        public CorpsCeleste CelestialBody;
        public PowerUp PowerUpToBuy;
        public Dictionary<PowerUp, bool> AvailablePowerUpsToBuy;

        public Turret Turret;
        public TurretAction TurretOption;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;

        public Turret TurretToBuy;
        public Turret TurretToPlace;
        public Dictionary<Turret, bool> AvailableTurretsToBuy;

        public GameAction GameAction;


        public PlayerSelection()
        {
            AvailablePowerUpsToBuy = new Dictionary<PowerUp, bool>();
            AvailablePowerUpsToBuy.Add(PowerUp.CollectTheRent, false);
            AvailablePowerUpsToBuy.Add(PowerUp.DoItYourself, false);
            AvailablePowerUpsToBuy.Add(PowerUp.FinalSolution, false);
            AvailablePowerUpsToBuy.Add(PowerUp.TheResistance, false);

            AvailableTurretOptions = new Dictionary<TurretAction, bool>();
            AvailableTurretOptions.Add(TurretAction.Sell, false);
            AvailableTurretOptions.Add(TurretAction.Update, false);

            AvailableTurretsToBuy = new Dictionary<Turret, bool>();

            CelestialBody = null;
            PowerUpToBuy = PowerUp.None;
            Turret = null;
            TurretToBuy = null;
            TurretOption = TurretAction.None;
            TurretToPlace = null;
            GameAction = GameAction.None;
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


        public void NextPowerUpToBuy()
        {
            PowerUp actual = PowerUpToBuy;

            int nbChoices = AvailablePowerUpsToBuy.Count;
            int actualInt = (int) actual;
            int next = -1;

            for (int i = 1; i < nbChoices; i++)
            {
                actualInt += 1;

                if (actualInt >= nbChoices)
                    actualInt = 0;

                if (AvailablePowerUpsToBuy[(PowerUp)actualInt])
                {
                    next = actualInt;
                    break;
                }
            }

            PowerUpToBuy = (PowerUp) next;
        }


        public void PreviousPowerUpToBuy()
        {
            PowerUp actual = PowerUpToBuy;

            int nbChoices = AvailablePowerUpsToBuy.Count;
            int actualInt = (int) actual;
            int previous = -1;

            for (int i = 1; i < nbChoices; i++)
            {
                actualInt -= 1;

                if (actualInt < 0)
                    actualInt = nbChoices - 1;

                if (AvailablePowerUpsToBuy[(PowerUp)actualInt])
                {
                    previous = actualInt;
                    break;
                }
            }

            PowerUpToBuy = (PowerUp) previous;
        }


        public PowerUp FirstPowerUpToBuyAvailable
        {
            get
            {
                PowerUp actual = PowerUp.None;

                for (int i = 0; i < AvailablePowerUpsToBuy.Count; i++)
                    if (AvailablePowerUpsToBuy[(PowerUp) i])
                    {
                        actual = (PowerUp) i;
                        break;
                    }

                return actual;
            }
        }


        public PowerUp LastPowerUpToBuyAvailable
        {
            get
            {
                PowerUp actual = PowerUp.None;

                for (int i = AvailablePowerUpsToBuy.Count - 1; i > -1; i--)
                    if (AvailablePowerUpsToBuy[(PowerUp) i])
                    {
                        actual = (PowerUp) i;
                        break;
                    }

                return actual;
            }
        }


        public void SynchronizeFrom(PlayerSelection other)
        {
            this.AvailablePowerUpsToBuy.Clear();
            this.AvailableTurretOptions.Clear();
            this.AvailableTurretsToBuy.Clear();

            foreach (var kvp in other.AvailablePowerUpsToBuy)
                this.AvailablePowerUpsToBuy.Add(kvp.Key, kvp.Value);

            foreach (var kvp in other.AvailableTurretOptions)
                this.AvailableTurretOptions.Add(kvp.Key, kvp.Value);

            foreach (var kvp in other.AvailableTurretsToBuy)
                this.AvailableTurretsToBuy.Add(kvp.Key, kvp.Value);

            this.CelestialBody = other.CelestialBody;
            this.PowerUpToBuy = other.PowerUpToBuy;
            this.TurretOption = other.TurretOption;
            this.Turret = other.Turret;
            this.TurretToBuy = other.TurretToBuy;
        }


        public override bool Equals(object obj)
        {
            PlayerSelection other = obj as PlayerSelection;

            if (other == null)
                return false;

            return
                this.CelestialBody == other.CelestialBody &&
                this.PowerUpToBuy == other.PowerUpToBuy &&
                this.TurretOption == other.TurretOption &&
                this.Turret == other.Turret &&
                this.TurretToBuy == other.TurretToBuy &&
                DictEquals<PowerUp, bool>(this.AvailablePowerUpsToBuy, other.AvailablePowerUpsToBuy) &&
                DictEquals<TurretAction, bool>(this.AvailableTurretOptions, other.AvailableTurretOptions) &&
                DictEquals<Turret, bool>(this.AvailableTurretsToBuy, other.AvailableTurretsToBuy);
        }


        private bool DictEquals<K, V>(IDictionary<K, V> d1, IDictionary<K, V> d2)
        {
            if (d1.Count != d2.Count)
                return false;

            foreach (var kvp in d1)
            {
                V value;
                bool contains = d2.TryGetValue(kvp.Key, out value);

                if (!contains)
                    return false;

                if (!kvp.Value.Equals(value))
                    return false;
            }

            return true;
        }
    }
}
