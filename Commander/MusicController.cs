namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;


    class MusicController
    {
        public bool SwitchMusicRandomly;
        public int FadeTime = 1000;

        private MusicContext Context;
        
        private string SelectedMusic;
        private double TimeBetweenTwoMusicChange = 0;

        private const int SwitchMusicRandomlyFadeTime = 10000;

        private bool CanChangeMusic;


        public MusicController()
        {
            SelectedMusic = AvailableMusics[Main.Random.Next(0, AvailableMusics.Count)];
            AvailableMusics.Remove(SelectedMusic);
            Context = MusicContext.Other;
            SwitchMusicRandomly = false;
            CanChangeMusic = true;
        }


        public void Update()
        {
            TimeBetweenTwoMusicChange -= Preferences.TargetElapsedTimeMs;
        }


        public void ChangeMusic(bool overrideMinimumTime)
        {
            if (!CanChangeMusic)
                return;

            if (!overrideMinimumTime && TimeBetweenTwoMusicChange > 0)
                return;

            string newMusic = AvailableMusics[Main.Random.Next(0, AvailableMusics.Count)];

            StopMusic(false);

            SelectedMusic = newMusic;
            AvailableMusics.Remove(SelectedMusic);

            PlayMusic(true);
            
            TimeBetweenTwoMusicChange = Preferences.TimeBetweenTwoMusics;
        }


        public void PlayMusic(bool switching)
        {
            PlayMusic(switching, true, true);
        }


        public void PlayMusic(bool switching, bool fade, bool loop)
        {
            if (!Audio.IsMusicPlaying(SelectedMusic))
            {
                if (SwitchMusicRandomly)
                    Audio.PlayMusic(SelectedMusic, fade, switching ? SwitchMusicRandomlyFadeTime : FadeTime, MusicFinished, SwitchMusicRandomlyFadeTime);
                else
                    Audio.PlayMusic(SelectedMusic, fade, FadeTime, loop);
            }

            else
            {
                Audio.ResumeMusic(SelectedMusic, fade, FadeTime);
            }
        }


        public void ResumeMusic()
        {
            if (!Audio.IsMusicPlaying(SelectedMusic))
                Audio.ResumeMusic(SelectedMusic, true, FadeTime);
        }


        public void PauseMusic()
        {
            Audio.PauseMusic(SelectedMusic, true, FadeTime);
        }


        public void PauseMusicNow()
        {
            Audio.PauseMusic(SelectedMusic, false, 0);
        }
        

        public void StopMusic(bool now)
        {
            var fadeTime = now ? 0 : SwitchMusicRandomly ? SwitchMusicRandomlyFadeTime / 5 : FadeTime;

            Audio.StopMusic(SelectedMusic, !now, fadeTime);

            switch (Context)
            {
                case MusicContext.Won: AvailableGameWonMusics.Add(SelectedMusic); break;
                case MusicContext.Lost: AvailableGameLostMusics.Add(SelectedMusic); break;
                case MusicContext.Other: AvailableMusics.Add(SelectedMusic); break;
            }
        }


        public void SwitchTo(MusicContext context)
        {
            SwitchTo(context, null, true);
        }


        public void SwitchTo(MusicContext context, string musicName, bool playNow)
        {
            Audio.StopMusic(SelectedMusic, true, 500);

            string lastMusic = SelectedMusic;

            // current
            switch (context)
            {
                case MusicContext.Won:
                    SelectedMusic = musicName == null ? AvailableGameWonMusics[Main.Random.Next(0, AvailableGameWonMusics.Count)] : musicName;
                    AvailableGameWonMusics.Remove(SelectedMusic);
                    CanChangeMusic = true;
                    break;
                case MusicContext.Lost:
                    SelectedMusic = musicName == null ? AvailableGameLostMusics[Main.Random.Next(0, AvailableGameLostMusics.Count)] : musicName;
                    AvailableGameLostMusics.Remove(SelectedMusic);
                    CanChangeMusic = true;
                    break;
                case MusicContext.Other:
                    SelectedMusic = musicName == null ? AvailableMusics[Main.Random.Next(0, AvailableMusics.Count)] : musicName;
                    AvailableMusics.Remove(SelectedMusic);
                    CanChangeMusic = true;
                    break;
                case MusicContext.Cutscene:
                    SelectedMusic = musicName == null ? AvailableCutsceneMusics[Main.Random.Next(0, AvailableCutsceneMusics.Count)] : musicName;
                    AvailableCutsceneMusics.Remove(SelectedMusic);
                    CanChangeMusic = false;
                    break;
            }


            // previous
            switch (Context)
            {
                case MusicContext.Won: AvailableGameWonMusics.Add(lastMusic); break;
                case MusicContext.Lost: AvailableGameLostMusics.Add(lastMusic); break;
                case MusicContext.Other: AvailableMusics.Add(lastMusic); break;
                case MusicContext.Cutscene: AvailableCutsceneMusics.Add(lastMusic); break;
            }

            Context = context;

            if (playNow)
                Audio.PlayMusic(SelectedMusic, true, FadeTime, true);
        }


        private void MusicFinished()
        {
            ChangeMusic(true);
        }


        //=====================================================================
        // Shared stuff
        //=====================================================================


        private static List<string> AvailableMusics = new List<string>()
        {
            "ingame1",
            "ingame2",
            "ingame3",
            "ingame4",
            "ingame5",
            "ingame6",
            "ingame7"
        };


        private static List<string> AvailableGameWonMusics = new List<string>()
        {
            "win1",
            "win2"
        };


        private static List<string> AvailableGameLostMusics = new List<string>()
        {
            "gameover1",
            "gameover2"
        };


        private static List<string> AvailableCutsceneMusics = new List<string>()
        {
            "introMusic"
        };


        public static void InitializeSfxPriorities()
        {
            Audio.SetMaxInstancesSfx(@"sfxTourelleBase", 2);
            Audio.SetMaxInstancesSfx(@"sfxTourelleMissile", 1);
            Audio.SetMaxInstancesSfx(@"sfxTourelleLaserMultiple", 1);
            Audio.SetMaxInstancesSfx(@"sfxTourelleMissileExplosion", 2);
            Audio.SetMaxInstancesSfx(@"sfxTourelleLaserSimple", 3);
            Audio.SetMaxInstancesSfx(@"sfxTourelleSlowMotion", 2);
            Audio.SetMaxInstancesSfx(@"sfxCorpsCelesteTouche", 2);
            Audio.SetMaxInstancesSfx(@"sfxCorpsCelesteExplose", 1);
            Audio.SetMaxInstancesSfx(@"sfxNouvelleVague", 2);
            Audio.SetMaxInstancesSfx(@"sfxPowerUpResistanceTire1", 1);
            Audio.SetMaxInstancesSfx(@"sfxPowerUpResistanceTire2", 1);
            Audio.SetMaxInstancesSfx(@"sfxPowerUpResistanceTire3", 1);
            Audio.SetMaxInstancesSfx(@"sfxTourelleVendue", 1);
            Audio.SetMaxInstancesSfx(@"sfxMoney1", 1);
            Audio.SetMaxInstancesSfx(@"sfxMoney2", 1);
            Audio.SetMaxInstancesSfx(@"sfxMoney3", 1);
            Audio.SetMaxInstancesSfx(@"sfxLifePack", 2);
            Audio.SetMaxInstancesSfx(@"sfxMineGround", 1);
        }


        public static void SetActiveBank(string bank)
        {
            Audio.SetActiveSfxBank(bank);
        }
    }
}
