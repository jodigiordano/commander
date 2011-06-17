namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    public enum InputType
    {
        Mouse,
        Gamepad,
        None
    }


    public static class Inputs
    {
        private static InputController InputController;
        internal static PlayerConnection PlayerConnection;
        internal static PlayersController PlayersController;
        private static Vibrator Vibrator;


        public static void Initialize(Vector2 mouseBasePosition)
        {
            PlayerConnection = new PlayerConnection();

            Vibrator = new Vibrator();
            Vibrator.Initialize();

            InputController = new InputController(mouseBasePosition);
            InputController.Initialize();

            PlayersController = new PlayersController();
            PlayersController.Initialize();
        }


        public static void UpdateInputSource(
            Player player,
            List<MouseButton> mouseButtons,
            List<Buttons> gamepadButtons,
            List<Keys> keyboardKeys)
        {
            InputController.MapMouseButtons(player, mouseButtons);
            InputController.MapGamePadButtons(player, gamepadButtons);
            InputController.MapKeys(player, keyboardKeys);
        }


        public static bool Active
        {
            set
            {
                InputController.Active = value;
            }
        }


        public static void ConnectPlayer(Player player)
        {
            PlayerConnection.Connect(player);
        }


        public static void AddListener(InputListener listener)
        {
            InputController.AddListener(listener);
            PlayersController.AddListener(listener);
        }


        public static void RemoveListener(InputListener listener)
        {
            InputController.RemoveListener(listener);
            PlayersController.RemoveListener(listener);
        }


        public static void Update(GameTime gameTime)
        {
            InputController.Update(gameTime);
            Vibrator.Update(gameTime);
        }


        public static bool IsKeyPressed(Player player, Keys key)
        {
            return InputController.IsKeyPressed(player, key);
        }


        public static bool IsMouseButtonPressed(Player player, MouseButton button)
        {
            return InputController.IsMouseButtonPressed(player, button);
        }


        public static bool IsGamePadButtonPressed(Player player, Buttons button)
        {
            return InputController.IsGamePadButtonPressed(player, button);
        }


        public static void VibrateController(Player player, float length, float left, float right)
        {
            if (player.InputType != InputType.Gamepad)
                return;

            Vibrator.Vibrate(player.Index, length, left, right);
        }


        public static void AddPlayer(Player player)
        {
            PlayersController.AddPlayer(player);
            InputController.AddPlayer(player);
        }


        public static List<Player> Players { get { return PlayersController.Players; } }

        public static Player MasterPlayer { get { return PlayersController.MasterPlayer; } }

        public static List<Player> ConnectedPlayers { get { return PlayersController.ConnectedPlayers; } }
    }
}
