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
            Musics.Add("Galaxy1Music", null);
            Musics.Add("WinMusic", null);
            Musics.Add("Raindrop", null);
            Musics.Add("LoseMusic", null);
            Musics.Add("BattleMusic", null);
            Musics.Add("CinematicIntro", null);
            Musics.Add("IntroNiveau", null);
        }


        public void Play(string musicName)
        {
            if (CurrentMusic != null)
                Musics[CurrentMusic].Pause();

            Cue music = Musics[musicName];

            if (music != null)
                music.StopNow();

            music = XACTAudio.GetCue(musicName, "Sound Bank");
            Musics[musicName] = music;

            music.PlayOrResume();

            CurrentMusic = musicName;
        }


        public void PlayOrResume(string musicName)
        {
            if (CurrentMusic != null)
                Musics[CurrentMusic].Pause();

            Cue music = Musics[musicName];

            if (music == null)
            {
                music = XACTAudio.GetCue(musicName, "Sound Bank");
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
    }
}
