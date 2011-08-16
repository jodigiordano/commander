namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PanelsController
    {
        public event NoneHandler PanelOpened;
        public event NoneHandler PanelClosed;
        public event PausePlayerHandler PausePlayerMoved;

        private Simulator Simulator;
        private Dictionary<PausePlayer, GUIPausePlayer> Players;

        public OptionsPanel OptionsPanel;
        public CreditsPanel CreditsPanel;
        private PausePanel PausePanel;
        public HelpPanel HelpPanel;
        public ControlsPanel ControlsPanel;


        public PanelsController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<PausePlayer, GUIPausePlayer>();


            CreditsPanel = new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 500), VisualPriorities.Default.CreditsPanel, Color.White) { Visible = false };
            OptionsPanel = new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 300), VisualPriorities.Default.OptionsPanel, Color.White) { Visible = false };
            PausePanel = new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.PausePanel, Color.White) { Visible = false };
            HelpPanel = new HelpPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 500), VisualPriorities.Default.HelpPanel, Color.White) { Visible = false };
            ControlsPanel = new ControlsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 500), VisualPriorities.Default.ControlsPanel, Color.White) { Visible = false };
        }


        public void Initialize()
        {
            Players.Clear();

            OptionsPanel.Initialize();
            PausePanel.Initialize();
            CreditsPanel.Initialize();
            HelpPanel.Initialize();
            ControlsPanel.Initialize();

            PausePanel.CloseButtonHandler = DoPausePanelClosed;
            OptionsPanel.CloseButtonHandler = DoOptionsPanelClosed;
            CreditsPanel.CloseButtonHandler = DoCreditsPanelClosed;
            HelpPanel.CloseButtonHandler = DoHelpPanelClosed;
            ControlsPanel.CloseButtonHandler = DoControlsPanelClosed;

            PausePanel.SetClickHandler(DoPausePanelClicked);
        }


        public bool IsPanelVisible
        {
            get { return PausePanel.Visible || OptionsPanel.Visible || CreditsPanel.Visible || HelpPanel.Visible || ControlsPanel.Visible; }
        }


        public void HidePanels()
        {
            bool panelOpened = IsPanelVisible;

            CloseOthersPanels(null);

            foreach (var p in Players.Values)
                p.Cursor.FadeOut();

            if (panelOpened)
                NotifyPanelClosed();
        }


        public void ShowCreditsPanel()
        {
            ShowPausePlayers();
            CloseOthersPanels(CreditsPanel);
            CreditsPanel.Fade(CreditsPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowOptionsPanel()
        {
            ShowPausePlayers();
            CloseOthersPanels(OptionsPanel);
            OptionsPanel.Fade(OptionsPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowPausePanel()
        {
            ShowPausePlayers();
            CloseOthersPanels(PausePanel);
            PausePanel.Fade(PausePanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowHelpPanel()
        {
            ShowPausePlayers();
            CloseOthersPanels(HelpPanel);
            HelpPanel.Fade(HelpPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowControlsPanel()
        {
            ShowPausePlayers();
            CloseOthersPanels(ControlsPanel);
            ControlsPanel.Fade(ControlsPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void DoMoveDelta(PausePlayer player, ref Vector3 delta)
        {
            if (!IsPanelVisible)
                return;

            player.Move(ref delta, MouseConfiguration.MovingSpeed);
        }


        public void DoDirectionDelta(PausePlayer player, ref Vector3 delta)
        {
            if (!IsPanelVisible)
                return;

            player.Rotate(ref delta, MouseConfiguration.RotatingSpeed);
        }


        public void DoPanelAction(PausePlayer player)
        {
            if (OptionsPanel.Visible)
                OptionsPanel.DoClick(player.Circle);
            else if (PausePanel.Visible)
                PausePanel.DoClick(player.Circle);
            else if (CreditsPanel.Visible)
                CreditsPanel.DoClick(player.Circle);
            else if (HelpPanel.Visible)
                HelpPanel.DoClick(player.Circle);
            else if (ControlsPanel.Visible)
                ControlsPanel.DoClick(player.Circle);
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            GUIPausePlayer player = new GUIPausePlayer(Simulator, p.Color, p.ImageName, p.BasePlayer.InputType);

            player.Cursor.Position = p.Position;

            Players.Add(p.PausePlayer, player);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Players.Remove(p.PausePlayer);
        }


        public void SyncPlayer(SimPlayer p)
        {
            GUIPausePlayer current;

            if (!Players.TryGetValue(p.PausePlayer, out current))
                return;

            if (current.Cursor.FrontImage.TextureName != p.ImageName || current.Cursor.Color != p.Color)
            {
                var previousAlpha = current.Cursor.Alpha;

                current.Cursor.Color = p.Color;
                current.Cursor.SetImage(p.ImageName);
                current.Cursor.Alpha = previousAlpha;
            }
        }


        public void DoGameStateChanged(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.Paused:
                    ShowPausePanel();

                    break;
                
                case GameState.Running:
                case GameState.Restart:
                case GameState.PausedToWorld:
                    HidePanels();

                    break;
            }
        }


        public void Update()
        {
            Simulator.CanSelectCelestialBodies = !IsPanelVisible;

            if (!IsPanelVisible)
                return;

            foreach (var player in Players.Keys)
            {
                // More friction on a celestial body and a turret
                if (player.SpaceshipMove.SteeringBehavior.NextMovement == Vector3.Zero)
                {
                    if (OptionsPanel.Visible && OptionsPanel.DoHover(player.Circle) && OptionsPanel.LastHoverWidget.Sticky)
                        player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;

                    if (PausePanel.Visible && PausePanel.DoHover(player.Circle) && PausePanel.LastHoverWidget.Sticky)
                        player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;

                    if (CreditsPanel.Visible && CreditsPanel.DoHover(player.Circle) && CreditsPanel.LastHoverWidget.Sticky)
                        player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;

                    if (HelpPanel.Visible && HelpPanel.DoHover(player.Circle) && HelpPanel.LastHoverWidget.Sticky)
                        player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;

                    if (ControlsPanel.Visible && ControlsPanel.DoHover(player.Circle) && ControlsPanel.LastHoverWidget.Sticky)
                        player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;
                }


                player.Update();

                Players[player].Cursor.Position = player.Position;
                Players[player].Cursor.Direction = player.Direction;

                NotifyPausePlayerMoved(player);
            }
        }


        public void Draw()
        {
            OptionsPanel.Draw();
            CreditsPanel.Draw();
            HelpPanel.Draw();
            ControlsPanel.Draw();

            foreach (var p in Players.Values)
                p.Draw();

            if (Simulator.DemoMode)
                return;

            PausePanel.Draw();
        }


        private void DoPausePanelClosed(PanelWidget widget)
        {
            Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoOptionsPanelClosed(PanelWidget widget)
        {
            OptionsPanel.SaveOnDisk();

            if (!Simulator.DemoMode)
                Simulator.ShowPausedGamePanel();
            else
                Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoHelpPanelClosed(PanelWidget widget)
        {
            if (!Simulator.DemoMode)
                Simulator.ShowPausedGamePanel();
            else
                Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoControlsPanelClosed(PanelWidget widget)
        {
            if (!Simulator.DemoMode)
                Simulator.ShowPausedGamePanel();
            else
                Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoCreditsPanelClosed(PanelWidget widget)
        {
            CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);

            HidePanels();
        }


        private void DoPausePanelClicked(PanelWidget widget)
        {
            if (widget.Name == "Help")
            {
                Simulator.ShowHelpPanel(false);
            }

            else if (widget.Name == "Controls")
            {
                Simulator.ShowControlsPanel(false);
            }

            else if (widget.Name == "Restart")
            {
                Simulator.TriggerNewGameState(GameState.Restart);
            }

            else if (widget.Name == "Options")
            {
                Simulator.ShowOptionsPanel(false);
            }

            else if (widget.Name == "GoBackToWorld")
            {
                Simulator.TriggerNewGameState(GameState.PausedToWorld);
            }

            else if (widget.Name == "Resume")
            {
                Simulator.TriggerNewGameState(GameState.Running);
            }
        }


        private void ShowPausePlayers()
        {
            foreach (var p in Players)
            {
                p.Value.Cursor.Position = p.Key.Position;
                p.Value.Cursor.Direction = p.Key.Direction;
                p.Value.Cursor.FadeIn();
            }
        }


        private void CloseOthersPanels(Panel butNotThisOne)
        {
            if (OptionsPanel != butNotThisOne && OptionsPanel.Visible)
                OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);

            if (PausePanel != butNotThisOne && PausePanel.Visible)
                PausePanel.Fade(PausePanel.Alpha, 0, 500);

            if (CreditsPanel != butNotThisOne && CreditsPanel.Visible)
                CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);

            if (HelpPanel != butNotThisOne && HelpPanel.Visible)
                HelpPanel.Fade(HelpPanel.Alpha, 0, 500);

            if (ControlsPanel != butNotThisOne && ControlsPanel.Visible)
                ControlsPanel.Fade(ControlsPanel.Alpha, 0, 500);
        }


        private void NotifyPanelOpened()
        {
            if (PanelOpened != null)
                PanelOpened();
        }


        private void NotifyPanelClosed()
        {
            if (PanelClosed != null)
                PanelClosed();
        }


        private void NotifyPausePlayerMoved(PausePlayer player)
        {
            if (PausePlayerMoved != null)
                PausePlayerMoved(player);
        }
    }
}
