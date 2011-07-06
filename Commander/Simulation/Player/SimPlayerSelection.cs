﻿namespace EphemereGames.Commander.Simulation
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

        public PausedGameChoice GameChoice;

        public EditorEditingState EditingState;


        public SimPlayerSelection()
        {
            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
            Turret = null;
            TurretToBuy = TurretType.None;
            TurretChoice = TurretChoice.None;
            TurretToPlace = null;
            GameChoice = PausedGameChoice.None;
            EditingState = EditorEditingState.None;

            AvailableTurretOptions = new Dictionary<TurretChoice, bool>(TurretActionComparer.Default);
            AvailableTurretOptions.Add(TurretChoice.Sell, false);
            AvailableTurretOptions.Add(TurretChoice.Update, false);
        }
    }
}