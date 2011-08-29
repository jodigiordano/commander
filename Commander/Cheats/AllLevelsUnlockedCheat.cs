namespace EphemereGames.Commander
{
    class AllLevelsUnlockedCheat : SequenceCheat
    {
        public AllLevelsUnlockedCheat()
            : base("AllLevelsUnlocked")
        {
            TimeToExecute = 1000;
            Recurrent = true;

            SequenceMouse.Add(KeyboardConfiguration.Cheat1);
            SequenceMouse.Add(KeyboardConfiguration.Cheat1);
            SequenceMouse.Add(KeyboardConfiguration.Cheat1);
            SequenceMouse.Add(KeyboardConfiguration.Cheat1);
            SequenceMouse.Add(KeyboardConfiguration.Cheat1);

            SequenceGamepad.Add(GamePadConfiguration.Cheat1);
            SequenceGamepad.Add(GamePadConfiguration.Cheat1);
            SequenceGamepad.Add(GamePadConfiguration.Cheat1);
            SequenceGamepad.Add(GamePadConfiguration.Cheat1);
            SequenceGamepad.Add(GamePadConfiguration.Cheat1);
        }
    }
}
