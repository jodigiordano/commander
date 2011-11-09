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
        protected EditorWorldChoice editorWorldChoice;
        protected EditorEditingState editingState;
        protected int editorGeneralMenuChoice;
        protected int editorCBChoice;

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
            EditorWorldChoice = EditorWorldChoice.None;
            NewGameChoice = -1;
            EditingState = EditorEditingState.None;
            OpenedMenu = null;

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


        public EditorWorldChoice EditorWorldChoice
        {
            get { return editorWorldChoice; }
            set { EditorWorldChoiceChanged = editorWorldChoice != value; editorWorldChoice = value; }
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
            editorWorldChoice = other.editorWorldChoice;
            newGameChoice = other.newGameChoice;
            editingState = other.editingState;
        }
    }
}
