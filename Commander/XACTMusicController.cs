namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.XACTAudio;


    class XACTMusicController
    {
        private Dictionary<string, Cue> Musics;
        private string CurrentMusic;
        private Dictionary<Cue, PathEffect> Fades;


        public XACTMusicController()
        {
            Musics = new Dictionary<string, Cue>();
            Fades = new Dictionary<Cue, PathEffect>();
            CurrentMusic = null;
        }


        public void Initialize()
        {
            //Musics.Add("EphemereGamesLogo", null);
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

                PathEffect fade = null;

                if (!Fades.TryGetValue(cue, out fade))
                    continue;

                fade.Update();
                cue.SetVariable("MusicFade", fade.Value);

                if (fade.Terminated)
                {
                    if (fade.Value <= 0)
                        cue.Pause();
                    else
                        cue.Resume();

                    Fades.Remove(cue);
                }
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
            FadeIn(musicName);

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
            }

            music.PlayOrResume();
            FadeIn(musicName);

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


        public void PauseCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            FadeOut(CurrentMusic);
        }


        public void ResumeCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            FadeIn(CurrentMusic);
        }


        public void AddMusic(string musicName)
        {
            if (Musics.ContainsKey(musicName))
                return;

            Musics.Add(musicName, null);
        }


        private void FadeIn(string musicName)
        {
            Fades.Add(Musics[musicName], new PathEffect(Path.CreateCurve(CurveType.Linear, 500), 500, Preferences.TargetElapsedTimeMs));
        }


        private void FadeOut(string musicName)
        {
            Fades.Add(Musics[musicName], new PathEffect(Path.CreateCurve(CurveType.InversedLinear, 500), 500, Preferences.TargetElapsedTimeMs));
        }
    }
}
