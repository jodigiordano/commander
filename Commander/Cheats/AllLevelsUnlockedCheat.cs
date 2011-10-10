namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework.Input;

    
    class AllLevelsUnlockedCheat : SequenceCheat
    {
        public AllLevelsUnlockedCheat()
            : base("AllLevelsUnlocked")
        {
            TimeToExecute = 1000;
            Recurrent = true;

            SequenceKeyboard.Add(Keys.P);
            SequenceKeyboard.Add(Keys.P);
            SequenceKeyboard.Add(Keys.P);

            SequenceGamepad.Add(Buttons.DPadRight);
            SequenceGamepad.Add(Buttons.DPadRight);
            SequenceGamepad.Add(Buttons.DPadRight);
        }
    }
}
