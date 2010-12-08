namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using System;

    class GUIController
    {
        public List<CorpsCeleste> CelestialBodies;
        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionNextWave;
        public Scenario Scenario;
        public List<Ennemi> Enemies;
        public VaguesInfinies InfiniteWaves;
        public LinkedList<Vague> Waves;
        public Chemin Path;
        public Chemin PathPreview;

        private Simulation Simulation;
        private SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        private MenuGeneral MenuGeneral;
        private ScenarioAnnunciation ScenarioAnnunciation;
        private ScenarioEndedAnnunciation ScenarioEndedAnnunciation;
        private AdvancedView AdvancedView;
        private PlayerLives PlayerLives;
        private MenuTurretSpotEmpty MenuTurretSpotEmpty;
        private MenuTurretSpotOccupied MenuTurretSpotOccupied;
        private MenuPowerUps MenuPowerUps;
        private FinalSolutionPreview FinalSolutionPreview;
        private PathPreview PathPreviewing;
        private Cursor Cursor;
        private bool InSpaceShip;


        public GUIController(Simulation simulation)
        {
            Simulation = simulation;
            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulation);
            MenuGeneral = new MenuGeneral(Simulation, new Vector3(400, -260, 0));
            Cursor = new Cursor(Simulation.Main, Simulation.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral);
            MenuTurretSpotEmpty = new MenuTurretSpotEmpty(Simulation, Preferences.PrioriteGUIPanneauCorpsCeleste);
            MenuTurretSpotOccupied = new MenuTurretSpotOccupied(Simulation, Preferences.PrioriteGUIPanneauCorpsCeleste);
            MenuPowerUps = new MenuPowerUps(Simulation, Preferences.PrioriteGUIPanneauCorpsCeleste);
            FinalSolutionPreview = new FinalSolutionPreview(Simulation);
            InSpaceShip = false;
        }


        public void Initialize()
        {
            MenuGeneral.CompositionNextWave = CompositionNextWave;
            ScenarioAnnunciation = new ScenarioAnnunciation(Simulation, Scenario);
            ScenarioEndedAnnunciation = new ScenarioEndedAnnunciation(Simulation, CelestialBodies);
            AdvancedView = new AdvancedView(Simulation, Enemies, CelestialBodies);
            PlayerLives = new PlayerLives(Simulation, Scenario.CorpsCelesteAProteger, new Color(255, 0, 220));
            PathPreviewing = new PathPreview(PathPreview);

            MenuGeneral.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            MenuGeneral.TimeNextWave = Waves.First.Value.TempsApparition;
            InSpaceShip = false;
        }


        public Sablier SandGlass
        {
            get { return MenuGeneral.SandGlass; }
        }


        public void doShowCompositionNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = true;
        }


        public void doHideCompositionNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = false;
        }


        public void doNextWave()
        {
            MenuGeneral.SandGlass.tourner();
        }


        public void doScoreChanged(int score)
        {
            MenuGeneral.Score = score;
        }


        public void doCashChanged(int cash)
        {
            MenuGeneral.Cash = cash;
        }


        public void doGameStateChanged(EtatPartie newGameState)
        {
            ScenarioEndedAnnunciation.doGameStateChanged(newGameState);
        }


        public void doShowAdvancedView()
        {
            AdvancedView.Visible = true;
        }


        public void doHideAdvancedView()
        {
            AdvancedView.Visible = false;
        }


        public void doObjectCreated(IObjetPhysique obj)
        {
            Vaisseau spaceship = obj as Vaisseau;

            if (spaceship != null)
            {
                Cursor.doHide();
                InSpaceShip = true;

                return;
            }
        }


        public void doObjectDestroyed(IObjetPhysique obj)
        {
            Vaisseau spaceship = obj as Vaisseau;

            if (spaceship != null)
            {
                Cursor.doShow();
                InSpaceShip = false;

                return;
            }
        }


        public void doWaveStarted()
        {
            MenuGeneral.TimeNextWave = double.MaxValue;
            MenuGeneral.RemainingWaves--;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxNouvelleVague");

            if (InfiniteWaves != null || MenuGeneral.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Vague> vagueSuivante = Waves.First;

            for (int i = 0; i < Waves.Count - MenuGeneral.RemainingWaves; i++)
                vagueSuivante = vagueSuivante.Next;

            MenuGeneral.TimeNextWave = vagueSuivante.Value.TempsApparition;
        }


        public void doPlayerMoved(SimPlayer player)
        {
            Cursor.Position = player.Position;
        }


        public void doPlayerSelectionChanged(SimPlayer player)
        {
            var selection = player.ActualSelection;

            MenuTurretSpotEmpty.TurretSpot = selection.TurretSpot;
            MenuTurretSpotEmpty.AvailableTurretsToBuy = selection.AvailableTurretsToBuy;
            MenuTurretSpotEmpty.TurretToBuy = selection.TurretToBuy;

            MenuTurretSpotOccupied.TurretSpot = selection.TurretSpot;
            MenuTurretSpotOccupied.AvailableTurretOptions = selection.AvailableTurretOptions;
            MenuTurretSpotOccupied.SelectedOption = selection.TurretOption;

            MenuPowerUps.CelestialBody = selection.CelestialBody;
            MenuPowerUps.Options = selection.AvailableCelestialBodyOptions;
            MenuPowerUps.SelectedOption = selection.CelestialBodyOption;

            SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            FinalSolutionPreview.CelestialBody =
                (selection.CelestialBodyOption == PowerUp.FinalSolution) ? selection.CelestialBody : null;

            if (PathPreviewing != null && selection.CelestialBody != null)
            {
                if (selection.CelestialBodyOption == PowerUp.FinalSolution && selection.CelestialBody.ContientTourelleGravitationnelle)
                    PathPreviewing.RemoveCelestialObject(selection.CelestialBody);
                else if (selection.TurretToBuy != null && selection.TurretToBuy.Type == TypeTourelle.Gravitationnelle)
                    PathPreviewing.AddCelestialObject(selection.CelestialBody);
                else if (selection.Turret != null && selection.Turret.Type == TypeTourelle.Gravitationnelle && selection.Turret.PeutVendre && selection.TurretOption == TurretAction.Sell)
                    PathPreviewing.RemoveCelestialObject(selection.CelestialBody);
                else
                    PathPreviewing.RollBack();
            }

            else if (PathPreviewing != null)
            {
                PathPreviewing.RollBack();
            }
        }


        public void Update(GameTime gameTime)
        {
            if (MenuGeneral.TimeNextWave > 0)
                MenuGeneral.TimeNextWave = Math.Max(0, MenuGeneral.TimeNextWave - gameTime.ElapsedGameTime.TotalMilliseconds);

            if (!InSpaceShip)
                SelectedCelestialBodyAnimation.Update(gameTime);

            //todo event-based
            MenuGeneral.MenuNextWave.Visible = Cursor.Actif && Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, SandGlass.Rectangle);

            if (!Simulation.ModeDemo)
            {
                MenuGeneral.Update(gameTime);
                ScenarioAnnunciation.Update(gameTime);
                ScenarioEndedAnnunciation.Update(gameTime);
                PlayerLives.Update(gameTime);
            }
        }


        public void Draw()
        {
            Cursor.Draw();
            Path.Draw();

            if (!InSpaceShip)
                SelectedCelestialBodyAnimation.Draw();

            if (Simulation.ModeDemo)
                return;

            MenuGeneral.Draw();
            ScenarioAnnunciation.Draw();
            ScenarioEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();

            if (InSpaceShip)
                return;

            MenuPowerUps.Draw();
            MenuTurretSpotEmpty.Draw();
            MenuTurretSpotOccupied.Draw();
            PathPreviewing.Draw();
            FinalSolutionPreview.Draw();
        }
    }
}
