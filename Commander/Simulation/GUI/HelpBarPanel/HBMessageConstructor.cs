namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;


    class HBMessageConstructor
    {
        private float LabelSize;
        private float ImageSize;


        public HBMessageConstructor()
        {
            LabelSize = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 1 : 2;
            ImageSize = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 0.5f : 1;
        }


        public KeyValuePair<string, PanelWidget> CreateLabel(int id, string message)
        {
            return new KeyValuePair<string, PanelWidget>("label" + id, new Label(GenerateLabel(message)));            
        }


        public KeyValuePair<string, PanelWidget> CreateSeparator(int id)
        {
            return new KeyValuePair<string, PanelWidget>("separator" + id, new VerticalSeparatorWidget());
        }


        public List<KeyValuePair<string, PanelWidget>> CreateMessage(InputType inputType, InputConfiguration config, HelpBarMessageType messageType, string label)
        {
            return CreateMessage(inputType, config, new List<KeyValuePair<HelpBarMessageType, string>>() { new KeyValuePair<HelpBarMessageType, string>(messageType, label) });
        }


        public List<KeyValuePair<string, PanelWidget>> CreateMessage(InputType inputType, InputConfiguration config, List<KeyValuePair<HelpBarMessageType, string>> subMessages)
        {
            var result = new List<KeyValuePair<string, PanelWidget>>();

            for (int i = 0; i < subMessages.Count; i++)
            {
                var msg = subMessages[i];

                switch (msg.Key)
                {
                    case HelpBarMessageType.Cancel: result.Add(GenerateCancelMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Move: result.Add(GenerateMoveMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Select: result.Add(GenerateSelectMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Toggle: result.Add(GenerateToggleMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Retry: result.Add(GenerateRetryMessage(inputType, config, msg.Value)); break;
                }

                if (subMessages.Count > 1 && i != subMessages.Count - 1)
                    result.Add(CreateSeparator(i));
            }

            return result;
        }


        public List<KeyValuePair<string, PanelWidget>> CreateSelectCancelMessage(InputType inputType, InputConfiguration config, string select, string cancel)
        {
            return CreateMessage(inputType, config, new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, select),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, cancel)
            });
        }


        public List<KeyValuePair<string, PanelWidget>> CreateToggleSelectMessage(InputType inputType, InputConfiguration config, string toggle, string label)
        {
            return CreateMessage(inputType, config, new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Toggle, toggle),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, label)
            });
        }


        private KeyValuePair<string, PanelWidget> GenerateSelectMessage(InputType inputType, InputConfiguration config, string message)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.Select]) { SizeX = ImageSize }, GenerateLabel(message))) :
                   inputType == InputType.KeyboardOnly ?
                new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Select]) { SizeX = ImageSize }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("select", new ImageLabel(new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Select]) { SizeX = ImageSize }, GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateToggleMessage(InputType inputType, InputConfiguration config, string message)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionPrevious]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionNext]) { SizeX = ImageSize } }, GenerateLabel(message))) :
                    inputType == InputType.KeyboardOnly ?
                new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionPrevious]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionNext]) { SizeX = ImageSize } }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(new List<Image>() {
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.SelectionPrevious]) { SizeX = ImageSize },
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.SelectionNext]) { SizeX = ImageSize } }, GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateMoveMessage(InputType inputType, InputConfiguration config, string message)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new KeyValuePair<string, PanelWidget>("move", new ImageLabel(new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveUp]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveLeft]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveDown]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveRight]) { SizeX = ImageSize } }, GenerateLabel(message))) :
                    inputType == InputType.KeyboardOnly ?
                new KeyValuePair<string, PanelWidget>("move", new ImageLabel(new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveCursor]) { SizeX = ImageSize }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("move", new ImageLabel(new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.MoveCursor]) { SizeX = ImageSize }, GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateCancelMessage(InputType inputType, InputConfiguration config, string message)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.Cancel]) { SizeX = ImageSize }, GenerateLabel(message))) :
                   inputType == InputType.KeyboardOnly ?
                new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Cancel]) { SizeX = ImageSize }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Cancel]) { SizeX = ImageSize }, GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateRetryMessage(InputType inputType, InputConfiguration config, string message)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.AlternateSelect]) { SizeX = ImageSize }, GenerateLabel(message))) :
                   inputType == InputType.KeyboardOnly ?
                new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.AlternateSelect]) { SizeX = ImageSize }, GenerateLabel(message))) :
                new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.RetryLevel]) { SizeX = ImageSize }, GenerateLabel(message)));
        }


        private Text GenerateLabel(string message)
        {
            return new Text(message, @"Pixelite") { SizeX = LabelSize };
        }
    }
}
