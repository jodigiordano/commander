//=====================================================================
//
// Tampon
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    
    internal class Tampon
    {
        //=====================================================================
        // Attributs
        //=====================================================================

        public bool GarderContenu;          // Lors d'un appel a Update, est-ce qu'on clear le buffer
        public Color CouleurDeFond;         // Couleur utilisée pour clearer le buffer
        internal RenderTarget2D Buffer;


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Tampon(int hauteur, int largeur)
        {
            // Initialisation des attributs
            GarderContenu = false;
            CouleurDeFond = Color.TransparentBlack;

            //
            // Création du buffer
            //
            // l'argument pointé (1) indique qu'on n'utilise pas de mipmap (diminue les calculs, mauvais pour zoom in/zoom out)
            // l'argument pointé (2) indique qu'on ne clean pas le buffer avant l'écriture
            // 
            // Ces deux arguments combinés permet de garder la texture dans le buffer et de ne pas devoir la recopier chaque fois
            // pour la rendre accessible (diminue les calculs)
            //

            Buffer = new RenderTarget2D(
                Preferences.GraphicsDeviceManager.GraphicsDevice,
                largeur,
                hauteur,
                1, // <---- (1)
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleType,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleQuality,
                RenderTargetUsage.PreserveContents //<---- 2
            );
        }


        //=====================================================================
        // Services
        //=====================================================================

        public Texture2D Texture
        {
            get { return Buffer.GetTexture(); }
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
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, Buffer);

            // effacer ce qu'il y avait dans le buffer
            if (!GarderContenu)
                Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(CouleurDeFond);
        }


        /// <summary>
        /// Vider manuellement le buffer
        /// </summary>
        public void viderBuffer()
        {
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, Buffer);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(CouleurDeFond);
        }
    }
}
