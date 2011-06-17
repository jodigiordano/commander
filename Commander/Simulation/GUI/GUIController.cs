namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class GUIController
    {
        public List<CelestialBody> CelestialBodies;
        public List<Turret> Turrets;
        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave;
        public Level Level;
        public LevelDescriptor LevelSelectedDemoMode;
        public List<Enemy> Enemies;
        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;
        public Path Path;
        public Path PathPreview;
        public HumanBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }

        private Simulator Simulation;
        private SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        private GameMenu MenuGeneral;
        private LevelStartedAnnunciation LevelStartedAnnunciation;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        private AdvancedView AdvancedView;
        private PlayerLives PlayerLives;
        private CelestialBodyMenu MenuCelestialBody;
        private PowerUpsMenu MenuPowerUps;
        private TurretMenu MenuTurret;
        private WorldMenu MenuDemo;
        private FinalSolutionPreview FinalSolutionPreview;
        private PathPreview PathPreviewing;
        private Cursor Cursor;
        private Cursor Crosshair;
        private bool PowerUpInputMode;
        private bool PowerUpFinalSolution;
        private TheResistance GamePausedResistance;


        public GUIController(Simulator simulation)
        {
            Simulation = simulation;

            MenuGeneral = new GameMenu(Simulation, new Vector3(400, -260, 0));
            MenuPowerUps = new PowerUpsMenu(Simulation, new Vector3(-550, 200, 0), Preferences.PrioriteGUIPanneauGeneral + 0.03f);
        }


        public void Initialize()
        {
            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulation);

            Cursor = new Cursor(Simulation.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral);
            Crosshair = new Cursor(Simulation.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, "crosshairRailGun", false);
            MenuTurret = new TurretMenu(Simulation, Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            MenuCelestialBody = new CelestialBodyMenu(Simulation, Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            
            MenuDemo = new WorldMenu(Simulation, Preferences.PrioriteGUIPanneauCorpsCeleste - 0.01f);
            FinalSolutionPreview = new FinalSolutionPreview(Simulation);
            PowerUpInputMode = false;
            GamePausedResistance = new TheResistance(Simulation);
            GamePausedResistance.Enemies = new List<Enemy>();
            GamePausedResistance.Initialize();
            GamePausedResistance.AlphaChannel = 100;
            
            LevelStartedAnnunciation = new LevelStartedAnnunciation(Simulation, Level);
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulation, CelestialBodies, Level);

            PlayerLives = new PlayerLives(Simulation, Level.CelestialBodyToProtect, new Color(255, 0, 220));
            PathPreviewing = new PathPreview(PathPreview, Path);
            MenuCelestialBody.Initialize();

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.Initialize();

            MenuGeneral.CompositionNextWave = CompositionNextWave;
            MenuGeneral.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            MenuGeneral.TimeNextWave = Waves.First.Value.StartingTime;

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;

            if (!Simulation.DemoMode)
            {
                AdvancedView = new AdvancedView(Simulation, Enemies, CelestialBodies);
            }
        }


        public SandGlass SandGlass
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
            MenuGeneral.SandGlass.Flip();
        }


        public void doCommonStashChanged(CommonStash stash)
        {
            MenuGeneral.Score = stash.TotalScore;
            MenuGeneral.Cash = stash.Cash;
        }


        public void doGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);
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
                Cursor.FadeOut();
                PowerUpInputMode = true;

                if (powerUp.Crosshair != "")
                {
                    Crosshair.SetRepresentation(powerUp.Crosshair, powerUp.CrosshairSize);
                    Crosshair.FadeIn();
                }

                if (powerUp.Type == PowerUpType.FinalSolution)
                    PowerUpFinalSolution = true;
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.NeedInput)
            {
                Cursor.FadeIn();
                PowerUpInputMode = false;

                if (powerUp.Crosshair != "")
                    Crosshair.FadeOut();

                if (powerUp.Type == PowerUpType.FinalSolution)
                {
                    PowerUpLastSolution p = (PowerUpLastSolution) powerUp;

                    if (p.GoAhead)
                        PathPreviewing.Commit();

                    PowerUpFinalSolution = false;
                }
            }
        }


        public void doWaveStarted()
        {
            MenuGeneral.TimeNextWave = double.MaxValue;
            MenuGeneral.RemainingWaves--;

            if (!Simulation.DemoMode)
                Audio.PlaySfx(@"Partie", @"sfxNouvelleVague");

            if (InfiniteWaves != null || MenuGeneral.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Wave> vagueSuivante = Waves.First;

            for (int i = 0; i < Waves.Count - MenuGeneral.RemainingWaves; i++)
                vagueSuivante = vagueSuivante.Next;

            MenuGeneral.TimeNextWave = vagueSuivante.Value.StartingTime;
        }


        public void DoPlayerMoved(SimPlayer player)
        {
            Cursor.Position = player.Position;
            Crosshair.Position = player.Position;
        }


        public void doTurretToPlaceSelected(Turret turret)
        {
            Cursor.FadeOut();
            turret.CelestialBody.ShowTurretsZone = true;
            turret.ShowRange = true;
            turret.ShowForm = true;

            foreach (var turret2 in turret.CelestialBody.Turrets)
                turret2.ShowForm = true;
        }


        public void doTurretToPlaceDeselected(Turret turret)
        {
            Cursor.FadeIn();
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
            MenuDemo.Level =
                (selection.CelestialBody != null &&
                LevelSelectedDemoMode != null) ? LevelSelectedDemoMode : null;
            MenuDemo.Action = selection.GameAction;

            SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            FinalSolutionPreview.CelestialBody =
                (PowerUpFinalSolution) ? selection.CelestialBody : null;

            if (PathPreviewing != null &&
                selection.Turret != null &&
                selection.Turret.Type == TurretType.Gravitational &&
                selection.Turret.CanSell &&
                !selection.Turret.Disabled &&
                selection.TurretOption == TurretAction.Sell)
                PathPreviewing.RemoveCelestialObject(selection.Turret.CelestialBody);
            else if (PathPreviewing != null &&
                selection.CelestialBody != null &&
                PowerUpFinalSolution &&
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

            //todo event-based
            MenuGeneral.MenuNextWave.Visible = Cursor.Active && Physics.collisionCercleRectangle(Cursor.Circle, SandGlass.Rectangle);

            if (Simulation.DemoMode)
            {
                GamePausedResistance.StartingObject = null;

                if (Main.GameInProgress != null && Main.GameInProgress.State == GameState.Paused)
                {
                    GamePausedResistance.StartingObject = Simulation.CelestialBodyPausedGame;

                    if (GamePausedResistance.StartingObject != null)
                        GamePausedResistance.Update();
                }
            }

            else
            {
                MenuGeneral.Update();
                LevelStartedAnnunciation.Update(gameTime);
                LevelEndedAnnunciation.Update(gameTime);
                PlayerLives.Update(gameTime);
                MenuPowerUps.Update();
                PathPreviewing.Update(gameTime);
            }
        }


        public void Draw()
        {
            Cursor.Draw();
            Crosshair.Draw();
            Path.Draw();
            SelectedCelestialBodyAnimation.Draw();

            if (Simulation.DemoMode)
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
            LevelStartedAnnunciation.Draw();
            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();
            MenuPowerUps.Draw();
            PathPreviewing.Draw();
            FinalSolutionPreview.Draw();

            if (PowerUpInputMode)
                return;

            MenuCelestialBody.Draw();
            MenuTurret.Draw();
        }
    }
}
