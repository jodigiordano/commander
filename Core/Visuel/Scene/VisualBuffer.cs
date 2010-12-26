namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    
    internal class VisualBuffer
    {
        public bool GarderContenu;          // Lors d'un appel a Update, est-ce qu'on clear le buffer
        public Color CouleurDeFond;         // Couleur utilisée pour clearer le buffer
        internal RenderTarget2D Buffer;

        
        public VisualBuffer(int hauteur, int largeur)
        {
            GarderContenu = false;
            CouleurDeFond = Color.Transparent;

            Buffer = new RenderTarget2D(
                Preferences.GraphicsDeviceManager.GraphicsDevice,
                largeur,
                hauteur,
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


        public int Largeur
        {
            get { return Buffer.Width; }
        }


        public int Hauteur
        {
            get { return Buffer.Height; }
        }


        public Vector2 Centre
        {
            get { return new Vector2(Largeur, Hauteur) / 2.0f; }
        }


        public void EcrireDebut()
        {
            // fait basculer le Render Target sur ce buffer, ce qui indique que
            // tout spriteBatch.Draw se fait dans ce buffer
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Buffer);

            // effacer ce qu'il y avait dans le buffer
            if (!GarderContenu)
                Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(CouleurDeFond);
        }


        public void viderBuffer()
        {
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Buffer);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(CouleurDeFond);
        }
    }
}
