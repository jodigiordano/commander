namespace ProjectMercury
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Renderers;

    /// <summary>
    /// A simple particle effect manager, ideal for consuming particle effects as a service.
    /// </summary>
    public class ParticleEffectManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEffectManager"/> class.
        /// </summary>
        public ParticleEffectManager()
        {
            this.Effects = new Dictionary<string, ParticleEffect>();
        }

        /// <summary>
        /// Gets or sets the effects dictionary.
        /// </summary>
        /// <value>The effects dictionary.</value>
        private Dictionary<string, ParticleEffect> Effects { get; set; }

        /// <summary>
        /// Gets or sets the renderer which will be used to render the effects.
        /// </summary>
        /// <value>The renderer.</value>
        public Renderer Renderer { get; set; }

        /// <summary>
        /// Adds the specified particle effect.
        /// </summary>
        /// <param name="effect">The particle effect.</param>
        /// <param name="name">The name of the effect.</param>
        public void Add(ParticleEffect effect, string name)
        {
            this.Effects.Add(name, effect);
        }

        /// <summary>
        /// Removes the particle effect with the specified name.
        /// </summary>
        /// <param name="name">The name of the particle effect.</param>
        public void Remove(string name)
        {
            this.Effects.Remove(name);
        }

        /// <summary>
        /// Gets an enumeration of all the particle effects.
        /// </summary>
        /// <value>The particle effects.</value>
        public IEnumerable<ParticleEffect> ParticleEffects
        {
            get
            {
                foreach (var pair in this.Effects)
                    yield return pair.Value;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="ProjectMercury.ParticleEffect"/> with the specified name.
        /// </summary>
        public ParticleEffect this[string name]
        {
            get { return this.Effects[name]; }
        }

        /// <summary>
        /// Updates all the particle effects.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var effect in this.ParticleEffects)
                effect.Update(deltaSeconds);
        }

        /// <summary>
        /// Draws all the particle effects using the renderer referenced by the Renderer property.
        /// </summary>
        public void Draw()
        {
            if (this.Renderer != null)
                foreach (var effect in this.ParticleEffects)
                    this.Renderer.RenderEffect(effect);
        }

        /// <summary>
        /// Draws the particles effects in the order specified by the string array.
        /// </summary>
        /// <param name="order">An array of strings containing the names of the particle effects.</param>
        public void Draw(string[] order)
        {
            if (this.Renderer != null)
            {
                foreach (string name in order)
                    this.Renderer.RenderEffect(this[name]);
            }
        }
    }
}