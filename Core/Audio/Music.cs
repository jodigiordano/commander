namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using EphemereGames.Core.Persistence;
    
    class Music : AbstractMusical, IAsset
    {
        public Music()
            : base()
        {
            Volume = SoundEffectsController.Instance.VolumeMusique;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Instances.Count > 1)
                throw new Exception("Ne devrait pas arriver.");

            if (Fade == null && Instances.Count != 0 && !Instances[0].IsDisposed)
                Etat = Instances[0].State;
        }

        public override void jouer(bool apparitionProgressive, int tempsFade, bool loop)
        {
            if (Instances.Count != 0 && Etat == SoundState.Paused)
            {
                reprendre(apparitionProgressive, tempsFade);
                return;
            }

            else if (Instances.Count != 0 && Etat == SoundState.Stopped)
                Instances.Clear();
            else if (Instances.Count != 0 && Etat == SoundState.Playing)
                return;

            base.jouer(apparitionProgressive, tempsFade, loop);
        }

        public string AssetType
        {
            get { return "Musique"; }
        }

        public object Load(string nom, string chemin, Dictionary<string, string> parametres, Microsoft.Xna.Framework.Content.ContentManager contenu)
        {
            Microsoft.Xna.Framework.Audio.SoundEffect son = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(chemin);

            Music musique = new Music();
            musique.Son = son;

            SoundEffectsController.Instance.setMusique(nom, musique);

            return musique;
        }

        public object Clone()
        {
            return this;
        }
    }
}
