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
    }
}
