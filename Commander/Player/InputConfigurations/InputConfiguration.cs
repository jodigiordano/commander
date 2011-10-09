namespace EphemereGames.Commander
{
    class InputConfiguration
    {
        public GamepadInputConfiguration GamepadConfiguration;
        public KeyboardInputConfiguration KeyboardConfiguration;
        public MouseInputConfiguration MouseConfiguration;


        public InputConfiguration()
        {
            GamepadConfiguration = new GamepadInputConfiguration();
            KeyboardConfiguration = new KeyboardInputConfiguration();
            MouseConfiguration = new MouseInputConfiguration();
        }
    }
}
