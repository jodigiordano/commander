namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpBarPanel : HorizontalPanel
    {
        private Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> GamepadPredefinedMessages;
        private Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> MousePredefinedMessages;

        private KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> Current;
        private Scene Scene;

        private HBMessageConstructor MouseHBMessageConstructor;
        private HBMessageConstructor GamepadHBMessageConstructor;

        public bool ActivePlayers;
        public bool ActiveOptions;
        public bool ShowOnForegroundLayer;


        public HelpBarPanel(Scene scene, double visualPriority)
            : base(scene, new Vector3(0, scene.Height / 2 - 30, 0), new Vector2(scene.Width, 35), visualPriority, Color.White)
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

            InitializePredefinedMessages();
        }


        public void Initialize()
        {
            HideCurrentMessage();
        }


        public bool Active
        {
            get { return ActivePlayers && ActiveOptions; }
        }


        public void ShowMessage(HelpBarMessage message, InputType inputType)
        {
            if (Current.Key == message)
                return;

            HideCurrentMessage();

            var widgets = GetPredefinedMessage(message, inputType);

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


        public List<KeyValuePair<string, PanelWidget>> GetPredefinedMessage(HelpBarMessage message, InputType type)
        {
            if (type == InputType.Gamepad)
                return GamepadPredefinedMessages[message];

            return MousePredefinedMessages[message];
        }


        public void HideCurrentMessage()
        {
            if (Current.Key == HelpBarMessage.None)
                return;

            foreach (var w in Current.Value)
                RemoveWidget(w.Key);

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessage.None, null);
        }


        private void InitializePredefinedMessages()
        {
            MouseHBMessageConstructor = new HBMessageConstructor(InputType.Mouse);
            GamepadHBMessageConstructor = new HBMessageConstructor(InputType.Gamepad);

            GamepadPredefinedMessages = new Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessageComparer.Default);
            MousePredefinedMessages = new Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessageComparer.Default);

            InitializeMessages(InputType.Mouse);
            InitializeMessages(InputType.Gamepad);
        }


        private void InitializeMessages(InputType inputType)
        {
            var constructor = inputType == InputType.Gamepad ? GamepadHBMessageConstructor : MouseHBMessageConstructor;
            var dic = inputType == InputType.Gamepad ? GamepadPredefinedMessages : MousePredefinedMessages;

            dic.Add(HelpBarMessage.Select, constructor.CreateMessage(HelpBarMessageType.Select, "Select"));
            dic.Add(HelpBarMessage.StartNewCampaign, constructor.CreateMessage(HelpBarMessageType.Select, "Start a new campaign"));
            dic.Add(HelpBarMessage.CallNextWave, constructor.CreateMessage(HelpBarMessageType.Select, "Start next wave now"));
            dic.Add(HelpBarMessage.BuyTurret, constructor.CreateMessage(HelpBarMessageType.Select, "Buy a turret"));
            dic.Add(HelpBarMessage.None, new List<KeyValuePair<string, PanelWidget>>() { });
            dic.Add(HelpBarMessage.HoldToSkip, constructor.CreateMessage(HelpBarMessageType.Select, "Hold to skip"));
            dic.Add(HelpBarMessage.Cancel, constructor.CreateMessage(HelpBarMessageType.Cancel, "Cancel"));
            dic.Add(HelpBarMessage.ToggleChoices, constructor.CreateMessage(HelpBarMessageType.Toggle, "Toggle choices"));
            dic.Add(HelpBarMessage.MoveYourSpaceship, constructor.CreateMessage(HelpBarMessageType.Move, "Move your spaceship over a planet"));
            dic.Add(HelpBarMessage.WorldNewGame, constructor.CreateMessage(HelpBarMessageType.Select, "Start a new game"));
            dic.Add(HelpBarMessage.WorldWarp, constructor.CreateMessage(HelpBarMessageType.Select, "Travel to hyperspace"));

            dic.Add(HelpBarMessage.ToggleChoicesSelect, constructor.CreateToggleSelectMessage("Toggle choices", "Select"));
            dic.Add(HelpBarMessage.WorldToggleNewGame, constructor.CreateToggleSelectMessage("Toggle choices", "Start a new game"));
            dic.Add(HelpBarMessage.WorldToggleResume, constructor.CreateToggleSelectMessage("Toggle choices", "Resume game"));
            dic.Add(HelpBarMessage.GameLost, constructor.CreateSelectCancelMessage("Retry", "Go back to galaxy"));

            dic.Add(HelpBarMessage.InstallTurret, constructor.CreateMessage(new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Move, "Move turret"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, "Install turret"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, "Cancel")
            }));

            dic.Add(HelpBarMessage.GameWon, constructor.CreateMessage(new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, "Next Level"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Retry, "Retry level"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, "Go back to galaxy")
            }));
        }
    }
}
