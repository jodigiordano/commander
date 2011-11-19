namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class PanelsController
    {
        public event StringHandler PanelOpened;
        public event StringHandler PanelClosed;

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
        }


        public Panel GetOpenedPanel()
        {
            return IsPanelVisible ? Simulator.Data.Panels[OpenedPanel] : null;
        }


        private string OpenedPanel
        {
            get
            {
                foreach (var p in Simulator.Data.Panels)
                    if (p.Value.Visible)
                        return p.Key;

                return "";
            }
        }


        public bool IsPanelVisible
        {
            get
            {
                return OpenedPanel != "";
            }
        }


        public void HidePanels()
        {
            string type = OpenedPanel;
            bool panelOpened = type != "";

            CloseOthersPanels("");

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
                DoPanelClosed(panel, null);
        }


        public void ShowPanel(string type, Vector3 position)
        {
            ShowPanelPlayers();
            CloseOthersPanels(type);

            var p = Simulator.Data.Panels[type];

            Simulator.Data.Battlefield.Clamp(ref position, p.Size.X / 2, p.Size.Y / 2);
            position -= new Vector3(p.Size / 2, 0);
            
            p.Position = position;
            p.Open();

            NotifyPanelOpened(type);
        }


        public void DoPanelAction(SimPlayer player)
        {
            if (!IsPanelVisible)
                return;

            Simulator.Data.Panels[OpenedPanel].DoClick(player.InnerPlayer);
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
                CloseOthersPanels("");
            }
        }


        public void Update()
        {
            if (!IsPanelVisible)
                return;

            var openedPanel = Simulator.Data.Panels[OpenedPanel];

            foreach (var player in Simulator.Data.Players.Values)
            {
                // More friction on a panel choice
                if (player.SpaceshipMove.SteeringBehavior.NextMovement == Vector3.Zero)
                {
                    if (player.VisualPlayer.Visible && openedPanel.DoHover(player.InnerPlayer) && openedPanel.LastHoverWidget.Sticky)
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
            var command = c as EditorPanelShowCommand;

            if (command == null)
                return;

            ShowPanel(command.Panel, command.UsePosition ? command.Position : command.Owner.Position);
        }


        private void DoPanelClosed(PanelWidget widget, Commander.Player player) //player may be null
        {
            var panel = Simulator.Data.Panels[OpenedPanel];

            if (panel.PanelToOpenOnClose != "")
            {
                ShowPanel(panel.PanelToOpenOnClose, panel.Position + new Vector3(panel.Size / 2, 0));
                panel.PanelToOpenOnClose = "";
            }

            else
            {
                panel.Close();

                foreach (var p in Simulator.Data.Players.Values)
                    p.SwitchToNormalMode();
            }
        }


        #region Input

        private void DoVirtualKeyboardAsked(Panel panel, TextBox textbox, string title)
        {
            var vk = (VirtualKeyboardPanel) Simulator.Data.Panels["VirtualKeyboard"];

            vk.TextBox = textbox;
            vk.PanelToOpenOnClose = panel.Name;
            vk.SetTitle(title);

            ShowPanel("VirtualKeyboard", panel.Position);
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


        private void ShowPanelPlayers()
        {
            foreach (var p in Simulator.Data.Players.Values)
                ShowPanelPlayer(p);
        }


        private void ShowPanelPlayer(SimPlayer p)
        {
            p.SwitchToPanelMode();
        }


        private void CloseOthersPanels(string type)
        {
            foreach (var p in Simulator.Data.Panels)
            {
                if (p.Key == type || !p.Value.Visible)
                    continue;

                p.Value.Close();
            }
        }


        private void NotifyPanelOpened(string panel)
        {
            if (PanelOpened != null)
                PanelOpened(panel);
        }


        private void NotifyPanelClosed(string panel)
        {
            if (PanelClosed != null)
                PanelClosed(panel);
        }
    }
}
