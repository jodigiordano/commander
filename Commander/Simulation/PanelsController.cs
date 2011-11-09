namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PanelsController
    {
        public event NoneHandler PanelOpened;
        public event NoneHandler PanelClosed;

        private Simulator Simulator;

        private Dictionary<PanelType, Panel> Panels;


        public PanelsController(Simulator simulator)
        {
            Simulator = simulator;

            Panels = new Dictionary<PanelType, Panel>(PanelTypeComparer.Default);
        }


        public void Initialize()
        {
            Panels.Clear();

            Panels.Add(PanelType.Credits, new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 550), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.GeneralNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.General, "What's up at Ephemere Games"));
            Panels.Add(PanelType.UpdatesNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.Updates, "You've just been updated!"));
            Panels.Add(PanelType.Options, new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 400), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Pause, new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Controls, new ControlsPanel(Simulator.Scene, Vector3.Zero, new Vector2(600, 700), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Help, new HelpPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 600), VisualPriorities.Default.Panel, Color.White));
            
            Panels.Add(PanelType.Login, new LoginPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.Register, new RegisterPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.JumpToWorld, new JumpToWorldPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.VirtualKeyboard, new VirtualKeyboardPanel(Simulator.Scene, Vector3.Zero));

            foreach (var p in Panels.Values)
            {
                p.Visible = false;
                p.Initialize();
                p.CloseButtonHandler = DoPanelClosed;
                p.VirtualKeyboardAsked += new PanelTextBoxStringHandler(DoVirtualKeyboardAsked);
                p.KeyboardAsked += new NoneHandler(DoKeyboardAsked);
                p.KeyboardClosed += new NoneHandler(DoKeyboardClosed);
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

            foreach (var p in Simulator.Data.Players.Values)
                p.SwitchToNormalMode();

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


        public void ShowPanel(PanelType type, Vector3 position)
        {
            ShowPanelPlayers();
            CloseOthersPanels(type);

            var p = Panels[type];

            p.Position = position;
            p.Open();

            Simulator.CanSelectCelestialBodies = false;

            NotifyPanelOpened();
        }


        public void DoPanelAction(SimPlayer player)
        {
            foreach (var p in Panels.Values)
                if (p.Visible)
                    p.DoClick(player.Circle);
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            if (IsPanelVisible)
                ShowPanelPlayer(p);
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            if (Simulator.Data.Players.Count == 0 && IsPanelVisible)
            {
                CloseOthersPanels(PanelType.None);
                Simulator.CanSelectCelestialBodies = true;
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

            foreach (var player in Simulator.Data.Players.Values)
            {
                // More friction on a panel choice
                if (player.SpaceshipMove.SteeringBehavior.NextMovement == Vector3.Zero)
                {
                    foreach (var p in Panels.Values)
                        if (p.Visible && player.VisualPlayer.Visible && p.DoHover(player.Circle) && p.LastHoverWidget.Sticky)
                        {
                            player.SpaceshipMove.SteeringBehavior.Friction = 0.04f;
                            break;
                        }
                }
            }
        }


        public void Draw()
        {
            foreach (var p in Panels.Values)
                p.Draw();
        }


        private void DoPanelClosed(PanelWidget widget)
        {
            if (Panels[PanelType.Options].Visible)
            {
                ((OptionsPanel) Panels[PanelType.Options]).SaveOnDisk();
            }

            if (Panels[PanelType.VirtualKeyboard].Visible)
            {
                var p = (VirtualKeyboardPanel) Panels[PanelType.VirtualKeyboard];

                p.TextBox.Value = p.Value;
                ShowPanel(p.PanelToReopenOnClose);

                return;
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


        #region Input

        private void DoVirtualKeyboardAsked(Panel panel, TextBox textbox, string title)
        {
            var vk = (VirtualKeyboardPanel) Panels[PanelType.VirtualKeyboard];

            vk.TextBox = textbox;
            vk.PanelToReopenOnClose = panel.Type;
            vk.SetTitle(title);

            ShowPanel(PanelType.VirtualKeyboard);
        }


        private void DoKeyboardAsked()
        {
            foreach (var p in Simulator.Data.Players.Values)
                p.VisualPlayer.CurrentVisual.FadeOut();
        }


        private void DoKeyboardClosed()
        {
            foreach (var p in Simulator.Data.Players.Values)
                p.VisualPlayer.CurrentVisual.FadeIn();
        }

        #endregion


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


        private void ShowPanel(PanelType type)
        {
            var p = Panels[type];

            ShowPanel(type, p.Position);
        }


        private void ShowPanelPlayers()
        {
            foreach (var p in Simulator.Data.Players.Values)
                ShowPanelPlayer(p);
        }


        private void ShowPanelPlayer(SimPlayer p)
        {
            p.SwitchToPanelMode();
        }


        private void CloseOthersPanels(PanelType type)
        {
            foreach (var p in Panels)
            {
                if (p.Key == type || !p.Value.Visible)
                    continue;

                p.Value.Close();
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
    }
}
