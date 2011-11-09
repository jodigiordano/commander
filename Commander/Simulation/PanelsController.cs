namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PanelsController
    {
        public event PanelTypeHandler PanelOpened;
        public event PanelTypeHandler PanelClosed;

        private Simulator Simulator;


        public PanelsController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            foreach (var p in Simulator.Data.Panels.Values)
            {
                p.Visible = false;
                p.Initialize();
                p.CloseButtonHandler = DoPanelClosed;
                p.VirtualKeyboardAsked += new PanelTextBoxStringHandler(DoVirtualKeyboardAsked);
                p.KeyboardAsked += new NoneHandler(DoKeyboardAsked);
                p.KeyboardClosed += new NoneHandler(DoKeyboardClosed);
            }

            Simulator.Data.Panels[PanelType.Pause].SetClickHandler(DoPausePanelClicked);
        }


        private PanelType OpenedPanel
        {
            get
            {
                foreach (var p in Simulator.Data.Panels)
                    if (p.Value.Visible)
                        return p.Key;

                return PanelType.None;
            }
        }


        public bool IsPanelVisible
        {
            get
            {
                return OpenedPanel != PanelType.None;
            }
        }


        public void HidePanels()
        {
            PanelType type = OpenedPanel;
            bool panelOpened = type != PanelType.None;

            CloseOthersPanels(PanelType.None);

            foreach (var p in Simulator.Data.Players.Values)
                p.SwitchToNormalMode();

            if (panelOpened)
                NotifyPanelClosed(type);
        }


        public void CloseCurrentPanel()
        {
            Panel panel = null;

            foreach (var p in Simulator.Data.Panels.Values)
                if (p.Visible)
                    panel = p;

            if (panel != null)
                DoPanelClosed(panel);
        }


        public void ShowPanel(PanelType type, Vector3 position)
        {
            ShowPanelPlayers();
            CloseOthersPanels(type);

            var p = Simulator.Data.Panels[type];

            p.Position = position;
            p.Open();

            Simulator.CanSelectCelestialBodies = false;

            NotifyPanelOpened(type);
        }


        public void DoPanelAction(SimPlayer player)
        {
            foreach (var p in Simulator.Data.Panels.Values)
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
                    foreach (var p in Simulator.Data.Panels.Values)
                        if (p.Visible && player.VisualPlayer.Visible && p.DoHover(player.Circle) && p.LastHoverWidget.Sticky)
                        {
                            player.SpaceshipMove.SteeringBehavior.Friction = 0.01f;
                            break;
                        }
                }
            }
        }


        public void Draw()
        {
            foreach (var p in Simulator.Data.Panels.Values)
                p.Draw();
        }


        public void DoEditorCommandExecuted(EditorCommand c)
        {
            var command = c as EditorShowPanelCommand;

            if (command == null)
                return;

            ShowPanel(command.Panel);
        }


        private void DoPanelClosed(PanelWidget widget)
        {
            if (Simulator.Data.Panels[PanelType.Options].Visible)
            {
                ((OptionsPanel) Simulator.Data.Panels[PanelType.Options]).SaveOnDisk();
            }

            if (Simulator.Data.Panels[PanelType.VirtualKeyboard].Visible)
            {
                var p = (VirtualKeyboardPanel) Simulator.Data.Panels[PanelType.VirtualKeyboard];

                p.TextBox.Value = p.Value;
                ShowPanel(p.PanelToReopenOnClose);

                return;
            }

            if (!Simulator.DemoMode && !Simulator.Data.Panels[PanelType.Pause].Visible)
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
            var vk = (VirtualKeyboardPanel) Simulator.Data.Panels[PanelType.VirtualKeyboard];

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
            var p = Simulator.Data.Panels[type];

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
            foreach (var p in Simulator.Data.Panels)
            {
                if (p.Key == type || !p.Value.Visible)
                    continue;

                p.Value.Close();
            }
        }


        private void NotifyPanelOpened(PanelType panel)
        {
            if (PanelOpened != null)
                PanelOpened(panel);
        }


        private void NotifyPanelClosed(PanelType panel)
        {
            if (PanelClosed != null)
                PanelClosed(panel);
        }
    }
}
