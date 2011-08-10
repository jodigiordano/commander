namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpBarPanel : HorizontalPanel
    {
        private Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> PredefinedMessagesXbox;
        private Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> PredefinedMessagesWindows;

        private KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> Current;
        private Scene Scene;

        public bool Active;


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
                Position -= new Vector3(0, Preferences.Xbox360DeadZoneV2.Y, 0);
                Padding = new Vector2(10 + Preferences.Xbox360DeadZoneV2.X, 0);
            }

            InitializePredefinedMessages();

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessage.None, null);

            Active = true;
        }


        public void Initialize()
        {
            HideCurrentMessage();
        }


        public void ShowMessage(HelpBarMessage message, InputType inputType)
        {
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
            if (!Active)
                return;

            base.Draw();
        }


        public override void Fade(int from, int to, double length)
        {
            base.Fade(from, to, length);
        }


        public List<KeyValuePair<string, PanelWidget>> GetPredefinedMessage(HelpBarMessage message, InputType type)
        {
            if (type == InputType.Gamepad)
                return PredefinedMessagesXbox[message];

            return PredefinedMessagesWindows[message];
        }


        private void HideCurrentMessage()
        {
            if (Current.Key == HelpBarMessage.None)
                return;

            foreach (var w in Current.Value)
                RemoveWidget(w.Key);

            Current = new KeyValuePair<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessage.None, null);
        }


        private void InitializePredefinedMessages()
        {
            PredefinedMessagesXbox = new Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessageComparer.Default)
            {
                { HelpBarMessage.Select, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Select", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.HoldToSkip, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Hold to skip", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.Cancel, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Cancel]), new Text("Cancel", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.ToggleChoices, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() { new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionPrevious]), new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionNext]) }, new Text("Toggle choices", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.MoveYourSpaceship, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("moveSpaceship", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.MoveCursor]), new Text("Move your spaceship over a planet", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.InstallTurret, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Install turret", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Cancel]), new Text("Cancel", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.CallNextWave, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("nextWave", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Call the next wave now!", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.WorldMenu, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() { new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionPrevious]), new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionNext]) }, new Text("Toggle choices", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Select", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.GameLost, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Retry", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Cancel]), new Text("Go back to the world", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.GameWon, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), new Text("Next Level", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("retry", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.RetryLevel]), new Text("Retry level", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator2", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Cancel]), new Text("Go back to the world", "Pixelite") { SizeX = 2f }))}},
            };


            PredefinedMessagesWindows = new Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>(HelpBarMessageComparer.Default)
            {
                { HelpBarMessage.Select, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Select", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.HoldToSkip, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Hold to skip", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.Cancel, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Cancel]), new Text("Cancel", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.ToggleChoices, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.SelectionPrevious]), new Text("Toggle choices", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.MoveYourSpaceship, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("moveSpaceship", new ImageLabel(new List<Image>() {
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveUp]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveLeft]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveDown]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveRight])  }, new Text("Move your spaceship over a planet", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.InstallTurret, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Install turret", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Cancel]), new Text("Cancel", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.CallNextWave, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("nextWave", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Call the next wave now!", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.WorldMenu, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.SelectionPrevious]), new Text("Toggle choices", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Select", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.GameLost, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Retry", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Cancel]), new Text("Go back to the world", "Pixelite") { SizeX = 2f }))}},

                { HelpBarMessage.GameWon, new List<KeyValuePair<string, PanelWidget>>() {
                    new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), new Text("Next Level", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator1", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("retry", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.AlternateSelect]), new Text("Retry level", "Pixelite") { SizeX = 2f })),
                    new KeyValuePair<string, PanelWidget>("separator2", new VerticalSeparatorWidget()),
                    new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Cancel]), new Text("Go back to the world", "Pixelite") { SizeX = 2f }))}},
            };
        }
    }
}
