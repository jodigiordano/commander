namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GUIController
    {
        public List<Turret> Turrets;
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;
        public PowerUpsBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }
        public HelpBarPanel HelpBar;
        public EnemiesData EnemiesData;

        private Simulator Simulator;

        //one
        private AdvancedView AdvancedView;
        private PowerUpsMenu MenuPowerUps;
        private PathPreview PathPreviewing;

        //not player-related
        private GameMenu GameMenu;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        private CBToProtectNearHitAnimation CelestialBodyNearHit;
        private AlienNextWaveAnimation AlienNextWaveAnimation;
        private TheResistance GamePausedResistance;
        private GameBarPanel GameBarPanel;
        private CelestialBodiesPathPreviews CelestialBodiesPathPreviews;

        private ContextualMenusCollisions ContextualMenusCollisions;

        private SimPlayer AdvancedViewCheckedIn;

        private Particle LoveEffect;

        private InfiniteWave InfiniteWaves { get { return Simulator.Data.Level.InfiniteWaves; } }
        private LinkedList<Wave> Waves { get { return Simulator.Data.Level.Waves; } }
        

        public GUIController(Simulator simulator)
        {
            Simulator = simulator;

            GameMenu = new Simulation.GameMenu(Simulator, new Vector3(450, -320, 0));
            MenuPowerUps = new PowerUpsMenu(Simulator, new Vector3(-550, 200, 0), VisualPriorities.Default.PowerUpsMenu);

            ContextualMenusCollisions = new ContextualMenusCollisions();

            HelpBar = new HelpBarPanel(
                simulator.Scene,
                Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 25 : 35,
                VisualPriorities.Foreground.HelpBar)
            {
                Alpha = 0,
                ShowOnForegroundLayer = true
            };

            GameBarPanel = new Simulation.GameBarPanel(
                Simulator,
                Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 25 : 45,
                VisualPriorities.Foreground.GameBar)
            {
                ShowOnForegroundLayer = true
            };

            CelestialBodiesPathPreviews = new CelestialBodiesPathPreviews(Simulator);
        }


        public void Initialize()
        {
            GamePausedResistance = new TheResistance(Simulator)
            {
                Enemies = new List<Enemy>(),
                AlphaChannel = 100
            };
            GamePausedResistance.Initialize();
            
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulator);

            PathPreviewing = new PathPreview(Simulator.Data.PathPreview, Simulator.Data.Path);

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.AvailablePowerUps = AvailablePowerUps;
            MenuPowerUps.Initialize();

            GameBarPanel.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;


            if (!Simulator.DemoMode)
                AdvancedView = new AdvancedView(Simulator);

            AdvancedViewCheckedIn = null;

            HelpBar.ActiveOptions = Main.Options.ShowHelpBar;
            HelpBar.Initialize();

            CelestialBodyNearHit = new CBToProtectNearHitAnimation(Simulator)
            {
                CelestialBody = Simulator.Data.Level.CelestialBodyToProtect,
                EnemiesData = EnemiesData
            };

            AlienNextWaveAnimation = new AlienNextWaveAnimation(Simulator)
            {
                Planet = Simulator.Data.Path.FirstCelestialBody,
                TimeNextWave = (Simulator.Data.Level.InfiniteWaves == null && Waves.Count != 0) ? Waves.First.Value.StartingTime : 0
            };

            GameBarPanel.Initialize();
            GameBarPanel.TimeNextWave = (Simulator.Data.Level.InfiniteWaves == null && Waves.Count != 0) ? Waves.First.Value.StartingTime : 0;

            LoveEffect = Simulator.Scene.Particles.Get("love");

            CelestialBodiesPathPreviews.Initialize();
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            PathPreviewing.DoPlayerConnected(p);
            HelpBar.ActivePlayers = Simulator.Data.Players.Count <= 1;
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            PathPreviewing.DoPlayerDisconnected(p);
            HelpBar.ActivePlayers = Simulator.Data.Players.Count <= 1;
        }


        public void DoPlayerBounced(SimPlayer p, Direction d)
        {
            float rotation = 0;
            Vector3 position = p.Position;

            switch (d)
            {
                case Direction.Left:
                    position -= new Vector3(25, 0, 0);
                    rotation = MathHelper.ToRadians(180);
                    break;
                case Direction.Right:
                    position += new Vector3(25, 0, 0);
                    break;
                case Direction.Up:
                    position -= new Vector3(0, 25, 0);
                    rotation = MathHelper.ToRadians(-90);
                    break;
                case Direction.Down:
                    position += new Vector3(0, 25, 0);
                    rotation = MathHelper.ToRadians(90);
                    break;
            }

            Simulator.Scene.Add(new ShieldHitAnimation("BorderField", position, position, p.Color, 6, VisualPriorities.Default.BattlefieldBorder, 0, 255) { Rotation = rotation });
        }


        public void TeleportPlayers(bool teleportOut)
        {
            foreach (var p in Simulator.Data.Players.Values)
            {
                if (teleportOut)
                    p.VisualPlayer.TeleportOut();
                else
                    p.VisualPlayer.TeleportIn();
            }
        }


        public void SyncNewGameMenu()
        {
            foreach (var p in Simulator.Data.Players.Values)
                p.VisualPlayer.SyncCampaignMenu();
        }


        public void DoObjectDestroyed(ICollidable obj)
        {

        }


        public void DoStartingPathCollision(Bullet b, CelestialBody cb)
        {
            if (!ShieldHitAnimation.IsHitFarEnough(cb.Position, b.Position, (int) cb.Size + 15))
                return;

            Simulator.Scene.Add(new ShieldHitAnimation(
                cb.Size == Size.Small ? "CBMask31" : cb.Size == Size.Normal ? "CBMask32" : "CBMask33",
                cb, b.Position, Colors.Default.AlienBright, 6, VisualPriorities.Default.CelestialBodyShield, (int) cb.Size + 15, 200));
        }


        public void DoNextWaveCompositionChanged(WaveDescriptor composition)
        {
            GameBarPanel.NextWaveComposition = composition;
        }


        public void DoCommonStashChanged(CommonStash stash)
        {
            //GameMenu.LevelScore = stash.TotalScore;
            GameMenu.Cash = stash.Cash;
            GameMenu.Lives = stash.Lives;

            GameBarPanel.Cash = stash.Cash;
            GameBarPanel.Lives = stash.Lives;
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);

            SyncNewGameMenu();

            HelpBar.ActivePlayers = true;
        }


        public void DoShowAdvancedViewAsked(SimPlayer p)
        {
            if (Simulator.DemoMode)
                return;

            if (AdvancedViewCheckedIn == null)
            {
                AdvancedView.Visible = true;
                AdvancedViewCheckedIn = p;
            }
        }


        public void DoHideAdvancedViewAsked(SimPlayer p)
        {
            if (Simulator.DemoMode)
                return;

            if (AdvancedViewCheckedIn == p)
            {
                AdvancedView.Visible = false;
                AdvancedViewCheckedIn = null;
            }
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer p)
        {
            var player = p.VisualPlayer;

            if (powerUp.NeedInput)
            {
                player.CurrentVisual.FadeOut();
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
            var player = p.VisualPlayer;

            if (powerUp.NeedInput)
            {
                player.CurrentVisual.FadeIn();
                player.PowerUpInputMode = false;

                if (powerUp.Crosshair != "")
                    player.Crosshair.FadeOut();

                if (powerUp.Type == PowerUpType.FinalSolution)
                {
                    PowerUpLastSolution pls = (PowerUpLastSolution) powerUp;

                    if (pls.GoAhead)
                        PathPreviewing.Commit(p);

                    player.PowerUpFinalSolution = false;
                }
            }
        }


        public void DoWaveStarted()
        {
            GameBarPanel.RemainingWaves = Simulator.Data.RemainingWaves;

            if (InfiniteWaves == null && Simulator.Data.Path.LastCelestialBody != null)
                Simulator.Scene.Add(new AlienNextWaveStartedAnimation(Simulator, Simulator.Data.Path.FirstCelestialBody));

            if (InfiniteWaves != null || GameBarPanel.RemainingWaves <= 0)
            {
                AlienNextWaveAnimation.TimeNextWave = 0;
                return;
            }

            //todo
            LinkedListNode<Wave> nextWave = Waves.First;

            for (int i = 0; i < Waves.Count - GameBarPanel.RemainingWaves; i++)
                nextWave = nextWave.Next;

            AlienNextWaveAnimation.TimeNextWave = nextWave.Value.StartingTime;
        }


        public void DoTurretToPlaceSelected(Turret turret, SimPlayer p)
        {
            var player = p.VisualPlayer;

            player.CurrentVisual.FadeOut();
            turret.CelestialBody.TurretsController.ShowTurretsZone = true;
            turret.ShowRange = true;
            turret.ShowForm = true;

            foreach (var turret2 in turret.CelestialBody.TurretsController.Turrets)
                turret2.ShowForm = true;

            HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.InstallTurret);
        }


        public void DoTurretToPlaceDeselected(Turret turret, SimPlayer p)
        {
            var player = p.VisualPlayer;

            player.CurrentVisual.FadeIn();
            turret.CelestialBody.TurretsController.ShowTurretsZone = false;
            turret.ShowRange = false;

            HelpBar.HideCurrentMessage();
        }


        public void DoTurretBought(Turret turret, SimPlayer p)
        {
            var player = p.VisualPlayer;

            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit(p);

            HelpBar.HideCurrentMessage();

            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.BuyPrice, false, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoTurretUpgraded(Turret turret, SimPlayer p)
        {
            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.BuyPrice, false, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoTurretSold(Turret turret, SimPlayer p)
        {
            var player = p.VisualPlayer;

            if (PathPreviewing != null &&
                turret.Type == TurretType.Gravitational)
                PathPreviewing.Commit(p);

            HelpBar.HideCurrentMessage();

            Simulator.Scene.Animations.Add(new TurretMoneyAnimation(turret.SellPrice, true, turret.Position, VisualPriorities.Default.TurretUpgradedAnimation));
        }


        public void DoPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            Vector3 middle = (p2.Position + p1.Position) / 2;

            LoveEffect.VisualPriority = p1.VisualPlayer.CurrentVisual.FrontImage.VisualPriority - 0.00001;
            LoveEffect.Trigger(ref middle);
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = p.VisualPlayer;

            BeginHelpMessages(p);

            // Firing
            player.CurrentVisual.ShowFiringCursor = p.Firing;

            player.SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            if (!Simulator.EditorMode || Simulator.EditorPlaytestingMode)
            {
                if (PathPreviewing != null && // preview: sell turret
                    selection.Turret != null &&
                    selection.Turret.Type == TurretType.Gravitational &&
                    selection.Turret.CanSell &&
                    !selection.Turret.Disabled &&
                    selection.TurretChoice == TurretChoice.Sell)
                {
                    PathPreviewing.RemoveCelestialObject(p, selection.Turret.CelestialBody);
                }
                else if (PathPreviewing != null && //preview: upgrade turret
                    selection.Turret != null &&
                    selection.Turret.Type == TurretType.Gravitational &&
                    selection.Turret.CanUpdate &&
                    selection.TurretChoice == TurretChoice.Update &&
                    selection.Turret.Level <= 1)
                {
                    PathPreviewing.UpgradeCelestialObject(p, selection.Turret.CelestialBody);
                }
                else if (PathPreviewing != null && //preview: final solution
                    selection.CelestialBody != null &&
                    player.PowerUpFinalSolution &&
                    selection.CelestialBody.TurretsController.HasGravitationalTurret)
                    PathPreviewing.RemoveCelestialObject(p, selection.CelestialBody);
                else if (PathPreviewing != null && //preview: buy grav. turret
                    selection.TurretToBuy == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(p, selection.CelestialBody);
                else if (PathPreviewing != null && //preview: place grav. turret
                    selection.TurretToPlace != null &&
                    selection.TurretToPlace.Type == TurretType.Gravitational)
                    PathPreviewing.AddCelestialObject(p, selection.TurretToPlace.CelestialBody);
                else if (PathPreviewing != null) //preview: rollback
                    PathPreviewing.RollBack(p);
            }
        }


        public void Update()
        {
            // Game bar panel
            GameBarPanel.TimeNextWave = Math.Max(0, GameBarPanel.TimeNextWave - Preferences.TargetElapsedTimeMs);

            if (PowerUpsToBuyCount == 0)
                MenuPowerUps.PowerUpToBuy = PowerUpType.None;

            //ShowGameBarHBMessage(); tmp: for now.

            AlienNextWaveAnimation.TimeNextWave = Math.Max(0, AlienNextWaveAnimation.TimeNextWave - Preferences.TargetElapsedTimeMs);

            if (Simulator.State != GameState.Paused)
            {
                LevelEndedAnnunciation.Update();
                MenuPowerUps.Update();
                PathPreviewing.Update();
                GameMenu.Update();
            }
        }


        private void ShowGameBarHBMessage()
        {
            if (Simulator.DemoMode)
                return;

            if (!HelpBar.Active)
            {
                HelpBar.HideCurrentMessage();
                return;
            }

            bool show = false;

            foreach (var p in Simulator.Data.Players.Values)
            {
                if (GameBarPanel.DoHover(p.Circle))
                {
                    HelpBar.ShowMessage(GameBarPanel.GetHelpBarMessage());
                    show = true;
                    break;
                }
            }

            if (!show)
                HelpBar.HideCurrentMessage();
        }


        public void Draw()
        {
            OrganizeContextualMenus();

            foreach (var player in Simulator.Data.Players.Values)
                player.Draw();

            Simulator.Data.Path.Draw();

            if (!Simulator.CutsceneMode)
                HelpBar.Draw();

            CelestialBodiesPathPreviews.Draw();

            if (Simulator.DemoMode)
                return;

            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            MenuPowerUps.Draw();
            Simulator.Data.PathPreview.Draw();
            CelestialBodyNearHit.Draw();
            AlienNextWaveAnimation.Draw();
            GameBarPanel.Draw();
        }


        public void ShowHelpBarMessage(Commander.Player p, HelpBarMessage message)
        {
            HelpBar.ShowMessage(p, message);
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
                //PlayerLives.Planet = null;
            }

            else if (command.Name == "AddPlanet" || command.Name == "AddPinkHole" || command.Name == "Remove")
            {
                CelestialBodiesPathPreviews.Sync();
            }

            else if (command.Name == "PushFirst" || command.Name == "PushLast")
            {
                //PlayerLives.Planet = Level.CelestialBodyToProtect;
            }

            else if (command.Name == "ToggleSize")
            {
                var c = (EditorCelestialBodyCommand) command;

                //if (c.Planet == PlayerLives.Planet)
                //{
                //    PlayerLives.Planet = null;
                //    PlayerLives.Planet = c.Planet;
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


        private void BeginHelpMessages(SimPlayer p)
        {
            var menu = p.ActualSelection.OpenedMenu;

            if (p.ActualSelection.OpenedMenuChanged && p.ActualSelection.OpenedMenu == null)
            {
                HelpBar.HideCurrentMessage();
            }

            if (p.ActualSelection.OpenedMenu != null)
            {
                var msg = menu.GetHelpBarMessage();

                if (msg.Count != 0)
                    HelpBar.ShowMessage(msg);

                return;
            }

            var selection = p.ActualSelection;
            var player = p.VisualPlayer;

            if (Simulator.WorldMode)
            {
                // CreatedWorld Menu
                if (selection.CelestialBody != null)
                {
                    if (selection.PausedGameChoice == PauseChoice.None && selection.CelestialBody is PinkHole)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldWarp);
                    else if (selection.PausedGameChoice == PauseChoice.None)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldNewGame);
                    else if (selection.PausedGameChoice == PauseChoice.New)
                        HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.WorldToggleNewGame);
                    else if (selection.PausedGameChoice == PauseChoice.Resume)
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
                if (selection.CelestialBody != null && WorldsFactory.IsCampaignCB(selection.CelestialBody))
                    HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.StartNewCampaign);
                else if (selection.CelestialBody != null)
                    HelpBar.ShowMessage(p.InnerPlayer, HelpBarMessage.Select);
                else
                    HelpBar.HideCurrentMessage();
            }
        }


        private int PowerUpsToBuyCount
        {
            get
            {
                int count = 0;

                foreach (var player in Simulator.Data.Players.Values)
                    if (player.ActualSelection.PowerUpToBuy != PowerUpType.None)
                        count++;

                return count;
            }
        }


        private void OrganizeContextualMenus()
        {
            ContextualMenusCollisions.Menus.Clear();

            foreach (var p in Simulator.Data.Players.Values)
            {
                var menu = p.VisualPlayer.GetOpenedMenu();

                if (menu != null)
                    ContextualMenusCollisions.Menus.Add(menu);
            }

            ContextualMenusCollisions.Sync();
        }
    }
}
