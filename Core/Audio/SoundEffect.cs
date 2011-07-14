namespace EphemereGames.Core.Audio
{
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    

    class SoundEffect : AbstractMusical, IAsset
    {
        public int InstancesActives;

        //private Dictionary<int, NoneHandler> InstancesCallbacks;
        private List<int> ToDelete;


        public SoundEffect()
            : base()
        {
            Volume = Audio.AudioController.SfxVolume;
            InstancesActives = 0;
            //InstancesCallbacks = new Dictionary<int, NoneHandler>();
            ToDelete = new List<int>();
        }


        //public void Play(bool progressive, int fadeTime, NoneHandler callback)
        //{
        //    Play(progressive, fadeTime, false);
        //    
        //}
        


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


        public object Load(string name, string path, Dictionary<string, string> parameters, ContentManager contenu)
        {
            string banque = parameters[@"bank"];

            Microsoft.Xna.Framework.Audio.SoundEffect son = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(path);
            son.Name = name;

            SoundEffect effetSonore = new SoundEffect();
            effetSonore.Sound = son;

            Audio.AudioController.SetSfx(banque, effetSonore);

            return effetSonore;
        }


        public object Clone()
        {
            return this;
        }
    }
}
