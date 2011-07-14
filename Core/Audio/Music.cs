namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    

    class Music : AbstractMusical, IAsset
    {
        private double CurrentPlayTime;
        private NoneHandler Callback;
        private double TimeToCallCallback;


        public Music()
            : base()
        {
            Volume = Audio.AudioController.MusicVolume;
            
            CurrentPlayTime = 0;
            Callback = null;
            TimeToCallCallback = 0;
        }


        public void Play(bool progressive, int fadeTime, NoneHandler callback, double timeBeforeEnd)
        {
            Play(progressive, fadeTime, false);
            Callback = callback;
            TimeToCallCallback = Sound.Duration.TotalMilliseconds - timeBeforeEnd;
        }


        public override int Play(bool progressive, int fadeTime, bool loop)
        {
            if (Instances.Count != 0)
            {
                if (State == SoundState.Paused)
                {
                    Resume(progressive, fadeTime);
                    return Id;
                }

                if (State == SoundState.Playing)
                {
                    return Id;
                }

                if (State == SoundState.Stopped)
                {
                    Instances.Clear();
                }
            }

            return base.Play(progressive, fadeTime, loop);
        }


        public override void Stop(bool progressive, int fadeTime)
        {
            Callback = null;

            base.Stop(progressive, fadeTime);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Instances.Count > 1)
                throw new Exception("Two same musics are loaded in memory.");

            if (Fade == null && Instances.Count != 0 && !Instances[Id].IsDisposed)
                State = Instances[Id].State;

            TimeToCallCallback -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Callback != null && TimeToCallCallback <= 0)
            {
                Callback();
                Callback = null;
            }
        }


        public string AssetType
        {
            get { return @"Music"; }
        }


        public object Load(string musicName, string path, Dictionary<string, string> parameters, ContentManager contenu)
        {
            Microsoft.Xna.Framework.Audio.SoundEffect sound = contenu.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(path);

            Music music = new Music();
            music.Sound = sound;

            Audio.AudioController.SetMusic(musicName, music);

            return music;
        }


        public object Clone()
        {
            return this;
        }
    }
}
