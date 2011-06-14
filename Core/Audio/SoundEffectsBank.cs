namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    class SoundEffectsBank
    {
        public Dictionary<string, SoundEffect> EffetsSonores { get; set; }

        public int InstancesActives;

        public SoundEffectsBank()
        {
            EffetsSonores = new Dictionary<string, SoundEffect>();
            InstancesActives = 0;
        }

        public bool Utilise
        {
            get
            {
                foreach (var kvp in EffetsSonores)
                    if (kvp.Value.Etat == SoundState.Playing)
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
            InstancesActives = 0;

            foreach (var kvp in EffetsSonores)
            {
                kvp.Value.Update(gameTime);
                InstancesActives += kvp.Value.InstancesActives;
            }
        }

        public void reprendre(bool apparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.reprendre(apparitionProgressive, tempsFade);
        }

        public void pauser(bool disparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.pauser(disparitionProgressive, tempsFade);
        }

        public void arreter(bool disparitionProgressive, int tempsFade)
        {
            foreach (var kvp in EffetsSonores)
                kvp.Value.arreter(disparitionProgressive, tempsFade);
        }

        public void arreter(string nomEffetSonore, bool disparitionProgressive, int tempsFade)
        {
            if (EffetsSonores.ContainsKey(nomEffetSonore))
                EffetsSonores[nomEffetSonore].arreter(disparitionProgressive, tempsFade);
        }

        public void jouer(string nomEffetSonore)
        {
#if DEBUG
                if (!EffetsSonores.ContainsKey(nomEffetSonore))
                        throw new Exception("L'effet sonore ne se trouve pas dans cette banque");
#endif

            EffetsSonores[nomEffetSonore].jouer(false, 0, false);
        }

        public void ajouter(string nomEffetSonore, SoundEffect effetSonore)
        {
            EffetsSonores[nomEffetSonore] = effetSonore;
        }
    }
}
