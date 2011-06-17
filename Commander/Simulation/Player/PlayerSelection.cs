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
        public Dictionary<PowerUpType, bool> AvailablePowerUpsToBuy;

        public Turret Turret;
        public TurretAction TurretOption;
        public Dictionary<TurretAction, bool> AvailableTurretOptions;

        public Turret TurretToBuy;
        public Dictionary<TurretType, bool> AvailableTurrets;

        public Turret TurretToPlace;

        public GameAction GameAction;


        public PlayerSelection(Simulator simulation)
        {
            AvailableTurrets = new Dictionary<TurretType, bool>(TurretTypeComparer.Default);

            AvailablePowerUpsToBuy = new Dictionary<PowerUpType, bool>(PowerUpTypeComparer.Default);

            foreach (var powerUp in simulation.PowerUpsFactory.Availables.Keys)
                AvailablePowerUpsToBuy.Add(powerUp, false);

            AvailableTurretOptions = new Dictionary<TurretAction, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretAction.Sell, false);
            AvailableTurretOptions.Add(TurretAction.Update, false);

            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
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


        public void SynchronizeFrom(PlayerSelection other)
        {
            this.AvailablePowerUpsToBuy.Clear();
            this.AvailableTurretOptions.Clear();

            foreach (var kvp in other.AvailablePowerUpsToBuy)
                this.AvailablePowerUpsToBuy.Add(kvp.Key, kvp.Value);

            foreach (var kvp in other.AvailableTurretOptions)
                this.AvailableTurretOptions.Add(kvp.Key, kvp.Value);

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
                DictEquals<PowerUpType, bool>(this.AvailablePowerUpsToBuy, other.AvailablePowerUpsToBuy) &&
                DictEquals<TurretAction, bool>(this.AvailableTurretOptions, other.AvailableTurretOptions);
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
