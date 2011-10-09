namespace EphemereGames.Commander
{
    class LevelWonCheat : SequenceCheat
    {
        public LevelWonCheat()
            : base("LevelWon")
        {
            TimeToExecute = 1000;
            Recurrent = true;

            //if (p.InputType == InputType.Mouse || p.InputType == InputType.OneJoystick)
            //{
            //    SequenceKeyboard.Add(p.KeyboardConfiguration.Cheat1);
            //    SequenceKeyboard.Add(p.KeyboardConfiguration.Cheat1);
            //    SequenceKeyboard.Add(p.KeyboardConfiguration.Cheat1);
            //}

            //else
            //{
            //    SequenceGamepad.Add(p.GamepadConfiguration.Cheat1);
            //    SequenceGamepad.Add(p.GamepadConfiguration.Cheat1);
            //    SequenceGamepad.Add(p.GamepadConfiguration.Cheat1);
            //}
        }
    }
}
