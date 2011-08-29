namespace EphemereGames.Commander
{
    class LevelWonCheat : SequenceCheat
    {
        public LevelWonCheat()
            : base("LevelWon")
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
