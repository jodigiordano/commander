namespace EphemereGames.Core.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using System;
    using EphemereGames.Core.Utilities;


    class InputSource
    {
        public List<Keys> MappedKeys;
        public List<MouseButton> MappedMouseButtons;
        public List<Buttons> MappedGamePadButtons;

        private PlayerIndex Index;
        private Vector2 MouseBasePosition;
        private int PreviousMouseWheelValue;

        private Dictionary<Keys, bool> KeysPressed;
        private Dictionary<Keys, bool> KeysPressedOnce;
        private Dictionary<Keys, bool> KeysReleased;

        private Dictionary<MouseButton, bool> MouseButtonsPressed;
        private Dictionary<MouseButton, bool> MouseButtonsPressedOnce;
        private Dictionary<MouseButton, bool> MouseButtonsReleased;
        private int MouseScrolled;
        private Vector3 MouseMoved;

        private Dictionary<Buttons, bool> GamePadButtonsPressed;
        private Dictionary<Buttons, bool> GamePadButtonsPressedOnce;
        private Dictionary<Buttons, bool> GamePadButtonsReleased;
        private Dictionary<Buttons, Vector3> GamePadJoystickMoved;


        public InputSource(PlayerIndex index, Vector2 mouseBasePosition)
        {
            Index = index;
            MouseBasePosition = mouseBasePosition;
            PreviousMouseWheelValue = 0;

            MappedKeys = new List<Keys>();
            MappedMouseButtons = new List<MouseButton>();
            MappedGamePadButtons = new List<Buttons>();

            KeysPressed = new Dictionary<Keys, bool>(KeysComparer.Default);
            KeysPressedOnce = new Dictionary<Keys, bool>(KeysComparer.Default);
            KeysReleased = new Dictionary<Keys, bool>(KeysComparer.Default);

            MouseButtonsPressed = new Dictionary<MouseButton, bool>(MouseButtonComparer.Default);
            MouseButtonsPressedOnce = new Dictionary<MouseButton, bool>(MouseButtonComparer.Default);
            MouseButtonsReleased = new Dictionary<MouseButton, bool>(MouseButtonComparer.Default);
            MouseScrolled = 0;
            MouseMoved = Vector3.Zero;

            GamePadButtonsPressed = new Dictionary<Buttons, bool>(ButtonsComparer.Default);
            GamePadButtonsPressedOnce = new Dictionary<Buttons, bool>(ButtonsComparer.Default);
            GamePadButtonsReleased = new Dictionary<Buttons, bool>(ButtonsComparer.Default);
            GamePadJoystickMoved = new Dictionary<Buttons, Vector3>(ButtonsComparer.Default);
        }


        public void MapKeys(List<Keys> keys)
        {
            MappedKeys.Clear();
            Set<Keys> hash = new Set<Keys>(keys);
            MappedKeys.AddRange(hash);

            KeysPressed.Clear();
            KeysPressedOnce.Clear();
            KeysReleased.Clear();

            foreach (var key in hash)
            {
                KeysPressed.Add(key, false);
                KeysPressedOnce.Add(key, false);
                KeysReleased.Add(key, false);
            }
        }


        public void MapMouseButtons(List<MouseButton> buttons)
        {
            MappedMouseButtons.Clear();
            Set<MouseButton> hash = new Set<MouseButton>(buttons);
            MappedMouseButtons.AddRange(hash);

            MouseButtonsPressed.Clear();
            MouseButtonsPressedOnce.Clear();
            MouseButtonsReleased.Clear();
            MouseScrolled = 0;
            MouseMoved = Vector3.Zero;

            foreach (var button in hash)
            {
                MouseButtonsPressed.Add(button, false);
                MouseButtonsPressedOnce.Add(button, false);
                MouseButtonsReleased.Add(button, false);
            }
        }


        public void MapGamePadButtons(List<Buttons> buttons)
        {
            MappedGamePadButtons.Clear();
            Set<Buttons> hash = new Set<Buttons>(buttons);
            MappedGamePadButtons.AddRange(hash);

            GamePadButtonsPressed.Clear();
            GamePadButtonsPressedOnce.Clear();
            GamePadButtonsReleased.Clear();

            foreach (var button in hash)
            {
                GamePadButtonsPressed.Add(button, false);
                GamePadButtonsPressedOnce.Add(button, false);
                GamePadButtonsReleased.Add(button, false);
            }

            GamePadJoystickMoved.Clear();
            GamePadJoystickMoved.Add(Buttons.LeftStick, Vector3.Zero);
            GamePadJoystickMoved.Add(Buttons.RightStick, Vector3.Zero);
        }


        public bool IsKeyPressed(Keys key)
        {
            return KeysPressed[key];
        }


        public bool IsMouseButtonPressed(MouseButton button)
        {
            return MouseButtonsPressed[button];
        }


        public bool IsGamePadButtonPressed(Buttons button)
        {
            return GamePadButtonsPressed[button];
        }


        public void doGamePadInput()
        {
            // Receive raw input
            GamePadState gamePadState = GamePad.GetState(Index, GamePadDeadZone.Circular);

            foreach (var button in MappedGamePadButtons)
            {
                bool before = GamePadButtonsPressed[button];
                bool now = gamePadState.IsButtonDown(button);

                GamePadButtonsPressed[button] = now;
                GamePadButtonsPressedOnce[button] = !before && now;
                GamePadButtonsReleased[button] = before && !now;
            }

            GamePadJoystickMoved[Buttons.LeftStick] = new Vector3(gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y, 0);
            GamePadJoystickMoved[Buttons.RightStick] = new Vector3(gamePadState.ThumbSticks.Right.X, -gamePadState.ThumbSticks.Right.Y, 0);
        }


        public void doMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            int currentMouseWheelValue = MouseWheelValue(mouseState.ScrollWheelValue);
            PreviousMouseWheelValue = mouseState.ScrollWheelValue;

            // Receive raw input
            foreach (var button in MappedMouseButtons)
            {
                bool before = MouseButtonsPressed[button];
                bool now = (button == MouseButton.Left) ?
                               mouseState.LeftButton == ButtonState.Pressed :
                           (button == MouseButton.Middle) ?
                               mouseState.MiddleButton == ButtonState.Pressed :
                           (button == MouseButton.MiddleUp) ?
                               currentMouseWheelValue > 0 :
                           (button == MouseButton.MiddleDown) ?
                               currentMouseWheelValue < 0 :
                               mouseState.RightButton == ButtonState.Pressed; //Middle   

                MouseButtonsPressed[button] = now;
                MouseButtonsPressedOnce[button] = !before && now;
                MouseButtonsReleased[button] = before && !now;
            }

            MouseScrolled = currentMouseWheelValue;
            MouseMoved = new Vector3(mouseState.X - MouseBasePosition.X, mouseState.Y - MouseBasePosition.Y, 0);
        }


        public void doKeyboardInput()
        {
            // Receive raw input
            KeyboardState keyboardState = Keyboard.GetState();

            foreach (var key in MappedKeys)
            {
                bool before = KeysPressed[key];
                bool now = keyboardState.IsKeyDown(key);

                KeysPressed[key] = now;
                KeysPressedOnce[key] = !before && now;
                KeysReleased[key] = before && !now;
            }
        }


        public void TellListener(InputListener listener)
        {
            foreach (var button in MappedGamePadButtons)
            {
                if (GamePadButtonsPressedOnce[button])
                    listener.doGamePadButtonPressedOnce(Index, button);

                if (GamePadButtonsReleased[button])
                    listener.doGamePadButtonReleased(Index, button);
            }


            if (GamePadJoystickMoved[Buttons.LeftStick] != Vector3.Zero)
                listener.doGamePadJoystickMoved(Index, Buttons.LeftStick, GamePadJoystickMoved[Buttons.LeftStick]);


            if (GamePadJoystickMoved[Buttons.RightStick] != Vector3.Zero)
                listener.doGamePadJoystickMoved(Index, Buttons.RightStick, GamePadJoystickMoved[Buttons.RightStick]);


            foreach (var button in MappedMouseButtons)
            {
                if (MouseButtonsPressedOnce[button])
                    listener.doMouseButtonPressedOnce(Index, button);

                if (MouseButtonsReleased[button])
                    listener.doMouseButtonReleased(Index, button);
            }


            if (MouseScrolled != 0)
                listener.doMouseScrolled(Index, MouseScrolled);


            if (MouseMoved != Vector3.Zero)
            {
                listener.doMouseMoved(Index, MouseMoved);
            }


            foreach (var keyboardkey in MappedKeys)
            {
                if (KeysPressedOnce[keyboardkey])
                    listener.doKeyPressedOnce(Index, keyboardkey);

                if (KeysReleased[keyboardkey])
                    listener.doKeyReleased(Index, keyboardkey);
            }
        }


        private int MouseWheelValue(int current)
        {
            return PreviousMouseWheelValue - current;
        }
    }
}
