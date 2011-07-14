namespace EphemereGames.Core.Audio
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;


    abstract class AbstractMusical
    {
        public Microsoft.Xna.Framework.Audio.SoundEffect Sound  { get; set; }
        public SoundState State                                 { get; protected set; }
        public SoundState NextState                             { protected get; set; }

        protected Dictionary<int, SoundEffectInstance> Instances { get; set; }
        protected Fade Fade                                     { get; set; }
        
        private float volume;

        public int Id;
        private static int NextId = 0;
        

        public AbstractMusical()
        {
            Sound = null;
            Fade = null;
            Instances = new Dictionary<int, SoundEffectInstance>();
            State = SoundState.Stopped;
            NextState = SoundState.Stopped;
        }


        public virtual float Volume
        {
            set
            {
                foreach (var instance in Instances.Values)
                    instance.Volume = value;

                volume = value;
            }

            private get
            {
                if (Instances.Count == 0)
                    return volume;
                
                if (Instances.Count == 1)
                    return Instances[Id].Volume;

                float averageVolume = 0;

                foreach (var instance in Instances.Values)
                    averageVolume += instance.Volume;

                averageVolume /= Instances.Count;

                return averageVolume;
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            if (Fade == null)
                return;

            if (!Fade.Finished)
                foreach (var instance in Instances.Values)
                    instance.Volume = Fade.Next(gameTime) / 100.0f;
            else
            {
                if (Fade.TypeFade == Fade.Type.OUT)
                {
                    foreach (var instance in Instances.Values)
                    {
                        if (NextState == SoundState.Stopped)
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


        public virtual void Resume(bool progressive, int fadeTime)
        {
            if (Instances.Count == 0 || State != SoundState.Paused)
                return;

            if (progressive)
            {
                Fade = new Fade((int) (Volume * 100), Fade.Type.IN, fadeTime, 0);
                Fade.Max = (int)(volume * 100);

                foreach (var instance in Instances.Values)
                    instance.Volume = 0;
            }

            foreach (var instance in Instances.Values)
                instance.Resume();

            State = SoundState.Playing;
            NextState = SoundState.Playing;
        }


        public virtual void Pause(bool progressive, int fadeTime)
        {
            if (Instances.Count == 0 || State != SoundState.Playing)
                return;

            if (progressive)
            {
                Fade = new Fade((int)(Volume * 100), Fade.Type.OUT, 0, fadeTime);

                State = SoundState.Playing;
                NextState = SoundState.Paused;
            }

            else
            {
                foreach (var instance in Instances.Values)
                    instance.Pause();

                State = SoundState.Paused;
                NextState = SoundState.Paused;
            }
        }


        public virtual void Stop(bool progressive, int fadeTime)
        {
            if (Instances.Count == 0 || State == SoundState.Stopped)
                return;

            if (progressive)
            {
                Fade = new Fade((int)(Volume * 100), Fade.Type.OUT, 0, fadeTime);
                NextState = SoundState.Stopped;
            }
            else
            {
                foreach (var instance in Instances.Values)
                    instance.Stop();

                State = SoundState.Stopped;
                NextState = SoundState.Stopped;
            }
        }


        public virtual int Play(bool progressive, int fadeTime, bool loop)
        {
            SoundEffectInstance instance = Sound.CreateInstance();

            instance.IsLooped = loop;

            if (progressive)
            {
                instance.Volume = 0;
                Fade = new Fade(0, Fade.Type.IN, fadeTime, 0);
                Fade.Max = (int)(volume * 100);
            }

            else
                instance.Volume = volume;

            instance.Play();

            Id = NextId++;

            Instances.Add(Id, instance);

            State = SoundState.Playing;
            NextState = SoundState.Playing;

            return Id;
        }
    }
}
