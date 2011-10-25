namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework.Input;


    class InputsFactory
    {
        private InputConfiguration DefaultConfiguration;
        private List<InputConfiguration> ArcadeRoyaleConfigurations;

        private HBMessageConstructor HBMessageConstructor;
        private Dictionary<InputConfiguration, Dictionary<InputType, Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>>> HBMessages;

        private int NextAvailableArcadeRoyaleConfiguration;


        public InputsFactory()
        {
            DefaultConfiguration = new InputConfiguration();
            ArcadeRoyaleConfigurations = new List<InputConfiguration>();
            HBMessageConstructor = new HBMessageConstructor();
            HBMessages = new Dictionary<InputConfiguration, Dictionary<InputType, Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>>>();
        }


        public void Initialize()
        {
            HBMessages.Clear();

            InitializeDefaultConfiguration();
            InitializeArcadeRoyaleConfigurations();
        }


        private void InitializeArcadeRoyaleConfigurations()
        {
            ArcadeRoyaleConfigurations.Clear();
            NextAvailableArcadeRoyaleConfiguration = 0;

            InputConfiguration ic;

            ic = new InputConfiguration()
            {
                KeyboardConfiguration = new KeyboardInputConfiguration()
                {
                    Disconnect = Keys.D1,
                    MoveCursor = Keys.P,
                    MoveUp = Keys.Up,
                    MoveLeft = Keys.Left,
                    MoveDown = Keys.Down,
                    MoveRight = Keys.Right,
                    RotateLeft = Keys.C,
                    RotateRight = Keys.V,
                    Fire = Keys.Z,
                    Select = Keys.C,
                    QuickToggle = Keys.V,
                    Cancel = Keys.X,
                    RetryLevel = Keys.V,
                    NextLevel = Keys.Z,
                    GoBackToWorld = Keys.X,
                    Home = Keys.D9,
                    LeftCoin = Keys.D5,
                    RightCoin = Keys.D6,
                    ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
                    {
                        { Keys.P, "ArcadeStick" },
                        { Keys.Z, "ArcadeA" },
                        { Keys.X, "ArcadeB" },
                        { Keys.C, "ArcadeC" },
                        { Keys.V, "ArcadeD" },
                        { Keys.D1, "ArcadeStart" },
                    }
                }
            };
            ArcadeRoyaleConfigurations.Add(ic);

            var hb = new Dictionary<InputType, Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>>() { { InputType.KeyboardOnly, InitializeMessages(InputType.KeyboardOnly, ic) } };
            HBMessages.Add(ic, hb);

            ic = new InputConfiguration()
            {
                KeyboardConfiguration = new KeyboardInputConfiguration()
                {
                    Disconnect = Keys.D2,
                    MoveCursor = Keys.P,
                    MoveUp = Keys.R,
                    MoveLeft = Keys.D,
                    MoveDown = Keys.F,
                    MoveRight = Keys.G,
                    RotateLeft = Keys.Q,
                    RotateRight = Keys.W,
                    Fire = Keys.A,
                    Select = Keys.Q,
                    QuickToggle = Keys.W,
                    Cancel = Keys.S,
                    RetryLevel = Keys.W,
                    NextLevel = Keys.A,
                    GoBackToWorld = Keys.S,
                    ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
                    {
                        { Keys.P, "ArcadeStick" },
                        { Keys.A, "ArcadeA" },
                        { Keys.S, "ArcadeB" },
                        { Keys.Q, "ArcadeC" },
                        { Keys.W, "ArcadeD" },
                        { Keys.D2, "ArcadeStart" },
                    }
                }
            };
            ArcadeRoyaleConfigurations.Add(ic);
            HBMessages.Add(ic, hb);

            ic = new InputConfiguration()
            {
                KeyboardConfiguration = new KeyboardInputConfiguration()
                {
                    Disconnect = Keys.D3,
                    MoveCursor = Keys.P,
                    MoveUp = Keys.O,
                    MoveLeft = Keys.K,
                    MoveDown = Keys.L,
                    MoveRight = Keys.OemSemicolon,
                    RotateLeft = Keys.Y,
                    RotateRight = Keys.U,
                    Fire = Keys.H,
                    Select = Keys.Y,
                    QuickToggle = Keys.U,
                    Cancel = Keys.J,
                    RetryLevel = Keys.U,
                    NextLevel = Keys.H,
                    GoBackToWorld = Keys.J,
                    ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
                    {
                        { Keys.P, "ArcadeStick" },
                        { Keys.H, "ArcadeA" },
                        { Keys.J, "ArcadeB" },
                        { Keys.Y, "ArcadeC" },
                        { Keys.U, "ArcadeD" },
                        { Keys.D3, "ArcadeStart" },
                    }
                }
            };
            ArcadeRoyaleConfigurations.Add(ic);
            HBMessages.Add(ic, hb);

            ic = new InputConfiguration()
            {
                KeyboardConfiguration = new KeyboardInputConfiguration()
                {
                    Disconnect = Keys.D4,
                    MoveCursor = Keys.P,
                    MoveUp = Keys.Home,
                    MoveLeft = Keys.Delete,
                    MoveDown = Keys.End,
                    MoveRight = Keys.PageDown,
                    RotateLeft = Keys.M,
                    RotateRight = Keys.OemComma,
                    Fire = Keys.B,
                    Select = Keys.M,
                    QuickToggle = Keys.OemComma,
                    Cancel = Keys.N,
                    RetryLevel = Keys.OemComma,
                    NextLevel = Keys.B,
                    GoBackToWorld = Keys.N,
                    ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
                    {
                        { Keys.P, "ArcadeStick" },
                        { Keys.B, "ArcadeA" },
                        { Keys.N, "ArcadeB" },
                        { Keys.M, "ArcadeC" },
                        { Keys.OemComma, "ArcadeD" },
                        { Keys.D4, "ArcadeStart" },
                    }
                }
            };
            ArcadeRoyaleConfigurations.Add(ic);
            HBMessages.Add(ic, hb);
        }


        private void InitializeDefaultConfiguration()
        {
            DefaultConfiguration = new InputConfiguration()
            {
                KeyboardConfiguration = new KeyboardInputConfiguration()
                {
                    AdvancedView = Keys.LeftShift,
                    Next = Keys.D,
                    Previous = Keys.A,
                    Back = Keys.Escape,
                    ChangeMusic = Keys.RightShift,
                    Debug = Keys.F1,
                    Editor = Keys.F2,
                    Disconnect = Keys.Delete,
                    Tweaking = Keys.T,
                    MoveUp = Keys.W,
                    MoveLeft = Keys.A,
                    MoveDown = Keys.S,
                    MoveRight = Keys.D,
                    QuickToggle = Keys.Space,
                    SelectionNext = Keys.E,
                    SelectionPrevious = Keys.Q,
                    Cheat1 = Keys.P,
                    ZoomIn = Keys.R,
                    ZoomOut = Keys.F,
                    ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
                    {
                        { Keys.W, "KeyW" },
                        { Keys.A, "KeyA" },
                        { Keys.S, "KeyS" },
                        { Keys.D, "KeyD" },
                        { Keys.R, "KeyR" },
                        { Keys.F, "KeyF" },
                        { Keys.E, "KeyE" },
                        { Keys.Q, "KeyQ" },
                        { Keys.Delete, "KeyDelete" }
                    }
                },

                MouseConfiguration = new MouseInputConfiguration()
                {
                    Select = MouseButton.Left,
                    Cancel = MouseButton.Right,
                    AlternateSelect = MouseButton.Middle,
                    SelectionNext = MouseButton.MiddleDown,
                    SelectionPrevious = MouseButton.MiddleUp,
                    Fire = MouseButton.Right,
                    Previous = MouseButton.Left,
                    Next = MouseButton.Right,
                    ToImage = new Dictionary<MouseButton, string>(MouseButtonComparer.Default)
                    {
                        { MouseButton.Left, "MouseLeft" },
                        { MouseButton.Right, "MouseRight" },
                        { MouseButton.Middle, "MouseMiddle" },
                        { MouseButton.MiddleUp, "MouseMiddleRoll" },
                        { MouseButton.MiddleDown, "MouseMiddleRoll" }
                    }
                },

                GamepadConfiguration = new GamepadInputConfiguration()
                {
                    Back = Buttons.Start,
                    Cancel = Buttons.B,
                    ChangeMusic = Buttons.DPadLeft,
                    ZoomIn = Buttons.RightShoulder,
                    ZoomOut = Buttons.LeftShoulder,
                    /* Debug = Buttons.LeftShoulder,
                    Tweaking = Buttons.RightShoulder, */
                    Select = Buttons.A,
                    AlternateSelect = Buttons.X,
                    AdvancedView = Buttons.Y,
                    SelectionNext = Buttons.RightTrigger,
                    SelectionPrevious = Buttons.LeftTrigger,
                    AlternateSelectionNext = Buttons.DPadDown,
                    AlternateSelectionPrevious = Buttons.DPadUp,
                    MoveCursor = Buttons.LeftStick,
                    DirectionCursor = Buttons.RightStick,
                    Disconnect = Buttons.Back,
                    RetryLevel = Buttons.Y,
                    Cheat1 = Buttons.DPadRight,
                    ToImage = new Dictionary<Buttons, string>(ButtonsComparer.Default)
                    {
                        { Buttons.Back, "ButtonBack" },
                        { Buttons.A, "ButtonA" },
                        { Buttons.B, "ButtonB" },
                        { Buttons.X, "ButtonX" },
                        { Buttons.Y, "ButtonY" },
                        { Buttons.LeftShoulder, "BumperLeft" },
                        { Buttons.RightShoulder, "BumperRight" },
                        { Buttons.LeftStick, "ThumbstickLeft" },
                        { Buttons.RightStick, "ThumbstickRight" },
                        { Buttons.LeftTrigger, "TriggerLeft" },
                        { Buttons.RightTrigger, "TriggerRight" },
                        { Buttons.Start, "ButtonStart" }
                    }
                }
            };

            HBMessages.Add(DefaultConfiguration, new Dictionary<InputType, Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>>());
            HBMessages[DefaultConfiguration].Add(InputType.MouseAndKeyboard, InitializeMessages(InputType.MouseAndKeyboard, DefaultConfiguration));
            HBMessages[DefaultConfiguration].Add(InputType.Gamepad, InitializeMessages(InputType.Gamepad, DefaultConfiguration));
        }


        public InputConfiguration GetDefaultConfiguration()
        {
            return DefaultConfiguration;
        }


        public InputConfiguration GetNextArcadeRoyaleConfiguration()
        {
            return ArcadeRoyaleConfigurations[NextAvailableArcadeRoyaleConfiguration++];
        }


        private Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>> InitializeMessages(InputType inputType, InputConfiguration config)
        {
            var dic = new Dictionary<HelpBarMessage, List<KeyValuePair<string, PanelWidget>>>();

            dic.Add(HelpBarMessage.Select, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Select"));
            dic.Add(HelpBarMessage.StartNewCampaign, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Start a new campaign"));
            dic.Add(HelpBarMessage.CallNextWave, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Start next wave now"));
            dic.Add(HelpBarMessage.BuyTurret, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Buy a turret"));
            dic.Add(HelpBarMessage.None, new List<KeyValuePair<string, PanelWidget>>() { });
            dic.Add(HelpBarMessage.HoldToSkip, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Hold to skip"));
            dic.Add(HelpBarMessage.Cancel, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Cancel, "Cancel"));
            dic.Add(HelpBarMessage.ToggleChoices, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Toggle, "Toggle choices"));
            dic.Add(HelpBarMessage.MoveYourSpaceship, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Move, "Move your spaceship over a planet"));
            dic.Add(HelpBarMessage.WorldNewGame, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Start a new game"));
            dic.Add(HelpBarMessage.WorldWarp, HBMessageConstructor.CreateMessage(inputType, config, HelpBarMessageType.Select, "Travel to hyperspace"));

            dic.Add(HelpBarMessage.ToggleChoicesSelect, HBMessageConstructor.CreateToggleSelectMessage(inputType, config, "Toggle choices", "Select"));
            dic.Add(HelpBarMessage.WorldToggleNewGame, HBMessageConstructor.CreateToggleSelectMessage(inputType, config, "Toggle choices", "Start a new game"));
            dic.Add(HelpBarMessage.WorldToggleResume, HBMessageConstructor.CreateToggleSelectMessage(inputType, config, "Toggle choices", "Resume game"));
            dic.Add(HelpBarMessage.GameLost, HBMessageConstructor.CreateSelectCancelMessage(inputType, config, "Retry", "Go back to galaxy"));

            dic.Add(HelpBarMessage.InstallTurret, HBMessageConstructor.CreateMessage(inputType, config, new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Move, "Move turret"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, "Install turret"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, "Cancel")
            }));

            dic.Add(HelpBarMessage.GameWon, HBMessageConstructor.CreateMessage(inputType, config, new List<KeyValuePair<HelpBarMessageType, string>>()
            {
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Select, "Next Level"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Retry, "Retry level"),
                new KeyValuePair<HelpBarMessageType, string>(HelpBarMessageType.Cancel, "Go back to galaxy")
            }));

            return dic;
        }


        public List<KeyValuePair<string, PanelWidget>> GetHBMessage(Commander.Player p, HelpBarMessage message)
        {
            return HBMessages[p.InputConfiguration][p.InputType][message];
        }
    }
}
