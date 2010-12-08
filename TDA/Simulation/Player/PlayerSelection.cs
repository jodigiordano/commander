namespace TDA
{
    using System.Collections.Generic;


    public enum PowerUp
    {
        Aucune = -1,
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


    class PlayerSelection
    {
        public CorpsCeleste CelestialBody;
        public PowerUp CelestialBodyOption;
        public Dictionary<PowerUp, bool> AvailableCelestialBodyOptions;

        public Emplacement TurretSpot;
        public TurretAction TurretOption;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;

        public Tourelle TurretToBuy;
        public Dictionary<Tourelle, bool> AvailableTurretsToBuy;


        public PlayerSelection()
        {
            CelestialBody = null;
            CelestialBodyOption = PowerUp.Aucune;

            AvailableCelestialBodyOptions = new Dictionary<PowerUp, bool>();
            AvailableCelestialBodyOptions.Add(PowerUp.CollectTheRent, false);
            AvailableCelestialBodyOptions.Add(PowerUp.DoItYourself, false);
            AvailableCelestialBodyOptions.Add(PowerUp.FinalSolution, false);
            AvailableCelestialBodyOptions.Add(PowerUp.TheResistance, false);

            TurretSpot = null;
            TurretToBuy = null;
            TurretOption = TurretAction.None;

            AvailableTurretOptions = new Dictionary<TurretAction, bool>();
            AvailableTurretOptions.Add(TurretAction.Sell, false);
            AvailableTurretOptions.Add(TurretAction.Update, false);

            AvailableTurretsToBuy = new Dictionary<Tourelle, bool>();
        }


        public Tourelle Turret
        {
            get { return (TurretSpot != null && TurretSpot.EstOccupe) ? TurretSpot.Tourelle : null; }
        }


        public void NextTurretOption()
        {
            int actual = (int) TurretOption;
            int nbChoices = AvailableTurretOptions.Count;
            int next = -1;

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
            int previous = -1;

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


        public void NextCelestialBodyOption()
        {
            PowerUp actual = CelestialBodyOption;

            int nbChoices = AvailableCelestialBodyOptions.Count;
            int actualInt = (int) actual;
            int next = -1;

            for (int i = 1; i < nbChoices; i++)
            {
                actualInt += 1;

                if (actualInt >= nbChoices)
                    actualInt = 0;

                if (AvailableCelestialBodyOptions[(PowerUp)actualInt])
                {
                    next = actualInt;
                    break;
                }
            }

            CelestialBodyOption = (PowerUp) next;
        }


        public void PreviousCelestialBodyOption()
        {
            PowerUp actual = CelestialBodyOption;

            int nbChoices = AvailableCelestialBodyOptions.Count;
            int actualInt = (int) actual;
            int previous = -1;

            for (int i = 1; i < nbChoices; i++)
            {
                actualInt -= 1;

                if (actualInt < 0)
                    actualInt = nbChoices - 1;

                if (AvailableCelestialBodyOptions[(PowerUp)actualInt])
                {
                    previous = actualInt;
                    break;
                }
            }

            CelestialBodyOption = (PowerUp) previous;
        }


        public void SynchronizeFrom(PlayerSelection other)
        {
            this.AvailableCelestialBodyOptions.Clear();
            this.AvailableTurretOptions.Clear();
            this.AvailableTurretsToBuy.Clear();

            foreach (var kvp in other.AvailableCelestialBodyOptions)
                this.AvailableCelestialBodyOptions.Add(kvp.Key, kvp.Value);

            foreach (var kvp in other.AvailableTurretOptions)
                this.AvailableTurretOptions.Add(kvp.Key, kvp.Value);

            foreach (var kvp in other.AvailableTurretsToBuy)
                this.AvailableTurretsToBuy.Add(kvp.Key, kvp.Value);

            this.CelestialBody = other.CelestialBody;
            this.CelestialBodyOption = other.CelestialBodyOption;
            this.TurretOption = other.TurretOption;
            this.TurretSpot = other.TurretSpot;
            this.TurretToBuy = other.TurretToBuy;
        }


        public override bool Equals(object obj)
        {
            PlayerSelection other = obj as PlayerSelection;

            if (other == null)
                return false;

            return
                this.CelestialBody == other.CelestialBody &&
                this.CelestialBodyOption == other.CelestialBodyOption &&
                this.TurretOption == other.TurretOption &&
                this.TurretSpot == other.TurretSpot &&
                this.TurretToBuy == other.TurretToBuy &&
                DictEquals<PowerUp, bool>(this.AvailableCelestialBodyOptions, other.AvailableCelestialBodyOptions) &&
                DictEquals<TurretAction, bool>(this.AvailableTurretOptions, other.AvailableTurretOptions) &&
                DictEquals<Tourelle, bool>(this.AvailableTurretsToBuy, other.AvailableTurretsToBuy);
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
