namespace ProjectMercury
{
    using Microsoft.Xna.Framework;
    using ProjectMercury.Renderers;

    /// <summary>
    /// A simple drawable game component that wraps a ParticleEffect instance.
    /// </summary>
    public class ParticleEffectGameComponent : DrawableGameComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEffectGameComponent"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public ParticleEffectGameComponent(Game game)
            : base(game) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEffectGameComponent"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="particleEffect">The particle effect.</param>
        public ParticleEffectGameComponent(Game game, string particleEffectPath, Renderer renderer)
            : this(game)
        {
            this.ParticleEffectPath = particleEffectPath;
            this.Renderer = renderer;
        }

        /// <summary>
        /// Gets or sets the particle effect path.
        /// </summary>
        /// <value>The particle effect path.</value>
        public string ParticleEffectPath { get; set; }

        /// <summary>
        /// Gets or sets the particle effect.
        /// </summary>
        /// <value>The particle effect.</value>
        public ParticleEffect ParticleEffect { get; set; }

        /// <summary>
        /// Gets or sets the renderer.
        /// </summary>
        /// <value>The renderer.</value>
        public Renderer Renderer { get; set; }

        /// <summary>
        /// Initializes the component. Override this method to load any non-graphics resources and query for any required services.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (this.ParticleEffect != null)
                this.ParticleEffect.Initialise();
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        protected override void LoadContent()
        {
            if (this.ParticleEffectPath != null)
            {
                this.ParticleEffect = base.Game.Content.Load<ParticleEffect>(this.ParticleEffectPath);
                this.ParticleEffect.LoadContent(base.Game.Content);
            }

            if (this.Renderer != null)
                this.Renderer.LoadContent(base.Game.Content);
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            if (this.ParticleEffect != null)
            {
                float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                this.ParticleEffect.Update(deltaSeconds);
            }
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn. Override this method with component-specific drawing code. Reference page contains links to related conceptual articles.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (this.Renderer != null)
                this.Renderer.RenderEffect(this.ParticleEffect);
        }
    }
}