#if !WINDOWS_PHONE

namespace EphemereGames.Core.XACTAudio
{
    using System;


    public class Cue
    {
        internal Microsoft.Xna.Framework.Audio.Cue InnerCue;
        private bool MasterIsPaused;
        private string Name;


        internal Cue(string name, Microsoft.Xna.Framework.Audio.Cue innerCue)
        {
            InnerCue = innerCue;
            Name = name;
            InnerCue.Disposing += new System.EventHandler<System.EventArgs>(DoDispose);
            MasterIsPaused = innerCue.IsPaused;
        }


        public bool IsReady
        {
            get { return !InnerCue.IsDisposed && !InnerCue.IsStopping && !InnerCue.IsStopped; }
        }


        public void PlayOrResume()
        {
            if (InnerCue.IsPaused)
                Resume();
            else
                Play();
        }


        public void Play()
        {
            if (InnerCue.IsPlaying)
                return;

            InnerCue.Play();
        }


        public void Pause()
        {
            if (InnerCue.IsPaused || !InnerCue.IsPlaying)
                return;

            InnerCue.Pause();
        }


        public void Resume()
        {
            if (!InnerCue.IsPaused)
                return;

            InnerCue.Resume();
        }


        public void SetVariable(string variable, float value)
        {
            InnerCue.SetVariable(variable, value);
        }


        public void Stop()
        {
            if (InnerCue.IsDisposed)
                return;

            InnerCue.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.AsAuthored);
        }


        public void StopNow()
        {
            if (InnerCue.IsDisposed)
                return;

            InnerCue.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
        }


        public void MasterPause()
        {
            if (InnerCue.IsStopped)
                return;

            MasterIsPaused = InnerCue.IsPaused;

            Pause();
        }


        public void MasterResume()
        {
            if (InnerCue.IsStopped)
                return;

            if (MasterIsPaused)
                return;

            Resume();
        }


        public void Update()
        {
            if (InnerCue.IsDisposed)
                return;

            if (InnerCue.IsStopped)
                InnerCue.Dispose();
        }


        private void DoDispose(object sender, EventArgs args)
        {
            InnerCue.Disposing -= this.DoDispose;
        }
    }
}

#endif