namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    enum TurretChoice
    {
        None = -1,
        Sell = 0,
        Update = 1
    }


    public enum PausedGameChoice
    {
        None = -1,
        Resume = 0,
        New = 1
    }


    class SimPlayerSelection
    {
        public CelestialBody CelestialBody;

        public PowerUpType PowerUpToBuy;

        public Turret Turret;
        public TurretChoice TurretChoice;
        public Dictionary<TurretChoice, bool> AvailableTurretOptions;

        public TurretType TurretToBuy;
        public Turret TurretToPlace;

        public PausedGameChoice GameChoice;


        public SimPlayerSelection()
        {
            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
            Turret = null;
            TurretToBuy = TurretType.None;
            TurretChoice = TurretChoice.None;
            TurretToPlace = null;
            GameChoice = PausedGameChoice.None;

            AvailableTurretOptions = new Dictionary<TurretChoice, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretChoice.Sell, false);
            AvailableTurretOptions.Add(TurretChoice.Update, false);
        }
    }
}
