namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    
    class KeyboardInput : InputListener
    {
        public NoneHandler DesactivatedCallback;
        public TextBoxGroup TextBoxGroup;

        private bool DeleteMode;
        private bool ShiftMode;
        private bool ShiftModeCooldown;
        private double DeleteSpeed;
        private double DeleteCounter;


        public KeyboardInput()
        {
            TextBoxGroup = null;
            DeleteSpeed = 200; //ms
        }


        public bool Active
        {
            set
            {
                Inputs.SetAllKeysOneListenerMode(value, Inputs.MasterPlayer, this);
                ShiftMode = false;
                ShiftModeCooldown = false;
                DeleteMode = false;
                DeleteCounter = DeleteSpeed;
            }
        }


        public void Update()
        {
            ShiftModeCooldown = false;

            if (DeleteMode)
            {
                DeleteCounter -= Preferences.TargetElapsedTimeMs;

                if (DeleteCounter <= 0 && TextBoxGroup != null && TextBoxGroup.Focus != null)
                {
                    TextBoxGroup.Focus.DoBackspace();
                    DeleteCounter = DeleteSpeed;
                }
            }
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

            DeleteMode = key == Keys.Back;
        }


        void InputListener.DoKeyPressedOnce(Core.Input.Player player, Keys key)
        {
            // todo: moving cursor around

            if (key == Keys.Enter || key == Keys.Escape)
            {
                Inputs.SetAllKeysOneListenerMode(false, Inputs.MasterPlayer, this);
                TextBoxGroup.SwitchTo(null);

                if (DesactivatedCallback != null)
                    DesactivatedCallback();
            }

            else if (key == Keys.Back)
            {
                TextBoxGroup.Focus.DoBackspace();
                DeleteMode = true;
            }

            else if (key == Keys.Delete)
            {
                TextBoxGroup.Focus.Value = "";
            }

            else if (key == Keys.Escape || key == Keys.Down || key == Keys.Left || key == Keys.Up || key == Keys.Right ||
                     key == Keys.PageUp || key == Keys.PageDown || key == Keys.Home || key == Keys.End || key == Keys.Enter)
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

                TextBoxGroup.Focus.Value += value;
            }

            else if (key == Keys.Tab)
            {
                TextBoxGroup.Toggle();
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
                else if (key == Keys.Space)
                    value = " ";
                else
                    value = key.ToString();

                TextBoxGroup.Focus.Value += value;
            }
        }


        void InputListener.DoKeyReleased(Core.Input.Player player, Keys key)
        {
            ShiftMode = !(key == Keys.LeftShift || key == Keys.RightShift);

            if (key == Keys.Back)
            {
                DeleteMode = false;
                DeleteCounter = DeleteSpeed;
            }
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
