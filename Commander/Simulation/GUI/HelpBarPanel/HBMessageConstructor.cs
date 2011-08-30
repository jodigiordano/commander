namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;


    class HBMessageConstructor
    {
        private InputType Input;


        public HBMessageConstructor(InputType input)
        {
            Input = input;
        }


        public List<KeyValuePair<string, PanelWidget>> CreateMessage(HelpBarMessageType type, string label)
        {
            return CreateMessage(new List<KeyValuePair<HelpBarMessageType, string>>() { new KeyValuePair<HelpBarMessageType, string>(type, label) });
        }


        public List<KeyValuePair<string, PanelWidget>> CreateMessage(List<KeyValuePair<HelpBarMessageType, string>> subMessages)
        {
            var result = new List<KeyValuePair<string, PanelWidget>>();

            for (int i = 0; i < subMessages.Count; i++)
            {
                var msg = subMessages[i];

                switch (msg.Key)
                {
                    case HelpBarMessageType.Cancel: result.Add(GenerateCancelMessage(msg.Value)); break;
                    case HelpBarMessageType.Move: result.Add(GenerateMoveMessage(msg.Value)); break;
                    case HelpBarMessageType.Select: result.Add(GenerateSelectMessage(msg.Value)); break;
                    case HelpBarMessageType.Toggle: result.Add(GenerateToggleMessage(msg.Value)); break;
                    case HelpBarMessageType.Retry: result.Add(GenerateRetryMessage(msg.Value)); break;
                }

                if (subMessages.Count > 1 && i != subMessages.Count - 1)
                    result.Add(GenerateSeparator(i));
            }

            return result;
        }


        public List<KeyValuePair<string, PanelWidget>> CreateSelectCancelMessage(string select, string cancel)
        {
            return CreateMessage(new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, select),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, cancel)
            });
        }


        public List<KeyValuePair<string, PanelWidget>> CreateToggleSelectMessage(string toggle, string label)
        {
            return CreateMessage(new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Toggle, toggle),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, label)
            });
        }


        private KeyValuePair<string, PanelWidget> GenerateSelectMessage(string message)
        {
            return Input == InputType.Mouse ?
                new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Select]), GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Select]), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateToggleMessage(string message)
        {
            return Input == InputType.Mouse ?
                new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() {
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.SelectionPrevious]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.SelectionNext])  }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() {
                    new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionPrevious]),
                    new Image(GamePadConfiguration.ToImage[GamePadConfiguration.SelectionNext]) }, GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateMoveMessage(string message)
        {
            return Input == InputType.Mouse ?
                new KeyValuePair<string, PanelWidget>("move", new ImageLabel(new List<Image>() {
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveUp]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveLeft]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveDown]),
                        new Image(KeyboardConfiguration.ToImage[KeyboardConfiguration.MoveRight])  }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("move", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.MoveCursor]), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateCancelMessage(string message)
        {
            return Input == InputType.Mouse ?
                new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.Cancel]), GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.Cancel]), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateRetryMessage(string message)
        {
            return Input == InputType.Mouse ?
                new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(new Image(MouseConfiguration.ToImage[MouseConfiguration.AlternateSelect]), GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(new Image(GamePadConfiguration.ToImage[GamePadConfiguration.RetryLevel]), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateSeparator(int id)
        {
            return new KeyValuePair<string, PanelWidget>("separator" + id, new VerticalSeparatorWidget());
        }


        private Text GenerateLabel(string message)
        {
            return new Text(message, @"Pixelite") { SizeX = 2f };
        }
    }
}
