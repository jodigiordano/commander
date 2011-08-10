namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class GUIController
    {
        public List<CelestialBody> CelestialBodies;
        public List<Turret> Turrets;
        public Level Level;
        public Dictionary<string, LevelDescriptor> AvailableLevelsDemoMode;
        public List<Enemy> Enemies;
        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;
        public Path Path;
        public Path PathPreview;
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public HumanBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }
        public HelpBarPanel HelpBar;
        public CommonStash CommonStash;
        public List<Wave> ActiveWaves;

        private Simulator Simulator;
        private Dictionary<SimPlayer, GUIPlayer> Players;

        //one
        private AdvancedView AdvancedView;
        private PowerUpsMenu MenuPowerUps;
        private PathPreview PathPreviewing;
        public StartingPathMenu StartingPathMenu;

        //not player-related
        private GameMenu GameMenu;
        private LevelStartedAnnunciation LevelStartedAnnunciation;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        private PlayerLives PlayerLives;
        private CelestialBodyNearHitAnimation CelestialBodyNearHit;
        private AlienNextWaveAnimation AlienNextWaveAnimation;
        private TheResistance GamePausedResistance;

        private ContextualMenusCollisions ContextualMenusCollisions;

        private GUIPlayer GamePausedMenuPlayerCheckedIn;
        private GUIPlayer AdvancedViewCheckedIn;

        private int lastEnemiesToReleaseCount;
        

        public GUIController(Simulator simulator)
        {
            Simulator = simulator;

            StartingPathMenu = new StartingPathMenu(Simulator, VisualPriorities.Default.StartingPathMenu);
            GameMenu = new Simulation.GameMenu(Simulator, new Vector3(450, -320, 0));
            MenuPowerUps = new PowerUpsMenu(Simulator, new Vector3(-550, 200, 0), VisualPriorities.Default.PowerUpsMenu);
            Players = new Dictionary<SimPlayer, GUIPlayer>();

            if (!Simulator.DemoMode)
                AdvancedView = new AdvancedView(Simulator);

            ContextualMenusCollisions = new ContextualMenusCollisions();

            HelpBar = new HelpBarPanel(simulator.Scene, VisualPriorities.Default.HelpBar)
            {
                Alpha = 0
            };
        }


        public void Initialize()
        {
            Players.Clear();

            GamePausedResistance = new TheResistance(Simulator)
            {
                Enemies = new List<Enemy>(),
                AlphaChannel = 100
            };
            GamePausedResistance.Initialize();
            
            LevelStartedAnnunciation = new LevelStartedAnnunciation(Simulator, Level);
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulator, Path, Level);

            PlayerLives = new PlayerLives(Simulator)
            {
                CelestialBody = Level.CelestialBodyToProtect
            };

            PathPreviewing = new PathPreview(PathPreview, Path);

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.AvailablePowerUps = AvailablePowerUps;
            MenuPowerUps.Initialize();

            StartingPathMenu.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            StartingPathMenu.TimeNextWave = (InfiniteWaves == null && Waves.Count != 0) ? Waves.First.Value.StartingTime : 0;


            if (!Simulator.DemoMode)
            {
                AdvancedView.Enemies = Enemies;
                AdvancedView.CelestialBodies = CelestialBodies;
                AdvancedView.Initialize();
            }

            GamePausedMenuPlayerCheckedIn = null;
            AdvancedViewCheckedIn = null;

            HelpBar.Initialize();

            CelestialBodyNearHit = new CelestialBodyNearHitAnimation(Simulator, Enemies, Path)
            {
                CelestialBody = Level.CelestialBodyToProtect
            };

            AlienNextWaveAnimation = new AlienNextWaveAnimation(Simulator)
            {
                CelestialBody = Path.FirstCelestialBody,
                TimeNextWave = StartingPathMenu.TimeNextWave
            };

            lastEnemiesToReleaseCount = -1;
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            GUIPlayer player = new GUIPlayer(
                Simulator, AvailableTurrets, AvailableLevelsDemoMode,
                p.Color, p.ImageName, p.BasePlayer.InputType);

            player.Cursor.Position = p.Position;

            Players.Add(p, player);

            PathPreviewing.DoPlayerConnected(player);

            HelpBar.Active = Players.Count <= 1;
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            PathPreviewing.DoPlayerDisconnected(Players[p]);
            Players.Remove(p);
        }


        public void SyncNewGameMenu()
        {
            foreach (var p in Players.Values)
                p.NewGameMenu.Initialize();
        }


        public void DoObjectDestroyed(ICollidable obj)
        {

        }


        public void DoNextWaveCompositionChanged(WaveDescriptor composition)
        {
            StartingPathMenu.NextWaveComposition = composition;
        }


        public void DoCommonStashChanged(CommonStash stash)
        {
            //GameMenu.Score = stash.TotalScore;
            GameMenu.Cash = stash.Cash;
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);

            SyncNewGameMenu();
        }


        public void DoPanelOpened()
        {
            foreach (var p in Players.Values)
                if (p.Cursor.Alpha != 0)
                    p.Cursor.FadeOut();
        }


        public void DoPanelClosed()
        {
            foreach (var p in Players)
                if (p.Key.ActualSelection.TurretToPlace == null)
                    p.Value.Cursor.FadeIn();
        }


        public void DoShowAdvancedViewAsked(SimPlayer p)
        {
            var player = Players[p];

            if (AdvancedViewCheckedIn == null)
            {
                AdvancedView.Visible = true;
                AdvancedViewCheckedIn = player;
            }
        }


        public void DoHideAdvancedViewAsked(SimPlayer p)
        {
            var player = Players[p];

            if (AdvancedViewCheckedIn == player)
            {
                AdvancedView.Visible = false;
                AdvancedViewCheckedIn = null;
            }
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer p)
        {
            var player = Players[p];

            if (powerUp.NeedInput)
            {
                player.Cursor.FadeOut();
                player.PowerUpInputMode = true;

                if (powerUp.Crosshair != "")
                {
                    player.Crosshair.SetFrontImage(powerUp.Crosshair, powerUp.CrosshairSize, p.Color);
                    player.Crosshair.Alpha = 0;
                    player.Crosshair.FadeIn();
                }

                if (powerUp.Type == PowerUpType.FinalSolution)
                    player.PowerUpFinalSolution = true;
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer p)
        {
            var player = Players[p];

            if (powerUp.NeedInput)
            {
                player.Cursor.FadeIn();
                player.PowerUpInputMode = false;

                if (powerUp.Crosshair != "")
                    player.Crosshair.FadeOut();

                if (powerUp.Type == PowerUpType.FinalSolution)
                {
                    PowerUpLastSolution pls = (PowerUpLastSolution) powerUp;

                    if (pls.GoAhead)
                        PathPreviewing.Commit(player);

                    player.PowerUpFinalSolution = false;
                }
            }
        }


        public void DoWaveStarted()
        {
            StartingPathMenu.RemainingWaves--;

            if (!Simulator.DemoMode)
                Audio.PlaySfx(@"sfxNouvelleVague");

            if (InfiniteWaves != null || StartingPathMenu.RemainingWaves <= 0)
            {
                StartingPathMenu.TimeNextWave = 0;
                AlienNextWaveAnimation.TimeNextWave = 0;
                return;
            }

            //todo
            LinkedListNode<Wave> nextWave = Waves.First;

            for (int i = 0; i < Waves.Count - StartingPathMenu.RemainingWaves; i++)
                nextWave = nextWave.Next;

            StartingPathMenu.TimeNextWave = nextWave.Value.StartingTime;
            AlienNextWaveAnimation.TimeNextWave = nextWave.Value.StartingTime;
        }


        public void DoPlayerMoved(SimPlayer p)
        {
            var player = Players[p];

            player.Cursor.Position = p.Position;
            player.Cursor.Direction = p.Direction;
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

            HelpBar.ShowMessage(HelpBarMessage.InstallTurret, p.BasePlayer.InputType);
        }


        public void DoTurretToPlaceDeselected(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            player.Cursor.FadeIn();
            turret.CelestialBody.ShowTurretsZone = false;
            turret.ShowRange = false;

            HelpBar.HideMessage(HelpBarMessage.InstallTurret);
        }


        public void DoTurretBought(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit(player);

            HelpBar.HideMessage(HelpBarMessage.CelestialBodyMenu);
        }


        public void DoTurretSold(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit(player);

            HelpBar.HideMessage(HelpBarMessage.TurretMenu);
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            BeginHelpMessages(p);

            // Check in the starting path menu
            if (StartingPathMenu.CheckedIn == null && selection.CelestialBody != null && selection.CelestialBody.FirstOnPath)
            {
                StartingPathMenu.CelestialBody = selection.CelestialBody;
                StartingPathMenu.Visible = true;
                StartingPathMenu.CheckedIn = p;
                StartingPathMenu.Color = p.Color;
                StartingPathMenu.Position = p.Position;

                player.CelestialBodyMenu.Visible = false;
            }

            // Check out the starting path menu
            if (StartingPathMenu.CheckedIn == p && (selection.CelestialBody == null || !selection.CelestialBody.FirstOnPath))
            {
                StartingPathMenu.Visible = false;
                StartingPathMenu.CheckedIn = null;
            }

            // Open the celestial body menu
            if (StartingPathMenu.CheckedIn != p)
            {
                player.CelestialBodyMenu.CelestialBody = selection.CelestialBody;
                player.CelestialBodyMenu.TurretToBuy = selection.TurretToBuy;
                player.CelestialBodyMenu.Visible = selection.TurretToPlace == null && selection.CelestialBody != null;
            }


            if (selection.PowerUpToBuy != PowerUpType.None)
                MenuPowerUps.PowerUpToBuy = selection.PowerUpToBuy;

            player.TurretMenu.Turret = selection.Turret;
            player.TurretMenu.AvailableTurretOptions = selection.AvailableTurretOptions;
            player.TurretMenu.SelectedOption = selection.TurretChoice;
            player.TurretMenu.Visible = player.TurretMenu.Turret != null && !player.TurretMenu.Turret.Disabled;

            if (Simulator.WorldMode)
            {
                player.WorldMenu.CelestialBody = selection.CelestialBody;
                player.WorldMenu.PausedGameChoice = selection.PausedGameChoice;

                if (GamePausedMenuPlayerCheckedIn == null && player.WorldMenu.PausedGameMenuVisible)
                {
                    GamePausedMenuPlayerCheckedIn = player;
                    player.WorldMenu.PausedGameMenuCheckedIn = true;
                }

                else if (GamePausedMenuPlayerCheckedIn == player && selection.CelestialBody == null)
                {
                    GamePausedMenuPlayerCheckedIn = null;
                    player.WorldMenu.PausedGameMenuCheckedIn = false;
                }
            }

            if (Simulator.DemoMode)
            {
                player.NewGameMenu.CelestialBody = selection.CelestialBody;
                player.NewGameMenu.NewGameChoice = selection.NewGameChoice;
            }

            player.SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            player.FinalSolutionPreview.CelestialBody =
                (player.PowerUpFinalSolution) ? selection.CelestialBody : null;

            if (!Simulator.EditorMode || (Simulator.EditorMode && Simulator.EditorState == EditorState.Playtest))
            {
                if (PathPreviewing != null &&
                    selection.Turret != null &&
                    selection.Turret.Type == TurretType.Gravitational &&
                    selection.Turret.CanSell &&
                    !selection.Turret.Disabled &&
                    selection.TurretChoice == TurretChoice.Sell)
                    PathPreviewing.RemoveCelestialObject(player, selection.Turret.CelestialBody);
                else if (PathPreviewing != null &&
                    selection.CelestialBody != null &&
                    player.PowerUpFinalSolution &&
                    selection.CelestialBody.HasGravitationalTurret)
                    PathPreviewing.RemoveCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null &&
                    selection.TurretToBuy == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null &&
                    selection.TurretToPlace != null &&
                    selection.TurretToPlace.Type == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null)
                    PathPreviewing.RollBack(player);
            }
        }


        public void Update()
        {
            SyncStartingPathMenu();

            if (PowerUpsToBuyCount == 0)
                MenuPowerUps.PowerUpToBuy = PowerUpType.None;

            AlienNextWaveAnimation.TimeNextWave = Math.Max(0, AlienNextWaveAnimation.TimeNextWave - Preferences.TargetElapsedTimeMs);

            if (Simulator.DemoMode)
            {
                //GamePausedResistance.StartingObject = null;

                //if (Main.GameInProgress != null && Main.GameInProgress.State == GameState.Paused)
                //{
                //    GamePausedResistance.StartingObject = Simulator.CelestialBodyPausedGame;

                //    if (GamePausedResistance.StartingObject != null)
                //        GamePausedResistance.Update();
                //}
            }

            else if (Simulator.State != GameState.Paused)
            {
                StartingPathMenu.Update();
                LevelStartedAnnunciation.Update();
                LevelEndedAnnunciation.Update();
                PlayerLives.Update();
                MenuPowerUps.Update();
                PathPreviewing.Update();
                GameMenu.Update();
            }
        }


        public void Draw()
        {
            OrganizeContextualMenus();

            foreach (var player in Players.Values)
                player.Draw();

            Path.Draw();

            if (!Simulator.CutsceneMode)
                HelpBar.Draw();

            //if (Simulator.WorldMode && GamePausedResistance.StartingObject != null)
            //    GamePausedResistance.Draw();

            if (Simulator.DemoMode)
                return;

            StartingPathMenu.Draw();
            LevelStartedAnnunciation.Draw();
            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();
            MenuPowerUps.Draw();
            PathPreviewing.Draw();
            CelestialBodyNearHit.Draw();
            AlienNextWaveAnimation.Draw();
            GameMenu.Draw();
        }


        public void ShowHelpBarMessage(HelpBarMessage message, InputType type)
        {
            HelpBar.ShowMessage(message, type);
        }


        public void DoEditorCommandExecuted(EditorCommand command)
        {
            if (command.Name == "AddOrRemovePowerUp")
            {
                MenuPowerUps.Position = new Vector3(-550, 200, 0);
                MenuPowerUps.Initialize();
            }

            else if (command.Name == "Clear")
            {
                PlayerLives.CelestialBody = null;
            }

            else if (command.Name == "AddPlanet" || command.Name == "PushFirst" || command.Name == "PushLast")
            {
                PlayerLives.CelestialBody = Level.CelestialBodyToProtect;
            }

            else if (command.Name == "Remove")
            {
                PlayerLives.CelestialBody = Level.CelestialBodyToProtect;
            }

            else if (command.Name == "ToggleSize")
            {
                var c = (EditorCelestialBodyCommand) command;

                if (c.CelestialBody == PlayerLives.CelestialBody)
                {
                    PlayerLives.CelestialBody = null;
                    PlayerLives.CelestialBody = c.CelestialBody;
                }
            }

            else if (command.Name == "ShowCelestialBodiesPaths")
            {
                AdvancedView.Visible = true;
            }

            else if (command.Name == "HideCelestialBodiesPaths")
            {
                AdvancedView.Visible = false;
            }
        }


        private void SyncStartingPathMenu()
        {
            // Sync enemies to release
            int enemiesToReleaseCount = 0;

            foreach (var w in ActiveWaves)
                enemiesToReleaseCount += w.EnemiesToCreateCount;

            if (enemiesToReleaseCount != lastEnemiesToReleaseCount)
                StartingPathMenu.RemainingEnemies = enemiesToReleaseCount;

            lastEnemiesToReleaseCount = enemiesToReleaseCount;

            // Sync remaining time for next wave
            StartingPathMenu.TimeNextWave = Math.Max(0, StartingPathMenu.TimeNextWave - Preferences.TargetElapsedTimeMs);
        }


        private void BeginHelpMessages(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            if (Simulator.DemoMode)
            {
                // Main menu
                if (player.CelestialBodyMenu.CelestialBody == null && selection.CelestialBody != null)
                    HelpBar.ShowMessage(HelpBarMessage.Select, p.BasePlayer.InputType);
                else if (player.CelestialBodyMenu.CelestialBody != null && selection.CelestialBody == null)
                    HelpBar.HideMessage(HelpBarMessage.Select);

                // World Menu
                if (player.WorldMenu.PausedGameChoice == PausedGameChoice.None && selection.PausedGameChoice != PausedGameChoice.None)
                    HelpBar.ShowMessage(HelpBarMessage.WorldMenu, p.BasePlayer.InputType);
                else if (player.WorldMenu.PausedGameChoice != PausedGameChoice.None && selection.PausedGameChoice == PausedGameChoice.None)
                    HelpBar.HideMessage(HelpBarMessage.WorldMenu);
            }

            else
            {
                // Celestial Body Menu
                if (player.CelestialBodyMenu.TurretToBuy != selection.TurretToBuy && selection.TurretToBuy != TurretType.None)
                    HelpBar.ShowMessage(HelpBarMessage.CelestialBodyMenu, player.CelestialBodyMenu.GetHelpBarMessage(selection.TurretToBuy));
                else if (player.CelestialBodyMenu.TurretToBuy != selection.TurretToBuy && selection.TurretToBuy == TurretType.None)
                    HelpBar.HideMessage(HelpBarMessage.CelestialBodyMenu);

                // Turret Menu
                if (player.TurretMenu.Turret != selection.Turret && selection.Turret != null && !selection.Turret.Disabled)
                    HelpBar.ShowMessage(HelpBarMessage.TurretMenu, player.TurretMenu.GetHelpBarMessage(selection.TurretChoice));
                else if ((selection.Turret != null && selection.Turret.Disabled) || (player.TurretMenu.Turret != selection.Turret && selection.Turret == null))
                    HelpBar.HideMessage(HelpBarMessage.TurretMenu);
                else if (selection.Turret != null && !selection.Turret.Disabled && player.TurretMenu.SelectedOption != selection.TurretChoice)
                    HelpBar.ShowMessage(HelpBarMessage.TurretMenu, player.TurretMenu.GetHelpBarMessage(selection.TurretChoice));
            }
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


        private void OrganizeContextualMenus()
        {
            ContextualMenusCollisions.Menus.Clear();

            foreach (var p in Players)
            {
                p.Value.Update();

                if (p.Value.OpenedMenu != null)
                    ContextualMenusCollisions.Menus.Add(p.Value.OpenedMenu);
                else if (StartingPathMenu.CheckedIn == p.Key)
                    ContextualMenusCollisions.Menus.Add(StartingPathMenu.Menu);
            }

            ContextualMenusCollisions.Sync();
        }
    }
}
