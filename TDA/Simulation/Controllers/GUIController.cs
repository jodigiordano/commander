namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Physique;
    using System;

    class GUIController
    {
        public List<CorpsCeleste> CelestialBodies;
        public List<Turret> Turrets;
        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave;
        public Scenario Scenario;
        public ScenarioDescriptor DemoModeSelectedScenario;
        public List<Ennemi> Enemies;
        public VaguesInfinies InfiniteWaves;
        public LinkedList<Wave> Waves;
        public Path Path;
        public Path PathPreview;
        public HumanBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }

        private Simulation Simulation;
        private SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        private MenuGeneral MenuGeneral;
        private ScenarioAnnunciation ScenarioAnnunciation;
        private ScenarioEndedAnnunciation ScenarioEndedAnnunciation;
        private AdvancedView AdvancedView;
        private PlayerLives PlayerLives;
        private MenuCelestialBody MenuCelestialBody;
        private MenuPowerUps MenuPowerUps;
        private MenuTurret MenuTurret;
        private MenuDemo MenuDemo;
        private FinalSolutionPreview FinalSolutionPreview;
        private PathPreview PathPreviewing;
        private Cursor Cursor;
        private Cursor Crosshair;
        private bool PowerUpInputMode;
        private TheResistance GamePausedResistance;


        public GUIController(Simulation simulation)
        {
            Simulation = simulation;
            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulation);
            MenuGeneral = new MenuGeneral(Simulation, new Vector3(400, -260, 0));
            Cursor = new Cursor(Simulation.Main, Simulation.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral);
            Crosshair = new Cursor(Simulation.Main, Simulation.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, "crosshair", false);
            MenuTurret = new MenuTurret(Simulation, Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            MenuCelestialBody = new MenuCelestialBody(Simulation, Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            MenuPowerUps = new MenuPowerUps(Simulation, new Vector3(-550, 200, 0), Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            MenuDemo = new MenuDemo(Simulation, Preferences.PrioriteGUIPanneauCorpsCeleste - 0.01f);
            FinalSolutionPreview = new FinalSolutionPreview(Simulation);
            PowerUpInputMode = false;
            GamePausedResistance = new TheResistance(Simulation);
            GamePausedResistance.Initialize(new List<Ennemi>());
            GamePausedResistance.AlphaChannel = 100;
        }


        public void Initialize()
        {
            MenuGeneral.CompositionNextWave = CompositionNextWave;
            ScenarioAnnunciation = new ScenarioAnnunciation(Simulation, Scenario);
            ScenarioEndedAnnunciation = new ScenarioEndedAnnunciation(Simulation, CelestialBodies, Scenario);
            AdvancedView = new AdvancedView(Simulation, Enemies, CelestialBodies);
            PlayerLives = new PlayerLives(Simulation, Scenario.CelestialBodyToProtect, new Color(255, 0, 220));
            PathPreviewing = new PathPreview(PathPreview, Path);
            MenuCelestialBody.Initialize();

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.Initialize();

            MenuGeneral.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            MenuGeneral.TimeNextWave = Waves.First.Value.StartingTime;
            PowerUpInputMode = false;
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


        public void doCommonStashChanged(CommonStash stash)
        {
            MenuGeneral.Score = stash.TotalScore;
            MenuGeneral.Cash = stash.Cash;
        }


        public void doGameStateChanged(GameState newGameState)
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


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
            {
                Cursor.doHide();
                PowerUpInputMode = true;

                if (powerUp.Type == PowerUpType.RailGun)
                    Crosshair.doShow();
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
            {
                Cursor.doShow();
                PowerUpInputMode = false;

                if (powerUp.Type == PowerUpType.RailGun)
                    Crosshair.doHide();
            }
        }


        public void doWaveStarted()
        {
            MenuGeneral.TimeNextWave = double.MaxValue;
            MenuGeneral.RemainingWaves--;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxNouvelleVague");

            if (InfiniteWaves != null || MenuGeneral.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Wave> vagueSuivante = Waves.First;

            for (int i = 0; i < Waves.Count - MenuGeneral.RemainingWaves; i++)
                vagueSuivante = vagueSuivante.Next;

            MenuGeneral.TimeNextWave = vagueSuivante.Value.StartingTime;
        }


        public void doPlayerMoved(SimPlayer player)
        {
            Cursor.Position = player.Position;
            Crosshair.Position = player.Position;
        }


        public void doTurretToPlaceSelected(Turret turret)
        {
            Cursor.doHide();
            turret.CelestialBody.ShowTurretsZone = true;
            turret.ShowRange = true;
            turret.ShowForm = true;

            foreach (var turret2 in turret.CelestialBody.Turrets)
                turret2.ShowForm = true;
        }


        public void doTurretToPlaceDeselected(Turret turret)
        {
            Cursor.doShow();
            turret.CelestialBody.ShowTurretsZone = false;
            turret.ShowRange = false;
        }


        public void doTurretBought(Turret turret)
        {
            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit();
        }


        public void doTurretSold(Turret turret)
        {
            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit();
        }


        public void doPlayerSelectionChanged(SimPlayer player)
        {
            var selection = player.ActualSelection;

            MenuCelestialBody.CelestialBody = selection.CelestialBody;
            MenuCelestialBody.AvailableTurrets = selection.AvailableTurrets;
            MenuCelestialBody.TurretToBuy = selection.TurretToBuy;
            MenuCelestialBody.Visible = selection.TurretToPlace == null && selection.CelestialBody != null && selection.AvailableTurrets.Count != 0;

            MenuPowerUps.AvailablePowerUpsToBuy = selection.AvailablePowerUpsToBuy;
            MenuPowerUps.PowerUpToBuy = selection.PowerUpToBuy;

            MenuTurret.Turret = selection.Turret;
            MenuTurret.AvailableTurretOptions = selection.AvailableTurretOptions;
            MenuTurret.SelectedOption = selection.TurretOption;

            MenuDemo.CelestialBody = selection.CelestialBody;
            MenuDemo.Scenario =
                (selection.CelestialBody != null &&
                DemoModeSelectedScenario != null) ? DemoModeSelectedScenario : null;
            MenuDemo.Action = selection.GameAction;

            SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            FinalSolutionPreview.CelestialBody =
                (selection.PowerUpToBuy == PowerUpType.FinalSolution) ? selection.CelestialBody : null;

            if (PathPreviewing != null &&
                selection.Turret != null &&
                selection.Turret.Type == TurretType.Gravitational &&
                selection.Turret.CanSell &&
                !selection.Turret.Disabled &&
                selection.TurretOption == TurretAction.Sell)
                PathPreviewing.RemoveCelestialObject(selection.Turret.CelestialBody);
            else if (PathPreviewing != null &&
                selection.CelestialBody != null &&
                selection.PowerUpToBuy == PowerUpType.FinalSolution &&
                selection.CelestialBody.ContientTourelleGravitationnelle)
                PathPreviewing.RemoveCelestialObject(selection.CelestialBody);
            else if (PathPreviewing != null &&
                selection.TurretToBuy != null &&
                selection.TurretToBuy.Type == TurretType.Gravitational)
                PathPreviewing.AddCelestialObject(selection.CelestialBody);
            else if (PathPreviewing != null &&
                selection.TurretToPlace != null &&
                selection.TurretToPlace.Type == TurretType.Gravitational)
                PathPreviewing.AddCelestialObject(selection.CelestialBody);
            else if (PathPreviewing != null)
                PathPreviewing.RollBack();
        }


        public void Update(GameTime gameTime)
        {
            if (MenuGeneral.TimeNextWave > 0)
                MenuGeneral.TimeNextWave = Math.Max(0, MenuGeneral.TimeNextWave - gameTime.ElapsedGameTime.TotalMilliseconds);

            if (!PowerUpInputMode)
                SelectedCelestialBodyAnimation.Update(gameTime);

            //todo event-based
            MenuGeneral.MenuNextWave.Visible = Cursor.Actif && EphemereGames.Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, SandGlass.Rectangle);

            if (Simulation.ModeDemo)
            {
                GamePausedResistance.StartingObject = null;

                if (Simulation.Main.GameInProgress != null && Simulation.Main.GameInProgress.State == GameState.Paused)
                {
                    GamePausedResistance.StartingObject = Simulation.CelestialBodyPausedGame;

                    if (GamePausedResistance.StartingObject != null)
                        GamePausedResistance.Update();
                }
            }

            else
            {
                MenuGeneral.Update(gameTime);
                ScenarioAnnunciation.Update(gameTime);
                ScenarioEndedAnnunciation.Update(gameTime);
                PlayerLives.Update(gameTime);
                MenuPowerUps.Update();
            }
        }


        public void Draw()
        {
            Cursor.Draw();
            Crosshair.Draw();
            Path.Draw();

            if (!PowerUpInputMode)
                SelectedCelestialBodyAnimation.Draw();

            if (Simulation.ModeDemo)
            {
                if (Simulation.WorldMode)
                {
                    MenuDemo.Draw();

                    if (GamePausedResistance.StartingObject != null)
                        GamePausedResistance.Draw();
                }

                return;
            }

            MenuGeneral.Draw();
            ScenarioAnnunciation.Draw();
            ScenarioEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();
            MenuPowerUps.Draw();

            if (PowerUpInputMode)
                return;

            MenuCelestialBody.Draw();
            MenuTurret.Draw();
            PathPreviewing.Draw();
            FinalSolutionPreview.Draw();
        }
    }
}
