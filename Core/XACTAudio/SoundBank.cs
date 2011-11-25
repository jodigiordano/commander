#if !WINDOWS_PHONE

namespace EphemereGames.Core.XACTAudio
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework.Content;
    

    class SoundBank : IAsset
    {
        public string Name { get; private set; }
        public Microsoft.Xna.Framework.Audio.SoundBank InnerSoundBank;


        public SoundBank()
            : base()
        {
            Name = "";
            InnerSoundBank = null;
        }


        public string AssetType
        {
            get { return "SoundBank"; }
        }


        public IAsset Load(string name, string path, Dictionary<string, string> parameters, ContentManager manager)
        {
            var engine = XACTAudio.AudioController.Engine;

            var soundBank = new SoundBank()
            {
                Name = name,
                InnerSoundBank = new Microsoft.Xna.Framework.Audio.SoundBank(engine, manager.RootDirectory + @"\" + path)
            };

            XACTAudio.AudioController.AddSoundBank(soundBank);

            return soundBank;
        }


        public void Unload()
        {
            InnerSoundBank.Dispose();
            XACTAudio.AudioController.RemoveSoundBank(this);
        }


        public object Clone()
        {
            return this;
        }
    }
}

#endif