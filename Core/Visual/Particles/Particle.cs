namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury;
    using ProjectMercury.Emitters;
    using ProjectMercury.Renderers;


    enum State
    {
        Idle,
        Active,
        Dying,
        Dead
    }


    public class Particle : IScenable, IAsset
    {
        public string Name;
        public double VisualPriority            { get; set; }
        public Vector3 Position                 { get; set; }
        public Scene Scene                      { get; set; }
        public int Id                           { get; private set; }
        public ParticleEffect Model;
        public SpriteBatchRenderer Renderer;
        internal State State;

        private BlendType blend;
        private bool ContentLoaded;

        
        public Particle()
        {
            Name = "";
            Position = Vector3.Zero;
            Blend = BlendType.Add;
            VisualPriority = 0;
            State = State.Idle;
            Id = Visuals.NextHashCode;
            ContentLoaded = false;
        }


        public void LoadContent()
        {
            if (ContentLoaded)
                return;

            ParticleEffect model = EphemereGames.Core.SimplePersistence.Persistence.GetAsset<Particle>(Name).Model;

            Model = model.DeepCopy();

            Model.Initialise();

            ContentLoaded = true;
        }


        public void Initialize()
        {
            foreach (var emitter in Model)
                emitter.Terminate();
        }


        public BlendType Blend
        {
            get { return blend; }
            set
            {
                blend = value;

                if (Model == null)
                    return;

                foreach (var effect in Model)
                    effect.BlendMode = ToMercuryBlend(value);
            }
        }


        public void Trigger(ref Vector2 position)
        {
            this.Model.Trigger(ref position);
        }


        public void Trigger(ref Vector3 position)
        {
            Vector2 position2d = new Vector2(position.X, position.Y);

            this.Model.Trigger(ref position2d);
        }


        public void Move(ref Vector2 varPosition)
        {
            for (int i = 0; i < Model.Count; i++)
                for (int j = 0; j < Model[i].ActiveParticlesCount; j++)
                    Vector2.Add(ref Model[i].Particles[j].Position, ref varPosition, out Model[i].Particles[j].Position);
        }


        public void Move(ref Vector3 varPosition)
        {
            Vector2 position2d = new Vector2(varPosition.X, varPosition.Y);

            Move(ref position2d);
        }


        private static BlendType ToCoreBlend(EmitterBlendMode mercury)
        {
            BlendType blend;

            switch (mercury)
            {
                case EmitterBlendMode.Add:          blend = BlendType.Add;        break;
                case EmitterBlendMode.Alpha:        blend = BlendType.Alpha;      break;
                case EmitterBlendMode.None:         blend = BlendType.None;       break;
                default:                            blend = BlendType.Add;        break;
            }

            return blend;
        }


        private static EmitterBlendMode ToMercuryBlend(BlendType core)
        {
            EmitterBlendMode blend;

            switch (core)
            {
                case BlendType.Add:          blend = EmitterBlendMode.Add;       break;
                case BlendType.Alpha:        blend = EmitterBlendMode.Alpha;     break;
                case BlendType.None:         blend = EmitterBlendMode.None;      break;
                default:                     blend = EmitterBlendMode.Add;       break;
            }

            return blend;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Renderer.RenderEffect(this.Model, ref Scene.Camera.Transform);
        }


        public string AssetType
        {
            get { return @"Particles"; }
        }


        public IAsset Load(string nom, string path, Dictionary<string, string> parameters, ContentManager contentManager)
        {
            Particle p = new Particle()
            {
                Model = contentManager.Load<ParticleEffect>(path)
            };

            // Load images (that must be in WindowsContent for now)
            p.Model.LoadContent(contentManager);

            return p;
        }


        public void Unload()
        {

        }


        public object Clone()
        {
            return this;
        }
    }
}
