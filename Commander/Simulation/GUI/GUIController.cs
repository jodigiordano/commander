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
        public Dictionary<PowerUpType, bool> AvailablePowerUps;
        public Dictionary<TurretType, bool> AvailableTurrets;

        public HumanBattleship HumanBattleship { get { return MenuPowerUps.HumanBattleship; } }

        private Simulator Simulator;
        private Dictionary<SimPlayer, GUIPlayer> Players;

        //one
        private GameMenu GameMenu;
        private AdvancedView AdvancedView;
        private PowerUpsMenu MenuPowerUps;
        private PathPreview PathPreviewing;

        //not player-related
        private LevelStartedAnnunciation LevelStartedAnnunciation;
        private LevelEndedAnnunciation LevelEndedAnnunciation;
        private PlayerLives PlayerLives;
        private TheResistance GamePausedResistance;

        private ContextualMenusCollisions ContextualMenusCollisions;

        private GUIPlayer GamePausedMenuPlayerCheckedIn;
        private GUIPlayer NextWaveCheckedIn;
        private GUIPlayer AdvancedViewCheckedIn;

        private HelpBarPanel HelpBar;


        public GUIController(Simulator simulator)
        {
            Simulator = simulator;

            GameMenu = new GameMenu(Simulator, new Vector3(400, -260, 0));
            MenuPowerUps = new PowerUpsMenu(Simulator, new Vector3(-550, 200, 0), Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            Players = new Dictionary<SimPlayer, GUIPlayer>();

            if (!Simulator.DemoMode)
                AdvancedView = new AdvancedView(Simulator);

            ContextualMenusCollisions = new ContextualMenusCollisions();

            HelpBar = new HelpBarPanel(simulator);
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
            LevelEndedAnnunciation = new LevelEndedAnnunciation(Simulator, CelestialBodies, Level);

            PlayerLives = new PlayerLives(Simulator, new Color(255, 0, 220))
            {
                CelestialBody = Level.CelestialBodyToProtect
            };
            PathPreviewing = new PathPreview(PathPreview, Path);

            MenuPowerUps.Turrets = Turrets;
            MenuPowerUps.AvailablePowerUps = AvailablePowerUps;
            MenuPowerUps.Initialize();

            GameMenu.CompositionNextWave = CompositionNextWave;
            GameMenu.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            GameMenu.TimeNextWave = Waves.Count == 0 ? 0 : Waves.First.Value.StartingTime;

            if (!Simulator.DemoMode)
            {
                AdvancedView.Enemies = Enemies;
                AdvancedView.CelestialBodies = CelestialBodies;
                AdvancedView.Initialize();
            }

            GamePausedMenuPlayerCheckedIn = null;
            NextWaveCheckedIn = null;
            AdvancedViewCheckedIn = null;

            HelpBar.Initialize();
        }


        public SandGlass SandGlass
        {
            get { return GameMenu.SandGlass; }
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            GUIPlayer player = 
                new GUIPlayer(Simulator, AvailableTurrets, AvailableLevelsDemoMode, p.Color, p.Representation);

            player.Cursor.Position = p.Position;

            Players.Add(p, player);

            PathPreviewing.DoPlayerConnected(player);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            PathPreviewing.DoPlayerDisconnected(Players[p]);
            Players.Remove(p);
        }


        public void DoShowNextWaveAsked(SimPlayer p)
        {
            var player = Players[p];

            if (NextWaveCheckedIn == null)
            {
                GameMenu.MenuNextWave.Visible = true;
                GameMenu.MenuNextWave.Color = p.Color;
                NextWaveCheckedIn = player;
            }

            HelpBar.ShowMessage(HelpBarMessage.CallNextWave);
        }


        public void DoHideNextWaveAsked(SimPlayer p)
        {
            var player = Players[p];

            if (NextWaveCheckedIn == player)
            {
                GameMenu.MenuNextWave.Visible = false;
                NextWaveCheckedIn = null;
            }

            HelpBar.HideMessage(HelpBarMessage.CallNextWave);
        }


        public void DoNextWave()
        {
            GameMenu.SandGlass.Flip();
        }


        public void DoCommonStashChanged(CommonStash stash)
        {
            GameMenu.Score = stash.TotalScore;
            GameMenu.Cash = stash.Cash;
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            LevelEndedAnnunciation.DoGameStateChanged(newGameState);
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
                    player.Crosshair.SetRepresentation(powerUp.Crosshair, powerUp.CrosshairSize);
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
            GameMenu.TimeNextWave = double.MaxValue;
            GameMenu.RemainingWaves--;

            if (!Simulator.DemoMode)
                Audio.PlaySfx(@"Partie", @"sfxNouvelleVague");

            if (InfiniteWaves != null || GameMenu.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Wave> nextWave = Waves.First;

            for (int i = 0; i < Waves.Count - GameMenu.RemainingWaves; i++)
                nextWave = nextWave.Next;

            GameMenu.TimeNextWave = nextWave.Value.StartingTime;
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

            HelpBar.ShowMessage(HelpBarMessage.InstallTurret);
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

            player.CelestialBodyMenu.CelestialBody = selection.CelestialBody;
            player.CelestialBodyMenu.TurretToBuy = selection.TurretToBuy;
            player.CelestialBodyMenu.Visible = selection.TurretToPlace == null && selection.CelestialBody != null;

            if (selection.PowerUpToBuy != PowerUpType.None)
                MenuPowerUps.PowerUpToBuy = selection.PowerUpToBuy;

            player.TurretMenu.Turret = selection.Turret;
            player.TurretMenu.AvailableTurretOptions = selection.AvailableTurretOptions;
            player.TurretMenu.SelectedOption = selection.TurretChoice;
            player.TurretMenu.Visible = player.TurretMenu.Turret != null && !player.TurretMenu.Turret.Disabled;

            if (Simulator.WorldMode)
            {
                player.WorldMenu.CelestialBody = selection.CelestialBody;
                player.WorldMenu.PausedGameChoice = selection.GameChoice;

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

            player.SelectedCelestialBodyAnimation.CelestialBody = selection.CelestialBody;

            player.FinalSolutionPreview.CelestialBody =
                (player.PowerUpFinalSolution) ? selection.CelestialBody : null;

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


        public void Update(GameTime gameTime)
        {
            bool fadeGameMenu = false;

            foreach (var player in Players.Values)
            {
                var menu = player.OpenedMenu;

                if (menu == null)
                    continue;

                if (Core.Physics.Physics.RectangleRectangleCollision(menu.Bubble.Dimension, GameMenu.Rectangle))
                    fadeGameMenu = true;
            }

            if (fadeGameMenu)
                GameMenu.FadeOut(100, 250);
            else
                GameMenu.FadeIn(255, 250);

            if (PowerUpsToBuyCount == 0)
                MenuPowerUps.PowerUpToBuy = PowerUpType.None;

            if (GameMenu.TimeNextWave > 0)
                GameMenu.TimeNextWave = Math.Max(0, GameMenu.TimeNextWave - gameTime.ElapsedGameTime.TotalMilliseconds);

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
                GameMenu.Update();
                LevelStartedAnnunciation.Update(gameTime);
                LevelEndedAnnunciation.Update(gameTime);
                PlayerLives.Update(gameTime);
                MenuPowerUps.Update();
                PathPreviewing.Update(gameTime);
            }
        }


        public void Draw()
        {
            OrganizeContextualMenus();

            foreach (var player in Players.Values)
                player.Draw();

            Path.Draw();
            HelpBar.Draw();

            if (Simulator.WorldMode && GamePausedResistance.StartingObject != null)
                GamePausedResistance.Draw();

            if (Simulator.DemoMode)
                return;

            GameMenu.Draw();
            LevelStartedAnnunciation.Draw();
            LevelEndedAnnunciation.Draw();
            AdvancedView.Draw();
            PlayerLives.Draw();
            MenuPowerUps.Draw();
            PathPreviewing.Draw();
        }


        public void ShowHelpBarMessage(HelpBarMessage message)
        {
            HelpBar.ShowMessage(message);
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


        private void BeginHelpMessages(SimPlayer p)
        {
            var selection = p.ActualSelection;
            var player = Players[p];

            if (Simulator.DemoMode)
            {
                // Main menu
                if (player.CelestialBodyMenu.CelestialBody == null && selection.CelestialBody != null)
                    HelpBar.ShowMessage(HelpBarMessage.Select);
                else if (player.CelestialBodyMenu.CelestialBody != null && selection.CelestialBody == null)
                    HelpBar.HideMessage(HelpBarMessage.Select);

                // World Menu
                if (player.WorldMenu.PausedGameChoice == PausedGameChoice.None && selection.GameChoice != PausedGameChoice.None)
                    HelpBar.ShowMessage(HelpBarMessage.WorldMenu);
                else if (player.WorldMenu.PausedGameChoice != PausedGameChoice.None && selection.GameChoice == PausedGameChoice.None)
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

            foreach (var p in Players.Values)
            {
                p.Update();

                if (p.OpenedMenu != null)
                    ContextualMenusCollisions.Menus.Add(p.OpenedMenu);
            }

            ContextualMenusCollisions.Sync();
        }
    }
}
