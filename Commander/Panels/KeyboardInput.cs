namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    
    class KeyboardInput : InputListener
    {
        public NoneHandler DesactivatedCallback;
        public TextBox TextBoxFocus;


        private bool ShiftMode;
        private bool ShiftModeCooldown;


        public KeyboardInput()
        {
            TextBoxFocus = null;
            ShiftMode = false;
        }


        public bool Active
        {
            set
            {
                Inputs.SetAllKeysOneListenerMode(value, Inputs.MasterPlayer, this);
                ShiftMode = false;
                ShiftModeCooldown = false;
            }
        }


        public void Update()
        {
            ShiftModeCooldown = false;
        }


        bool InputListener.EnableInputs
        {
            get { return true; }
        }


        void InputListener.DoKeyPressed(Core.Input.Player player, Keys key)
        {
            ShiftMode = key == Keys.LeftShift || key == Keys.RightShift;

            if (ShiftMode)
                ShiftModeCooldown = true;
        }


        void InputListener.DoKeyPressedOnce(Core.Input.Player player, Keys key)
        {
            if (key == Keys.Enter)
            {
                Inputs.SetAllKeysOneListenerMode(false, Inputs.MasterPlayer, this);
                TextBoxFocus = null;

                if (DesactivatedCallback != null)
                    DesactivatedCallback();
            }

            else if (key == Keys.Back)
            {
                TextBoxFocus.DoBackspace();
            }

            else if (key == Keys.Delete)
            {
                TextBoxFocus.Value = "";
            }

            else if (key == Keys.Escape || key == Keys.Down || key == Keys.Left || key == Keys.Up || key == Keys.Right ||
                     key == Keys.PageUp || key == Keys.PageDown || key == Keys.Home || key == Keys.End)
            {
                // ignore
            }

            else if (ShiftMode || ShiftModeCooldown)
            {
                ShiftModeCooldown = false;

                var value = "";

                if (key == Keys.D0)
                    value = ")";
                else if (key == Keys.D1)
                    value = "!";
                else if (key == Keys.D2)
                    value = "@";
                else if (key == Keys.D3)
                    value = "#";
                else if (key == Keys.D4)
                    value = "$";
                else if (key == Keys.D5)
                    value = "%";
                else if (key == Keys.D6)
                    value = "^";
                else if (key == Keys.D7)
                    value = "&";
                else if (key == Keys.D8)
                    value = "*";
                else if (key == Keys.D9)
                    value = "(";
                else if (key == Keys.OemTilde)
                    value = "`";
                else if (key == Keys.OemMinus)
                    value = "_";
                else if (key == Keys.OemPlus)
                    value = "=";
                else if (key == Keys.OemSemicolon)
                    value = ":";
                else if (key == Keys.OemComma)
                    value = "<";
                else if (key == Keys.OemPeriod)
                    value = ">";

                TextBoxFocus.Value += value;
            }

            else
            {
                var value = "";

                if (key == Keys.D0)
                    value = "0";
                else if (key == Keys.D1)
                    value = "1";
                else if (key == Keys.D2)
                    value = "2";
                else if (key == Keys.D3)
                    value = "3";
                else if (key == Keys.D4)
                    value = "4";
                else if (key == Keys.D5)
                    value = "5";
                else if (key == Keys.D6)
                    value = "6";
                else if (key == Keys.D7)
                    value = "7";
                else if (key == Keys.D8)
                    value = "8";
                else if (key == Keys.D9)
                    value = "9";
                else if (key == Keys.Add)
                    value = "+";
                else if (key == Keys.Decimal)
                    value = "-";
                else if (key == Keys.Divide)
                    value = "/";
                else if (key == Keys.Multiply)
                    value = "*";
                else if (key == Keys.OemBackslash)
                    value = "\\";
                else if (key == Keys.OemComma)
                    value = ",";
                else if (key == Keys.OemMinus)
                    value = "-";
                else if (key == Keys.OemPeriod)
                    value = ".";
                else if (key == Keys.OemPlus)
                    value = "+";
                else if (key == Keys.OemQuestion)
                    value = "?";
                else if (key == Keys.OemQuotes)
                    value = "\"";
                else if (key == Keys.OemSemicolon)
                    value = ";";
                else if (key == Keys.OemTilde)
                    value = "~";
                else
                    value = key.ToString();

                TextBoxFocus.Value += value;
            }

            // todo: tabulation
        }


        void InputListener.DoKeyReleased(Core.Input.Player player, Keys key)
        {
            ShiftMode = !(key == Keys.LeftShift || key == Keys.RightShift);
        }


        void InputListener.DoMouseButtonPressed(Core.Input.Player player, MouseButton button)
        {

        }


        void InputListener.DoMouseButtonPressedOnce(Core.Input.Player player, MouseButton button)
        {

        }


        void InputListener.DoMouseButtonReleased(Core.Input.Player player, MouseButton button)
        {

        }


        void InputListener.DoMouseScrolled(Core.Input.Player player, int delta)
        {

        }


        void InputListener.DoMouseMoved(Core.Input.Player player, Vector3 delta)
        {

        }


        void InputListener.DoGamePadButtonPressedOnce(Core.Input.Player player, Buttons button)
        {

        }


        void InputListener.DoGamePadButtonReleased(Core.Input.Player player, Buttons button)
        {

        }


        void InputListener.DoGamePadJoystickMoved(Core.Input.Player player, Buttons button, Vector3 delta)
        {

        }


        void InputListener.PlayerKeyboardConnectionRequested(Core.Input.Player Player, Keys key)
        {

        }


        void InputListener.PlayerMouseConnectionRequested(Core.Input.Player Player, MouseButton button)
        {

        }


        void InputListener.PlayerGamePadConnectionRequested(Core.Input.Player Player, Buttons button)
        {

        }


        void InputListener.DoPlayerConnected(Core.Input.Player player)
        {

        }


        void InputListener.DoPlayerDisconnected(Core.Input.Player player)
        {

        }
    }
}
