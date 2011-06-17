namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    class SoundEffect : AbstractMusical, IAsset
    {
        public int InstancesActives;

        public SoundEffect()
            : base()
        {
            Volume = Audio.SoundEffectsController.SfxVolume;
            InstancesActives = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InstancesActives = 0;

            for (int i = Instances.Count - 1; i > -1; i--)
            {
                SoundEffectInstance instance = Instances[i];

                if (instance.State == SoundState.Stopped)
                {
                    instance.Dispose();
                    Instances.RemoveAt(i);
                }

                else
                {
                    InstancesActives++;
                }
            }
        }

        public string AssetType
        {
            get { return "EffetSonore"; }
        }

        public object Load(string nom, string chemin, Dictionary<string, string> parametres, Microsoft.Xna.Framework.Content.ContentManager contenu)
        {
            string banque = parametres["banque"];

            Microsoft.Xna.Framework.Audio.SoundEffect son = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(chemin);
            son.Name = nom;

            SoundEffect effetSonore = new SoundEffect();
            effetSonore.Sound = son;

            Audio.SoundEffectsController.SetSfx(banque, effetSonore);

            return effetSonore;
        }

        public object Clone()
        {
            return this;
        }
    }
}
