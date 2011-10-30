namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.XACTAudio;


    class XACTMusicController
    {
        private Dictionary<string, Cue> Musics;
        private string CurrentMusic;
        private Dictionary<Cue, MusicFade> Fades;


        public XACTMusicController()
        {
            Musics = new Dictionary<string, Cue>();
            Fades = new Dictionary<Cue, MusicFade>();
            CurrentMusic = null;
        }


        public void Initialize()
        {
            Musics.Add("MainMenuMusic", null);
            Musics.Add("WinMusic", null);
            Musics.Add("Raindrop", null);
            Musics.Add("LoseMusic", null);
            Musics.Add("BattleMusic", null);
            Musics.Add("CinematicIntro", null);
            Musics.Add("IntroNiveau", null);
        }


        public void Update()
        {
            foreach (var cue in Musics.Values)
            {
                if (cue == null || !cue.IsReady)
                    continue;

                MusicFade fade = null;

                if (!Fades.TryGetValue(cue, out fade))
                    continue;

                fade.Update();

                if (fade.Terminated)
                    Fades.Remove(cue);
            }
        }


        public void Play(string musicName)
        {
            Play(musicName, "Sound Bank");
        }


        public void PlayOrResume(string musicName)
        {
            PlayOrResume(musicName, "Sound Bank");
        }


        public void Play(string musicName, string bankName)
        {
            PauseCurrentMusic();

            Cue music = Musics[musicName];

            if (music != null)
                music.StopNow();

            music = XACTAudio.GetCue(musicName, bankName);
            Musics[musicName] = music;
            music.PlayOrResume();
            music.SetVariable("MusicFade", 1);
            CurrentMusic = musicName;
        }


        public void PlayOrResume(string musicName, string bankName)
        {
            PauseCurrentMusic();

            Cue music = Musics[musicName];

            if (music == null)
            {
                music = XACTAudio.GetCue(musicName, bankName);
                Musics[musicName] = music;
                music.PlayOrResume();
                music.SetVariable("MusicFade", 1);
                CurrentMusic = musicName;
                return;
            }

            music.PlayOrResume();
            FadeIn(musicName, true, 1);
            CurrentMusic = musicName;
        }


        public void StopCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            Stop(CurrentMusic);
        }


        public void Stop(string musicName)
        {
            if (!Musics.ContainsKey(musicName) || Musics[musicName] == null)
                return;

            Musics[musicName].Stop();
            Musics[musicName] = null;

            if (musicName == CurrentMusic)
                CurrentMusic = null;
        }


        public void ToggleMusic(string musicName)
        {

        }


        public void ToggleCurrentMusic()
        {
            if (CurrentMusic == null)
                return;
        }


        public void PauseCurrentMusicNow()
        {
            if (CurrentMusic == null)
                return;

            Musics[CurrentMusic].Pause();
        }


        public void PauseCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            FadeOut(CurrentMusic, true, 0);
        }


        public void ResumeCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            FadeIn(CurrentMusic, true, 1);
        }


        public void AddMusic(string musicName)
        {
            if (Musics.ContainsKey(musicName))
                return;

            Musics.Add(musicName, null);
        }


        public void FadeIn(string musicName, bool pauseOrResumeAfter, float to)
        {
            if (!Musics.ContainsKey(musicName) || Musics[musicName] == null)
                return;

            var music = Musics[musicName];

            if (Fades.ContainsKey(music))
                Fades.Remove(music);

            Fades.Add(music, new MusicFade(music, pauseOrResumeAfter, to));
        }


        public void FadeOut(string musicName, bool pauseOrResumeAfter, float to)
        {
            if (!Musics.ContainsKey(musicName) || Musics[musicName] == null || !Musics[musicName].IsReady)
                return;

            var music = Musics[musicName];

            if (Fades.ContainsKey(music))
                Fades.Remove(music);

            Fades.Add(music, new MusicFade(music, pauseOrResumeAfter, to));
        }


        public void FadeInCurrentMusic(bool pauseOrResumeAfter, float to)
        {
            if (CurrentMusic == null)
                return;

            FadeIn(CurrentMusic, pauseOrResumeAfter, to);
        }


        public void FadeOutCurrentMusic(bool pauseOrResumeAfter, float to)
        {
            if (CurrentMusic == null)
                return;

            FadeOut(CurrentMusic, pauseOrResumeAfter, to);
        }
    }


    class MusicFade
    {
        private Cue Cue;
        private PathEffect Effect;
        private bool PauseOrResumeAfter;
        private float To;


        public MusicFade(Cue cue, bool pauseOrResumeAfter, float to)
        {
            Cue = cue;
            Effect = new PathEffect(Path.CreateCurve(CurveType.Linear, Cue.GetVariable("MusicFade"), to, 500), 500, Preferences.TargetElapsedTimeMs);
            PauseOrResumeAfter = pauseOrResumeAfter;
            To = to;
        }


        public bool Terminated
        {
            get { return Effect.Terminated; }
        }


        public void Update()
        {
            if (Terminated)
                return;

            Effect.Update();
            Cue.SetVariable("MusicFade", Effect.Value);

            if (Terminated && PauseOrResumeAfter)
            {
                if (Effect.Value <= 0)
                    Cue.Pause();
                else
                    Cue.Resume();
            }
        }
    }
}
