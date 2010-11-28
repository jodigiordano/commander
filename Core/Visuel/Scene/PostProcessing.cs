//=====================================================================
//
// Effets de post-processing
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public static class PostProcessing
    {
        private static ResolveTexture2D textureBackBuffer;
        private static Effect effetPostProcessing;

        public static Scene SceneDessinee { get; set; }
        public static Scene SceneHote { get; set; }

        private static SpriteBatch spriteBatch;
        private static float offsetLuminosite;
        private static float offsetContraste;
        private static Scene sceneAveugle;
        private static bool aveugle = false;


        public static void init()
        {
            PostProcessing.effetPostProcessing = Preferences.Content.Load<Effect>("PostProcessing");

            PostProcessing.textureBackBuffer = new ResolveTexture2D(
                Preferences.GraphicsDeviceManager.GraphicsDevice,
                Preferences.FenetreLargeur,
                Preferences.FenetreHauteur,
                1,
                Preferences.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat);

            PostProcessing.spriteBatch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
        }

        public static void appliquerLuminositeContraste(float offsetLuminosite, float offsetContraste)
        {
            if (textureBackBuffer == null)
                return;

            PostProcessing.offsetLuminosite = offsetLuminosite;
            PostProcessing.offsetContraste = offsetContraste;

            Preferences.GraphicsDeviceManager.GraphicsDevice.ResolveBackBuffer(textureBackBuffer);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            effetPostProcessing.CurrentTechnique = effetPostProcessing.Techniques["LuminositeContraste"];
            effetPostProcessing.Parameters["offsetLuminosite"].SetValue(offsetLuminosite);
            effetPostProcessing.Parameters["offsetContraste"].SetValue(offsetContraste);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            effetPostProcessing.Begin();

            foreach (EffectPass passe in effetPostProcessing.CurrentTechnique.Passes)
            {
                passe.Begin();

                spriteBatch.Draw(textureBackBuffer, Vector2.Zero, Color.White);

                passe.End();
            }

            effetPostProcessing.End();

            spriteBatch.End();
        }

        public static void setAveugle(Scene sceneAveugle)
        {
            PostProcessing.aveugle = sceneAveugle != null;
            PostProcessing.sceneAveugle = sceneAveugle;
        }

        public static void appliquerEffets()
        {
            if (aveugle)
            {
                sceneAveugle.Draw();

                Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, null);
                Texture2D texture = SceneHote.Texture;

                Preferences.GraphicsDeviceManager.GraphicsDevice.Textures[1] = sceneAveugle.Texture;
                effetPostProcessing.CurrentTechnique = effetPostProcessing.Techniques["Masque"];

                Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, SceneHote.Tampon.Buffer);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                effetPostProcessing.Begin();

                foreach (EffectPass passe in effetPostProcessing.CurrentTechnique.Passes)
                {
                    passe.Begin();
                    spriteBatch.Draw(texture, Vector2.Zero, Color.White);
                    passe.End();
                }

                effetPostProcessing.End();
                spriteBatch.End();

            }
        }
    }
}
