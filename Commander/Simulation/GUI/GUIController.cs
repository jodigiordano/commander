namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class GUIController
    {
        public List<CelestialBody> CelestialBodies;
        public List<Turret> Turrets;
        public Dictionary<EnemyType, EnemyDescriptor> CompositionNextWave;
        public Level Level;
        public Dictionary<string, LevelDescriptor> AvailableLevelsDemoMode;
        public List<Enemy> Enemies;
        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;
        public Path Path;
        public Path PathPreview;
        public HumanBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;

        private Simulator Simulator;

        private Dictionary<SimPlayer, GUIPlayer> Players;


        //one
        private GameMenu MenuGeneral;
        private AdvancedView AdvancedView;
        private PowerUpsMenu MenuPowerUps;
        private PathPreview PathPreviewing;


        //not player-related
        private LevelStartedAnnunciation LevelStartedAnnunciation;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        private PlayerLives PlayerLives;
        private TheResistance GamePausedResistance;


        public GUIController(Simulator simulator)
        {
            Simulator = simulator;

            MenuGeneral = new GameMenu(Simulator, new Vector3(400, -260, 0));
            MenuPowerUps = new PowerUpsMenu(Simulator, new Vector3(-550, 200, 0), Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            Players = new Dictionary<SimPlayer, GUIPlayer>();
        }


        public void Initialize()
        {
            GamePausedResistance = new TheResistance(Simulator);
            GamePausedResistance.Enemies = new List<Enemy>();
            GamePausedResistance.Initialize();
            GamePausedResistance.AlphaChannel = 100;
            
            LevelStartedAnnunciation = new LevelStartedAnnunciation(Simulator, Level);
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulator, CelestialBodies, Level);

            PlayerLives = new PlayerLives(Simulator, Level.CelestialBodyToProtect, new Color(255, 0, 220));
            PathPreviewing = new PathPreview(PathPreview, Path);

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.AvailablePowerUps = AvailablePowerUps;
            MenuPowerUps.Initialize();

            MenuGeneral.CompositionNextWave = CompositionNextWave;
            MenuGeneral.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            MenuGeneral.TimeNextWave = Waves.First.Value.StartingTime;

            if (!Simulator.DemoMode)
                AdvancedView = new AdvancedView(Simulator, Enemies, CelestialBodies);
        }


        public SandGlass SandGlass
        {
            get { return MenuGeneral.SandGlass; }
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            Players.Add(p, new GUIPlayer(Simulator, AvailableTurrets, AvailableLevelsDemoMode));
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Players.Remove(p);
        }


        public void DoShowNextWaveAsked()
        {
            MenuGeneral.MenuNextWave.Visible = true;
        }


        public void DoHideNextWaveAsked()
        {
            MenuGeneral.MenuNextWave.Visible = false;
        }


        public void DoNextWave()
        {
            MenuGeneral.SandGlass.Flip();
        }


        public void DoCommonStashChanged(CommonStash stash)
        {
            MenuGeneral.Score = stash.TotalScore;
            MenuGeneral.Cash = stash.Cash;
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);
        }


        public void DoShowAdvancedViewAsked()
        {
            AdvancedView.Visible = true;
        }


        public void DoHideAdvancedViewAsked()
        {
            AdvancedView.Visible = false;
        }


        public void DoShowNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = true;
        }


        public void DoHideNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = false;
        }


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            var player = Players[powerUp.Owner];

            if (powerUp.NeedInput)
            {
                player.Cursor.FadeOut();
                player.PowerUpInputMode = true;

                if (powerUp.Crosshair != "")
                {
                    player.Crosshair.SetRepresentation(powerUp.Crosshair, powerUp.CrosshairSize);
                    player.Crosshair.FadeIn();
                }

                if (powerUp.Type == PowerUpType.FinalSolution)
                    player.PowerUpFinalSolution = true;
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            var player = Players[powerUp.Owner];

            if (powerUp.NeedInput)
            {
                player.Cursor.FadeIn();
                player.PowerUpInputMode = false;

                if (powerUp.Crosshair != "")
                    player.Crosshair.FadeOut();

                if (powerUp.Type == PowerUpType.FinalSolution)
                {
                    PowerUpLastSolution p = (PowerUpLastSolution) powerUp;

                    if (p.GoAhead)
                        PathPreviewing.Commit();

                    player.PowerUpFinalSolution = false;
                }
            }
        }


        public void DoWaveStarted()
        {
            MenuGeneral.TimeNextWave = double.MaxValue;
            MenuGeneral.RemainingWaves--;

            if (!Simulator.DemoMode)
                Audio.PlaySfx(@"Partie", @"sfxNouvelleVague");

            if (InfiniteWaves != null || MenuGeneral.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Wave> vagueSuivante = Waves.First;

            for (int i = 0; i < Waves.Count - MenuGeneral.RemainingWaves; i++)
                vagueSuivante = vagueSuivante.Next;

            MenuGeneral.TimeNextWave = vagueSuivante.Value.StartingTime;
        }


        public void DoPlayerMoved(SimPlayer p)
        {
            var player = Players[p];

            player.Cursor.Position = p.Position;
            player.Crosshair.Position = p.Position;
        }


        public void DoTurretToPlaceSelected(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            player.Cursor.FadeOut();
            turret.CelestialBody.ShowTurretsZone = true;
            turret.ShowRange = true;
            turret.ShowForm = true;

            foreach (var turret2 in turret.CelestialBody.Turrets)
                turret2.ShowForm = true;
        }


        public void DoTurretToPlaceDeselected(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            player.Cursor.FadeIn();
            turret.CelestialBody.ShowTurretsZone = false;
            turret.ShowRange = false;
        }


        public void DoTurretBought(Turret turret)
        {
            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit();
        }


        public void DoTurretSold(Turret turret)
        {
            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit();
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            player.MenuCelestialBody.CelestialBody = selection.CelestialBody;
            player.MenuCelestialBody.TurretToBuy = selection.TurretToBuy;
            player.MenuCelestialBody.Visible = selection.TurretToPlace == null && selection.CelestialBody != null;

            if (selection.PowerUpToBuy != PowerUpType.None)
                MenuPowerUps.PowerUpToBuy = selection.PowerUpToBuy;

            player.MenuTurret.Turret = selection.Turret;
            player.MenuTurret.AvailableTurretOptions = selection.AvailableTurretOptions;
            player.MenuTurret.SelectedOption = selection.TurretOption;

            player.WorldMenu.CelestialBody = selection.CelestialBody;
            player.WorldMenu.Action = selection.GameAction;

            player.SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            player.FinalSolutionPreview.CelestialBody =
                (player.PowerUpFinalSolution) ? selection.CelestialBody : null;

            if (PathPreviewing != null &&
                selection.Turret != null &&
                selection.Turret.Type == TurretType.Gravitational &&
                selection.Turret.CanSell &&
                !selection.Turret.Disabled &&
                selection.TurretOption == TurretAction.Sell)
                PathPreviewing.RemoveCelestialObject(selection.Turret.CelestialBody);
            else if (PathPreviewing != null &&
                selection.CelestialBody != null &&
                player.PowerUpFinalSolution &&
                selection.CelestialBody.ContientTourelleGravitationnelle)
                PathPreviewing.RemoveCelestialObject(selection.CelestialBody);
            else if (PathPreviewing != null &&
                selection.TurretToBuy == TurretType.Gravitational)
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
            if (PowerUpsToBuyCount == 0)
                MenuPowerUps.PowerUpToBuy = PowerUpType.None;

            if (MenuGeneral.TimeNextWave > 0)
                MenuGeneral.TimeNextWave = Math.Max(0, MenuGeneral.TimeNextWave - gameTime.ElapsedGameTime.TotalMilliseconds);

            if (Simulator.DemoMode)
            {
                GamePausedResistance.StartingObject = null;

                if (Main.GameInProgress != null && Main.GameInProgress.State == GameState.Paused)
                {
                    GamePausedResistance.StartingObject = Simulator.CelestialBodyPausedGame;

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
            foreach (var player in Players.Values)
                player.Draw();

            Path.Draw();

            if (Simulator.WorldMode && GamePausedResistance.StartingObject != null)
                GamePausedResistance.Draw();

            if (Simulator.DemoMode)
                return;

            MenuGeneral.Draw();
            LevelStartedAnnunciation.Draw();
            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();
            MenuPowerUps.Draw();
            PathPreviewing.Draw();
        }


        private int PowerUpsToBuyCount
        {
            get
            {
                int count = 0;

                foreach (var player in Players.Keys)
                    if (player.ActualSelection.PowerUpToBuy != PowerUpType.None)
                        count++;

                return count;
            }
        }
    }
}
