namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class InputController
    {
        public bool Active;
        private Vector2 MouseBasePosition;
        private Dictionary<Player, InputSource> Sources;
        private List<InputListener> Listeners;


        public InputController(Vector2 mouseBasePosition)
        {
            Active = true;
            MouseBasePosition = mouseBasePosition;

            Sources = new Dictionary<Player, InputSource>();
            Listeners = new List<InputListener>();
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
            if (player.InputType != InputType.Mouse)
                return false;

            return Sources[player].IsKeyPressed(key);
        }


        public bool IsMouseButtonPressed(Player player, MouseButton button)
        {
            if (player.InputType != InputType.Mouse)
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


        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            // Receive raw input
            foreach (var source in Sources.Values)
            {
                source.DoKeyboardInput();
                source.DoMouseInput();
                source.DoGamePadInput();
            }

            // Spread the word
            for (int i = 0; i < Listeners.Count; i++) //because Scenes can be created in another thread
            {
                if (!Listeners[i].EnableInputs)
                    continue;

                foreach (var source in Sources.Values)
                    source.TellListener(Listeners[i]);
            }

            Mouse.SetPosition((int)MouseBasePosition.X, (int)MouseBasePosition.Y);
        }
    }
}
