namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PausedGameController
    {
        public OptionsPanel OptionsPanel;
        public PausePanel PausePanel;

        public event NoneHandler PanelOpened;
        public event NoneHandler PanelClosed;

        private Simulator Simulator;
        private Dictionary<PausePlayer, GUIPausePlayer> Players;


        public PausedGameController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<PausePlayer, GUIPausePlayer>();


            OptionsPanel = new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 300), VisualPriorities.Default.OptionsPanel, Color.White) { Visible = false };
            PausePanel = new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.PausePanel, Color.White) { Visible = false };
        }


        public void Initialize()
        {
            Players.Clear();

            OptionsPanel.Initialize();
            PausePanel.Initialize();
        }


        public bool IsPanelVisible
        {
            get { return PausePanel.Visible || OptionsPanel.Visible; }
        }


        public void HidePanels()
        {
            bool panelOpened = OptionsPanel.Visible || PausePanel.Visible;

            if (OptionsPanel.Visible)
                OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);

            if (PausePanel.Visible)
                PausePanel.Fade(PausePanel.Alpha, 0, 500);

            foreach (var p in Players.Values)
                p.Cursor.FadeOut();

            if (panelOpened)
                PanelClosed();
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
            OptionsPanel.Fade(OptionsPanel.Alpha, 255, 500);

            PanelOpened();
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
            OptionsPanel.Fade(OptionsPanel.Alpha, 0, 500);

            PanelOpened();
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


        public void DoPausePlayerMoved(PausePlayer p)
        {
            var player = Players[p];

            player.Cursor.Position = p.Position;
            player.Cursor.Direction = p.Direction;
        }


        public void Update()
        {
            Simulator.CanSelectCelestialBodies = !IsPanelVisible;
        }


        public void Draw()
        {
            OptionsPanel.Draw();

            foreach (var p in Players.Values)
                p.Draw();

            if (Simulator.DemoMode)
                return;

            PausePanel.Draw();
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
