#if !WINDOWS_PHONE

namespace EphemereGames.Core.XACTAudio
{
    using EphemereGames.Core.SimplePersistence;


    public static class XACTAudio
    {
        internal static AudioController AudioController;


        public static void Initialize(string configPath)
        {
            Persistence.AddAssetType(new WaveBank());
            Persistence.AddAssetType(new SoundBank());

            AudioController = new AudioController(configPath);
        }


        public static void Update()
        {
            AudioController.Update();
        }


        public static void PlayCue(string cueName, string bankName)
        {
            AudioController.PlayCue(cueName, bankName);
        }


        public static Cue GetCue(string cueName, string bankName)
        {
            return AudioController.GetCue(cueName, bankName);
        }


        public static void SetGlobalVariable(string variable, float value)
        {
            AudioController.SetGlobalVariable(variable, value);
        }


        public static float GetGlobalVariable(string variable)
        {
            return AudioController.GetGlobalVariable(variable);
        }


        public static void ChangeCategoryVolume(string category, int volume)
        {
            AudioController.ChangeCategoryVolume(category, volume);
        }


        //public static bool Active
        //{
        //    get { return AudioController.Active; }
        //}


        //public static void Pause()
        //{
        //    AudioController.Pause();
        //}


        //public static void Resume()
        //{
        //    AudioController.Resume();
        //}
    }
}

#endif