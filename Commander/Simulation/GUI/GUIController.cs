namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GUIController
    {
        public List<CelestialBody> CelestialBodies;
        public List<Turret> Turrets;
        public Level Level;
        public List<Enemy> Enemies;
        public InfiniteWave InfiniteWaves;
        public LinkedList<Wave> Waves;
        public Path Path;
        public Path PathPreview;
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public PowerUpsBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }
        public HelpBarPanel HelpBar;
        public CommonStash CommonStash;
        public List<Wave> ActiveWaves;
        public EnemiesData EnemiesData;

        private Simulator Simulator;
        private Dictionary<SimPlayer, GUIPlayer> Players;

        //one
        private AdvancedView AdvancedView;
        private PowerUpsMenu MenuPowerUps;
        private PathPreview PathPreviewing;
        public StartingPathMenu StartingPathMenu;

        //not player-related
        private GameMenu GameMenu;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        //private PlayerLivesLiteral PlayerLives;
        private CelestialBodyNearHitAnimation CelestialBodyNearHit;
        private AlienNextWaveAnimation AlienNextWaveAnimation;
        private TheResistance GamePausedResistance;
        private NextWavePreview NextWavePreview;
        private GameBarPanel GameBarPanel;

        private ContextualMenusCollisions ContextualMenusCollisions;

        private GUIPlayer GamePausedMenuPlayerCheckedIn;
        private GUIPlayer AdvancedViewCheckedIn;

        private int lastEnemiesToReleaseCount;
        private Particle LoveEffect;
        

        public GUIController(Simulator simulator)
        {
            Simulator = simulator;

            StartingPathMenu = new StartingPathMenu(Simulator, VisualPriorities.Default.StartingPathMenu);
            GameMenu = new Simulation.GameMenu(Simulator, new Vector3(450, -320, 0));
            MenuPowerUps = new PowerUpsMenu(Simulator, new Vector3(-550, 200, 0), VisualPriorities.Default.PowerUpsMenu);
            Players = new Dictionary<SimPlayer, GUIPlayer>();

            ContextualMenusCollisions = new ContextualMenusCollisions();

            HelpBar = new HelpBarPanel(simulator.Scene, VisualPriorities.Default.HelpBar)
            {
                Alpha = 0
            };

            GameBarPanel = new Simulation.GameBarPanel(Simulator, VisualPriorities.Default.GameBar);

            NextWavePreview = new NextWavePreview(simulator, VisualPriorities.Default.NextWavePreview);
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
            
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulator, Path, Level);

            PathPreviewing = new PathPreview(PathPreview, Path);

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.AvailablePowerUps = AvailablePowerUps;
            MenuPowerUps.Initialize();

            StartingPathMenu.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            StartingPathMenu.TimeNextWave = (InfiniteWaves == null && Waves.Count != 0) ? Waves.First.Value.StartingTime : 0;
            NextWavePreview.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            GameBarPanel.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;


            if (!Simulator.DemoMode)
            {
                AdvancedView = new AdvancedView(Simulator);
                AdvancedView.Enemies = Enemies;
                AdvancedView.CelestialBodies = CelestialBodies;
                AdvancedView.Initialize();
            }

            GamePausedMenuPlayerCheckedIn = null;
            AdvancedViewCheckedIn = null;

            HelpBar.ActiveOptions = Main.Options.ShowHelpBar;
            HelpBar.Initialize();

            CelestialBodyNearHit = new CelestialBodyNearHitAnimation(Simulator)
            {
                CelestialBody = Level.CelestialBodyToProtect,
                EnemiesData = EnemiesData
            };

            AlienNextWaveAnimation = new AlienNextWaveAnimation(Simulator)
            {
                CelestialBody = Path.FirstCelestialBody,
                TimeNextWave = StartingPathMenu.TimeNextWave
            };

            lastEnemiesToReleaseCount = -1;

            GameBarPanel.Initialize();

            LoveEffect = Simulator.Scene.Particles.Get("love");
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            GUIPlayer player = new GUIPlayer(
                Simulator, AvailableTurrets,
                p.Color, p.ImageName, p.InnerPlayer);

            player.Cursor.Position = p.Position;

            Players.Add(p, player);

            PathPreviewing.DoPlayerConnected(player);

            HelpBar.ActivePlayers = Players.Count <= 1;

            player.Cursor.TeleportIn();
            player.Cursor.FadeIn();
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            var player = Players[p];

            PathPreviewing.DoPlayerDisconnected(player);
            Players.Remove(p);

            player.Cursor.TeleportOut();
        }


        public void SyncPlayer(SimPlayer p)
        {
            GUIPlayer current;

            if (!Players.TryGetValue(p, out current))
                return;
            
            if (current.Cursor.FrontImage.TextureName != p.ImageName || current.Cursor.Color != p.Color)
            {
                current.Cursor.Color = p.Color;
                current.Cursor.SetImage(p.ImageName);
            }
        }


        public void TeleportPlayers(bool teleportOut)
        {
            foreach (var p in Players.Values)
            {
                if (teleportOut)
                    p.Cursor.TeleportOut();
                else
                    p.Cursor.TeleportIn();
            }
        }


        public void SyncNewGameMenu()
        {
            foreach (var p in Players.Values)
                p.NewGameMenu.Initialize();
        }


        public void DoObjectDestroyed(ICollidable obj)
        {

        }


        public void DoStartingPathCollision(Bullet b, CelestialBody cb)
        {
            Vector3 direction = b.Position - cb.Position;
            float lengthSquared = direction.LengthSquared();
            float cbLengthSquared = ((int) cb.Size + 15) * ((int) cb.Size + 15);
            direction.Normalize();
            float rotation = Core.Physics.Utilities.VectorToAngle(direction);
            Vector3 position = b.Position - direction * 30;

            if (lengthSquared >= cbLengthSquared)
                Simulator.Scene.Add(new CelestialBodyShieldHitAnimation(cb.Size, position, rotation, Colors.Default.AlienBright, VisualPriorities.Default.CelestialBodyShield));
        }


        public void DoNextWaveCompositionChanged(WaveDescriptor composition)
        {
            StartingPathMenu.NextWaveComposition = composition;
            GameBarPanel.NextWaveComposition = composition;
        }


        public void DoCommonStashChanged(CommonStash stash)
        {
            //GameMenu.Score = stash.TotalScore;
            GameMenu.Cash = stash.Cash;
            GameMenu.Lives = stash.Lives;

            GameBarPanel.Cash = stash.Cash;
            GameBarPanel.Lives = stash.Lives;
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);

            SyncNewGameMenu();
        }


        public void DoPanelOpened()
        {
            foreach (var p in Players.Values)
                if (p.Cursor.Alpha > 100)
                    p.Cursor.FadeOut(100);
        }


        public void DoPanelClosed()
        {
            foreach (var p in Players)
                if (p.Key.ActualSelection.TurretToPlace == null)
                    p.Value.Cursor.FadeIn();
        }


        public void DoShowAdvancedViewAsked(SimPlayer p)
        {
            if (Simulator.DemoMode)
                return;

            var player = Players[p];

            if (AdvancedViewCheckedIn == null)
            {
                AdvancedView.Visible = true;
                AdvancedViewCheckedIn = player;
            }
        }


        public void DoHideAdvancedViewAsked(SimPlayer p)
        {
            if (Simulator.DemoMode)
                return;

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
            NextWavePreview.RemainingWaves--;
            GameBarPanel.RemainingWaves--;

            if (InfiniteWaves == null && Path.LastCelestialBody != null)
                Simulator.Scene.Add(new AlienNextWaveStartedAnimation(Simulator, Path.FirstCelestialBody));

            if (InfiniteWaves != null || NextWavePreview.RemainingWaves <= 0)
            {
                StartingPathMenu.TimeNextWave = 0;
                AlienNextWaveAnimation.TimeNextWave = 0;
                return;
            }

            //todo
            LinkedListNode<Wave> nextWave = Waves.First;

            for (int i = 0; i < Waves.Count - NextWavePreview.RemainingWaves; i++)
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

            HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.InstallTurret);
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

            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.BuyPrice, false, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoTurretUpgraded(Turret turret, SimPlayer p)
        {
            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.BuyPrice, false, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoTurretSold(Turret turret, SimPlayer p)
        {
            var player = Players[p];

            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit(player);

            HelpBar.HideMessage(HelpBarMessage.TurretMenu);

            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.SellPrice, true, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            Vector3 middle = (p2.Position + p1.Position) / 2;

            LoveEffect.VisualPriority = Players[p1].Cursor.FrontImage.VisualPriority - 0.00001;
            LoveEffect.Trigger(ref middle);
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            BeginHelpMessages(p);

            // Firing
            player.Cursor.ShowFiringCursor = p.Firing;

            if (!Simulator.DemoMode)
            {
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
                player.WorldMenu.EditorChoice = selection.EditorWorldChoice;

                if (GamePausedMenuPlayerCheckedIn == null && player.WorldMenu.PausedGameMenuVisible)
                {
                    GamePausedMenuPlayerCheckedIn = player;
                    player.WorldMenu.MenuCheckedIn = true;
                }

                else if (GamePausedMenuPlayerCheckedIn == player && selection.CelestialBody == null)
                {
                    GamePausedMenuPlayerCheckedIn = null;
                    player.WorldMenu.MenuCheckedIn = false;
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
                if (PathPreviewing != null && // preview: sell turret
                    selection.Turret != null &&
                    selection.Turret.Type == TurretType.Gravitational &&
                    selection.Turret.CanSell &&
                    !selection.Turret.Disabled &&
                    selection.TurretChoice == TurretChoice.Sell)
                {
                    PathPreviewing.RemoveCelestialObject(player, selection.Turret.CelestialBody);
                }
                else if (PathPreviewing != null && //preview: upgrade turret
                    selection.Turret != null &&
                    selection.Turret.Type == TurretType.Gravitational &&
                    selection.Turret.CanUpdate &&
                    selection.TurretChoice == TurretChoice.Update &&
                    selection.Turret.Level <= 1)
                {
                    PathPreviewing.UpgradeCelestialObject(player, selection.Turret.CelestialBody);
                }
                else if (PathPreviewing != null && //preview: final solution
                    selection.CelestialBody != null &&
                    player.PowerUpFinalSolution &&
                    selection.CelestialBody.HasGravitationalTurret)
                    PathPreviewing.RemoveCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null && //preview: buy grav. turret
                    selection.TurretToBuy == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null && //preview: place grav. turret
                    selection.TurretToPlace != null &&
                    selection.TurretToPlace.Type == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(player, selection.CelestialBody);
                else if (PathPreviewing != null) //preview: rollback
                    PathPreviewing.RollBack(player);
            }
        }


        public void Update()
        {
            SyncStartingPathMenu();

            if (PowerUpsToBuyCount == 0)
                MenuPowerUps.PowerUpToBuy = PowerUpType.None;

            ShowGameBarHBMessage();

            AlienNextWaveAnimation.TimeNextWave = Math.Max(0, AlienNextWaveAnimation.TimeNextWave - Preferences.TargetElapsedTimeMs);

            if (Simulator.State != GameState.Paused)
            {
                StartingPathMenu.Update();
                LevelEndedAnnunciation.Update();
                MenuPowerUps.Update();
                PathPreviewing.Update();
                GameMenu.Update();
            }

            GameBarPanel.Update();
        }


        private void ShowGameBarHBMessage()
        {
            if (Simulator.DemoMode)
                return;

            if (!HelpBar.Active)
            {
                HelpBar.HideMessage(HelpBarMessage.GameBar);
                return;
            }

            bool show = false;

            foreach (var p in Players)
            {
                if (GameBarPanel.DoHover(p.Key.Circle))
                {
                    HelpBar.ShowMessage(HelpBarMessage.GameBar, GameBarPanel.GetHelpBarMessage());
                    show = true;
                    break;
                }
            }

            if (!show)
                HelpBar.HideMessage(HelpBarMessage.GameBar);
        }


        public void Draw()
        {
            OrganizeContextualMenus();

            foreach (var player in Players.Values)
                player.Draw();

            Path.Draw();

            if (!Simulator.CutsceneMode)
                HelpBar.Draw();

            if (Simulator.DemoMode)
                return;

            if (!(Simulator.EditorMode && Simulator.EditorState == EditorState.Editing))
                StartingPathMenu.Draw();

            //LevelStartedAnnunciation.Draw();
            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            //PlayerLives.Draw();
            MenuPowerUps.Draw();
            PathPreview.Draw();
            CelestialBodyNearHit.Draw();
            AlienNextWaveAnimation.Draw();
            //GameMenu.Draw();
            //NextWavePreview.Draw();
            GameBarPanel.Draw();
        }


        public void ShowHelpBarMessage(Player p, HelpBarMessage message)
        {
            HelpBar.ShowMessage((Commander.Player) p, message);
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
                //PlayerLives.CelestialBody = null;
            }

            else if (command.Name == "AddPlanet" || command.Name == "PushFirst" || command.Name == "PushLast")
            {
                //PlayerLives.CelestialBody = Level.CelestialBodyToProtect;
            }

            else if (command.Name == "Remove")
            {
                //PlayerLives.CelestialBody = Level.CelestialBodyToProtect;
            }

            else if (command.Name == "ToggleSize")
            {
                var c = (EditorCelestialBodyCommand) command;

                //if (c.CelestialBody == PlayerLives.CelestialBody)
                //{
                //    PlayerLives.CelestialBody = null;
                //    PlayerLives.CelestialBody = c.CelestialBody;
                //}
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

            StartingPathMenu.ActiveWaves = ActiveWaves.Count;

            // Sync next wave preview
            NextWavePreview.CelestialBody = Path.FirstCelestialBody;
            NextWavePreview.TimeNextWave = StartingPathMenu.TimeNextWave;

            // Game bar panel
            GameBarPanel.TimeNextWave = StartingPathMenu.TimeNextWave;
        }


        private void BeginHelpMessages(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            if (Simulator.WorldMode)
            {
                // World Menu
                if (selection.CelestialBody != null)
                {
                    if (selection.PausedGameChoice == PausedGameChoice.None && selection.CelestialBody is PinkHole)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldWarp);
                    else if (selection.PausedGameChoice == PausedGameChoice.None)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldNewGame);
                    else if (selection.PausedGameChoice == PausedGameChoice.New)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldToggleNewGame);
                    else if (selection.PausedGameChoice == PausedGameChoice.Resume)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldToggleResume);
                }

                else
                {
                    HelpBar.HideCurrentMessage();
                }
            }

            else if (Simulator.DemoMode)
            {
                // Main menu
                if (selection.CelestialBody != null && selection.CelestialBody.Name == "save the world")
                    HelpBar.ShowMessage(p.InnerPlayer, player.NewGameMenu.GetHelpBarMessage());
                else
                {
                    HelpBar.HideMessage(player.NewGameMenu.GetHelpBarMessage());

                    if (selection.CelestialBody != null)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.Select);
                    else
                        HelpBar.HideMessage(HelpBarMessage.Select);
                }
            }

            else if (!Simulator.EditorMode)
            {
                // Turret Menu
                if (selection.Turret != null && !selection.Turret.Disabled)
                    HelpBar.ShowMessage(HelpBarMessage.TurretMenu, player.TurretMenu.GetHelpBarMessage(selection.TurretChoice));
                else
                    HelpBar.HideMessage(HelpBarMessage.TurretMenu);

                // Celestial Body Menu
                if (selection.Turret == null && selection.TurretToPlace == null)
                {
                    if (selection.CelestialBody != null && selection.CelestialBody.FirstOnPath)
                        HelpBar.ShowMessage(HelpBarMessage.CallNextWave, StartingPathMenu.GetHelpBarMessage(p.InnerPlayer));
                    else if (selection.CelestialBody != null && selection.TurretToBuy != TurretType.None)
                        HelpBar.ShowMessage(HelpBarMessage.CelestialBodyMenu, player.CelestialBodyMenu.GetHelpBarMessage(selection.TurretToBuy));
                    else
                    {
                        HelpBar.HideMessage(HelpBarMessage.CallNextWave);
                        HelpBar.HideMessage(HelpBarMessage.CelestialBodyMenu);
                    }
                }
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
