namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    enum TurretAction
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


        public PlayerSelection()
        {
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
    }
}
