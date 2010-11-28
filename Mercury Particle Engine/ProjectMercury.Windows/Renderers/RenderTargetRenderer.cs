/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Renderers
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;

    /// <summary>
    /// Defines a renderer which renders to a RenderTarget using a specified renderer.
    /// </summary>
    public sealed class RenderTargetRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetRenderer"/> class.
        /// </summary>
        public RenderTargetRenderer() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetRenderer"/> class.
        /// </summary>
        /// <param name="innerRenderer">The renderer which will be used to render to the render target.</param>
        public RenderTargetRenderer(Renderer innerRenderer)
        {
            this.InnerRenderer = innerRenderer;
        }

        /// <summary>
        /// The renderer which will be used to render to the render target.
        /// </summary>
        public Renderer InnerRenderer { get; set; }

        /// <summary>
        /// Gets the render target.
        /// </summary>
        /// <value>The render target.</value>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the render target index to use, or uses zero if null.
        /// </summary>
        public int? RenderTargetIndex { get; set; }

        /// <summary>
        /// Loads any content needed by the Renderer.
        /// </summary>
        public override void LoadContent(ContentManager content)
        {
            if (base.GraphicsDeviceService == null)
                throw new InvalidOperationException("Need a reference to graphics device service.");

            base.LoadContent(content);

            GraphicsDevice graphicsDevice = base.GraphicsDeviceService.GraphicsDevice;

            PresentationParameters presentationParams = graphicsDevice.PresentationParameters;

            this.RenderTarget = new RenderTarget2D(graphicsDevice, presentationParams.BackBufferWidth,
                                                                   presentationParams.BackBufferHeight,
                                                                   1, graphicsDevice.DisplayMode.Format);

            if (this.InnerRenderer != null)
                this.InnerRenderer.LoadContent(content);
        }

        /// <summary>
        /// Renders the specified Emitter, applying the specified transformation offset.
        /// </summary>
        public override void RenderEmitter(Emitter emitter, ref Matrix transform)
        {
            if (this.InnerRenderer == null)
                return;

            GraphicsDevice graphicsDevice = base.GraphicsDeviceService.GraphicsDevice;

            graphicsDevice.SetRenderTarget(this.RenderTargetIndex.GetValueOrDefault(0), this.RenderTarget);

            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.TransparentBlack, 0f, 0);

            this.InnerRenderer.RenderEmitter(emitter, ref transform);

            graphicsDevice.SetRenderTarget(this.RenderTargetIndex.GetValueOrDefault(0), null);
        }

        /// <summary>
        /// Sets the render state of the graphics device before rendering an Emitter.
        /// </summary>
        /// <param name="emitter">The emitter which is about to be rendered.</param>
        protected override void SetRenderState(Emitter emitter)
        {
            base.SetRenderState(emitter);

            RenderState state = base.GraphicsDeviceService.GraphicsDevice.RenderState;

            state.SeparateAlphaBlendEnabled = true;
            state.AlphaSourceBlend = Blend.One;
            state.AlphaDestinationBlend = Blend.InverseSourceAlpha;
        }
    }
}