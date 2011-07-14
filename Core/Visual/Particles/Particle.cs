namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
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
        public ParticleEffect ParticleEffect;
        public SpriteBatchRenderer Renderer;
        internal State State;

        private TypeBlend blend;

        
        public Particle()
        {
            Name = "";
            Position = Vector3.Zero;
            Blend = TypeBlend.Add;
            VisualPriority = 0;
            State = State.Idle;
            Id = Visuals.NextHashCode;
        }


        public void Initialize()
        {
            ProjectMercury.ParticleEffect model = EphemereGames.Core.Persistence.Persistence.GetAsset<ProjectMercury.ParticleEffect>(Name);

            ParticleEffect = model.DeepCopy();

            ParticleEffect.Initialise();
        }


        public TypeBlend Blend
        {
            get { return blend; }
            set
            {
                blend = value;

                if (ParticleEffect == null)
                    return;

                foreach (var effect in ParticleEffect)
                    effect.BlendMode = ToMercuryBlend(value);
            }
        }


        public void Trigger(ref Vector2 position)
        {
            this.ParticleEffect.Trigger(ref position);
        }


        public void Trigger(ref Vector3 position)
        {
            Vector2 position2d = new Vector2(position.X, position.Y);

            this.ParticleEffect.Trigger(ref position2d);
        }


        public void Move(ref Vector2 varPosition)
        {
            for (int i = 0; i < ParticleEffect.Count; i++)
                for (int j = 0; j < ParticleEffect[i].ActiveParticlesCount; j++)
                    Vector2.Add(ref ParticleEffect[i].Particles[j].Position, ref varPosition, out ParticleEffect[i].Particles[j].Position);
        }


        public void Move(ref Vector3 varPosition)
        {
            Vector2 position2d = new Vector2(varPosition.X, varPosition.Y);

            Move(ref position2d);
        }


        private static TypeBlend ToCoreBlend(EmitterBlendMode mercury)
        {
            TypeBlend blend;

            switch (mercury)
            {
                case EmitterBlendMode.Add:          blend = TypeBlend.Add;        break;
                case EmitterBlendMode.Alpha:        blend = TypeBlend.Alpha;      break;
                case EmitterBlendMode.None:         blend = TypeBlend.None;       break;
                default:                            blend = TypeBlend.Add;        break;
            }

            return blend;
        }


        private static EmitterBlendMode ToMercuryBlend(TypeBlend core)
        {
            EmitterBlendMode blend;

            switch (core)
            {
                case TypeBlend.Add:          blend = EmitterBlendMode.Add;       break;
                case TypeBlend.Alpha:        blend = EmitterBlendMode.Alpha;     break;
                case TypeBlend.None:         blend = EmitterBlendMode.None;      break;
                default:                     blend = EmitterBlendMode.Add;       break;
            }

            return blend;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Renderer.RenderEffect(this.ParticleEffect, ref Scene.Camera.Transform);
        }


        public string AssetType
        {
            get { return @"Particles"; }
        }


        public object Load(string nom, string chemin, Dictionary<string, string> parametres, ContentManager contenu)
        {
            // charge la particule à partir du fichier XML (une seule fois)
            ParticleEffect particle = contenu.Load<ParticleEffect>(chemin);

            // charge les images qui doivent (pour l'instant) se trouver dans WindowsContent
            particle.LoadContent(contenu);

            return particle;
        }


        public object Clone()
        {
            return this;
        }
    }
}
