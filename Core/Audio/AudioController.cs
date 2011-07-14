namespace EphemereGames.Core.Audio
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class AudioController
    {
        public string ActiveBank { get; set; }

        private Dictionary<string, SoundEffectsBank> SoundEffects = new Dictionary<string, SoundEffectsBank>();
        private Dictionary<string, Music> Musics = new Dictionary<string, Music>();

        public int MaxSimultaneousSfx;
        private int MaxActivesSfxInstances;
        private Dictionary<string, int> MaxSfxInstances;


        public AudioController()
        {
            foreach (var kvp in Musics)
                kvp.Value.Stop(false, 0);


            foreach (var kvp in SoundEffects)
            {
                kvp.Value.Stop(false, 0);
                kvp.Value.Update(null);
            }


            SoundEffects.Clear();
            MaxActivesSfxInstances = 0;
            MaxSimultaneousSfx = int.MaxValue;
            MaxSfxInstances = new Dictionary<string, int>();

            MusicVolume = 0.5f;
            SfxVolume = 0.5f;
            ActiveBank = @"Default";
        }


        private float musicVolume;
        public float MusicVolume
        {
            get { return musicVolume; }

            set
            {
                musicVolume = value;

                foreach (var kvp in Musics)
                    kvp.Value.Volume = value;
            }
        }


        private float sfxVolume;
        public float SfxVolume
        {
            get { return sfxVolume; }

            set
            {
                sfxVolume = value;

                foreach (var kvp in SoundEffects)
                    kvp.Value.Volume = value;
            }
        }


        public Music GetMusic(string musicName)
        {
            return Musics[musicName];
        }


        public void PlaySfx(string sfxName)
        {
            PlaySfx(ActiveBank, sfxName, false);
        }


        public int PlaySfx(string sfxName, bool loop)
        {
            return PlaySfx(ActiveBank, sfxName, loop);
        }


        public int PlaySfx(string bankName, string sfxName, bool loop)
        {
            int maxInstancesActives = 0;

            if (MaxActivesSfxInstances >= MaxSimultaneousSfx ||
                (MaxSfxInstances.TryGetValue(sfxName, out maxInstancesActives) && SoundEffects[bankName].SoundEffects[sfxName].InstancesActives >= maxInstancesActives))
                return -1;

            return SoundEffects[bankName].Play(sfxName);
        }


        public void StopSfx(string sfxName)
        {
            StopSfx(ActiveBank, sfxName);
        }


        public void StopSfx(string sfxName, int id)
        {

        }


        public void StopSfx(string bankName, string sfxName)
        {
            SoundEffects[bankName].Stop(sfxName, true, 100);
        }

        public SoundEffectsBank GetSfxBank(string bankName)
        {
            return SoundEffects[bankName];
        }


        public void SetSfxMaxInstances(string sfxName, int maxInstances)
        {

        }


        public void SetMusic(string musicName, Music music)
        {
            Musics[musicName] = music;

            music.Sound.Name = musicName;
        }


        public void ReplaceMusic(string oldMusic, string newMusic)
        {
            Musics[oldMusic].Stop(false, 0);

            Musics[newMusic].Sound.Name = oldMusic;

            Microsoft.Xna.Framework.Audio.SoundEffect se = Musics[oldMusic].Sound;

            Musics[oldMusic] = new Music();
            Musics[oldMusic].Sound = se;
        }


        public void SetSfx(string bankName, SoundEffect sfx)
        {
            if (!SoundEffects.ContainsKey(bankName))
                SoundEffects[bankName] = new SoundEffectsBank();

            SoundEffects[bankName].Add(sfx.Sound.Name, sfx);
        }


        public void Update(GameTime gameTime)
        {
            foreach (var m in Musics.Values)
                m.Update(gameTime);

            MaxActivesSfxInstances = 0;

            foreach (var sfx in SoundEffects)
            {
                sfx.Value.Update(gameTime);
                MaxActivesSfxInstances += SoundEffects[sfx.Key].ActiveInstances;
            }
        }


        public void SetMaxActivesSfxInstances(string sfxName, int maxInstances)
        {
            if (!MaxSfxInstances.ContainsKey(sfxName))
                MaxSfxInstances.Add(sfxName, maxInstances);
            else
                MaxSfxInstances[sfxName] = maxInstances;
        }
    }
}
