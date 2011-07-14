namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    class SoundEffectsBank
    {
        public Dictionary<string, SoundEffect> SoundEffects { get; set; }

        public int ActiveInstances;

        public SoundEffectsBank()
        {
            SoundEffects = new Dictionary<string, SoundEffect>();
            ActiveInstances = 0;
        }

        public bool InUse
        {
            get
            {
                foreach (var kvp in SoundEffects)
                    if (kvp.Value.State == SoundState.Playing)
                        return true;

                return false;
            }
        }

        private float volume;
        public float Volume
        {
            set
            {
                foreach (var kvp in SoundEffects)
                    kvp.Value.Volume = value;

                volume = value;
            }

            private get
            {
                return volume;
            }
        }

        public void Update(GameTime gameTime)
        {
            ActiveInstances = 0;

            foreach (var kvp in SoundEffects)
            {
                kvp.Value.Update(gameTime);
                ActiveInstances += kvp.Value.InstancesActives;
            }
        }

        public void reprendre(bool progressive, int fadeTime)
        {
            foreach (var kvp in SoundEffects)
                kvp.Value.Resume(progressive, fadeTime);
        }

        public void Pause(bool progressive, int fadeTime)
        {
            foreach (var kvp in SoundEffects)
                kvp.Value.Pause(progressive, fadeTime);
        }

        public void Stop(bool progressive, int fadeTime)
        {
            foreach (var kvp in SoundEffects)
                kvp.Value.Stop(progressive, fadeTime);
        }

        public void Stop(string sfxName, bool progressive, int fadeTime)
        {
            if (SoundEffects.ContainsKey(sfxName))
                SoundEffects[sfxName].Stop(progressive, fadeTime);
        }

        public int Play(string sfxName)
        {
#if DEBUG
            if (!SoundEffects.ContainsKey(sfxName))
                throw new Exception("This bank does not have this sfx");
#endif

            return SoundEffects[sfxName].Play(false, 0, false);
        }

        public void Add(string sfxName, SoundEffect sfx)
        {
            SoundEffects[sfxName] = sfx;
        }
    }
}
