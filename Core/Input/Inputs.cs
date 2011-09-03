namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    
    
    public enum InputType
    {
        Mouse = 0x0000,
        Gamepad = 0x0100,
        None = 0x1100
    }


    public static class Inputs
    {
#if WINDOWS && TRIAL
        internal static Setting Target = Setting.WindowsDemo;
#elif WINDOWS
        internal static Setting Target = Setting.WindowsFull;
#else
        internal static Setting Target = Setting.Xbox360;
#endif

        internal static InputsController InputsController;
        internal static PlayerConnection PlayerConnection;
        internal static PlayersController PlayersController;
        private static Vibrator Vibrator;


        public static void Initialize(Vector2 mouseBasePosition)
        {
            PlayerConnection = new PlayerConnection();

            Vibrator = new Vibrator();
            Vibrator.Initialize();

            InputsController = new InputsController(mouseBasePosition);
            InputsController.Initialize();

            PlayersController = new PlayersController();
            PlayersController.Initialize();
        }


        public static void Ready()
        {
            PlayerConnection.Initialize();
        }


        public static void UpdateInputSource(
            Player player,
            List<MouseButton> mouseButtons,
            List<Buttons> gamepadButtons,
            List<Keys> keyboardKeys)
        {
            InputsController.MapMouseButtons(player, mouseButtons);
            InputsController.MapGamePadButtons(player, gamepadButtons);
            InputsController.MapKeys(player, keyboardKeys);
        }


        public static bool Active
        {
            set
            {
                InputsController.Active = value;
            }
        }


        public static void ConnectPlayer(Player player)
        {
            PlayerConnection.Connect(player);
        }


        public static void DisconnectPlayer(Player player)
        {
            PlayerConnection.Disconnect(player);
        }


        public static void AddListener(InputListener listener)
        {
            InputsController.AddListener(listener);
            PlayersController.AddListener(listener);
        }


        public static void RemoveListener(InputListener listener)
        {
            InputsController.RemoveListener(listener);
            PlayersController.RemoveListener(listener);
        }


        public static void Update(GameTime gameTime)
        {
            InputsController.Update();
            PlayersController.Update();
            Vibrator.Update(gameTime);
        }


        public static bool IsKeyPressed(Player player, Keys key)
        {
            return InputsController.IsKeyPressed(player, key);
        }


        public static bool IsMouseButtonPressed(Player player, MouseButton button)
        {
            return InputsController.IsMouseButtonPressed(player, button);
        }


        public static bool IsGamePadButtonPressed(Player player, Buttons button)
        {
            return InputsController.IsGamePadButtonPressed(player, button);
        }


        public static void VibrateControllerLowFrequency(Player player, double length, float amount)
        {
            if (player.InputType != InputType.Gamepad)
                return;

            Vibrator.VibrateLowFrequency(player.Index, length, amount);
        }


        public static void VibrateControllerHighFrequency(Player player, double length, float amount)
        {
            if (player.InputType != InputType.Gamepad)
                return;

            Vibrator.VibrateHighFrequency(player.Index, length, amount);
        }


        public static void StopAllVibrators()
        {
            Vibrator.Initialize();
        }


        public static void AddPlayer(Player player)
        {
            PlayersController.AddPlayer(player);
            InputsController.AddPlayer(player);
        }


        public static List<Player> Players { get { return PlayersController.Players; } }

        public static Player MasterPlayer { get { return PlayersController.MasterPlayer; } }

        public static List<Player> ConnectedPlayers { get { return PlayersController.ConnectedPlayers; } }
    }
}
