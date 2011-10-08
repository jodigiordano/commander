namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.XACTAudio;


    class XACTMusicController
    {
        private Dictionary<string, Cue> Musics;
        private string CurrentMusic;


        public XACTMusicController()
        {
            Musics = new Dictionary<string, Cue>();
            CurrentMusic = null;
        }


        public void Initialize()
        {
            Musics.Add("EphemereGamesLogo", null);
            Musics.Add("MainMenuMusic", null);
            Musics.Add("WinMusic", null);
            Musics.Add("Raindrop", null);
            Musics.Add("LoseMusic", null);
            Musics.Add("BattleMusic", null);
            Musics.Add("CinematicIntro", null);
            Musics.Add("IntroNiveau", null);
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
            if (CurrentMusic != null)
                Musics[CurrentMusic].Pause();

            Cue music = Musics[musicName];

            if (music != null)
                music.StopNow();

            music = XACTAudio.GetCue(musicName, bankName);
            Musics[musicName] = music;

            music.PlayOrResume();

            CurrentMusic = musicName;
        }


        public void PlayOrResume(string musicName, string bankName)
        {
            if (CurrentMusic != null)
                Musics[CurrentMusic].Pause();

            Cue music = Musics[musicName];

            if (music == null)
            {
                music = XACTAudio.GetCue(musicName, bankName);
                Musics[musicName] = music;
            }

            music.PlayOrResume();

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

            Musics[CurrentMusic].Pause();
        }


        public void ResumeCurrentMusic()
        {
            if (CurrentMusic == null)
                return;

            Musics[CurrentMusic].Resume();
        }


        public void AddMusic(string musicName)
        {
            if (Musics.ContainsKey(musicName))
                return;

            Musics.Add(musicName, null);
        }
    }
}
