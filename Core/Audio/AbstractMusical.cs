namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using EphemereGames.Core.Utilities;

    using System.Linq;

    abstract class AbstractMusical
    {
        public Microsoft.Xna.Framework.Audio.SoundEffect Sound { get; set; }
        protected List<SoundEffectInstance> Instances { get; set; }
        protected Fade Fade { get; set; }
        public SoundState State { get; protected set; }
        public SoundState ProchainEtat { protected get; set; }

        public AbstractMusical()
        {
            Sound = null;
            Fade = null;
            Instances = new List<SoundEffectInstance>();
            State = SoundState.Stopped;
            ProchainEtat = SoundState.Stopped;
        }

        private float volume;
        public virtual float Volume
        {
            set
            {
                foreach (var instance in Instances)
                    instance.Volume = value;

                volume = value;
            }

            private get
            {
                return volume;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Fade == null)
                return;

            if (!Fade.Termine)
                foreach (var instance in Instances)
                    instance.Volume = Fade.suivant(gameTime) / 100.0f;
            else
            {
                if (Fade.TypeFade == Fade.Type.OUT)
                {
                    foreach (var instance in Instances)
                    {
                        if (ProchainEtat == SoundState.Stopped)
                        {
                            State = SoundState.Stopped;
                            instance.Stop();
                        }
                        else
                        {
                            State = SoundState.Paused;
                            instance.Pause();
                        }
                    }
                }

                Fade = null;
            }
        }

        public virtual void Unpause(bool apparitionProgressive, int tempsFade)
        {
            if (Instances.Count == 0 || State != SoundState.Paused)
                return;

            if (apparitionProgressive)
            {
                Fade = new Fade(0, Fade.Type.IN, tempsFade, 0);
                Fade.Max = (int)(Volume * 100);

                foreach (var instance in Instances)
                    instance.Volume = 0;
            }

            foreach (var instance in Instances)
                instance.Resume();

            State = SoundState.Playing;
            ProchainEtat = SoundState.Playing;
        }

        public virtual void Pause(bool disparitionProgressive, int tempsFade)
        {
            if (Instances.Count == 0 || State != SoundState.Playing)
                return;

            if (disparitionProgressive)
            {
                Fade = new Fade((int)(Volume * 100), Fade.Type.OUT, 0, tempsFade);

                State = SoundState.Playing;
                ProchainEtat = SoundState.Paused;
            }

            else
            {
                foreach (var instance in Instances)
                    instance.Pause();

                State = SoundState.Paused;
                ProchainEtat = SoundState.Paused;
            }
        }

        public virtual void Stop(bool disparitionProgressive, int tempsFade)
        {
            if (Instances.Count == 0 || State == SoundState.Stopped)
                return;

            if (disparitionProgressive)
            {
                Fade = new Fade((int)(Volume * 100), Fade.Type.OUT, 0, tempsFade);
                //Etat = ...; en pause ou joue
                ProchainEtat = SoundState.Stopped;
            }
            else
            {
                foreach (var instance in Instances)
                    instance.Stop();

                State = SoundState.Stopped;
                ProchainEtat = SoundState.Stopped;
            }
        }

        public virtual void jouer(bool apparitionProgressive, int tempsFade, bool loop)
        {
            SoundEffectInstance instance = Sound.CreateInstance();

            instance.IsLooped = loop;

            if (apparitionProgressive)
            {
                instance.Volume = 0;
                Fade = new Fade(0, Fade.Type.IN, tempsFade, 0);
                Fade.Max = (int)(Volume * 100);
            }

            else
                instance.Volume = Volume;

            instance.Play();

            Instances.Add(instance);

            State = SoundState.Playing;
            ProchainEtat = SoundState.Playing;
        }
    }
}
