namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using EphemereGames.Core.Persistance;
    
    class EffetSonore : Musical, IContenu
    {
        public int InstancesActives;

        public EffetSonore()
            : base()
        {
            Volume = GestionnaireSons.Instance.VolumeEffetsSonores;
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

        public string TypeAsset
        {
            get { return "EffetSonore"; }
        }

        public object charger(string nom, string chemin, Dictionary<string, string> parametres, Microsoft.Xna.Framework.Content.ContentManager contenu)
        {
            String banque = parametres["banque"];

            SoundEffect son = contenu.Load<SoundEffect>(chemin);
            son.Name = nom;

            EffetSonore effetSonore = new EffetSonore();
            effetSonore.Son = son;

            GestionnaireSons.Instance.setEffetSonore(banque, effetSonore);

            return effetSonore;
        }

        public object Clone()
        {
            return this;
        }
    }
}
