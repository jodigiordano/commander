namespace EphemereGames.Core.XACTAudio
{
    using EphemereGames.Core.Persistence;


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
    }
}
