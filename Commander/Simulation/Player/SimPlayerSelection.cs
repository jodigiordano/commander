namespace EphemereGames.Commander.Simulation
{
    class SimPlayerSelection
    {
        protected CelestialBody celestialBody;
        protected Turret turret;
        protected ContextualMenu openedMenu;

        // in-game
        protected PowerUpType powerUpToBuy;
        protected TurretChoice turretChoice;
        protected TurretType turretToBuy;
        protected Turret turretToPlace;

        // world
        protected PauseChoice pausedGameChoice;

        // main menu
        protected int newGameChoice;

        // editor
        protected EditorCommand editorWorldLevelCommand;
        protected EditorCommand editorWorldCommand;
        protected EditorCommand editorBuildMenuCommand;
        protected EditorCommand editorCelestialBodyMenuCommand;
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
        public bool OpenedMenuChanged;
        public bool EditorWorldLevelCommandChanged;
        public bool EditorWorldCommandChanged;
        public bool EditorBuildMenuCommandChanged;
        public bool EditorCelestialBodyMenuCommandChanged;


        public SimPlayerSelection()
        {
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
            PausedGameChoice = PauseChoice.None;
            NewGameChoice = -1;
            EditingState = EditorEditingState.None;
            OpenedMenu = null;
            EditorWorldLevelCommand = null;
            EditorWorldCommand = null;
            EditorBuildMenuCommand = null;
            EditorCelestialBodyMenuCommand = null;

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
            OpenedMenuChanged = false;
            EditorWorldLevelCommandChanged = false;
            EditorWorldCommandChanged = false;
            EditorBuildMenuCommandChanged = false;
            EditorCelestialBodyMenuCommandChanged = false;
        }


        public EditorCommand EditorBuildMenuCommand
        {
            get { return editorBuildMenuCommand; }
            set { EditorBuildMenuCommandChanged = editorBuildMenuCommand != value; editorBuildMenuCommand = value; }
        }


        public EditorCommand EditorCelestialBodyMenuCommand
        {
            get { return editorCelestialBodyMenuCommand; }
            set { EditorCelestialBodyMenuCommandChanged = editorCelestialBodyMenuCommand != value; editorCelestialBodyMenuCommand = value; }
        }


        public EditorCommand EditorWorldLevelCommand
        {
            get { return editorWorldLevelCommand; }
            set { EditorWorldLevelCommandChanged = editorWorldLevelCommand != value; editorWorldLevelCommand = value; }
        }


        public EditorCommand EditorWorldCommand
        {
            get { return editorWorldCommand; }
            set { EditorWorldCommandChanged = editorWorldCommand != value; editorWorldCommand = value; }
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


        public ContextualMenu OpenedMenu
        {
            get { return openedMenu; }
            set { OpenedMenuChanged = openedMenu != value; openedMenu = value; }
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


        public PauseChoice PausedGameChoice
        {
            get { return pausedGameChoice; }
            set { PausedGameChoiceChanged = pausedGameChoice != value; pausedGameChoice = value; }
        }


        public int NewGameChoice
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
            newGameChoice = other.newGameChoice;
            editingState = other.editingState;
            editorWorldCommand = other.editorWorldCommand;
            editorWorldLevelCommand = other.editorWorldLevelCommand;
            editorBuildMenuCommand = other.editorBuildMenuCommand;
            editorCelestialBodyMenuCommand = other.editorCelestialBodyMenuCommand;
            openedMenu = other.openedMenu;
        }
    }
}
