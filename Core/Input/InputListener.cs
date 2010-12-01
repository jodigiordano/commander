﻿namespace Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    interface InputListener
    {
        bool Active { get; }

        void doKeyPressedOnce(PlayerIndex inputIndex, Keys key);
        void doKeyReleased(PlayerIndex inputIndex, Keys key);

        void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button);
        void doMouseButtonReleased(PlayerIndex inputIndex, MouseButton button);
        void doMouseScrolled(PlayerIndex inputIndex, int delta);
        void doMouseMoved(PlayerIndex inputIndex, Vector2 delta);

        void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button);
        void doGamePadButtonReleased(PlayerIndex inputIndex, Buttons button);
        void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector2 delta);
    }
}