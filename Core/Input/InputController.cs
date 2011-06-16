namespace EphemereGames.Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    class InputController
    {
        public bool Active;
        private Vector2 MouseBasePosition;
        private Dictionary<PlayerIndex, InputSource> Sources;
        private List<InputListener> Listeners;


        public InputController(Vector2 mouseBasePosition)
        {
            Active = true;
            MouseBasePosition = mouseBasePosition;

            Sources = new Dictionary<PlayerIndex, InputSource>(PlayerIndexComparer.Default);
            Listeners = new List<InputListener>();

            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
                Sources.Add(player, new InputSource(player, MouseBasePosition));
        }


        public void Initialize()
        {
            Active = true;

            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                this.MapKeys(player, new List<Keys>());
                this.MapMouseButtons(player, new List<MouseButton>());
                this.MapGamePadButtons(player, new List<Buttons>());
            }

            Mouse.SetPosition((int) MouseBasePosition.X, (int) MouseBasePosition.Y);

            Visual.Visuals.GetNotifiedTransition(DoTransitionStarted, DoTransitionStopped);
        }


        public void MapKeys(PlayerIndex player, List<Keys> keys)
        {
            Sources[player].MapKeys(keys);
        }


        public void MapMouseButtons(PlayerIndex player, List<MouseButton> buttons)
        {
            Sources[player].MapMouseButtons(buttons);
        }


        public void MapGamePadButtons(PlayerIndex player, List<Buttons> buttons)
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


        public bool IsKeyPressed(PlayerIndex player, Keys key)
        {
            return Sources[player].IsKeyPressed(key);
        }


        public bool IsMouseButtonPressed(PlayerIndex player, MouseButton button)
        {
            return Sources[player].IsMouseButtonPressed(button);
        }


        public bool IsGamePadButtonPressed(PlayerIndex player, Buttons button)
        {
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
                source.doKeyboardInput();
                source.doMouseInput();
                source.doGamePadInput();
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
