namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

#if WINDOWS
    using System.Net;
    using System.Collections.Specialized;
#endif


    public static class ErrorHandling
    {
        public static void Run<T>(int width, int height, bool fullscreen, Version version) where T : Game, new()
        {
            if (Debugger.IsAttached)
            {
                using (var g = new T())
                    g.Run();
            }
            else
            {
                try
                {
                    using (var g = new T())
                        g.Run();
                }
                catch (Exception e)
                {
                    using (var g = new ExceptionGame(e, width, height, fullscreen, version))
                        g.Run();
                }
            }
        }
    }


    class ExceptionGame : Game
    {
        private enum State
        {
            NotSend,
            Sending,
            Sent
        }

#if WINDOWS
        private WebClient ClientWeb;
#endif

        private string ScriptUrl = "http://www.ephemeregames.com/utilities/reporterror.php";

        private string errorTitle = "Oh man! :(";
        private string errorMessage;

        private Dictionary<State, string> SendingStateMessages;

        private readonly Exception exception;

        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D fade;
        private State SendingState;
        private Version Version;


        public ExceptionGame(Exception e, int width, int height, bool fullscreen, Version version)
        {
            new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height,
                IsFullScreen = fullscreen
            };

            exception = e;
            Content.RootDirectory = "Content";
            Version = version;

#if WINDOWS
            ClientWeb = new WebClient();
#endif

            SendingState = State.NotSend;
            SendingStateMessages = new Dictionary<State, string>();
            SendingStateMessages.Add(State.NotSend, "Press the Left mouse button OR the A button on a gamepad to send the message.\nPress the Right mouse button OR the B button on a gamepad to exit.");
            SendingStateMessages.Add(State.Sending, "Sending...\nPlease wait.");
            SendingStateMessages.Add(State.Sent, "Thank you, Commander!\nPress the Left mouse button OR the A button on a gamepad to exit.");

            errorMessage =
               "The game had an unexpected error and had to shut down. I'm sooooo sorry for the inconvenience!\n" +
               "The good news is that you can send me the error via the interwebz by pressing the Left mouse button OR\n" +
               "the A button on a gamepad. By doing so, you will greatly help to make this game better and better.\n\n" +
               "With a little bit of luck, it should not happen again if you restart the game. Even more, the bug may already fixed\n" +
               "in a more recent version! Your current version is " + Version.ToString() + ". Please visit commander.ephemeregames.com\n" +
               "to know how to upgrade the game to the current version! By doing so, you could also benefit from new stuff\n" +
               "that I'm adding on every update!";
        }


        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Debug");
            fade = Content.Load<Texture2D>("fade");
        }


        protected override void Update(GameTime gameTime)
        {

            MouseState mouseState = Mouse.GetState();
            List<GamePadState> gamepadStates = new List<GamePadState>();
            gamepadStates.Add(GamePad.GetState(PlayerIndex.One));
            gamepadStates.Add(GamePad.GetState(PlayerIndex.Two));
            gamepadStates.Add(GamePad.GetState(PlayerIndex.Three));
            gamepadStates.Add(GamePad.GetState(PlayerIndex.Four));

            if (SendingState == State.NotSend &&
                (mouseState.LeftButton == ButtonState.Pressed ||
                gamepadStates[0].Buttons.A == ButtonState.Pressed ||
                gamepadStates[1].Buttons.A == ButtonState.Pressed ||
                gamepadStates[2].Buttons.A == ButtonState.Pressed ||
                gamepadStates[3].Buttons.A == ButtonState.Pressed))
            {
                SendingState = State.Sending;

#if WINDOWS
                NameValueCollection inputs = new NameValueCollection();
                inputs.Add("source", exception.Source);
                inputs.Add("version", Version.ToString());
                inputs.Add("message", exception.Message);
                inputs.Add("stacktrace", exception.StackTrace);

                ClientWeb.UploadValuesCompleted += new UploadValuesCompletedEventHandler(SendCompleted);
                ClientWeb.UploadValuesAsync(new Uri(ScriptUrl), inputs);
#endif
            }

            else if (((SendingState == State.NotSend || SendingState == State.Sent) &&
                (mouseState.RightButton == ButtonState.Pressed ||
                gamepadStates[0].Buttons.B == ButtonState.Pressed ||
                gamepadStates[1].Buttons.B == ButtonState.Pressed ||
                gamepadStates[2].Buttons.B == ButtonState.Pressed ||
                gamepadStates[3].Buttons.B == ButtonState.Pressed)) ||
                (SendingState == State.Sent &&
                (mouseState.LeftButton == ButtonState.Pressed ||
                gamepadStates[0].Buttons.A == ButtonState.Pressed ||
                gamepadStates[1].Buttons.A == ButtonState.Pressed ||
                gamepadStates[2].Buttons.A == ButtonState.Pressed ||
                gamepadStates[3].Buttons.A == ButtonState.Pressed)))
            {
                this.Exit();
            }

            base.Update(gameTime);
        }


#if WINDOWS
        private void SendCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            SendingState = State.Sent;
        }
#endif


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Vector2 positionBase = new Vector2(
               GraphicsDevice.Viewport.TitleSafeArea.X + 50,
               GraphicsDevice.Viewport.TitleSafeArea.Y + 50);

            batch.Begin();
            batch.DrawString(font, errorTitle, positionBase, Color.White);
            batch.DrawString(font, errorMessage, positionBase + new Vector2(0, 50), Color.White);
            batch.DrawString(font, SendingStateMessages[SendingState], positionBase + new Vector2(0, 250), new Color(182, 255, 0));
            batch.DrawString(font, exception.ToString(), positionBase + new Vector2(0, 350), Color.White);
            batch.Draw(fade, Vector2.Zero, new Color(196, 0, 0));
            batch.End();

            base.Draw(gameTime);
        }
    }
}
