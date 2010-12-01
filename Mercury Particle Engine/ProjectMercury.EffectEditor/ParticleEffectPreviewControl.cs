﻿/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Renderers;

    public enum ImageOptions
    {
        Stretch,
        Center,
        Tile
    }

    public class ParticleEffectPreviewControl : GraphicsDeviceControl
    {
        protected override void Initialize()
        {
            this.SpriteBatch = new SpriteBatch(base.GraphicsDevice);
        }

        private Vector2 Origin { get; set; }

        private ImageOptions ImageOptions { get; set; }

        public ParticleEffect ParticleEffect { get; set; }

        public Renderer Renderer { get; set; }

        private Vector3 BackgroundColour;

        private SpriteBatch SpriteBatch;
        new private Texture2D BackgroundImage;

        public void SetBackgroundColor(byte r, byte g, byte b)
        {
            this.BackgroundColour.X = r / 255f;
            this.BackgroundColour.Y = g / 255f;
            this.BackgroundColour.Z = b / 255f;
        }

        public void LoadBackgroundImage(string filePath)
        {
            this.BackgroundImage = Texture2D.FromFile(base.GraphicsDevice, filePath);
            
            this.Origin = new Vector2(BackgroundImage.Width / 2, BackgroundImage.Height / 2);
        }

        public void ClearBackgroundImage()
        {
            if (this.BackgroundImage != null)
            {
                this.BackgroundImage.Dispose();
                this.BackgroundImage = null;
            }
        }

        protected override void Draw()
        {
            base.GraphicsDevice.Clear(new Color(this.BackgroundColour));

            if (this.BackgroundImage != null)
            {
                this.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                // Is this line necessary?
                base.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

                switch (this.ImageOptions)
                {
                    case ImageOptions.Stretch:
                        {
                            this.SpriteBatch.Draw(this.BackgroundImage, new Rectangle(0, 0, base.ClientSize.Width, base.ClientSize.Height), Color.White);
                         
                            break;
                        }
                    case ImageOptions.Center:
                        {
                            this.SpriteBatch.Draw(this.BackgroundImage, new Rectangle(base.ClientSize.Width / 2, base.ClientSize.Height / 2, this.BackgroundImage.Width, this.BackgroundImage.Height), null, Color.White, 0, this.Origin, SpriteEffects.None, 1);
                         
                            break;
                        }
                    case ImageOptions.Tile:
                        {
                            for (int x = 0; x < this.Width; x += this.BackgroundImage.Width)
                            {
                                for (int y = 0; y < this.Height; y += this.BackgroundImage.Height)
                                {
                                    this.SpriteBatch.Draw(this.BackgroundImage, new Rectangle(x, y, this.BackgroundImage.Width, this.BackgroundImage.Height), Color.White);
                                }
                            }
                         
                            break;
                        }
                }

                this.SpriteBatch.End();
            }

            if (this.Renderer != null)
                if (this.ParticleEffect != null)
                    this.Renderer.RenderEffect(this.ParticleEffect);
        }

        internal void ImageOptionsChanged(ImageOptions imageOptions)
        {
            this.ImageOptions = imageOptions;
        }
    }
}