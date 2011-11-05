namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class InputsController
    {
        public bool Active;

        private Vector2 MouseBasePosition;
        private Dictionary<Player, InputSource> Sources;
        private List<InputListener> Listeners;

        private InputSource AllKeysSource;
        private bool AllKeysOneListenerMode;
        private InputListener AllKeysListener;


        public InputsController(Vector2 mouseBasePosition)
        {
            Active = true;
            MouseBasePosition = mouseBasePosition;

            Sources = new Dictionary<Player, InputSource>();
            Listeners = new List<InputListener>();

            SetAllKeysMode(false, null, null);
        }


        public void Initialize()
        {
            Active = true;

            Mouse.SetPosition((int) MouseBasePosition.X, (int) MouseBasePosition.Y);

            Visual.Visuals.AddTransitionListener(DoTransitionStarted, DoTransitionStopped);
        }


        public void AddPlayer(Player player)
        {
            Sources.Add(player, new InputSource(player, MouseBasePosition));

            MapKeys(player, player.KeysToListenTo);
            MapMouseButtons(player, player.MouseButtonsToListenTo);
            MapGamePadButtons(player, player.GamePadButtonsToListenTo);
        }


        public void MapKeys(Player player, List<Keys> keys)
        {
            Sources[player].MapKeys(keys);
        }


        public void MapMouseButtons(Player player, List<MouseButton> buttons)
        {
            Sources[player].MapMouseButtons(buttons);
        }


        public void MapGamePadButtons(Player player, List<Buttons> buttons)
        {
            Sources[player].MapGamePadButtons(buttons);
        }


        public void AddListener(InputListener listener)
        {
            Listeners.Add(listener);
        }


        public void RemoveListener(InputListener listener)
        {
            Listeners.Remove(listener);
        }


        public bool IsKeyPressed(Player player, Keys key)
        {
            if (player.InputType == InputType.Gamepad || player.InputType == InputType.None)
                return false;

            return Sources[player].IsKeyPressed(key);
        }


        public bool IsMouseButtonPressed(Player player, MouseButton button)
        {
            if (player.InputType == InputType.Gamepad || player.InputType == InputType.None)
                return false;

            return Sources[player].IsMouseButtonPressed(button);
        }


        public bool IsGamePadButtonPressed(Player player, Buttons button)
        {
            if (player.InputType != InputType.Gamepad)
                return false;

            return Sources[player].IsGamePadButtonPressed(button);
        }


        private void DoTransitionStarted()
        {
            this.Active = false;
        }


        private void DoTransitionStopped()
        {
            this.Active = true;
        }


        public void Update()
        {
            if (!Active)
                return;

            if (AllKeysOneListenerMode)
            {
                UpdateAllKeysOneListenerMode();
                return;
            }

            // Receive raw input
            foreach (var source in Sources)
            {
                var player = source.Key;
                var state = source.Value;

                if (player.State == PlayerState.Connected && player.InputType == InputType.Gamepad)
                {
                    state.DoGamePadInput();
                }

                else if (player.State == PlayerState.Connected && player.InputType == InputType.KeyboardOnly)
                {
                    state.DoKeyboardInput();
                }

                else if (player.State == PlayerState.Connected)
                {
                    state.DoKeyboardInput();
                    state.DoMouseInput();
                }

                else
                {
                    state.DoKeyboardInput();
                    state.DoMouseInput();
                    state.DoGamePadInput();
                }
            }

            // Spread the word
            for (int i = 0; i < Listeners.Count; i++)
            {
                if (!Listeners[i].EnableInputs)
                    continue;

                foreach (var source in Sources.Values)
                    source.TellListener(Listeners[i]);
            }

            Mouse.SetPosition((int)MouseBasePosition.X, (int)MouseBasePosition.Y);
        }


        private void UpdateAllKeysOneListenerMode()
        {
            AllKeysSource.DoKeyboardInput();

            if (!AllKeysListener.EnableInputs)
                return;

            AllKeysSource.TellListener(AllKeysListener);
        }


        public void SetAllKeysMode(bool active, Player player, InputListener listener)
        {
            AllKeysOneListenerMode = active;

            if (!AllKeysOneListenerMode)
            {
                AllKeysSource = null;
                AllKeysListener = null;
                return;
            }

            AllKeysSource = new InputSource(player, MouseBasePosition);

            AllKeysSource.MapKeys(AllKeys);

            AllKeysListener = listener;
        }


        private static List<Keys> AllKeys = new List<Keys>()
        {
            // letters
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K,
            Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
            Keys.W, Keys.X, Keys.Y, Keys.Z,
                    
            // numbers
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
            Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,

            // controls
            Keys.Escape, Keys.Back, Keys.Delete, Keys.Space, Keys.Enter, Keys.Up, Keys.Down, Keys.Left,
            Keys.Right, Keys.PageUp, Keys.PageDown, Keys.Home, Keys.End, Keys.LeftShift, Keys.RightShift,
            Keys.Tab,

            // special characters
            Keys.Add, Keys.Decimal, Keys.Divide, Keys.Multiply, Keys.OemBackslash, Keys.OemComma,
            Keys.OemMinus, Keys.OemPeriod, Keys.OemPlus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon,
            Keys.OemTilde,
        };
    }
}
