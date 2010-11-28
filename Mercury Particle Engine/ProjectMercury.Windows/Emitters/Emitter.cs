/*  
 Copyright � 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Emitters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Modifiers;

    /// <summary>
    /// Defines an event handler for a Particle related event.
    /// </summary>
    /// <param name="copy">The Emitter which raised the event.</param>
    /// <param name="particle">The Particle which is the subject of the event.</param>
    public delegate void ParticleEventHandler(Emitter emitter, ref Particle particle);

    /// <summary>
    /// Defines the base class for a Particle Emitter. The basic implementation releases Particles from a single point.
    /// </summary>
#if WINDOWS
    [TypeConverter("ProjectMercury.Design.Emitters.EmitterTypeConverter, ProjectMercury.Design")]
#endif
    public class Emitter
    {
        static private int CreationIndex;

        /// <summary>
        /// Gets a default name for the next Emitter.
        /// </summary>
        static internal string NextEmitterName()
        {
            return String.Format("Emitter{0:00}", Emitter.CreationIndex++);
        }

        private string _name;

        /// <summary>
        /// Gets or sets the name of the Emitter.
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set
            {
                Guard.ArgumentNullOrEmpty("Name", value);

                if (this.Name != value)
                {
                    this._name = value;

                    this.OnNameChanged(EventArgs.Empty);
                }
            }
        }

        private float TotalSeconds;

        [ContentSerializerIgnore]
        public bool Initialised { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating wether or not the Emitter is enabled (can be triggered).
        /// </summary>
        [ContentSerializer(Optional = true)]
        public bool Enabled;

        private int _budget;

        /// <summary>
        /// Gets or sets the number of Particles which are available to the Emitter.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if trying to set this property after the Emitter has been initialised.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the specified value is less than 1.</exception>
        public int Budget
        {
            get { return this._budget; }
            set
            {
                Guard.IsTrue(this.Initialised, "Cannot alter Budget after Emitter is initialised.");
                Guard.ArgumentLessThan("Budget", value, 1);

                this._budget = value;
            }
        }

        private float _term;

        /// <summary>
        /// Gets or sets the length of time that released Particles will remain active, in whole and fractional seconds.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if trying to set this property after the Emitter has been initialised.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the supplied value is less than or equal to 0.</exception>
        public float Term
        {
            get { return this._term; }
            set
            {
                Guard.IsTrue(this.Initialised, "Cannot alter Term after Emitter is initialised.");
                Guard.ArgumentNotFinite("Term", value);
                Guard.ArgumentLessThan("Term", value, float.Epsilon);

                this._term = value;
            }
        }

        [ContentSerializerIgnore]
        public Particle[] Particles;

        private int Idle;

        private int _releaseQuantity;

        /// <summary>
        /// Gets or sets the number of Particles which will be released on each trigger.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the specified value is less than 1.</exception>
        public int ReleaseQuantity
        {
            get { return this._releaseQuantity; }
            set
            {
                Guard.ArgumentLessThan("ReleaseQuantity", value, 1);

                this._releaseQuantity = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed at which Particles travel when they are released.
        /// </summary>
        public VariableFloat ReleaseSpeed;

        /// <summary>
        /// Gets or sets the colour of released Particles.
        /// </summary>
        public VariableFloat3 ReleaseColour;

        /// <summary>
        /// Gets or sets the opacity of released Particles.
        /// </summary>
        public VariableFloat ReleaseOpacity;

        /// <summary>
        /// Gets or sets the scale of released particles.
        /// </summary>
        public VariableFloat ReleaseScale;

        /// <summary>
        /// Gets or sets the rotation of released Particles.
        /// </summary>
        public VariableFloat ReleaseRotation;

        /// <summary>
        /// Gets or sets the initial impulse applied to Particles as they are relased.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Vector2 ReleaseImpulse;

        /// <summary>
        /// Gets the asset name of a texture to load in the LoadContent method.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public string ParticleTextureAssetName;

        /// <summary>
        /// Gets or sets the Texture2D used to display the Particles.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D ParticleTexture;

        /// <summary>
        /// Gets the collection of Modifiers which are acting upon the Emitter.
        /// </summary>
        public ModifierCollection Modifiers;

        /// <summary>
        /// Gets or sets the ITagProvider implementation used to attach custom data to Particles.
        /// </summary>
        [ContentSerializerIgnore]
        public ITagProvider TagProvider;

        /// <summary>
        /// The array of custom tags which are currently attached to the active Particles.
        /// </summary>
        [ContentSerializerIgnore]
        private object[] Tags;

        /// <summary>
        /// The blending mode to be used by Renderers when rendering this Emitter.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public BlendMode BlendMode;

        /// <summary>
        /// The Emitters trigger offset in relation to the ParticleEffect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Vector2 TriggerOffset;

        /// <summary>
        /// Defines the minimum amount of time between triggers for the Emitter, expressed in
        /// whole and fractional seconds. Triggers which occur during this period will be ignored.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float MinimumTriggerPeriod;

        /// <summary>
        /// Stores the time at which the Emitter was most recently triggered.
        /// </summary>
        private float MostRecentTrigger;

        /// <summary>
        /// Gets the number of Particles which are currently active.
        /// </summary>
        public int ActiveParticlesCount
        {
            get { return this.Idle; }
        }

        /// <summary>
        /// Raised when the name of the Emitter has been changed.
        /// </summary>
        public event EventHandler NameChanged;

        /// <summary>
        /// Raises the NameChanged event.
        /// </summary>
        protected virtual void OnNameChanged(EventArgs e)
        {
            if (this.NameChanged != null)
                this.NameChanged(this, e);
        }

        /// <summary>
        /// Raised when a Particle is released by the Emitter.
        /// </summary>
        public event ParticleEventHandler ParticleReleased;

        /// <summary>
        /// Raises the ParticleReleased event.
        /// </summary>
        /// <param name="particle">The particle which has been released.</param>
        protected virtual void OnParticleReleased(ref Particle particle)
        {
            if (this.ParticleReleased != null)
                this.ParticleReleased(this, ref particle);
        }

        /// <summary>
        /// Raised when a Particle expires.
        /// </summary>
        public event ParticleEventHandler ParticleRetired;

        /// <summary>
        /// Raises the ParticleRetired event.
        /// </summary>
        /// <param name="particle">The particle which has been retired.</param>
        protected virtual void OnParticleRetired(ref Particle particle)
        {
            if (this.ParticleRetired != null)
                this.ParticleRetired(this, ref particle);
        }

        /// <summary>
        /// Instantiates a new instance of the Emitter class.
        /// </summary>
        public Emitter()
        {
            this.Name = Emitter.NextEmitterName();

            this.Enabled = true;

            this.Modifiers = new ModifierCollection();
        }

        /// <summary>
        /// Returns an unitialised deep copy of the Emitter.
        /// </summary>
        /// <returns>A deep copy of the Emitter.</returns>
        public virtual Emitter DeepCopy()
        {
            Emitter emitter = new Emitter();

            this.CopyBaseFields(emitter);

            return emitter;
        }

        /// <summary>
        /// Copies the fields of the Emitter base class into the specified Emitter.
        /// </summary>
        /// <param name="copy">The Emitter which will be copied into.</param>
        protected void CopyBaseFields(Emitter emitter)
        {
            emitter.BlendMode                = this.BlendMode;
            emitter.Budget                   = this.Budget;
            emitter.Enabled                  = this.Enabled;
            emitter.MinimumTriggerPeriod     = this.MinimumTriggerPeriod;
            emitter.Modifiers                = this.Modifiers.DeepCopy();
            emitter.Name                     = String.Format("Copy of {0}", this.Name);
            emitter.ParticleTexture          = this.ParticleTexture;
            emitter.ParticleTextureAssetName = String.Copy(this.ParticleTextureAssetName ?? String.Empty);
            emitter.ReleaseColour            = this.ReleaseColour;
            emitter.ReleaseOpacity           = this.ReleaseOpacity;
            emitter.ReleaseQuantity          = this.ReleaseQuantity;
            emitter.ReleaseRotation          = this.ReleaseRotation;
            emitter.ReleaseScale             = this.ReleaseScale;
            emitter.ReleaseSpeed             = this.ReleaseSpeed;
            emitter.ReleaseImpulse           = this.ReleaseImpulse;
            emitter.TagProvider              = this.TagProvider;
            emitter.Term                     = this.Term;
            emitter.TriggerOffset            = this.TriggerOffset;

        }

        /// <summary>
        /// Initialises the Emitter.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the Term and/or Budget properties have not been set.</exception>
        public void Initialise()
        {
            Guard.IsTrue(this.Term < float.Epsilon, "Term property has not been assigned a valid value.");
            Guard.IsTrue(this.Budget < 1, "Budget property has not been assigned a valid value.");

            this.Particles = new Particle[this.Budget];

            this.Tags = new object[this.Budget];

            this.Idle = 0;

            this.TotalSeconds = 0f;
            this.MostRecentTrigger = 0f;

            this.Initialised = true;
        }

        /// <summary>
        /// Initialises the Emitter.
        /// </summary>
        /// <param name="budget">The number of Particles which are available to the Emitter.</param>
        /// <param name="term">The length of time that released Particles will remain active, in whole and fractional seconds.</param>
        /// <exception cref="System.ArgumentException">Thrown if the budget parameter is less than one, or if the term paramter
        /// is not a positive value.</exception>
        public void Initialise(int budget, float term)
        {
            Guard.ArgumentNotFinite("Term", term);
            Guard.ArgumentLessThan("budget", budget, 1);
            Guard.ArgumentLessThan("term", term, float.Epsilon);

            this.Initialised = false;

            this.Budget = budget;
            this.Term = term;

            if (this.TagProvider != null)
                for (int i = 0; i < this.Idle; i++)
                    this.TagProvider.DisposeTag(this.Tags[i]);

            this.Initialise();
        }

        /// <summary>
        /// Terminates the emitter immediately.
        /// </summary>
        public void Terminate()
        {
            this.Idle = 0;
        }

        /// <summary>
        /// Forces the Emitter to execute its next trigger, even if it has a minimum trigger period and is
        /// currently 'cooling down'.
        /// </summary>
        public void ForceNextTrigger()
        {
            this.MostRecentTrigger = 0f;
        }

        /// <summary>
        /// Loads resources required by the Emitter via a ContentManager.
        /// </summary>
        /// <param name="content">The ContentManager used to load resources.</param>
        /// <exception cref="Microsoft.Xna.Framework.Content.ContentLoadException">Thrown if the asset defined
        /// in the ParticleTextureAssetName property could not be loaded.</exception>
        public void LoadContent(ContentManager content)
        {
            Guard.ArgumentNull("content", content);

            if (String.IsNullOrEmpty(this.ParticleTextureAssetName) == false)
            {
                try
                {
                    if (this.ParticleTexture == null)
                        this.ParticleTexture = content.Load<Texture2D>(this.ParticleTextureAssetName);
                }
                catch (ContentLoadException e)
                {
                    string msg = String.Format(@"Unable to load the specified content item '{0}'
                                                 Please check the 'ParticleTextureAssetName' property!",
                                               this.ParticleTextureAssetName);

                    throw new ContentLoadException(msg, e);
                }
            }
        }

        /// <summary>
        /// Retires the specified number of Particles.
        /// </summary>
        private void RetireParticles(int count)
        {
            if (this.TagProvider != null)
            {
                // Instruct the ITagProvider to remove tags for the retired Particles...
                for (int i = 0; i < count; i++)
                    this.TagProvider.DisposeTag(this.Tags[i]);

                // Move the remaining tags to the front of the Tags array...
                Array.Copy(this.Tags, count, this.Tags, 0, this.Idle - count);
            }

            // Move the remaining particles to the front of the Particles array...
            Array.Copy(this.Particles, count, this.Particles, 0, this.Idle - count);

            // The Array.Copy method appears to be slow in some circumstances. In those situations
            // it should be faster to use the following code instead...
            /*
            for (int i = count; i < this.Idle; i++)
            {
                this.Particles[i - count] = this.Particles[i];
                this.Tags[i - count] = this.Tags[i];
            }
            */

            // Decrement the idle marker accordingly...
            this.Idle -= count;
        }

        // Possible alternative implementation - need to benchmark...
        private unsafe void RetireParticles(Particle* particleArray, int count)
        {
            Particle* src = (particleArray + count);
            Particle* dst = particleArray;

            int num = this.Idle - count;

            for (int i = 0; i < num; i++)
            {
                *dst = *src;

                src++;
                dst++;
            }

            this.Idle -= count;
        }

        /// <summary>
        /// Updates the Emitter and all Particles within.
        /// </summary>
        /// <param name="deltaSeconds">Elapsed frame time in whole and fractional seconds.</param>
        public unsafe void Update(float deltaSeconds)
        {
            Guard.IsFalse(this.Initialised, "Emitter has not been initialised.");
            Guard.ArgumentNotFinite("deltaSeconds", deltaSeconds);

            this.TotalSeconds += deltaSeconds;

            // Track the number of Particles which have expired...
            int expiredCount = 0;

            // Flag whether or not we need to check the age of the remaining Particles...
            bool checkAge = true;

            // Create a fixed pointer to the particle array...
            fixed (Particle* particleArray = this.Particles)
            {
                // Begin looping through all the particles in the array...
                for (int i = 0; i < this.Idle; i++)
                {
                    Particle* particle = (particleArray + i);

                    float actualAge = this.TotalSeconds - particle->Inception;

                    if (checkAge)
                    {
                        if (actualAge > this.Term)
                        {
                            expiredCount++;

                            continue;
                        }
                        else
                        {
                            checkAge = false;
                        }
                    }

                    // Update the Particle (inlined from Particle.Update method)...
                    particle->Age = actualAge / this.Term;

                    particle->Momentum.X += particle->Velocity.X;
                    particle->Momentum.Y += particle->Velocity.Y;

                    particle->Velocity.X = particle->Velocity.Y = 0f;

                    particle->Position.X += particle->Momentum.X * deltaSeconds;
                    particle->Position.Y += particle->Momentum.Y * deltaSeconds;
                }

                // Retire particles if necessary...
                if (expiredCount > 0)
                    this.RetireParticles(expiredCount);
                    //this.RetireParticles(particleArray, expiredCount);

                // Send the particle array to the modifiers for processing...
                this.Modifiers.RunProcessors(deltaSeconds, particleArray, this.ActiveParticlesCount);
            }
        }

        /// <summary>
        ///  Triggers the Emitter at the specified position...
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the Emitter has not been initialised.</exception>
        public unsafe void Trigger(ref Vector2 triggerPosition)
        {
            Guard.IsFalse(this.Initialised, "Emitter has not been initialised.");

            // Bail if the Emitter is disabled...
            if (this.Enabled == false)
                return;

            // Bail if the Emitter is still in its cool down period...
            if (this.TotalSeconds - this.MostRecentTrigger < this.MinimumTriggerPeriod)
                return;

            // Add the Emitter offset vector to the trigger position...
            Vector2 position = new Vector2
            {
                X = triggerPosition.X + this.TriggerOffset.X,
                Y = triggerPosition.Y + this.TriggerOffset.Y,
            };

            int oldIdle = this.Idle;

            for (int i = oldIdle; i < oldIdle + this.ReleaseQuantity; i++)
            {
                if (i < this.Budget)
                {
                    fixed (Particle* particle = &this.Particles[i])
                    {
                        Vector2 offset, force;

                        // Generate and offset and force vector for the particle...
                        this.GenerateOffsetAndForce(out offset, out force);

                        // Calculate the velocity of the particle using the force vector and the release velocity...
                        float speed = this.ReleaseSpeed.Sample();

                        particle->Inception  = this.TotalSeconds;
                        particle->Position.X = position.X + offset.X;
                        particle->Position.Y = position.Y + offset.Y;
                        particle->Velocity.X = force.X * speed;
                        particle->Velocity.Y = force.Y * speed;
                        particle->Momentum   = this.ReleaseImpulse;
                        particle->Age        = 0f;
                        particle->Colour     = new Vector4(this.ReleaseColour.Sample(), this.ReleaseOpacity.Sample());
                        particle->Scale      = this.ReleaseScale.Sample();
                        particle->Rotation   = 0f;
                        particle->Rotate(this.ReleaseRotation.Sample());

                        // Instruct the ITagProvider to provide a tag for the Particle...
                        if (this.TagProvider != null)
                            this.Tags[i] = this.TagProvider.GetTag(ref *particle);

                        this.OnParticleReleased(ref *particle);
                    }

                    this.Idle++;
                }
                else
                {
                    // There are no more idle Particles...
                    break;
                }
            }

            this.MostRecentTrigger = this.TotalSeconds;
        }

        /// <summary>
        ///  Triggers the Emitter at the specified position...
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the Emitter has not been initialised.</exception>
        public void Trigger(Vector2 position)
        {
            this.Trigger(ref position);
        }

        /// <summary>
        /// Generates an offset vector and force vector for a Particle when it is released.
        /// </summary>
        /// <param name="offset">The offset of the Particle from the trigger location.</param>
        /// <param name="force">A unit vector defining the initial force of the Particle.</param>
        protected virtual void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
        {
            offset = Vector2.Zero;

            force = RandomHelper.NextUnitVector();
        }
    }
}