namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;


    class MusicController
    {
        public bool SwitchMusicRandomly;
        
        private string SelectedMusic;
        private MusicContext Context;
        private double TimeBetweenTwoMusicChange = 0;

        private const int SwitchMusicRandomlyFadeTime = 10000;
        private const int FadeTime = 1000;



        public MusicController()
        {
            SelectedMusic = AvailableMusics[Main.Random.Next(0, AvailableMusics.Count)];
            AvailableMusics.Remove(SelectedMusic);
            Context = MusicContext.Other;
            SwitchMusicRandomly = false;
        }


        public void Update()
        {
            TimeBetweenTwoMusicChange -= Preferences.TargetElapsedTimeMs;
        }


        public void ChangeMusic(bool overrideMinimumTime)
        {
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
            if (!Audio.IsMusicPlaying(SelectedMusic))
            {
                if (SwitchMusicRandomly)
                    Audio.PlayMusic(SelectedMusic, true, switching ? SwitchMusicRandomlyFadeTime : FadeTime, MusicFinished, SwitchMusicRandomlyFadeTime);
                else
                    Audio.PlayMusic(SelectedMusic, true, FadeTime, true);
            }

            else
            {
                Audio.ResumeMusic(SelectedMusic, true, FadeTime);
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
            Audio.StopMusic(SelectedMusic, true, 500);

            string lastMusic = SelectedMusic;

            // current
            switch (context)
            {
                case MusicContext.Won:
                    SelectedMusic = AvailableGameWonMusics[Main.Random.Next(0, AvailableGameWonMusics.Count)];
                    AvailableGameWonMusics.Remove(SelectedMusic);
                    break;
                case MusicContext.Lost:
                    SelectedMusic = AvailableGameLostMusics[Main.Random.Next(0, AvailableGameLostMusics.Count)];
                    AvailableGameLostMusics.Remove(SelectedMusic);
                    break;
                case MusicContext.Other:
                    SelectedMusic = AvailableMusics[Main.Random.Next(0, AvailableMusics.Count)];
                    AvailableMusics.Remove(SelectedMusic);
                    break;
            }


            // previous
            switch (Context)
            {
                case MusicContext.Won: AvailableGameWonMusics.Add(lastMusic); break;
                case MusicContext.Lost: AvailableGameLostMusics.Add(lastMusic); break;
                case MusicContext.Other: AvailableMusics.Add(lastMusic); break;
            }

            Context = context;

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
