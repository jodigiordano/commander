namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using MultiInput;

    class InputController
    {
        private bool Active;

        private Dictionary<PlayerIndex, Dictionary<Keys, bool>> KeysPressed;
        private Dictionary<PlayerIndex, Dictionary<Keys, bool>> KeysPressedOnce;
        private Dictionary<PlayerIndex, Dictionary<Keys, bool>> KeysReleased;

        private Dictionary<PlayerIndex, Dictionary<MouseButton, bool>> MouseButtonsPressed;
        private Dictionary<PlayerIndex, Dictionary<MouseButton, bool>> MouseButtonsPressedOnce;
        private Dictionary<PlayerIndex, Dictionary<MouseButton, bool>> MouseButtonsReleased;
        private Dictionary<PlayerIndex, int> MouseScrolled;
        private Dictionary<PlayerIndex, Vector2> MouseMoved;
        private Dictionary<PlayerIndex, RawMouse> MouseRaw;
        private Vector2 MouseBasePosition;
        private MultiInputController MouseController;

        private Dictionary<PlayerIndex, Dictionary<Buttons, bool>> GamePadButtonsPressed;
        private Dictionary<PlayerIndex, Dictionary<Buttons, bool>> GamePadButtonsPressedOnce;
        private Dictionary<PlayerIndex, Dictionary<Buttons, bool>> GamePadButtonsReleased;
        private Dictionary<PlayerIndex, Dictionary<Buttons, Vector2>> GamePadJoystickMoved;

        private Dictionary<PlayerIndex, List<Keys>> AllKeys;
        private Dictionary<PlayerIndex, List<MouseButton>> AllMouseButtons;
        private Dictionary<PlayerIndex, List<Buttons>> AllGamePadButtons;

        private List<InputListener> Listeners;


        public InputController()
        {
            Active = true;

            Listeners = new List<InputListener>();
            
            KeysPressed = new Dictionary<PlayerIndex, Dictionary<Keys, bool>>(4);
            KeysPressedOnce = new Dictionary<PlayerIndex, Dictionary<Keys, bool>>(4);
            KeysReleased = new Dictionary<PlayerIndex, Dictionary<Keys, bool>>(4);

            MouseButtonsPressed = new Dictionary<PlayerIndex, Dictionary<MouseButton, bool>>(4);
            MouseButtonsPressedOnce = new Dictionary<PlayerIndex, Dictionary<MouseButton, bool>>(4);
            MouseButtonsReleased = new Dictionary<PlayerIndex, Dictionary<MouseButton, bool>>(4);
            MouseScrolled = new Dictionary<PlayerIndex, int>(4);
            MouseMoved = new Dictionary<PlayerIndex, Vector2>(4);
            MouseBasePosition = Vector2.Zero;
            MouseRaw = new Dictionary<PlayerIndex, RawMouse>(4);
            MouseController = new MultiInputController(Mouse.WindowHandle);

            GamePadButtonsPressed = new Dictionary<PlayerIndex, Dictionary<Buttons, bool>>(4);
            GamePadButtonsPressedOnce = new Dictionary<PlayerIndex, Dictionary<Buttons, bool>>(4);
            GamePadButtonsReleased = new Dictionary<PlayerIndex, Dictionary<Buttons, bool>>(4);
            GamePadJoystickMoved = new Dictionary<PlayerIndex, Dictionary<Buttons, Vector2>>(4);

            int i = 0;

            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                KeysPressed.Add(player, new Dictionary<Keys, bool>(120));
                KeysPressedOnce.Add(player, new Dictionary<Keys, bool>(120));
                KeysReleased.Add(player, new Dictionary<Keys, bool>(120));

                MouseButtonsPressed.Add(player, new Dictionary<MouseButton, bool>(10));
                MouseButtonsPressedOnce.Add(player, new Dictionary<MouseButton, bool>(10));
                MouseButtonsReleased.Add(player, new Dictionary<MouseButton, bool>(10));
                MouseScrolled.Add(player, 0);
                MouseMoved.Add(player, Vector2.Zero);
                MouseRaw.Add(player, MouseController.Mice[Math.Min(i, MouseController.MiceCount)]);
                
                GamePadButtonsPressed.Add(player, new Dictionary<Buttons, bool>(30));
                GamePadButtonsPressedOnce.Add(player, new Dictionary<Buttons, bool>(30));
                GamePadButtonsReleased.Add(player, new Dictionary<Buttons, bool>(30));
                GamePadJoystickMoved.Add(player, new Dictionary<Buttons, Vector2>(2));

                i++;
            }

            AllKeys = new Dictionary<PlayerIndex, List<Keys>>(4);
            AllMouseButtons = new Dictionary<PlayerIndex, List<MouseButton>>(4);
            AllGamePadButtons = new Dictionary<PlayerIndex, List<Buttons>>(4);

            Visuel.Facade.etreNotifierTransition(doTransitionStarted, doTransitionStopped);
        }


        public void Initialize()
        {
            Active = true;

            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                this.addKeys(player, new List<Keys>());
                this.addMouseButtons(player, new List<MouseButton>());
                this.addGamePadButtons(player, new List<Buttons>());
            }

            MouseBasePosition = Vector2.Zero;
        }


        public void Dispose()
        {
            MouseController.Dispose();
        }


        public void addKeys(PlayerIndex player, List<Keys> keys)
        {
            AllKeys[player].Clear();
            AllKeys[player].AddRange(keys);

            KeysPressed[player].Clear();
            KeysPressedOnce[player].Clear();
            KeysReleased[player].Clear();

            foreach (var key in keys)
            {
                KeysPressed[player].Add(key, false);
                KeysPressedOnce[player].Add(key, false);
                KeysReleased[player].Add(key, false);
            }
        }


        public void addMouseButtons(PlayerIndex player, List<MouseButton> buttons)
        {
            AllMouseButtons[player].Clear();
            AllMouseButtons[player].AddRange(buttons);

            MouseButtonsPressed[player].Clear();
            MouseButtonsPressedOnce[player].Clear();
            MouseButtonsReleased[player].Clear();
            MouseScrolled[player] = 0;
            MouseMoved[player] = Vector2.Zero;

            foreach (var button in buttons)
            {
                MouseButtonsPressed[player].Add(button, false);
                MouseButtonsPressedOnce[player].Add(button, false);
                MouseButtonsReleased[player].Add(button, false);
            }
        }


        public void addGamePadButtons(PlayerIndex player, List<Buttons> buttons)
        {
            AllGamePadButtons[player].Clear();
            AllGamePadButtons[player].AddRange(buttons);

            GamePadButtonsPressed[player].Clear();
            GamePadButtonsPressedOnce[player].Clear();
            GamePadButtonsReleased[player].Clear();

            GamePadJoystickMoved[player].Clear();
            GamePadJoystickMoved[player].Add(Buttons.LeftStick, Vector2.Zero);
            GamePadJoystickMoved[player].Add(Buttons.RightStick, Vector2.Zero);
        }


        public void addListener(InputListener listener)
        {
            Listeners.Add(listener);
        }


        public bool isKeyPressed(PlayerIndex player, Keys key)
        {
            return KeysPressed[player][key];
        }


        public bool isMouseButtonPressed(PlayerIndex player, MouseButton button)
        {
            return MouseButtonsPressed[player][button];
        }


        public bool isGamePadButtonPressed(PlayerIndex player, Buttons button)
        {
            return GamePadButtonsPressed[player][button];
        }


        private void doTransitionStarted(object sender, EventArgs e)
        {
            this.Active = false;
        }


        private void doTransitionStopped(object sender, EventArgs e)
        {
            this.Active = true;
        }


        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            doKeyboardInput();
            doMouseInput();
            doGamePadInput();
        }


        private void doGamePadInput()
        {
            // Receive raw input
            foreach (var kvp in AllGamePadButtons)
            {
                GamePadState gamePadState = GamePad.GetState(kvp.Key, GamePadDeadZone.Circular);

                foreach (var button in kvp.Value)
                {
                    bool before = GamePadButtonsPressed[kvp.Key][button];
                    bool now = gamePadState.IsButtonDown(button);

                    GamePadButtonsPressed[kvp.Key][button] = now;
                    GamePadButtonsPressedOnce[kvp.Key][button] = !before && now;
                    GamePadButtonsReleased[kvp.Key][button] = before && !now;

                    GamePadJoystickMoved[kvp.Key][Buttons.LeftStick] = gamePadState.ThumbSticks.Left;
                    GamePadJoystickMoved[kvp.Key][Buttons.RightStick] = gamePadState.ThumbSticks.Left;
                }
            }

            // Spread the word
            foreach (var listener in Listeners)
            {
                if (!listener.Active)
                    continue;

                foreach (var kvp in AllGamePadButtons) //foreach player
                {
                    foreach (var button in kvp.Value) //foreach keyboard key
                    {
                        if (GamePadButtonsPressedOnce[kvp.Key][button])
                            listener.doGamePadButtonPressedOnce(kvp.Key, button);

                        if (GamePadButtonsReleased[kvp.Key][button])
                            listener.doGamePadButtonReleased(kvp.Key, button);

                        if (GamePadJoystickMoved[kvp.Key][Buttons.LeftStick] != Vector2.Zero)
                            listener.doGamePadJoystickMoved(kvp.Key, Buttons.LeftStick, GamePadJoystickMoved[kvp.Key][Buttons.LeftStick]);

                        if (GamePadJoystickMoved[kvp.Key][Buttons.RightStick] != Vector2.Zero)
                            listener.doGamePadJoystickMoved(kvp.Key, Buttons.RightStick, GamePadJoystickMoved[kvp.Key][Buttons.RightStick]);
                    }
                }
            }
        }


        private void doMouseInput()
        {
            // Receive raw input
            foreach (var kvp in MouseRaw)
            {
                foreach (var button in AllMouseButtons[kvp.Key])
                {
                    bool before = MouseButtonsPressed[kvp.Key][button];
                    bool now = kvp.Value.Buttons[(int) button];

                    MouseButtonsPressed[kvp.Key][button] = now;
                    MouseButtonsPressedOnce[kvp.Key][button] = !before && now;
                    MouseButtonsReleased[kvp.Key][button] = before && !now;
                    MouseScrolled[kvp.Key] = kvp.Value.ZDelta;
                    MouseMoved[kvp.Key] = new Vector2(kvp.Value.XDelta, kvp.Value.YDelta);
                }
            }


            foreach (var listener in Listeners)
            {
                if (!listener.Active)
                    continue;

                foreach (var kvp in AllMouseButtons) //foreach player
                {
                    foreach (var button in kvp.Value) //foreach mouse button
                    {
                        if (MouseButtonsPressedOnce[kvp.Key][button])
                            listener.doMouseButtonPressedOnce(kvp.Key, button);

                        if (MouseButtonsReleased[kvp.Key][button])
                            listener.doMouseButtonReleased(kvp.Key, button);

                        if (MouseScrolled[kvp.Key] != 0)
                            listener.doMouseScrolled(kvp.Key, MouseScrolled[kvp.Key]);

                        if (MouseMoved[kvp.Key] != Vector2.Zero)
                            listener.doMouseMoved(kvp.Key, MouseMoved[kvp.Key]);
                    }
                }
            }
        }

        private void doKeyboardInput()
        {
            // Receive raw input
            KeyboardState keyboardState = Keyboard.GetState();

            foreach (var kvp in AllKeys)
            {
                foreach (var keyboardkey in kvp.Value)
                {
                    bool before = KeysPressed[kvp.Key][keyboardkey];
                    bool now = keyboardState.IsKeyDown(keyboardkey);

                    KeysPressed[kvp.Key][keyboardkey] = now;
                    KeysPressedOnce[kvp.Key][keyboardkey] = !before && now;
                    KeysReleased[kvp.Key][keyboardkey] = before && !now;
                }
            }


            // Spread the word
            foreach (var listener in Listeners)
            {
                if (!listener.Active)
                    continue;

                foreach (var kvp in AllKeys) //foreach player
                {
                    foreach (var keyboardkey in kvp.Value) //foreach keyboard key
                    {
                        if (KeysPressedOnce[kvp.Key][keyboardkey])
                            listener.doKeyPressedOnce(kvp.Key, keyboardkey);

                        if (KeysReleased[kvp.Key][keyboardkey])
                            listener.doKeyReleased(kvp.Key, keyboardkey);
                    }
                }
            }
        }
    }
}
