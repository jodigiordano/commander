namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class SimPlayerSelection
    {
        public Dictionary<TurretChoice, bool> AvailableTurretOptions;
        public Dictionary<NewGameChoice, bool> AvailableNewGameChoices;

        protected CelestialBody celestialBody;
        protected PowerUpType powerUpToBuy;
        protected Turret turret;
        protected TurretChoice turretChoice;
        protected TurretType turretToBuy;
        protected Turret turretToPlace;
        protected PausedGameChoice pausedGameChoice;
        protected EditorWorldChoice editorWorldChoice;
        protected NewGameChoice newGameChoice;
        protected EditorEditingState editingState;

        public bool CelestialBodyChanged;
        public bool PowerUpToBuyChanged;
        public bool TurretChanged;
        public bool TurretChoiceChanged;
        public bool TurretToBuyChanged;
        public bool TurretToPlaceChanged;
        public bool PausedGameChoiceChanged;
        public bool EditorWorldChoiceChanged;
        public bool NewGameChoiceChanged;
        public bool EditingStateChanged;


        public SimPlayerSelection()
        {
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

            Initialize();
        }


        public void Initialize()
        {
            CelestialBody = null;
            PowerUpToBuy = PowerUpType.None;
            Turret = null;
            TurretToBuy = TurretType.None;
            TurretChoice = TurretChoice.None;
            TurretToPlace = null;
            PausedGameChoice = PausedGameChoice.None;
            EditorWorldChoice = EditorWorldChoice.None;
            NewGameChoice = NewGameChoice.None;
            EditingState = EditorEditingState.None;

            Update();
        }


        public void Update()
        {
            CelestialBodyChanged = false;
            PowerUpToBuyChanged = false;
            TurretChanged = false;
            TurretChoiceChanged = false;
            TurretToBuyChanged = false;
            TurretToPlaceChanged = false;
            PausedGameChoiceChanged = false;
            EditorWorldChoiceChanged = false;
            NewGameChoiceChanged = false;
            EditingStateChanged = false;
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set { CelestialBodyChanged = celestialBody != value; celestialBody = value; }
        }


        public PowerUpType PowerUpToBuy
        {
            get { return powerUpToBuy; }
            set { PowerUpToBuyChanged = powerUpToBuy != value; powerUpToBuy = value; }
        }


        public Turret Turret
        {
            get { return turret; }
            set { TurretChanged = turret != value; turret = value; }
        }


        public TurretChoice TurretChoice
        {
            get { return turretChoice; }
            set { TurretChoiceChanged = turretChoice != value; turretChoice = value; }
        }


        public TurretType TurretToBuy
        {
            get { return turretToBuy; }
            set { TurretToBuyChanged = turretToBuy != value; turretToBuy = value; }
        }


        public Turret TurretToPlace
        {
            get { return turretToPlace; }
            set { TurretToPlaceChanged = turretToPlace != value; turretToPlace = value; }
        }


        public PausedGameChoice PausedGameChoice
        {
            get { return pausedGameChoice; }
            set { PausedGameChoiceChanged = pausedGameChoice != value; pausedGameChoice = value; }
        }


        public EditorWorldChoice EditorWorldChoice
        {
            get { return editorWorldChoice; }
            set { EditorWorldChoiceChanged = editorWorldChoice != value; editorWorldChoice = value; }
        }


        public NewGameChoice NewGameChoice
        {
            get { return newGameChoice; }
            set { NewGameChoiceChanged = newGameChoice != value; newGameChoice = value; }
        }


        public EditorEditingState EditingState
        {
            get { return editingState; }
            set { EditingStateChanged = editingState != value; editingState = value; }
        }


        public void Sync(SimPlayerSelection other)
        {
            celestialBody = other.celestialBody;
            powerUpToBuy = other.powerUpToBuy;
            turret = other.turret;
            turretChoice = other.turretChoice;
            turretToBuy = other.turretToBuy;
            turretToPlace = other.turretToPlace;
            pausedGameChoice = other.pausedGameChoice;
            editorWorldChoice = other.editorWorldChoice;
            newGameChoice = other.newGameChoice;
            editingState = other.editingState;
        }
    }
}
