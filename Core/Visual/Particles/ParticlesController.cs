namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using ParallelTasks;


    public class ParticlesController
    {
        public List<Particle> Particles { get; private set; }

        private Dictionary<string, Utilities.Pool<Particle>> ParticlesPools;
        private ProjectMercury.Renderers.SpriteBatchRenderer Renderer;
        private Scene Scene;


        public ParticlesController(Scene scene)
        {
            ParticlesPools = new Dictionary<string, Utilities.Pool<Particle>>();
            Particles = new List<Particle>();


            Renderer = new ProjectMercury.Renderers.SpriteBatchRenderer
            {
                GraphicsDeviceService = Preferences.GraphicsDeviceManager
            };

            Renderer.LoadContent(Preferences.Content);

            this.Scene = scene;
        }


        public void Add(List<string> names, bool hard)
        {
            if (hard)
                Clear();

            foreach (var name in names)
                Add(name);
        }


        public void Add(string name)
        {
            if (!ParticlesPools.ContainsKey(name))
                ParticlesPools.Add(name, new Utilities.Pool<Particle>());
        }


        public void SetMaxInstances(string name, int maxInstances)
        {
            ParticlesPools[name].MaxInstances = maxInstances;
        }


        public Particle Get(string name)
        {
            Particle particle = ParticlesPools[name].Get();

            if (particle == null)
                return null;

            particle.State = State.Active;
            particle.Scene = Scene;
            particle.Name = name;
            particle.Renderer = this.Renderer;
            particle.Initialize();

            Particles.Add(particle);

            return particle;
        }


        public void Return(Particle particle)
        {
            if (particle.ParticleEffect.ActiveParticlesCount != 0)
                particle.State = State.Dying;
            else
                particle.State = State.Dead;
        }


        public void Clear()
        {
            foreach (var p in Particles)
            {
                //todo: clear the particles
                p.State = State.Idle;
                ParticlesPools[p.Name].Return(p);
            }

            Particles.Clear();
        }


        public void Update(GameTime gameTime)
        {
            Parallel.For(0, (int) Math.Ceiling(Particles.Count / 50.0), i =>
            {
                AsyncUpdate(i * 50, Math.Min(i * 50 + 50, Particles.Count), (float) gameTime.ElapsedGameTime.TotalSeconds);
            });

            RemoveDeadParticles();
        }


        private void RemoveDeadParticles()
        {
            for (int i = 0; i < Particles.Count; i++)
                if (Particles[i].State == State.Dead)
                {
                    Particles[i].State = State.Idle;
                    ParticlesPools[Particles[i].Name].Return(Particles[i]);
                    
                    Particles.RemoveAt(i);
                }
        }


        private void AsyncUpdate(int i, int j, float elapsedSeconds)
        {
            for (int k = i; k < j; k++)
            {
                Particles[k].ParticleEffect.Update(elapsedSeconds);

                if (Particles[k].State == State.Dying && Particles[k].ParticleEffect.ActiveParticlesCount == 0)
                    Particles[k].State = State.Dead;
            }
        }
    }
}
