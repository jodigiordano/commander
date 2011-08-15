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


        public PanelsController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<PausePlayer, GUIPausePlayer>();


            CreditsPanel = new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), VisualPriorities.Default.CreditsPanel, Color.White) { Visible = false };
            OptionsPanel = new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 300), VisualPriorities.Default.OptionsPanel, Color.White) { Visible = false };
            PausePanel = new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.PausePanel, Color.White) { Visible = false };
        }


        public void Initialize()
        {
            Players.Clear();

            OptionsPanel.Initialize();
            PausePanel.Initialize();
            CreditsPanel.Initialize();

            PausePanel.CloseButtonHandler = DoPausePanelClosed;
            OptionsPanel.CloseButtonHandler = DoOptionsPanelClosed;
            CreditsPanel.CloseButtonHandler = DoCreditsPanelClosed;

            PausePanel.SetClickHandler(DoPausePanelClicked);
        }


        public bool IsPanelVisible
        {
            get { return PausePanel.Visible || OptionsPanel.Visible || CreditsPanel.Visible; }
        }


        public void HidePanels()
        {
            bool panelOpened = IsPanelVisible;

            if (OptionsPanel.Visible)
                OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);

            if (PausePanel.Visible)
                PausePanel.Fade(PausePanel.Alpha, 0, 500);

            if (CreditsPanel.Visible)
                CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);

            foreach (var p in Players.Values)
                p.Cursor.FadeOut();

            if (panelOpened)
                NotifyPanelClosed();
        }


        public void ShowCreditsPanel()
        {
            foreach (var p in Players)
            {
                p.Value.Cursor.Position = p.Key.Position;
                p.Value.Cursor.Direction = p.Key.Direction;
                p.Value.Cursor.FadeIn();
            }

            PausePanel.Fade(PausePanel.Alpha, 0, 500);
            OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);
            CreditsPanel.Fade(CreditsPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowOptionsPanel()
        {
            foreach (var p in Players)
            {
                p.Value.Cursor.Position = p.Key.Position;
                p.Value.Cursor.Direction = p.Key.Direction;
                p.Value.Cursor.FadeIn();
            }

            PausePanel.Fade(PausePanel.Alpha, 0, 500);
            CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);
            OptionsPanel.Fade(OptionsPanel.Alpha, 255, 500);

            NotifyPanelOpened();
        }


        public void ShowPausePanel()
        {
            foreach (var p in Players)
            {
                p.Value.Cursor.Position = p.Key.Position;
                p.Value.Cursor.Direction = p.Key.Direction;
                p.Value.Cursor.FadeIn();
            }

            PausePanel.Fade(PausePanel.Alpha, 255, 500);
            CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);
            OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);

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


        private void DoCreditsPanelClosed(PanelWidget widget)
        {
            CreditsPanel.Fade(CreditsPanel.Alpha, 0, 500);

            HidePanels();
        }


        private void DoPausePanelClicked(PanelWidget widget)
        {
            if (widget.Name == "Help")
            {

            }

            else if (widget.Name == "Controls")
            {

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
