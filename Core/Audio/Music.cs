namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    class Music : AbstractMusical, IAsset
    {
        public Music()
            : base()
        {
            Volume = Audio.SoundEffectsController.MusicVolume;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Instances.Count > 1)
                throw new Exception("Ne devrait pas arriver.");

            if (Fade == null && Instances.Count != 0 && !Instances[0].IsDisposed)
                State = Instances[0].State;
        }


        public override void jouer(bool apparitionProgressive, int tempsFade, bool loop)
        {
            if (Instances.Count != 0 && State == SoundState.Paused)
            {
                Unpause(apparitionProgressive, tempsFade);
                return;
            }

            else if (Instances.Count != 0 && State == SoundState.Stopped)
                Instances.Clear();
            else if (Instances.Count != 0 && State == SoundState.Playing)
                return;

            base.jouer(apparitionProgressive, tempsFade, loop);
        }


        public string AssetType
        {
            get { return "Musique"; }
        }


        public object Load(string musicName, string chemin, Dictionary<string, string> parametres, Microsoft.Xna.Framework.Content.ContentManager contenu)
        {
            Microsoft.Xna.Framework.Audio.SoundEffect son = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(chemin);

            Music music = new Music();
            music.Sound = son;

            Audio.SoundEffectsController.SetMusic(musicName, music);

            return music;
        }


        public object Clone()
        {
            return this;
        }
    }
}
