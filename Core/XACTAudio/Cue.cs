namespace EphemereGames.Core.XACTAudio
{
    public class Cue
    {
        internal Microsoft.Xna.Framework.Audio.Cue InnerCue;


        internal Cue(Microsoft.Xna.Framework.Audio.Cue innerCue)
        {
            InnerCue = innerCue;
        }


        public void PlayOrResume()
        {
            if (InnerCue.IsPlaying)
                return;

            if (InnerCue.IsPaused)
                Resume();
            else
                Play();
        }


        public void Play()
        {
            InnerCue.Play();
        }


        public void Pause()
        {
            InnerCue.Pause();
        }


        public void Resume()
        {
            InnerCue.Resume();
        }


        public void SetVariable(string variable, float value)
        {
            InnerCue.SetVariable(variable, value);
        }


        public void Stop()
        {
            InnerCue.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.AsAuthored);
        }
    }
}
