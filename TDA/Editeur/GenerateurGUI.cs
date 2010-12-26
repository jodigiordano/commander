﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;

    public struct DonneesGenerateur
    {
        // Système planétaire
        public int NbCorpsCelestes;
        public int NbCorpsCelestesFixes;
        public bool AvecEtoileAuMilieu;
        public int NbPlanetesCheminDeDepart;
        public int NbEmplacements;
        public bool SystemeCentre;

        // Gameplay
        public List<TypeTourelle> TourellesDisponibles;
        public List<PowerUp> PowerUpsDisponibles;        
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

    class GenerateurGUI : DrawableGameComponent
    {
        private DonneesGenerateur DonneesGenerateur;
        private DescripteurScenario DescriptionScenario;

        private Simulation Simulation;
        private GenerateurScenario GenerateurScenario;
        private GenerateurVagues GenerateurVagues;
        private Vector3 Position;

        #region Systeme Planetaire
        private IVisible FiltreSystemePlanetaire;
        private IVisible SystemePlanetaire;

        private IVisible NbPlanetesRep;
        private HorizontalSlider NbPlanetesSlider;

        private IVisible NbEmplacementsRep;
        private HorizontalSlider NbEmplacementsSlider;

        private IVisible NbPlanetesFixesRep;
        private HorizontalSlider NbPlanetesFixesSlider;

        private IVisible NbPlanetesCheminDepartRep;
        private HorizontalSlider NbPlanetesCheminDepartSlider;

        private IVisible EtoileRep;
        private CheckBox EtoileCheckBox;

        private IVisible SystemeCentreRep;
        private CheckBox SystemeCentreCheckBox;

        private IVisible GenererSystemePlanetaireRep;
        private PushButton GenererSystemePlanetairePushButton;

        #endregion

        #region Gameplay elements

        private IVisible FiltreGameplay;
        private IVisible Gameplay;

        private IVisible NbViesRep;
        private HorizontalSlider NbViesSlider;

        private IVisible ValeurMinerauxRep;
        private HorizontalSlider ValeurMinerauxSlider;

        private IVisible NbPacksVieRep;
        private HorizontalSlider NbPacksVieSlider;

        private IVisible ReserveDepartRep;
        private HorizontalSlider ReserveDepartSlider;

        private Dictionary<TypeTourelle, IVisible> TourellesRep;
        private Dictionary<TypeTourelle, CheckBox> TourellesCheckBoxes;

        private Dictionary<PowerUp, IVisible> PowerUpsRep;
        private Dictionary<PowerUp, CheckBox> PowerUpsCheckBoxes;

        private IVisible GenererGameplayRep;
        private PushButton GenererGameplayPushButton;

        #endregion

        #region Waves

        private IVisible FiltreWaves;
        private IVisible Waves;

        private IVisible DifficulteDebutRep;
        private HorizontalSlider DifficulteDebutSlider;

        private IVisible DifficulteFinRep;
        private HorizontalSlider DifficulteFinSlider;

        private IVisible QteEnnemisRep;
        private HorizontalSlider QteEnnemisSlider;

        private IVisible NbVaguesRep;
        private HorizontalSlider NbVaguesSlider;

        private IVisible ReserveEnnemisRep;
        private HorizontalSlider ReserveEnnemisSlider;

        private Dictionary<EnemyType, IVisible> EnnemisDisponiblesRep;
        private Dictionary<EnemyType, CheckBox> EnnemisDisponiblesCheckBoxes;

        private IVisible GenererVaguesRep;
        private PushButton GenererVaguesPushButton;

        #endregion

        #region Editeur

        private IVisible FiltreEditeur;
        private IVisible Editeur;

        private IVisible GenererRep;
        private PushButton GenererPushButton;

        private IVisible LoadRep;
        private HorizontalSlider LoadSlider;
        private PushButton LoadPushButton;

        private IVisible DeleteRep;
        private HorizontalSlider DeleteSlider;
        private PushButton DeletePushButton;

        private IVisible RestartRep;
        private PushButton RestartPushButton;

        private IVisible SaveRep;
        private PushButton SavePushButton;

        #endregion

        public GenerateurGUI(Simulation simulation, Cursor curseur, Vector3 position)
            : base(simulation.Main)
        {
            Simulation = simulation;
            Position = position;

            #region Systeme Planetaire

            FiltreSystemePlanetaire = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Position - new Vector3(0, 90, 0));
            FiltreSystemePlanetaire.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreSystemePlanetaire.TailleVecteur = new Vector2(540, 320);
            FiltreSystemePlanetaire.Couleur = new Color(255, 0, 220, 128);
            FiltreSystemePlanetaire.Origine = FiltreSystemePlanetaire.Centre;

            SystemePlanetaire = new IVisible("Planetary System", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(0, 250, 0));
            SystemePlanetaire.Taille = 4;
            SystemePlanetaire.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            SystemePlanetaire.Origine = SystemePlanetaire.Centre;


            NbPlanetesRep = new IVisible("Planets", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 220, 0));
            NbPlanetesRep.Taille = 2;
            NbPlanetesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, Position - new Vector3(-150, 210, 0), 1, 15, 3, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbPlanetesFixesRep = new IVisible("Fixed planets", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 180, 0));
            NbPlanetesFixesRep.Taille = 2;
            NbPlanetesFixesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesFixesSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, Position - new Vector3(-150, 170, 0), 0, 15, 0, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbEmplacementsRep = new IVisible("Slots", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 140, 0));
            NbEmplacementsRep.Taille = 2;
            NbEmplacementsRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbEmplacementsSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, Position - new Vector3(-150, 130, 0), 1, 50, 10, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            NbPlanetesCheminDepartRep = new IVisible("Path length", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 100, 0));
            NbPlanetesCheminDepartRep.Taille = 2;
            NbPlanetesCheminDepartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPlanetesCheminDepartSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, Position - new Vector3(-150, 90, 0), 0, 15, 3, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            EtoileRep = new IVisible("Star in the middle?", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 60, 0));
            EtoileRep.Taille = 2;
            EtoileRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            EtoileCheckBox = new CheckBox(Simulation, curseur, Position - new Vector3(-200, 50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            SystemeCentreRep = new IVisible("Centre?", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(250, 20, 0));
            SystemeCentreRep.Taille = 2;
            SystemeCentreRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            SystemeCentreCheckBox = new CheckBox(Simulation, curseur, Position - new Vector3(-200, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            GenererSystemePlanetaireRep = new IVisible("Generate", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position - new Vector3(-90, -40, 0));
            GenererSystemePlanetaireRep.Taille = 2;
            GenererSystemePlanetaireRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererSystemePlanetairePushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, Position - new Vector3(-240, -50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            #endregion

            #region Gameplay Elements

            FiltreGameplay = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Position + new Vector3(600, -90, 0));
            FiltreGameplay.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreGameplay.TailleVecteur = new Vector2(540, 320);
            FiltreGameplay.Couleur = new Color(76, 255, 0, 128);
            FiltreGameplay.Origine = FiltreSystemePlanetaire.Centre;

            Gameplay = new IVisible("Gameplay", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(0, 160, 0));
            Gameplay.Taille = 4;
            Gameplay.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Gameplay.Origine = Gameplay.Centre;

            ReserveDepartRep = new IVisible("Minerals", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(250, 120, 0));
            ReserveDepartRep.Taille = 2;
            ReserveDepartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ReserveDepartSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 110, 0), 0, 10000, 0, 100, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbViesRep = new IVisible("Lives", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(250, 80, 0));
            NbViesRep.Taille = 2;
            NbViesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbViesSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 70, 0), 1, 50, 5, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            ValeurMinerauxRep = new IVisible("Enemies minerals", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(250, 40, 0));
            ValeurMinerauxRep.Taille = 2;
            ValeurMinerauxRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ValeurMinerauxSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, 30, 0), 0, 5000, 250, 250, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            NbPacksVieRep = new IVisible("Life packs", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(250, 0, 0));
            NbPacksVieRep.Taille = 2;
            NbPacksVieRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbPacksVieSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-150, -10, 0), 0, 20, 5, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            TourellesRep = new Dictionary<TypeTourelle, IVisible>();
            TourellesCheckBoxes = new Dictionary<TypeTourelle, CheckBox>();

            Vector3 positionTourelle = FiltreGameplay.Position - new Vector3(230, -60, 0);

            foreach (var tourelle in FactoryTourelles.GetTourellesDisponibles())
            {
                IVisible iv = (IVisible) tourelle.representationBase.Clone();
                iv.Position = positionTourelle;
                iv.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

                TourellesRep.Add(tourelle.Type, iv);
                TourellesCheckBoxes.Add(tourelle.Type, new CheckBox(Simulation, curseur, iv.Position + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));

                TourellesCheckBoxes[tourelle.Type].Checked = true;

                positionTourelle += new Vector3(85, 0, 0);
            }


            Vector3 positionPowerUp = FiltreGameplay.Position - new Vector3(230, -120, 0);

            PowerUpsRep = new Dictionary<PowerUp, IVisible>();
            PowerUpsCheckBoxes = new Dictionary<PowerUp, CheckBox>();

            PowerUpsRep.Add(PowerUp.DoItYourself, new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Vaisseau"), positionPowerUp));
            PowerUpsRep[PowerUp.DoItYourself].Taille = 4;
            PowerUpsRep[PowerUp.DoItYourself].Origine = PowerUpsRep[PowerUp.DoItYourself].Centre;
            PowerUpsRep[PowerUp.DoItYourself].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUp.DoItYourself, new CheckBox(Simulation, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUp.DoItYourself].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUp.CollectTheRent, new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Collecteur"), positionPowerUp));
            PowerUpsRep[PowerUp.CollectTheRent].Taille = 4;
            PowerUpsRep[PowerUp.CollectTheRent].Origine = PowerUpsRep[PowerUp.CollectTheRent].Centre;
            PowerUpsRep[PowerUp.CollectTheRent].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUp.CollectTheRent, new CheckBox(Simulation, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUp.CollectTheRent].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUp.FinalSolution, new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Destruction"), positionPowerUp));
            PowerUpsRep[PowerUp.FinalSolution].Taille = 4;
            PowerUpsRep[PowerUp.FinalSolution].Origine = PowerUpsRep[PowerUp.FinalSolution].Centre;
            PowerUpsRep[PowerUp.FinalSolution].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUp.FinalSolution, new CheckBox(Simulation, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUp.FinalSolution].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            PowerUpsRep.Add(PowerUp.TheResistance, new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("TheResistance"), positionPowerUp));
            PowerUpsRep[PowerUp.TheResistance].Taille = 4;
            PowerUpsRep[PowerUp.TheResistance].Origine = PowerUpsRep[PowerUp.TheResistance].Centre;
            PowerUpsRep[PowerUp.TheResistance].VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            PowerUpsCheckBoxes.Add(PowerUp.TheResistance, new CheckBox(Simulation, curseur, positionPowerUp + new Vector3(40, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));
            PowerUpsCheckBoxes[PowerUp.TheResistance].Checked = true;
            positionPowerUp += new Vector3(85, 0, 0);

            GenererGameplayRep = new IVisible("Generate", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreGameplay.Position - new Vector3(-90, -130, 0));
            GenererGameplayRep.Taille = 2;
            GenererGameplayRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererGameplayPushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreGameplay.Position - new Vector3(-240, -140, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            #endregion

            #region Waves

            FiltreWaves = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Position + new Vector3(300, 175, 0));
            FiltreWaves.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreWaves.TailleVecteur = new Vector2(1137, 140);
            FiltreWaves.Couleur = new Color(255, 0, 0, 128);
            FiltreWaves.Origine = FiltreWaves.Centre;


            Waves = new IVisible("Waves", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(0, 70, 0));
            Waves.Taille = 4;
            Waves.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Waves.Origine = Waves.Centre;


            NbVaguesRep = new IVisible("Waves", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(550, 50, 0));
            NbVaguesRep.Taille = 2;
            NbVaguesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            NbVaguesSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, 40, 0), 1, 30, 1, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            QteEnnemisRep = new IVisible("Quantity", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(550, 10, 0));
            QteEnnemisRep.Taille = 2;
            QteEnnemisRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            QteEnnemisSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, 0, 0), 20, 1000, 20, 20, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            ReserveEnnemisRep = new IVisible("Money", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(550, -30, 0));
            ReserveEnnemisRep.Taille = 2;
            ReserveEnnemisRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            ReserveEnnemisSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(300, -40, 0), 0, 10000, 0, 250, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            DifficulteDebutRep = new IVisible("Diff. start", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(200, 30, 0));
            DifficulteDebutRep.Taille = 2;
            DifficulteDebutRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DifficulteDebutSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-100, 20, 0), 2, 100, 2, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            DifficulteFinRep = new IVisible("Diff. end", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(-200, 30, 0));
            DifficulteFinRep.Taille = 2;
            DifficulteFinRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DifficulteFinSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-450, 20, 0), 2, 100, 2, 2, Preferences.PrioriteGUIConsoleEditeur + 0.005f);

 
            EnnemisDisponiblesRep = new Dictionary<EnemyType, IVisible>();
            EnnemisDisponiblesCheckBoxes = new Dictionary<EnemyType, CheckBox>();

            Vector3 positionEnnemis = FiltreWaves.Position - new Vector3(190, -30, 0);

            foreach (var ennemi in FactoryEnnemis.Instance.GetEnnemisDisponibles(Simulation))
            {
                ennemi.Initialize();
                IVisible iv = (IVisible)ennemi.RepresentationVivant.Clone();
                iv.Position = positionEnnemis;
                iv.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

                EnnemisDisponiblesRep.Add(ennemi.Type, iv);
                EnnemisDisponiblesCheckBoxes.Add(ennemi.Type, new CheckBox(Simulation, curseur, iv.Position + new Vector3(45, 0, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f));

                EnnemisDisponiblesCheckBoxes[ennemi.Type].Checked = true;

                positionEnnemis += new Vector3(100, 0, 0);
            }


            GenererVaguesRep = new IVisible("Generate", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreWaves.Position - new Vector3(-390, -40, 0));
            GenererVaguesRep.Taille = 2;
            GenererVaguesRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererVaguesPushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreWaves.Position - new Vector3(-540, -50, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            #endregion

            #region Editeur

            FiltreEditeur = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Position + new Vector3(300, -340, 0));
            FiltreEditeur.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.01f;
            FiltreEditeur.TailleVecteur = new Vector2(740, 120);
            FiltreEditeur.Couleur = new Color(0, 148, 255, 128);
            FiltreEditeur.Origine = FiltreEditeur.Centre;

            Editeur = new IVisible("Editor", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(0, 60, 0));
            Editeur.Taille = 4;
            Editeur.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;
            Editeur.Origine = Editeur.Centre;


            LoadRep = new IVisible("Load", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(350, 20, 0));
            LoadRep.Taille = 2;
            LoadRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            LoadSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(150, 10, 0), -1, 1000, 0, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);
            LoadPushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(50, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            DeleteRep = new IVisible("Delete", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(350, -20, 0));
            DeleteRep.Taille = 2;
            DeleteRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            DeleteSlider = new HorizontalSlider(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(150, -30, 0), -1, 1000, -1, 1, Preferences.PrioriteGUIConsoleEditeur + 0.005f);
            DeletePushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(50, -30, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            GenererRep = new IVisible("Generate", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(-175, 20, 0));
            GenererRep.Taille = 2;
            GenererRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            GenererPushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-330, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            RestartRep = new IVisible("Restart", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(0, 20, 0));
            RestartRep.Taille = 2;
            RestartRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            RestartPushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-150, 10, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);


            SaveRep = new IVisible("Save", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, FiltreEditeur.Position - new Vector3(0, -20, 0));
            SaveRep.Taille = 2;
            SaveRep.VisualPriority = Preferences.PrioriteGUIConsoleEditeur + 0.005f;

            SavePushButton = new PushButton(Simulation.Main, Simulation.Scene, curseur, FiltreEditeur.Position - new Vector3(-150, -30, 0), Preferences.PrioriteGUIConsoleEditeur + 0.005f);

            #endregion

            GenerateurScenario = new GenerateurScenario();
            GenerateurVagues = new GenerateurVagues();
        }


        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            NbPlanetesSlider.Update(gameTime);
            NbEmplacementsSlider.Update(gameTime);
            GenererPushButton.Update(gameTime);
            NbPlanetesFixesSlider.Update(gameTime);
            NbPlanetesCheminDepartSlider.Update(gameTime);
            NbViesSlider.Update(gameTime);
            ValeurMinerauxSlider.Update(gameTime);
            NbPacksVieSlider.Update(gameTime);
            EtoileCheckBox.Update(gameTime);
            ReserveDepartSlider.Update(gameTime);
            LoadPushButton.Update(gameTime);
            LoadSlider.Update(gameTime);
            DeletePushButton.Update(gameTime);
            DeleteSlider.Update(gameTime);
            RestartPushButton.Update(gameTime);
            SavePushButton.Update(gameTime);
            NbVaguesSlider.Update(gameTime);
            QteEnnemisSlider.Update(gameTime);
            DifficulteDebutSlider.Update(gameTime);
            DifficulteFinSlider.Update(gameTime);
            GenererSystemePlanetairePushButton.Update(gameTime);
            GenererVaguesPushButton.Update(gameTime);
            GenererGameplayPushButton.Update(gameTime);
            ReserveEnnemisSlider.Update(gameTime);
            SystemeCentreCheckBox.Update(gameTime);

            foreach (var tourelle in TourellesCheckBoxes)
                tourelle.Value.Update(gameTime);

            foreach (var powerUp in PowerUpsCheckBoxes)
                powerUp.Value.Update(gameTime);

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                ennemi.Value.Update(gameTime);

            NbPlanetesFixesSlider.Valeur = (int)MathHelper.Clamp(NbPlanetesFixesSlider.Valeur, NbPlanetesFixesSlider.Min, NbPlanetesSlider.Valeur);
            NbPlanetesCheminDepartSlider.Valeur = (int)MathHelper.Clamp(NbPlanetesCheminDepartSlider.Valeur, 0, NbPlanetesSlider.Valeur);
            DifficulteFinSlider.Valeur = (int)MathHelper.Clamp(DifficulteFinSlider.Valeur, DifficulteDebutSlider.Valeur, DifficulteFinSlider.Max);
            //LoadSlider.Valeur = (int)MathHelper.Clamp(LoadSlider.Valeur, -1, Simulation.Main.SaveGame.DescriptionsScenarios.Count);
            //DeleteSlider.Valeur = (int)MathHelper.Clamp(DeleteSlider.Valeur, -1, Simulation.Main.SaveGame.DescriptionsScenarios.Count);

            if (GenererPushButton.Pressed)
            {
                updateVagues();
                updateSystemePlanetaire();
                updateGameplay();

                GenerateurVagues.generer();
                GenerateurScenario.genererSystemePlanetaire();
                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Vagues;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            if (GenererSystemePlanetairePushButton.Pressed)
            {
                updateSystemePlanetaire();

                if (GenerateurVagues.Vagues == null)
                {
                    updateVagues();
                    updateGameplay();

                    GenerateurVagues.generer();
                }

                GenerateurScenario.genererSystemePlanetaire();
                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Vagues;

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

                GenerateurVagues.generer();
                GenerateurScenario.genererCeintureAsteroides();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Vagues;

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

                    GenerateurVagues.generer();
                    GenerateurScenario.genererSystemePlanetaire();
                }

                GenerateurScenario.genererGameplay();

                DescriptionScenario = GenerateurScenario.DescripteurScenario;
                DescriptionScenario.Waves = GenerateurVagues.Vagues;

                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }


            if (RestartPushButton.Pressed && DescriptionScenario != null)
            {
                Simulation.DescriptionScenario = DescriptionScenario;
                Simulation.Initialize();
            }

            //if (LoadPushButton.Pressed && LoadSlider.Valeur != -1 && LoadSlider.Valeur < Simulation.Main.SaveGame.DescriptionsScenarios.Count)
            //{
            //    DescriptionScenario = Simulation.Main.SaveGame.DescriptionsScenarios[LoadSlider.Valeur];
            //    DonneesGenerateur = Simulation.Main.SaveGame.DonneesGenerateur[LoadSlider.Valeur];

            //    GenerateurScenario.DescripteurScenario = DescriptionScenario;
            //    GenerateurVagues.Vagues = DescriptionScenario.Vagues;

            //    updateGUI();

            //    Simulation.DescriptionScenario = DescriptionScenario;
            //    Simulation.Initialize();
            //}

            //if (SavePushButton.Pressed && DescriptionScenario != null)
            //{
            //    updateDonneesSystemePlanetaire();
            //    updateDonneesGameplay();
            //    updateDonneesVagues();

            //    if (LoadSlider.Valeur == -1 || LoadSlider.Valeur >= Simulation.Main.SaveGame.DescriptionsScenarios.Count)
            //    {
            //        Simulation.Main.SaveGame.DescriptionsScenarios.Add(DescriptionScenario);
            //        Simulation.Main.SaveGame.DonneesGenerateur.Add(DonneesGenerateur);
            //    }

            //    else
            //    {
            //        Simulation.Main.SaveGame.DescriptionsScenarios[LoadSlider.Valeur] = DescriptionScenario;
            //        Simulation.Main.SaveGame.DonneesGenerateur[LoadSlider.Valeur] = DonneesGenerateur;
            //    }

            //    EphemereGames.Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
            //}

            //if (DeletePushButton.Pressed && DeleteSlider.Valeur != -1 && DeleteSlider.Valeur < Simulation.Main.SaveGame.DescriptionsScenarios.Count)
            //{
            //    Simulation.Main.SaveGame.DescriptionsScenarios.RemoveAt(DeleteSlider.Valeur);
            //    Simulation.Main.SaveGame.DonneesGenerateur.RemoveAt(DeleteSlider.Valeur);

            //    DeleteSlider.Valeur = -1;

            //    EphemereGames.Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            Simulation.Scene.ajouterScenable(FiltreSystemePlanetaire);
            Simulation.Scene.ajouterScenable(SystemePlanetaire);
            Simulation.Scene.ajouterScenable(NbPlanetesRep);
            Simulation.Scene.ajouterScenable(NbEmplacementsRep);
            Simulation.Scene.ajouterScenable(NbPlanetesFixesRep);

            Simulation.Scene.ajouterScenable(NbPlanetesCheminDepartRep);
            Simulation.Scene.ajouterScenable(NbViesRep);
            Simulation.Scene.ajouterScenable(ValeurMinerauxRep);
            Simulation.Scene.ajouterScenable(NbPacksVieRep);

            Simulation.Scene.ajouterScenable(EtoileRep);
            Simulation.Scene.ajouterScenable(ReserveDepartRep);

            Simulation.Scene.ajouterScenable(FiltreGameplay);
            Simulation.Scene.ajouterScenable(Gameplay);

            Simulation.Scene.ajouterScenable(GenererSystemePlanetaireRep);
            Simulation.Scene.ajouterScenable(GenererVaguesRep);
            Simulation.Scene.ajouterScenable(GenererGameplayRep);

            foreach (var tourelle in TourellesRep)
                Simulation.Scene.ajouterScenable(tourelle.Value);
            
            foreach (var tourelle in TourellesCheckBoxes)
                tourelle.Value.Draw(gameTime);

            foreach (var powerUp in PowerUpsRep)
                Simulation.Scene.ajouterScenable(powerUp.Value);

            foreach (var powerUp in PowerUpsCheckBoxes)
                powerUp.Value.Draw(gameTime);

            foreach (var ennemi in EnnemisDisponiblesRep)
                Simulation.Scene.ajouterScenable(ennemi.Value);

            foreach (var ennemi in EnnemisDisponiblesCheckBoxes)
                ennemi.Value.Draw(gameTime);


            Simulation.Scene.ajouterScenable(FiltreEditeur);
            Simulation.Scene.ajouterScenable(Editeur);

            Simulation.Scene.ajouterScenable(LoadRep);
            Simulation.Scene.ajouterScenable(RestartRep);
            Simulation.Scene.ajouterScenable(SaveRep);
            Simulation.Scene.ajouterScenable(GenererRep);

            Simulation.Scene.ajouterScenable(FiltreWaves);
            Simulation.Scene.ajouterScenable(Waves);
            Simulation.Scene.ajouterScenable(NbVaguesRep);
            Simulation.Scene.ajouterScenable(QteEnnemisRep);
            Simulation.Scene.ajouterScenable(DifficulteDebutRep);
            Simulation.Scene.ajouterScenable(DifficulteFinRep);
            Simulation.Scene.ajouterScenable(ReserveEnnemisRep);
            Simulation.Scene.ajouterScenable(DeleteRep);

            Simulation.Scene.ajouterScenable(SystemeCentreRep);

            SystemeCentreCheckBox.Draw(gameTime);
            LoadSlider.Draw(gameTime);
            DeletePushButton.Draw(gameTime);
            DeleteSlider.Draw(gameTime);

            NbVaguesSlider.Draw(gameTime);
            QteEnnemisSlider.Draw(gameTime);
            DifficulteDebutSlider.Draw(gameTime);
            DifficulteFinSlider.Draw(gameTime);
            ReserveEnnemisSlider.Draw(gameTime);

            NbPlanetesSlider.Draw(gameTime);
            NbEmplacementsSlider.Draw(gameTime);
            NbPlanetesFixesSlider.Draw(gameTime);
            GenererPushButton.Draw(gameTime);
            NbPlanetesCheminDepartSlider.Draw(gameTime);
            NbViesSlider.Draw(gameTime);
            ValeurMinerauxSlider.Draw(gameTime);
            NbPacksVieSlider.Draw(gameTime);
            EtoileCheckBox.Draw(gameTime);
            ReserveDepartSlider.Draw(gameTime);
            LoadPushButton.Draw(gameTime);
            RestartPushButton.Draw(gameTime);
            SavePushButton.Draw(gameTime);
            GenererSystemePlanetairePushButton.Draw(gameTime);
            GenererVaguesPushButton.Draw(gameTime);
            GenererGameplayPushButton.Draw(gameTime);
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

            GenerateurVagues.DifficulteDebut = DonneesGenerateur.DifficulteDebut;
            GenerateurVagues.DifficulteFin = DonneesGenerateur.DifficulteFin;
            GenerateurVagues.NbVagues = DonneesGenerateur.NbVagues;
            GenerateurVagues.QteEnnemis = DonneesGenerateur.QteEnnemis;
            GenerateurVagues.EnnemisPresents = DonneesGenerateur.EnnemisPresents;
            GenerateurScenario.EnnemisPresents = DonneesGenerateur.EnnemisPresents;
            GenerateurVagues.ArgentEnnemis = DonneesGenerateur.ArgentEnnemis;
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

            List<TypeTourelle> tourellesDisponibles = new List<TypeTourelle>();

            foreach (var tourelle in TourellesCheckBoxes)
                if (tourelle.Value.Checked)
                    tourellesDisponibles.Add(tourelle.Key);

            DonneesGenerateur.TourellesDisponibles = tourellesDisponibles;

            List<PowerUp> powerUpsDisponibles = new List<PowerUp>();

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
