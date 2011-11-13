namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PausePanel : VerticalPanel
    {
        private PushButton Options;
        private PushButton Help;
        private PushButton Controls;
        private PushButton Restart;
        private PushButton Resume;
        private PushButton GoBackToWorld;

        private Simulator Simulator;
        private bool ChangeStateOnClose;


        public PausePanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            SetTitle("Game Paused");

            Simulator = simulator;

            Resume = new PushButton(new Text("Resume", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoResume };
            Restart = new PushButton(new Text("Restart", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoRestart };
            GoBackToWorld = new PushButton(new Text("Go to galaxy", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoGoBackToWorld };
            Options = new PushButton(new Text("Options", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoOptions };
            Help = new PushButton(new Text("How to play", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoHelp };
            Controls = new PushButton(new Text("Controls", @"Pixelite") { SizeX = 2 }, 300) { ClickHandler = DoControls };

            AddWidget("Resume", Resume);
            AddWidget("Options", Options);
            AddWidget("Help", Help);
            AddWidget("Controls", Controls);
            AddWidget("Restart", Restart);
            AddWidget("GoBackToWorld", GoBackToWorld);

            Alpha = 0;
        }


        public override void Open()
        {
            base.Open();

            ChangeStateOnClose = true;
        }


        public override void Close()
        {
            base.Close();

            if (ChangeStateOnClose)
                Simulator.TriggerNewGameState(GameState.Running);
        }


        private void DoOptions(PanelWidget widget, Commander.Player player)
        {
            Simulator.Data.Panels["Options"].PanelToOpenOnClose = "Pause";
            PanelToOpenOnClose = "Options";
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }


        private void DoHelp(PanelWidget widget, Commander.Player player)
        {
            Simulator.Data.Panels["Help"].PanelToOpenOnClose = "Pause";
            PanelToOpenOnClose = "Help";
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }


        private void DoControls(PanelWidget widget, Commander.Player player)
        {
            Simulator.Data.Panels["Controls"].PanelToOpenOnClose = "Pause";
            PanelToOpenOnClose = "Controls";
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }


        private void DoRestart(PanelWidget widget, Commander.Player player)
        {
            Simulator.TriggerNewGameState(GameState.Restart);
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }


        private void DoGoBackToWorld(PanelWidget widget, Commander.Player player)
        {
            Simulator.TriggerNewGameState(GameState.PausedToWorld);
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }


        private void DoResume(PanelWidget widget, Commander.Player player)
        {
            Simulator.TriggerNewGameState(GameState.Running);
            ChangeStateOnClose = false;
            CloseButtonHandler(this, player);
        }
    }
}
