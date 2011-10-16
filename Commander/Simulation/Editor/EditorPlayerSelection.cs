namespace EphemereGames.Commander.Simulation
{
    class EditorPlayerSelection
    {
        protected EditorGeneralMenuChoice generalMenuChoice;
        protected int generalMenuSubMenuIndex;
        protected int celestialBodyChoice;
        protected CelestialBody celestialBody;

        public bool GeneralMenuChoiceChanged;
        public bool GeneralMenuSubMenuIndexChanged;
        public bool CelestialBodyChoiceChanged;
        public bool CelestialBodyChanged;


        public EditorPlayerSelection()
        {
            Initialize();
        }


        public void Initialize()
        {
            GeneralMenuChoice = EditorGeneralMenuChoice.None;
            GeneralMenuSubMenuIndex = -1;
            CelestialBodyChoice = -1;
            CelestialBody = null;

            Update();
        }


        public void Update()
        {
            GeneralMenuChoiceChanged = false;
            GeneralMenuSubMenuIndexChanged = false;
            CelestialBodyChoiceChanged = false;
            CelestialBodyChanged = false;
        }

        
        public EditorGeneralMenuChoice GeneralMenuChoice
        {
            get { return generalMenuChoice; }
            set { GeneralMenuChoiceChanged = generalMenuChoice != value; generalMenuChoice = value; }
        }


        public int GeneralMenuSubMenuIndex
        {
            get { return generalMenuSubMenuIndex; }
            set { GeneralMenuSubMenuIndexChanged = generalMenuSubMenuIndex != value; generalMenuSubMenuIndex = value; }
        }


        public int CelestialBodyChoice
        {
            get { return celestialBodyChoice; }
            set { CelestialBodyChoiceChanged = celestialBodyChoice != value; celestialBodyChoice = value; }
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set { CelestialBodyChanged = celestialBody != value; celestialBody = value; }
        }


        public void Sync(EditorPlayerSelection other)
        {
            generalMenuChoice = other.generalMenuChoice;
            generalMenuSubMenuIndex = other.generalMenuSubMenuIndex;
            celestialBodyChoice = other.celestialBodyChoice;
            celestialBody = other.celestialBody;
        }
    }
}
