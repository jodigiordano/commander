namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    internal class VisualBuffer
    {
        public bool KeepContent;
        public Color ClearColor;

        internal RenderTarget2D Buffer;


        public VisualBuffer(int width, int height)
        {
            KeepContent = false;
            ClearColor = Color.Transparent;

            Buffer = new RenderTarget2D(
                Preferences.GraphicsDeviceManager.GraphicsDevice,
                width,
                height,
                false,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.PreserveContents
            );
        }


        public Texture2D Texture
        {
            get { return Buffer; }
        }


        public int Width
        {
            get { return Buffer.Width; }
        }


        public int Height
        {
            get { return Buffer.Height; }
        }


        public Vector2 Center
        {
            get { return new Vector2(Width, Height) / 2.0f; }
        }


        public void BeginWriting()
        {
            // fait basculer le Render Target sur ce buffer, ce qui indique que
            // tout spriteBatch.Draw se fait dans ce buffer
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Buffer);

            // effacer ce qu'il y avait dans le buffer
            if (!KeepContent)
                Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(ClearColor);
        }


        public void Clear()
        {
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Buffer);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(ClearColor);
        }
    }
}
