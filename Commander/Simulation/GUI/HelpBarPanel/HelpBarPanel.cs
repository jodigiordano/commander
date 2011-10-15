namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpBarPanel : HorizontalPanel
    {
        private KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> Current;
        private Scene Scene;

        public bool ActivePlayers;
        public bool ActiveOptions;
        public bool ShowOnForegroundLayer;


        public HelpBarPanel(CommanderScene scene, double visualPriority)
            : base(scene, new Vector3(0, scene.CameraView.Height / 2 - 30, 0), new Vector2(scene.CameraView.Width, 35), visualPriority, Color.White)
        {
            Scene = scene;

            ShowCloseButton = false;
            ShowFrame = false;
            Padding = new Vector2(10, 0);
            BackgroundAlpha = 100;

            if (Preferences.Target == Core.Utilities.Setting.Xbox360)
            {
                Position -= new Vector3(0, Preferences.DeadZoneV2.Y, 0);
                Padding = new Vector2(10 + Preferences.DeadZoneV2.X, 0);
            }

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessage.None, null);

            ActivePlayers = true;
            ActiveOptions = true;
            ShowOnForegroundLayer = false;
        }


        public void Initialize()
        {
            HideCurrentMessage();
        }


        public bool Active
        {
            get { return ActivePlayers && ActiveOptions; }
        }


        public void ShowMessage(Commander.Player p, HelpBarMessage message)
        {
            if (Current.Key == message)
                return;

            HideCurrentMessage();

            var widgets = Main.InputsFactory.GetHBMessage(p, message);

            foreach (var m in widgets)
                AddWidget(m.Key, m.Value);

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(message, widgets);
        }


        public void ShowMessage(HelpBarMessage type, List<KeyValuePair<string, PanelWidget>> message)
        {
            HideCurrentMessage();

            foreach (var m in message)
                AddWidget(m.Key, m.Value);

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(type, message);
        }


        public void HideMessage(HelpBarMessage message)
        {
            if (Current.Key == message)
                HideCurrentMessage();
        }


        public override void Draw()
        {
            if (!ActivePlayers || !ActiveOptions)
                return;

            if (ShowOnForegroundLayer)
                Scene.BeginForeground();

            base.Draw();

            if (ShowOnForegroundLayer)
                Scene.EndForeground();
        }


        public override void Fade(int from, int to, double length)
        {
            base.Fade(from, to, length);
        }


        public List<KeyValuePair<string, PanelWidget>> GetPredefinedMessage(Commander.Player p, HelpBarMessage message)
        {
            return Main.InputsFactory.GetHBMessage(p, message);
        }


        public void HideCurrentMessage()
        {
            if (Current.Key == HelpBarMessage.None)
                return;

            foreach (var w in Current.Value)
                RemoveWidget(w.Key);

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessage.None, null);
        }
    }
}
