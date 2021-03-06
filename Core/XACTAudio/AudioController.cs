﻿namespace EphemereGames.Core.XACTAudio
{
    using System.Collections.Generic;


    class AudioController
    {
        public Microsoft.Xna.Framework.Audio.AudioEngine Engine { get; private set; }

        private Dictionary<string, WaveBank> WaveBanks;
        private Dictionary<string, SoundBank> SoundBanks;

        private List<Cue> Cues;


        public AudioController(string fileName)
        {
            Engine = new Microsoft.Xna.Framework.Audio.AudioEngine(fileName);
            WaveBanks = new Dictionary<string, WaveBank>();
            SoundBanks = new Dictionary<string,SoundBank>();
            Cues = new List<Cue>();
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

            for (int i = 0; i < Cues.Count; i++)
            {
                var cue = Cues[i];

                cue.Update();

                if (cue.InnerCue.IsDisposed)
                    Cues.RemoveAt(i);
            }
        }


        public void PlayCue(string cueName, string bankName)
        {
            SoundBanks[bankName].InnerSoundBank.PlayCue(cueName);
        }


        public Cue GetCue(string cueName, string bankName)
        {
            var cue = new Cue(cueName, SoundBanks[bankName].InnerSoundBank.GetCue(cueName));

            Cues.Add(cue);

            return cue;
        }


        public void SetGlobalVariable(string variable, float value)
        {
            Engine.SetGlobalVariable(variable, value);
        }


        public float GetGlobalVariable(string variable)
        {
            return Engine.GetGlobalVariable(variable);
        }
    }
}
