namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class SimPlayerSelection
    {
        public CelestialBody CelestialBody;

        public PowerUpType PowerUpToBuy;

        public Turret Turret;
        public TurretChoice TurretChoice;
        public Dictionary<TurretChoice, bool> AvailableTurretOptions;

        public TurretType TurretToBuy;
        public Turret TurretToPlace;

        public PausedGameChoice PausedGameChoice;

        public NewGameChoice NewGameChoice;
        public Dictionary<NewGameChoice, bool> AvailableNewGameChoices;

        public EditorEditingState EditingState;


        public SimPlayerSelection()
        {
            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
            Turret = null;
            TurretToBuy = TurretType.None;
            TurretChoice = TurretChoice.None;
            TurretToPlace = null;
            PausedGameChoice = PausedGameChoice.None;
            NewGameChoice = NewGameChoice.None;
            EditingState = EditorEditingState.None;

            AvailableTurretOptions = new Dictionary<TurretChoice, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretChoice.Sell, false);
            AvailableTurretOptions.Add(TurretChoice.Update, false);

            AvailableNewGameChoices = new Dictionary<NewGameChoice, bool>(NewGameChoiceComparer.Default);
            AvailableNewGameChoices.Add(NewGameChoice.Continue, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld1, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld2, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld3, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld4, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld5, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld6, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld7, false);
            AvailableNewGameChoices.Add(NewGameChoice.WrapToWorld8, false);
            AvailableNewGameChoices.Add(NewGameChoice.NewGame, true);
        }
    }
}
