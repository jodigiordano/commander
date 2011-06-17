namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    class SoundEffectsBank
    {
        public Dictionary<string, SoundEffect> EffetsSonores { get; set; }

        public int ActiveInstances;

        public SoundEffectsBank()
        {
            EffetsSonores = new Dictionary<string, SoundEffect>();
            ActiveInstances = 0;
        }

        public bool InUse
        {
            get
            {
                foreach (var kvp in EffetsSonores)
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
                foreach (var kvp in EffetsSonores)
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

            foreach (var kvp in EffetsSonores)
            {
                kvp.Value.Update(gameTime);
                ActiveInstances += kvp.Value.InstancesActives;
            }
        }

        public void reprendre(bool apparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.Unpause(apparitionProgressive, tempsFade);
        }

        public void Pause(bool disparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.Pause(disparitionProgressive, tempsFade);
        }

        public void Stop(bool disparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.Stop(disparitionProgressive, tempsFade);
        }

        public void Stop(string nomEffetSonore, bool disparitionProgressive, int tempsFade)
        {
            if (EffetsSonores.ContainsKey(nomEffetSonore))
                EffetsSonores[nomEffetSonore].Stop(disparitionProgressive, tempsFade);
        }

        public void Play(string nomEffetSonore)
        {
#if DEBUG
                if (!EffetsSonores.ContainsKey(nomEffetSonore))
                        throw new Exception("L'effet sonore ne se trouve pas dans cette banque");
#endif

            EffetsSonores[nomEffetSonore].jouer(false, 0, false);
        }

        public void Add(string nomEffetSonore, SoundEffect effetSonore)
        {
            EffetsSonores[nomEffetSonore] = effetSonore;
        }
    }
}
