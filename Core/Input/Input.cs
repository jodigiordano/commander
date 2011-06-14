namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;
    
    public static class Input
    {
        private static InputController InputController;
        private static Vibrator Vibrator;
        public static PlayerConnection PlayerConnection;


        public static void Initialize(Vector2 mouseBasePosition)
        {
            PlayerConnection = new PlayerConnection();

            Vibrator = new Vibrator();
            Vibrator.Initialize();

            InputController = new InputController(mouseBasePosition);
            InputController.Initialize();
        }


        public static void UpdateInputSource(
            PlayerIndex inputIndex,
            List<MouseButton> mouseButtons,
            List<Buttons> gamepadButtons,
            List<Keys> keyboardKeys)
        {
            InputController.MapMouseButtons(inputIndex, mouseButtons);
            InputController.MapGamePadButtons(inputIndex, gamepadButtons);
            InputController.MapKeys(inputIndex, keyboardKeys);
        }


        public static bool Active
        {
            set
            {
                InputController.Active = value;
            }
        }


        public static void ConnectPlayer(PlayerIndex inputIndex)
        {
            PlayerConnection.Connect(inputIndex);
        }


        public static void AddListener(InputListener listener)
        {
            InputController.AddListener(listener);
        }


        public static void RemoveListener(InputListener listener)
        {
            InputController.RemoveListener(listener);
        }


        public static void Update(GameTime gameTime)
        {
            InputController.Update(gameTime);
            Vibrator.Update(gameTime);
        }


        public static bool IsKeyPressed(PlayerIndex inputIndex, Keys key)
        {
            return InputController.IsKeyPressed(inputIndex, key);
        }


        public static bool IsMouseButtonPressed(PlayerIndex inputIndex, MouseButton button)
        {
            return InputController.IsMouseButtonPressed(inputIndex, button);
        }


        public static bool IsGamePadButtonPressed(PlayerIndex inputIndex, Buttons button)
        {
            return InputController.IsGamePadButtonPressed(inputIndex, button);
        }


        public static void VibrateController(PlayerIndex inputIndex, float length, float left, float right)
        {
            Vibrator.Vibrate(inputIndex, length, left, right);
        }
    }
}
