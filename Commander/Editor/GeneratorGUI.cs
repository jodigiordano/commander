namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public struct GeneratorGUI
    {
        // Système planétaire
        public int NbCorpsCelestes;
        public int NbCorpsCelestesFixes;
        public bool AvecEtoileAuMilieu;
        public int NbPlanetesCheminDeDepart;
        public int NbEmplacements;
        public bool SystemeCentre;

        // Gameplay
        public List<TurretType> TourellesDisponibles;
        public List<PowerUpType> PowerUpsDisponibles;        
        public int ViesPlaneteAProteger;
        public int ArgentExtra;
        public int NbPacksVie;
        public int ArgentDepart;

        // Vagues
        public int DifficulteDebut;
        public int DifficulteFin;
        public int QteEnnemis;
        public List<EnemyType> EnnemisPresents;
        public int NbVagues;
        public int ArgentEnnemis;
    }


    class GenerateurGUI
    {
        public bool Visible;

        private GeneratorGUI DonneesGenerateur;
        private ScenarioDescriptor DescriptionScenario;

        private Simulation Simulation;
        private ScenarioGenerator GenerateurScenario;
        private WaveGenerator GenerateurVagues;
        private Vector3 Position;


        #region Systeme Planetaire
        private Image FiltreSystemePlanetaire;
        private Text SystemePlanetaire;

        private Text NbPlanetesRep;
        private HorizontalSlider NbPlanetesSlider;

        private Text NbEmplacementsRep;
        private HorizontalSlider NbEmplacementsSlider;

        private Text NbPlanetesFixesRep;
        private HorizontalSlider NbPlanetesFixesSlider;

        private Text NbPlanetesCheminDepartRep;
        private HorizontalSlider NbPlanetesCheminDepartSlider;

        private Text EtoileRep;
        private CheckBox EtoileCheckBox;

        private Text SystemeCentreRep;
        private CheckBox SystemeCentreCheckBox;

        private Text GenererSystemePlanetaireRep;
        private PushButton GenererSystemePlanetairePushButton;

        #endregion

        #region Gameplay elements

        private Image FiltreGameplay;
        private Text Gameplay;

        private Text NbViesRep;
        private HorizontalSlider NbViesSlider;

        private Text ValeurMinerauxRep;
        private HorizontalSlider ValeurMinerauxSlider;

        private Text NbPacksVieRep;
        private HorizontalSlider NbPacksVieSlider;

        private Text ReserveDepartRep;
        private HorizontalSlider ReserveDepartSlider;

        private Dictionary<TurretType, Image> TourellesRep;
        private Dictionary<TurretType, CheckBox> TourellesCheckBoxes;

        private Dictionary<PowerUpType, Image> PowerUpsRep;
        private Dictionary<PowerUpType, CheckBox> PowerUpsCheckBoxes;

        private Text GenererGameplayRep;
        private PushButton GenererGameplayPushButton;

        #endregion

        #region Waves

        private Image FiltreWaves;
        private Text Waves;

        private Text DifficulteDebutRep;
        private HorizontalSlider DifficulteDebutSlider;

        private Text DifficulteFinRep;
        private HorizontalSlider DifficulteFinSlider;

        private Text QteEnnemisRep;
        private HorizontalSlider QteEnnemisSlider;

        private Text NbVaguesRep;
        private HorizontalSlider NbVaguesSlider;

        private Text ReserveEnnemisRep;
        private HorizontalSlider ReserveEnnemisSlider;

        private Dictionary<EnemyType, Image> EnnemisDisponiblesRep;
        private Dictionary<EnemyType, CheckBox> EnnemisDisponiblesCheckBoxes;

        private Text GenererVaguesRep;
        private PushButton GenererVaguesPushButton;

        #endregion

        #region Editeur

        private Image FiltreEditeur;
        private Text Editeur;

        private Text GenererRep;
        private PushButton GenererPushButton;

        private Text LoadRep;
        private HorizontalSlider LoadSlider;
        private PushButton LoadPushButton;

        private Text DeleteRep;
        private HorizontalSlider DeleteSlider;
        private PushButton DeletePushButton;

        private Text RestartRep;
        private PushButton RestartPushButton;

        private Text SaveRep;
        private PushButton SavePushButton;

        #endregion

        public GenerateurGUI(Simulation simulation, Cursor curseur, Vector3 position)
        {
            Simulation = simulation;
            Position = position;
            Visible = true;

            #region Systeme Planetaire

            FiltreSystemePlanetaire = new Image("PixelBlanc", Position - new Vector3(0, 90, 0));
            FiltreSystemePlanetaire.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreSystemePlanetaire.Size = new Vector2(540, 320);
            FiltreSystemePlanetaire.Color = new Color(255, 0, 220, 128);

            SystemePlanetaire = new Text("Planetary System", "Pixelite", Color.White, Position - new Vector3(0, 250, 0));
            SystemePlanetaire.SizeX = 4;
            SystemePlanetaire.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            SystemePlanetaire.Origin = SystemePlanetaire.Center;


            NbPlanetesRep = new Text("Planets", "Pixelite", Color.White, Position - new Vector3(250, 220, 0));
            NbPlanetesRep.SizeX = 2;
            NbPlanetesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesSlider = new HorizontalSlider(Simulation.Scene, curseur, Position - new Vector3(-150, 210, 0), 1, 15, 3, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbPlanetesFixesRep = new Text("Fixed planets", "Pixelite", Color.White, Position - new Vector3(250, 180, 0));
            NbPlanetesFixesRep.SizeX = 2;
            NbPlanetesFixesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesFixesSlider = new HorizontalSlider(Simulation.Scene, curseur, Position - new Vector3(-150, 170, 0), 0, 15, 0, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbEmplacementsRep = new Text("Slots", "Pixelite", Color.White, Position - new Vector3(250, 140, 0));
            NbEmplacementsRep.SizeX = 2;
            NbEmplacementsRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbEmplacementsSlider = new HorizontalSlider(Simulation.Scene, curseur, Position - new Vector3(-150, 130, 0), 1, 50, 10, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            NbPlanetesCheminDepartRep = new Text("Path length", "Pixelite", Color.White, Position - new Vector3(250, 100, 0));
            NbPlanetesCheminDepartRep.SizeX = 2;
            NbPlanetesCheminDepartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesCheminDepartSlider = new HorizontalSlider(Simulation.Scene, curseur, Position - new Vector3(-150, 90, 0), 0, 15, 3, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            EtoileRep = new Text("Star in the middle?", "Pixelite", Color.White, Position - new Vector3(250, 60, 0));
            EtoileRep.SizeX = 2;
            EtoileRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            EtoileCheckBox = new CheckBox(Simulation.Scene, curseur, Position - new Vector3(-200, 50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            SystemeCentreRep = new Text("Centre?", "Pixelite", Color.White, Position - new Vector3(250, 20, 0));
            SystemeCentreRep.SizeX = 2;
            SystemeCentreRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            SystemeCentreCheckBox = new CheckBox(Simulation.Scene, curseur, Position - new Vector3(-200, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            GenererSystemePlanetaireRep = new Text("Generate", "Pixelite", Color.White, Position - new Vector3(-90, -40, 0));
            GenererSystemePlanetaireRep.SizeX = 2;
            GenererSystemePlanetaireRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererSystemePlanetairePushButton = new PushButton(Simulation.Scene, curseur, Position - new Vector3(-240, -50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            #endregion

            #region Gameplay Elements

            FiltreGameplay = new Image("PixelBlanc", Position + new Vector3(600, -90, 0));
            FiltreGameplay.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreGameplay.Size = new Vector2(540, 320);
            FiltreGameplay.Color = new Color(76, 255, 0, 128);
            FiltreGameplay.Origin = FiltreSystemePlanetaire.Center;

            Gameplay = new Text("Gameplay", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(0, 160, 0));
            Gameplay.SizeX = 4;
            Gameplay.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Gameplay.Origin = Gameplay.Center;

            ReserveDepartRep = new Text("Minerals", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(250, 120, 0));
            ReserveDepartRep.SizeX = 2;
            ReserveDepartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ReserveDepartSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 110, 0), 0, 10000, 0, 100, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbViesRep = new Text("Lives", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(250, 80, 0));
            NbViesRep.SizeX = 2;
            NbViesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbViesSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 70, 0), 1, 50, 5, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            ValeurMinerauxRep = new Text("Enemies minerals", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(250, 40, 0));
            ValeurMinerauxRep.SizeX = 2;
            ValeurMinerauxRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ValeurMinerauxSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 30, 0), 0, 5000, 250, 250, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbPacksVieRep = new Text("Life packs", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(250, 0, 0));
            NbPacksVieRep.SizeX = 2;
            NbPacksVieRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPacksVieSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, -10, 0), 0, 20, 5, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            TourellesRep = new Dictionary<TurretType, Image>();
            TourellesCheckBoxes = new Dictionary<TurretType, CheckBox>();

            Vector3 positionTourelle = FiltreGameplay.Position - new Vector3(230, -60, 0);

            foreach (var tourelle in simulation.TurretsFactory.Availables)
            {
                Image iv = tourelle.Value.BaseImage.Clone();
                iv.Position = positionTourelle;
                iv.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

                TourellesRep.Add(tourelle.Key, iv);
                TourellesCheckBoxes.Add(tourelle.Key, new CheckBox(Simulation.Scene, curseur, iv.Position + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));

                TourellesCheckBoxes[tourelle.Key].Checked = true;

                positionTourelle += new Vector3(85, 0, 0);
            }


            Vector3 positionPowerUp = FiltreGameplay.Position - new Vector3(230, -120, 0);

            PowerUpsRep = new Dictionary<PowerUpType, Image>();
            PowerUpsCheckBoxes = new Dictionary<PowerUpType, CheckBox>();

            PowerUpsRep.Add(PowerUpType.Spaceship, new Image("Vaisseau", positionPowerUp));
            PowerUpsRep[PowerUpType.Spaceship].SizeX = 4;
            PowerUpsRep[PowerUpType.Spaceship].Origin = PowerUpsRep[PowerUpType.Spaceship].Center;
            PowerUpsRep[PowerUpType.Spaceship].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUpType.Spaceship, new CheckBox(Simulation.Scene, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUpType.Spaceship].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUpType.Collector, new Image("Collecteur", positionPowerUp));
            PowerUpsRep[PowerUpType.Collector].SizeX = 4;
            PowerUpsRep[PowerUpType.Collector].Origin = PowerUpsRep[PowerUpType.Collector].Center;
            PowerUpsRep[PowerUpType.Collector].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUpType.Collector, new CheckBox(Simulation.Scene, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUpType.Collector].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUpType.FinalSolution, new Image("Destruction", positionPowerUp));
            PowerUpsRep[PowerUpType.FinalSolution].SizeX = 4;
            PowerUpsRep[PowerUpType.FinalSolution].Origin = PowerUpsRep[PowerUpType.FinalSolution].Center;
            PowerUpsRep[PowerUpType.FinalSolution].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUpType.FinalSolution, new CheckBox(Simulation.Scene, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUpType.FinalSolution].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUpType.TheResistance, new Image("TheResistance", positionPowerUp));
            PowerUpsRep[PowerUpType.TheResistance].SizeX = 4;
            PowerUpsRep[PowerUpType.TheResistance].Origin = PowerUpsRep[PowerUpType.TheResistance].Center;
            PowerUpsRep[PowerUpType.TheResistance].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUpType.TheResistance, new CheckBox(Simulation.Scene, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUpType.TheResistance].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            GenererGameplayRep = new Text("Generate", "Pixelite", Color.White, FiltreGameplay.Position - new Vector3(-90, -130, 0));
            GenererGameplayRep.SizeX = 2;
            GenererGameplayRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererGameplayPushButton = new PushButton(Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-240, -140, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            #endregion

            #region Waves

            FiltreWaves = new Image("PixelBlanc", Position + new Vector3(300, 175, 0));
            FiltreWaves.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreWaves.Size = new Vector2(1137, 140);
            FiltreWaves.Color = new Color(255, 0, 0, 128);
            FiltreWaves.Origin = FiltreWaves.Center;


            Waves = new Text("Waves", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(0, 70, 0));
            Waves.SizeX = 4;
            Waves.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Waves.Origin = Waves.Center;


            NbVaguesRep = new Text("Waves", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(550, 50, 0));
            NbVaguesRep.SizeX = 2;
            NbVaguesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbVaguesSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, 40, 0), 1, 30, 1, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            QteEnnemisRep = new Text("Quantity", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(550, 10, 0));
            QteEnnemisRep.SizeX = 2;
            QteEnnemisRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            QteEnnemisSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, 0, 0), 20, 1000, 20, 20, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            ReserveEnnemisRep = new Text("Money", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(550, -30, 0));
            ReserveEnnemisRep.SizeX = 2;
            ReserveEnnemisRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ReserveEnnemisSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, -40, 0), 0, 10000, 0, 250, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            DifficulteDebutRep = new Text("Diff. start", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(200, 30, 0));
            DifficulteDebutRep.SizeX = 2;
            DifficulteDebutRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DifficulteDebutSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-100, 20, 0), 2, 100, 2, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            DifficulteFinRep = new Text("Diff. end", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(-200, 30, 0));
            DifficulteFinRep.SizeX = 2;
            DifficulteFinRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DifficulteFinSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-450, 20, 0), 2, 100, 2, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

 
            EnnemisDisponiblesRep = new Dictionary<EnemyType, Image>();
            EnnemisDisponiblesCheckBoxes = new Dictionary<EnemyType, CheckBox>();

            Vector3 positionEnnemis = FiltreWaves.Position - new Vector3(190, -30, 0);

            //foreach (var ennemi in Simulation.EnemiesFactory.AvailableEnemies)
            //{
            //    ennemi.Initialize();
            //    Image iv = (Image)ennemi.Image.Clone();
            //    iv.Position = positionEnnemis;
            //    iv.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            //    EnnemisDisponiblesRep.Add(ennemi.Type, iv);
            //    EnnemisDisponiblesCheckBoxes.Add(ennemi.Type, new CheckBox(Simulation.Scene, curseur, iv.Position + new Vector3(45, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));

            //    EnnemisDisponiblesCheckBoxes[ennemi.Type].Checked = true;

            //    positionEnnemis += new Vector3(100, 0, 0);
            //}


            GenererVaguesRep = new Text("Generate", "Pixelite", Color.White, FiltreWaves.Position - new Vector3(-390, -40, 0));
            GenererVaguesRep.SizeX = 2;
            GenererVaguesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererVaguesPushButton = new PushButton(Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-540, -50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            #endregion

            #region Editeur

            FiltreEditeur = new Image("PixelBlanc", Position + new Vector3(300, -340, 0));
            FiltreEditeur.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreEditeur.Size = new Vector2(740, 120);
            FiltreEditeur.Color = new Color(0, 148, 255, 128);
            FiltreEditeur.Origin = FiltreEditeur.Center;

            Editeur = new Text("Editor", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(0, 60, 0));
            Editeur.SizeX = 4;
            Editeur.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Editeur.Origin = Editeur.Center;


            LoadRep = new Text("Load", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(350, 20, 0));
            LoadRep.SizeX = 2;
            LoadRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            LoadSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(150, 10, 0), -1, 1000, 0, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);
            LoadPushButton = new PushButton(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(50, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            DeleteRep = new Text("Delete", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(350, -20, 0));
            DeleteRep.SizeX = 2;
            DeleteRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DeleteSlider = new HorizontalSlider(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(150, -30, 0), -1, 1000, -1, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);
            DeletePushButton = new PushButton(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(50, -30, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            GenererRep = new Text("Generate", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(-175, 20, 0));
            GenererRep.SizeX = 2;
            GenererRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererPushButton = new PushButton(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-330, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            RestartRep = new Text("Restart", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(0, 20, 0));
            RestartRep.SizeX = 2;
            RestartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            RestartPushButton = new PushButton(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-150, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            SaveRep = new Text("Save", "Pixelite", Color.White, FiltreEditeur.Position - new Vector3(0, -20, 0));
            SaveRep.SizeX = 2;
            SaveRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            SavePushButton = new PushButton(Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-150, -30, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            #endregion

            GenerateurScenario = new ScenarioGenerator();
            GenerateurVagues = new WaveGenerator();
        }


        public void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            NbPlanetesFixesSlider.Valeur = (int)MathHelper.Clamp(NbPlanetesFixesSlider.Valeur, NbPlanetesFixesSlider.Min, NbPlanetesSlider.Valeur);
            NbPlanetesCheminDepartSlider.Valeur = (int)MathHelper.Clamp(NbPlanetesCheminDepartSlider.Valeur, 0, NbPlanetesSlider.Valeur);
            DifficulteFinSlider.Valeur = (int)MathHelper.Clamp(DifficulteFinSlider.Valeur, DifficulteDebutSlider.Valeur, DifficulteFinSlider.Max);
            //LoadSlider.Valeur = (int)MathHelper.Clamp(LoadSlider.Valeur, -1, Main.SaveGame.DescriptionsScenarios.Count);
            //DeleteSlider.Valeur = (int)MathHelper.Clamp(DeleteSlider.Valeur, -1, Main.SaveGame.DescriptionsScenarios.Count);

            if (GenererPushButton.Pressed)
            {
                updateVagues();
                updateSystemePlanetaire();
                updateGameplay();

                GenerateurVagues.Generate();
                GenerateurScenario.genererSystemePlanetaire();
                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Waves;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            if (GenererSystemePlanetairePushButton.Pressed)
            {
                updateSystemePlanetaire();

                if (GenerateurVagues.Waves == null)
                {
                    updateVagues();
                    updateGameplay();

                    GenerateurVagues.Generate();
                }

                GenerateurScenario.genererSystemePlanetaire();
                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Waves;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            if (GenererVaguesPushButton.Pressed)
            {
                updateVagues();

                if (GenerateurScenario.DescripteurScenario == null)
                {
                    updateSystemePlanetaire();
                    updateGameplay();

                    GenerateurScenario.genererSystemePlanetaire();
                    GenerateurScenario.genererGameplay();
                }

                GenerateurVagues.Generate();
                GenerateurScenario.genererCeintureAsteroides();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Waves;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            if (GenererGameplayPushButton.Pressed)
            {
                updateGameplay();

                if (GenerateurScenario.DescripteurScenario == null)
                {
                    updateSystemePlanetaire();
                    updateVagues();

                    GenerateurVagues.Generate();
                    GenerateurScenario.genererSystemePlanetaire();
                }

                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Waves;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }


            if (RestartPushButton.Pressed && DescriptionScenario != null)
            {
                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            //if (LoadPushButton.Pressed && LoadSlider.Valeur != -1 && LoadSlider.Valeur < Main.SaveGame.DescriptionsScenarios.Count)
            //{
            //    DescriptionScenario = Main.SaveGame.DescriptionsScenarios[LoadSlider.Valeur];
            //    DonneesGenerateur = Main.SaveGame.DonneesGenerateur[LoadSlider.Valeur];

            //    GenerateurScenario.DescripteurScenario = DescriptionScenario;
            //    GenerateurVagues.Vagues = DescriptionScenario.Vagues;

            //    updateGUI();

            //    Simulation.DescriptionScenario = DescriptionScenario;
            //    Simulation.Initialize();
            //}

            if ( SavePushButton.Pressed && DescriptionScenario != null )
            {
                updateDonneesSystemePlanetaire();
                updateDonneesGameplay();
                updateDonneesVagues();

                if ( LoadSlider.Valeur == -1 || LoadSlider.Valeur >= Main.GeneratorData.Scenarios.Count )
                {
                    Main.GeneratorData.Scenarios.Add( DescriptionScenario );
                }

                else
                {
                    Main.GeneratorData.Scenarios[LoadSlider.Valeur] = DescriptionScenario;
                }

                Persistence.SaveData( "generateurData" );
            }

            //if (DeletePushButton.Pressed && DeleteSlider.Valeur != -1 && DeleteSlider.Valeur < Main.SaveGame.DescriptionsScenarios.Count)
            //{
            //    Main.SaveGame.DescriptionsScenarios.RemoveAt(DeleteSlider.Valeur);
            //    Main.SaveGame.DonneesGenerateur.RemoveAt(DeleteSlider.Valeur);

            //    DeleteSlider.Valeur = -1;

            //    EphemereGames.Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
            //}

            GenererPushButton.Update(gameTime);
            LoadPushButton.Update(gameTime);
            DeletePushButton.Update(gameTime);
            RestartPushButton.Update(gameTime);
            SavePushButton.Update(gameTime);
            GenererSystemePlanetairePushButton.Update(gameTime);
            GenererVaguesPushButton.Update(gameTime);
            GenererGameplayPushButton.Update(gameTime);
        }


        public void DoClick()
        {
            NbPlanetesSlider.doClick();
            NbEmplacementsSlider.doClick();
            NbPlanetesFixesSlider.doClick();
            NbPlanetesCheminDepartSlider.doClick();
            NbViesSlider.doClick();
            ValeurMinerauxSlider.doClick();
            NbPacksVieSlider.doClick();
            EtoileCheckBox.doClick();
            ReserveDepartSlider.doClick();
            LoadSlider.doClick();
            DeleteSlider.doClick();
            NbVaguesSlider.doClick();
            QteEnnemisSlider.doClick();
            DifficulteDebutSlider.doClick();
            DifficulteFinSlider.doClick();
            ReserveEnnemisSlider.doClick();
            SystemeCentreCheckBox.doClick();
            GenererPushButton.doClick();
            LoadPushButton.doClick();
            DeletePushButton.doClick();
            RestartPushButton.doClick();
            SavePushButton.doClick();
            GenererSystemePlanetairePushButton.doClick();
            GenererVaguesPushButton.doClick();
            GenererGameplayPushButton.doClick();

            foreach (var tourelle in TourellesCheckBoxes)
                tourelle.Value.doClick();

            foreach (var powerUp in PowerUpsCheckBoxes)
                powerUp.Value.doClick();

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                ennemi.Value.doClick();
        }


        public void Draw()
        {
            if (!Visible)
                return;

            Simulation.Scene.Add(FiltreSystemePlanetaire);
            Simulation.Scene.Add(SystemePlanetaire);
            Simulation.Scene.Add(NbPlanetesRep);
            Simulation.Scene.Add(NbEmplacementsRep);
            Simulation.Scene.Add(NbPlanetesFixesRep);

            Simulation.Scene.Add(NbPlanetesCheminDepartRep);
            Simulation.Scene.Add(NbViesRep);
            Simulation.Scene.Add(ValeurMinerauxRep);
            Simulation.Scene.Add(NbPacksVieRep);

            Simulation.Scene.Add(EtoileRep);
            Simulation.Scene.Add(ReserveDepartRep);

            Simulation.Scene.Add(FiltreGameplay);
            Simulation.Scene.Add(Gameplay);

            Simulation.Scene.Add(GenererSystemePlanetaireRep);
            Simulation.Scene.Add(GenererVaguesRep);
            Simulation.Scene.Add(GenererGameplayRep);

            foreach (var tourelle in TourellesRep)
                Simulation.Scene.Add(tourelle.Value);
            
            foreach (var tourelle in TourellesCheckBoxes)
                tourelle.Value.Draw();

            foreach (var powerUp in PowerUpsRep)
                Simulation.Scene.Add(powerUp.Value);

            foreach (var powerUp in PowerUpsCheckBoxes)
                powerUp.Value.Draw();

            foreach (var ennemi in EnnemisDisponiblesRep)
                Simulation.Scene.Add(ennemi.Value);

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                ennemi.Value.Draw();


            Simulation.Scene.Add(FiltreEditeur);
            Simulation.Scene.Add(Editeur);

            Simulation.Scene.Add(LoadRep);
            Simulation.Scene.Add(RestartRep);
            Simulation.Scene.Add(SaveRep);
            Simulation.Scene.Add(GenererRep);

            Simulation.Scene.Add(FiltreWaves);
            Simulation.Scene.Add(Waves);
            Simulation.Scene.Add(NbVaguesRep);
            Simulation.Scene.Add(QteEnnemisRep);
            Simulation.Scene.Add(DifficulteDebutRep);
            Simulation.Scene.Add(DifficulteFinRep);
            Simulation.Scene.Add(ReserveEnnemisRep);
            Simulation.Scene.Add(DeleteRep);

            Simulation.Scene.Add(SystemeCentreRep);

            SystemeCentreCheckBox.Draw();
            LoadSlider.Draw();
            DeletePushButton.Draw();
            DeleteSlider.Draw();

            NbVaguesSlider.Draw();
            QteEnnemisSlider.Draw();
            DifficulteDebutSlider.Draw();
            DifficulteFinSlider.Draw();
            ReserveEnnemisSlider.Draw();

            NbPlanetesSlider.Draw();
            NbEmplacementsSlider.Draw();
            NbPlanetesFixesSlider.Draw();
            GenererPushButton.Draw();
            NbPlanetesCheminDepartSlider.Draw();
            NbViesSlider.Draw();
            ValeurMinerauxSlider.Draw();
            NbPacksVieSlider.Draw();
            EtoileCheckBox.Draw();
            ReserveDepartSlider.Draw();
            LoadPushButton.Draw();
            RestartPushButton.Draw();
            SavePushButton.Draw();
            GenererSystemePlanetairePushButton.Draw();
            GenererVaguesPushButton.Draw();
            GenererGameplayPushButton.Draw();
        }

        private void updateSystemePlanetaire()
        {
            updateDonneesSystemePlanetaire();

            GenerateurScenario.NbCorpsCelestes = DonneesGenerateur.NbCorpsCelestes;
            GenerateurScenario.NbEmplacements = DonneesGenerateur.NbEmplacements;
            GenerateurScenario.NbCorpsCelestesFixes = DonneesGenerateur.NbCorpsCelestesFixes;
            GenerateurScenario.NbPlanetesCheminDeDepart = DonneesGenerateur.NbPlanetesCheminDeDepart;
            GenerateurScenario.AvecEtoileAuMilieu = DonneesGenerateur.AvecEtoileAuMilieu;
            GenerateurScenario.EnnemisPresents = DonneesGenerateur.EnnemisPresents;
            GenerateurScenario.SystemeCentre = DonneesGenerateur.SystemeCentre;

        }

        private void updateGameplay()
        {
            updateDonneesGameplay();

            GenerateurScenario.ViesPlaneteAProteger = DonneesGenerateur.ViesPlaneteAProteger;
            GenerateurScenario.ArgentExtra = DonneesGenerateur.ArgentExtra;
            GenerateurScenario.NbPacksVie = DonneesGenerateur.NbPacksVie;
            GenerateurScenario.ArgentDepart = DonneesGenerateur.ArgentDepart;
            GenerateurScenario.TourellesDisponibles = DonneesGenerateur.TourellesDisponibles;
            GenerateurScenario.PowerUpsDisponibles = DonneesGenerateur.PowerUpsDisponibles;
        }

        private void updateVagues()
        {
            updateDonneesVagues();

            GenerateurVagues.DifficultyStart = DonneesGenerateur.DifficulteDebut;
            GenerateurVagues.DifficultyEnd = DonneesGenerateur.DifficulteFin;
            GenerateurVagues.NbWaves = DonneesGenerateur.NbVagues;
            GenerateurVagues.QtyEnemies = DonneesGenerateur.QteEnnemis;
            GenerateurVagues.Enemies = DonneesGenerateur.EnnemisPresents;
            GenerateurScenario.EnnemisPresents = DonneesGenerateur.EnnemisPresents;
            GenerateurVagues.MineralsPerWave = DonneesGenerateur.ArgentEnnemis;
        }

        private void updateDonneesSystemePlanetaire()
        {
            DonneesGenerateur.AvecEtoileAuMilieu = EtoileCheckBox.Checked;
            DonneesGenerateur.NbCorpsCelestes = NbPlanetesSlider.Valeur;
            DonneesGenerateur.NbCorpsCelestesFixes = NbPlanetesFixesSlider.Valeur;
            DonneesGenerateur.NbEmplacements = NbEmplacementsSlider.Valeur;
            DonneesGenerateur.NbPlanetesCheminDeDepart = NbPlanetesCheminDepartSlider.Valeur;
            DonneesGenerateur.SystemeCentre = SystemeCentreCheckBox.Checked;
        }

        private void updateDonneesGameplay()
        {
            DonneesGenerateur.ArgentDepart = ReserveDepartSlider.Valeur;
            DonneesGenerateur.ArgentExtra = ValeurMinerauxSlider.Valeur;
            DonneesGenerateur.NbPacksVie = NbPacksVieSlider.Valeur;
            DonneesGenerateur.ViesPlaneteAProteger = NbViesSlider.Valeur;

            List<TurretType> tourellesDisponibles = new List<TurretType>();

            foreach (var tourelle in TourellesCheckBoxes)
                if (tourelle.Value.Checked)
                    tourellesDisponibles.Add(tourelle.Key);

            DonneesGenerateur.TourellesDisponibles = tourellesDisponibles;

            List<PowerUpType> powerUpsDisponibles = new List<PowerUpType>();

            foreach (var powerUp in PowerUpsCheckBoxes)
                if (powerUp.Value.Checked)
                    powerUpsDisponibles.Add(powerUp.Key);

            DonneesGenerateur.PowerUpsDisponibles = powerUpsDisponibles;
        }

        private void updateDonneesVagues()
        {
            DonneesGenerateur.NbVagues = NbVaguesSlider.Valeur;
            DonneesGenerateur.QteEnnemis = QteEnnemisSlider.Valeur;
            DonneesGenerateur.DifficulteDebut = DifficulteDebutSlider.Valeur;
            DonneesGenerateur.DifficulteFin = DifficulteFinSlider.Valeur;
            DonneesGenerateur.ArgentEnnemis = ReserveEnnemisSlider.Valeur;

            List<EnemyType> ennemisDisponibles = new List<EnemyType>();

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                if (ennemi.Value.Checked)
                    ennemisDisponibles.Add(ennemi.Key);

            DonneesGenerateur.EnnemisPresents = ennemisDisponibles;
        }

        private void updateGUI()
        {
            EtoileCheckBox.Checked = DonneesGenerateur.AvecEtoileAuMilieu;
            NbPlanetesSlider.Valeur = DonneesGenerateur.NbCorpsCelestes;
            NbPlanetesFixesSlider.Valeur = DonneesGenerateur.NbCorpsCelestesFixes;
            NbEmplacementsSlider.Valeur = DonneesGenerateur.NbEmplacements;
            NbPlanetesCheminDepartSlider.Valeur = DonneesGenerateur.NbPlanetesCheminDeDepart;
            SystemeCentreCheckBox.Checked = DonneesGenerateur.SystemeCentre;

            ReserveDepartSlider.Valeur = DonneesGenerateur.ArgentDepart;
            ValeurMinerauxSlider.Valeur = DonneesGenerateur.ArgentExtra;
            NbPacksVieSlider.Valeur = DonneesGenerateur.NbPacksVie;
            NbViesSlider.Valeur = DonneesGenerateur.ViesPlaneteAProteger;

            foreach (var tourelle in TourellesCheckBoxes)
                tourelle.Value.Checked = DonneesGenerateur.TourellesDisponibles.Contains(tourelle.Key);

            foreach (var powerUp in PowerUpsCheckBoxes)
                powerUp.Value.Checked = DonneesGenerateur.PowerUpsDisponibles.Contains(powerUp.Key);

            NbVaguesSlider.Valeur = DonneesGenerateur.NbVagues;
            QteEnnemisSlider.Valeur = DonneesGenerateur.QteEnnemis;
            DifficulteDebutSlider.Valeur = DonneesGenerateur.DifficulteDebut;
            DifficulteFinSlider.Valeur = DonneesGenerateur.DifficulteFin;
            ReserveEnnemisSlider.Valeur = DonneesGenerateur.ArgentEnnemis;

            List<EnemyType> ennemisDisponibles = new List<EnemyType>();

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                ennemi.Value.Checked = DonneesGenerateur.EnnemisPresents.Contains(ennemi.Key);
        }
    }
}
