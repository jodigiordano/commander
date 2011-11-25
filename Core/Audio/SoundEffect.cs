namespace EphemereGames.Core.Audio
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    

    class SoundEffect : AbstractMusical, IAsset
    {
        public int InstancesActives;

        private List<int> ToDelete;


        public SoundEffect()
            : base()
        {
            Volume = Audio.AudioController.SfxVolume;
            InstancesActives = 0;
            ToDelete = new List<int>();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InstancesActives = 0;

            ToDelete.Clear();

            foreach (var sfx in Instances)
            {
                if (sfx.Value.State == SoundState.Stopped)
                {
                    sfx.Value.Dispose();
                    ToDelete.Add(sfx.Key);
                }

                else
                    InstancesActives++;
            }

            foreach (var id in ToDelete)
                Instances.Remove(id);
        }


        public string AssetType
        {
            get { return @"Sfx"; }
        }


        public IAsset Load(string name, string path, Dictionary<string, string> parameters, ContentManager contenu)
        {
            string bank = parameters[@"bank"];

            Microsoft.Xna.Framework.Audio.SoundEffect sound = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(path);
            sound.Name = name;

            SoundEffect soundEffect = new SoundEffect();
            soundEffect.Sound = sound;

            Audio.AudioController.SetSfx(bank, soundEffect);

            return soundEffect;
        }


        public void Unload()
        {

        }


        public object Clone()
        {
            return this;
        }
    }
}
