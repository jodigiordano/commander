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

        private Dictionary<PanelType, Panel> Panels;


        public PanelsController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<PausePlayer, GUIPausePlayer>();

            Panels = new Dictionary<PanelType, Panel>(PanelTypeComparer.Default);
        }


        public void Initialize()
        {
            Players.Clear();
            Panels.Clear();

            Panels.Add(PanelType.Credits, new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 550), VisualPriorities.Default.CreditsPanel, Color.White) { Visible = false });
            Panels.Add(PanelType.GeneralNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.NewsPanel, Color.White, NewsType.General, "What's up at Ephemere Games") { Visible = false });
            Panels.Add(PanelType.UpdatesNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.NewsPanel, Color.White, NewsType.Updates, "You've just been updated!") { Visible = false });
            Panels.Add(PanelType.Options, new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 400), VisualPriorities.Default.OptionsPanel, Color.White) { Visible = false });
            Panels.Add(PanelType.Pause, new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.PausePanel, Color.White) { Visible = false });
            Panels.Add(PanelType.Help, new HelpPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 600), VisualPriorities.Default.HelpPanel, Color.White) { Visible = false });
            Panels.Add(PanelType.Controls, new ControlsPanel(Simulator.Scene, Vector3.Zero, new Vector2(600, 700), VisualPriorities.Default.ControlsPanel, Color.White) { Visible = false });

            foreach (var p in Panels.Values)
            {
                p.Initialize();
                p.CloseButtonHandler = DoPanelClosed;
            }

            Panels[PanelType.Pause].SetClickHandler(DoPausePanelClicked);
        }


        public bool IsPanelVisible
        {
            get
            {
                foreach (var p in Panels.Values)
                    if (p.Visible)
                        return true;

                return false;
            }
        }


        public void HidePanels()
        {
            bool panelOpened = IsPanelVisible;

            CloseOthersPanels(PanelType.None);

            foreach (var p in Players.Values)
                p.Cursor.FadeOut();

            if (panelOpened)
                NotifyPanelClosed();
        }


        public void CloseCurrentPanel()
        {
            Panel panel = null;

            foreach (var p in Panels.Values)
                if (p.Visible)
                    panel = p;

            if (panel != null)
                DoPanelClosed(panel);
        }


        public void ShowPanel(PanelType type)
        {
            ShowPausePlayers();
            CloseOthersPanels(type);

            var p = Panels[type];

            p.Fade(p.Alpha, 255, 500);

            Simulator.CanSelectCelestialBodies = false;

            NotifyPanelOpened();
        }


        public void DoMoveDelta(SimPlayer p, ref Vector3 delta)
        {
            if (!IsPanelVisible)
                return;

            p.PausePlayer.Move(ref delta, p.InnerPlayer.MovingSpeed);
        }


        public void DoDirectionDelta(SimPlayer p, ref Vector3 delta)
        {
            if (!IsPanelVisible)
                return;

            p.PausePlayer.Rotate(ref delta, p.InnerPlayer.RotatingSpeed);
        }


        public void DoPanelAction(PausePlayer player)
        {
            foreach (var p in Panels.Values)
                if (p.Visible)
                    p.DoClick(player.Circle);
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            GUIPausePlayer player = new GUIPausePlayer(Simulator, p);

            Players.Add(p.PausePlayer, player);

            if (IsPanelVisible)
                ShowPausePlayer(player);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Players.Remove(p.PausePlayer);

            if (Players.Count == 0 && IsPanelVisible)
            {
                CloseOthersPanels(PanelType.None);
                Simulator.CanSelectCelestialBodies = true;
            }
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
                    ShowPanel(PanelType.Pause);

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
            if (!IsPanelVisible)
                return;

            foreach (var player in Players.Keys)
            {
                // More friction on a celestial body and a turret
                if (player.SpaceshipMove.SteeringBehavior.NextMovement == Vector3.Zero)
                {
                    foreach (var p in Panels.Values)
                        if (p.Visible && p.DoHover(player.Circle) && p.LastHoverWidget.Sticky)
                        {
                            player.SpaceshipMove.SteeringBehavior.Friction = 0.1f;
                            break;
                        }
                }


                player.Update();

                Players[player].Cursor.Position = player.Position;
                Players[player].Cursor.Direction = player.Direction;

                NotifyPausePlayerMoved(player);
            }
        }


        public void Draw()
        {
            foreach (var p in Panels.Values)
                p.Draw();

            foreach (var p in Players.Values)
                p.Draw();
        }


        private void DoPanelClosed(PanelWidget widget)
        {
            if (Panels[PanelType.Options].Visible)
            {
                ((OptionsPanel) Panels[PanelType.Options]).SaveOnDisk();
            }

            if (!Simulator.DemoMode && !Panels[PanelType.Pause].Visible)
            {
                ShowPanel(PanelType.Pause);
            }

            else
            {
                Simulator.TriggerNewGameState(GameState.Running);
                Simulator.CanSelectCelestialBodies = true;
            }
        }


        private void DoPausePanelClicked(PanelWidget widget)
        {
            if (widget.Name == "Help")
            {
                ShowPanel(PanelType.Help);
            }

            else if (widget.Name == "Controls")
            {
                ShowPanel(PanelType.Controls);
            }

            else if (widget.Name == "Restart")
            {
                Simulator.TriggerNewGameState(GameState.Restart);
            }

            else if (widget.Name == "Options")
            {
                ShowPanel(PanelType.Options);
            }

            else if (widget.Name == "GoBackToWorld")
            {
                Simulator.TriggerNewGameState(GameState.PausedToWorld);
                Simulator.CanSelectCelestialBodies = true;
            }

            else if (widget.Name == "Resume")
            {
                Simulator.TriggerNewGameState(GameState.Running);
                Simulator.CanSelectCelestialBodies = true;
            }
        }


        private void ShowPausePlayers()
        {
            foreach (var p in Players.Values)
                ShowPausePlayer(p);
        }


        private void ShowPausePlayer(GUIPausePlayer p)
        {
            p.Sync();
            p.Cursor.FadeIn();
        }


        private void CloseOthersPanels(PanelType type)
        {
            foreach (var p in Panels)
            {
                if (p.Key == type || !p.Value.Visible)
                    continue;

                p.Value.Fade(p.Value.Alpha, 0, 500);
            }
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
