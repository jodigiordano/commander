#if !WINDOWS_PHONE

namespace EphemereGames.Core.XACTAudio
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework.Content;
    

    class WaveBank : IAsset
    {
        public string Name { get; private set; }
        public Microsoft.Xna.Framework.Audio.WaveBank InnerWaveBank;


        public WaveBank()
            : base()
        {
            Name = "";
            InnerWaveBank = null;
        }


        public string AssetType
        {
            get { return "WaveBank"; }
        }


        public IAsset Load(string name, string path, Dictionary<string, string> parameters, ContentManager manager)
        {
            var engine = XACTAudio.AudioController.Engine;

            var waveBank = new WaveBank()
            {
                Name = name,
                InnerWaveBank = new Microsoft.Xna.Framework.Audio.WaveBank(engine, manager.RootDirectory + @"\" + path)
            };

            XACTAudio.AudioController.AddWaveBank(waveBank);

            return waveBank;
        }


        public void Unload()
        {
            InnerWaveBank.Dispose();
            XACTAudio.AudioController.RemoveWaveBank(this);
        }


        public object Clone()
        {
            return this;
        }
    }
}

#endif