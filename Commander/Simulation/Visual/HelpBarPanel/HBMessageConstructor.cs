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
                var type = 
                    Preferences.Target == Core.Utilities.Setting.ArcadeRoyale &&
                    msg.Key == HelpBarMessageType.Toggle &&
                    inputType == InputType.KeyboardOnly ? HelpBarMessageType.QuickToggle : msg.Key;

                switch (type)
                {
                    case HelpBarMessageType.Cancel: result.Add(GenerateCancelMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Move: result.Add(GenerateMoveMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Select: result.Add(GenerateSelectMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Toggle: result.Add(GenerateToggleMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Retry: result.Add(GenerateRetryMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.QuickToggle: result.Add(GenerateAltSelectMessage(inputType, config, msg.Value)); break;
                    case HelpBarMessageType.Fire: result.Add(GenerateFireMessage(inputType, config, msg.Value)); break;
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


        public List<Image> CreateSelectImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new List<Image>() { new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.Select]) { SizeX = ImageSize } } :
                   inputType == InputType.KeyboardOnly ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Select]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Select]) { SizeX = ImageSize } };
        }


        public List<Image> CreateAltSelectImage(InputType inputType, InputConfiguration config)
        {
            return (inputType == InputType.MouseAndKeyboard || inputType == InputType.KeyboardOnly) ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.QuickToggle]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.QuickToggle]) { SizeX = ImageSize } };
        }


        public List<Image> CreateToggleImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionPrevious]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.SelectionNext]) { SizeX = ImageSize } } :
                    inputType == InputType.KeyboardOnly ?
                new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.QuickToggle]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.QuickToggle]) { SizeX = ImageSize } } :
                new List<Image>() {
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.SelectionPrevious]) { SizeX = ImageSize },
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.SelectionNext]) { SizeX = ImageSize } };
        }


        public List<Image> CreateMoveImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveUp]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveLeft]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveDown]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveRight]) { SizeX = ImageSize } } :
                    inputType == InputType.KeyboardOnly ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.MoveCursor]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.MoveCursor]) { SizeX = ImageSize } };
        }


        public List<Image> CreateCancelImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                       new List<Image>() { new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.Cancel]) { SizeX = ImageSize } } :
                   inputType == InputType.KeyboardOnly ?
                       new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Cancel]) { SizeX = ImageSize } } :
                       new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Cancel]) { SizeX = ImageSize } };
        }


        public List<Image> CreateRetryImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new List<Image>() { new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.AlternateSelect]) { SizeX = ImageSize } } :
                   inputType == InputType.KeyboardOnly ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.RetryLevel]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.RetryLevel]) { SizeX = ImageSize } };
        }


        public List<Image> CreateFireImage(InputType inputType, InputConfiguration config)
        {
            return inputType == InputType.MouseAndKeyboard ?
                new List<Image>() { new Image(config.MouseConfiguration.ToImage[config.MouseConfiguration.Fire]) { SizeX = ImageSize } } :
                   inputType == InputType.KeyboardOnly ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Fire]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.DirectionCursor]) { SizeX = ImageSize } };
        }


        public List<Image> CreateBackImage(InputType inputType, InputConfiguration config)
        {
            return (inputType == InputType.MouseAndKeyboard || inputType == InputType.KeyboardOnly) ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Back]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Back]) { SizeX = ImageSize } };
        }


        public List<Image> CreateDisconnectImage(InputType inputType, InputConfiguration config)
        {
            return (inputType == InputType.MouseAndKeyboard || inputType == InputType.KeyboardOnly) ?
                new List<Image>() { new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.Disconnect]) { SizeX = ImageSize } } :
                new List<Image>() { new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.Disconnect]) { SizeX = ImageSize } };
        }


        public List<Image> CreateZoomImage(InputType inputType, InputConfiguration config)
        {
            return (inputType == InputType.MouseAndKeyboard || inputType == InputType.KeyboardOnly) ?
                new List<Image>() {
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.ZoomIn]) { SizeX = ImageSize },
                        new Image(config.KeyboardConfiguration.ToImage[config.KeyboardConfiguration.ZoomOut]) { SizeX = ImageSize } } :
                new List<Image>() {
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.ZoomIn]) { SizeX = ImageSize },
                    new Image(config.GamepadConfiguration.ToImage[config.GamepadConfiguration.ZoomOut]) { SizeX = ImageSize } };
        }


        private KeyValuePair<string, PanelWidget> GenerateSelectMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("select", new ImageLabel(CreateSelectImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateAltSelectMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("select", new ImageLabel(CreateAltSelectImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateFireMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("fire", new ImageLabel(CreateFireImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateToggleMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("toggle", new ImageLabel(CreateToggleImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateMoveMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("move", new ImageLabel(CreateMoveImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateCancelMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("cancel", new ImageLabel(CreateCancelImage(inputType, config), GenerateLabel(message)));
        }


        private KeyValuePair<string, PanelWidget> GenerateRetryMessage(InputType inputType, InputConfiguration config, string message)
        {
            return new KeyValuePair<string, PanelWidget>("alselect", new ImageLabel(CreateRetryImage(inputType, config), GenerateLabel(message)));
        }


        private Text GenerateLabel(string message)
        {
            return new Text(message, @"Pixelite") { SizeX = LabelSize };
        }
    }
}
