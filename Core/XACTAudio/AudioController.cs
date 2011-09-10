namespace EphemereGames.Core.XACTAudio
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Audio;


    class AudioController
    {
        public AudioEngine Engine { get; private set; }

        private Dictionary<string, WaveBank> WaveBanks;
        private Dictionary<string, SoundBank> SoundBanks;


        public AudioController(string fileName)
        {
            Engine = new AudioEngine(fileName);
            WaveBanks = new Dictionary<string, WaveBank>();
            SoundBanks = new Dictionary<string,SoundBank>();
        }


        public void AddWaveBank(WaveBank bank)
        {
            WaveBanks.Add(bank.Name, bank);
        }


        public void RemoveWaveBank(WaveBank bank)
        {
            WaveBanks.Remove(bank.Name);
        }


        public void AddSoundBank(SoundBank bank)
        {
            SoundBanks.Add(bank.Name, bank);
        }


        public void RemoveSoundBank(SoundBank bank)
        {
            SoundBanks.Remove(bank.Name);
        }


        public void Update()
        {
            Engine.Update();
        }


        public void PlayCue(string cueName, string bankName)
        {
            SoundBanks[bankName].InnerSoundBank.PlayCue(cueName);
        }
    }
}
